# Skill: Write Changelog Entry

## Objetivo

Gera e aplica uma nova entrada no `CHANGELOG.md` a partir dos PRs mergeados e commits desde a última release. Sugere automaticamente a versão com base no tipo de alterações e atualiza o arquivo diretamente.

---

## Contextos Necessários

- [changelog.md](../context/documentation/changelog-template.md)
- [release-process.md](../context/engineering-process/release-process.md)
- [commit-standards.md](../context/engineering-process/commit-standards.md)

---

## Entrada

A skill coleta as informações automaticamente. O usuário pode fornecer opcionalmente:

- **Versão da release** — se não informada, a skill sugere com base nas alterações
- **Data da release** — se não informada, usa a data atual

---

## Passos

### 1. Coletar alterações desde a última release

Analisar as seguintes fontes:

- **PRs mergeados** em `main` desde a última tag de release — título, número e labels
- **Commits** desde a última tag de release — tipo, escopo e descrição

### 2. Identificar a última versão

Verificar a última tag de release no repositório:

```
Última release: v[MAJOR].[MINOR].[PATCH]
Data: [data da última release]
```

Se não houver release anterior, assumir `v0.0.0` como base.

### 3. Classificar alterações por categoria

Agrupar as alterações conforme [changelog.md](../context/documentation/changelog-template.md):

| Tipo de commit / Label do PR | Categoria |
|------------------------------|-----------|
| `feat` | 🚀 Novas Funcionalidades |
| `fix` | 🐛 Correções de Bug |
| `perf` | ⚡ Melhorias de Performance |
| `refactor` | ♻️ Refatorações |
| `test` | 🧪 Testes |
| `chore` | 📦 Dependências e Configurações |
| `docs` | 📝 Documentação |
| `style`, `revert` | _(ignorados — sem impacto para o consumidor)_ |

### 4. Sugerir versão

Com base nas alterações classificadas, sugerir a versão seguindo Semantic Versioning:

| Condição | Incremento sugerido |
|----------|---------------------|
| Há alterações incompatíveis com versões anteriores | `MAJOR` |
| Há novas funcionalidades (`feat`) sem breaking changes | `MINOR` |
| Apenas correções de bug ou melhorias (`fix`, `perf`, `chore`) | `PATCH` |

Apresentar sugestão ao usuário antes de prosseguir:

```
Com base nas alterações identificadas, a versão sugerida é:

v[MAJOR].[MINOR].[PATCH] — [motivo: ex: "contém novas funcionalidades sem breaking changes"]

Última versão: v[anterior]
Nova versão sugerida: v[sugerida]

Deseja usar essa versão ou informar outra?
1. Usar a versão sugerida
2. Informar outra versão
```

### 5. Gerar entrada do changelog

Montar a entrada seguindo o template de [changelog.md](../context/documentation/changelog-template.md):

```markdown
## [1.1.0] - 2024-04-10

### 🚀 Novas Funcionalidades
- Adicionar endpoint de criação de pedido (#42)
- Integrar gateway de pagamento (#57)

### 🐛 Correções de Bug
- Corrigir validação de token expirado (#63)

### ♻️ Refatorações
- Extrair lógica de cálculo de frete para ValueObject (#71)

### 📦 Dependências e Configurações
- Atualizar pacotes NuGet para versões mais recentes (#48)
```

Categorias sem entradas são omitidas.

### 6. Atualizar CHANGELOG.md

Inserir a nova entrada logo abaixo da seção `[Unreleased]`, preservando todo o conteúdo existente:

```markdown
# Changelog

...

## [Unreleased]

## [1.1.0] - 2024-04-10   ← nova entrada inserida aqui

### 🚀 Novas Funcionalidades
...

## [1.0.0] - 2024-03-15   ← entradas anteriores preservadas
...
```

### 7. Confirmar alteração

Após atualizar o arquivo, confirmar ao usuário:

```
✅ CHANGELOG.md atualizado com sucesso!

Nova entrada: v[versão] — [data]
Categorias incluídas: [lista das categorias com entradas]
Alterações registradas: N
```

---

## Output Esperado

```
[nome-do-projeto]/
└── CHANGELOG.md — atualizado com nova entrada de release
```

---

## Validação

Antes de atualizar o arquivo, verificar:

- [ ] Versão confirmada pelo usuário antes de aplicar
- [ ] Alterações dos tipos `style` e `revert` excluídas do changelog
- [ ] Cada entrada referencia o número do PR entre parênteses — ex: `(#42)`
- [ ] Categorias sem entradas omitidas
- [ ] Nova entrada inserida abaixo de `[Unreleased]` — nunca no topo ou no final
- [ ] Conteúdo existente do `CHANGELOG.md` preservado integralmente
- [ ] Data no formato `YYYY-MM-DD`
- [ ] Versão no formato `v[MAJOR].[MINOR].[PATCH]`
- [ ] Conteúdo em **português**
