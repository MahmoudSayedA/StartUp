#!/bin/bash

# Start SQL Server in background
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start
echo "Waiting for SQL Server to start..."
sleep 30s

# Create databases
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StartUpDb')
CREATE DATABASE StartUpDb;

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'StartUpDb_Hangfire')
CREATE DATABASE StartUpDb_Hangfire;
"

echo "Databases created successfully!"

# Keep SQL Server running
wait