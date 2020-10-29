# Docker Compose
 - docker-compose (or kind where applicable) to create local/extra environments to run a full app including the service-owned persistence.Service dependencies can be mocked w/ ad-hoc compose service or be delegated to other environment service if read-only (eg. FSP or FDF).

## Links

 - [Overview of Docker Compose](https://docs.docker.com/compose/)
 - [Compose command-line reference](https://docs.docker.com/compose/reference/)
 - [Overview of docker-compose CLI](https://docs.docker.com/compose/reference/overview/)
 - [Compose file version 3 reference](https://docs.docker.com/compose/compose-file/)
 - [Get started with Docker Compose](https://docs.docker.com/compose/gettingstarted/)
 - [Github - Docker Compose](https://github.com/docker/compose)
 - [Essential Docker for ASP.NET Core MVC](https://www.apress.com/gp/book/9781484227770)
 - [Using docker named volumes to persist databases in SQL Server](https://dbafromthecold.com/2019/03/21/using-docker-named-volumes-to-persist-databases-in-sql-server/)

## Ideas

 - Simple api project that use sql server
 - Unit tests using in memory db
 - integrations test using docker compose
 - Performance/Load tests? using docker compose?
 - Profiling memory using docker compose?

## Roadmap

 - DOCKER
   - docker sqlserver 
   - run docker
   - mount volume
   - connect via sql management studio
   - create friends database db/table etc...
   - stop/rerun/destory/recreate docker
     - I want to understand how reuse a database as "template"
 
 - API
   - create an hello world api
   - create a database/table friends
   - create a hello controller 
     - create a GET /hello/{id} that produce {"response" : "hello $firstname"}
   - unit test/in memory database is out of scope (nice to have)
 - create an integration test project (nunit)
   - use in the test the docker instance
 
 - PIPELINES
   - investigate docker/docker compose steps
  
