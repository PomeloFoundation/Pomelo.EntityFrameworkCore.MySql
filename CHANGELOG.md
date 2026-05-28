# Changelog

Manutenzione attiva di questo fork: [omarbaruzzo/Pomelo.EntityFrameworkCore.MySql](https://github.com/omarbaruzzo/Pomelo.EntityFrameworkCore.MySql)  
Derivato da [PomeloFoundation/Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).

Il formato è basato su [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
e questo progetto aderisce al [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [10.0.0] - 2026-05-28

Prima release del fork con supporto **Entity Framework Core 10** e **.NET 10**.  
Il provider ufficiale Pomelo 9.x non è compatibile con EF Core 10; questa linea è mantenuta per stack .NET 10 / EF 10.

### Added

- Target framework **net10.0** per `Pomelo.EntityFrameworkCore.MySql`.
- Supporto **Microsoft.EntityFrameworkCore 10.0.x** (pacchetti centrali: **10.0.7**).
- `VisitRightJoin` in `MySqlBoolOptimizingExpressionVisitor` (join destro introdotto in EF Core 10).
- `CHANGELOG.md` e documentazione di compatibilità per la linea 10.x del fork.

### Changed

- `Directory.Build.props`: `PomeloTargetFramework` / `EfCoreTargetFramework` da `net8.0` a **net10.0**.
- `Directory.Packages.props`: dipendenze EF e Microsoft.Extensions allineate a **10.0.7**; `MySqlConnector` **2.5.0**.
- `MySqlParameterBasedSqlProcessor`: pipeline su `Process(ParametersCacheDecorator)` (EF Core 10); `ProcessSqlNullability` aggiornato.
- `MySqlDbContextOptionsBuilderExtensions`: `UseParameterizedCollectionMode(ParameterTranslationMode.Constant)` al posto di `TranslateParameterizedCollectionsToConstants()`.
- `MySqlStringComparisonMethodTranslator`: prefisso parametri query `__`; accesso parametri via `QueryContext.Parameters`.
- `MySqlQueryCompilationContext`: costruttore precompilazione allineato a EF Core 10 (4 parametri).
- `Check.cs` (shared): messaggi di errore inline (API `AbstractionsStrings` non più invocabile come metodo in EF 10).
- `global.json`: SDK **10.0.103**.
- Versione assembly: **10.0.0** (da 9.0.1-servicing del upstream).

### Fixed

- `MissingMethodException` su `AbstractionsStrings.ArgumentIsEmpty` quando il provider EF 9 veniva usato con applicazioni **net10.0** / **EF Core 10**.
- Incompatibilità runtime tra assembly Pomelo compilati per EF 9 e host su EF 10.

### Notes

- Basato sul commit upstream `c44302e` (*feat: to ef-core-10*), con porting EF 10 completato nel fork.
- La suite completa di test funzionali Pomelo non è stata rieseguita in questa release; validazione su consumer **ZahelLibrary** / **IdentityProvider** (build + unit test).
- Per la matrice ufficiale Pomelo 9.x / EF 9, vedere il [README del Pomelo Foundation](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).
