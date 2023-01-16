## About

_Pomelo.EntityFrameworkCore.MySql.NetTopologySuite_ adds spatial support for [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) to [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).

## How to Use

```csharp
optionsBuilder.UseMySql(
    connectionString,
    serverVersion,
    options => options.UseNetTopologySuite())
```

## Related Packages

* [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql)
* [NetTopologySuite](https://www.nuget.org/packages/NetTopologySuite)

## License

_Pomelo.EntityFrameworkCore.MySql.NetTopologySuite_ is released as open source under the [MIT license](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/blob/master/LICENSE).

## Feedback

Bug reports and contributions are welcome at our [GitHub repository](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql).