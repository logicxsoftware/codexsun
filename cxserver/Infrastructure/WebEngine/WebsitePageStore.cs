using cxserver.Application.Abstractions;
using cxserver.Domain.WebEngine;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.WebEngine;

internal sealed class WebsitePageStore : IWebsitePageStore
{
    private readonly ITenantDbContextAccessor _tenantDbContextAccessor;
    private readonly IDateTimeProvider _dateTimeProvider;

    public WebsitePageStore(ITenantDbContextAccessor tenantDbContextAccessor, IDateTimeProvider dateTimeProvider)
    {
        _tenantDbContextAccessor = tenantDbContextAccessor;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WebsitePageItem?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var normalized = NormalizeSlug(slug);
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var entity = await dbContext.WebsitePages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == normalized, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<WebsitePageItem?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var normalized = NormalizeSlug(slug);
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var entity = await dbContext.WebsitePages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Slug == normalized && x.IsPublished, cancellationToken);

        return entity is null ? null : Map(entity);
    }

    public async Task<IReadOnlyList<WebsitePageSectionItem>> GetSectionsAsync(Guid pageId, bool publishedOnly, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var query = dbContext.WebsitePageSections
            .AsNoTracking()
            .Where(x => x.PageId == pageId);

        if (publishedOnly)
        {
            query = query.Where(x => x.IsPublished);
        }

        var sections = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return sections.Select(Map).ToList();
    }

    public async Task<WebsitePageItem> CreatePageAsync(CreateWebsitePageInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var slug = NormalizeSlug(input.Slug);
        var exists = await dbContext.WebsitePages.AsNoTracking().AnyAsync(x => x.Slug == slug, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException("Page slug already exists.");
        }

        var now = _dateTimeProvider.UtcNow;
        var entity = Page.Create(Guid.NewGuid(), slug, input.Title, input.SeoTitle, input.SeoDescription, now);
        await dbContext.WebsitePages.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<WebsitePageSectionItem?> AddSectionAsync(AddWebsitePageSectionInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var page = await dbContext.WebsitePages
            .Include(x => x.Sections)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == input.PageId, cancellationToken);

        if (page is null)
        {
            return null;
        }

        var created = page.AddSection(
            Guid.NewGuid(),
            input.SectionType,
            input.DisplayOrder,
            input.SectionData,
            input.IsPublished,
            _dateTimeProvider.UtcNow);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(created);
    }

    public async Task<WebsitePageSectionItem?> UpdateSectionAsync(UpdateWebsitePageSectionInput input, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var page = await dbContext.WebsitePages
            .Include(x => x.Sections)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == input.PageId, cancellationToken);

        if (page is null)
        {
            return null;
        }

        try
        {
            var updated = page.UpdateSection(
                input.SectionId,
                input.DisplayOrder,
                input.SectionData,
                input.IsPublished,
                _dateTimeProvider.UtcNow);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Map(updated);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task<bool> RemoveSectionAsync(Guid pageId, Guid sectionId, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var page = await dbContext.WebsitePages
            .Include(x => x.Sections)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == pageId, cancellationToken);

        if (page is null)
        {
            return false;
        }

        try
        {
            page.RemoveSection(sectionId, _dateTimeProvider.UtcNow);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<bool> ReorderSectionsAsync(Guid pageId, IReadOnlyList<WebsitePageSectionOrderInput> sections, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var page = await dbContext.WebsitePages
            .Include(x => x.Sections)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == pageId, cancellationToken);

        if (page is null)
        {
            return false;
        }

        try
        {
            page.ReorderSections(sections.Select(x => (x.SectionId, x.DisplayOrder)).ToList(), _dateTimeProvider.UtcNow);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public async Task<bool> PublishPageAsync(Guid pageId, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var page = await dbContext.WebsitePages.AsTracking().FirstOrDefaultAsync(x => x.Id == pageId, cancellationToken);
        if (page is null)
        {
            return false;
        }

        page.Publish(_dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UnpublishPageAsync(Guid pageId, CancellationToken cancellationToken)
    {
        var dbContext = await _tenantDbContextAccessor.GetAsync(cancellationToken);
        var page = await dbContext.WebsitePages.AsTracking().FirstOrDefaultAsync(x => x.Id == pageId, cancellationToken);
        if (page is null)
        {
            return false;
        }

        page.Unpublish(_dateTimeProvider.UtcNow);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NormalizeSlug(string slug)
    {
        return slug.Trim().ToLowerInvariant();
    }

    private static WebsitePageItem Map(Page page)
    {
        return new WebsitePageItem(
            page.Id,
            page.Slug,
            page.Title,
            page.SeoTitle,
            page.SeoDescription,
            page.IsPublished,
            page.PublishedAtUtc,
            page.CreatedAtUtc,
            page.UpdatedAtUtc);
    }

    private static WebsitePageSectionItem Map(PageSection section)
    {
        return new WebsitePageSectionItem(
            section.Id,
            section.PageId,
            section.SectionType,
            section.DisplayOrder,
            section.SectionData,
            section.IsPublished,
            section.PublishedAtUtc,
            section.CreatedAtUtc,
            section.UpdatedAtUtc);
    }
}
