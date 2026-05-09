# 📋 Planejamento Executivo — Copa 2026 Figurinhas Manager

**Data:** Maio 2026  
**Produto:** Sistema de Gerenciamento de Coleção de Figurinhas  
**PO & Scrum Master:** Squad Buddy  
**Duração:** 4 Sprints = 8 semanas

---

## 🎯 Visão do Produto

Uma aplicação backend robusta que permite colecionadores gerenciar figurinhas da Copa 2026, com funcionalidades de:
- Cadastro e gestão de coleção pessoal
- Identificação automática de faltantes
- Troca entre usuários
- Estatísticas e progresso

**MVP:** Backend 100% funcional (API REST) + consumo via Postman/Swagger

---

## 📊 Priorização — MoSCoW

| Nível | Definição | Sprint |
|-------|-----------|--------|
| **Must Have** (P0) | Essencial para MVP funcionar | 0, 1, 2 |
| **Should Have** (P1) | Importante, completa a experiência | 2, 3 |
| **Could Have** (P2) | Bom ter, mas pode esperar pós-MVP | 4+ |
| **Won't Have** | Fora do escopo atual | — |

---

## 📈 Escalas de Story Points

Usando Fibonacci: **1, 2, 3, 5, 8, 13, 21**

| Complexidade | SP | Estimativa |
|--------------|-----|-----------|
| Trivial | 1 | < 4 horas |
| Pequeno | 2 | 4-8 horas |
| Médio | 3 | 8-16 horas |
| Médio-Alto | 5 | 16-24 horas |
| Alto | 8 | 1-2 dias |
| Muito Alto | 13 | 2-3 dias |
| Epicamente Alto | 21 | 3-5 dias |

---

## 🏗️ Arquitetura do Projeto

### Camadas (Clean Architecture)

```
Copa2026.Api
├── Presentation/
│   ├── Controllers/
│   ├── DTOs/
│   └── Validators/
│
├── Application/
│   ├── UseCases/
│   ├── Services/
│   ├── Mappers/
│   └── Interfaces/
│
├── Domain/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── DomainEvents/
│   └── Interfaces/
│
└── Infrastructure/
    ├── Persistence/
    │   ├── Repositories/
    │   ├── Migrations/
    │   └── Context/
    ├── External/
    └── Services/
```

### Stack Tecnológico

| Componente | Tecnologia | Versão |
|-----------|-----------|--------|
| **Runtime** | .NET | 8.0 |
| **API** | Minimal APIs | Nativa |
| **ORM** | Entity Framework Core | 8.0 |
| **Banco** | PostgreSQL | 15+ |
| **Validação** | FluentValidation | 11.x |
| **Mapeamento** | AutoMapper | 13.x |
| **Testes** | xUnit + Shouldly + Moq | — |
| **Logging** | Serilog | 7.x |
| **DI** | Microsoft.Extensions.DependencyInjection | Nativa |

### Padrões Utilizados

- **Repository Pattern** com Unit of Work
- **Result Pattern** para erros de negócio
- **CQRS Leve** (separação de queries de commands)
- **Soft Delete** para exclusões lógicas
- **Value Objects** para conceitos do domínio
- **Domain Events** para sincronização entre agregados

### Entity Relationship Diagram (ERD)

```sql
-- Usuários
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    deleted_at TIMESTAMP
);

-- Seleções da Copa
CREATE TABLE teams (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    code VARCHAR(3) UNIQUE NOT NULL,
    flag_url VARCHAR(500)
);

-- Figurinhas (catálogo)
CREATE TABLE stickers (
    id SERIAL PRIMARY KEY,
    number INTEGER NOT NULL,
    team_id INTEGER NOT NULL REFERENCES teams(id),
    player_name VARCHAR(255),
    rarity VARCHAR(50), -- comum, rara, ultra-rara
    created_at TIMESTAMP DEFAULT NOW()
);

-- Coleção do usuário (figurinhas que possui)
CREATE TABLE user_collections (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id),
    sticker_id INTEGER NOT NULL REFERENCES stickers(id),
    quantity_owned INTEGER DEFAULT 1,
    quantity_duplicate INTEGER DEFAULT 0,
    acquired_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(user_id, sticker_id),
    deleted_at TIMESTAMP
);

-- Ofertas de troca
CREATE TABLE trade_offers (
    id SERIAL PRIMARY KEY,
    user_id_from INTEGER NOT NULL REFERENCES users(id),
    user_id_to INTEGER NOT NULL REFERENCES users(id),
    status VARCHAR(50), -- pending, accepted, rejected, completed
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    deleted_at TIMESTAMP
);

-- Detalhes da troca (quais figurinhas)
CREATE TABLE trade_offer_items (
    id SERIAL PRIMARY KEY,
    trade_offer_id INTEGER NOT NULL REFERENCES trade_offers(id),
    sticker_id_from INTEGER NOT NULL REFERENCES stickers(id),
    sticker_id_to INTEGER NOT NULL REFERENCES stickers(id),
    deleted_at TIMESTAMP
);
```

### Domains Principais

#### 1. **User** (Agregado)
- Entidade raiz: `User`
- Value Objects: `Email`, `Password`
- Métodos: `Register()`, `UpdateProfile()`, `Delete()`

#### 2. **Collection** (Agregado)
- Entidade raiz: `UserCollection`
- Value Objects: `Quantity`, `Rarity`
- Métodos: `AddSticker()`, `RemoveSticker()`, `MarkAsDuplicate()`

#### 3. **Sticker** (Agregado)
- Entidade raiz: `Sticker`
- Value Objects: `StickerNumber`, `Team`, `Rarity`
- Métodos: `GetRarityLevel()`, `GetTeamInfo()`

#### 4. **TradeOffer** (Agregado)
- Entidade raiz: `TradeOffer`
- Value Objects: `TradeStatus`, `TradeItem`
- Métodos: `Accept()`, `Reject()`, `Complete()`

---

## 🎯 Backlog Estruturado por Sprint

### 📦 Sprint 0 — Setup & Infraestrutura (1 semana)
**Objetivo:** Estabelecer base sólida do projeto

| # | Card | Prioridade | SP | Descrição |
|---|------|-----------|-----|-----------|
| **0-1** | Setup Projeto .NET 8 | Must | 5 | Criar solução com estrutura Clean Architecture, registar dependências, configurar Swagger |
| **0-2** | Schema PostgreSQL | Must | 8 | Criar banco com todas as tabelas, índices e constraints; rodar migrations iniciais |
| **0-3** | CI/CD Pipeline | Must | 8 | Configurar GitHub Actions para build, testes e deploy em staging |
| **0-4** | Documentação Técnica | Should | 3 | README.md com instruções de setup, arquitetura e decisões técnicas |

**Sprint Total:** 24 SP | **Duração:** 1 semana | **Recursos:** 1 Dev Sênior

---

### 🔐 Sprint 1 — Autenticação & Base (2 semanas)
**Objetivo:** Foundation de usuários e segurança

| # | Card | Prioridade | SP | Descrição |
|---|------|-----------|-----|-----------|
| **1-1** | Cadastro de Usuários | Must | 5 | Endpoint POST /auth/register com validação de email, hash de senha, testes |
| **1-2** | Login e JWT | Must | 8 | Endpoint POST /auth/login retornando JWT; middleware de autenticação |
| **1-3** | Perfil do Usuário | Must | 5 | GET /users/me, PUT /users/profile para editar nome e email |
| **1-4** | Recuperação de Senha | Should | 5 | Fluxo completo com token temporário (stub de email) |
| **1-5** | Testes Autenticação | Must | 8 | Testes unitários + integração para todos os endpoints de auth (85%+ cobertura) |
| **1-6** | Seed de Dados — Teams | Must | 3 | Popular banco com 32 seleções da Copa 2026 |

**Sprint Total:** 34 SP | **Duração:** 2 semanas | **Recursos:** 1-2 Devs

---

### 📚 Sprint 2 — Coleção (2 semanas)
**Objetivo:** Core de gerenciamento de figurinhas

| # | Card | Prioridade | SP | Descrição |
|---|------|-----------|-----|-----------|
| **2-1** | Seed de Stickers | Must | 8 | Popular banco com ~650 figurinhas (32 times × ~20 por time + especiais) |
| **2-2** | Adicionar Figurinha | Must | 5 | POST /collection com validação de propriedade; incrementar quantity_owned |
| **2-3** | Remover Figurinha | Must | 3 | DELETE /collection/{id} com soft delete |
| **2-4** | Marcar Duplicata | Must | 3 | PATCH /collection/{id}/duplicate para mover para quantidade_duplicada |
| **2-5** | Listar Coleção | Must | 5 | GET /collection com filtros por time, rarity, paginação (100 por página) |
| **2-6** | Buscar Faltantes | Should | 5 | GET /collection/missing lista todas as figurinhas que o usuário não tem |
| **2-7** | Upload em Lote (CSV) | Could | 8 | POST /collection/import aceita CSV com números de figurinhas; valida e importa |
| **2-8** | Estatísticas da Coleção | Should | 5 | GET /collection/stats retorna % completo, por time, raridades encontradas |
| **2-9** | Testes Sprint 2 | Must | 13 | Testes unitários + integração para collection (85%+ cobertura) |

**Sprint Total:** 55 SP | **Duração:** 2 semanas | **Recursos:** 2 Devs

---

### 🤝 Sprint 3 — Álbum Virtual & Troca (2 semanas)
**Objetivo:** Completing the MVP com features sociais

| # | Card | Prioridade | SP | Descrição |
|---|------|-----------|-----|-----------|
| **3-1** | Visualizar Álbum Completo | Should | 5 | GET /album lista o status de todas as 650 figurinhas (obtida/faltante) |
| **3-2** | Progresso por Time | Should | 3 | GET /album/{teamId}/progress mostra % de conclusão por seleção |
| **3-3** | Exportar Faltantes | Could | 3 | GET /collection/missing/export (JSON ou CSV) |
| **3-4** | Criar Oferta de Troca | Should | 8 | POST /trades com validação de figurinhas disponíveis, status=pending |
| **3-5** | Listar Ofertas Recebidas | Should | 5 | GET /trades/inbox com filtros por status; mostrar quem oferece o quê |
| **3-6** | Aceitar/Rejeitar Troca | Should | 5 | PATCH /trades/{id}/accept ou /reject; atualizar collections automaticamente |
| **3-7** | Notificações (Stub) | Could | 5 | GET /notifications para figurinhas faltantes que surgiram em ofertas; sem push real |
| **3-8** | Histórico de Trocas | Could | 5 | GET /trades/history lista trocas completadas com datas e parceiros |
| **3-9** | Gráficos de Progresso | Could | 5 | GET /stats/progress-timeline retorna % completo por semana |
| **3-10** | Testes Sprint 3 | Should | 13 | Testes unitários + integração para trades e álbum (85%+ cobertura) |

**Sprint Total:** 57 SP | **Duração:** 2 semanas | **Recursos:** 2-3 Devs

---

## 📋 Sequência de Sprints

```
┌─────────────────────────────────────────────────────────┐
│ SPRINT 0 (1 sem)  │ Setup (24 SP)    │ Bloqueador de tudo
├─────────────────────────────────────────────────────────┤
│ SPRINT 1 (2 sem)  │ Auth (34 SP)     │ Dependência: Sprint 0
├─────────────────────────────────────────────────────────┤
│ SPRINT 2 (2 sem)  │ Coleção (55 SP)  │ Dependência: Sprint 1
├─────────────────────────────────────────────────────────┤
│ SPRINT 3 (2 sem)  │ Troca (57 SP)    │ Dependência: Sprint 2
├─────────────────────────────────────────────────────────┤
│ 🎉 MVP Completo — 170 SP em 7 semanas
└─────────────────────────────────────────────────────────┘
```

---

## 🎓 Definição de Pronto (DoD)

Cada card é considerado "Done" quando:

- ✅ Código escrito em C# seguindo os padrões do projeto
- ✅ Testes unitários com 85%+ de cobertura
- ✅ Testes de integração passando
- ✅ CI/CD pipeline verde (build + testes)
- ✅ Code review aprovado por Tech Lead
- ✅ Documentação de API atualizada (Swagger XML comments)
- ✅ Commit com mensagem semântica: `feat(auth): create JWT endpoint`
- ✅ PR fechada e branch deletada

---

## 👥 Estrutura do Time

| Função | Responsabilidades | Dedicação |
|--------|-------------------|-----------|
| **Product Owner** | Priorização, validação de AC, demo | 20% |
| **Tech Lead** | Revisão de código, decisões arquiteturais | 30% |
| **Developer Sênior** | Sprint 0-1, mentorar juniors | 100% |
| **Developer Júnior** | Sprint 2-3, pair programming | 100% |

---

## 📅 Timeline Estimado

| Sprint | Início | Fim | Duração | Marcos |
|--------|--------|-----|---------|--------|
| 0 | Semana 1 | Semana 1 | 1 | Setup completo |
| 1 | Semana 2 | Semana 3 | 2 | Autenticação 100% |
| 2 | Semana 4 | Semana 5 | 2 | MVP Core funcional |
| 3 | Semana 6 | Semana 7 | 2 | 🎉 MVP Completo |

---

## 🚀 Pós-MVP (Roadmap)

- [ ] Frontend React + TypeScript
- [ ] Autenticação OAuth (Google, Facebook)
- [ ] Notificações Real-time (SignalR)
- [ ] Mobile App (Flutter/React Native)
- [ ] Marketplace de trocas com gamificação
- [ ] Analytics e Dashboard de negócio

