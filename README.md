# Description 

This repository at the moment is a poc. The name is misleading, I know. 

It contains 
 - a dotnetcore 3.1 api boilerplate.
 - a unit test project
 - an integration test project
 - an e2e test project
 - a template (backup) of sql server database
 
It contains a series of docker-compose yaml to:
 - local development  (docker-compose.yml)
   - start up api + start up sql server with restored initial database
 - build and test (docker-compose-testapi)
   - unit tests + integration tests (+ sql server startup) + code coverage report
   - e2e typed client code coverage report (start up api, db, run e2e tests)
 - load tests (docker-compose-loadtests.yml)
   - launch wrk against api+db in docker compose
 - profile memory (docker-compose-profiling.yml)
   - attach profiler to the api, wrk, save report 

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

 - docker-compose build
 - docker-compose up
 - docker-compose down

### load tests 
 
 - docker-compose build
 - docker-compose up
 - docker-compose -f .\docker-compose-loadtests.yml up --build wrk6sync 
 - docker-compose -f .\docker-compose-loadtests.yml up --build wrk12sync 
 - docker-compose -f .\docker-compose-loadtests.yml up --build wrk6async 
 - docker-compose -f .\docker-compose-loadtests.yml up --build wrk12async 
  
### profiling 

 - docker-compose -f ./docker-compose-profiling.yml build
 - docker-compose -f ./docker-compose-profiling.yml up api

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
            dockerfile: Dockerfile.UnitTests
        volumes:
            - ./results:/app/results
    integration:
        image: friendapi-integration-tests
        container_name: friendapi-integration-tests
        build:
            context: .
            dockerfile: Dockerfile.IntegrationTests
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
            dockerfile: Dockerfile.E2eTests
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
            dockerfile: Dockerfile.ReportGenerator
        volumes:
            - ./results:/app/results
    api:
        image: friendapi
        container_name: friendapi
        build:
            context: .
            dockerfile: Dockerfile
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

## Load tests - wrk
 
 - build docker-compose
   - docker-compose build
 - startup api & db
   - docker-compose up
 - launch wrk 
   - docker-compose -f .\docker-compose-loadtests.yml up --build wrk6sync 
     - Requests/sec:     85.13
     - 855 requests
   - docker-compose -f .\docker-compose-loadtests.yml up --build wrk12sync
     - Requests/sec:      1.29
     - 13 requests
   - docker-compose -f .\docker-compose-loadtests.yml up --build wrk6async  
     - Requests/sec:    183.22
     - 1847 requests
   - docker-compose -f .\docker-compose-loadtests.yml up --build wrk12async  
     - Requests/sec:    258.09
     - 2598 requests

### Code changed

 - Repository
```csharp
  public interface IFriendsRepository
	{
		Task<List<Friend>> ListAsync();
		List<Friend> List();
    //...
	}
  public class FriendsRepository : IFriendsRepository
	{
    //...
    public async Task<List<Friend>> ListAsync()
    {
      return await _dbContext.Friends.ToListAsync();
    }
    public List<Friend> List()
    {
      return _dbContext.Friends.ToList();
    }
    //...
  }
```

 - controller

```csharp
    [HttpGet(Routes.Friends)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Friend>>> GetAsync(bool sync)
    {
        if (sync)
            return Ok(_friendsRepository.List());

        var result = await _friendsRepository.ListAsync();
        return Ok(result);
    }
```

### Load test compose yaml file

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
    wrk6async:
        image: skandyla/wrk
        container_name: wrk6async 
        networks:
            - apinet
        command: http://api/friends -t6 -c10 -d10s --latency 
    wrk12async:
        image: skandyla/wrk
        container_name: wrk12async  
        networks:
            - apinet
        command: http://api/friends -t12 -c50 -d10s --latency 
    wrk6sync:
        image: skandyla/wrk
        container_name: wrk6sync 
        networks:
            - apinet
        command: http://api/friends?sync=true -t6 -c10 -d10s --latency 
    wrk12sync:
        image: skandyla/wrk
        container_name: wrk12sync
        networks:
            - apinet
        command: http://api/friends?sync=true -t12 -c50 -d10s --latency
networks:
    dbnet:
        name: dbnet
    apinet:
        name: apinet
        

```

### Log

```powershell
PS C:\Github\dotnetcore-api-template> docker-compose -f .\docker-compose-loadtests.yml up --build wrk6sync              
WARNING: Found orphan containers (wrk6, wrk12) for this project. If you removed or renamed this service in your compose file, you can run this command with the --remove-orphans flag to clean it up.
Creating wrk6sync ... done                                                                                              
Attaching to wrk6sync
wrk6sync      | Running 10s test @ http://api/friends?sync=true
wrk6sync      |   6 threads and 10 connections
wrk6sync      |   Thread Stats   Avg      Stdev     Max   +/- Stdev
wrk6sync      |     Latency   157.26ms  351.76ms   1.89s    89.75%
wrk6sync      |     Req/Sec    25.46     12.40    60.00     60.25%
wrk6sync      |   Latency Distribution
wrk6sync      |      50%   36.20ms
wrk6sync      |      75%   59.63ms
wrk6sync      |      90%  509.52ms
wrk6sync      |      99%    1.76s
wrk6sync      |   855 requests in 10.04s, 193.93KB read
wrk6sync      |   Socket errors: connect 0, read 0, write 0, timeout 6
wrk6sync      | Requests/sec:     85.13
wrk6sync      | Transfer/sec:     19.31KB
wrk6sync exited with code 0
PS C:\Github\dotnetcore-api-template> docker-compose -f .\docker-compose-loadtests.yml up --build wrk12sync             
WARNING: Found orphan containers (wrk12, wrk6) for this project. If you removed or renamed this service in your compose file, you can run this command with the --remove-orphans flag to clean it up.
Creating wrk12sync ... done                                                                                             
Attaching to wrk12sync
wrk12sync     | Running 10s test @ http://api/friends?sync=true
wrk12sync     |   12 threads and 50 connections
wrk12sync     |   Thread Stats   Avg      Stdev     Max   +/- Stdev
wrk12sync     |     Latency   367.91ms  407.03ms 809.68ms   55.56%
wrk12sync     |     Req/Sec     6.00     13.14    40.00     88.89%
wrk12sync     |   Latency Distribution
wrk12sync     |      50%   40.60ms
wrk12sync     |      75%  789.50ms
wrk12sync     |      90%  809.68ms
wrk12sync     |      99%  809.68ms
wrk12sync     |   13 requests in 10.10s, 2.95KB read
wrk12sync     |   Socket errors: connect 0, read 0, write 0, timeout 4
wrk12sync     | Requests/sec:      1.29
wrk12sync     | Transfer/sec:     298.64B
wrk12sync exited with code 0
PS C:\Github\dotnetcore-api-template> docker-compose -f .\docker-compose-loadtests.yml up --build wrk6async             
WARNING: Found orphan containers (wrk6, wrk12) for this project. If you removed or renamed this service in your compose file, you can run this command with the --remove-orphans flag to clean it up.
Creating wrk6async ... done                                                                                             
Attaching to wrk6async
wrk6async     | Running 10s test @ http://api/friends
wrk6async     |   6 threads and 10 connections
wrk6async     |   Thread Stats   Avg      Stdev     Max   +/- Stdev
wrk6async     |     Latency    32.66ms   13.97ms 118.35ms   76.52%
wrk6async     |     Req/Sec    30.59      9.06    60.00     51.25%
wrk6async     |   Latency Distribution
wrk6async     |      50%   30.90ms
wrk6async     |      75%   38.17ms
wrk6async     |      90%   48.70ms
wrk6async     |      99%   83.19ms
wrk6async     |   1847 requests in 10.08s, 418.46KB read
wrk6async     | Requests/sec:    183.22
wrk6async     | Transfer/sec:     41.51KB
wrk6async exited with code 0
PS C:\Github\dotnetcore-api-template> docker-compose -f .\docker-compose-loadtests.yml up --build wrk12async            
WARNING: Found orphan containers (wrk12, wrk6) for this project. If you removed or renamed this service in your compose file, you can run this command with the --remove-orphans flag to clean it up.
Creating wrk12async ... done                                                                                            
Attaching to wrk12async
wrk12async    | Running 10s test @ http://api/friends
wrk12async    |   12 threads and 50 connections
wrk12async    |   Thread Stats   Avg      Stdev     Max   +/- Stdev
wrk12async    |     Latency   182.79ms   35.00ms 336.47ms   71.05%
wrk12async    |     Req/Sec    22.34      9.48    40.00     66.00%
wrk12async    |   Latency Distribution
wrk12async    |      50%  182.98ms
wrk12async    |      75%  202.81ms
wrk12async    |      90%  225.95ms
wrk12async    |      99%  269.23ms
wrk12async    |   2598 requests in 10.07s, 588.61KB read
wrk12async    | Requests/sec:    258.09
wrk12async    | Transfer/sec:     58.47KB
wrk12async exited with code 0
```

## Profiling

 -  docker-compose -f ./docker-compose-profiling.yml build
 -  docker-compose -f ./docker-compose-profiling.yml up api


### Compose Yaml

 - docker-compose-profiling.yml


```yaml
version: "3"
services:
    api:
        image: friendapi-profiling
        container_name: friendapi-profiling
        build:
            context: .
            dockerfile: ./Dockerfile.Profiling
        volumes:
            - ./snapshots:/snapshots
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
### Dockerfile - Api

 - Dockerfile.Profiling
   - download and compile wrk
   - download and untar dottrace (TODO)
   - download and untar dotmemory (TODO)
   - build and publish api
   - copy tools and entrypoint
   - exec entrypoint

```dockerfile
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /tools/wrk

RUN apt-get update && apt-get install build-essential libssl-dev git -y
RUN git clone https://github.com/wg/wrk.git /tools/wrk
RUN make

WORKDIR /tools/dottrace

RUN wget -qO- https://download.jetbrains.com/resharper/dotUltimate.2020.2.4/JetBrains.dotTrace.CommandLineTools.linux-x64.2020.2.4.tar.gz | tar xvz -C /tools/dottrace

WORKDIR /tools/dotmemory

RUN wget -qO- https://download.jetbrains.com/resharper/dotUltimate.2020.2.4/JetBrains.dotMemory.Console.linux-x64.2020.2.4.tar.gz | tar xvz -C /tools/dotmemory

WORKDIR /app

COPY ./src ./src

WORKDIR /app/src/FriendsApi.Host
RUN dotnet restore 
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal AS runtime

WORKDIR /tools
COPY --from=build /tools ./

WORKDIR /app

VOLUME /snapshots

COPY --from=build /app/src/FriendsApi.Host/out ./
COPY ./entrypoint-profiling.sh ./
COPY ./docs/post.lua /tmp

CMD /app/entrypoint-profiling.sh
# CMD /bin/bash
```

### Docker Api - Entrypoint

 - attach in background dotmemory profiler
 - launch 30s of GET to /friends route
 - launch 20s of POST (new friends) to /friends route
 - launch 40s of GET to /friends route (1k records now not 3)
 - save dump in /snapshots
 - exit

```bash
/tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots  --trigger-timer=30s --trigger-max-snapshots=5 &

sleep 10

/tools/wrk/wrk -t2 -c3 -d30s --latency http://127.0.0.1:80/friends  > /snapshots/wrk-results-1.txt
/tools/wrk/wrk -t1 -c1 -d20s --latency -s /tmp/post.lua http://127.0.0.1:80/friends  > /snapshots/wrk-results-2.txt
/tools/wrk/wrk -t2 -c3 -d40s --latency http://127.0.0.1:80/friends  > /snapshots/wrk-results-3.txt

kill $(ps aux | grep 'dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}')

sleep 10
```

