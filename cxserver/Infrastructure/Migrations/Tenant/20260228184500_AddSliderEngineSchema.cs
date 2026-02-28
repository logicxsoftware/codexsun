using System;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Tenant
{
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260228184500_AddSliderEngineSchema")]
    public partial class AddSliderEngineSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "slider_configs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    height_mode = table.Column<int>(type: "int", nullable: false),
                    height_value = table.Column<int>(type: "int", nullable: false),
                    container_mode = table.Column<int>(type: "int", nullable: false),
                    content_alignment = table.Column<int>(type: "int", nullable: false),
                    autoplay = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    loop = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    show_progress = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    show_nav_arrows = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    show_dots = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    parallax = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    particles = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    default_variant = table.Column<int>(type: "int", nullable: false),
                    default_intensity = table.Column<int>(type: "int", nullable: false),
                    default_direction = table.Column<int>(type: "int", nullable: false),
                    default_background_mode = table.Column<int>(type: "int", nullable: false),
                    scroll_behavior = table.Column<int>(type: "int", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slider_configs", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "slides",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    slider_config_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    display_order = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tagline = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action_text = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action_href = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cta_color = table.Column<int>(type: "int", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    direction = table.Column<int>(type: "int", nullable: false),
                    variant = table.Column<int>(type: "int", nullable: false),
                    intensity = table.Column<int>(type: "int", nullable: false),
                    background_mode = table.Column<int>(type: "int", nullable: false),
                    show_overlay = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    overlay_token = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    background_url = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    media_type = table.Column<int>(type: "int", nullable: false),
                    youtube_video_id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slides", x => x.id);
                    table.ForeignKey(
                        name: "FK_slides_slider_configs_slider_config_id",
                        column: x => x.slider_config_id,
                        principalTable: "slider_configs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "highlights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    slide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    text = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    variant = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    display_order = table.Column<int>(type: "int", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_highlights", x => x.id);
                    table.ForeignKey(
                        name: "FK_highlights_slides_slide_id",
                        column: x => x.slide_id,
                        principalTable: "slides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "slide_layers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    slide_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    display_order = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    media_url = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    position_x = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    position_y = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    width = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    animation_from = table.Column<int>(type: "int", nullable: false),
                    animation_delay = table.Column<int>(type: "int", nullable: false),
                    animation_duration = table.Column<int>(type: "int", nullable: false),
                    animation_easing = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    responsive_visibility = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_slide_layers", x => x.id);
                    table.ForeignKey(
                        name: "FK_slide_layers_slides_slide_id",
                        column: x => x.slide_id,
                        principalTable: "slides",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ux_slider_configs_tenant_soft_delete",
                table: "slider_configs",
                columns: new[] { "tenant_id", "is_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_slides_lookup",
                table: "slides",
                columns: new[] { "slider_config_id", "is_active", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "ux_slides_order_soft_delete",
                table: "slides",
                columns: new[] { "slider_config_id", "display_order", "is_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_slide_layers_order_soft_delete",
                table: "slide_layers",
                columns: new[] { "slide_id", "display_order", "is_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_highlights_order_soft_delete",
                table: "highlights",
                columns: new[] { "slide_id", "display_order", "is_deleted" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "highlights");
            migrationBuilder.DropTable(name: "slide_layers");
            migrationBuilder.DropTable(name: "slides");
            migrationBuilder.DropTable(name: "slider_configs");
        }
    }
}
