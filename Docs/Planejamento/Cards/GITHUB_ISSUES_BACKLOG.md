# GitHub Issues — Copa 2026 Figurinhas Manager

## ✅ Como usar este documento

1. Copie o conteúdo de cada card
2. Acesse: `https://github.com/seu-usuario/seu-repo/issues/new`
3. Cole o título na seção "Title"
4. Cole o conteúdo markdown na seção "Body"
5. Adicione as labels conforme indicado
6. Clique em "Submit new issue"

---

## SPRINT 0 (Setup & Infrastructure)

### Card #1: [SPRINT 0] Setup Projeto .NET 8
**Story Points:** 5 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-0` `infrastructure` `priority-high`

#### Body
```
## Descrição

Criar solução com estrutura Clean Architecture, configurar DI, Swagger e primeiras migrations.

## Critérios de Aceite

- [ ] Projeto criado com estrutura: Presentation, Application, Domain, Infrastructure
- [ ] AutoMapper configurado com profiles básicos
- [ ] Swagger integrado e documentado
- [ ] Serilog configurado para logging
- [ ] DI container (Microsoft.Extensions.DependencyInjection) totalmente configurado
- [ ] Primeira migration executada com sucesso
- [ ] README.md com instruções de setup

## Tasks Técnicas

- [ ] Criar solução com 4 projetos .NET 8 Class Library
- [ ] Configurar arquivo appsettings.json e appsettings.Development.json
- [ ] Instalar NuGet packages: EF Core, Serilog, AutoMapper, FluentValidation
- [ ] Configurar DbContext base
- [ ] Criar primeira migration vazia

## Observações

> Bloqueador de todos os outros cards. Sênior apenas.
```

---

### Card #2: [SPRINT 0] Schema PostgreSQL
**Story Points:** 8 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-0` `database` `priority-high`

#### Body
```
## Descrição

Criar banco PostgreSQL com todas as tabelas, índices e constraints para MVP.

## Critérios de Aceite

- [ ] Tabela 'users' criada com campos: id, email, name, password_hash, created_at, updated_at, deleted_at
- [ ] Tabela 'teams' criada com 32 seleções pré-populadas
- [ ] Tabela 'stickers' com 650+ figurinhas
- [ ] Tabela 'user_collections' com constraint UNIQUE(user_id, sticker_id)
- [ ] Tabela 'trade_offers' e 'trade_offer_items' criadas
- [ ] Índices em: email, user_id, team_id, sticker_id
- [ ] Foreign keys com ON DELETE CASCADE/SET NULL conforme necessário
- [ ] Migration EF Core gerada e testada

## Tasks Técnicas

- [ ] Criar DbContext com DbSets para cada entidade
- [ ] Configurar Data Annotations ou Fluent API para relacionamentos
- [ ] Gerar migration via: dotnet ef migrations add InitialCreate
- [ ] Executar migration em banco de desenvolvimento
- [ ] Documentar schema em wiki do repositório

## Observações

> Dependência: 0-1. Executar em paralelo com 0-1 é aceitável.
```

---

### Card #3: [SPRINT 0] CI/CD Pipeline
**Story Points:** 8 | **Type:** Chore | **Priority:** Must Have  
**Labels:** `chore` `sprint-0` `devops` `priority-high`

#### Body
```
## Descrição

Configurar GitHub Actions para build, testes e deploy automático.

## Critérios de Aceite

- [ ] Workflow para pull requests: build + testes unitários
- [ ] Workflow para push em main: build + testes + deploy staging
- [ ] Testes com cobertura mínima 85%
- [ ] SonarQube integrado (código coverage)
- [ ] Notificação de falha no Slack/Teams (opcional)
- [ ] Workflow bem documentado em .github/workflows/

## Tasks Técnicas

- [ ] Criar arquivo .github/workflows/ci.yml
- [ ] Criar arquivo .github/workflows/deploy-staging.yml
- [ ] Configurar secrets do GitHub (DB_CONNECTION_STRING, etc)
- [ ] Testar fluxo completo com PR dummy

## Observações

> Pode ser feito em paralelo com 0-1 e 0-2. Não bloqueia testes locais.
```

---

### Card #4: [SPRINT 0] Documentação Técnica
**Story Points:** 3 | **Type:** Chore | **Priority:** Should Have  
**Labels:** `chore` `sprint-0` `documentation` `priority-medium`

#### Body
```
## Descrição

Criar README.md com arquitetura, stack e decisões técnicas.

## Critérios de Aceite

- [ ] README.md com: descrição do projeto, stack, arquitetura (diagrama ASCII)
- [ ] Instruções de setup (clone, restore, migrations, run)
- [ ] Decisões arquiteturais documentadas (por que Clean Architecture, por que PostgreSQL, etc)
- [ ] Links para documentação de padrões (Repository, Result Pattern, Value Objects)

## Tasks Técnicas

- [ ] Escrever seções no README
- [ ] Adicionar diagrama de camadas em ASCII
- [ ] Incluir exemplo de execução

## Observações

> Pode ser feito em paralelo com outros tasks de Sprint 0.
```

---

## SPRINT 1 (Authentication & User)

### Card #5: [SPRINT 1] Cadastro de Usuários
**Story Points:** 5 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-1` `auth` `priority-high`

#### Body
```
## Descrição

Implementar endpoint POST /auth/register com validação de email e hash de senha.

## Critérios de Aceite

- [ ] Endpoint POST /auth/register aceita JSON com email, name, password
- [ ] Validação: email único, email válido, senha forte (mín 8 chars, 1 maiúscula, 1 número)
- [ ] Senha armazenada com bcrypt (mín 10 rounds)
- [ ] Retorna HTTP 201 com user_id em caso de sucesso
- [ ] Retorna HTTP 400 com mensagem clara em caso de erro
- [ ] Testes unitários para validações
- [ ] Testes de integração para endpoint

## Tasks Técnicas

- [ ] Criar DTO: RegisterRequest (email, name, password)
- [ ] Criar Validator com FluentValidation
- [ ] Criar UseCase/Service: RegisterUserUseCase
- [ ] Implementar Repository.CreateUserAsync()
- [ ] Criar endpoint com TypedResults
- [ ] Escrever testes (mín 5 cenários)

## Observações

> Dependência: 0-1, 0-2. Iniciar assim que Sprint 0 terminar.
```

---

### Card #6: [SPRINT 1] Login e JWT
**Story Points:** 8 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-1` `auth` `priority-high`

#### Body
```
## Descrição

Implementar endpoint POST /auth/login e middleware de autenticação JWT.

## Critérios de Aceite

- [ ] Endpoint POST /auth/login aceita email + password
- [ ] Valida credenciais contra password_hash
- [ ] Gera JWT com claims: user_id, email, exp=1h
- [ ] Retorna token no format: { access_token, token_type: 'Bearer', expires_in: 3600 }
- [ ] Middleware de autenticação em Presentation layer
- [ ] Atributo [Authorize] funciona em endpoints protegidos
- [ ] Testes para login sucesso e falha
- [ ] Testes para JWT expirado/inválido

## Tasks Técnicas

- [ ] Gerar chave secreta para JWT (appsettings.json)
- [ ] Criar JwtTokenGenerator service
- [ ] Configurar autenticação no Program.cs
- [ ] Implementar middleware AuthenticationMiddleware
- [ ] Criar endpoint /auth/login com TypedResults
- [ ] Escrever testes (mín 6 cenários)

## Observações

> Dependência: 1-1. Executar em paralelo com 1-3.
```

---

### Card #7: [SPRINT 1] Perfil do Usuário
**Story Points:** 5 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-1` `auth` `priority-high`

#### Body
```
## Descrição

Implementar GET /users/me e PUT /users/profile para gerenciar dados do usuário.

## Critérios de Aceite

- [ ] GET /users/me retorna perfil do usuário autenticado (email, name, created_at)
- [ ] PUT /users/profile aceita email (opcional) e name (opcional)
- [ ] Valida email único se sendo alterado
- [ ] Retorna HTTP 200 com dados atualizados
- [ ] Requer autenticação [Authorize]
- [ ] Testes com usuário autenticado
- [ ] Testes sem token (HTTP 401)

## Tasks Técnicas

- [ ] Criar DTO: UserProfileResponse, UpdateProfileRequest
- [ ] Criar UseCase: GetUserProfileUseCase, UpdateProfileUseCase
- [ ] Endpoints GET /users/me e PUT /users/profile
- [ ] Extrair user_id do JWT claim
- [ ] Escrever testes (mín 4 cenários)

## Observações

> Dependência: 1-2. Bloqueia 1-4 e 1-5.
```

---

### Card #8: [SPRINT 1] Recuperação de Senha
**Story Points:** 5 | **Type:** Feature | **Priority:** Should Have  
**Labels:** `feat` `sprint-1` `auth` `priority-medium`

#### Body
```
## Descrição

Implementar fluxo de recuperação de senha (forgot password + reset).

## Critérios de Aceite

- [ ] Endpoint POST /auth/forgot-password aceita email
- [ ] Gera token temporário válido por 24h
- [ ] Valida email existente no banco
- [ ] Endpoint POST /auth/reset-password aceita token + new_password
- [ ] Valida token (não expirado, válido)
- [ ] Atualiza senha e invalida token
- [ ] Stub de envio de email (log apenas, não real)
- [ ] Testes para fluxo completo

## Tasks Técnicas

- [ ] Criar tabela password_reset_tokens (user_id, token, expires_at)
- [ ] Criar UseCase: ForgotPasswordUseCase, ResetPasswordUseCase
- [ ] Endpoints POST /auth/forgot-password e /auth/reset-password
- [ ] Implementar geração de token único (Guid)
- [ ] Adicionar validação de expiração
- [ ] Escrever testes (mín 5 cenários)

## Observações

> Dependência: 1-3. Could passar para pós-MVP se tempo apertar.
```

---

### Card #9: [SPRINT 1] Testes Autenticação
**Story Points:** 8 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-1` `testing` `priority-high`

#### Body
```
## Descrição

Conjunto completo de testes unitários e integração para autenticação (85%+ cobertura).

## Critérios de Aceite

- [ ] Testes unitários para: PasswordHasher, JwtTokenGenerator, Validators
- [ ] Testes de integração para endpoints: /auth/register, /auth/login, /auth/forgot-password
- [ ] WebApplicationFactory setup com banco in-memory
- [ ] Testes com fixtures para dados de teste
- [ ] Cobertura mínima de 85% em camada de Application
- [ ] Todos os testes passando no CI/CD
- [ ] Relatório de cobertura gerado

## Tasks Técnicas

- [ ] Criar projeto Copa2026.Tests.Integration
- [ ] Implementar fixtures para users e dados
- [ ] Escrever testes para cada UseCase
- [ ] Configurar xUnit + Shouldly + Moq
- [ ] Rodar dotnet test com /p:CollectCoverage=true

## Observações

> Dependência: 1-1, 1-2, 1-3, 1-4. Executar últimos dias de Sprint 1.
```

---

### Card #10: [SPRINT 1] Seed de Dados — Teams
**Story Points:** 3 | **Type:** Chore | **Priority:** Must Have  
**Labels:** `chore` `sprint-1` `database` `priority-high`

#### Body
```
## Descrição

Popular banco com 32 seleções da Copa do Mundo 2026.

## Critérios de Aceite

- [ ] Seed script com 32 teams oficiais da Copa 2026
- [ ] Cada time com: id, name, code (ISO 3166-1 alpha-3), flag_url
- [ ] Migration que executa seed automaticamente
- [ ] Idempotente (pode rodar multiple vezes sem erro)
- [ ] Verificar dados no banco após executar

## Tasks Técnicas

- [ ] Criar migration com HasData() fluent API
- [ ] Listar 32 times da Copa 2026 com codes corretos
- [ ] Adicionar URLs para bandeiras (Wikipedia ou CDN)

## Observações

> Pode ser executado em paralelo com 1-1 a 1-4. Necessário antes de Sprint 2.
```

---

## SPRINT 2 (Collection Management)

### Card #11: [SPRINT 2] Seed de Stickers
**Story Points:** 8 | **Type:** Chore | **Priority:** Must Have  
**Labels:** `chore` `sprint-2` `database` `priority-high`

#### Body
```
## Descrição

Popular banco com ~650 figurinhas da Copa 2026.

## Critérios de Aceite

- [ ] Cada seleção com ~20 figurinhas base
- [ ] 32 figurinhas especiais (técnicos, mascotes, etc)
- [ ] Cada figurinha com: number, team_id, player_name, rarity
- [ ] Rarity levels: comum (80%), rara (15%), ultra-rara (5%)
- [ ] Migration idempotente
- [ ] Seed executado com sucesso

## Tasks Técnicas

- [ ] Pesquisar estrutura do álbum oficial 2026
- [ ] Criar script em C# para gerar dados
- [ ] Criar migration com seed
- [ ] Validar contagem total (650+)

## Observações

> Dependência: 1-6. Bloqueia 2-2 a 2-9.
```

---

### Card #12: [SPRINT 2] Adicionar Figurinha
**Story Points:** 5 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-2` `collection` `priority-high`

#### Body
```
## Descrição

Implementar endpoint POST /collection para adicionar figurinha à coleção do usuário.

## Critérios de Aceite

- [ ] Endpoint POST /collection aceita sticker_id
- [ ] Valida se figurinha existe
- [ ] Cria entrada em user_collections ou incrementa quantity_owned
- [ ] Retorna HTTP 201 com collection_id
- [ ] Requer autenticação [Authorize]
- [ ] Valida se user já possui máximo de duplicatas (ex: 10)
- [ ] Testes para sucesso e erros

## Tasks Técnicas

- [ ] Criar DTO: AddToCollectionRequest
- [ ] Criar UseCase: AddStickerToCollectionUseCase
- [ ] Endpoint POST /collection
- [ ] Validações no service
- [ ] Testes (mín 4 cenários)

## Observações

> Dependência: 2-1, autenticação. Executar dias 1-2 de Sprint 2.
```

---

### Card #13: [SPRINT 2] Remover Figurinha
**Story Points:** 3 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-2` `collection` `priority-high`

#### Body
```
## Descrição

Implementar endpoint DELETE /collection/{id} para remover figurinha (soft delete).

## Critérios de Aceite

- [ ] Endpoint DELETE /collection/{id}
- [ ] Executa soft delete (marca deleted_at)
- [ ] Retorna HTTP 204 No Content
- [ ] Requer autenticação
- [ ] Valida se collection_id pertence ao usuário
- [ ] Testes para sucesso e erros (not found, unauthorized)

## Tasks Técnicas

- [ ] Criar UseCase: RemoveStickerFromCollectionUseCase
- [ ] Endpoint DELETE /collection/{id}
- [ ] Validação de permissão
- [ ] Testes (mín 3 cenários)

## Observações

> Dependência: 2-2. Pode executar em paralelo.
```

---

### Card #14: [SPRINT 2] Marcar Duplicata
**Story Points:** 3 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-2` `collection` `priority-high`

#### Body
```
## Descrição

Implementar endpoint PATCH /collection/{id}/duplicate para gerenciar duplicatas.

## Critérios de Aceite

- [ ] Endpoint PATCH /collection/{id}/duplicate
- [ ] Move 1 figurinha de quantity_owned para quantity_duplicate
- [ ] Ou move de duplicate de volta para owned
- [ ] Valida quantidade disponível
- [ ] Retorna HTTP 200 com dados atualizados
- [ ] Testes para ambas as direções

## Tasks Técnicas

- [ ] Criar DTO: ToggleDuplicateRequest (com ação 'mark' ou 'unmark')
- [ ] Criar UseCase: ToggleDuplicateUseCase
- [ ] Endpoint PATCH /collection/{id}/duplicate
- [ ] Testes (mín 4 cenários)

## Observações

> Dependência: 2-2. Executar em paralelo com 2-3.
```

---

### Card #15: [SPRINT 2] Listar Coleção
**Story Points:** 5 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-2` `collection` `priority-high`

#### Body
```
## Descrição

Implementar GET /collection com filtros e paginação.

## Critérios de Aceite

- [ ] Endpoint GET /collection?page=1&limit=100
- [ ] Filtros: team_id, rarity, sort (number/player_name/acquired_at)
- [ ] Retorna: [ { sticker_id, player_name, team, rarity, quantity_owned, quantity_duplicate } ]
- [ ] Paginação padrão 100 itens/página
- [ ] Requer autenticação
- [ ] Testes com vários filtros
- [ ] Performance: query otimizada com índices

## Tasks Técnicas

- [ ] Criar DTO: CollectionItemResponse, CollectionQueryRequest
- [ ] Criar UseCase: ListCollectionUseCase
- [ ] Endpoint GET /collection
- [ ] Repository com LINQ otimizado (Select projection)
- [ ] Testes (mín 5 cenários: sem filtro, filtro team, filtro rarity, paginação)

## Observações

> Dependência: 2-2. Bloqueia 2-9.
```

---

### Card #16: [SPRINT 2] Buscar Faltantes
**Story Points:** 5 | **Type:** Feature | **Priority:** Should Have  
**Labels:** `feat` `sprint-2` `collection` `priority-medium`

#### Body
```
## Descrição

Implementar GET /collection/missing para listar figurinhas que faltam.

## Critérios de Aceite

- [ ] Endpoint GET /collection/missing?sort=rarity
- [ ] Retorna todas as figurinhas que o usuário NÃO possui
- [ ] Ordena por: rarity (raras primeiro), team, number
- [ ] Retorna: [ { sticker_id, player_name, team, rarity, number } ]
- [ ] Requer autenticação
- [ ] Performance: 650+ figurinhas em <500ms
- [ ] Testes

## Tasks Técnicas

- [ ] Criar UseCase: ListMissingStickerUseCase
- [ ] Endpoint GET /collection/missing
- [ ] Query: SELECT stickers WHERE NOT EXISTS (user_collections)
- [ ] Testes (mín 3 cenários)

## Observações

> Dependência: 2-2. Pode executar em paralelo com 2-5 ou depois.
```

---

### Card #17: [SPRINT 2] Upload em Lote (CSV)
**Story Points:** 8 | **Type:** Feature | **Priority:** Could Have  
**Labels:** `feat` `sprint-2` `collection` `priority-low`

#### Body
```
## Descrição

Implementar POST /collection/import para importar figurinhas via CSV.

## Critérios de Aceite

- [ ] Endpoint POST /collection/import aceita arquivo CSV
- [ ] CSV com colunas: sticker_number, quantity (opcional, padrão 1)
- [ ] Valida cada linha antes de importar
- [ ] Retorna HTTP 200 com { imported: 50, failed: 2, errors: [{ line, reason }] }
- [ ] Transação: tudo ou nada (rollback se erro crítico)
- [ ] Limite máximo 1000 linhas por arquivo
- [ ] Testes com arquivo válido e inválido

## Tasks Técnicas

- [ ] Criar DTO: CsvImportRequest, ImportResult
- [ ] Parsing CSV com CsvHelper NuGet
- [ ] Validação linha a linha
- [ ] Transação com DbContext
- [ ] Testes (mín 4 cenários)

## Observações

> Could Have — pode pular de Sprint 2 para Sprint 3+ se prazo apertar.
```

---

### Card #18: [SPRINT 2] Estatísticas da Coleção
**Story Points:** 5 | **Type:** Feature | **Priority:** Should Have  
**Labels:** `feat` `sprint-2` `stats` `priority-medium`

#### Body
```
## Descrição

Implementar GET /collection/stats com métricas da coleção.

## Critérios de Aceite

- [ ] Endpoint GET /collection/stats
- [ ] Retorna: { total_owned: 450, total_missing: 200, completion_percentage: 69.2, duplicates: 15 }
- [ ] Breakdown por seleção: [ { team, owned, total, percentage } ]
- [ ] Breakdown por rarity: [ { rarity, owned, total } ]
- [ ] Requer autenticação
- [ ] Testes com dados variados

## Tasks Técnicas

- [ ] Criar DTO: CollectionStatsResponse, TeamStatsDto
- [ ] Criar UseCase: GetCollectionStatsUseCase
- [ ] Endpoint GET /collection/stats
- [ ] Queries otimizadas com GROUP BY
- [ ] Testes (mín 2 cenários)

## Observações

> Dependência: 2-2. Executar fins de Sprint 2.
```

---

### Card #19: [SPRINT 2] Testes Collection
**Story Points:** 13 | **Type:** Feature | **Priority:** Must Have  
**Labels:** `feat` `sprint-2` `testing` `priority-high`

#### Body
```
## Descrição

Testes unitários e integração para todos os endpoints de coleção (85%+ cobertura).

## Critérios de Aceite

- [ ] Testes para: AddStickerUseCase, RemoveStickerUseCase, ToggleDuplicateUseCase, ListCollectionUseCase, ListMissingUseCase, GetStatsUseCase
- [ ] Testes de integração para endpoints: POST /collection, DELETE /collection, PATCH /collection/{id}/duplicate, GET /collection, GET /collection/missing, GET /collection/stats
- [ ] Fixtures com usuários e figurinhas
- [ ] Cobertura mínima 85% em Application layer
- [ ] Testes de erros: figurinha não existe, coleção não pertence ao usuário, etc
- [ ] Relatório de cobertura

## Tasks Técnicas

- [ ] Setup WebApplicationFactory com banco de teste
- [ ] Escrever ~25 testes de integração
- [ ] Escrever ~15 testes unitários
- [ ] Rodar dotnet test com coverage

## Observações

> Dependência: 2-2 a 2-8. Último task de Sprint 2.
```

---

## SPRINT 3 (Album, Trades & Statistics)

### Card #20: [SPRINT 3] Visualizar Álbum Completo
**Story Points:** 5 | **Type:** Feature | **Priority:** Should Have  
**Labels:** `feat` `sprint-3` `album` `priority-medium`

#### Body
```
## Descrição

Implementar GET /album para visualizar status de todas as 650 figurinhas.

## Critérios de Aceite

- [ ] Endpoint GET /album
- [ ] Retorna array com TODAS as figurinhas do catálogo
- [ ] Cada item: { sticker_id, number, player_name, team, rarity, owned: boolean }
- [ ] Ordenado por: team, number
- [ ] Requer autenticação
- [ ] Performance: <1s mesmo com 650+ itens
- [ ] Testes

## Tasks Técnicas

- [ ] Criar DTO: AlbumItemResponse
- [ ] Criar UseCase: GetFullAlbumUseCase
- [ ] Endpoint GET /album
- [ ] LEFT JOIN com user_collections
- [ ] Testes (mín 2 cenários)

## Observações

> Dependência: 2-5. Primeiro task Sprint 3.
```

---

### Card #21: [SPRINT 3] Progresso por Time
**Story Points:** 3 | **Type:** Feature | **Priority:** Should Have  
**Labels:** `feat` `sprint-3` `album` `priority-medium`

#### Body
```
## Descrição

Implementar GET /album/{teamId}/progress para mostrar % de conclusão por seleção.

## Critérios de Aceite

- [ ] Endpoint GET /album/{teamId}/progress
- [ ] Retorna: { team_id, team_name, total_stickers, owned_count, completion_percentage: 75.5 }
- [ ] Valida se teamId existe
- [ ] Requer autenticação
- [ ] Testes

## Tasks Técnicas

- [ ] Criar DTO: TeamProgressResponse
- [ ] Criar UseCase: GetTeamProgressUseCase
- [ ] Endpoint GET /album/{teamId}/progress
- [ ] Testes (mín 2 cenários)

## Observações

> Dependência: 3-1. Pode executar em paralelo.
```

---

### Card #22: [SPRINT 3] Testes Sprint 3
**Story Points:** 13 | **Type:** Feature | **Priority:** Should Have  
**Labels:** `feat` `sprint-3` `testing` `priority-medium`

#### Body
```
## Descrição

Testes unitários e integração para álbum, trocas e notificações (85%+ cobertura).

## Critérios de Aceite

- [ ] Testes para todos os UseCases de Sprint 3
- [ ] Testes de integração para todos os endpoints
- [ ] Cobertura mínima 85%
- [ ] Testes de cenários complexos: troca de múltiplas figurinhas, validações de propriedade, etc
- [ ] Relatório de cobertura

## Tasks Técnicas

- [ ] Escrever ~30 testes de integração
- [ ] Escrever ~15 testes unitários
- [ ] Setup fixtures complexas com trades
- [ ] Rodar dotnet test com coverage

## Observações

> Dependência: 3-1 a 3-9. Último task Sprint 3.
```

---

## 📊 Resumo

| Sprint | Cards | Story Points | Foco |
|--------|-------|--------------|------|
| Sprint 0 | 4 | 24 | Setup & Infrastructure |
| Sprint 1 | 6 | 34 | Authentication & User |
| Sprint 2 | 9 | 55 | Collection Management |
| Sprint 3 | 3 | 21 | Album & Tests |
| **Total** | **22** | **134** | **MVP Copa 2026** |

## 🏷️ Distribuição de Labels

- **Priority:** `priority-high` (Must Have), `priority-medium` (Should Have), `priority-low` (Could Have)
- **Type:** `feat` (Feature), `chore` (Chore/Tech Debt), `spike` (Investigation)
- **Theme:** `infrastructure`, `database`, `devops`, `documentation`, `auth`, `testing`, `collection`, `stats`, `album`
- **Sprint:** `sprint-0`, `sprint-1`, `sprint-2`, `sprint-3`
