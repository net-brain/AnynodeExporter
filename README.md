# AnynodeExporter
prometheus exporter for TE-Systems anynode
## Purpose
AnynodeExporter collects runtime values from TE-Systems anynode for monitoring the system with prometheus. You should be familiar with the concepts of prometheus, docker and grafana. Pleaser consider the following sources: https://prometheus.io/ https://www.docker.com/ https://grafana.com/
## Configuration
AnynodeExporter is developed as an ASP.net Core Application and therefore configured via appsettings. You can overwrite the appsettings file or use an appsettings.Production.json file in your runtime directory, or use environment variables. For the concepts in .NET 5.0 please read here: https://docs.microsoft.com/de-de/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0
### Config via apsettings file
An example for an appsettings.Production.json file:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Anynode": {
    "User": "anadmin",
    "Password": "secret",
    "Url": "https://192.168.178.10",
    "Period": 30
  }
}
```
### Config via environment variables
For setting the configuration via environment variables all variables must start with the prefix ae_. 
```
setx ae_Anynode__User anadmin /M
setx ae_Anynode__Password secret /M
setx ae_Anynode__Url https://192.168.178.10 /M
```
