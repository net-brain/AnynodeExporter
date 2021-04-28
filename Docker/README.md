# Docker

We highly recommend running the anynode exporter under a container environment. In the following, the configuration is carried out using docker-compose. 
In addition to the Docker runtime, you must also install the program `docker-compose` on the target machine. You can finde the docker repository under 

 - Create a directory in which you copy the docker-compose.yaml and the  appsettings.Production.json.  
 - Configure the  appsettings.Production.json according to your requirements.
 - Start  the application with docker-compose up -d

Now the program should run under the URL http://localhost:9910/
You may stop the app with docker-compose down. For further operations consult the docker runtime and docker-compose documentation.
