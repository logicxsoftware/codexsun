using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Master
{
    /// <inheritdoc />
    public partial class InitialMasterSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    identifier = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    database_name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    connection_string = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    feature_settings_json = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isolation_metadata_json = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_identifier_status_soft_delete",
                table: "tenants",
                columns: new[] { "identifier", "status", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ix_tenants_is_deleted",
                table: "tenants",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_tenants_status",
                table: "tenants",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ux_tenants_database_name",
                table: "tenants",
                column: "database_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_tenants_identifier",
                table: "tenants",
                column: "identifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}
