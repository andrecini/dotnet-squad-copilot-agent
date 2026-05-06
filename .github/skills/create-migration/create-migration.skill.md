---
name: create-migration
description: 'Use this skill when the user asks to create or apply an EF Core migration. Trigger for prompts like "create a migration", "add a migration for X", "generate migration for new entity". Do not trigger for repository or service creation — use the specific skills instead.'
license: MIT
compatibility: 'Requires .NET 8 SDK, Entity Framework Core and dotnet-ef CLI tool. Must be executed within a solution following the standards defined in ef-standards.md.'
model: gpt-4o
metadata:
  version: "1.0"
argument-hint: 'Required: description of the change (new entity, new column, altered column). Optional: raw SQL if already written.'
---

## Guardrails

- **Escopo restrito às camadas de Domain e Infrastructure** — nunca criar arquivos fora de `[componente].Domain/Entities/`, `[componente].Infrastructure/Data/`
- **Sem execução automática de migrations** — sempre confirmar com o usuário antes de executar `dotnet ef database update`
- **Sem alteração de migrations existentes** — nunca modificar arquivos de migration já gerados; criar nova migration para correções
- **Sem acesso a bancos de produção** — apenas ambientes de desenvolvimento e staging
- **Sem acesso a arquivos de configuração sensíveis** — nunca ler ou alterar `appsettings.Production.json`
- **Sem criação de stored procedures ou triggers** — apenas migrations de schema

# Skill: Create Migration

## Objetivo

Guia a criação de uma migration EF Core a partir de uma nova entidade, alteração de entidade existente ou detecção automática de alterações em staging. Atualiza a entidade e sua configuração quando necessário e executa o comando de migration após confirmação do usuário.

---

## Contextos Necessários

- [ef-standards.md](../context/persistence/ef-standards.md)
- [sql.md](../context/persistence/sql.md)
- [layer-domain.md](../context/architecture/layer-domain.md)
- [layer-infrastructure.md](../context/architecture/layer-infrastructure.md)

---

## Entrada

O usuário deve fornecer uma das seguintes opções:

- **Descrição da alteração** — ex: "adicionar coluna discount na tabela orders", "criar entidade Product"
- **Alterações em staging** — o agente detecta automaticamente as mudanças nas entidades e configurações

Se nenhuma opção for fornecida, perguntar:

```
Como deseja criar a migration?
1. Descrever a alteração — informar o que mudou
2. Detectar automaticamente — usar alterações em staging
```

---

## Passos

### 1. Identificar o tipo de alteração

A partir da entrada do usuário ou das alterações em staging, identificar:

- **Nova entidade** — criação completa de entidade, configuração e migration
- **Nova coluna** — adição de propriedade na entidade e configuração existente
- **Alteração de coluna** — modificação de tipo, tamanho ou constraint
- **Remoção de coluna** — soft removal via migration (nunca remover sem análise de impacto)
- **Novo índice ou constraint** — adição na configuração EF

### 2. Atualizar ou criar entidade
Seguindo [ef-standards.md](../context/persistence/ef-standards.md):

Se a entidade não existe, criar em `[componente].Domain/Entities/` herdando de `BaseEntity`:

```csharp
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
```

Se a entidade já existe, adicionar ou alterar apenas as propriedades necessárias.

### 3. Atualizar ou criar configuração EF
Seguindo [ef-standards.md](../context/persistence/ef-standards.md) e [sql.md](../context/persistence/sql.md):

Se a configuração não existe, criar em `[componente].Infrastructure/Data/Configurations/`:

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.StockQuantity)
            .HasColumnName("stock_quantity")
            .IsRequired();

        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").IsRequired();
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(x => x.DeletedAt == null);
    }
}
```

Se a configuração já existe, adicionar ou alterar apenas as configurações necessárias.

### 4. Atualizar DbContext
Se for uma nova entidade, adicionar o `DbSet` correspondente em `AppDbContext`:

```csharp
public DbSet<Product> Products => Set<Product>();
```

### 5. Confirmar execução do comando

Antes de executar, apresentar ao usuário:

```
Pronto para criar a migration. Deseja executar o comando agora?

Comando a ser executado:
dotnet ef migrations add [NomeDaMigration] \
  --project src/3\ -\ Infrastructure/[componente].Infrastructure \
  --startup-project src/0\ -\ Presentation/[componente].Api

1. Sim — executar agora
2. Não — apenas mostrar o comando
```

### 6. Executar ou exibir comando

**Se confirmado:** executar o comando e reportar o resultado.

**Se não confirmado:** exibir o comando completo para execução manual:

```bash
dotnet ef migrations add [NomeDaMigration] \
  --project src/3\ -\ Infrastructure/[componente].Infrastructure \
  --startup-project src/0\ -\ Presentation/[componente].Api
```

### 7. Verificar migration gerada

Após a execução, verificar se a migration gerada em `[componente].Infrastructure/Data/Migrations/` está coerente com as alterações realizadas e alertar o usuário caso haja inconsistências.

---

## Nomenclatura da Migration

O nome da migration deve ser descritivo e seguir o padrão `PascalCase`:

| Tipo de alteração | Exemplo |
|-------------------|---------|
| Nova entidade | `AddProductEntity` |
| Nova coluna | `AddDiscountColumnToOrders` |
| Alteração de coluna | `AlterOrderStatusColumnType` |
| Novo índice | `AddIndexToOrdersCustomerId` |
| Nova constraint | `AddUniqueConstraintToUsersEmail` |

---

## Output Esperado

```
[componente].Domain/Entities/
└── [Entidade].cs                                    — criado ou atualizado

[componente].Infrastructure/Data/
├── AppDbContext.cs                                   — atualizado (se nova entidade)
├── Configurations/[Entidade]Configuration.cs        — criado ou atualizado
└── Migrations/[Timestamp]_[NomeDaMigration].cs      — gerado pelo EF Core
```

---

## Validação

Antes de entregar o output, verificar:

- [ ] Entidade herda de `BaseEntity` — nunca define `Id`, `CreatedAt`, `UpdatedAt` ou `DeletedAt` diretamente
- [ ] Configuração usa `IEntityTypeConfiguration<T>` — nunca Fluent API direto no `OnModelCreating`
- [ ] Nomes de tabelas e colunas em `snake_case` — consulte [sql.md](../context/persistence/sql.md)
- [ ] `HasQueryFilter` aplicado para soft delete em toda nova entidade
- [ ] Enums configurados com `HasConversion<string>()`
- [ ] Timestamps configurados — `created_at`, `updated_at`, `deleted_at`
- [ ] `DbSet` adicionado ao `AppDbContext` se for nova entidade
- [ ] Nome da migration é descritivo e segue o padrão `PascalCase`
- [ ] Migration gerada está coerente com as alterações realizadas
