using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
var repositorio = new PedidosEmMemoria();
var pedido = new Pedido(Guid.NewGuid());
pedido.AdicionarItem("LIVRO-DDD", 2, new Dinheiro(100m, "BRL"));
repositorio.Salvar(pedido);
new ConfirmarPedido(repositorio).Executar(pedido.Id);
Console.WriteLine($"Pedido {pedido.Id}: {pedido.Status} | {pedido.Total.Valor:C}");
foreach (var evento in pedido.Eventos)
    Console.WriteLine($"Evento: {evento.GetType().Name}");

if (args.Contains("--test"))
{
    void Verificar(bool condicao, string caso)
    {
        if (!condicao) throw new Exception($"Falhou: {caso}");
    }
    void Rejeitar(Action acao, string caso)
    {
        try { acao(); }
        catch (ArgumentException) { return; }
        catch (InvalidOperationException) { return; }
        throw new Exception($"Deveria rejeitar: {caso}");
    }
    Verificar(new Dinheiro(10, "BRL") == new Dinheiro(10, "BRL"), "Igualdade por valor");
    Rejeitar(() => new Dinheiro(-1, "BRL"), "Preço negativo");
    Rejeitar(() => new Dinheiro(10, "USD"), "Moeda não suportada");
    var vazio = new Pedido(Guid.NewGuid());
    Rejeitar(vazio.Confirmar, "Pedido vazio");
    Verificar(vazio.Status == StatusPedido.Rascunho && vazio.Eventos.Count == 0, "Falha não altera estado");
    Rejeitar(() => vazio.AdicionarItem("SKU", 0, new Dinheiro(10, "BRL")), "Quantidade zero");
    Rejeitar(() => vazio.AdicionarItem("", 1, new Dinheiro(10, "BRL")), "SKU vazio");
    Verificar(pedido.Total.Valor == 200m, "Total calculado");
    Rejeitar(() => pedido.AdicionarItem("SKU", 1, new Dinheiro(1, "BRL")), "Alteração após confirmação");
    Rejeitar(pedido.Confirmar, "Confirmação duplicada");
    Verificar(pedido.Eventos.Count == 1 && pedido.Itens.Count == 1, "Uma confirmação e itens protegidos");
    var evento = pedido.Eventos.Single();
    Verificar(evento.PedidoId == pedido.Id && evento.Total == pedido.Total, "Evento corresponde ao pedido");
    Rejeitar(() => new ConfirmarPedido(repositorio).Executar(Guid.NewGuid()), "Pedido inexistente");
    Console.WriteLine("Verificações de domínio passaram.");
}

// Objeto de valor: imutável e comparado pelos valores. A loja usa apenas BRL.
public sealed record Dinheiro
{
    public decimal Valor { get; }
    public string Moeda { get; }
    public Dinheiro(decimal valor, string moeda)
    {
        if (valor < 0) throw new ArgumentOutOfRangeException(nameof(valor));
        if (moeda != "BRL") throw new ArgumentException("A loja opera em BRL.", nameof(moeda));
        Valor = valor;
        Moeda = moeda;
    }
}

// Neste modelo, a linha é um valor imutável; não possui ciclo de vida próprio.
public sealed record ItemPedido(string Sku, int Quantidade, Dinheiro PrecoUnitario);
public enum StatusPedido { Rascunho, Confirmado }
public sealed record PedidoConfirmado(Guid PedidoId, Dinheiro Total);

// Entidade e raiz do agregado: toda mudança passa por ela.
public sealed class Pedido
{
    private readonly List<ItemPedido> itens = new();
    private readonly List<PedidoConfirmado> eventos = new();
    public Guid Id { get; }
    public StatusPedido Status { get; private set; } = StatusPedido.Rascunho;
    public IReadOnlyList<ItemPedido> Itens => itens.AsReadOnly();
    public IReadOnlyList<PedidoConfirmado> Eventos => eventos.AsReadOnly();
    public Dinheiro Total => new(itens.Sum(i => i.PrecoUnitario.Valor * i.Quantidade), "BRL");
    public Pedido(Guid id)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id obrigatório.", nameof(id));
        Id = id;
    }
    public void AdicionarItem(string sku, int quantidade, Dinheiro preco)
    {
        ExigirRascunho();
        if (string.IsNullOrWhiteSpace(sku)) throw new ArgumentException("SKU obrigatório.", nameof(sku));
        if (quantidade <= 0) throw new ArgumentOutOfRangeException(nameof(quantidade));
        ArgumentNullException.ThrowIfNull(preco);
        itens.Add(new ItemPedido(sku, quantidade, preco));
    }
    public void Confirmar()
    {
        ExigirRascunho();
        if (itens.Count == 0) throw new InvalidOperationException("Pedido vazio não pode ser confirmado.");
        var total = Total;
        Status = StatusPedido.Confirmado;
        eventos.Add(new PedidoConfirmado(Id, total));
    }
    private void ExigirRascunho()
    {
        if (Status != StatusPedido.Rascunho)
            throw new InvalidOperationException("Pedido confirmado não pode ser alterado.");
    }
}

public interface IPedidos
{
    Pedido? Obter(Guid id);
    void Salvar(Pedido pedido);
}

// Aplicação: coordena; a decisão de confirmar pertence ao agregado.
public sealed class ConfirmarPedido(IPedidos pedidos)
{
    public void Executar(Guid id)
    {
        var pedido = pedidos.Obter(id) ?? throw new InvalidOperationException("Pedido não encontrado.");
        pedido.Confirmar();
        pedidos.Salvar(pedido);
    }
}

// Infraestrutura didática: referências em memória, sem transações ou isolamento.
public sealed class PedidosEmMemoria : IPedidos
{
    private readonly Dictionary<Guid, Pedido> dados = new();
    public Pedido? Obter(Guid id) => dados.GetValueOrDefault(id);
    public void Salvar(Pedido pedido) => dados[pedido.Id] = pedido;
}
