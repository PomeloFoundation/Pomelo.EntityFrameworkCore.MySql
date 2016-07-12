# Pomelo.Data.MySql

[![Build status](https://ci.appveyor.com/api/projects/status/r4cpdgpfwn2bh56n/branch/master?svg=true)](https://ci.appveyor.com/project/Kagamine/pomelo-data-mysql/branch/master)  [![Build status](https://travis-ci.org/PomeloFoundation/Pomelo.Data.MySql.svg)](https://travis-ci.org/PomeloFoundation/Pomelo.Data.MySql)

Contains MySQL implementations of the System.Data.Common(Both .NET Core and .NET Framework) interfaces.

You can access this library by using MyGet Feed: `https://www.myget.org/F/pomelo/api/v2/`

## Getting Started

After adding myget feed, you can put `Pomelo.Data.MySql` into your `project.json`, and the version should be `1.0.0`.

`MySqlConnection`, `MySqlCommand` and etc are included in the namespace `Pomelo.Data.MySql`. The following console application sample will show you how to use this library to write a record into MySQL database.

```C#
using Pomelo.Data.MySql;

namespace MySqlAdoSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var conn = new MySqlConnection("server=localhost;database=adosample;uid=root;pwd=yourpwd"))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("INSERT INTO `test` (`content`) VALUES ('Hello MySQL')", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
```

## Contribute

One of the easiest ways to contribute is to participate in discussions and discuss issues. You can also contribute by submitting pull requests with code changes.

## License

[MIT](https://github.com/CodeCombLLC/ADO.NET-MySQL/blob/master/LICENSE)
