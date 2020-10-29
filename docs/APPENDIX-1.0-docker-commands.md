# SQL SERVER

## RUN - VOLUMES
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1433:1433 -d --volume mssqlsystem:/var/opt/mssql --volume mssqluser:/var/opt/sqlserver --name sql1433 mcr.microsoft.com/mssql/server:2017-latest

## RUN - NO VOLUMES
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1434:1434 -d --name sql1434 mcr.microsoft.com/mssql/server:2017-latest 

## EXEC - SQL SERVER CLI - sqlcmd
docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "SELECT Name FROM sys.Databases"

## EXEC - SQL SERVER CLI - backup db
docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "BACKUP DATABASE friends TO DISK='/tmp/friends.bak'"

## EXEC - ls directory
docker exec -it sql1433 ls /tmp/ -la  

## EXEC - tar.gz compress file
docker exec -it sql1433 tar -czvf /tmp/friends.tar.gz -C /tmp friends.bak

## EXEC - tar.gx extract file
docker exec -it sql1433 tar -xzvf /tmp/friends.tar.gz -C /var/opt/mssql/data

## CP - copy from container to host
docker cp sql1433:/tmp/friends.tar.gz C:\VSTS\PLAYGROUND\testing-playground\docs\

## CP - copy from host to container
docker cp C:\VSTS\PLAYGROUND\testing-playground\docs\friends.tar.gz sql1433:/tmp/

## EXEC - SQL SERVER CLI - restore db
docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "RESTORE DATABASE friends FROM DISK='/var/opt/mssql/data/friends.bak'"
