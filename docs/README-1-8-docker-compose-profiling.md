 - https://www.jetbrains.com/profiler/download/#section=commandline
 - https://www.jetbrains.com/dotmemory/download/#section=commandline
 - https://stackoverflow.com/questions/26982274/ps-command-doesnt-work-in-docker-container
   - apt-get update && apt-get install -y procps

 - docker-compose -f .\docker-compose-profiling.yml build
 - docker-compose -f .\docker-compose-profiling.yml up api
 - attach shell

root@c52abfa729c8:/app# cd /tools/JetBrains.dotMemory.Console.linux-x64.2020.2.4
root@c52abfa729c8:/tools/JetBrains.dotMemory.Console.linux-x64.2020.2.4# ./dotMemory.sh attach 1 --save-to-dir=/snapshots