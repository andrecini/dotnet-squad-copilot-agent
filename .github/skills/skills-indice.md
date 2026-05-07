# Índice de Skills

Mapa completo de todas as skills disponíveis para o agente SQUAD, organizadas por categoria com localização e descrição resumida.

---

## 🏗️ Criação de Artefatos

| Skill | Localização | Descrição | Modelo |
|-------|-------------|-----------|--------|
| `create-feature` | `.github/skills/create-feature/create-feature.skill.md` | Criação completa de feature ponta a ponta — endpoint, service, repository e testes | Claude Sonnet |
| `create-endpoint` | `.github/skills/create-endpoint/create-endpoint.skill.md` | Criação isolada de endpoint com Minimal API, Validator, AppService e Swagger | GPT-4o |
| `create-service` | `.github/skills/create-service/create-service.skill.md` | Criação isolada de service com interface no Domain e implementação no Application | GPT-4o |
| `create-repository` | `.github/skills/create-repository/create-repository.skill.md` | Criação isolada de repository com decisão automática EF Core vs Dapper | GPT-4o |
| `create-migration` | `.github/skills/create-migration/create-migration.skill.md` | Criação de migration EF Core com atualização de entidade e configuração | GPT-4o |
| `create-dapper-query` | `.github/skills/create-dapper-query/create-dapper-query.skill.md` | Criação de query Dapper com constante no Domain e implementação no repositório | GPT-4o |
| `create-integration` | `.github/skills/create-integration/create-integration.skill.md` | Criação de integração externa — API, AWS, Kafka ou RabbitMQ | Claude Sonnet |

---

## 🧪 Testes

| Skill | Localização | Descrição | Modelo |
|-------|-------------|-----------|--------|
| `create-unit-test` | `.github/skills/create-unit-test/create-unit-test.skill.md` | Criação do conjunto completo de testes unitários — Data Mock, Mock Class e Teste | Claude Sonnet |
| `create-integration-test` | `.github/skills/create-integration-test/create-integration-test.skill.md` | Criação de testes de integração com WebApplicationFactory e rollback de transação | Claude Sonnet |
| `check-coverage` | `.github/skills/check-coverage/check-coverage.skill.md` | Execução dos testes, coleta de cobertura e identificação de classes abaixo de 85% | GPT-4o |

---

## 🔍 Qualidade e Padrões

| Skill | Localização | Descrição | Modelo |
|-------|-------------|-----------|--------|
| `code-review` | `.github/skills/code-review/code-review.skill.md` | Review estruturado de PR ou staging com Blockers, Warnings e Suggestions | Claude Sonnet |
| `check-standards` | `.github/skills/check-standards/check-standards.skill.md` | Diagnóstico de aderência aos padrões do projeto sem aplicar alterações | GPT-4o |
| `refactor-to-standards` | `.github/skills/refactor-to-standards/refactor-to-standards.skill.md` | Refatoração de arquivos para aderência aos padrões com opção keep/undo | GPT-4o |

---

## 📝 Documentação e Git

| Skill | Localização | Descrição | Modelo |
|-------|-------------|-----------|--------|
| `write-readme` | `.github/skills/write-readme/write-readme.skill.md` | Geração ou atualização do README.md seguindo o template do projeto | GPT-4o mini |
| `write-commit` | `.github/skills/write-commit/write-commit.skill.md` | Geração de mensagem de commit no padrão Conventional Commits em português | Claude Haiku |
| `write-changelog-entry` | `.github/skills/write-changelog-entry/write-changelog-entry.skill.md` | Geração e inserção de entrada no CHANGELOG.md com sugestão automática de versão | GPT-4o mini |

---

## 🔀 Ágil e Processo

| Skill | Localização | Descrição | Modelo |
|-------|-------------|-----------|--------|
| `create-card` | `.github/skills/create-card/create-card.skill.md` | Criação de Issue no GitHub seguindo os templates por tipo de card | GPT-4o mini |
| `daily-summary` | `.github/skills/daily-summary/daily-summary.skill.md` | Criação da Issue de daily assíncrona coletiva no GitHub | Claude Haiku |
| `onboarding-checklist` | `.github/skills/onboarding-checklist/onboarding-checklist.skill.md` | Geração de checklist de onboarding personalizado por perfil | Claude Haiku |

---

## Encadeamento Recomendado de Skills

```
Novo card identificado
  → create-card
    → create-feature (ou create-endpoint + create-service + create-repository)
      → create-migration (se nova entidade)
        → create-unit-test
          → create-integration-test
            → check-coverage
              → code-review
                → write-commit
```

```
Revisão de qualidade
  → check-standards
    → refactor-to-standards
      → check-coverage
        → create-unit-test (se necessário)
          → code-review
```

```
Release
  → write-changelog-entry
```

---

**Total: 19 skills** organizadas em **4 categorias**
