## About

_Microting.EntityFrameworkCore.MySql.Json.Newtonsoft_ adds JSON support for `Newtonsoft.Json` (the Newtonsoft JSON/Json.NET stack) to [Microting.EntityFrameworkCore.MySql](https://github.com/microting/Microting.EntityFrameworkCore.MySql).


## DISCLAIMER!!!
This is a fast moving version of [Microting.EntityFrameworkCore.MySql.Json.Newtonsoft](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql.Json.Newtonsoft) intented to follow .net release cycle closely!
There are no intentions for this package to implement new features. Feature requests needs to be directed at the originating [Github repository](https://github.com/microting/Microting.EntityFrameworkCore.MySql)

## How to Use

```csharp
optionsBuilder.UseMySql(
    connectionString,
    serverVersion,
    options => options.UseNewtonsoftJson())
```

## Related Packages

* [Microting.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql)
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)

## License

_Microting.EntityFrameworkCore.MySql.Json.Newtonsoft_ is released as open source under the [MIT license](https://github.com/microting/Microting.EntityFrameworkCore.MySql/blob/main/LICENSE).

## Feedback

Checkout the originating [Github repository](https://github.com/microting/Microting.EntityFrameworkCore.MySql) before filing any new issues.

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/microting/Microting.EntityFrameworkCore.MySql).