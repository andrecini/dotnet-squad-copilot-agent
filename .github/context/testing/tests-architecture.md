# Test Architecture

## Visão Geral

Os projetos de teste espelham a estrutura de camadas da solução. Cada camada possui seu próprio projeto de testes, organizado dentro de uma pasta `Tests/` na raiz da solution. Isso garante isolamento entre as suítes de teste e clareza sobre qual camada cada teste cobre.

---

## Estrutura

```
[componente]/
├── 0 - Presentation/
│   └── [componente].Api/
├── 1 - Application/
│   └── [componente].Application/
├── 2 - Domain/
│   └── [componente].Domain/
├── 3 - Infrastructure/
│   └── [componente].Infrastructure/
└── Tests/
    ├── 0 - Presentation/
    │   └── [componente].Api.Tests/
    │       ├── DataMocks/
    │       ├── Mocks/
    │       └── Tests/
    ├── 1 - Application/
    │   └── [componente].Application.Tests/
    │       ├── DataMocks/
    │       ├── Mocks/
    │       └── Tests/
    ├── 2 - Domain/
    │   └── [componente].Domain.Tests/
    │       ├── DataMocks/
    │       ├── Mocks/
    │       └── Tests/
    └── 3 - Infrastructure/
        └── [componente].Infrastructure.Tests/
            ├── DataMocks/
            ├── Mocks/
            └── Tests/
```

---

## Responsabilidade por Projeto de Testes

| Projeto | O que testa |
|---------|-------------|
| `[componente].Api.Tests` | Endpoints, validators de request, app services, filtros e middlewares |
| `[componente].Application.Tests` | Services de aplicação, middlewares de aplicação |
| `[componente].Domain.Tests` | Regras de domínio, entidades, value objects, result pattern |
| `[componente].Infrastructure.Tests` | Repositórios, clientes de integração, consumers e producers de mensageria |

---

## Estrutura Interna de cada Projeto

Cada projeto de testes segue a mesma organização interna:

```
[componente].X.Tests/
├── DataMocks/          — objetos de dados reutilizáveis por cenário de teste
│   ├── Requests/
│   ├── Responses/
│   ├── Models/
│   └── Messages/
├── Mocks/              — mock classes de dependências via Moq
│   ├── AppServices/
│   ├── Services/
│   ├── Repositories/
│   └── Integrations/
└── Tests/              — classes de teste organizadas por contexto
    ├── AppServices/
    ├── Services/
    ├── Validators/
    └── Repositories/
```

---

## Convenções

- Cada projeto de testes referencia apenas o projeto da camada correspondente — nunca projetos de outras camadas diretamente
- O nome do projeto de testes segue o padrão `[componente].[Escopo].Tests`
- A organização interna de `DataMocks/`, `Mocks/` e `Tests/` segue os padrões definidos em `data-mocks.md`, `mock-classes.md` e `unit-tests.md`
- Testes de integração entre camadas, quando necessários, são tratados em projetos separados e não pertencem a nenhum dos projetos acima