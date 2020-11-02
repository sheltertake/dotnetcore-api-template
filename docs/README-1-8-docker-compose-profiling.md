 - https://www.jetbrains.com/profiler/download/#section=commandline
 - https://www.jetbrains.com/dotmemory/download/#section=commandline
 - https://stackoverflow.com/questions/26982274/ps-command-doesnt-work-in-docker-container
   - apt-get update && apt-get install -y procps
sudo apt-get update
sudo apt-get install wrk
 - docker-compose -f .\docker-compose-profiling.yml build
 - docker-compose -f .\docker-compose-profiling.yml up api
 - attach shell

root@c52abfa729c8:/app# cd /tools/JetBrains.dotMemory.Console.linux-x64.2020.2.4
root@c52abfa729c8:/tools/JetBrains.dotMemory.Console.linux-x64.2020.2.4# ./dotMemory.sh attach 1 --save-to-dir=/snapshots

/tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots --trigger-timer=30s
/tools/dotmemory/dotMemory.sh attach 1 --save-to-dir=/snapshots --trigger-timer=30s

timeout 60s /tools/dotmemory/dotMemory.sh attach 1 --save-to-dir=/snapshots --trigger-timer=30s

/tools/dotmemory/dotMemory.sh attach 1 --save-to-dir=/snapshots --trigger-timer=30s --trigger-max-snapshots=2 & sleep 30 ; kill $!
docker kill --signal="SIGTERM" friendapi-profiling

docker kill --signal="SIGINT" friendapi-profiling

kill $(ps aux | grep '/usr/share/dotnet/dotnet /app/FriendsApi.Host.dll' | awk '{print $2}')

ps aux | grep 'dotnet'

ps aux | grep '[d]otnet'

kill $(ps aux | grep '[d]otnet' | awk '{print $2}')

kill $(ps aux | grep 'dotnet FriendsApi.Host.dll' | awk '{print $2}')
kill -9 $(ps aux | grep '/usr/share/dotnet/dotnet /app/FriendsApi.Host.dll' | awk '{print $2}')

docker run -it --rm --name friendapi-profiling --network="dbnet" -v=./snapshots:/snapshots -p 5001:80 friendapi-profiling
/tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots

/tools/dotmemory/dotMemory.sh get-snapshot $(ps aux | grep '/usr/share/dotnet/dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}')

ps aux | grep '/usr/share/dotnet/dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}'

TTY 2
/tools/dotmemory/dotMemory.sh get-snapshot $(ps aux | grep 'dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}') --save-to-dir=/snapshots > /snapshot/get-snapshot-1-log.txt

/tools/wrk/wrk -t2 -c2 -d10s --latency http://127.0.0.1:80/friends  > /snapshot/wrk-log.txt

/tools/dotmemory/dotMemory.sh get-snapshot $(ps aux | grep 'dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}') --save-to-dir=/snapshots > /snapshot/get-snapshot-2-log.txt

---

TTY 1 (background - sleep 5)
dotnet /app/FriendsApi.Host.dll

/tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots  --trigger-timer=30s --trigger-max-snapshots=2

---

HOST: docker start
docker run -it --rm --name friendapi-profiling --network="dbnet" -v C:\Github\dotnetcore-api-template\snapshots:/snapshots -p 5001:80  -e ASPNETCORE_ENVIRONMENT=compose friendapi-profiling

---

TTY1:
dotnet /app/FriendsApi.Host.dll

TTY2:
apt-get update && apt-get install wget
wget -O /snapshots/friends.json http://127.0.0.1:80/friends
/tools/dotmemory/dotMemory.sh get-snapshot $(ps aux | grep 'dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}') --save-to-dir=/snapshots
/tools/wrk/wrk -t2 -c2 -d10s --latency http://127.0.0.1:80/friends  > /snapshots/wrk-results.txt
/tools/dotmemory/dotMemory.sh get-snapshot $(ps aux | grep 'dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}') --save-to-dir=/snapshots

---


# ENTRYPOINT ["dotnet", "FriendsApi.Host.dll"]
# RUN dotnet /app/FriendsApi.Host.dll &

#RUN sleep 10

#CMD /tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots --trigger-timer=30s
# CMD /tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots --trigger-timer=30s --trigger-max-snapshots=2
#CMD /tools/dotmemory/dotMemory.sh get-snapshot 1 --save-to-dir=/snapshots
# & sleep 60 ; kill $! && sleep 20

---

/tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots > /snapshots/profiling-log.txt &
PROFILER_PID=$!

/bin/bash

# sleep 10 && /tools/wrk/wrk -t2 -c2 -d10s --latency http://127.0.0.1:80/friends  > /snapshots/wrk-log.txt

# sleep 20 && kill $PROFILER_PID

# sleep 20

# dotnet /app/FriendsApi.Host.dll &
# HOST_PID=$!
# # do other stuff



# # /tools/dotmemory/dotMemory.sh get-snapshot $HOST_PID --save-to-dir=/snapshots

# # /tools/dotmemory/dotMemory.sh get-snapshot $HOST_PID --save-to-dir=/snapshots

# sleep 10 && /tools/dotmemory/dotMemory.sh attach $HOST_PID --save-to-dir=/snapshots --trigger-timer=30s &
# PROFILER_PID=$!
# # /tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots --trigger-timer=30s --trigger-max-snapshots=2

# sleep 10 && /tools/wrk/wrk -t2 -c2 -d10s --latency http://127.0.0.1:80/friends  

# sleep 10 && /tools/dotmemory/dotMemory.sh get-snapshot $HOST_PID --save-to-dir=/snapshots 

# sleep 20 && kill $HOST_PID

# sleep 20


---

TTY1
/tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots  --trigger-timer=30s --trigger-max-snapshots=2

/tools/wrk/wrk -t2 -c3 -d60s --latency http://127.0.0.1:80/friends  > /snapshots/wrk-results-1.txt
/tools/wrk/wrk -t1 -c1 -d10s --latency -s /tmp/post.lua http://127.0.0.1:80/friends  > /snapshots/wrk-results-2.txt
/tools/wrk/wrk -t2 -c3 -d60s --latency http://127.0.0.1:80/friends  > /snapshots/wrk-results-3.txt

kill $(ps aux | grep 'dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}')

---

docker build -f .\Dockerfile.Profiling -t friendapi-profiling .

---


HOST: docker start
docker run -it --rm --name friendapi-profiling --network="dbnet" -v C:\Github\dotnetcore-api-template\snapshots:/snapshots -p 5001:80  -e ASPNETCORE_ENVIRONMENT=compose friendapi-profiling