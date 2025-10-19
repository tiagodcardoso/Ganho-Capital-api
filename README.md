# API Ganho de Capital - Itau Unibanco

## Descricao
API REST para calculo de imposto sobre ganho de capital de operacoes de compra e venda de acoes.

## Arquitetura

### Estrutura do Projeto
```
GanhoDeCapital/
├── GanhoDeCapital/           # Camada de Apresentacao (API, UI)
├── GanhoDeCapital.Core/      # Camada de Aplicacao (Casos de Uso, Logica da Aplicacao)
├── GanhoDeCapital.Domain/    # Camada de Dominio (Entidades, Regras de Negocio)
├── GanhoDeCapital.Infra/     # Camada de Infraestrutura (Banco de Dados, Servicos Externos)
└── GanhoDeCapital.Tests/     # Camada de Testes (Unitarios, Integracao, etc.)
```

### Principios Aplicados
- **Clean Architecture**: Separacao de responsabilidades em camadas
- **SOLID**: Principios de design orientado a objetos
- **DDD**: Domain-Driven Design para modelagem do dominio
- **Dependency Injection**: Inversao de controle
- **Repository Pattern**: Abstracao de acesso a dados

## Tecnologias

- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core (In-Memory)
- xUnit para testes
- FluentValidation para validacoes

### Pre-requisitos
- .NET 8.0 SDK ou superior

### Passos
```bash
# Clone o repositório
git clone 

# Navegue até o diretório
cd GanhoDeCapital

# Restaure as dependências
dotnet restore

# Execute os testes
dotnet test

# Execute a aplicação
cd GanhoDeCapital.Api
dotnet run
```

A API estara disponivel em: `https://localhost:7079` ou `http://localhost:5200`

## Endpoint

### POST /process-taxes

Processa lotes de operacoes e calcula o imposto devido.

#### Request Body
```json
[
  {
    "operation-id": 1,
    "client-id": "41a23999-37f0-49d9-b9fb-5ac823fa4cb3",
    "operations": [
      { "operation": "buy", "unit-cost": 10.00, "quantity": 10000 },
      { "operation": "sell", "unit-cost": 17.00, "quantity": 10000 }
    ]
  }
]
```

#### Response
```json
[
  {
    "operation-id": 1,
    "client-id": "41a23999-37f0-49d9-b9fb-5ac823fa4cb3",
    "client-name": "JOAO SILVA",
    "client-cpf": "111.111.111-11",
    "tax": 14000.00,
    "status": "Calculado com sucesso"
  }
]
```

## Regras de Negocio

1. **Imposto**: 20% sobre o lucro em operacoes de venda
2. **Preco Medio Ponderado**: Calculado a cada nova compra
3. **Deducao de Prejuizo**: Prejuizos deduzem lucros futuros
4. **Isencao**: Vendas ≤ R$ 20.000,00 sao isentas
5. **Sem Imposto em Compras**: Apenas vendas geram imposto

## Tratamento de Erros

### Erro de Enriquecimento
- `client-id` nulo
- `client-id` nao encontrado
- Status: "Erro no enriquecimento"

### Erro de Calculo
- Apenas operacoes de compra
- Lista de operacoes vazia
- Status: "Erro no calculo"

## Testes

Execute os testes unitarios:
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Cobertura de Testes
- Calculo de lucro simples
- Vendas isentas
- Prejuizo abatendo lucro
- Preco medio ponderado
- Casos de erro

## Exemplos de Uso

### Caso 1: Lucro Simples
Compra de 10.000 acoes a R$ 10,00 e venda a R$ 17,00
- Lucro: R$ 70.000,00
- Imposto: R$ 14.000,00 (20%)

### Caso 2: Venda Isenta
Venda total de R$ 15.000,00 (abaixo de R$ 20.000,00)
- Imposto: R$ 0,00

### Caso 3: Prejuizo Dedutivel
Prejuizo de R$ 25.000,00 deduz lucro de R$ 30.000,00
- Lucro liquido: R$ 5.000,00
- Imposto: R$ 1.000,00

## Design Patterns Utilizados

- **Repository Pattern**: Abstracao de acesso a dados
- **Service Layer**: Logica de negocio isolada
- **Factory Pattern**: Criacao de objetos complexos
- **Strategy Pattern**: Diferentes estrategias de calculo
- **Dependency Injection**: Inversao de controle

## Documentacao Adicional

- [ARCHITECTURE.md] - Proposta de arquitetura para MVP2

## Autor

Desenvolvido por Tiago Cardoso como parte do Desafio Tecnico

## Licenca

Este projeto e parte de um processo seletivo.