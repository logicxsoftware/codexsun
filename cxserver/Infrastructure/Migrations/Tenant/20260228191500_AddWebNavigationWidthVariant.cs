using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Tenant;

[DbContext(typeof(TenantDbContext))]
[Migration("20260228191500_AddWebNavigationWidthVariant")]
public partial class AddWebNavigationWidthVariant : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            SET @exists := (
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'web_navigation_configs'
                  AND COLUMN_NAME = 'width_variant'
            );
            SET @sql := IF(
                @exists = 0,
                'ALTER TABLE `web_navigation_configs` ADD COLUMN `width_variant` int NOT NULL DEFAULT 0',
                'SELECT 1'
            );
            PREPARE stmt FROM @sql;
            EXECUTE stmt;
            DEALLOCATE PREPARE stmt;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            SET @exists := (
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = 'web_navigation_configs'
                  AND COLUMN_NAME = 'width_variant'
            );
            SET @sql := IF(
                @exists = 1,
                'ALTER TABLE `web_navigation_configs` DROP COLUMN `width_variant`',
                'SELECT 1'
            );
            PREPARE stmt FROM @sql;
            EXECUTE stmt;
            DEALLOCATE PREPARE stmt;
            """);
    }
}
