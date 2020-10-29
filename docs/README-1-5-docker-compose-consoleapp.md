# Docker compose

## Links

 - [Get started with Docker Compose](https://docs.docker.com/compose/gettingstarted/)
 - [docker-compose build](https://docs.docker.com/compose/reference/build/)
 - [Overview of docker-compose CLI](https://docs.docker.com/compose/reference/overview/)
 - [docker-compose up vs docker-compose up --build vs docker-compose build --no-cache](https://stackoverflow.com/questions/39988844/docker-compose-up-vs-docker-compose-up-build-vs-docker-compose-build-no-cach)
 - [EXPOSE](https://docs.docker.com/engine/reference/builder/)
 - [Network settings](https://docs.docker.com/engine/reference/run/#network-settings)
 - [The case of Windows line-ending in bash-script](https://techblog.dorogin.com/case-of-windows-line-ending-in-bash-script-7236f056abe)
 - [connection string for sqlserver in Docker container](https://stackoverflow.com/questions/45712122/connection-string-for-sqlserver-in-docker-container)
 - [What is the best way to get a docker container to call itself via HTTP](https://stackoverflow.com/questions/53338268/what-is-the-best-way-to-get-a-docker-container-to-call-itself-via-http)
 - [Resolve container name in extra_hosts option in docker-compose](https://stackoverflow.com/questions/47577490/resolve-container-name-in-extra-hosts-option-in-docker-compose)
 - [Using an Environment Variable in a .NET Core Console App Hosted in Docker](https://stackoverflow.com/questions/51794900/using-an-environment-variable-in-a-net-core-console-app-hosted-in-docker)
 - [Docker not opening SQL connection](https://github.com/microsoft/mssql-docker/issues/435)
 - [How do I set hostname in docker-compose?](https://stackoverflow.com/questions/29924843/how-do-i-set-hostname-in-docker-compose)
 - [Environment variables](https://docs.docker.com/compose/environment-variables/)
 - [How to create & run .net Core Console App in docker](https://stackoverflow.com/questions/51769324/how-to-create-run-net-core-console-app-in-docker)
 - [docker-compose cheatsheet](https://devhints.io/docker-compose)
 - [Create a Dockerfile for an ASP.NET Core application](https://docs.docker.com/engine/examples/dotnetcore/)

## Pipeline

 -  docker-compose -f .\docker-compose-consoleapp.yml up console

In pipeline up only console. Otherwise the task didn't end.

## Howto

 -  docker-compose -f .\docker-compose-consoleapp.yml build  
 -  docker-compose -f .\docker-compose-consoleapp.yml up  

## The yaml

```yaml
version: "3"
services:
    console:
        image: consoleapp
        container_name: consoleapp
        build:
            context: ./src/0-consoleapp-use-dockersql/ConsoleAppDockerSql
            dockerfile: Dockerfile
        networks:
            - dbnet
        depends_on:
            - db  
        environment:
            - HOST_DB=db
    db:
        image: custom-mssql
        container_name: custom-mssql
        build:
            context: ./database
            dockerfile: Dockerfile
        networks:
            - dbnet
        ports:
            - "1433:1433"        
networks:
    dbnet:
        name: dbnet
        

```

## sh windows issue

Switching branch (I think only the first time) and deleting restore.sh from local filesystem caused a new download from git.
The end of line has been changed to windows EOF. This cause an error in docker (linux).
I added the following gitattributes.
To be tested with a clone from scratch.

```txt
* text=auto
*.sh text eol=lf
```

## Connection string "trick"

 - running dotnet run or dotnet dll in local machine -> "127.0.0.1"
 - running compose -> db (the service compose alias)

```csharp
            var hostdb = Environment.GetEnvironmentVariable("HOST_DB", EnvironmentVariableTarget.Process) ?? "127.0.0.1";
            Console.WriteLine("HOST DB {0}", hostdb);
            optionsBuilder.UseSqlServer(@"Server="+hostdb+";Initial Catalog=Friends;Persist Security Info=True;User ID=sa;Password=yourStrong1234!Password;MultipleActiveResultSets=True;Application Name=ConsoleAppDockerSql");
            
```

```yaml
    console:
        image: consoleapp
        container_name: consoleapp
        build:
            context: ./src/0-consoleapp-use-dockersql/ConsoleAppDockerSql
            dockerfile: Dockerfile
        networks:
            - dbnet
        depends_on:
            - db  
        environment:
            - HOST_DB=db
```


## Issues

### Understanding docker compose networking

 - https://github.com/sheltertake/compose-playground

 - http://localhost:5001/weatherforecast/self
 - http://localhost:5001/weatherforecast/other
 - http://localhost:5002/weatherforecast/self
 - http://localhost:5002/weatherforecast/other

```yaml
version: "3"
services:
    api1:
        image: api1
        container_name: api1
        build:
            context: ./api1
            dockerfile: Dockerfile
        networks:
            - api
        ports:
            - "5001:80"
    api2:
        image: api2
        container_name: api2
        build:
            context: ./api2
            dockerfile: Dockerfile
        networks:
            - api
        ports:
            - "5002:80"        
networks:
    api:
        name: api
```

 - use alias (servicename, not image name not container name. compose service name).

```csharp 

        [HttpGet("self")]
        public async Task<IEnumerable<WeatherForecast>> SelfAsync()
        {
             using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync("http://api1/weatherforecast");
                var content = await result.Content.ReadAsStreamAsync();
                var items = await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(content);
                return items;
            }
        }
        [HttpGet("other")]
        public async Task<IEnumerable<WeatherForecast>> OtherAsync()
        {
             using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync("http://api2/weatherforecast");
                var content = await result.Content.ReadAsStreamAsync();
                var items = await JsonSerializer.DeserializeAsync<List<WeatherForecast>>(content);
                return items;
            }
        }
```

### Solved with this yaml

```yaml
version: "3"
services:
    console:
        image: consoleapp
        container_name: consoleapp
        build:
            context: ./src/0-consoleapp-use-dockersql/ConsoleAppDockerSql
            dockerfile: Dockerfile
        networks:
            frontend:
                aliases:
                    - consoleapp
        depends_on:
            - db
        ports:
            - "6001:6001"    
    db:
        image: custom-mssql
        container_name: custom-mssql
        build:
            context: ./database
            dockerfile: Dockerfile
        networks:
            frontend:
                aliases:
                    - mssql
        ports:
            - "1433:1433"        
networks:
    frontend:
        name: frontend
        driver: bridge

```