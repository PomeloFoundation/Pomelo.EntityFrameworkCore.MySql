# Known Issues

This directory contains documentation for known issues and their fixes/workarounds.

## Issues

### JSON Owned Entity Updates (EF Core 10)

**Issue**: EF Core 10 bug where updating properties within JSON-mapped owned entities sends incorrect data

**Tracking**: [dotnet/efcore#37411](https://github.com/dotnet/efcore/issues/37411) 🔗

**Status**: ✅ Fixed in Pomelo — uses `JSON_SET()` for partial JSON updates

**Requirements**: MySQL 5.7.8+ or MariaDB 10.2.3+

**Documentation**: [json-owned-entity-updates.md](json-owned-entity-updates.md)
