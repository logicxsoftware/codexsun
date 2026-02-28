namespace cxserver.Domain.ProductCatalog;

public sealed class Product
{
    private Product(
        Guid id,
        Guid tenantId,
        string name,
        string slug,
        string description,
        string? shortDescription,
        decimal price,
        decimal? comparePrice,
        string sku,
        int stockQuantity,
        bool isActive,
        Guid categoryId,
        DateTimeOffset createdAtUtc)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Slug = slug;
        Description = description;
        ShortDescription = shortDescription;
        Price = price;
        ComparePrice = comparePrice;
        SKU = sku;
        StockQuantity = stockQuantity;
        IsActive = isActive;
        CategoryId = categoryId;
        CreatedAtUtc = createdAtUtc;
    }

    private Product()
    {
        Name = string.Empty;
        Slug = string.Empty;
        Description = string.Empty;
        SKU = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string Description { get; private set; }
    public string? ShortDescription { get; private set; }
    public decimal Price { get; private set; }
    public decimal? ComparePrice { get; private set; }
    public string SKU { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CategoryId { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public Category Category { get; private set; } = default!;
    public ICollection<ProductImage> Images { get; } = new List<ProductImage>();
    public ICollection<ProductAttribute> Attributes { get; } = new List<ProductAttribute>();

    public static Product Create(
        Guid id,
        Guid tenantId,
        string name,
        string slug,
        string description,
        string? shortDescription,
        decimal price,
        decimal? comparePrice,
        string sku,
        int stockQuantity,
        bool isActive,
        Guid categoryId,
        DateTimeOffset createdAtUtc)
    {
        return new Product(
            id,
            tenantId,
            name.Trim(),
            slug.Trim().ToLowerInvariant(),
            description.Trim(),
            string.IsNullOrWhiteSpace(shortDescription) ? null : shortDescription.Trim(),
            price,
            comparePrice,
            sku.Trim(),
            stockQuantity,
            isActive,
            categoryId,
            createdAtUtc);
    }

    public void Update(
        string name,
        string slug,
        string description,
        string? shortDescription,
        decimal price,
        decimal? comparePrice,
        string sku,
        int stockQuantity,
        bool isActive,
        Guid categoryId)
    {
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Description = description.Trim();
        ShortDescription = string.IsNullOrWhiteSpace(shortDescription) ? null : shortDescription.Trim();
        Price = price;
        ComparePrice = comparePrice;
        SKU = sku.Trim();
        StockQuantity = stockQuantity;
        IsActive = isActive;
        CategoryId = categoryId;
    }
}
