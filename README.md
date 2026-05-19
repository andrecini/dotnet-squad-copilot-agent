# Sticker Manager — Copa 2026

> API para gerenciamento de álbum de figurinhas da Copa do Mundo 2026. Permite que usuários montem suas coleções, registrem figurinhas repetidas e realizem trocas entre si.

[![CI — Build and Test](https://github.com/andrecini/dotnet-squad-copilot-agent/actions/workflows/ci.yml/badge.svg)](https://github.com/andrecini/dotnet-squad-copilot-agent/actions/workflows/ci.yml)
[![Deploy — Staging](https://github.com/andrecini/dotnet-squad-copilot-agent/actions/workflows/deploy-staging.yml/badge.svg)](https://github.com/andrecini/dotnet-squad-copilot-agent/actions/workflows/deploy-staging.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=andrecini_dotnet-squad-copilot-agent&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=andrecini_dotnet-squad-copilot-agent)

---

## Indice

- [Sobre o Projeto](#sobre-o-projeto)
- [Arquitetura](#arquitetura)
- [Pre-requisitos](#pre-requisitos)
- [Instalacao e Configuracao](#instalacao-e-configuracao)
- [Como Rodar](#como-rodar)
- [Testes](#testes)
- [Entidades do Dominio](#entidades-do-dominio)
- [Decisoes Arquiteturais](#decisoes-arquiteturais)
- [CI/CD](#cicd)
- [Agente SQUAD — `.claude/`](#agente-squad--claude)
- [Como Contribuir](#como-contribuir)
- [Changelog](#changelog)

---

## Sobre o Projeto

O **Sticker Manager** e uma API REST construida em .NET 8 para suportar a dinamica de album de figurinhas da Copa do Mundo 2026. O sistema permite que usuarios registrem suas colecoes, marquem figurinhas como repetidas e negociem trocas com outros usuarios.

**Stack principal:**

| Tecnologia | Uso |
|---|---|
| .NET 8 / C# 12 | Linguagem e runtime |
| ASP.NET Core Minimal APIs | Camada de apresentacao |
| Entity Framework Core | ORM para persistencia relacional |
| Dapper | Queries de leitura customizadas |
| PostgreSQL | Banco de dados relacional |
| FluentValidation | Validacao de entradas |
| AutoMapper | Mapeamento entre camadas |
| Serilog | Logging estruturado |
| xUnit + Shouldly + Moq | Testes unitarios e de integracao |

---

## Arquitetura

O projeto segue **Clean Architecture** com separacao estrita de responsabilidades entre quatro camadas. Dependencias sempre apontam de fora para dentro — Infrastructure e Presentation dependem de Application e Domain; Application depende de Domain; Domain nao depende de ninguem.

```
┌─────────────────────────────────────────────────────┐
│  0 - Presentation                                   │
│  Copilot.SquadAgent.StickerManager.Api              │
│  Minimal APIs · Validators · DTOs de request        │
│  Swagger · Middlewares · launchSettings             │
└───────────────────┬─────────────────────────────────┘
                    │ depende de
┌───────────────────▼─────────────────────────────────┐
│  1 - Application                                    │
│  Copilot.SquadAgent.StickerManager.Application      │
│  AppServices · UseCases · DTOs de resposta          │
│  AutoMapper Profiles · Interfaces de servico        │
└───────────────────┬─────────────────────────────────┘
                    │ depende de
┌───────────────────▼─────────────────────────────────┐
│  2 - Domain                                         │
│  Copilot.SquadAgent.StickerManager.Domain           │
│  Entities · Enums · Result Pattern                  │
│  Interfaces de repositorio · Queries SQL (constantes)│
└───────────────────┬─────────────────────────────────┘
                    │ implementa
┌───────────────────▼─────────────────────────────────┐
│  3 - Infrastructure                                 │
│  Copilot.SquadAgent.StickerManager.Infrastructure   │
│  Repositories EF Core / Dapper · DbContext          │
│  Migrations · Configuracoes EF · XDependency        │
└─────────────────────────────────────────────────────┘
```

---

## Pre-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [PostgreSQL](https://www.postgresql.org/download/) 14 ou superior
- Ferramenta global `dotnet-ef`:

```bash
dotnet tool install --global dotnet-ef
```

---

## Instalacao e Configuracao

**1. Clone o repositorio:**

```bash
git clone https://github.com/andrecini/dotnet-squad-copilot-agent.git
cd dotnet-squad-copilot-agent
```

**2. Restaure as dependencias:**

```bash
dotnet restore Copilot.SquadAgent.StickerManager/Copilot.SquadAgent.StickerManager.slnx
```

**3. Configure a connection string:**

Abra o arquivo abaixo e substitua o placeholder pelo valor do ambiente local:

```
Copilot.SquadAgent.StickerManager/0 - Presentation/Copilot.SquadAgent.StickerManager.Api/appsettings.json
```

| Variavel | Descricao |
|---|---|
| `ConnectionStrings__DefaultConnection` | String de conexao com o PostgreSQL (ex: `Host=localhost;Database=sticker_manager;Username=postgres;Password=sua_senha`) |

> Nunca comite valores reais de connection string. Use `appsettings.Development.json` (ignorado pelo `.gitignore`) para valores locais.

**4. Aplique as migrations:**

```bash
dotnet ef database update \
  --project "Copilot.SquadAgent.StickerManager/3 - Infrastructure/Copilot.SquadAgent.StickerManager.Infrastructure/Copilot.SquadAgent.StickerManager.Infrastructure.csproj" \
  --startup-project "Copilot.SquadAgent.StickerManager/0 - Presentation/Copilot.SquadAgent.StickerManager.Api/Copilot.SquadAgent.StickerManager.Api.csproj"
```

---

## Como Rodar

```bash
dotnet run --project "Copilot.SquadAgent.StickerManager/0 - Presentation/Copilot.SquadAgent.StickerManager.Api/Copilot.SquadAgent.StickerManager.Api.csproj"
```

A API estara disponivel em:

- **HTTPS:** `https://localhost:7103`
- **HTTP:** `http://localhost:5117`
- **Swagger UI:** `https://localhost:7103/swagger`

---

## Testes

Para executar todos os testes:

```bash
dotnet test Copilot.SquadAgent.StickerManager/Copilot.SquadAgent.StickerManager.slnx
```

Para executar com relatorio de cobertura:

```bash
dotnet test Copilot.SquadAgent.StickerManager/Copilot.SquadAgent.StickerManager.slnx \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage
```

A cobertura minima exigida e de **85%** por linha. O pipeline de CI bloqueia merges que nao atingem esse limite.

---

## Entidades do Dominio

| Entidade | Descricao |
|---|---|
| `User` | Usuario do sistema. Possui colecao de figurinhas e pode criar ofertas de troca. |
| `Team` | Selecao participante da Copa 2026. Cada figurinha pertence a um time. |
| `Sticker` | Figurinha do album. Possui raridade (`StickerRarity`) e referencia ao time. |
| `UserCollection` | Registro de uma figurinha na colecao de um usuario, com flag de repetida. |
| `TradeOffer` | Oferta de troca criada por um usuario. Possui status (`TradeOfferStatus`) e lista de itens. |
| `TradeOfferItem` | Item individual de uma oferta de troca, com direcao (`TradeOfferItemDirection`): figurinha oferecida ou solicitada. |

**Enums disponiveis:**

- `StickerRarity` — raridade da figurinha (ex: Common, Rare, Legendary)
- `TradeOfferStatus` — estado da oferta (ex: Pending, Accepted, Rejected, Cancelled)
- `TradeOfferItemDirection` — sentido do item na troca (Offered, Requested)

---

## Decisoes Arquiteturais

| Decisao | Justificativa |
|---|---|
| Clean Architecture com separacao estrita de camadas | Isola regras de negocio de detalhes de infraestrutura, facilitando testes e evolucao independente |
| Result Pattern — sem excecoes de negocio | Services e repositories retornam `Result<T>` ou `Result`, eliminando o uso de exceptions para controle de fluxo |
| Soft delete via `DeletedAt` | Registros nunca sao excluidos fisicamente; `Remove()` do EF Core nao e utilizado em nenhuma camada |
| AutoMapper para mapeamento entre camadas | Nenhum mapeamento manual entre objetos de camadas distintas; cada camada possui seus proprios DTOs |
| Enums armazenados como string no banco | Facilita legibilidade dos dados e evita problemas de migracao ao adicionar novos valores |
| Construtores primarios com DI | Padrao C# 12 adotado em todas as classes que recebem dependencias injetadas |
| Queries SQL como constantes no Domain | Queries Dapper sao definidas como constantes na camada de Domain, nunca inline nos repositorios |

---

## CI/CD

O projeto possui dois workflows no GitHub Actions:

### `ci.yml` — Integrado a Pull Requests

Executado automaticamente em PRs para `main`, `develop` e branches `feature/**`.

Etapas:
1. Checkout do repositorio
2. Setup do .NET 8 SDK
3. `dotnet restore` da solution
4. `dotnet build` em modo Release
5. `dotnet test` com coleta de cobertura (threshold de 85% por linha)
6. Upload do relatorio de cobertura como artefato (retencao: 7 dias)

### `deploy-staging.yml` — Integrado ao push na `main`

Executado automaticamente a cada push na branch `main`. Depende de tres jobs encadeados:

| Job | Descricao |
|---|---|
| `build-and-test` | Mesmo pipeline do `ci.yml` |
| `sonarcloud` | Analise estatica de codigo via SonarCloud (requer secrets `SONAR_TOKEN` e variaveis `SONAR_ORGANIZATION`, `SONAR_PROJECT_KEY`) |
| `deploy-staging` | Publicacao da aplicacao e deploy para o ambiente de staging (placeholder — substituir pelo mecanismo do ambiente real) |

---

## Agente SQUAD — `.claude/`

Este repositorio inclui um **agente de desenvolvimento completo** configurado para Claude Code, localizado no diretorio `.claude/`. O objetivo e estudar e demonstrar como estruturar um copilot especializado usando skills, contextos e orchestrator agent dentro de um projeto .NET real.

### Por que isso existe

A pasta `.claude/` e o resultado de um estudo pratico sobre como transformar um modelo de linguagem generico em um assistente especializado com comportamento previsivel, consistente e alinhado aos padroes arquiteturais do projeto. A ideia central e que o agente conheca profundamente o codigo antes de gerar qualquer coisa — e que toda resposta siga os mesmos padroes que um desenvolvedor experiente do time seguiria.

### Orchestrator Agent

O agente principal esta definido em `.claude/agents/squad.md` e e configurado como um **sub-agente do Claude Code** (`/squad`). Ele combina as perspectivas de Developer, Tech Lead, Product Owner e Scrum Master em um unico ponto de entrada.

**Ferramentas habilitadas:** `Read`, `Edit`, `Write`, `Glob`, `Grep`, `Bash`, `Agent`

**Fluxos predefinidos:**

| Gatilho | Sequencia de Skills |
|---|---|
| Nova feature | `create-card` → `create-feature` → `create-migration` → `create-unit-test` → `code-review` → `write-commit` |
| Verificacao de qualidade | `check-standards` → `check-coverage` → `refactor-to-standards` |
| Onboarding | `onboarding-checklist` |
| Release | `write-changelog-entry` |

**Servidores MCP configurados** (`.claude/mcp.json`):

| Servidor | Proposito |
|---|---|
| `github` | Leitura e criacao de issues, PRs e releases via `@modelcontextprotocol/server-github` |
| `filesystem` | Acesso estruturado ao codigo-fonte via `@modelcontextprotocol/server-filesystem` |
| `git` | Inspecao de historico e estado do repositorio via `@modelcontextprotocol/server-git` |
| `postgres` | Consulta direta ao banco para validar schema em migrations e queries |
| `mongodb` | Consulta ao banco NoSQL para validar documentos e colecoes |

### Skills (19 no total)

Cada skill e um arquivo `SKILL.md` que define objetivo, contextos necessarios, perguntas obrigatorias e o algoritmo de execucao. O agente nunca executa uma skill sem ter as informacoes minimas — e nunca carrega todos os contextos de uma vez.

**Criacao de Artefatos**

| Skill | Descricao |
|---|---|
| `create-feature` | Feature completa ponta a ponta — endpoint, service, repository e testes |
| `create-endpoint` | Endpoint isolado com Minimal API, Validator, AppService e Swagger |
| `create-service` | Service com interface no Domain e implementacao no Application |
| `create-repository` | Repository com decisao automatica EF Core vs Dapper |
| `create-migration` | Migration EF Core com inspecao de schema |
| `create-dapper-query` | Query Dapper com constante no Domain e implementacao no repositorio |
| `create-integration` | Integracao externa — API, AWS, Kafka ou RabbitMQ |

**Testes**

| Skill | Descricao |
|---|---|
| `create-unit-test` | Conjunto completo — Data Mock, Mock Class e Teste |
| `create-integration-test` | Testes de integracao com WebApplicationFactory e rollback |
| `check-coverage` | Execucao de testes e analise de cobertura (meta: 85%) |

**Qualidade e Padroes**

| Skill | Descricao |
|---|---|
| `code-review` | Review estruturado com Blockers, Warnings e Suggestions |
| `check-standards` | Diagnostico de aderencia aos padroes sem alteracoes |
| `refactor-to-standards` | Refatoracao com opcao keep/undo por arquivo |

**Documentacao e Git**

| Skill | Descricao |
|---|---|
| `write-readme` | Geracao ou atualizacao do README.md |
| `write-commit` | Mensagem de commit no padrao Conventional Commits |
| `write-changelog-entry` | Entrada no CHANGELOG.md com sugestao de versao |

**Agil e Processo**

| Skill | Descricao |
|---|---|
| `create-card` | Issue no GitHub seguindo templates por tipo |
| `daily-summary` | Issue de daily assincrona coletiva |
| `onboarding-checklist` | Checklist de onboarding personalizado por perfil |

### Contextos (46 arquivos em 9 categorias)

Os contextos ensinam ao agente os padroes do projeto. Cada skill declara quais contextos precisa — o agente carrega apenas esses, nunca todos de uma vez.

| Categoria | Arquivos | O que cobre |
|---|---|---|
| `architecture/` | 8 | Clean Architecture, estrutura da solution, responsabilidades por camada, AutoMapper |
| `development/` | 8 | Minimal APIs, validators, DI, auth, exception handling, logging, Swagger |
| `persistence/` | 5 | EF Core, Dapper, MongoDB, query patterns, SQL |
| `patterns/` | 5 | Result Pattern, SOLID, Builder, Generic Repository, Unit of Work |
| `testing/` | 5 | Arquitetura de testes, unit tests, mock classes, data mocks, integration tests |
| `integrations/` | 5 | APIs externas, AWS, Kafka, RabbitMQ, resiliencia de mensageria |
| `engineering-process/` | 5 | GitFlow, Conventional Commits, code review checklist, CI/CD, release process |
| `agile/` | 2 | Cerimonias ageis assincronas, especificacao de cards |
| `documentation/` | 3 | Templates de README, CHANGELOG e onboarding |

---

## Como Contribuir

Antes de abrir um PR, siga os padroes adotados no projeto:

**Branching:**

| Prefixo | Uso |
|---|---|
| `feature/YYYYMM/descricao` | Nova funcionalidade |
| `fix/YYYYMM/descricao` | Correcao de bug |
| `hotfix/YYYYMM/descricao` | Correcao urgente em producao |
| `refactor/YYYYMM/descricao` | Refatoracao sem mudanca de comportamento |
| `docs/YYYYMM/descricao` | Atualizacao de documentacao |

**Commits:** seguem o padrao Conventional Commits com escopo e descricao em portugues:

```
FEAT(users): adiciona endpoint de registro de usuario
FIX(trade): corrige validacao de oferta duplicada
DOCS(readme): atualiza secao de instalacao
```

**Pull Requests:**
- Todo PR deve referenciar a issue correspondente
- Requer ao menos uma aprovacao antes do merge
- O pipeline de CI deve passar (build, testes e cobertura >= 85%)

---

## Changelog

Todas as mudancas relevantes sao documentadas no [CHANGELOG.md](./CHANGELOG.md).
