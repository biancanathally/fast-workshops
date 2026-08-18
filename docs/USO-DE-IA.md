# Uso de IA no desenvolvimento

Este documento registra como o Claude (Anthropic) e o Gemini foram utilizados como ferramenta de apoio durante o desenvolvimento do FAST Workshops, conforme solicitado. O objetivo é tornar visível **onde a IA contribuiu, onde suas sugestões foram aceitas, e — principalmente — onde foram questionadas, corrigidas ou rejeitadas**.

O uso foi consistentemente de **parceiro de raciocínio e revisor**: a IA propunha, eu avaliava contra o enunciado e minhas próprias premissas, e a decisão final — inclusive quando divergia da sugestão — foi minha.

---

## Como o processo funcionou, de forma geral

1. Eu trazia decisões já parcialmente formadas (ex.: "não vou mover as implementações para Infrastructure") e pedia que a IA avaliasse essas escolhas, não que as tomasse por mim.
2. Pedi revisões periódicas do repositório real (via clone do GitHub), não apenas do código colado na conversa — isso permitiu que a IA apontasse divergências entre o que eu disse que tinha feito e o que estava de fato commitado.
3. Várias sugestões da IA foram implementadas exatamente como propostas; outras foram adaptadas; algumas foram recusadas. As três categorias estão documentadas abaixo.

---

## Decisões aceitas

| Sugestão | Onde se aplica | Por que aceitei |
|---|---|---|
| Arquitetura em 4 camadas (Domain / Application / Infrastructure / Api) | Estrutura geral da solution | Separação de responsabilidades clara para um projeto deste porte, sem introduzir complexidade desnecessária (ex.: CQRS/MediatR foram avaliados e descartados por overengineering) |
| `TryParseExact` com formato `yyyy-MM-dd` em vez do model binder padrão do ASP.NET | Filtros de data em `AtasController` e `WorkshopsController` | O binder padrão depende da cultura configurada no servidor e gera mensagens de erro genéricas; a validação explícita dá controle total sobre o formato aceito e a mensagem retornada |
| `AsSplitQuery()` em consultas com múltiplos `Include` de coleção | `AtaRepository`, `ColaboradorRepository` | Evita explosão cartesiana (linhas duplicadas via `JOIN`) quando uma entidade tem duas coleções navegadas na mesma query |
| `EF.Functions.Like` em vez de `.Contains()` nos filtros de texto | Repositórios de Ata e Workshop | Garante tradução para `LIKE` no SQL, compatível com índice — `.Contains()` pode ser avaliado em memória dependendo do provider |
| Filtro de data por intervalo (`>= inicio && < fim`) em vez de `.Date ==` | Mesmo ponto acima | `.Date` não é sargável — impede o uso de índice na coluna |
| `StringComparer.Create(new CultureInfo("pt-BR"), ignoreCase: true)` na ordenação de colaboradores | `ColaboradorService.ListarAsync` | Ordenação alfabética linguisticamente correta (ex.: "Álvaro" antes de "Ana"), independente do collation configurado no banco. Validei isso incluindo nomes acentuados propositalmente no seed |
| `IUnitOfWork` separado dos repositórios, com `CommitAsync` centralizando a persistência | `Domain.Abstractions.IUnitOfWork` + `EfUnitOfWork` | Eu havia deixado `SalvarAsync` duplicado em cada repositório *e* um `IUnitOfWork.CommitAsync` — a IA apontou a ambiguidade e sugeri remover a duplicação, mantendo só o Unit of Work |
| Tradução de erro de violação de índice único (SQL Server 2601/2627) em `ConflictException` no commit | `EfUnitOfWork.CommitAsync` | Havia uma janela de corrida entre a checagem prévia (`ExistePorWorkshopAsync`) e o `SaveChangesAsync`; duas requisições simultâneas para o mesmo workshop podiam gerar um 500 em vez de 409. A tradução do erro do banco fecha essa lacuna sem reescrever a lógica de validação |
| Camada de dados plugável no frontend (`VITE_USE_MOCK`) | `frontend/src/services/api.js` | Resolve dois requisitos do PDF ao mesmo tempo — "crie um mock" (frontend) e "a API real que eu já tinha" — sem duplicar componentes |
| `AbortController` em toda chamada do frontend, com debounce nos filtros | `App.jsx`, `useDebounce.js` | Contraparte, no cliente, dos `CancellationToken` que eu já propagava em todo o backend; evita que uma resposta antiga e lenta sobrescreva um resultado mais recente na tela |

---

## Decisões rejeitadas ou corrigidas

### 1. AutoMapper — rejeitado

**Sugestão da IA:** usar AutoMapper para o mapeamento entre entidades e DTOs.

**Minha decisão:** mappers manuais (`Mappers.cs` com métodos de extensão estáticos).

**Motivo:** para o número de entidades e DTOs deste projeto (três agregados, mapeamentos simples e diretos), o AutoMapper adiciona uma camada de configuração e comportamento "mágico" em tempo de execução sem ganho real. Prefiro que o mapeamento seja código explícito, visível e depurável — inclusive porque, numa entrevista técnica, "por que esse campo não veio preenchido?" tem resposta imediata quando o mapeamento é uma função comum, e não uma configuração de convenção.

### 2. Implementações de repositório dentro do projeto Domain — decisão minha, revertida após revisão

**Contexto:** em um momento do desenvolvimento, decidi manter as implementações concretas dos repositórios (que usam Entity Framework Core) fisicamente dentro da pasta `Domain/Repositories/Impl/`, compiladas para o projeto `Infrastructure` via `<Compile Include Link>` no `.csproj`. A ideia era não mexer na localização física dos arquivos.

**O que a IA apontou:** essa solução compilava e funcionava, mas mascarava uma violação real da separação de camadas — o `Domain` é a camada que **não deveria** conhecer tecnologia de persistência, e ter arquivos com `using Microsoft.EntityFrameworkCore;` fisicamente dentro dela (mesmo que compilados "para outro lugar" via link) é o tipo de detalhe que um avaliador percebe ao abrir o repositório no GitHub, antes mesmo de entender o `.csproj`.

**Minha decisão final:** revertida. Movi fisicamente os quatro arquivos de implementação (`AtaRepository`, `ColaboradorRepository`, `WorkshopRepository`, `EfUnitOfWork`) para `Infrastructure/Persistence/Repositories/`, removendo o hack de compilação. Este é o exemplo mais claro, neste projeto, de uma decisão minha que foi questionada, e que eu mudei depois de entender o argumento — não porque a IA "mandou", mas porque a justificativa técnica (a diferença entre "compila corretamente" e "representa corretamente a arquitetura") fazia sentido.

### 3. Nomenclatura com sufixo `Impl` — parcialmente aceito, com correção de inconsistência

**Contexto:** o projeto usava `AtaServiceImpl : AtaService` (sem prefixo `I` na interface) em paralelo com `AtaRepositoryImpl : IAtaRepository` (com prefixo `I`) — dois padrões diferentes coexistindo.

**Sugestão da IA:** padronizar com o prefixo `I` nas interfaces (convenção das Framework Design Guidelines da Microsoft) e remover o sufixo `Impl`, alinhando repositórios e serviços ao mesmo estilo.

**O que aconteceu na prática:** ao aplicar a padronização do prefixo `I`, o rename ficou incompleto numa primeira tentativa — `IColaboradorService.cs` e `IWorkshopService.cs` tinham o nome do arquivo certo, mas a interface *dentro* do arquivo continuava sem o `I`. Isso foi identificado numa revisão seguinte do repositório e corrigido. Depois, decidi também remover o sufixo `Impl` das implementações de serviço, para ficarem consistentes com o padrão já aplicado aos repositórios (mesmo nome de arquivo, pastas diferentes, sem sufixo).

**Por que registro isso:** é um exemplo de erro real de execução (rename parcial), não de decisão de design — vale deixar registrado porque mostra que a revisão em cima do repositório real (não da memória da conversa) pegou uma inconsistência que teria passado despercebida num code review superficial.

### 4. `DateTime` sem fuso horário — mantido conscientemente, não corrigido

**Observação da IA:** `Workshop.DataRealizacao` é `DateTime` sem `Kind` definido, o que em produção seria motivo de atenção (ambiguidade de fuso ao persistir em `datetime2`).

**Minha decisão:** mantive como está. Justificativa: o escopo do desafio é local e não distribuído geograficamente — não há requisito de múltiplos fusos. Troquei por `DateTimeOffset` seria "resolver" um problema que este projeto não tem, às custas de mais complexidade em toda a cadeia (DTOs, filtros, frontend). Decisão consciente de **não** aplicar a sugestão de robustecimento, com o trade-off documentado no README.

### 5. Exclusão de colaborador (`DELETE /api/colaboradores/{id}`) — não implementado, por leitura estrita do enunciado

Não chegou a ser uma sugestão da IA que eu rejeitei — foi uma pergunta minha ("por que dá pra remover colaborador de uma ata mas não apagar o colaborador?") que gerou uma checagem conjunta do PDF. Confirmamos que o enunciado nunca pede esse endpoint — o único `DELETE` relacionado a colaborador é o que remove a **presença** dele numa ata, não o cadastro. Decidi não implementar, para não assumir um comportamento (cascade? bloqueio se já tiver histórico?) que o enunciado não define. Documentado como ambiguidade resolvida no README principal.

## Registro das conversas

- [Claude — desenvolvimento do backend, arquitetura em camadas, revisões de código e frontend](https://claude.ai/share/76f6daff-e18e-4097-a2d0-ab62d63c31cf)
- [Gemini — apoio complementar durante o desenvolvimento](https://share.gemini.google/dy8Mm0K5ShJu)
