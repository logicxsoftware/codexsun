using System;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Tenant
{
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260227234500_AddWebsitePageEngineSchema")]
    public partial class AddWebsitePageEngineSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "website_pages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    slug = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seo_title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seo_description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_published = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    published_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_website_pages", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "website_page_sections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    page_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    section_type = table.Column<int>(type: "int", nullable: false),
                    display_order = table.Column<int>(type: "int", nullable: false),
                    section_data_json = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_published = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    published_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_website_page_sections", x => x.id);
                    table.ForeignKey(
                        name: "FK_website_page_sections_website_pages_page_id",
                        column: x => x.page_id,
                        principalTable: "website_pages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_website_pages_is_published",
                table: "website_pages",
                column: "is_published");

            migrationBuilder.CreateIndex(
                name: "ux_website_pages_slug",
                table: "website_pages",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_website_page_sections_display_order",
                table: "website_page_sections",
                column: "display_order");

            migrationBuilder.CreateIndex(
                name: "ix_website_page_sections_page_id",
                table: "website_page_sections",
                column: "page_id");

            migrationBuilder.CreateIndex(
                name: "ix_website_page_sections_published_lookup",
                table: "website_page_sections",
                columns: new[] { "page_id", "is_published", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ux_website_page_sections_page_order_soft_delete",
                table: "website_page_sections",
                columns: new[] { "page_id", "display_order", "is_deleted" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "website_page_sections");

            migrationBuilder.DropTable(
                name: "website_pages");
        }
    }
}
