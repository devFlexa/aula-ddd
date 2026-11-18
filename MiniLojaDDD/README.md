# Mini Loja DDD

Requer SDK .NET 8. Na pasta da aula:

```powershell
dotnet run --project MiniLojaDDD
dotnet run --project MiniLojaDDD -- --test
```

O pedido tem dois livros de R$ 100,00 e total de R$ 200,00. As verificações cobrem invariantes, igualdade por valor, confirmação e dados do evento.

O exemplo concentra os tipos em Program.cs para facilitar o estudo. Em um projeto maior, organize domínio, aplicação e infraestrutura por módulos e dependências. O repositório guarda referências em memória e os eventos ficam registrados no pedido: não há banco, publicação de mensagens, controle de concorrência nem garantia de entrega.

Missões e respostas comentadas estão em `../aula_ddd_interativa.html`.
