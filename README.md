# Docker Compose - Test Dotnetcore REST Api

## How to

### tests and code coverage 
 - docker-compose -f ./docker-compose-testapi.yml up unit integration e2e
 - docker-compose -f ./docker-compose-testapi.yml up coverage

### build tests - destroy tests containers
 - docker-compose -f ./docker-compose-testapi.yml build
 - docker-compose -f ./docker-compose-testapi.yml down

### local api up/build/downn (db+api)
 - docker-compose up
 - docker-compose down
 - docker-compose build

## Azure pipelines

```yaml

steps:
- bash: 'docker-compose -f ./docker-compose-testapi.yml up unit integration e2e'
  displayName: 'Docker Compose Test'

steps:
- bash: 'docker-compose -f ./docker-compose-testapi.yml up coverage'
  displayName: 'Docker Compose Coverage'

steps:
- task: PublishTestResults@2
  displayName: 'Publish Test Results **/*.trx'
  inputs:
    testResultsFormat: VSTest
    testResultsFiles: '**/*.trx'
    mergeTestResults: true

steps:
- task: PublishCodeCoverageResults@1
  displayName: 'Publish code coverage from **/Cobertura.xml'
  inputs:
    codeCoverageTool: Cobertura
    summaryFileLocation: '**/Cobertura.xml'
```


## Compose - YAML

### Test api compose yaml

```yaml
version: "3"
services:
    unit:
        image: friendapi-unit-tests
        container_name: friendapi-unit-tests
        build:
            context: .
            dockerfile: ./Dockerfile.UnitTests
        volumes:
            - ./results:/app/results
    integration:
        image: friendapi-integration-tests
        container_name: friendapi-integration-tests
        build:
            context: .
            dockerfile: ./Dockerfile.IntegrationTests
        volumes:
            - ./results:/app/results    
        depends_on:
            - db  
        networks:
            - dbnet
        environment:
            - HOST_DB=db
    e2e:
        image: friendapi-e2e-tests
        container_name: friendapi-e2e-tests
        build:
            context: .
            dockerfile: ./Dockerfile.E2eTests
        volumes:
            - ./results:/app/results    
        depends_on:
            - db  
            - api
        networks:
            - apinet
    coverage:
        image: friendapi-reportgenerator
        container_name: friendapi-reportgenerator
        build:
            context: .
            dockerfile: ./Dockerfile.ReportGenerator
        depends_on:
            - unit
            - integration 
        volumes:
            - ./results:/app/results
    api:
        image: friendapi
        container_name: friendapi
        build:
            context: .
            dockerfile: ./Dockerfile
        networks:
            - dbnet
            - apinet
        depends_on:
            - db  
        environment:
            - ASPNETCORE_ENVIRONMENT=compose
        ports:    
            - "5001:80"        
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
    apinet:
        name: apinet    
```

### Api compose local development

```yaml
version: "3"
services:
    api:
        image: friendapi
        container_name: friendapi
        build:
            context: .
            dockerfile: ./Dockerfile
        networks:
            - dbnet
        depends_on:
            - db  
        environment:
            - ASPNETCORE_ENVIRONMENT=compose
        ports:    
            - "5001:80"     
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


## Dockerfiles

### Database

```dockerfile
FROM mcr.microsoft.com/mssql/server:2017-latest

RUN mkdir /work
COPY /friends.tar.gz /work
COPY /restore.sql /work
COPY /restore.sh /work

WORKDIR /work
RUN tar -xzvf ./friends.tar.gz -C ./
RUN chmod 666 friends.bak
RUN chmod 755 restore.sh

EXPOSE 1433
RUN ./restore.sh restore.sql
```

### Api - local development - e2e tests in build pipeline

```dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY . .

WORKDIR /app/src/FriendsApi.Host
RUN dotnet restore 
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/src/FriendsApi.Host/out ./

ENTRYPOINT ["dotnet", "FriendsApi.Host.dll"]
```

### Unit tests

```dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app

VOLUME /app/results

COPY . .

WORKDIR /app/tests/FriendsApi.UnitTests/

ENV ASPNETCORE_ENVIRONMENT=compose

CMD  dotnet test ./FriendsApi.UnitTests.csproj --configuration Release --logger "trx;LogFileName=/app/results/testresults-unittests.trx" /p:CollectCoverage=true  /p:CoverletOutputFormat=Cobertura /p:CoverletOutput=/app/results/results-unittests.xml /p:Include="[FriendsApi*]*"

```

### Integration tests 

```dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app

VOLUME /app/results

COPY . .

WORKDIR /app/tests/FriendsApi.SelfHostedTests/

ENV ASPNETCORE_ENVIRONMENT=compose

CMD  dotnet test ./FriendsApi.SelfHostedTests.csproj --configuration Release --logger "trx;LogFileName=/app/results/testresults-selfhostedtests.trx" /p:CollectCoverage=true  /p:CoverletOutputFormat=Cobertura /p:CoverletOutput=/app/results/results-selfhostedtests.xml /p:Include="[FriendsApi*]*"

```

### E2e Tests

```dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app

VOLUME /app/results

COPY . .

WORKDIR /app/tests/FriendsApi.SpecFlowTests/

ENV ASPNETCORE_ENVIRONMENT=compose

CMD  dotnet test ./FriendsApi.SpecFlowTests.csproj --configuration Release --logger "trx;LogFileName=/app/results/testresults-specflowtests.trx" /p:CollectCoverage=true /p:ExcludeByAttribute="" /p:Include="[*]*"  /p:CoverletOutputFormat=Cobertura /p:CoverletOutput=/app/results/results-specflowtests.xml
# /p:Include="[FriendsApi.SpecFlowTests.Proxies*]*"

```

### Code coverage report generator

```dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /app
VOLUME /app/results

COPY . .

RUN dotnet tool install --tool-path /app/tools dotnet-reportgenerator-globaltool
CMD /app/tools/reportgenerator -reports:/app/results/results*.xml -targetdir:/app/results -reporttypes:HtmlInline_AzurePipelines\;Cobertura  -assemblyfilters:+FriendsApi* 
#classfilters:+*\;-*.Proxies.*
```


## Links

 - [POC latest build](TODO)
 - [POC Final pipeline](TODO)
 
 
 - [report generator](https://github.com/danielpalme/ReportGenerator)
 - [coverlet issues](https://github.com/coverlet-coverage/coverlet/issues)
 - [coverlet](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/MSBuildIntegration.md)
 - [Docker Compose Wait for Dependencies](https://www.datanovia.com/en/courses/docker-compose-wait-for-dependencies/)
 - [compose - wait for exit](https://github.com/docker/compose/issues/5007)
 - [compose - start up order](https://docs.docker.com/compose/startup-order/)
 - [compose file - volumes](https://docs.docker.com/compose/compose-file/#volumes)
 - [mount volume in compose](https://stackoverflow.com/questions/38228386/mount-a-volume-in-docker-compose-how-is-it-done)
 - [Understanding docker -v command](https://stackoverflow.com/questions/32269810/understanding-docker-v-command)
 - [volume example](https://docs.docker.com/engine/reference/builder/#volume)
 - [docker - volume cli](https://docs.docker.com/engine/reference/commandline/volume_create/)
 - [dockerfile - VOLUME](https://stackoverflow.com/questions/41935435/understanding-volume-instruction-in-dockerfile)
 - [compose - volumes](https://docs.docker.com/storage/volumes/)
 - [compose - dependant services](https://stackoverflow.com/questions/47615751/docker-compose-run-a-script-after-container-has-started)
 - [ASPNETCORE_ENVIRONMENT in test runner](https://github.com/microsoft/vstest/issues/669)
 - [environment variables in test runner](https://github.com/xunit/xunit/issues/857)
 - [docker build - environment variables](https://docs.docker.com/engine/reference/builder/)
 - [docker build - network](https://docs.docker.com/engine/reference/commandline/build/)
 path=%2Fdocker%2Fdocker-fbs-time-contacts-tests.Dockerfile)
 - [Dockerfile ENV](https://www.scottbrady91.com/Docker/ASPNET-Core-and-Docker-Environment-Variables)

## Issues

 - report generator doesn't find src code on azure pipeline
 - compose up coverage -> I'd like to wait unit integration and e2e but depend wait only start not end

## Local commands launched during writing

 - PS C:\VSTS\PLAYGROUND\testing-playground> docker-compose build
 - PS C:\VSTS\PLAYGROUND\testing-playground> docker-compose up api

 - C:\VSTS\PLAYGROUND\testing-playground\src\1-api>docker build -t friendapi -f Dockerfile .
 - C:\VSTS\PLAYGROUND\testing-playground\src\1-api>
   - docker build -t friendapi-integration-tests -f .\Dockerfile.IntegrationTests --network=dbnet .
   - docker run --rm -it --network=dbnet --name  friendapi-integration-tests friendapi-integration-tests 
   - docker build -t friendapi-unit-tests -f .\Dockerfile.UnitTests .
   - docker run --rm -it --name  friendapi-unit-tests friendapi-unit-tests 
   - docker build -t friendapi-viewresults -f .\Dockerfile.ViewResults .
   - docker run --rm -it --name  friendapi-viewresults friendapi-viewresults