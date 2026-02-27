using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using cxserver.Infrastructure.Persistence;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Master
{
    [DbContext(typeof(MasterDbContext))]
    [Migration("20260227194000_AddTenantDomain")]
    public partial class AddTenantDomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "domain",
                table: "tenants",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql("UPDATE tenants SET domain = CONCAT(identifier, '.localhost') WHERE domain = '' OR domain IS NULL");

            migrationBuilder.CreateIndex(
                name: "ux_tenants_domain",
                table: "tenants",
                column: "domain",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_tenants_domain",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "domain",
                table: "tenants");
        }
    }
}
