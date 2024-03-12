Integration and Performance Tests
================================

**Configuring the Database**

1. Configure your MySQL database by opening the `config.json.example` file, specifying the connection string and saving the changed file as `config.json`.
2. Run the `scripts/rebuild.ps1` PowerShell script on Linux or Windows to rebuild all migrations (for installing PowerShell, see [Install PowerShell on Windows, Linux, and macOS](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell)). Any time you make changes to the database models, run the rebuild script again.

**Running Integration Tests**

1. Ensure that you configured the database correctly (see previous paragraph).
2. Run `dotnet test`. This will execute all tests in the `Tests/` directory.

**Running Performance Tests**

1. Configure the database.
2. Run `dotnet run`. This will start a .NET Core MVC API application on "http://localhost:5000".

Methods:

`GET  /api/async` and `GET /api/sync` return the last ten posts.

`POST /api/async` and `POST /api/sync` create a new post. The request body should be `Content-Type: application/json` in the form:

```json
{
	"Title": "Test Blog",
	"Posts": [
		{
			"Title": "Post 1",
			"Content": "A great blog post"
		},
		{
			"Title": "Post 2",
			"Content": "An even better blog post"
		}
	]
}
```

The `scripts` directory contains load testing scripts. These scripts require that the  [Vegeta](https://github.com/tsenart/vegeta/releases) binary is installed and accessible in your PATH. Here are some examples of how to call the load testing scripts:
```
# runs 50 async queries per second for 5 seconds by default 
./stress.ps1

# runs 100 sync queries per second for 1 minute
./stress.ps1 100 1m sync
```
