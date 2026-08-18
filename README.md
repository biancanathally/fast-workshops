# FAST Workshops

Sistema de rastreamento de participação em workshops trimestrais da FAST Soluções — API REST em .NET com persistência em SQL Server, e interface web em React para consulta de atas, filtros e indicadores de participação.

Desenvolvido como desafio técnico para a vaga de Pesquisa em Engenharia de Software e IA, projeto do CIn-UFPE em parceria com a FlowUp.

---

## Sumário

- [Stack](#stack)
- [Pré-requisitos](#pré-requisitos)
- [Como rodar](#como-rodar)
- [Endpoints da API](#endpoints-da-api)
- [Arquitetura](#arquitetura)
- [Decisões técnicas](#decisões-técnicas)
- [Ambiguidades do enunciado e como foram resolvidas](#ambiguidades-do-enunciado-e-como-foram-resolvidas)
- [Testes](#testes)
- [Uso de IA no desenvolvimento](#uso-de-ia-no-desenvolvimento)
- [Gestão do projeto](#gestão-do-projeto)

---

- [Protótipo de referência visual (Figma)](https://www.figma.com/design/FMKHLSdaXg06v90NVyLI9N/Fast-Workshops-Mockup---Bianca-Lima?node-id=0-1&t=Y4aTWtlgNxeQODTO-1)

---

## Stack

**Backend**
- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 (SQL Server)
- Swashbuckle (Swagger/OpenAPI)
- Arquitetura em camadas: Domain / Application / Infrastructure / Api
- Autenticação JWT (Bearer)

**Frontend**
- React 19 + Vite
- Recharts (gráficos de participação)
- Sem bibliotecas de UI ou CSS framework — estilização própria

**Infraestrutura**
- SQL Server 2022 via Docker Compose
- Swagger UI para documentação e teste manual da API

---

## Pré-requisitos

> ⚠️ **Este projeto usa .NET 10 e o formato de solução `.slnx`.** É necessário o **SDK do .NET 10** instalado — versões anteriores (.NET 8, por exemplo) não abrem a solução nem compilam o projeto.

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) e Docker Compose
- [Node.js 20+](https://nodejs.org/) e npm
- `dotnet-ef` (ferramenta global):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## Como rodar

### 1. Banco de dados

Na raiz do repositório:

```bash
docker compose up -d
docker compose ps   # aguarde o serviço ficar "healthy" (~30-40s na primeira vez)
```

### 2. Backend

```bash
dotnet run --project src/FastWorkshops.Api
```

A API sobe em **`http://localhost:5045`**. Migrations e seed de dados são aplicados automaticamente na primeira execução (ver [Decisões técnicas](#decisões-técnicas)).

O Swagger fica disponível na raiz: **http://localhost:5045**

### 3. Frontend

Em outro terminal:

```bash
cd frontend
cp .env.example .env.development   # já vem pré-configurado para localhost:5045
npm install
npm run dev
```

A interface sobe em **`http://localhost:5173`**.

### 4. Resetar o banco (voltar ao seed)

```bash
docker compose down -v   # remove o container e o volume de dados
docker compose up -d
dotnet run --project src/FastWorkshops.Api   # recria schema e reseeda automaticamente
```

### Modo mock (frontend sem backend)

Para rodar a interface sem a API no ar, usando dados de exemplo estáticos:

```bash
# frontend/.env.development
VITE_USE_MOCK=true
```

---

## Endpoints da API

Documentação interativa completa no Swagger (`http://localhost:5045`). Resumo:

### Autenticação

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/auth/login` | Autentica e retorna um token JWT |

Os endpoints de **escrita** (`POST`, `PUT`, `DELETE`) exigem autenticação — requisições sem token retornam `401`. Os endpoints de **leitura** (`GET`) são públicos, permitindo o uso do frontend sem login.

**Credenciais de teste:** usuário `admin`, senha `Fast@2026`.

Para autenticar no Swagger: chame `POST /api/auth/login`, copie o `token` da resposta e cole no botão **Authorize** (sem o prefixo "Bearer").

### Workshops

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/workshops` | Cadastra um workshop |
| `GET` | `/api/workshops/{id}` | Obtém um workshop por id |
| `GET` | `/api/workshops?nome=&data=` | Lista workshops com filtros opcionais e combináveis |

### Colaboradores

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/colaboradores` | Cadastra um colaborador |
| `GET` | `/api/colaboradores/{id}` | Obtém um colaborador por id |
| `GET` | `/api/colaboradores` | Lista colaboradores em ordem alfabética (pt-BR), com os workshops que cada um participou |

### Atas

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/atas` | Cria a ata de presença de um workshop (`colaboradorIds` opcional) |
| `GET` | `/api/atas/{id}` | Obtém uma ata por id |
| `GET` | `/api/atas?workshopNome=&data=&colaboradorNome=` | Lista atas com filtros opcionais e combináveis |
| `PUT` | `/api/atas/{ataId}/colaboradores/{colaboradorId}` | Adiciona um colaborador à ata (idempotente) |
| `DELETE` | `/api/atas/{ataId}/colaboradores/{colaboradorId}` | Remove um colaborador da ata |

**Formato de data em filtros:** `yyyy-MM-dd` (ex.: `2025-06-12`). Formato inválido retorna `400` com mensagem explicativa.

**Formato de erro:** todas as respostas de erro seguem [RFC 7807 (Problem Details)](https://www.rfc-editor.org/rfc/rfc7807), com `title`, `status`, `detail` e `traceId`.

Roteiro de verificação manual com todos os endpoints e casos de erro disponível em [`src/FastWorkshops.Api/FastWorkshops.Api.http`](src/FastWorkshops.Api/FastWorkshops.Api.http) (compatível com a extensão REST Client do VS Code e nativo no Rider/Visual Studio).

---

## Arquitetura

### Backend — camadas

```
src/
├── FastWorkshops.Domain/          # Entidades, interfaces de repositório, exceções, IUnitOfWork
├── FastWorkshops.Application/     # DTOs, mappers manuais, serviços com regras de negócio
├── FastWorkshops.Infrastructure/  # DbContext, configurations EF Core, implementações de repositório, migrations, seed
└── FastWorkshops.Api/             # Controllers, middleware de erros, Swagger, CORS
```

Regra de dependência: `Domain` não depende de nada. `Application` depende de `Domain`. `Infrastructure` depende de `Domain` (implementa os contratos). `Api` depende de `Application` e `Infrastructure`. Sem ciclos.

### Modelo de dados

- **Colaborador** (`Id`, `Nome`) — N:N com `Ata`
- **Workshop** (`Id`, `Nome`, `DataRealizacao`, `Descricao`) — 1:1 com `Ata`
- **Ata** (`Id`, `WorkshopId`, `Colaboradores`) — join table `AtaColaboradores`

Índice único em `Atas.WorkshopId`: garante no banco a regra "um workshop tem no máximo uma ata".

### Frontend

```
frontend/src/
├── App.jsx                    # orquestração: filtros, listagem, gráficos, modal
├── components/
│   ├── FiltrosBar.jsx          campos de busca (colaborador, workshop, data)
│   ├── ListaAtas.jsx           listagem de atas
│   ├── DetalheWorkshop.jsx     modal com colaboradores presentes
│   ├── EstadoVazio.jsx         estados de loading/vazio/erro
│   ├── GraficoBarras.jsx       workshops por colaborador (bônus)
│   └── GraficoPizza.jsx        colaboradores por workshop (bônus)
├── hooks/
│   └── useDebounce.js
└── services/
    ├── api.js                  camada de dados plugável (real ⇄ mock)
    └── mock*.json               dados de exemplo estáticos
```

---

## Decisões técnicas

| Decisão | Motivo |
|---|---|
| Mappers manuais em vez de AutoMapper | Complexidade e "mágica" em tempo de execução desnecessárias nesta escala |
| `TryParseExact` com `yyyy-MM-dd` em vez do model binder padrão | Formato único e explícito, independente da cultura configurada no servidor; mensagem de erro clara em vez do genérico "the value is not valid" |
| `AsSplitQuery()` em queries com múltiplos `Include` de coleção | Evita explosão cartesiana (linhas duplicadas via `JOIN`) |
| `EF.Functions.Like` em vez de `.Contains()` nos filtros de texto | Traduz para `LIKE` no SQL, compatível com índice; `.Contains()` pode ser avaliado em memória dependendo do provider |
| Filtro de data por intervalo (`>= inicio && < fim`), não `.Date ==` | Sargabilidade — `.Date` impede o uso de índice |
| `AsNoTracking()` apenas em consultas de leitura | Escrita (ex.: adicionar colaborador a uma ata) precisa de change tracking; leitura pura não |
| `StringComparer.Create(new CultureInfo("pt-BR"), ignoreCase: true)` na ordenação de colaboradores | Ordenação alfabética linguisticamente correta (ex.: "Álvaro" antes de "Ana"), independente do collation configurado no banco |
| `.Trim()` em entradas de texto antes de persistir | Evita que espaços incidentais criem registros "diferentes" que deveriam ser iguais, e quebrem buscas por `LIKE` |
| `IUnitOfWork` separado dos repositórios | Um repositório não deveria saber "quando" persistir — essa responsabilidade é centralizada, e abre caminho para operações que envolvem múltiplas entidades numa única transação |
| Tradução de violação de índice único (SQL Server 2601/2627) em `ConflictException` no `EfUnitOfWork.CommitAsync` | Mitiga condição de corrida: duas requisições simultâneas de `POST /api/atas` para o mesmo workshop podem ambas passar pela checagem prévia; a segunda é barrada pelo próprio banco e o erro é traduzido para `409`, evitando um `500` |
| `MigrateAsync()` + seed automáticos no startup da API | Conveniência para avaliação (sobe com um único comando). **Não é padrão recomendado para produção** — lá, migrations rodariam em pipeline de deploy separado. O risco de concorrência em múltiplas instâncias subindo simultaneamente é mitigado aqui pelo `healthcheck` + `depends_on: condition: service_healthy` do Docker Compose |
| `HttpsRedirection` removido | Em desenvolvimento local, o redirect para HTTPS com certificado autoassinado é fonte recorrente de atrito com CORS; o frontend acessa a API via HTTP direto |
| Camada de dados do frontend plugável (`VITE_USE_MOCK`) | Atende simultaneamente ao requisito de "criar um mock" e à existência de uma API real: a mesma interface funciona nos dois modos, sem duplicar componentes |
| Sem biblioteca de estado global (Redux, Zustand etc.) | Estado local (`useState`/`useEffect`) é suficiente para o escopo — uma tela, poucos componentes |
| Sem React Router | O enunciado descreve uma tela única (lista + detalhe em modal); adicionar roteamento seria complexidade sem propósito |
| `AbortController` em toda chamada de API do frontend | Contraparte, no cliente, dos `CancellationToken` propagados em toda a cadeia do backend — cancela requisições obsoletas quando o usuário digita rapidamente nos filtros |
| `IUnitOfWork` em `Domain.Abstractions`, repositórios em `Domain.Repositories` | Separação proposital: Unit of Work é um contrato de transação, não um contrato de acesso a dados — apesar de usado ao lado dos repositórios em todo serviço, representa uma responsabilidade diferente |
| `[Authorize]` apenas nos endpoints de escrita | Leituras públicas permitem avaliar o frontend sem login; o modelo do PDF não define entidade de usuário, então as credenciais ficam em configuração — criar tabela de usuários extrapolaria o modelo de dados do enunciado |

---

## Ambiguidades do enunciado e como foram resolvidas

O PDF de instruções contém algumas definições que admitem mais de uma leitura. As decisões abaixo foram tomadas de forma consciente e estão refletidas no código:

1. **Contexto narrativo vs. regra de validação.** O enunciado descreve os workshops como trimestrais, sempre às quintas-feiras, das 16h às 17h. Nenhum requisito funcional pede validação desses atributos no cadastro. **Decisão:** tratado como contexto de negócio, não como regra a ser validada — `POST /api/workshops` aceita qualquer data/hora.

2. **`Ata` no singular vs. plural.** A seção de definições descreve a ata como tendo "colaborador" (singular), mas o construtor sugerido é explícito: `Ata(int Id, Workshop workshop, List<Colaborador> colaboradores)`. **Decisão:** seguido o construtor — relação N:N entre `Ata` e `Colaborador`.

3. **`Workshop` com lista de colaboradores embutida.** Na etapa de frontend, o enunciado descreve o workshop como tendo "lista de colaboradores que participaram", mas o construtor sugerido para essa mesma entidade não inclui esse campo. **Decisão:** a relação continua modelada via `Ata` (que já carrega os colaboradores), sem duplicar a lista na entidade `Workshop` — evita uma segunda fonte de verdade para o mesmo dado.

4. **Escopo do `POST /api/atas`.** Não fica explícito se a ata deve nascer vazia (com colaboradores adicionados depois, via `PUT`) ou já populada. **Decisão:** `colaboradorIds` é aceito como campo opcional no corpo da requisição — atende às duas leituras possíveis do enunciado.

5. **Exclusão de colaborador não implementada.** O enunciado especifica `DELETE` apenas para remover a **presença** de um colaborador em uma ata específica (`/api/atas/{ataId}/colaboradores/{colaboradorId}`), nunca para excluir o **cadastro** do colaborador. Não existe `DELETE /api/colaboradores/{id}` em nenhuma parte do documento. **Decisão:** esse endpoint não foi implementado — adicioná-lo exigiria decidir um comportamento não especificado (bloquear exclusão de quem já tem histórico? cascade?), e isso seria inventar requisito, não interpretá-lo.

6. **Filtro por nome de colaborador em `GET /api/atas`.** O PDF define filtros por nome de workshop e por data, mas o mockup de frontend também pede um campo de busca por colaborador — sem um endpoint correspondente descrito na etapa de backend. **Decisão:** adicionado `colaboradorNome` como terceiro parâmetro opcional e combinável em `GET /api/atas`, para que o filtro do frontend seja resolvido no servidor (com índice), em vez de filtrado no cliente sobre a lista completa.

7. **`GET /api/workshops` não é um endpoint explicitamente pedido no PDF**, mas foi adicionado porque o frontend precisa popular filtros e listagens gerais sem depender de uma ata existir. Extensão consciente do contrato, não um desvio dele.

---

## Testes

Suíte de testes unitários com xUnit + NSubstitute + FluentAssertions, cobrindo as regras de negócio da camada de serviço e as invariantes da camada de domínio:

```bash
dotnet test
```

**Cobertura atual** (`tests/FastWorkshops.UnitTests`):
- **`Entities/AtaTests.cs`** — invariantes da entidade `Ata`: idempotência de `AdicionarColaborador` (não duplica presença) e retorno de `RemoverColaborador` quando o colaborador não está presente.
- **`Services/AtaServiceTests.cs`** — regras de negócio de `AtaService`: rejeição de workshop inexistente, conflito ao criar ata duplicada para o mesmo workshop, identificação de colaboradores inexistentes na criação, deduplicação de ids na requisição, idempotência do `PUT` e erro ao remover colaborador não presente na ata.
- **`Services/ColaboradorServiceTests.cs`** — ordenação alfabética pt-BR (validada com nomes acentuados) e normalização (`.Trim()`) de nome na criação.

<!-- **Não coberto ainda:**
- Testes de integração ponta a ponta (`WebApplicationFactory`), incluindo os códigos de status de erro (400/404/409) e o comportamento do middleware global de exceções.
- Testes automatizados do `WorkshopService`. -->

Roteiro de verificação manual de todos os endpoints e casos de erro, incluindo os de integração ainda não automatizados, disponível em [`src/FastWorkshops.Api/FastWorkshops.Api.http`](src/FastWorkshops.Api/FastWorkshops.Api.http).

---

## Uso de IA no desenvolvimento

Claude (Anthropic) foi utilizado como parceiro de raciocínio ao longo do desenvolvimento — para revisão de arquitetura, identificação de bugs antes da execução, e validação de decisões técnicas. Registro detalhado de prompts, decisões aceitas e **decisões rejeitadas** em [`docs/USO-DE-IA.md`](docs/USO-DE-IA.md). Além disso, o Gemini foi utilizado pontualmente como apoio na interpretação de mensagens de erro; os links de ambas as conversas estão no documento acima.

Dois exemplos de decisões revisadas e corrigidas ao longo do processo, detalhados no documento acima:
- As implementações de repositório inicialmente ficaram fisicamente dentro do projeto `Domain`, compiladas para `Infrastructure` via `<Compile Include Link>` no `.csproj` — uma solução que compilava, mas mascarava uma violação da separação de camadas. Foi identificada em revisão e corrigida, movendo os arquivos fisicamente para `Infrastructure`.
- A sugestão de usar AutoMapper para os DTOs foi avaliada e rejeitada em favor de mappers manuais, por controle explícito e ausência de mágica em tempo de execução desnecessária nesta escala de projeto.

---

## Gestão do projeto

O desenvolvimento foi acompanhado através de um board Kanban: [github.com/users/biancanathally/projects/1](https://github.com/users/biancanathally/projects/1).

---

## Autor

Bianca Nathally — [github.com/biancanathally](https://github.com/biancanathally)
