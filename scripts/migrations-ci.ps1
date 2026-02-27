param(
    [string]$Provider = "MariaDb",
    [string]$MasterConnection = "server=localhost;port=3306;database=codexsun_dev;user=root;password=Computer.1;",
    [string]$TenantConnection = "server=localhost;port=3306;database=tenant_dev;user=root;password=Computer.1;"
)

$ErrorActionPreference = "Stop"

$project = "cxserver/cxserver.csproj"
$startup = "cxserver/cxserver.csproj"

$env:CODEXSUN_DB_PROVIDER = $Provider
$env:CODEXSUN_MASTER_CONNECTION = $MasterConnection
$env:CODEXSUN_TENANT_CONNECTION = $TenantConnection

 dotnet restore codexsun.sln
 dotnet build codexsun.sln --configuration Release
 dotnet ef migrations script --idempotent --project $project --startup-project $startup --context MasterDbContext --output scripts/master-migrations-ci.sql
 dotnet ef migrations script --idempotent --project $project --startup-project $startup --context TenantDbContext --output scripts/tenant-migrations-ci.sql
 dotnet test cxtest/cxtest.csproj --configuration Release
