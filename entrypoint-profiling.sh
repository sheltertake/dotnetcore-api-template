/tools/dotmemory/dotMemory.sh start-net-core /app/FriendsApi.Host.dll --save-to-dir=/snapshots  --trigger-timer=30s --trigger-max-snapshots=5 &

sleep 10

/tools/wrk/wrk -t2 -c3 -d30s --latency http://127.0.0.1:80/friends  > /snapshots/wrk-results-1.txt
/tools/wrk/wrk -t1 -c1 -d20s --latency -s /tmp/post.lua http://127.0.0.1:80/friends  > /snapshots/wrk-results-2.txt
/tools/wrk/wrk -t2 -c3 -d40s --latency http://127.0.0.1:80/friends  > /snapshots/wrk-results-3.txt

kill $(ps aux | grep 'dotnet /app/FriendsApi.Host.dll' | grep -v grep | awk '{print $2}')

sleep 10