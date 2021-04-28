# AnynodeExporter
prometheus exporter for TE-Systems anynode
## Purpose
AnynodeExporter collects runtime values from TE-Systems anynode for monitoring the system with prometheus.
## Configuration
AnynodeExporter is developed as an ASP.net Core Application and therefore configured via appsettings. you can use the appsettings file or use an appsettings.Production.json file in your runtime directory. 

An example for a appsettings.Production.json file could be the following:
'''
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
'''
