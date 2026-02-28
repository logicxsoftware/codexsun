namespace cxserver.Application.Abstractions;

public interface IProductCatalogQueryService
{
    Task<ProductListResponse> GetProductsAsync(Guid tenantId, ProductQueryRequest query, CancellationToken cancellationToken);
    Task<ProductDetailResponse?> GetProductBySlugAsync(Guid tenantId, string slug, CancellationToken cancellationToken);
}

public sealed record ProductQueryRequest(
    string? Category,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Search,
    string? Sort,
    int Page,
    int PageSize,
    IReadOnlyDictionary<string, string[]> Attributes);

public sealed record ProductListResponse(
    IReadOnlyList<ProductListItem> Data,
    ProductPaginationMeta Pagination,
    ProductFilterMeta Filters);

public sealed record ProductListItem(
    Guid Id,
    string Name,
    string Slug,
    string? ShortDescription,
    decimal Price,
    decimal? ComparePrice,
    int StockQuantity,
    string CategoryName,
    string CategorySlug,
    string? ImageUrl);

public sealed record ProductPaginationMeta(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPrevious,
    bool HasNext);

public sealed record ProductFilterMeta(
    IReadOnlyList<ProductCategoryFilterOption> Categories,
    IReadOnlyList<ProductAttributeFilterGroup> Attributes,
    ProductPriceRangeFilter PriceRange);

public sealed record ProductCategoryFilterOption(
    string Name,
    string Slug,
    int Count);

public sealed record ProductAttributeFilterGroup(
    string Key,
    IReadOnlyList<ProductAttributeFilterOption> Options);

public sealed record ProductAttributeFilterOption(
    string Value,
    int Count);

public sealed record ProductPriceRangeFilter(
    decimal? Min,
    decimal? Max);

public sealed record ProductDetailResponse(
    ProductDetailDto Product,
    IReadOnlyList<ProductCardDto> RelatedProducts);

public sealed record ProductDetailDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    string? ShortDescription,
    decimal Price,
    decimal? ComparePrice,
    bool InStock,
    string CategoryName,
    string CategorySlug,
    IReadOnlyList<string> Images,
    IReadOnlyDictionary<string, string> Specifications,
    string? Sku);

public sealed record ProductCardDto(
    Guid Id,
    string Name,
    string Slug,
    decimal Price,
    decimal? ComparePrice,
    string? ImageUrl);
