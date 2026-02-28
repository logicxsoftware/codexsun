namespace cxserver.Domain.ProductCatalog;

public sealed class Category
{
    private Category(
        Guid id,
        Guid tenantId,
        string name,
        string slug,
        Guid? parentId,
        int order)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Slug = slug;
        ParentId = parentId;
        Order = order;
    }

    private Category()
    {
        Name = string.Empty;
        Slug = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public Guid? ParentId { get; private set; }
    public int Order { get; private set; }

    public ICollection<Product> Products { get; } = new List<Product>();

    public static Category Create(
        Guid id,
        Guid tenantId,
        string name,
        string slug,
        Guid? parentId,
        int order)
    {
        return new Category(
            id,
            tenantId,
            name.Trim(),
            slug.Trim().ToLowerInvariant(),
            parentId,
            order);
    }

    public void Update(
        string name,
        string slug,
        Guid? parentId,
        int order)
    {
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        ParentId = parentId;
        Order = order;
    }
}
