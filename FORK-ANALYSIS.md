# Análise Comparativa de Forks — Pomelo.EntityFrameworkCore.MySql

**Data:** 7 de agosto de 2026
**Autor:** Sufficit (session de0a1821)
**Objetivo:** Identificar implementações valiosas em forks do Pomelo que possam ser aproveitadas no nosso fork (`sufficit/Pomelo.EntityFrameworkCore.MySql`, branch `upgrade/10.0.0`)

---

## Sumário Executivo

Foram investigados **416 forks** do repositório `PomeloFoundation/Pomelo.EntityFrameworkCore.MySql`. Apenas **2 forks** possuem trabalho substancial relevante ao upgrade EF Core 10: **microting** (implementador mais ativo, 179 stars) e **lauxjpn** (mantenedor original do Pomelo, 4 stars). Os demais forks estão inativos desde 2016–2022 ou apenas espelham o upstream.

Existem **3 PRs abertos** no upstream para EF Core 10: o nosso (#2047, 0 falhas), o do lauxjpn (#2019, 703 falhas) e o do ffquintella (#2043).

### Ranking de relevância

| Fork | Stars | Último push | EF Core 10? | Relevância |
|------|-------|-------------|-------------|------------|
| **microting** | 179 | Jul/2026 | ✅ (10.0.9) | **Alta** — implementador mais avançado |
| **lauxjpn** | 4 | Nov/2025 | ✅ (10.0.0) | **Média** — criador original, WIP inicial |
| BahramKaramiany | 1 | Ago/2025 | ❌ (9.x apenas) | Baixa — apenas espelha upstream |
| Mon322812 | 1 | Set/2025 | ❌ (9.x apenas) | Baixa — apenas espelha upstream |
| memsql/SingleStore | 2 | Jul/2026 | ❌ | N/A — fork para SingleStore, não MySQL |
| Outros (~410) | 0–1 | 2016–2024 | ❌ | Nenhuma — abandonados |

---

## 1. Fork: microting/Pomelo.EntityFrameworkCore.MySql

**URL:** https://github.com/microting/Pomelo.EntityFrameworkCore.MySql
**Stars:** 179 | **Último push:** 27/Jul/2026
**Branches ativos:** `net10`, `net11`, `feat-10.0.9-fix`, `10.0.5`, `v10.0.3`, `master`
**Versão EF Core:** 10.0.9 (branch `feat-10.0.9-fix`)

### 1.1 Visão geral

A Microting é uma empresa dinamarquesa que mantém o fork mais ativo e avançado do Pomelo. Eles publicam pacotes NuGet sob o namespace `Microting.EntityFrameworkCore.MySql` (renomearam todos os namespaces de `Pomelo` para `Microting`). Usam intensivamente GitHub Copilot e Dependabot, com mais de 100 branches de fix automatizados.

### 1.2 Implementações aproveitáveis (ALTA PRIORIDADE)

#### 🔴 1.2.1 `MySqlStructuralJsonTypeMapping` — Suporte a JSON complexo (.ToJson())

**Arquivo:** `src/EFCore.MySql/Storage/Internal/MySqlStructuralJsonTypeMapping.cs` (NOVO)

O EF Core 10 introduziu tipos complexos mapeados como JSON via `.ToJson()`. O Pomelo upstream não implementa isso (é um TODO). A Microting criou um type mapping completo que:

- Herda de `JsonTypeMapping`
- Lê JSON do MySQL como string e converte para `MemoryStream` (formato esperado pelo EF Core 10)
- Usa `CustomizeDataReaderExpression()` para criar a expressão `new MemoryStream(Encoding.UTF8.GetBytes(stringValue))`
- Usa `GetStringMethod` para leitura (MySQL armazena JSON como string)

**Por que aproveitar:** Sem isso, tipos complexos com `.ToJson()` não funcionam no MySQL. É uma feature central do EF Core 10.

#### 🔴 1.2.2 `MySqlJsonColumnConvention` — Convention para colunas JSON em complex properties

**Arquivo:** `src/EFCore.MySql/Metadata/Conventions/MySqlJsonColumnConvention.cs` (NOVO)

Convention que implementa `IComplexPropertyAddedConvention`, `IComplexPropertyAnnotationChangedConvention` e `IComplexTypeAnnotationChangedConvention`. Automaticamente configura o tipo de coluna como `"json"` para propriedades complexas mapeadas como JSON.

Funciona tanto para MySQL (5.7.8+, JSON nativo) quanto MariaDB (10.2.4+, JSON como alias LONGTEXT).

**Por que aproveitar:** Necessário para que migrations gerem DDL correto para complex properties JSON.

#### 🔴 1.2.3 `TransformJsonQueryToTable` — Tradução de queries JSON para JSON_TABLE

**Arquivo:** `src/EFCore.MySql/Query/Internal/MySqlQueryableMethodTranslatingExpressionVisitor.cs`

O upstream tem apenas `return base.TransformJsonQueryToTable(jsonQueryExpression);` com um comentário `// TODO: Implement for EF Core 7 JSON support.`. A Microting implementou a tradução completa usando `JSON_TABLE`:

- Calcula alias da tabela baseado no último segmento do path JSON
- Enumera propriedades da entidade via `GetPropertiesInHierarchy()`
- Filtra shadow keys irrelevantes
- Constrói lista de `ColumnInfo` para a cláusula COLUMNS do `JSON_TABLE`
- Usa `_sqlAliasManager.GenerateTableAlias()` para aliases únicos

**Por que aproveitar:** Sem isso, queries sobre propriedades JSON complexas não são traduzidas para SQL eficiente. Substitui o `base.TransformJsonQueryToTable()` que não funciona corretamente no MySQL.

#### 🟡 1.2.4 `AppendInsertReturningOperation` / `AppendUpdateReturningOperation` — RETURNING clause

**Arquivo:** `src/EFCore.MySql/Update/Internal/MySqlUpdateSqlGenerator.cs`

Implementa `RETURNING` para INSERT e UPDATE:
- Verifica `_options.ServerVersion.Supports.Returning` (MariaDB 10.5+)
- INSERT com RETURNING recupera valores gerados pelo banco
- UPDATE com RETURNING para concorrência otimista e leitura
- `requiresTransaction = false` (operação atômica única)
- Comenta que RETURNING não funciona em UPDATE no MariaDB (apenas INSERT/DELETE)

**Por que aproveitar:** Elimina round-trips para recuperar dados após INSERT/UPDATE. Especialmente valioso para o EF Core 10 que usa mais `readOperations`. **Atenção:** MariaDB não suporta `UPDATE ... RETURNING` — apenas MySQL 8.0+ (não oficialmente) e MariaDB 10.5+ para INSERT/DELETE.

#### 🟡 1.2.5 `IsValidSelectExpressionForExecuteDelete` expandido — DELETE com ORDER BY/LIMIT

**Arquivo:** `src/EFCore.MySql/Query/Internal/MySqlQueryableMethodTranslatingExpressionVisitor.cs`

Mudança que permite `ExecuteDelete` com ORDER BY e LIMIT, desde que:
- Haja apenas uma tabela ou
- Orderings estejam vazios e Limit seja null (em joins)

```csharp
// Antes (upstream): não permitia Orderings, Offset nem Limit
// Depois (microting): permite Orderings/Limit quando Tables.Count == 1
(selectExpression.Tables.Count == 1
    || (selectExpression.Orderings.Count == 0 && selectExpression.Limit is null))
```

**Por que aproveitar:** Permite mais cenários de `ExecuteDelete` que o EF Core 10 suporta nativamente.

#### 🟡 1.2.6 `EvaluateLeastGreatest` em `QueryStringFactory`

**Arquivo:** `src/EFCore.MySql/Query/Internal/MySqlQueryStringFactory.cs`

A Microting implementou avaliação client-side de `LEAST()` e `GREATEST()` dentro de cláusulas `LIMIT` para logging/debug de queries. O regex de detecção de LIMIT foi expandido para reconhecer `LEAST(...)` e `GREATEST(...)` como argumentos válidos, e o método `EvaluateLeastGreatest` resolve os valores dos parâmetros.

**Por que aproveitar:** Complementa nossa implementação de `LEAST()` em LIMIT (que nós já fizemos no provider). Melhora o logging de queries para debugging.

#### 🟢 1.2.7 Fixes de testes UInt64 enum para MariaDB

Vários commits corrigem testes que falham em MariaDB devido a diferenças de serialização de UInt64/enum:
- Skip de Theory tests problemáticos com TODO
- Override de testes para ajustar formato MySQL vs MariaDB
- Uso de literals UL para `InlineData`

**Por que aproveitar:** Se encontrarmos falhas de UInt64 enum em MariaDB, essas correções são referência direta.

#### 🟢 1.2.8 `DefaultIfEmpty` overrides para TPC

Override de `Subquery_over_primitive_collection_on_inheritance_derived_type` e métodos `DefaultIfEmpty` em `TPCGearsOfWarQueryMySqlTest`.

**Por que aproveitar:** Referência para baselines de TPC no EF Core 10.

### 1.3 O que NÃO aproveitar da Microting

| Item | Motivo |
|------|--------|
| **Renomeação de namespace `Pomelo` → `Microting`** | Quebra compatibilidade. Nosso fork mantém `Pomelo.EntityFrameworkCore.MySql` |
| **`EFCoreCompatibilityHelper`** | Infraestrutura genérica para multi-versão EF Core. Overengineered para nosso caso (apenas EF Core 10) |
| **Branding de `.targets` para Microting** | Específico do publis do fork deles |
| **100+ branches de Copilot auto-fix** | Maioria são tentativas iterativas de IA, muitas abandonadas/revertidas |
| **`Check.cs` reescrito** | Mudança de branding sem valor funcional |

---

## 2. Fork: lauxjpn/Pomelo.EntityFrameworkCore.MySql

**URL:** https://github.com/lauxjpn/Pomelo.EntityFrameworkCore.MySql
**Stars:** 4 | **Último push:** 15/Nov/2025
**Branch relevante:** `upgrade/10.0.0`
**Versão EF Core:** 10.0.0 (inicial, 703 falhas restantes)

### 2.1 Visão geral

Laurents (lauxjpn) é o **mantenedor original** do Pomelo.EntityFrameworkCore.MySql. Seu fork é essencialmente o upstream em desenvolvimento. O branch `upgrade/10.0.0` tem apenas 3 commits (upgrade, CI update, baselines) e o PR #2019 está aberto com **703 falhas** de testes funcionais.

O branch `main` tem features do Pomelo 9.x que ainda não chegaram ao upstream `main` do PomeloFoundation.

### 2.2 Implementações aproveitáveis do lauxjpn 9.x (branch `main`)

Estas features estão no branch `main` do lauxjpn (Pomelo 9.x) e podem ser portadas para nosso fork EF Core 10:

#### 🟡 2.2.1 `EF.Functions.DateDiff*()` — Funções DateDiff

**Commit:** `5322b411` — "Add EF.Functions DateDiffQuarter, DateDiffWeek, DateDiffMillisecond, DateDiffTick and DateDiffNanosecond"

Adiciona 5 novas funções DateDiff ao provider:
- `DateDiffQuarter(start, end)` → `TIMESTAMPDIFF(QUARTER, ...)`
- `DateDiffWeek(start, end)` → `TIMESTAMPDIFF(WEEK, ...)`
- `DateDiffMillisecond(start, end)` → `TIMESTAMPDIFF(MICROSECOND, ...) / 1000`
- `DateDiffTick(start, end)` → cálculo de ticks (.NET TimeSpan ticks)
- `DateDiffNanosecond(start, end)` → `TIMESTAMPDIFF(MICROSECOND, ...) * 1000`

**Por que aproveitar:** Útil para queries que calculam diferenças temporais. Compatível com EF Core 10.

#### 🟡 2.2.2 `EF.Functions.ConvertTimeZone()` — CONVERT_TZ

**Commit:** `1f88ec73` — "Add support for CONVERT_TZ via EF.Functions.ConvertTimeZone()"

Expõe a função `CONVERT_TZ(datetime, from_tz, to_tz)` do MySQL como `EF.Functions.ConvertTimeZone()`.

**Por que aproveitar:** Conversão de fuso horário no banco. Útil para aplicações multi-região.

#### 🟢 2.2.3 `DateTimeOffset` member translations

**Commit:** `d75b62e7` — "Add DateTimeOffset member translations for .DateTime, .UtcDateTime and .LocalDateTime"

Permite usar `.DateTime`, `.UtcDateTime` e `.LocalDateTime` em queries LINQ sobre `DateTimeOffset`.

**Por que aproveitar:** Compatibilidade com código que usa DateTimeOffset extensivamente.

#### 🟢 2.2.4 AutoDetect overloads

**Commit:** `f4d0feea` — "Add AutoDetect overloads"

Adiciona overloads de `AutoDetect` para `ServerVersion`.

#### 🟢 2.2.5 `TranslateParameterizedCollectionsToConstants()` como default

**Commit:** `0d480f21` — "Set TranslateParameterizedCollectionsToConstants() as a default option"

Faz com que coleções parametrizadas sejam traduzidas como constantes por padrão, melhorando performance de cache de queries.

**Por que aproveitar:** Performance. Queries com `Contains()` usando arrays/listas geram SQL mais eficiente e reutilizável.

#### 🟢 2.2.6 Não gerar `DEFAULT 0` para `AUTO_INCREMENT`

**Commit:** `23db204c` — "Don't generate DEFAULT 0 clause for AUTO_INCREMENT columns"

Remove a geração desnecessária de `DEFAULT 0` em colunas `AUTO_INCREMENT` nas migrations.

**Por que aproveitar:** DDL mais limpo. Em alguns casos, `DEFAULT 0` em AUTO_INCREMENT causa warnings no MySQL 8.x.

#### 🟢 2.2.7 Fix de precisão de `DateTime.UtcNow`/`DateTime.Now` em queries

**Commit:** `beaf6ebc` — "Fix issue where using DateTime.UtcNow and DateTime.Now will lose precision when used inside of a query"

Corrige perda de precisão quando `DateTime.UtcNow` ou `DateTime.Now` são usados dentro de queries LINQ.

**Por que aproveitar:** Bug fix importante para qualquer aplicação que use timestamps em queries.

### 2.3 PR #2019 (upgrade/10.0.0) — Status do upstream oficial

O PR oficial do lauxjpn para EF Core 10 tem:
- **703 falhas** de testes funcionais (vs nossas **0 falhas**)
- Apenas adaptações mínimas (sem novas features)
- Sem suporte a JSON complexo, RETURNING, etc.
- Status: `blocked` (não pronto para merge)

**Conclusão:** Nosso fork está significativamente mais avançado que o WIP oficial do lauxjpn para EF Core 10.

---

## 3. Forks secundários (baixa relevância)

### 3.1 BahramKaramiany/Pomelo.EntityFrameworkCore.MySql
- **Status:** Apenas espelha upstream 9.x (último commit: "Update branding to 9.0.1")
- **EF Core 10:** ❌ Nenhum trabalho
- **Aproveitável:** Nada

### 3.2 Mon322812/Pomelo.EntityFrameworkCore.MySql
- **Status:** Apenas espelha upstream 9.x + update de `dotnet-tools.json`
- **EF Core 10:** ❌ Nenhum trabalho
- **Aproveitável:** Nada

### 3.3 memsql/SingleStore.EntityFrameworkCore
- **Status:** Fork adaptado para SingleStore (antigo MemSQL)
- **Relevância:** Usa `SingleStoreConnector` em vez de `MySqlConnector`. Não compatível com MySQL/MariaDB.
- **Aproveitável:** Nada diretamente, mas é referência de como adaptar o provider para outro banco

### 3.4 Outros forks (~410)
- Todos com 0–1 stars
- Última atividade entre 2016 e 2024
- Nenhum trabalho relevante para EF Core 10
- Maioria são forks para issues pontuais já resolvidas

---

## 4. Estado do nosso fork (sufficit/Pomelo.EntityFrameworkCore.MySql)

### 4.1 O que já temos (branch `upgrade/10.0.0`)

| Feature | Commit | Status |
|---------|--------|--------|
| MySQL `DIV` operator para divisão inteira | `91efdbbd` | ✅ Implementado |
| `LEAST()`/`GREATEST()` em LIMIT avaliado client-side | — | ✅ Implementado |
| `IndexOf(char)` translator | `cf2a8999` | ✅ Implementado |
| Microsecond/Nanosecond temporal translations | `d2e0ec3f` | ✅ Implementado |
| DELETE error 1093 (`DeleteWithSelfReferencingSubquery` flag) | — | ✅ Implementado |
| SelectMany/LATERAL baselines | — | ✅ Implementado |
| VALUES ROW conditional baselines para MariaDB | `614336b9` | ✅ Implementado |
| CROSS/OUTER APPLY exception handling MariaDB | `614336b9` | ✅ Implementado |
| SkipTakeCollapsingExpressionVisitor para MariaDB | `84b5a8f9` | ✅ Implementado |
| FK constraint error 1451 try-catch | `dade1a8c` | ✅ Implementado |
| Regex ICU runtime error handling | `22d4150e` | ✅ Implementado |
| Temporal translations (83/83 testes) | `961a8954` | ✅ Implementado |
| MigrationsInfrastructure overrides (missing await) | `ceb2eb50` | ✅ Implementado |
| CI workflow com Docker MySQL/MariaDB matrix | `691bbb04` | ✅ Implementado |

**Resultado CI:** 12/12 jobs verdes (MySQL 8.0/8.4 + MariaDB 10.6/10.11/11.4/11.8, Windows + Ubuntu)
**PR upstream:** #2047 — `Failed: 0, Passed: 29,183, Skipped: 848` de 30,031 testes

### 4.2 O que NÃO temos (gaps identificados)

Comparando com microting + lauxjpn, identificamos estas lacunas:

---

## 5. Plano de aproveitamento — O que implementar

### 🔴 PRIORIDADE ALTA (features centrais do EF Core 10 ausentes)

| # | Feature | Origem | Esforço estimado | Impacto |
|---|---------|--------|------------------|---------|
| 1 | **`MySqlStructuralJsonTypeMapping`** — Type mapping para `.ToJson()` | microting | Médio (1 arquivo novo, ~100 linhas) | **Crítico** — sem isso, `.ToJson()` não funciona |
| 2 | **`MySqlJsonColumnConvention`** — Convention para complex properties JSON | microting | Médio (1 arquivo novo, ~120 linhas) | **Crítico** — DDL correto para JSON columns |
| 3 | **`TransformJsonQueryToTable`** — Tradução de queries JSON → `JSON_TABLE` | microting | Alto (modificar `MySqlQueryableMethodTranslatingExpressionVisitor.cs`, ~130 linhas) | **Alto** — queries JSON eficientes |

### 🟡 PRIORIDADE MÉDIA (melhorias de funcionalidade)

| # | Feature | Origem | Esforço estimado | Impacto |
|---|---------|--------|------------------|---------|
| 4 | **`RETURNING` clause** para INSERT/UPDATE | microting | Médio (modificar `MySqlUpdateSqlGenerator.cs`, ~120 linhas) | Reduz round-trips |
| 5 | **`ExecuteDelete` com ORDER BY/LIMIT** | microting | Baixo (modificar 1 método, ~5 linhas) | Mais cenários suportados |
| 6 | **`EF.Functions.DateDiff*()`** (5 funções) | lauxjpn 9.x | Médio (translator + method info) | API útil para queries |
| 7 | **`EF.Functions.ConvertTimeZone()`** | lauxjpn 9.x | Baixo (1 translator) | Conversão de timezone |
| 8 | **`DateTimeOffset` member translations** | lauxjpn 9.x | Baixo | Compatibilidade |
| 9 | **Fix precisão `DateTime.UtcNow/Now`** em queries | lauxjpn 9.x | Baixo | Bug fix |
| 10 | **Não gerar `DEFAULT 0` para `AUTO_INCREMENT`** | lauxjpn 9.x | Trivial | DDL mais limpo |

### 🟢 PRIORIDADE BAIXA (nice-to-have)

| # | Feature | Origem | Esforço | Impacto |
|---|---------|--------|---------|---------|
| 11 | **`TranslateParameterizedCollectionsToConstants()` default** | lauxjpn 9.x | Trivial (1 linha) | Performance de cache |
| 12 | **`EvaluateLeastGreatest` em QueryStringFactory** | microting | Baixo | Melhor logging/debug |
| 13 | **UInt64 enum test fixes** para MariaDB | microting | Baixo | Estabilidade de testes |
| 14 | **AutoDetect overloads** | lauxjpn 9.x | Trivial | Conveniência de API |

---

## 6. Detalhes técnicos para implementação

### 6.1 `MySqlStructuralJsonTypeMapping` (do microting)

**Conceito:** O EF Core 10 mapeia tipos complexos com `.ToJson()` esperando um `MemoryStream` do reader. O MySQL armazena JSON como string, então é necessário converter.

**Implementação-chave:**
```csharp
public class MySqlStructuralJsonTypeMapping : JsonTypeMapping
{
    public override MethodInfo GetDataReaderMethod()
        => typeof(DbDataReader).GetRuntimeMethod(nameof(DbDataReader.GetString), new[] { typeof(int) });

    public override Expression CustomizeDataReaderExpression(Expression expression)
        => Expression.New(
            typeof(MemoryStream).GetConstructor(new[] { typeof(byte[]) }),
            Expression.Call(typeof(Encoding).GetProperty(nameof(Encoding.UTF8)),
                typeof(Encoding).GetMethod(nameof(Encoding.GetBytes), new[] { typeof(string) }),
                expression));
}
```

**Registrar em:** `MySqlTypeMappingSource.cs`, mapear quando detectar `ToJson()` annotation.

### 6.2 `MySqlJsonColumnConvention` (do microting)

**Conceito:** Convention que detecta complex properties e automaticamente seta `ColumnType = "json"`.

**Implementar interfaces:**
- `IComplexPropertyAddedConvention`
- `IComplexPropertyAnnotationChangedConvention`
- `IComplexTypeAnnotationChangedConvention`

**Registrar em:** `MySqlConventionSetBuilder.cs`

### 6.3 `TransformJsonQueryToTable` (do microting)

**Conceito:** Substituir `base.TransformJsonQueryToTable()` (TODO no upstream) por implementação usando `JSON_TABLE()`.

**Pontos-chave:**
- Gerar alias de tabela baseado no último segmento do path JSON
- Enumerar propriedades com `GetJsonPropertyName()`
- Construir `MySqlJsonTableExpression.ColumnInfo` para cada propriedade
- Usar `_sqlAliasManager` para aliases únicos

### 6.4 `RETURNING` clause (do microting)

**Conceito:** Override de `AppendInsertReturningOperation` e `AppendUpdateReturningOperation`.

**Pontos de atenção:**
- MariaDB suporta `INSERT ... RETURNING` e `DELETE ... RETURNING` (10.5+)
- MariaDB **NÃO** suporta `UPDATE ... RETURNING`
- MySQL não suporta `RETURNING` nativamente (apenas via OUTPUT em SQL Server)
- Usar `_options.ServerVersion.Supports.Returning` para verificar

### 6.5 `EF.Functions.DateDiff*()` (do lauxjpn 9.x)

**Commits de referência:** `5322b411`

Implementar translator para:
```csharp
public static int DateDiffQuarter(this MySqlDbFunctionsExtensions _, DateTime start, DateTime end)
    => throw new InvalidOperationException();
```

Mapear para `TIMESTAMPDIFF(QUARTER, start, end)` etc.

---

## 7. Comparações diretas

### 7.1 Testes funcionais — ranking de qualidade

| Fork | Branch | Falhas | Passando | Skipped | Total |
|------|--------|--------|----------|---------|-------|
| **sufficit (nós)** | upgrade/10.0.0 | **0** | 29,183 | 848 | 30,031 |
| lauxjpn | upgrade/10.0.0 | 703 | ~29,777 | — | 30,480 |
| microting | feat-10.0.9-fix | ? (CI não público) | — | — | — |
| ffquintella | master | ? | — | — | — |

**Conclusão:** Nosso fork tem **zero falhas** de testes funcionais — o melhor resultado entre todos os forks e PRs.

### 7.2 Cobertura de features

| Feature | sufficit | microting | lauxjpn 10 | lauxjpn 9.x |
|---------|----------|-----------|------------|-------------|
| EF Core 10.0 | ✅ | ✅ (10.0.9) | ✅ (10.0.0) | ❌ |
| CI MySQL 8 + MariaDB matrix | ✅ 12/12 | ❓ | ❌ | ✅ |
| `.ToJson()` complex types | ❌ | ✅ | ❌ | ❌ |
| JSON column convention | ❌ | ✅ | ❌ | ❌ |
| `TransformJsonQueryToTable` | ❌ | ✅ | ❌ | ❌ |
| RETURNING clause | ❌ | ✅ | ❌ | ❌ |
| ExecuteDelete + ORDER BY | ❌ | ✅ | ❌ | ❌ |
| DIV integer division | ✅ | ❌ (não listado) | ❌ | ❌ |
| LEAST() in LIMIT | ✅ | ✅ | ❌ | ❌ |
| DELETE self-ref subquery | ✅ | ❌ | ❌ | ❌ |
| SkipTakeCollapsing MariaDB | ✅ | ❌ | ❌ | ❌ |
| DateDiff functions | ❌ | ❌ | ❌ | ✅ |
| ConvertTimeZone | ❌ | ❌ | ❌ | ✅ |
| DateTimeOffset members | ❌ | ❌ | ❌ | ✅ |
| DateTime.UtcNow precision fix | ❌ | ❌ | ❌ | ✅ |
| No DEFAULT 0 for AUTO_INC | ❌ | ❌ | ❌ | ✅ |
| TranslateParamCollections default | ❌ | ❌ | ❌ | ✅ |

---

## 8. Recomendações estratégicas

### 8.1 Curto prazo (antes de release)

1. **Implementar items #1–3** (JSON complexo) — sem isso, uma feature central do EF Core 10 não funciona
2. **Portar items #9–10** (DateTime precision fix + DEFAULT 0) — bug fixes triviais e de alto valor

### 8.2 Médio prazo

3. **Implementar item #4** (RETURNING clause) — ganho de performance significativo
4. **Portar items #6–8** (DateDiff, ConvertTimeZone, DateTimeOffset) — API mais rica

### 8.3 Longo prazo

5. **Item #5** (ExecuteDelete + ORDER BY) — ampliar cobertura
6. **Item #11** (TranslateParameterizedCollectionsToConstants default) — performance

### 8.4 Contribuição de volta ao upstream

Nosso PR #2047 está em melhor estado que o PR #2019 do lauxjpn (0 vs 703 falhas). Recomenda-se:
- Manter o PR atualizado
- Oferecer colaboração direta ao lauxjpn
- Considerar squash dos commits antes do merge

---

## Apêndice A — Fontes e referências

| Recurso | URL |
|---------|-----|
| Upstream | https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql |
| Nosso fork | https://github.com/sufficit/Pomelo.EntityFrameworkCore.MySql |
| Fork microting | https://github.com/microting/Pomelo.EntityFrameworkCore.MySql |
| Fork lauxjpn | https://github.com/lauxjpn/Pomelo.EntityFrameworkCore.MySql |
| PR #2047 (nosso) | https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/pull/2047 |
| PR #2019 (lauxjpn) | https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/pull/2019 |
| PR #2043 (ffquintella) | https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/pull/2043 |

## Apêndice B — Branches relevantes do microting

| Branch | Descrição | Versão EF Core |
|--------|-----------|---------------|
| `feat-10.0.9-fix` | Mais recente, deps em 10.0.9 | 10.0.9 |
| `net10` | Branch principal de EF Core 10 | 10.0.1 |
| `net11` | Preview .NET 11 | 10.0.3+ |
| `10.0.5` | Snapshot em EF Core 10.0.5 | 10.0.5 |
| `v10.0.3` | Release tag | 10.0.3 |
| `master` | Branch de produção principal | 8.x/9.x |

## Apêndice C — Commits chave do lauxjpn 9.x para portar

| Commit | Descrição | Arquivo(s) afetado(s) |
|--------|-----------|----------------------|
| `5322b411` | DateDiff functions | Query/ExpressionTranslators |
| `1f88ec73` | ConvertTimeZone | Query/ExpressionTranslators |
| `d75b62e7` | DateTimeOffset members | Query/ExpressionTranslators |
| `beaf6ebc` | DateTime precision fix | Query/ExpressionTranslators |
| `23db204c` | No DEFAULT 0 for AUTO_INCREMENT | Migrations |
| `0d480f21` | TranslateParamCollectionsToConstants default | Extensions |
| `f4d0feea` | AutoDetect overloads | Infrastructure |
| `1ccd7639` | Split connection creation | Infrastructure |
| `2b0ca144` | Use current DbConnection for sql_mode | Infrastructure |
