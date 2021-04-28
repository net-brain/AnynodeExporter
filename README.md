# AnynodeExporter
prometheus exporter for TE-Systems anynode
## Purpose
AnynodeExporter collects runtime values from TE-Systems anynode for monitoring the system with prometheus.
## Configuration
AnynodeExporter is developed as an ASP.net Core Application and therefore configured via appsettings. You can overwrite the appsettings file or use an appsettings.Production.json file in your runtime directory, or use environment variables.
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
