## About

_Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft_ adds JSON support for `Newtonsoft.Json` (the Newtonsoft JSON/Json.NET stack) to [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).

## How to Use

```csharp
optionsBuilder.UseMySql(
    connectionString,
    serverVersion,
    options => options.UseNewtonsoftJson())
```

## Related Packages

* [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql)
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## License

_Pomelo.EntityFrameworkCore.MySql.Json.Newtonsoft_ is released as open source under the [MIT license](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/main/LICENSE).

## Feedback

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).