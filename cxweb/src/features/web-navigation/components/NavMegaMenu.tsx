import type { MenuRenderDto } from "@/features/menu-admin/types/menu-types"

type NavMegaMenuProps = {
  menu: MenuRenderDto
}

function NavMegaMenu({ menu }: NavMegaMenuProps) {
  return (
    <div className="group relative">
      <button type="button" className="rounded-md px-3 py-2 text-sm font-medium text-foreground/90 hover:bg-menu-hover">
        {menu.name}
      </button>
      <div className="invisible absolute left-0 top-full z-50 mt-2 min-w-[540px] rounded-lg border border-border bg-card p-3 opacity-0 shadow-lg transition-all group-hover:visible group-hover:opacity-100">
        <div className="grid grid-cols-2 gap-2">
          {menu.items.map((item) => (
            <div key={`${menu.slug}-${item.slug}`} className="rounded-md border border-border/60 bg-background p-2">
              <a href={item.url} target={item.target === 2 ? "_blank" : "_self"} rel={item.target === 2 ? "noreferrer" : undefined} className="text-sm font-medium text-foreground hover:text-link-hover">
                {item.title}
              </a>
              {item.children.length > 0 ? (
                <div className="mt-1 grid gap-1">
                  {item.children.map((child) => (
                    <a
                      key={`${item.slug}-${child.slug}`}
                      href={child.url}
                      target={child.target === 2 ? "_blank" : "_self"}
                      rel={child.target === 2 ? "noreferrer" : undefined}
                      className="text-xs text-muted-foreground hover:text-link-hover"
                    >
                      {child.title}
                    </a>
                  ))}
                </div>
              ) : null}
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

export default NavMegaMenu
