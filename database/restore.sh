echo "Setting Environment variables."
export ACCEPT_EULA=Y
export SA_PASSWORD=yourStrong1234!Password
echo "Environment variables set."
echo "Starting SqlServr"
/opt/mssql/bin/sqlservr &
sleep 60 | echo "Waiting for 60s to start Sql Server"
# echo "Setting RAM to 2GB usage."
# /opt/mssql/bin/mssql-conf set memory.memorylimitmb 2048
# echo "Restarting to apply the changes."
# systemctl restart mssql-server.service
echo "Restoring DB."
/opt/mssql-tools/bin/sqlcmd -S127.0.0.1 -U sa -P $SA_PASSWORD -i $1
echo "DB restored."
echo "Deleting backup files."
rm -rf /work/*.bak