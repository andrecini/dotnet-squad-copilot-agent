# 🤖 Agente SQUAD — .NET 8

> Assistente de engenharia especializado em desenvolvimento back-end com .NET 8, projetado para atuar como um membro sênior do time: opina, revisa, gera código e documenta seguindo os padrões do squad.

---

## 📋 Índice

- [🤖 Agente SQUAD — .NET 8](#-agente-squad--net-8)
  - [📋 Índice](#-índice)
  - [Visão Geral](#visão-geral)
  - [Contextos Carregados](#contextos-carregados)
    - [🗂️ Parte 1 — Ágil, Arquitetura e Desenvolvimento](#️-parte-1--ágil-arquitetura-e-desenvolvimento)
    - [🗂️ Parte 2 — Documentação, Integrações e Padrões](#️-parte-2--documentação-integrações-e-padrões)
    - [🗂️ Parte 3 — Processo de Engenharia e Testes](#️-parte-3--processo-de-engenharia-e-testes)
  - [Capacidades](#capacidades)
    - [🏗️ Geração de Código](#️-geração-de-código)
    - [📐 Arquitetura](#-arquitetura)
    - [🔍 Code Review](#-code-review)
    - [📝 Documentação](#-documentação)
    - [✅ Cerimônias Ágeis](#-cerimônias-ágeis)
  - [Arquitetura de Referência](#arquitetura-de-referência)
  - [Padrões e Convenções](#padrões-e-convenções)
    - [Código](#código)
    - [Commits](#commits)
    - [Branching](#branching)
  - [Integrações Suportadas](#integrações-suportadas)
  - [Engenharia de Software](#engenharia-de-software)
    - [CI/CD](#cicd)
    - [Logging](#logging)
    - [Auth](#auth)
  - [Testes](#testes)
  - [Como Usar](#como-usar)
  - [Exemplos de Prompts](#exemplos-de-prompts)
  - [Observações](#observações)

---

## Visão Geral

O **Agente SQUAD** é um assistente de IA configurado com o conhecimento completo do time de engenharia: arquitetura da solução, padrões de código, convenções de commit, fluxo de CI/CD, integrações e muito mais.

Ele não apenas responde perguntas — ele age como um colega de time que conhece profundamente o contexto do projeto e pode contribuir de forma direta e opinativa.

---

## Contextos Carregados

O agente possui conhecimento sobre **três grandes grupos de arquivos de contexto**:

### 🗂️ Parte 1 — Ágil, Arquitetura e Desenvolvimento

| Grupo | Arquivos |
|---|---|
| **Agile** | `agile-ceremonies.md`, `card-specification.md`, `sprint-planning.md` |
| **Architecture** | `solution-architecture.md`, `project-structure.md`, `layer-presentation.md`, `layer-application.md`, `layer-domain.md`, `layer-infrastructure.md`, `layer-objects.md`, `automapper-profiles.md` |
| **Development** | `app-services.md`, `minimal-apis.md`, `validators.md`, `filters.md`, `dependency-injection.md`, `auth.md`, `exception-handling.md`, `logging-standards.md`, `api-documentation.md` |

### 🗂️ Parte 2 — Documentação, Integrações e Padrões

| Grupo | Arquivos |
|---|---|
| **Documentation** | `readme-template.md`, `changelog-template.md`, `onboarding-summary.md` |
| **Integrations** | `apis-integrations.md`, `aws-integrations.md`, `kafka-integrations.md`, `rabbit-mq-integrations.md`, `messaging-resilience.md` |
| **Patterns** | `solid.md`, `builder.md`, `result-pattern.md`, `generic-repository.md`, `unit-of-work.md` |
| **Persistence** | `sql.md`, `nosql.md`, `ef-standards.md`, `dapper-standards.md`, `query-patterns.md` |

### 🗂️ Parte 3 — Processo de Engenharia e Testes

| Grupo | Arquivos |
|---|---|
| **Engineering Process** | `branching-strategy.md`, `commit-standards.md`, `code-review-checklist.md`, `ci-cd-overview.md`, `release-process.md` |
| **Testing** | `test-architecture.md`, `unit-tests.md`, `mock-classes.md`, `data-mocks.md`, `integration-tests.md` |
| **Raiz** | `indice.md` |

---

## Capacidades

### 🏗️ Geração de Código
- Minimal APIs com .NET 8 seguindo os padrões do squad
- Application Services, Validators e Filters
- Repositórios genéricos e Unit of Work
- AutoMapper Profiles
- Injeção de dependência configurada corretamente
- Exception Handling global com respostas padronizadas

### 📐 Arquitetura
- Revisa e opina sobre decisões arquiteturais
- Orienta sobre separação de camadas (Presentation, Application, Domain, Infrastructure)
- Sugere uso correto de objetos de transferência (DTOs, ViewModels, Commands)

### 🔍 Code Review
- Aplica o checklist de code review do squad
- Avalia aderência aos princípios SOLID
- Identifica violações de padrões e sugere correções

### 📝 Documentação
- Gera READMEs seguindo o template do squad
- Produz entradas de CHANGELOG
- Auxilia no onboarding de novos membros

### ✅ Cerimônias Ágeis
- Auxilia na especificação de cards (critérios de aceite, DoD)
- Apoia o planejamento de sprint
- Esclarece dúvidas sobre cerimônias do time

---

## Arquitetura de Referência

```
src/
├── Presentation/          # Minimal APIs, Filters, Middlewares
├── Application/           # App Services, DTOs, Validators, Commands
├── Domain/                # Entidades, Interfaces, Regras de Negócio
├── Infrastructure/        # Repositórios, EF Core, Dapper, Integrações
└── Objects/               # ViewModels, Requests, Responses compartilhados
```

O agente conhece profundamente cada camada e orienta o desenvolvedor a colocar cada responsabilidade no lugar correto.

---

## Padrões e Convenções

### Código
- **Result Pattern** para retorno de operações sem exceções de controle de fluxo
- **Builder Pattern** para construção de objetos complexos
- **Generic Repository** com Unit of Work para acesso a dados
- **FluentValidation** para validação de entrada
- **AutoMapper** para mapeamento entre camadas

### Commits
O agente conhece os padrões de commit do squad e pode ajudar a escrever mensagens no formato correto (Conventional Commits ou o padrão adotado pelo time).

### Branching
Orientação sobre estratégia de branches: criação, nomenclatura, proteção de `main`/`develop` e fluxo de PRs.

---

## Integrações Suportadas

| Tecnologia | Contexto disponível |
|---|---|
| **AWS** | Padrões de integração com serviços AWS |
| **Apache Kafka** | Produção e consumo de mensagens |
| **RabbitMQ** | Filas, exchanges e resiliência de mensagens |
| **APIs Externas** | Padrões de integração HTTP, retry policies, circuit breaker |
| **SQL** | EF Core e Dapper com padrões do squad |
| **NoSQL** | Padrões para bancos não relacionais |

---

## Engenharia de Software

### CI/CD
O agente conhece o pipeline de CI/CD do squad e pode:
- Explicar etapas do pipeline
- Orientar sobre o processo de release
- Tirar dúvidas sobre o fluxo de deploy

### Logging
Orientação sobre padrões de log estruturado adotados pelo time, níveis de log e boas práticas.

### Auth
Conhecimento sobre o padrão de autenticação e autorização utilizado nas APIs do squad.

---

## Testes

O agente pode gerar e revisar:

- **Testes unitários** seguindo a arquitetura de testes do squad
- **Classes de mock** padronizadas
- **Dados de mock** (builders e fixtures)
- **Testes de integração** com a abordagem adotada pelo time

---

## Como Usar

O agente está disponível via interface de chat. Basta interagir em linguagem natural descrevendo o que você precisa. Quanto mais contexto você fornecer, mais precisa e aderente ao padrão do squad será a resposta.

**Dicas para melhores resultados:**
1. Informe em qual camada ou contexto você está trabalhando
2. Cole o código existente quando quiser uma revisão ou extensão
3. Mencione restrições ou requisitos específicos do seu card
4. Pergunte diretamente: *"isso está seguindo nossos padrões?"*

---

## Exemplos de Prompts

```
"Crie um Application Service para o caso de uso de criação de pedido,
 seguindo os padrões do squad com Result Pattern e FluentValidation."
```

```
"Revise esse repositório e diga se está alinhado com o generic repository
 que usamos no squad."
```

```
"Me ajuda a escrever os critérios de aceite para o card de integração
 com o Kafka de notificações."
```

```
"Gera o CHANGELOG da versão 2.3.0 com base nessas features e correções."
```

```
"Como devemos estruturar os testes de integração para esse endpoint?"
```

---

## Observações

- O agente reflete o estado dos contextos no momento em que foi configurado. Atualizações nos arquivos de contexto requerem uma nova carga.
- Para contextos muito específicos não cobertos pelos arquivos carregados, o agente irá indicar a limitação e sugerir o caminho mais próximo do padrão do squad.

---

*Agente SQUAD .NET 8 — Documentação gerada automaticamente.*