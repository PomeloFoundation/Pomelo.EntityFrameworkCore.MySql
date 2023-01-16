## About

_Pomelo.EntityFrameworkCore.MySql.Json.Microsoft_ adds JSON support for `System.Text.Json` (the Microsoft JSON stack) to [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).

## How to Use

```csharp
optionsBuilder.UseMySql(
    connectionString,
    serverVersion,
    options => options.UseMicrosoftJson())
```

## Related Packages

* [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql)
* [System.Text.Json](https://www.nuget.org/packages/System.Text.Json)

## License

_Pomelo.EntityFrameworkCore.MySql.Json.Microsoft_ is released as open source under the [MIT license](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE).

## Feedback

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).