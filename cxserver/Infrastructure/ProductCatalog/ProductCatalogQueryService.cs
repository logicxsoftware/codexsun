using cxserver.Application.Abstractions;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.ProductCatalog;

internal sealed class ProductCatalogQueryService : IProductCatalogQueryService
{
    private readonly ITenantDbContextAccessor _dbContextAccessor;

    public ProductCatalogQueryService(ITenantDbContextAccessor dbContextAccessor)
    {
        _dbContextAccessor = dbContextAccessor;
    }

    public async Task<ProductListResponse> GetProductsAsync(Guid tenantId, ProductQueryRequest query, CancellationToken cancellationToken)
    {
        var dbContext = await _dbContextAccessor.GetAsync(cancellationToken);
        var normalized = Normalize(query);

        var baseQuery = dbContext.Products
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.IsActive);

        baseQuery = ApplyFilters(baseQuery, normalized);
        baseQuery = ApplySorting(baseQuery, normalized.Sort);

        var totalItems = await baseQuery.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)normalized.PageSize);
        var page = Math.Min(normalized.Page, totalPages);

        var items = await baseQuery
            .Skip((page - 1) * normalized.PageSize)
            .Take(normalized.PageSize)
            .Select(p => new ProductListItem(
                p.Id,
                p.Name,
                p.Slug,
                p.ShortDescription,
                p.Price,
                p.ComparePrice,
                p.StockQuantity,
                p.Category.Name,
                p.Category.Slug,
                null))
            .ToListAsync(cancellationToken);

        var itemIds = items.Select(x => x.Id).ToList();
        var images = await dbContext.ProductImages
            .AsNoTracking()
            .Where(x => itemIds.Contains(x.ProductId))
            .OrderBy(x => x.Order)
            .Select(x => new { x.ProductId, x.ImageUrl, x.Order })
            .ToListAsync(cancellationToken);

        var imageMap = images
            .GroupBy(x => x.ProductId)
            .ToDictionary(group => group.Key, group => group.First().ImageUrl);

        items = items
            .Select(x => x with { ImageUrl = imageMap.TryGetValue(x.Id, out var imageUrl) ? imageUrl : null })
            .ToList();

        var filters = await BuildFilterMetaAsync(dbContext, tenantId, cancellationToken);

        return new ProductListResponse(
            items,
            new ProductPaginationMeta(
                page,
                normalized.PageSize,
                totalItems,
                totalPages,
                page > 1,
                page < totalPages),
            filters);
    }

    public async Task<ProductDetailResponse?> GetProductBySlugAsync(Guid tenantId, string slug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var normalizedSlug = slug.Trim().ToLowerInvariant();
        var dbContext = await _dbContextAccessor.GetAsync(cancellationToken);

        var product = await dbContext.Products
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.IsActive && p.Slug == normalizedSlug)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Slug,
                p.Description,
                p.ShortDescription,
                p.Price,
                p.ComparePrice,
                p.SKU,
                p.StockQuantity,
                CategoryName = p.Category.Name,
                CategoryId = p.CategoryId,
                CategorySlug = p.Category.Slug,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return null;
        }

        var images = await dbContext.ProductImages
            .AsNoTracking()
            .Where(x => x.ProductId == product.Id)
            .OrderBy(x => x.Order)
            .Select(x => x.ImageUrl)
            .ToListAsync(cancellationToken);

        var specifications = await dbContext.ProductAttributes
            .AsNoTracking()
            .Where(x => x.ProductId == product.Id)
            .OrderBy(x => x.Key)
            .ThenBy(x => x.Value)
            .Select(x => new { x.Key, x.Value })
            .ToListAsync(cancellationToken);

        var specificationMap = specifications
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => string.Join(", ", group.Select(x => x.Value).Distinct()), StringComparer.OrdinalIgnoreCase);

        var related = await dbContext.Products
            .AsNoTracking()
            .Where(p =>
                p.TenantId == tenantId &&
                p.IsActive &&
                p.Slug != normalizedSlug &&
                p.CategoryId == product.CategoryId)
            .OrderByDescending(p => p.CreatedAtUtc)
            .Take(6)
            .Select(p => new ProductCardDto(
                p.Id,
                p.Name,
                p.Slug,
                p.Price,
                p.ComparePrice,
                null))
            .ToListAsync(cancellationToken);

        var relatedIds = related.Select(x => x.Id).ToList();
        var relatedImages = await dbContext.ProductImages
            .AsNoTracking()
            .Where(x => relatedIds.Contains(x.ProductId))
            .OrderBy(x => x.Order)
            .Select(x => new { x.ProductId, x.ImageUrl, x.Order })
            .ToListAsync(cancellationToken);

        var relatedImageMap = relatedImages
            .GroupBy(x => x.ProductId)
            .ToDictionary(group => group.Key, group => group.First().ImageUrl);

        related = related
            .Select(x => x with { ImageUrl = relatedImageMap.TryGetValue(x.Id, out var imageUrl) ? imageUrl : null })
            .ToList();

        return new ProductDetailResponse(
            new ProductDetailDto(
                product.Id,
                product.Name,
                product.Slug,
                product.Description,
                product.ShortDescription,
                product.Price,
                product.ComparePrice,
                product.StockQuantity > 0,
                product.CategoryName,
                product.CategorySlug,
                images,
                specificationMap,
                product.SKU),
            related);
    }

    private static ProductQueryRequest Normalize(ProductQueryRequest query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 12 : Math.Min(query.PageSize, 60);
        var attributes = query.Attributes
            .Where(entry => entry.Key.Trim().Length > 0)
            .ToDictionary(
                entry => entry.Key.Trim().ToLowerInvariant(),
                entry => entry.Value
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Select(v => v.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                StringComparer.OrdinalIgnoreCase);

        return query with
        {
            Category = string.IsNullOrWhiteSpace(query.Category) ? null : query.Category.Trim().ToLowerInvariant(),
            Search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim(),
            Sort = string.IsNullOrWhiteSpace(query.Sort) ? "latest" : query.Sort.Trim().ToLowerInvariant(),
            Page = page,
            PageSize = pageSize,
            Attributes = attributes,
        };
    }

    private static IQueryable<Domain.ProductCatalog.Product> ApplyFilters(
        IQueryable<Domain.ProductCatalog.Product> query,
        ProductQueryRequest filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(p => p.Category.Slug == filter.Category);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = $"%{filter.Search}%";
            query = query.Where(p =>
                EF.Functions.Like(p.Name, pattern) ||
                EF.Functions.Like(p.Description, pattern) ||
                (p.ShortDescription != null && EF.Functions.Like(p.ShortDescription, pattern)));
        }

        foreach (var attribute in filter.Attributes)
        {
            if (attribute.Value.Length == 0)
            {
                continue;
            }

            var key = attribute.Key;
            var values = attribute.Value.ToList();
            query = query.Where(p => p.Attributes.Any(a => a.Key == key && values.Contains(a.Value)));
        }

        return query;
    }

    private static IQueryable<Domain.ProductCatalog.Product> ApplySorting(
        IQueryable<Domain.ProductCatalog.Product> query,
        string? sort)
    {
        return sort switch
        {
            "price_asc" => query.OrderBy(p => p.Price).ThenByDescending(p => p.CreatedAtUtc),
            "price_desc" => query.OrderByDescending(p => p.Price).ThenByDescending(p => p.CreatedAtUtc),
            "name_asc" => query.OrderBy(p => p.Name).ThenByDescending(p => p.CreatedAtUtc),
            "name_desc" => query.OrderByDescending(p => p.Name).ThenByDescending(p => p.CreatedAtUtc),
            _ => query.OrderByDescending(p => p.CreatedAtUtc),
        };
    }

    private static async Task<ProductFilterMeta> BuildFilterMetaAsync(
        TenantDbContext dbContext,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var categoryRows = await (
                from product in dbContext.Products.AsNoTracking()
                join category in dbContext.Categories.AsNoTracking() on product.CategoryId equals category.Id
                where product.TenantId == tenantId && product.IsActive
                group product by new { category.Name, category.Slug }
                into grouped
                select new
                {
                    grouped.Key.Name,
                    grouped.Key.Slug,
                    Count = grouped.Count(),
                })
            .OrderBy(option => option.Name)
            .ToListAsync(cancellationToken);

        var categories = categoryRows
            .Select(x => new ProductCategoryFilterOption(x.Name, x.Slug, x.Count))
            .ToList();

        var attributes = await (
                from attribute in dbContext.ProductAttributes.AsNoTracking()
                join product in dbContext.Products.AsNoTracking() on attribute.ProductId equals product.Id
                where product.TenantId == tenantId && product.IsActive
                group attribute by new { attribute.Key, attribute.Value }
                into grouped
                select new
                {
                    grouped.Key.Key,
                    grouped.Key.Value,
                    Count = grouped.Count(),
                })
            .ToListAsync(cancellationToken);

        var groupedAttributes = attributes
            .GroupBy(x => x.Key)
            .OrderBy(group => group.Key)
            .Select(group => new ProductAttributeFilterGroup(
                group.Key,
                group
                    .OrderBy(x => x.Value)
                    .Select(x => new ProductAttributeFilterOption(x.Value, x.Count))
                    .ToList()))
            .ToList();

        var priceRange = await dbContext.Products
            .AsNoTracking()
            .Where(p => p.TenantId == tenantId && p.IsActive)
            .GroupBy(_ => 1)
            .Select(group => new ProductPriceRangeFilter(
                group.Min(x => (decimal?)x.Price),
                group.Max(x => (decimal?)x.Price)))
            .FirstOrDefaultAsync(cancellationToken)
            ?? new ProductPriceRangeFilter(null, null);

        return new ProductFilterMeta(categories, groupedAttributes, priceRange);
    }
}
