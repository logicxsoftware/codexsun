using System;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Tenant
{
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260228153000_AddWebNavigationAndFooterConfigs")]
    public partial class AddWebNavigationAndFooterConfigs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "footer_configs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    layout_config_json = table.Column<string>(type: "json", nullable: false),
                    style_config_json = table.Column<string>(type: "json", nullable: false),
                    behavior_config_json = table.Column<string>(type: "json", nullable: false),
                    component_config_json = table.Column<string>(type: "json", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_footer_configs", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "web_navigation_configs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    layout_config_json = table.Column<string>(type: "json", nullable: false),
                    style_config_json = table.Column<string>(type: "json", nullable: false),
                    behavior_config_json = table.Column<string>(type: "json", nullable: false),
                    component_config_json = table.Column<string>(type: "json", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_web_navigation_configs", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_footer_configs_active",
                table: "footer_configs",
                columns: new[] { "is_active", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ux_footer_configs_tenant_soft_delete",
                table: "footer_configs",
                columns: new[] { "tenant_id", "is_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_web_navigation_configs_active",
                table: "web_navigation_configs",
                columns: new[] { "is_active", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ux_web_navigation_configs_tenant_soft_delete",
                table: "web_navigation_configs",
                columns: new[] { "tenant_id", "is_deleted" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "footer_configs");
            migrationBuilder.DropTable(name: "web_navigation_configs");
        }
    }
}
