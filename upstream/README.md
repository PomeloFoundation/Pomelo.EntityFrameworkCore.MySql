# Entity Framework Core Upstream Reference

This directory contains a copy of [EntityFrameworkCore](https://github.com/aspnet/EntityFrameworkCore) SQL Server projects with the following changes:

- `SqlServer` Symbols have been renamed to `MySql`
- The `Microsoft.EntityFrameworkCore.SqlServer` namespace has been renamed to `Pomelo.EntityFrameworkCore.MySql`

Code in this directory is generated using [AutoEF/CodeGen](https://github.com/AutoEF/CodeGen).  It is meant to be used as a reference when developing this provider library.  All of the methods still contain SQL Server code, which must be ported to MySql code and implemented in the provider library.
