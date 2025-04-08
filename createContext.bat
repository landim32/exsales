@echo off
cd .\Backend\exSales\DB.Infra
dotnet ef dbcontext scaffold "Host=167.172.240.71;Port=5432;Database=exsales;Username=postgres;Password=eaa69cpxy2" Npgsql.EntityFrameworkCore.PostgreSQL --context ExSalesContext --output-dir Context -f
pause