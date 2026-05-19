# Changelog

Todas as mudanças relevantes deste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/) e este projeto adota [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Unreleased]

## [8.0.0.0] - 2026-05-19

### Novas Funcionalidades

- Criar agente SQUAD como sub-agente do Claude Code em `.claude/agents/squad.md` com ferramentas Read, Edit, Write, Glob, Grep, Bash e Agent
- Criar 19 skills organizadas em 5 categorias: criacao de artefatos, testes, qualidade e padroes, documentacao/git e agil/processo
- Criar 46 arquivos de contexto organizados em 9 diretorios cobrindo arquitetura, desenvolvimento, persistencia, padroes, testes, integracoes, engenharia, agil e documentacao
- Configurar servidores MCP em `.claude/mcp.json` para GitHub, filesystem, git, PostgreSQL e MongoDB
- Definir fluxos predefinidos no agente para nova feature, verificacao de qualidade, onboarding e release
- Criar indice de skills (`.claude/skills/indice.md`) e indice de contextos (`.claude/context/indice.md`) como ponto de entrada do agente
- Implementar endpoints de Users, Teams, Stickers, UserCollections e TradeOffers com Minimal APIs, validators e AppServices
- Adicionar services e repositories para todas as entidades do dominio com Result Pattern
- Criar testes unitarios com Data Mocks, Mock Classes e cobertura minima de 85% por camada
- Adicionar projetos de teste isolados por camada (Application, Domain, Infrastructure) em `Tests/`

### Refatoracoes

- Reorganizar estrutura de pastas da solution StickerManager separando camadas em diretorios numerados
- Padronizar responses de rotas com paginacao usando objeto `PagedResult<T>` consistente entre endpoints
- Refatorar services para retornar `Result<T>` em todos os metodos, eliminando excecoes de negocio

### Correcoes de Bug

- Corrigir path da solution nas pipelines `ci.yml` e `deploy-staging.yml` apos reorganizacao de pastas

### Documentacao

- Atualizar README.md com secao dedicada ao agente SQUAD e a estrutura `.claude/`, incluindo lista de skills, categorias de contexto, fluxos predefinidos e servidores MCP configurados

## [0.1.0] - 2026-05-10

### Novas Funcionalidades

- Estruturar solução base com Clean Architecture (.Api, .Application, .Domain, .Infrastructure) (#34)
- Adicionar BaseEntity, Result Pattern (Result, Result\<T\>, ResultCode) e AppDbContext com soft delete ao Domain (#34)
- Configurar XDependency.cs nas 4 camadas, Program.cs com Minimal APIs, Serilog e Swagger (#34)
- Criar schema inicial do PostgreSQL com entities (User, Team, Sticker, UserCollection, TradeOffer, TradeOfferItem) (#35)
- Adicionar enums StickerRarity, TradeOfferStatus e TradeOfferItemDirection ao Domain (#35)
- Implementar IEntityTypeConfiguration para as 6 entidades com snake_case, índices, FKs e soft delete global (#35)
- Gerar migration InitialSchema via EF Core (#35)

### Correcoes de Bug

- Corrigir ação do SonarCloud fixando por hash de commit para eliminar security hotspot (#36)

### Dependencias e Configuracoes

- Configurar pipeline de CI com build, testes e cobertura mínima de 85% via GitHub Actions (#36)
- Configurar pipeline de deploy staging com SonarCloud integrado (#36)
- Substituir connection string por {{SECRET}} no appsettings.json e adicionar UserSecretsId no Api.csproj (#36)
- Remover EF Core Design do Api.csproj (duplicidade com Infrastructure) (#36)

### Documentacao

- Adicionar README.md com arquitetura, setup, entidades, decisões arquiteturais e CI/CD (#34)
