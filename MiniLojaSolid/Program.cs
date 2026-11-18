using System.Globalization;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
// Ponto de composição: escolhe as implementações fora da regra de negócio.
var avisos = new NotificadorMemoria();
var checkout = new Checkout(new DescontoVip(), avisos);
var total = checkout.Finalizar(new Carrinho(200m));
Console.WriteLine($"Total: {total:C}");
Console.WriteLine(avisos.Mensagens[0]);

if (args.Contains("--test"))
{
    foreach (IPoliticaDesconto politica in new IPoliticaDesconto[]
             { new DescontoVip(), new DescontoBlackFriday() })
    {
        foreach (decimal entrada in new[] { 0m, 0.01m, 200m, 999m })
        {
            decimal saida = politica.Aplicar(entrada);
            Verificar(saida >= 0 && saida <= entrada, "Contrato do desconto");
        }
    }
    var fake = new NotificadorMemoria();
    var sut = new Checkout(new DescontoBlackFriday(), fake);
    Verificar(sut.Finalizar(new Carrinho(200m)) == 150m, "Black Friday: 150");
    Verificar(fake.Mensagens.Count == 1, "Uma notificação no sucesso");
    try
    {
        sut.Finalizar(new Carrinho(-1m));
        throw new Exception("Deveria rejeitar carrinho inválido");
    }
    catch (ArgumentOutOfRangeException) { }
    Verificar(fake.Mensagens.Count == 1, "Pedido inválido não notifica");
    Console.WriteLine("Verificações passaram.");
}

static void Verificar(bool condicao, string cenario)
{
    if (!condicao) throw new Exception($"Falhou: {cenario}");
}

public record Carrinho(decimal Total);

public interface IPoliticaDesconto
{
    // Entrada >= 0. Retorna valor entre 0 e a entrada.
    decimal Aplicar(decimal total);
}

public class DescontoVip : IPoliticaDesconto
{
    public decimal Aplicar(decimal total) => total * 0.90m;
}

public class DescontoBlackFriday : IPoliticaDesconto
{
    public decimal Aplicar(decimal total) => total * 0.75m;
}

public interface INotificador
{
    // Registra ou encaminha o aviso; não garante entrega ao destinatário.
    // Falhas do canal podem ser propagadas ao chamador.
    void Enviar(string mensagem);
}

public class NotificadorMemoria : INotificador
{
    public List<string> Mensagens { get; } = new();
    public void Enviar(string mensagem) => Mensagens.Add(mensagem);
}

public class Checkout
{
    private readonly IPoliticaDesconto _desconto;
    private readonly INotificador _notificador;

    public Checkout(IPoliticaDesconto desconto, INotificador notificador)
    {
        _desconto = desconto;
        _notificador = notificador;
    }

    public decimal Finalizar(Carrinho carrinho)
    {
        if (carrinho.Total <= 0)
            throw new ArgumentOutOfRangeException(nameof(carrinho));

        decimal total = _desconto.Aplicar(carrinho.Total);
        _notificador.Enviar("Pedido concluído");
        return total;
    }
}
