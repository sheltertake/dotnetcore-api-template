# Docker 

## Links

 - [Sql server with volumes](https://dbafromthecold.com/2019/03/21/using-docker-named-volumes-to-persist-databases-in-sql-server/)
 - [/opt/mssql-tools/bin/sqlcmd](https://stackoverflow.com/questions/48125560/creating-a-docker-image-with-sql-server-linux-and-a-database-of-my-own-databa)
 - [Attach mdf and recreate ldf](https://stackoverflow.com/questions/38787861/mssql-recreate-ldf-file-for-database)
 - [Backup and restore db from cmd](https://www.howtogeek.com/50295/backup-your-sql-server-database-from-the-command-line/)

## Docker - Sql Server 

 - run docker desktop (already installed)
 - vscode docker extensions (already installed)
 - cleanup images (optional - I want to start from scratch) - *powershell

```powershell
docker rmi -f $(docker images -a -q)
```

```powershell
docker images -a -q | % { docker image rm $_ -f }
```

 - run docker sqlserver (powershell)

```powershell
C:\VSTS\PLAYGROUND\testing-playground>docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1433:1433 -d --name sql1433 mcr.microsoft.com/mssql/server:2017-latest
Unable to find image 'mcr.microsoft.com/mssql/server:2017-latest' locally
2017-latest: Pulling from mssql/server
59ab41dd721a: Pulling fs layer
59ab41dd721a: Pull complete
57da90bec92c: Pull complete
06fe57530625: Pull complete
5a6315cba1ff: Pull complete
739f58768b3f: Pull complete
3a58fde0fc61: Pull complete
89b44069090d: Pull complete
99e39479713c: Pull complete
c0124e7c4de8: Pull complete
```

 - try to connect via sql management studio -> OK
 - create db friends
 - create table friends
 - seed table with 3 records
 - stop container
 - run container
 - check with sql management studio the db

```text

/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [id]
      ,[name]
  FROM [friends].[dbo].[Friends]

id          name
----------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
1           Marco
2           Silvio

(2 rows affected)


Completion time: 2020-10-27T12:40:41.0769492+01:00  
```
 - stop and remove stopped container
 - re run in powershell. the database doesn't exists anymore
 - stop and remove sqlserver

## Docker - Sql Server - Volumes

 - recreate sql server instance with volumes
 - https://dbafromthecold.com/2019/03/21/using-docker-named-volumes-to-persist-databases-in-sql-server/
 - create volumes

```powershell
docker volume create mssqlsystem
docker volume create mssqluser
```
 - run sqlserver with volumes

```powershell
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1433:1433 -d --volume mssqlsystem:/var/opt/mssql --volume mssqluser:/var/opt/sqlserver --name testsqlserver mcr.microsoft.com/mssql/server:2017-latest
```

 - open sql management
 - create database Friends
 - create and seed friends table
 - stop and purge container
 - rerun with volumes
 - OK it works

 - copy files from volumes

```powershell
PS C:\temp> docker ps
CONTAINER ID        IMAGE                                        COMMAND                  CREATED             STATUS              PORTS                    NAMES
44f7ef543fe5        mcr.microsoft.com/mssql/server:2017-latest   "/opt/mssql/bin/nonr…"   9 minutes ago       Up 9 minutes        0.0.0.0:1433->1433/tcp   testsqlserver

PS C:\temp> docker cp 44f7ef543fe5:/var/opt/mssql/data/Friends.mdf c:\temp\poc-docker
PS C:\temp> docker cp 44f7ef543fe5:/var/opt/mssql/data/Friends_log.ldf c:\temp\poc-docker
```

## Docker - Sql Server - CLI

 - cleanup containers
 - docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1433:1433 -d --name sql1433 mcr.microsoft.com/mssql/server:2017-latest
 - create db/table/populate
 - docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "SELECT Name FROM sys.Databases"

```powershell
PS C:\temp\poc-docker> docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "SELECT Name FROM sys.Databases"

Name                                                                                                                    
--------------------------------------------------------------------------------------------------------------------------------
master                                                                                                                  
tempdb                                                                                                                  
model                                                                                                                   
msdb                                                                                                                    
friends                                                                                                                 

(5 rows affected)
```

## Docker - Sql Server - CLI Backup Database
 
 - docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "BACKUP DATABASE friends TO DISK='/tmp/friends.bak'"


```powershell
PS C:\temp\poc-docker> docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "BACKUP DATABASE friends TO DISK='/tmp/friends.bak'"                                                              
Processed 352 pages for database 'friends', file 'friends' on file 1.
Processed 3 pages for database 'friends', file 'friends_log' on file 1.
BACKUP DATABASE successfully processed 355 pages in 0.080 seconds (34.667 MB/sec).
```
 - docker exec -it sql1433 ls /tmp/ -la

```powershell
PS C:\temp\poc-docker> docker exec -it sql1433 ls /tmp/ -la                                                             
total 2956
drwxrwxrwt 1 root root    4096 Oct 28 06:04 .
drwxr-xr-x 1 root root    4096 Oct 28 05:51 ..
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .ICE-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .Test-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .X11-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .XIM-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .font-unix
-rw-r----- 1 root root 2998272 Oct 28 06:04 friends.bak
```

## Docker - Sql Server - Tar.gz backup

 - docker exec -it sql1433 tar -czvf /tmp/friends.tar.gz -C /tmp friends.bak 

```powershell
PS C:\temp\poc-docker> docker exec -it sql1433 tar -czvf /tmp/friends.tar.gz -C /tmp friends.bak                        
friends.bak
PS C:\temp\poc-docker>  docker exec -it sql1433 ls /tmp/ -la                                                            
total 3284
drwxrwxrwt 1 root root    4096 Oct 28 06:41 .
drwxr-xr-x 1 root root    4096 Oct 28 05:51 ..
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .ICE-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .Test-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .X11-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .XIM-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .font-unix
-rw-r----- 1 root root 2998272 Oct 28 06:04 friends.bak
-rw-r--r-- 1 root root  335432 Oct 28 06:41 friends.tar.gz
PS C:\temp\poc-docker>   
```

## Docker - extract file from container

 - docker cp sql1433:/tmp/friends.tar.gz C:\VSTS\PLAYGROUND\testing-playground\docs\

## Destroy container and restart from scratch

 - stop and purge containers
 - re run a new docker sql
   - docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1433:1433 -d --name sql1433 mcr.microsoft.com/mssql/server:2017-latest 
 - copy tar.gz into containers
   - docker cp C:\VSTS\PLAYGROUND\testing-playground\docs\friends.tar.gz sql1433:/tmp/
   - docker exec -it sql1433 ls /tmp -la
 - untar tar.gz into /var/opt/mssql/data
   - docker exec -it sql1433 tar -xzvf /tmp/friends.tar.gz -C /var/opt/mssql/data
   - docker exec -it sql1433 ls /var/opt/mssql/data -la
 - restore db
   - docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "RESTORE DATABASE friends FROM DISK='/var/opt/mssql/data/friends.bak'"
 - check databases
   - docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "SELECT Name FROM sys.Databases"
 - check friends table
   - docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "USE friends; select * from friends"

```powershell
PS C:\temp\poc-docker> docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1433:1433 -d --name sql1433 mcr.microsoft.com/mssql/server:2017-latest                                                                         4f069b3973b845b36e63bd7cddd0188bb6d0df1d95a43955a3d86edca15fa4a0
PS C:\temp\poc-docker> docker cp C:\VSTS\PLAYGROUND\testing-playground\docs\friends.tar.gz sql1433:/tmp/                
PS C:\temp\poc-docker> docker exec -it sql1433 tar -xzvf /tmp/friends.tar.gz -C /var/opt/mssql/data                     
friends.bak
PS C:\temp\poc-docker> docker exec -it sql1433 ls /var/opt/mssql/data -la                                               
total 56120
drwxr-xr-x 2 root root     4096 Oct 28 07:05 .
drwxr-xr-x 6 root root     4096 Oct 28 07:05 ..
-rw-r----- 1 root root  2998272 Oct 28 06:04 friends.bak
-rw-r----- 1 root root  4194304 Oct 28 07:05 master.mdf
-rw-r----- 1 root root  2097152 Oct 28 07:05 mastlog.ldf
-rw-r----- 1 root root  8388608 Oct 28 07:05 model.mdf
-rw-r----- 1 root root  8388608 Oct 28 07:06 modellog.ldf
-rw-r----- 1 root root 14090240 Oct 28 07:05 msdbdata.mdf
-rw-r----- 1 root root   524288 Oct 28 07:06 msdblog.ldf
-rw-r----- 1 root root  8388608 Oct 28 07:05 tempdb.mdf
-rw-r----- 1 root root  8388608 Oct 28 07:05 templog.ldf
PS C:\temp\poc-docker> docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "RESTORE DATABASE friends FROM DISK='/var/opt/mssql/data/friends.bak'"                                            
Processed 352 pages for database 'friends', file 'friends' on file 1.
Processed 3 pages for database 'friends', file 'friends_log' on file 1.
RESTORE DATABASE successfully processed 355 pages in 0.111 seconds (24.985 MB/sec).
PS C:\temp\poc-docker> docker exec -it sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "USE friends; select * from friends"                                                                              
Changed database context to 'friends'.
id          name                                                                                                                                                                                                                                
----------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
          1 Marco                                                                                                                                                                                                                               
          2 Silvio                                                                                                                                                                                                                              
          3 Luca                                                                                                                                                                                                                                

(3 rows affected)
PS C:\temp\poc-docker>                                                                                                                                 
```