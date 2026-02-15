Functional Tests
================================

**Configuring the Database**

The functional tests are intended to be run against **MySQL 8.0+** or an equivalent **MariaDB** version (see the main README for tested versions).

Configure your MySQL database by opening the `config.json.example` file, specifying the connection string and saving the changed file as `config.json`.

**Running Functional Tests**

1. Ensure that you configured the database (see previous paragraph).
2. Run `dotnet test`. This will execute all tests in the project.