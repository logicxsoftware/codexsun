using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Tenant;

[DbContext(typeof(TenantDbContext))]
[Migration("20260228183305_AddAboutPageSchema")]
public partial class AddAboutPageSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS `about_page_sections` (
              `id` char(36) COLLATE ascii_general_ci NOT NULL,
              `tenant_id` char(36) COLLATE ascii_general_ci NOT NULL,
              `hero_title` varchar(512) NOT NULL,
              `hero_subtitle` varchar(1024) NOT NULL,
              `about_title` varchar(512) NOT NULL,
              `about_subtitle` varchar(1024) NOT NULL,
              `created_at_utc` datetime(6) NOT NULL,
              `updated_at_utc` datetime(6) NOT NULL,
              PRIMARY KEY (`id`),
              UNIQUE KEY `ux_about_page_sections_tenant_id` (`tenant_id`),
              KEY `ix_about_page_sections_tenant_id` (`tenant_id`)
            ) CHARACTER SET utf8mb4;
            """);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS `about_team_members` (
              `id` char(36) COLLATE ascii_general_ci NOT NULL,
              `section_id` char(36) COLLATE ascii_general_ci NOT NULL,
              `tenant_id` char(36) COLLATE ascii_general_ci NOT NULL,
              `name` varchar(256) NOT NULL,
              `role` varchar(256) NOT NULL,
              `bio` varchar(1024) NOT NULL,
              `image` varchar(1024) NOT NULL,
              `display_order` int NOT NULL,
              PRIMARY KEY (`id`),
              UNIQUE KEY `ux_about_team_members_section_order` (`section_id`,`display_order`),
              KEY `ix_about_team_members_tenant_id` (`tenant_id`),
              KEY `ix_about_team_members_display_order` (`display_order`),
              CONSTRAINT `fk_about_team_members_section` FOREIGN KEY (`section_id`) REFERENCES `about_page_sections` (`id`) ON DELETE CASCADE
            ) CHARACTER SET utf8mb4;
            """);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS `about_testimonials` (
              `id` char(36) COLLATE ascii_general_ci NOT NULL,
              `section_id` char(36) COLLATE ascii_general_ci NOT NULL,
              `tenant_id` char(36) COLLATE ascii_general_ci NOT NULL,
              `name` varchar(256) NOT NULL,
              `company` varchar(256) NULL,
              `quote` varchar(2048) NOT NULL,
              `rating` int NULL,
              `display_order` int NOT NULL,
              PRIMARY KEY (`id`),
              UNIQUE KEY `ux_about_testimonials_section_order` (`section_id`,`display_order`),
              KEY `ix_about_testimonials_tenant_id` (`tenant_id`),
              KEY `ix_about_testimonials_display_order` (`display_order`),
              CONSTRAINT `fk_about_testimonials_section` FOREIGN KEY (`section_id`) REFERENCES `about_page_sections` (`id`) ON DELETE CASCADE
            ) CHARACTER SET utf8mb4;
            """);

        migrationBuilder.Sql(
            """
            CREATE TABLE IF NOT EXISTS `about_roadmap_milestones` (
              `id` char(36) COLLATE ascii_general_ci NOT NULL,
              `section_id` char(36) COLLATE ascii_general_ci NOT NULL,
              `tenant_id` char(36) COLLATE ascii_general_ci NOT NULL,
              `year` varchar(32) NOT NULL,
              `title` varchar(256) NOT NULL,
              `description` varchar(1024) NOT NULL,
              `display_order` int NOT NULL,
              PRIMARY KEY (`id`),
              UNIQUE KEY `ux_about_roadmap_milestones_section_order` (`section_id`,`display_order`),
              KEY `ix_about_roadmap_milestones_tenant_id` (`tenant_id`),
              KEY `ix_about_roadmap_milestones_display_order` (`display_order`),
              CONSTRAINT `fk_about_roadmap_section` FOREIGN KEY (`section_id`) REFERENCES `about_page_sections` (`id`) ON DELETE CASCADE
            ) CHARACTER SET utf8mb4;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TABLE IF EXISTS `about_roadmap_milestones`;");
        migrationBuilder.Sql("DROP TABLE IF EXISTS `about_testimonials`;");
        migrationBuilder.Sql("DROP TABLE IF EXISTS `about_team_members`;");
        migrationBuilder.Sql("DROP TABLE IF EXISTS `about_page_sections`;");
    }
}
