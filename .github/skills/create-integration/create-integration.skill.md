# Skill: Create Integration

## Objetivo

Guia a criação completa de uma integração externa seguindo os padrões arquiteturais do projeto. O tipo de integração e o escopo de geração são definidos pelo usuário — se não informados, devem ser perguntados antes de iniciar.

---

## Contextos Necessários

Consulte os seguintes arquivos antes de executar, conforme o tipo de integração:

| Tipo | Contextos |
|------|-----------|
| API Externa | [apis-integrations.md](../context/integrations/apis-integrations.md) |
| AWS | [aws-integrations.md](../context/integrations/aws-integrations.md) |
| Kafka | [kafka-integrations.md](../context/integrations/kafka-integrations.md) · [messaging-resilience.md](../context/integrations/messaging-resilience.md) |
| RabbitMQ | [rabbit-mq-integrations.md](../context/integrations/rabbit-mq-integrations.md) · [messaging-resilience.md](../context/integrations/messaging-resilience.md) |
| Todos | [layer-objects.md](../context/architecture/layer-objects.md) · [automapper-profiles.md](../context/architecture/automapper-profiles.md) · [dependency-injection.md](../context/development/dependency-injection.md) · [validators.md](../context/development/validators.md) |

---

## Entrada

O usuário deve fornecer:

- **Tipo de integração** — API Externa, AWS, Kafka ou RabbitMQ. Se não informado, perguntar:

```
Qual o tipo de integração?
1. API Externa (HttpClient + Polly)
2. AWS (AWSSDK)
3. Kafka (Confluent.Kafka)
4. RabbitMQ (RabbitMQ.Client)
```

- **Nome da integração** — ex: `PaymentGateway`, `S3Storage`, `OrderCreated`

- **Escopo de geração** — se não informado, perguntar:

```
Qual o escopo de geração?
1. Completo (Contrato + Implementação + Validator + Testes)
2. Apenas contrato (Domain)
3. Apenas implementação (Infrastructure)
4. Customizado — informar o que gerar
```

- **Para Kafka e RabbitMQ** — se não informado, perguntar:

```
O componente atuará como:
1. Producer
2. Consumer
3. Ambos
```

---

## Passos

### 1. Confirmar entradas
Se tipo, nome, escopo ou papel (Kafka/RabbitMQ) não foram informados, perguntar antes de prosseguir.

### 2. Gerar contratos no Domain

#### API Externa
Seguindo [apis-integrations.md](../context/integrations/apis-integrations.md):

```
[componente].Domain/Integrations/Apis/[ApiName]/
├── Interfaces/I[ApiName]Client.cs
├── [ApiName]Request.cs
└── [ApiName]Response.cs
```

#### AWS
Seguindo [aws-integrations.md](../context/integrations/aws-integrations.md):

```
[componente].Domain/Integrations/Aws/[ServiceName]/
├── Interfaces/I[ServiceName]Client.cs
├── [ServiceName]Request.cs
└── [ServiceName]Response.cs
```

#### Kafka
Seguindo [kafka-integrations.md](../context/integrations/kafka-integrations.md):

```
[componente].Domain/Integrations/Kafka/[TopicName]/
├── Interfaces/I[TopicName]Producer.cs  (se producer ou ambos)
├── Interfaces/I[TopicName]Consumer.cs  (se consumer ou ambos)
└── [TopicName]Message.cs
```

#### RabbitMQ
Seguindo [rabbit-mq-integrations.md](../context/integrations/rabbit-mq-integrations.md):

```
[componente].Domain/Integrations/RabbitMq/[QueueOrExchangeName]/
├── Interfaces/I[QueueOrExchangeName]Producer.cs  (se producer ou ambos)
├── Interfaces/I[QueueOrExchangeName]Consumer.cs  (se consumer ou ambos)
└── [QueueOrExchangeName]Message.cs
```

### 3. Gerar validator
Seguindo [validators.md](../context/development/validators.md):

- Criar validator para o objeto de request ou message em `[componente].Infrastructure/`
- Cobrir todos os campos obrigatórios e regras de formato

### 4. Gerar implementação no Infrastructure

#### API Externa
```
[componente].Infrastructure/Integrations/Apis/[ApiName]/
└── [ApiName]Client.cs
```
- Implementar `I[ApiName]Client` via `IHttpClientFactory`
- Aplicar circuit breaker via `CircuitBreakerPolicy`

#### AWS
```
[componente].Infrastructure/Integrations/Aws/[ServiceName]/
└── [ServiceName]Client.cs
```
- Implementar `I[ServiceName]Client` via cliente AWSSDK injetado
- Aplicar circuit breaker via `CircuitBreakerPolicy`

#### Kafka
```
[componente].Infrastructure/Integrations/Kafka/[TopicName]/
├── [TopicName]Producer.cs  (se producer ou ambos)
└── [TopicName]Consumer.cs  (se consumer ou ambos)
```
- Producer: implementar `I[TopicName]Producer` via `IProducer<string, string>`
- Consumer: herdar de `ResilientConsumerBase<TMessage>` — consulte [messaging-resilience.md](../context/integrations/messaging-resilience.md)
- Nomear tópicos com sufixos `-main`, `-retry`, `-dead-letter`

#### RabbitMQ
```
[componente].Infrastructure/Integrations/RabbitMq/[QueueOrExchangeName]/
├── [QueueOrExchangeName]Producer.cs  (se producer ou ambos)
└── [QueueOrExchangeName]Consumer.cs  (se consumer ou ambos)
```
- Producer: implementar `I[QueueOrExchangeName]Producer` via `IConnection`
- Consumer: herdar de `ResilientConsumerBase<TMessage>` — consulte [messaging-resilience.md](../context/integrations/messaging-resilience.md)
- Nomear filas com sufixos `-main`, `-retry`, `-dead-letter`

### 5. Gerar profile AutoMapper
Seguindo [automapper-profiles.md](../context/architecture/automapper-profiles.md):

- Adicionar mapeamentos em `[componente].Domain/Mappings/[IntegrationName]Profile.cs`:
  - Request/Message interno → Request/Message da integração
  - Response/Message da integração → Model interno

### 6. Registrar DI
Seguindo [dependency-injection.md](../context/development/dependency-injection.md):

- Clientes API/AWS → `Singleton` em `InfrastructureDependency.cs`
- Producers Kafka/RabbitMQ → `Singleton` em `InfrastructureDependency.cs`
- Consumers Kafka/RabbitMQ → `AddHostedService<T>()` em `InfrastructureDependency.cs`
- Validators → `InfrastructureDependency.cs`
- Profile AutoMapper → `AddAutoMapper()` em `DomainDependency.cs`

### 7. Gerar testes
Seguindo [unit-tests.md](../context/testing/unit-tests.md), [mock-classes.md](../context/testing/mock-classes.md) e [data-mocks.md](../context/testing/data-mocks.md):

- **Data Mocks** em `[componente].Infrastructure.Tests/DataMocks/`
  - `[IntegrationName]RequestMock` ou `[IntegrationName]MessageMock` com `Valid()` e cenários de falha
- **Mock Classes** em `[componente].Infrastructure.Tests/Mocks/Integrations/`
  - `[IntegrationName]ClientMock` com setup dos métodos utilizados
- **Testes** em `[componente].Infrastructure.Tests/Tests/Integrations/`
  - `[IntegrationName]ClientTests` cobrindo cenários de sucesso e falha
  - `[IntegrationName]ValidatorTests` cobrindo todos os campos

---

## Output Esperado

```
[componente].Domain/Integrations/[Tipo]/[IntegrationName]/
├── Interfaces/
├── [IntegrationName]Request.cs | [IntegrationName]Message.cs
└── [IntegrationName]Response.cs (se aplicável)

[componente].Domain/Mappings/[IntegrationName]Profile.cs

[componente].Infrastructure/Integrations/[Tipo]/[IntegrationName]/
├── [IntegrationName]Client.cs | [IntegrationName]Producer.cs | [IntegrationName]Consumer.cs

[componente].Infrastructure.Tests/
├── DataMocks/[IntegrationName]RequestMock.cs | [IntegrationName]MessageMock.cs
├── Mocks/Integrations/[IntegrationName]ClientMock.cs
└── Tests/Integrations/[IntegrationName]ClientTests.cs
```

---

## Validação

Antes de entregar o output, verificar:

- [ ] Contratos definidos no Domain — nunca na Infrastructure
- [ ] Implementações dependem apenas de abstrações do Domain
- [ ] AutoMapper usado para mapeamentos entre modelos internos e objetos da integração
- [ ] Validator cobre todos os campos obrigatórios do request ou message
- [ ] Consumers herdam de `ResilientConsumerBase` — [messaging-resilience.md](../context/integrations/messaging-resilience.md)
- [ ] Tópicos/filas nomeados com sufixos `-main`, `-retry`, `-dead-letter`
- [ ] Circuit breaker aplicado em clientes HTTP e AWS
- [ ] Construtores primários em todas as classes com DI
- [ ] `CancellationToken` propagado em todas as operações assíncronas
- [ ] Todos os componentes registrados na `InfrastructureDependency.cs`
- [ ] Data Mock possui método `Valid()` obrigatório
