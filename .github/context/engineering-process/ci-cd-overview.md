# CI/CD Overview

## Visão Geral

As pipelines de CI/CD são configuradas via **GitHub Actions**. O projeto possui dois ambientes — `staging` e `production` — cada um com sua própria pipeline. Não há etapa de deploy configurada neste momento. As pipelines cobrem build, testes unitários, validação de cobertura de código e análise estática de qualidade.

-----

## Localização

```
[nome-do-projeto]/
└── .github/
    └── workflows/
        ├── ci-staging.yml
        └── ci-production.yml
```

-----

## Ferramentas

|Ferramenta       |Propósito                                        |
|-----------------|-------------------------------------------------|
|GitHub Actions   |Orquestração das pipelines                       |
|Coverlet         |Coleta de cobertura de testes                    |
|dotnet-coverage  |Geração e validação do relatório de cobertura    |
|Compilador .NET 8|Análise estática via warnings tratados como erros|

-----

## Pipelines

### CI — Staging

Disparada em todo Push ou Pull Request para a branch `develop`.

```yaml
name: CI - Staging

on:
  push:
    branches: [develop]
  pull_request:
    branches: [develop]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET 8
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore src/[componente].sln

      - name: Build
        run: dotnet build src/[componente].sln --no-restore --configuration Release /warnaserror

      - name: Run tests with coverage
        run: |
          dotnet test src/[componente].sln \
            --no-build \
            --configuration Release \
            --collect:"XPlat Code Coverage" \
            --results-directory ./coverage

      - name: Validate coverage threshold
        run: |
          dotnet tool install --global dotnet-coverage
          dotnet-coverage merge ./coverage/**/*.xml --output coverage.xml --output-format cobertura
          dotnet-coverage analyze --report coverage.xml --threshold 85
```

### CI — Production

Disparada em todo Push ou Pull Request para a branch `main`.

```yaml
name: CI - Production

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET 8
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore src/[componente].sln

      - name: Build
        run: dotnet build src/[componente].sln --no-restore --configuration Release /warnaserror

      - name: Run tests with coverage
        run: |
          dotnet test src/[componente].sln \
            --no-build \
            --configuration Release \
            --collect:"XPlat Code Coverage" \
            --results-directory ./coverage

      - name: Validate coverage threshold
        run: |
          dotnet tool install --global dotnet-coverage
          dotnet-coverage merge ./coverage/**/*.xml --output coverage.xml --output-format cobertura
          dotnet-coverage analyze --report coverage.xml --threshold 85
```

-----

## Etapas da Pipeline

|Etapa       |Descrição                                                          |
|------------|-------------------------------------------------------------------|
|Checkout    |Clona o repositório                                                |
|Setup .NET 8|Instala o SDK do .NET 8                                            |
|Restore     |Restaura os pacotes NuGet                                          |
|Build       |Compila a solution em modo Release com warnings tratados como erros|
|Test        |Executa os testes unitários com coleta de cobertura via Coverlet   |
|Coverage    |Valida se a cobertura mínima de **85%** foi atingida               |

-----

## Qualidade de Código

A análise estática é feita pelo próprio compilador do .NET 8 com a flag `/warnaserror`, que trata todos os warnings de compilação como erros, bloqueando o build em caso de violações. Isso cobre:

- Nullable reference types não tratados
- Variáveis não utilizadas
- Membros obsoletos
- Inconsistências de tipo e assinatura

-----

## Convenções

- Pull Requests para `develop` e `main` só podem ser mergeados após a pipeline passar com sucesso
- A cobertura mínima exigida é de **85%** — pipelines que não atingirem esse threshold falham
- Warnings de compilação bloqueiam o build — nenhum warning é ignorado em Release
- As pipelines de `staging` e `production` são idênticas neste momento — a diferença entre elas será o ponto de entrada para futuras etapas de deploy