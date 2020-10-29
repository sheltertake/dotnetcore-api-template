# Azure Pipeline - Console App uses docker sql

 - I want to create a build pipeline
 - I want to build the console app
 - I want to run docker sqlserver in the selfhosted agent
 - I want to use volumes (useless?)
 - I want to unzip Friend mdf/ldf and copy into sql server data folder
 - I want to run the console app
 - I want to see the output

## Links

 - [Error “The input device is not a TTY”](https://stackoverflow.com/questions/43099116/error-the-input-device-is-not-a-tty)
 - [The test pipeline](TODO)

## First Attempt - QUERY select * from friends in Azure Pipeline

 - Run in azure pipeline agent the sequence tested in README-1-1-docker.primer

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1433:1433 -d --name sql1433 mcr.microsoft.com/mssql/server:2017-latest

docker cp $(Build.SourcesDirectory)/docs/friends.tar.gz sql1433:/tmp/
docker exec sql1433 ls /tmp -la

docker exec sql1433 tar -xzvf /tmp/friends.tar.gz -C /var/opt/mssql/data
docker exec sql1433 ls /var/opt/mssql/data -la

docker exec sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "RESTORE DATABASE friends FROM DISK='/var/opt/mssql/data/friends.bak'"
docker exec sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "SELECT Name FROM sys.Databases"
docker exec sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "USE friends; select * from friends"
```

ERROR!!!

```bash

-rw-r--r-- 1 1001  121 335432 Oct 28 07:20 friends.tar.gz
tar: /var/opt/mssql/data: Cannot open: No such file or directory

```

## Second Attempt - The Azure pipeline user is not root

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=yourStrong1234!Password' -p 1433:1433 -d --name sql1433 mcr.microsoft.com/mssql/server:2017-latest

docker cp $(Build.SourcesDirectory)/docs/friends.tar.gz sql1433:/tmp/
docker exec sql1433 ls /tmp -la

docker exec sql1433 tar -xzvf /tmp/friends.tar.gz -C /tmp
docker exec sql1433 ls /tmp -la

sleep 40

docker exec sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "RESTORE DATABASE friends FROM DISK='/tmp/friends.bak'"
docker exec sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "SELECT Name FROM sys.Databases"
docker exec sql1433 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "USE friends; select * from friends"
```

```bash
drwxrwxrwt 1 root root    4096 Oct 28 07:36 .
drwxr-xr-x 1 root root    4096 Oct 28 07:36 ..
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .ICE-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .Test-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .X11-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .XIM-unix
drwxrwxrwt 2 root root    4096 Sep  2 21:49 .font-unix
-rw-r----- 1 root root 2998272 Oct 28 06:04 friends.bak
-rw-r--r-- 1 1001  121  335432 Oct 28 07:35 friends.tar.gz
Processed 352 pages for database 'friends', file 'friends' on file 1.
Processed 3 pages for database 'friends', file 'friends_log' on file 1.
RESTORE DATABASE successfully processed 355 pages in 0.091 seconds (30.477 MB/sec).
Name                                                                                                                            
--------------------------------------------------------------------------------------------------------------------------------
master                                                                                                                          
tempdb                                                                                                                          
model                                                                                                                           
msdb                                                                                                                            
friends                                                                                                                         

(5 rows affected)
Changed database context to 'friends'.
id          name                                                                                                                                                                                                                                                           
----------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
          1 Marco                                                                                                                                                                                                                                                          
          2 Silvio                                                                                                                                                                                                                                                         
          3 Luca                                                                                                                                                                                                                                                           

(3 rows affected)

Finishing: Bash Script

```