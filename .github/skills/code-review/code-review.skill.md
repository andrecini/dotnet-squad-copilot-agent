# Skill: Code Review

## Objetivo

Executa o code review de um Pull Request ou das alterações em staging, gerando um relatório estruturado com resultado de aprovação, pontos de melhoria e sugestões de correção com código.

---

## Contextos Necessários

- [code-review-checklist.md](../context/engineering-process/code-review-checklist.md)
- [solution-architecture.md](../context/architecture/solution-architecture.md)
- [layer-objects.md](../context/architecture/layer-objects.md)
- [automapper-profiles.md](../context/architecture/automapper-profiles.md)
- [result-pattern.md](../context/patterns/result-pattern.md)
- [solid.md](../context/patterns/solid.md)
- [minimal-apis.md](../context/development/minimal-apis.md)
- [validators.md](../context/development/validators.md)
- [dependency-injection.md](../context/development/dependency-injection.md)
- [logging-standards.md](../context/development/logging-standards.md)
- [unit-tests.md](../context/testing/unit-tests.md)
- [mock-classes.md](../context/testing/mock-classes.md)
- [data-mocks.md](../context/testing/data-mocks.md)

---

## Entrada

O usuário deve fornecer uma das seguintes opções:

- **Link do PR** — o agente analisa as alterações do Pull Request
- **Alterações em staging** — o agente analisa o diff das alterações atuais

Se nenhuma das opções for fornecida, perguntar:

```
Como deseja realizar o code review?
1. Informar o link do Pull Request
2. Usar as alterações em staging
```

---

## Passos

### 1. Coletar alterações

- Se **link do PR** → analisar todos os arquivos alterados no PR
- Se **staging** → analisar todos os arquivos com alterações não commitadas

### 2. Identificar contexto das alterações

Para cada arquivo alterado, identificar:

- **Camada** — Presentation, Application, Domain, Infrastructure ou Tests
- **Tipo de artefato** — Endpoint, AppService, Service, Repository, Validator, Filter, Middleware, Test, etc.
- **Contextos de revisão aplicáveis** — conforme [code-review-checklist.md](../context/engineering-process/code-review-checklist.md)

### 3. Executar checklist por categoria

Executar apenas as categorias do checklist relevantes às alterações identificadas:

- **Arquitetura e Camadas** — sempre
- **Código** — sempre
- **Minimal APIs e Endpoints** — se há alterações em endpoints
- **Validação** — se há alterações em validators ou requests
- **App Services** — se há alterações em AppServices
- **Testes** — se há alterações em classes de teste ou classes testáveis
- **Integrações** — se há alterações em clientes de integração ou mensageria
- **Injeção de Dependência** — se há novos serviços ou alterações em XDependency
- **Documentação e Padrões** — sempre
- **Segurança** — sempre

### 4. Classificar problemas encontrados

Cada problema encontrado deve ser classificado em:

| Classificação | Descrição |
|---------------|-----------|
| 🔴 **Blocker** | Viola padrão arquitetural, regra de segurança ou impede aprovação |
| 🟡 **Warning** | Desvio de padrão que deve ser corrigido mas não bloqueia |
| 🔵 **Suggestion** | Melhoria opcional que agrega qualidade sem ser obrigatória |

### 5. Gerar relatório

Produzir o relatório estruturado conforme o template abaixo.

---

## Template de Relatório

```markdown
# Code Review — [Nome do PR ou "Staging"]

## Resultado

> ✅ Aprovado | ⚠️ Aprovado com ressalvas | ❌ Reprovado

**Motivo:** [resumo objetivo do resultado]

---

## Resumo das Alterações

- **Arquivos analisados:** N
- **Camadas afetadas:** [Presentation, Application, Domain, Infrastructure, Tests]
- **Blockers:** N
- **Warnings:** N
- **Suggestions:** N

---

## Problemas Encontrados

### 🔴 Blockers

#### [NomeDoArquivo.cs] — [Linha ou trecho]
**Problema:** [descrição clara do problema e qual padrão está sendo violado]

**Como está:**
```csharp
// código atual
```

**Como deve ficar:**
```csharp
// código corrigido
```

---

### 🟡 Warnings

#### [NomeDoArquivo.cs] — [Linha ou trecho]
**Problema:** [descrição clara do problema]

**Como está:**
```csharp
// código atual
```

**Como deve ficar:**
```csharp
// código corrigido
```

---

### 🔵 Suggestions

#### [NomeDoArquivo.cs]
**Sugestão:** [descrição da melhoria e por que agrega valor]

**Exemplo:**
```csharp
// código sugerido
```

---

## Checklist de Aprovação

- [ ] Nenhum Blocker identificado
- [ ] Pipeline de CI passando
- [ ] Cobertura mínima de 85% mantida
- [ ] Nenhum warning de compilação

---

## Próximos Passos

> [lista objetiva do que precisa ser feito antes da aprovação, se houver]
```

---

## Critérios de Resultado

| Resultado | Condição |
|-----------|----------|
| ✅ Aprovado | Nenhum Blocker identificado |
| ⚠️ Aprovado com ressalvas | Nenhum Blocker, mas há Warnings que devem ser endereçados |
| ❌ Reprovado | Um ou mais Blockers identificados |

---

## Validação

Antes de entregar o relatório, verificar:

- [ ] Todas as categorias aplicáveis do checklist foram executadas
- [ ] Todo problema classificado como Blocker possui sugestão de correção com código
- [ ] Todo problema classificado como Warning possui sugestão de correção com código
- [ ] O resultado final está coerente com os problemas encontrados
- [ ] Nenhum item do checklist foi ignorado sem justificativa
- [ ] O relatório referencia o arquivo e trecho exato de cada problema encontrado
