using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cxserver.Infrastructure.Migrations.Tenant;

[DbContext(typeof(TenantDbContext))]
[Migration("20260228201500_UpdateFooterBackgroundTokenToBlack80")]
public partial class UpdateFooterBackgroundTokenToBlack80 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            UPDATE `footer_configs`
            SET `style_config_json` = JSON_SET(
                `style_config_json`,
                '$.backgroundToken', 'black',
                '$.textToken', 'footer-foreground',
                '$.linkToken', 'footer-foreground',
                '$.linkHoverToken', 'footer-foreground'
            ),
                `layout_config_json` = JSON_SET(
                    `layout_config_json`,
                    '$.sectionOrder', JSON_ARRAY('about','links','legal','social','bottom')
                ),
                `behavior_config_json` = JSON_SET(
                    `behavior_config_json`,
                    '$.showPayments', false,
                    '$.showNewsletter', false
                ),
                `component_config_json` = JSON_SET(
                    `component_config_json`,
                    '$.payments.enabled', false,
                    '$.payments.providers', JSON_ARRAY()
                );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            UPDATE `footer_configs`
            SET `style_config_json` = JSON_SET(
                `style_config_json`,
                '$.backgroundToken', 'footer-bg',
                '$.linkToken', 'link',
                '$.linkHoverToken', 'link-hover'
            )
            WHERE JSON_UNQUOTE(JSON_EXTRACT(`style_config_json`, '$.backgroundToken')) = 'black';
            """);
    }
}
