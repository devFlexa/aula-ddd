# Avaliação e revisão da aula SOLID

O material original tinha boa organização, exemplos antes/depois, navegação, modo escuro e quiz. As melhorias se concentraram em precisão conceitual, coerência entre explicação e prática e leitura dos diagramas.

## Melhorias realizadas

- Objetivos observáveis, pré-requisitos e distinção entre recortes didáticos e programa completo.
- SRP explicado por motivos de mudança e necessidades do negócio, com discussão sobre o custo de extrair uma validação simples.
- OCP delimitado ao eixo de variação escolhido; o ponto de composição continua podendo mudar.
- LSP explicado por entradas, garantias e comportamento observável. O exemplo inclui o consumidor de frete e destaca que permitir null exige migrar os clientes.
- ISP explicado também pelo ponto de vista do consumidor.
- DIP reforçado com contrato definido pela necessidade do núcleo e distinção entre injeção e inversão.
- Dez diagramas SVG incorporados em cinco pares antes/depois, com títulos acessíveis, descrições, legendas e cores compatíveis com os temas. Funcionam offline e não dependem de imagens externas.
- Simulador com políticas e notificadores substituíveis e indicação da estratégia selecionada.
- Projeto MiniLojaSolid executável, com apresentação fora do checkout, notificador em memória e verificações de comportamento.
- Referências primárias da Microsoft e de Liskov/Wing incluídas na aula.

## Validação

- `dotnet run --project MiniLojaSolid -- --test`: compilação e verificações passaram no SDK 8.0.422.
- JavaScript da aula analisado sintaticamente sem erros.
- IDs sem duplicação e âncoras locais válidas.
- Doze combinações de desconto e notificação verificadas: totais 200, 180, 160 e 150.
- Dez SVGs com título e descrição; dois em cada princípio.

Não foi realizada inspeção visual em navegador. Em telas estreitas, os pares de diagramas são empilhados para manter a legibilidade. O laboratório é uma simulação didática sem persistência ou envio real; políticas de arredondamento e tratamento de falhas de comunicação exigiriam decisões adicionais em produção.

## Ajuste para estudo dos alunos

Roteiro de quatro horas e indicações de duração removidos. Cada princípio apresenta dois diagramas pequenos, com cenário problemático e solução melhorada, lado a lado em telas largas e empilhados em telas estreitas.

