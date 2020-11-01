dotnet /app/FriendsApi.Host.dll &
HOST_PID=$!
# do other stuff



# /tools/dotmemory/dotMemory.sh get-snapshot $HOST_PID --save-to-dir=/snapshots

# /tools/dotmemory/dotMemory.sh get-snapshot $HOST_PID --save-to-dir=/snapshots

sleep 10 && /tools/dotmemory/dotMemory.sh attach $HOST_PID --save-to-dir=/snapshots --trigger-timer=30s &
PROFILER_PID=$!
# /tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots --trigger-timer=30s --trigger-max-snapshots=2

sleep 10 && /tools/wrk/wrk -t2 -c2 -d10s --latency http://127.0.0.1:80/friends  

sleep 10 && /tools/dotmemory/dotMemory.sh get-snapshot $HOST_PID --save-to-dir=/snapshots 

sleep 20 && kill $HOST_PID

sleep 20