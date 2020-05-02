Integration and Performance Tests
================================

**Configuring the Database**

1. Configure your MySQL database by opening the `config.json.example` file, specifying the connection string and saving the changed file as `config.json`.
2. Run the `scripts/rebuild.sh` script on Linux or the `scripts/rebuild.ps1` script on Windows to rebuild all migrations. Any time you make changes to database models, run the rebuild script again.

**Running Integration Tests**

1. Ensure that you configured the database (see previous paragraph).
2. Run `dotnet test`. This will execute all tests in the Tests/ directory.

**Running Performance Tests**

1. Configure the Database
2. Run `dotnet run`. This will start a .NET Core MVC API application on "http://localhost:5000".

Methods:

`GET  /api/async` and `GET /api/sync` return the most recent 10 posts.

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

The `scripts` directory contains load testing scripts. These scripts require that the  [Vegeta](https://github.com/tsenart/vegeta/releases) binary is installed and accessible in your PATH. Here are examples of how to call the load testing scripts:
```
# by default, runs 50 async queries per second for 5 seconds
./stress.sh     # bash for linux
./stress.ps1    # powershell for windows

# runs 100 async queries per second for 10 seconds on linux
./stress.sh 100 10s async

# run 50 sync queries per second for 1 minute on windows
./stress.ps1 50 1m sync
```
