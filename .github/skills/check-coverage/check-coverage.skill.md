# Skill: Check Coverage

## Objetivo

Executa os testes unitários, coleta o relatório de cobertura e identifica classes sem testes ou abaixo do threshold mínimo de 85%. Ao final, sugere a execução do `create-unit-test` para as classes com cobertura insuficiente e aguarda a resposta do usuário.

---

## Contextos Necessários

- [unit-tests.md](../context/testing/unit-tests.md)
- [test-architecture.md](../context/testing/test-architecture.md)
- [ci-cd-overview.md](../context/engineering-process/ci-cd-overview.md)

---

## Entrada

Por padrão, a skill analisa todo o repositório. O usuário pode restringir o escopo:

```
Deseja verificar a cobertura do repositório completo ou de um escopo específico?
1. Completo — todos os projetos de testes
2. Por camada — Presentation, Application, Domain ou Infrastructure
3. Por projeto de testes — informar o projeto específico
```

---

## Passos

### 1. Definir escopo

- Se **completo** → executar todos os projetos de testes da solution
- Se **por camada** → executar apenas o projeto de testes correspondente
- Se **por projeto** → executar apenas o projeto informado

### 2. Executar testes com coleta de cobertura

Executar o comando conforme o escopo definido:

#### Completo
```bash
dotnet test src/[componente].sln \
  --configuration Release \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage
```

#### Por projeto
```bash
dotnet test src/Tests/[camada]/[componente].[Escopo].Tests \
  --configuration Release \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage
```

Reportar o resultado da execução antes de prosseguir:

```
✅ Testes executados com sucesso.
Total: N | Passou: N | Falhou: N | Ignorados: N
```

Se houver falhas, alertar o usuário:

```
⚠️ N teste(s) falharam durante a execução.
A análise de cobertura será feita apenas sobre os testes que passaram.
Deseja ver os detalhes das falhas antes de continuar?
1. Sim — exibir detalhes
2. Não — continuar com a análise de cobertura
```

### 3. Consolidar relatório de cobertura

Consolidar os arquivos de cobertura gerados:

```bash
dotnet tool install --global dotnet-coverage
dotnet-coverage merge ./coverage/**/*.xml --output coverage.xml --output-format cobertura
```

### 4. Analisar cobertura por classe

Para cada classe de negócio identificada no escopo, extrair:

- **Cobertura de linhas** — percentual de linhas cobertas
- **Cobertura de branches** — percentual de branches cobertos
- **Métodos não cobertos** — lista de métodos sem cobertura

Classificar cada classe em:

| Status | Condição |
|--------|----------|
| ✅ Conforme | Cobertura ≥ 85% |
| ⚠️ Atenção | Cobertura entre 60% e 84% |
| ❌ Crítico | Cobertura < 60% ou sem testes |

### 5. Gerar relatório

```markdown
# Relatório de Cobertura — [escopo analisado]
**Data:** [data atual]

---

## Resumo Executivo

| Métrica | Valor |
|---------|-------|
| Projetos analisados | N |
| Classes analisadas | N |
| Cobertura geral | N% |
| Classes conformes (≥ 85%) | N |
| Classes em atenção (60-84%) | N |
| Classes críticas (< 60%) | N |
| Classes sem testes | N |
| Threshold mínimo | 85% |
| Status geral | ✅ Aprovado / ❌ Reprovado |

---

## Cobertura por Camada

### 0 - Presentation
| Classe | Cobertura | Status |
|--------|-----------|--------|
| `OrderAppService` | 92% | ✅ Conforme |
| `CreateOrderRequestValidator` | 78% | ⚠️ Atenção |

### 1 - Application
| Classe | Cobertura | Status |
|--------|-----------|--------|
| `OrderService` | 45% | ❌ Crítico |
| `PaymentService` | 0% | ❌ Sem testes |

### 2 - Domain
_Todas as classes conformes._

### 3 - Infrastructure
| Classe | Cobertura | Status |
|--------|-----------|--------|
| `OrderRepository` | 70% | ⚠️ Atenção |

---

## Classes que precisam de atenção

### ❌ Críticas — cobertura < 60% ou sem testes
| Classe | Cobertura | Métodos não cobertos |
|--------|-----------|---------------------|
| `OrderService` | 45% | `UpdateAsync`, `DeleteAsync` |
| `PaymentService` | 0% | Todos |

### ⚠️ Em atenção — cobertura entre 60% e 84%
| Classe | Cobertura | Métodos não cobertos |
|--------|-----------|---------------------|
| `CreateOrderRequestValidator` | 78% | Cenários de itens inválidos |
| `OrderRepository` | 70% | `GetPagedAsync` |
```

### 6. Sugerir criação de testes

Após o relatório, apresentar sugestão para as classes com cobertura insuficiente:

```
As seguintes classes estão abaixo do threshold mínimo de 85%:

❌ OrderService (45%) — métodos não cobertos: UpdateAsync, DeleteAsync
❌ PaymentService (0%) — sem testes
⚠️ CreateOrderRequestValidator (78%) — cenários de itens inválidos
⚠️ OrderRepository (70%) — GetPagedAsync não coberto

Deseja criar os testes faltantes agora?
1. Sim, todas as classes — executar create-unit-test para cada uma
2. Selecionar — informar quais classes priorizar
3. Não — apenas registrar o relatório
```

Aguardar resposta do usuário antes de prosseguir.

Se o usuário confirmar, executar `create-unit-test` para cada classe selecionada na ordem de criticidade — classes sem testes primeiro, depois as abaixo de 60%, depois as entre 60% e 84%.

---

## Output Esperado

- Relatório de cobertura exibido no chat
- Sugestão de execução do `create-unit-test` para classes com cobertura insuficiente
- Execução do `create-unit-test` se confirmado pelo usuário

---

## Validação

Antes de entregar o relatório, verificar:

- [ ] Testes executados com sucesso — falhas reportadas ao usuário
- [ ] Cobertura calculada para todas as classes do escopo
- [ ] Classes sem testes identificadas e sinalizadas como críticas
- [ ] Threshold mínimo de 85% aplicado corretamente
- [ ] Status geral (Aprovado/Reprovado) coerente com os resultados
- [ ] Sugestão de `create-unit-test` apresentada para classes com cobertura insuficiente
- [ ] Resposta do usuário aguardada antes de executar qualquer skill adicional
