using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Tenant
{
    /// <inheritdoc />
    public partial class AddBlogModuleSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "blog_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_categories", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "blog_tags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_tags", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "blog_posts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slug = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    excerpt = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    body = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    featured_image = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    meta_keywords = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    published = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_posts", x => x.id);
                    table.ForeignKey(
                        name: "FK_blog_posts_blog_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "blog_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "blog_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    post_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    body = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    approved = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_blog_comments_blog_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "blog_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "blog_likes",
                columns: table => new
                {
                    post_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    liked = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_likes", x => new { x.post_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_blog_likes_blog_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "blog_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "blog_post_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    post_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    image_path = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alt_text = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    caption = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort_order = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_post_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_blog_post_images_blog_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "blog_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "blog_post_tags",
                columns: table => new
                {
                    post_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tag_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    tenant_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    created_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at_utc = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blog_post_tags", x => new { x.post_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_blog_post_tags_blog_posts_post_id",
                        column: x => x.post_id,
                        principalTable: "blog_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_blog_post_tags_blog_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "blog_tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_blog_categories_tenant_id",
                table: "blog_categories",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ux_blog_categories_tenant_name",
                table: "blog_categories",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_blog_categories_tenant_slug",
                table: "blog_categories",
                columns: new[] { "tenant_id", "slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_blog_comments_post_approved",
                table: "blog_comments",
                columns: new[] { "post_id", "approved" });

            migrationBuilder.CreateIndex(
                name: "ix_blog_comments_tenant_id",
                table: "blog_comments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_blog_likes_tenant_id",
                table: "blog_likes",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_blog_post_images_post_sort",
                table: "blog_post_images",
                columns: new[] { "post_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_blog_post_images_tenant_id",
                table: "blog_post_images",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_blog_post_tags_tag_id",
                table: "blog_post_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_blog_post_tags_tenant_id",
                table: "blog_post_tags",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_blog_posts_category_id",
                table: "blog_posts",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_blog_posts_visibility",
                table: "blog_posts",
                columns: new[] { "tenant_id", "published", "active", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ux_blog_posts_tenant_slug",
                table: "blog_posts",
                columns: new[] { "tenant_id", "slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_blog_tags_tenant_id",
                table: "blog_tags",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ux_blog_tags_tenant_name",
                table: "blog_tags",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_blog_tags_tenant_slug",
                table: "blog_tags",
                columns: new[] { "tenant_id", "slug" },
                unique: true);

            if (ActiveProvider.Contains("Npgsql", StringComparison.Ordinal))
            {
                migrationBuilder.Sql("""
                    ALTER TABLE blog_posts
                    ADD COLUMN IF NOT EXISTS search_vector tsvector;
                    """);

                migrationBuilder.Sql("""
                    CREATE OR REPLACE FUNCTION blog_posts_search_vector_update() RETURNS trigger AS $$
                    BEGIN
                        NEW.search_vector := to_tsvector(
                            'english',
                            coalesce(NEW.title, '') || ' ' || coalesce(NEW.excerpt, '') || ' ' || coalesce(NEW.body, ''));
                        RETURN NEW;
                    END
                    $$ LANGUAGE plpgsql;
                    """);

                migrationBuilder.Sql("""
                    DROP TRIGGER IF EXISTS trg_blog_posts_search_vector_update ON blog_posts;
                    CREATE TRIGGER trg_blog_posts_search_vector_update
                    BEFORE INSERT OR UPDATE ON blog_posts
                    FOR EACH ROW EXECUTE FUNCTION blog_posts_search_vector_update();
                    """);

                migrationBuilder.Sql("""
                    UPDATE blog_posts
                    SET search_vector = to_tsvector(
                        'english',
                        coalesce(title, '') || ' ' || coalesce(excerpt, '') || ' ' || coalesce(body, ''));
                    """);

                migrationBuilder.Sql("""
                    CREATE INDEX IF NOT EXISTS ix_blog_posts_search_vector
                    ON blog_posts USING GIN (search_vector);
                    """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blog_comments");

            migrationBuilder.DropTable(
                name: "blog_likes");

            migrationBuilder.DropTable(
                name: "blog_post_images");

            migrationBuilder.DropTable(
                name: "blog_post_tags");

            migrationBuilder.DropTable(
                name: "blog_posts");

            migrationBuilder.DropTable(
                name: "blog_tags");

            migrationBuilder.DropTable(
                name: "blog_categories");

            if (ActiveProvider.Contains("Npgsql", StringComparison.Ordinal))
            {
                migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_blog_posts_search_vector_update ON blog_posts;");
                migrationBuilder.Sql("DROP FUNCTION IF EXISTS blog_posts_search_vector_update();");
            }
        }
    }
}
