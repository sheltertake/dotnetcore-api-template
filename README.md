# Docker Compose - Test Dotnetcore REST Api

## Initial requirement

docker-compose (or kind where applicable) to create local/extra environments to run a full app including the service-owned persistence.
Service dependencies can be mocked with ad-hoc compose service or be delegated to other environment service if read-only. 

## Tools used in this POC

 - Docker Desktop 
 - VSCode
   - Docker extension
 - Visual Studio 2019 
 - Sql Management Studio
 - NSwagStudio
 - dotnetcore 3.1 LTS
   - Serilog
   - Microsoft.EntityFrameworkCore.SqlServer 
   - Swashbuckle.AspNetCore

 - Test libraries
   - coverlet.msbuild
   - Microsoft.NET.Test.Sdk
   - Microsoft.AspNetCore.TestHost
   - Microsoft.AspNet.WebApi.Client
   - nunit
   - FluentAssertions
   - Moq
   - SpecFlow

## TODO/NiceToHave

 - Add other tests (unit tests are not implemented, integration tests against memory database - standard scenarios)
 - Add other type of tests
   - performance/load/profiling
   - fitness/arch tests (out of scope but interesting)
 - Add other backend scenario

 - Investigate better the depends_on option on compose. 
 - Investigate why these 2 builds produced different results 
   - [OK](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build/results?buildId=10&view=codecoverage-tab)
   - [KO](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build/results?buildId=9&view=codecoverage-tab)

## How to

### api tests and code coverage

 - docker-compose -f ./docker-compose-testapi.yml up unit integration
 - docker-compose -f ./docker-compose-testapi.yml up coverage

### e2e client code coverage 

 - docker-compose -f ./docker-compose-testapi.yml up e2e
 - docker-compose -f ./docker-compose-testapi.yml up coverage

### build tests - destroy tests containers

 - docker-compose -f ./docker-compose-testapi.yml build
 - docker-compose -f ./docker-compose-testapi.yml down

### local api up/build/down (db+api)

 - docker-compose up
 - docker-compose down
 - docker-compose build


## Azure pipelines

### Api tests (pipelines/azure-pipelines-api-tests-and-coverage.yml)

   - [Definition](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build?definitionId=3)
   - [Results](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build/results?buildId=14&view=results)
   - [Test results](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build/results?buildId=14&view=ms.vss-test-web.build-test-results-tab)
   - [Code coverage](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build/results?buildId=14&view=codecoverage-tab)

This pipeline launch Unit tests (no dependencies) and Integration Tests. Integration tests uses database so before starting tests compose startup the sql docker instance.

Thanks [Daniel](https://github.com/danielpalme) for the help about the conflict between the report generated in compose step and the auto-generated report by PublishCodeCoverageResults task. More info [here](https://github.com/danielpalme/ReportGenerator/wiki/Integration#attention).

```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

variables:
  disable.coverage.autogenerate: 'true'

steps:
- bash: 'docker-compose -f ./docker-compose-testapi.yml up unit integration'
  displayName: 'Docker Compose - Unit And Integration Test Against Docker Sql Server'

- bash: 'docker-compose -f ./docker-compose-testapi.yml up coverage'
  displayName: 'Docker Compose - Code Coverage Report'

- task: PublishTestResults@2
  displayName: 'Publish Test Results **/*.trx'
  inputs:
    testResultsFormat: VSTest
    testResultsFiles: '**/*.trx'
    mergeTestResults: true

- task: PublishCodeCoverageResults@1
  displayName: 'Publish code coverage from **/Cobertura.xml'
  inputs:
    codeCoverageTool: Cobertura
    summaryFileLocation: '**/Cobertura.xml'
    reportDirectory: '**/results'
```

### E2e Client Code Coverage:  (pipelines/azure-pipelines-e2e-coverage.yml)

   - [Definition](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build?definitionId=2)
   - [Results](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build/results?buildId=13&view=results)
   - [Test results](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build/results?buildId=13&view=ms.vss-test-web.build-test-results-tab)
   - [Code coverage](https://dev.azure.com/sheltertake/dotnetcore-api-template/_build/results?buildId=13&view=codecoverage-tab)

  This pipeline launch E2e Test project. Compose depends_on startup database and then the api. When api is up E2e runs tests against it. The report generator tool collect coverage only for the api client. 

  
```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

variables:
  disable.coverage.autogenerate: 'true'

steps:
- bash: 'docker-compose -f ./docker-compose-testapi.yml up e2e'
  displayName: 'Docker Compose - E2e Test only'

- bash: 'docker-compose -f ./docker-compose-testapi.yml up coverage'
  displayName: 'Docker Compose - E2e Client Coverage'

- task: PublishTestResults@2
  displayName: 'Publish Test Results **/*.trx'
  inputs:
    testResultsFormat: VSTest
    testResultsFiles: '**/*.trx'
    mergeTestResults: true

- task: PublishCodeCoverageResults@1
  displayName: 'Publish code coverage from **/Cobertura.xml'
  inputs:
    codeCoverageTool: Cobertura
    summaryFileLocation: '**/Cobertura.xml'
    reportDirectory: '**/results'
```

## Github actions  (wip)

 - https://github.com/sheltertake/dotnetcore-api-template
 - [Action wip](https://github.com/sheltertake/dotnetcore-api-template/runs/1328354542?check_suite_focus=true)

```yaml
# This is a basic workflow to help you get started with Actions

name: CI

# Controls when the action will run. Triggers the workflow on push or pull request
# events but only for the main branch
on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

# A workflow run is made up of one or more jobs that can run sequentially or in parallel
jobs:
  # This workflow contains a single job called "build"
  build:
    # The type of runner that the job will run on
    runs-on: ubuntu-latest

    # Steps represent a sequence of tasks that will be executed as part of the job
    steps:
      # Checks-out your repository under $GITHUB_WORKSPACE, so your job can access it
      - uses: actions/checkout@v2

      # Runs a single command using the runners shell
      - name: Docker Compose Test
        run: docker-compose -f ./docker-compose-testapi.yml up unit integration e2e

      # Runs a set of commands using the runners shell
      - name: Docker Compose Coverage
        run: docker-compose -f ./docker-compose-testapi.yml up coverage
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
 - [report generator integration](https://github.com/danielpalme/ReportGenerator/wiki/Integration#attention)
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

 - compose up coverage -> I'd like to wait unit integration and e2e but depend wait only start not end
