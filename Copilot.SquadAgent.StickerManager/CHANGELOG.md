# Changelog

Todas as mudanças relevantes deste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.0.0/) e este projeto adota [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Unreleased]

## [0.1.0] - 2026-05-10

### 🚀 Novas Funcionalidades

- Estruturar solução base com Clean Architecture (.Api, .Application, .Domain, .Infrastructure) (#34)
- Adicionar BaseEntity, Result Pattern (Result, Result\<T\>, ResultCode) e AppDbContext com soft delete ao Domain (#34)
- Configurar XDependency.cs nas 4 camadas, Program.cs com Minimal APIs, Serilog e Swagger (#34)
- Criar schema inicial do PostgreSQL com entities (User, Team, Sticker, UserCollection, TradeOffer, TradeOfferItem) (#35)
- Adicionar enums StickerRarity, TradeOfferStatus e TradeOfferItemDirection ao Domain (#35)
- Implementar IEntityTypeConfiguration para as 6 entidades com snake_case, índices, FKs e soft delete global (#35)
- Gerar migration InitialSchema via EF Core (#35)

### 🐛 Correções de Bug

- Corrigir ação do SonarCloud fixando por hash de commit para eliminar security hotspot (#36)

### 📦 Dependências e Configurações

- Configurar pipeline de CI com build, testes e cobertura mínima de 85% via GitHub Actions (#36)
- Configurar pipeline de deploy staging com SonarCloud integrado (#36)
- Substituir connection string por {{SECRET}} no appsettings.json e adicionar UserSecretsId no Api.csproj (#36)
- Remover EF Core Design do Api.csproj (duplicidade com Infrastructure) (#36)

### 📝 Documentação

- Adicionar README.md com arquitetura, setup, entidades, decisões arquiteturais e CI/CD (#34)
