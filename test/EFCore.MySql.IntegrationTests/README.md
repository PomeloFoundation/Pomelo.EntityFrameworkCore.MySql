Integration and Performance Tests
================================

**Configuring the Database**

You first must configure your MySql Database.  Open the `config.json.example` file, configure the connection string, and save it as `config.json`.

Next, you must rebuild migrations.  Run the `scripts/rebuild.sh` script on Linux or the `scripts/rebuild.ps1` script on Windows.  Any time you make changes to database models, run the rebuild script.

**Running Integration Tests**

1. Configure the Database
2. Run `dotnet test`
3. This will run through all of the tests in the Test/ directory.

**Running Performance Tests**

1. Configure the Database
2. Run `dotnet run`
3. This will start a .NET Core MVC API application running on http://localhost:5000

Methods:

`GET  /api/async` and `GET /api/sync` return the most recent 10 posts.

`POST /api/async` and `POST /api/sync` create a new post.  The request body should be `Content-Type: application/json` in the form:

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

The `scripts` directory contains load testing scripts.  These scripts require that the  [Vegeta](https://github.com/tsenart/vegeta/releases) binary is installed and accessible in your PATH.  Here are examples of how to call the load testing scripts:
```
# by default, runs 50 async queries per second for 5 seconds
./stress.sh     # bash for linux
./stress.ps1    # powershell for windows

# runs 100 async queries per second for 10 seconds on linux
./stress.sh 100 10s async

# run 50 sync queries per second for 1 minute on windows
./stress.ps1 50 1m sync
```
