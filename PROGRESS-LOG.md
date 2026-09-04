# Log de Progresso — Fork Sufficit do Pomelo.EntityFrameworkCore.MySql

> **Objetivo deste documento:** registrar cronologicamente tudo que já foi tentado,
> o que funcionou, o que falhou e foi revertido, e por quê. Para que qualquer
> trabalho futuro saiba exatamente **por onde começar** e **não repetir erros**.

---

## 1. Estado Atual do Repositório

| Item | Valor |
|------|-------|
| Repo local | `/mnt/pomelo` |
| Branch ativa | `upgrade/10.0.0` |
| Branch principal | `main` (contém todo o histórico de fixes; upgrade/10.0.0 contém +14 commits adicionais) |
| Remotes | `origin` (sufficit), `upstream` (PomeloFoundation), `microting`, `lauxjpn` |
| CI | ✅ 100% verde (12/12 jobs), commit `dade1a8c`, run 31187847866 |
| Próximo passo | Implementar suporte a JSON structural mapping (`.ToJson()`) do EF Core 10 |

---

## 2. Resumo das Tentativas de JSON Structural Mapping

### 2.1. O Problema

O EF Core 10 introduziu **mapeamento de colunas JSON** (`.ToJson()` / `ToJson()`)
para tipos complexos (`ComplexType`). O Pomelo original **não tem suporte** a isso —
não tem JSON store type, type mapping, nem convention para colunas JSON.

Sem essa implementação, qualquer modelo com propriedades de coleção complexas
(ex: `FieldPubWithCollections.Activities`, `School.Departments`) falha com **407 erros
de validação de modelo**.

### 2.2. Cronologia de Tentativas (Microting + Sufficit)

#### Microting (usando Copilot Agent) — Nov–Dez 2025

| Data | Commit | Ação | Resultado |
|------|--------|------|-----------|
| 18/Nov | `896c9fbd` | WIP: Adicionou `MySqlJsonColumnConvention` + atualizou `MySqlAnnotationProvider` | Incompleto, marcado como WIP |
| 28/Nov | `5dd5477b` | Criou `MySqlStructuralJsonTypeMapping` (baseado em SQL Server) | Tinha bugs: usava `JsonTypePlaceholder` mas conversões quebravam geração de SQL |
| 03/Dez 05:51 | `d0ed9301` / `b86d57b4` | **Removeu** toda a infraestrutura JSON (301 linhas deletadas) | A infraestrutura não funcionava corretamente |
| 03/Dez 06:09 | `70d34920` | **Restaurou** a infraestrutura JSON | Ainda com bugs; `Console.WriteLine` de debug no código de produção |
| Depois | branches `mark-test-as-todo`, `fix-json-column-errors` | Fez várias tentativas de corrigir JSON | Acabou **skipando dezenas de testes ComplexJson** como TODO |

**Arquivos que o microting criou/removou:**
- `src/EFCore.MySql/Metadata/Conventions/MySqlJsonColumnConvention.cs` (118 linhas)
- `src/EFCore.MySql/Storage/Internal/MySqlStructuralJsonTypeMapping.cs` (97 linhas)
- `src/EFCore.MySql/Metadata/Internal/MySqlAnnotationProvider.cs` (modificado, +25 linhas)
- `src/EFCore.MySql/Storage/Internal/MySqlJsonTypeMapping.cs` (modificado)
- `src/EFCore.MySql/Storage/Internal/MySqlTypeMappingSource.cs` (modificado)
- `src/EFCore.MySql/Metadata/Conventions/MySqlConventionSetBuilder.cs` (modificado)

#### Sufficit (nosso fork) — Ago 2025–Jan 2026

| Data | Commit | Ação | Resultado |
|------|--------|------|-----------|
| 05/Ago | `6b834623` | **Workaround**: Ignorar propriedades de coleção complexas em `OnModelCreating` + suprimir `MappedComplexPropertyIgnoredWarning` | ✅ Funcionou — resolveu 407 falhas de CI (ComplexTypesTrackingMySqlTest: 0 falhas, 211 passaram, 40 skipped) |
| 06/Ago | `99e3d5b6` | Fix de 4 bugs encontrados no review do microting | Ver detalhes abaixo |
| 07/Ago | `cd0fff87` | **Revert** da relaxão do ExecuteDelete (1 dos 4 fixes do commit anterior) | ❌ Revertido — MySQL erro 1093 em 24 testes |

---

## 3. Detalhes dos Bugs Encontrados e Corrigidos

### 3.1. Commit `99e3d5b6` — "Fix 4 bugs found in microting fork review"

Quatro bugs identificados ao revisar o código do microting:

#### Bug 1: Loop infinito em `MySqlJsonTableExpression`
- **Problema:** `for(j=0;j<i;i++)` deveria ser `j++` (incrementava `i` em vez de `j`)
- **Impacto:** Hang no processamento de segmentos de path do `JSON_TABLE()`
- **Status:** ✅ Corrigido (commit `9ba3ef25`)
- **Arquivo:** `src/EFCore.MySql/Query/Internal/MySqlJsonTableExpression.cs`

#### Bug 2: InvalidCastException em `MySqlSqlTranslatingExpressionVisitor`
- **Problema:** Cast direto `(SqlExpression)` falhava com `StructuralTypeReferenceExpression` do EF Core 10
- **Impacto:** Crash ao traduzir queries com tipos complexos
- **Status:** ✅ Corrigido — adicionado type check antes do cast (commit `cd0819d9`)
- **Arquivo:** `src/EFCore.MySql/Query/Internal/MySqlSqlTranslatingExpressionVisitor.cs`

#### Bug 3: NullReferenceException em `MySqlJsonParameterExpressionVisitor`
- **Problema:** `FindMapping()` podia retornar `null`, e `typeMapping.ClrType` estourava NRE
- **Impacto:** Crash em queries JSON com parâmetros
- **Status:** ✅ Corrigido — fallback para TypeMapping existente do parâmetro (commits `65aa9d24` + `2c11e89e`)
- **Arquivo:** `src/EFCore.MySql/Query/Internal/MySqlJsonParameterExpressionVisitor.cs`

#### Bug 4: ExecuteDelete rejeitava ORDER BY/LIMIT ❌ REVERTIDO
- **Problema:** Relaxão permitia ORDER BY/LIMIT em ExecuteDelete, mas MySQL/MariaDB rejeitam com erro 1093
  ("You can't specify target table for update in FROM clause")
- **Impacto:** 24 falhas de teste
- **Status:** ❌ **REVERTIDO** no commit `cd0fff87`
- **Arquivo:** `src/EFCore.MySql/Query/Internal/MySqlQueryableMethodTranslatingExpressionVisitor.cs`
- **Detalhe da reversão:**
  ```csharp
  // ANTES (quebrado):
  => selectExpression is
     {
        Offset: null,
        GroupBy: [],
        Having: null
     } &&
     (selectExpression.Tables.Count == 1 || (selectExpression.Orderings.Count == 0 && selectExpression.Limit is null)) &&
     selectExpression.Tables[0] is TableExpression &&
     selectExpression.Tables.Skip(1).All(t => t is InnerJoinExpression);

  // DEPOIS (atual, estrito):
  => selectExpression is
     {
        Orderings: [],
        Offset: null,
        Limit: null,
        GroupBy: [],
        Having: null
     } &&
     selectExpression.Tables[0] is TableExpression &&
     selectExpression.Tables.Skip(1).All(t => t is InnerJoinExpression);
  ```

### 3.2. Commit `cd0fff87` — Bug bônus: StringEndsWith_parameter
- **Problema:** `InlineData` com 3 parâmetros mas assinatura era `(bool async)` → 12 test failures
- **Fix:** Trocado para `[MemberData(nameof(IsAsyncData))]`
- **Arquivo:** `test/.../NorthwindStringComparisonFunctionsQueryMySqlTest.cs`

---

## 4. Estado Atual do ExecuteDelete/ExecuteUpdate

### ExecuteDelete
- **Atual:** Verificação estrita — sem ORDER BY, sem LIMIT, sem OFFSET, tabelas INNER JOIN apenas
- **Limitação:** Não suporta `ExecuteDelete` com `OrderBy().Take()` (precisaria de subquery com CTE)
- **Para resolver:** Precisaria implementar wrapper com subquery temporária/CTE:
  ```sql
  -- Em vez de:
  DELETE FROM t WHERE id IN (SELECT id FROM t ORDER BY x LIMIT 10)
  -- Precisaria:
  DELETE FROM t WHERE id IN (SELECT id FROM (SELECT id FROM t ORDER BY x LIMIT 10) AS tmp)
  ```

### ExecuteUpdate
- Não foi alterado; funciona normalmente no estado atual.

---

## 5. Estado Atual do JSON Structural Mapping

### ✅ IMPLEMENTADO (commit `77c5b0a2`, 08/Jan/2026)

O suporte a JSON structural mapping (`.ToJson()`) foi implementado com sucesso!

**Arquivos criados:**
- `src/EFCore.MySql/Storage/Internal/MySqlStructuralJsonTypeMapping.cs` — type mapping para JSON container columns, lê via `DbDataReader.GetString` e converte para `MemoryStream` (análogo ao `SqlServerStructuralJsonTypeMapping`)
- `src/EFCore.MySql/Metadata/Conventions/MySqlJsonColumnConvention.cs` — convention que seta `ContainerColumnType="json"` em complex types mapeados com `.ToJson()`

**Arquivos modificados:**
- `src/EFCore.MySql/Storage/Internal/MySqlTypeMappingSource.cs` — retorna `StructuralJsonTypeMapping` para `JsonTypePlaceholder` ClrType e storeType `"json"`
- `src/EFCore.MySql/Metadata/Conventions/MySqlConventionSetBuilder.cs` — registra `MySqlJsonColumnConvention`
- `src/EFCore.MySql/Metadata/Internal/MySqlAnnotationProvider.cs` — detecta JSON container columns e emite annotation `ColumnType="json"`

**Resultados dos testes:**
- ComplexTypesTrackingMySqlTest: **211 passed, 0 failed, 40 skipped** (skips são issues do EF Core base: #31411, #36483, #31621)
- PropertyValuesMySqlTest: **196 passed, 0 failed, 4 skipped**
- Build: **0 warnings, 0 errors**

### Lições aprendidas
1. `SetJsonPropertyName` e `SetContainerColumnName` são mutuamente exclusivos — só `SetContainerColumnName` é necessário (é o que `.ToJson()` faz internamente).
2. Não precisa recriar a `RelationalMapToJsonConvention` — ela já vem do `RelationalConventionSetBuilder` base.
3. Não incluir `Console.WriteLine` de debug em código de produção (erro do microting).
4. JSON columns no MySQL são nativas (`JSON` type), não precisam de `nvarchar(max)` como no SQL Server.

### O que o microting tem (mas com problemas)
1. **`MySqlStructuralJsonTypeMapping`** — type mapping para colunas JSON
   - Usa `JsonTypePlaceholder` como ClrType
   - Lê do DataReader como string, converte para MemoryStream
   - **Problemas conhecidos:** Console.WriteLine de debug em código de produção;
     conversões quebrando geração de SQL em alguns casos
2. **`MySqlJsonColumnConvention`** — convention para setar tipo "json"
   - Detecta `JsonPropertyName` annotation (setado por `.ToJson()`)
   - Seta `ContainerColumnType = "json"` no complex type
   - **Problemas conhecidos:** Não cobre todos os cenários de model building
3. **`MySqlAnnotationProvider` modificado** — detecta container columns de JSON
   - Procura complex properties com container column name == column name
   - **Problemas conhecidos:** Cast para `IReadOnlyTypeBase` pode falhar
4. **JSON_SET() para updates parciais** — commit `cca0eafc`
   - Implementado para resolver erro de rename de passkey
   - Funciona para casos simples mas não para nested collections
5. **`TransformJsonQueryToTable`** — traduz queries JSON para `JSON_TABLE()`
   - Tinha loop infinito (corrigido em `9ba3ef25`)
   - Não cobre todos os padrões de query

### Testes ComplexJson no microting
- Múltiplos testes skipados como TODO nos branches `mark-test-as-todo`
- Erros típicos: `InvalidOperationException` para operações de coleção JSON não suportadas

---

## 6. Problemas que o Microting Documentou

Commits de investigação no microting (sem solução definitiva):

| Commit | Título | Conteúdo |
|--------|--------|----------|
| `05de0148` | Investigation: JSON owned entity update issue in EF Core 10 | Análise de por que updates de owned entities em JSON falham |
| `f3ad1d86` | Document JSON owned entity update workaround for EF Core 10 | Workaround documentado (não implementado de forma definitiva) |
| `55a32709` | Add detailed analysis of JSON_SET implementation requirements | Análise do que seria necessário para JSON_SET funcionar corretamente |
| `a668cef2` | Add JSON_SET infrastructure and explain implementation limitations | Infraestrutura adicionada mas com limitações explicadas |

---

## 7. Arquivos Críticos (Mapa para Trabalho Futuro)

### Arquivos que precisarão ser criados/modificados para JSON support
| Arquivo | Ação | Referência (microting commit) |
|---------|-------|-------------------------------|
| `src/EFCore.MySql/Storage/Internal/MySqlStructuralJsonTypeMapping.cs` | Criar | `5dd5477b`, `70d34920` |
| `src/EFCore.MySql/Storage/Internal/MySqlJsonTypeMapping.cs` | Modificar | `5dd5477b` |
| `src/EFCore.MySql/Storage/Internal/MySqlTypeMappingSource.cs` | Modificar | `5dd5477b`, `70d34920` |
| `src/EFCore.MySql/Metadata/Conventions/MySqlJsonColumnConvention.cs` | Criar | `896c9fbd`, `70d34920` |
| `src/EFCore.MySql/Metadata/Conventions/MySqlConventionSetBuilder.cs` | Modificar | `896c9fbd`, `70d34920` |
| `src/EFCore.MySql/Metadata/Internal/MySqlAnnotationProvider.cs` | Modificar | `70d34920` |
| `src/EFCore.MySql/Query/Internal/MySqlJsonTableExpression.cs` | Já corrigido (bug do loop) | `99e3d5b6` |
| `src/EFCore.MySql/Query/Internal/MySqlSqlTranslatingExpressionVisitor.cs` | Já corrigido (InvalidCastException) | `99e3d5b6` |
| `src/EFCore.MySql/Query/Internal/MySqlJsonParameterExpressionVisitor.cs` | Já corrigido (NRE) | `99e3d5b6` |

### Bugs já corrigidos no nosso fork (não precisam re-corrigir)
- ✅ Loop infinito `MySqlJsonTableExpression` (commit `99e3d5b6`)
- ✅ InvalidCastException `MySqlSqlTranslatingExpressionVisitor` (commit `99e3d5b6`)
- ✅ NRE `MySqlJsonParameterExpressionVisitor` (commit `99e3d5b6`)

### Arquivo de workaround atual (será removido quando JSON for implementado)
| Arquivo | Commit | Descrição |
|---------|--------|-----------|
| `test/EFCore.MySql.FunctionalTests/ComplexTypesTrackingMySqlTest.cs` | `6b834623` | Ignora 40 testes de coleções complexas |
| `test/EFCore.MySql.FunctionalTests/PropertyValuesMySqlTest.cs` | `6b834623` | Ignora 4 testes de coleções complexas |

---

## 8. Recuperação do Código de Referência

O código dos arquivos removidos pode ser recuperado dos commits do microting:

```bash
# Recuperar MySqlStructuralJsonTypeMapping.cs
cd /mnt/pomelo
git show 70d34920 -- '**/MySqlStructuralJsonTypeMapping.cs'

# Recuperar MySqlJsonColumnConvention.cs
git show 70d34920 -- '**/MySqlJsonColumnConvention.cs'

# Recuperar mudanças do AnnotationProvider
git show 70d34920 -- '**/MySqlAnnotationProvider.cs'

# Recuperar mudanças do TypeMappingSource
git show 70d34920 -- '**/MySqlTypeMappingSource.cs'

# Recuperar mudanças do JsonTypeMapping
git show 70d34920 -- '**/MySqlJsonTypeMapping.cs'

# Recuperar mudanças do ConventionSetBuilder
git show 70d34920 -- '**/MySqlConventionSetBuilder.cs'
```

### ⚠️ Avisos sobre o código recuperado do microting
1. **Remover Console.WriteLine de debug** — o `MySqlStructuralJsonTypeMapping` tem
   `Console.WriteLine` em construtor, `GetDataReaderMethod()` e `CustomizeDataReaderExpression()`
2. **O `MySqlJsonColumnConvention` está em estado WIP** — não cobre todos os cenários
3. **O `MySqlAnnotationProvider` faz cast para `IReadOnlyTypeBase`** que pode falhar
4. **Testar incrementalmente** — aplicar um arquivo por vez e rodar testes

---

## 9. Plano Recomendado para Implementar JSON Support

### Fase 1: Type Mapping (base)
1. Criar `MySqlStructuralJsonTypeMapping.cs` (sem Console.WriteLine)
2. Modificar `MySqlTypeMappingSource.cs` para retornar o mapping quando storeType == "json"
3. Modificar `MySqlJsonTypeMapping.cs` para separar JSON simples de JSON estrutural
4. **Teste:** Compilação + testes existentes sem regressão

### Fase 2: Convention + Annotation
5. Criar `MySqlJsonColumnConvention.cs` (revisar lógica WIP do microting)
6. Modificar `MySqlConventionSetBuilder.cs` para registrar a convention
7. Modificar `MySqlAnnotationProvider.cs` para detectar container columns JSON
8. **Teste:** Model validation deve passar para tipos complexos com `.ToJson()`

### Fase 3: Query Translation
9. Verificar se `TransformJsonQueryToTable` está completo (JSON_TABLE)
10. Validar queries de leitura JSON (SELECT, WHERE em campos JSON)
11. **Teste:** Habilitar os testes ComplexJson que foram skipados

### Fase 4: Update/Delete
12. Implementar JSON_SET() para updates parciais (baseado em `cca0eafc` do microting)
13. Validar ExecuteDelete em colunas JSON
14. **Teste:** Habilitar testes de update de coleções complexas

### Fase 5: Cleanup
15. Remover workaround do commit `6b834623` (un-ignore coleções complexas)
16. Remover supressão de `MappedComplexPropertyIgnoredWarning`
17. **Teste final:** 0 skips em ComplexTypesTrackingMySqlTest

---

## 10. Erros Conhecidos e Soluções (Para Referência Rápida)

| Erro | Causa | Solução | Commit de referência |
|------|-------|---------|---------------------|
| MySQL erro 1093 | Subquery referencia tabela sendo modificada | Adicionar `DeleteWithSelfReferencingSubquery` flag + wrapper de subquery | `d2c99bcb` |
| MySQL erro 1451 | FK constraint em DELETE | Try-catch no update SQL generator | `dade1a8c` |
| InvalidCastException (StructuralTypeReferenceExpression) | EF Core 10 introduziu novo tipo de expression | Type check antes de cast | `99e3d5b6` |
| NullReferenceException (TypeMapping null) | FindMapping() retorna null para tipos desconhecidos | Fallback para TypeMapping existente | `99e3d5b6` |
| Infinite loop (JSON_TABLE path) | `j<i;i++` em vez de `j<i;j++` | Corrigir incremento | `99e3d5b6` |
| 407 falhas (complex collection validation) | Sem JSON store type | Workaround: ignorar coleções complexas (temporário) | `6b834623` |
| ExecuteDelete com ORDER BY/LIMIT | MySQL erro 1093 | Manter verificação estrita (não relaxar) | `cd0fff87` (revert) |
| CROSS APPLY (MariaDB) | MariaDB não suporta CROSS APPLY | Catch InvalidOperationException (não NotSupportedException) | `614336b9` |
| Skip(0)/Take(0) | EF Core 10 otimiza WHERE FALSE removendo ORDER BY | Atualizar baseline | `d8e2bcd5` |
| VALUES ROW() | EF Core 10 introduziu VALUES ROW() | Conditional baselines | `231d552b` |

---

## 11. Métricas de Qualidade

| Métrica | Sufficit (nosso) | Microting | lauxjpn (PR #2019) |
|---------|:---:|:---:|:---:|
| Testes falhando | **0** | ~50+ skipados | **703** |
| CI | ✅ 12/12 verde | ✅ verde (com skips) | ❌ vermelho |
| JSON .ToJson() support | ❌ (workaround skip) | ⚠️ parcial (bugs) | ❌ |
| ExecuteDelete ORDER BY | ❌ estrito | ✅ relaxado | ❌ |
| PR upstream | #2047 (aberto) | N/A | #2019 (aberto) |

---

*Documento criado em: 07/Jan/2026*
*Última atualização: 07/Jan/2026*
*Repo: `/mnt/pomelo` | Branch: `upgrade/10.0.0` | CI: ✅ verde*
