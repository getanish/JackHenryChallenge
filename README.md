//Todo: Fill up readme


## Windows instructions

Create environment variable with key as `AppSettings:TwitterConfig:BearerToken` and Value as the bearer token.

Alternatively, uncomment the line 6 and populate it in the launchsettings.json

### Test coverage report

1. Run tests
```
dotnet test --collect:"XPlat Code Coverage"
```
2. Copy the `coverage.cobertura.xml` file locations in the output to 
```
reportgenerator -reports:"<File location 1>";"<File location 2>" -targetdir:"coveragereport" -reporttypes:Html
```


Test coverage report is available in coveragereport/index.html
