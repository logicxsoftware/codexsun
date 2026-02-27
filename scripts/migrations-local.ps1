param(
    [ValidateSet("add","update","drop","script")]
    [string]$Action = "update",
    [string]$Name = "Migration",
    [string]$MasterConnection = "server=localhost;port=3306;database=codexsun_dev;user=root;password=Computer.1;",
    [string]$TenantConnection = "server=localhost;port=3306;database=tenant_dev;user=root;password=Computer.1;",
    [string]$Provider = "MariaDb"
)

$ErrorActionPreference = "Stop"

$project = "cxserver/cxserver.csproj"
$startup = "cxserver/cxserver.csproj"

$env:CODEXSUN_DB_PROVIDER = $Provider
$env:CODEXSUN_MASTER_CONNECTION = $MasterConnection
$env:CODEXSUN_TENANT_CONNECTION = $TenantConnection

function Add-Migrations {
    dotnet ef migrations add "$Name`_Master" --project $project --startup-project $startup --context MasterDbContext --output-dir Infrastructure/Migrations/Master
    dotnet ef migrations add "$Name`_Tenant" --project $project --startup-project $startup --context TenantDbContext --output-dir Infrastructure/Migrations/Tenant
}

function Update-Databases {
    dotnet ef database update --project $project --startup-project $startup --context MasterDbContext
    dotnet ef database update --project $project --startup-project $startup --context TenantDbContext
}

function Drop-Databases {
    dotnet ef database drop --force --project $project --startup-project $startup --context MasterDbContext
    dotnet ef database drop --force --project $project --startup-project $startup --context TenantDbContext
}

function Generate-Scripts {
    dotnet ef migrations script --idempotent --project $project --startup-project $startup --context MasterDbContext --output scripts/master-migrations.sql
    dotnet ef migrations script --idempotent --project $project --startup-project $startup --context TenantDbContext --output scripts/tenant-migrations.sql
}

switch ($Action) {
    "add" { Add-Migrations }
    "update" { Update-Databases }
    "drop" { Drop-Databases }
    "script" { Generate-Scripts }
}
