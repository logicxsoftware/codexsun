using System.Text.Json;
using cxserver.Domain.WebEngine;

namespace cxserver.Application.Abstractions;

public interface IWebsitePageStore
{
    Task<WebsitePageItem?> GetBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<WebsitePageItem?> GetPublishedBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<IReadOnlyList<WebsitePageSectionItem>> GetSectionsAsync(Guid pageId, bool publishedOnly, CancellationToken cancellationToken);
    Task<WebsitePageItem> CreatePageAsync(CreateWebsitePageInput input, CancellationToken cancellationToken);
    Task<WebsitePageSectionItem?> AddSectionAsync(AddWebsitePageSectionInput input, CancellationToken cancellationToken);
    Task<WebsitePageSectionItem?> UpdateSectionAsync(UpdateWebsitePageSectionInput input, CancellationToken cancellationToken);
    Task<bool> RemoveSectionAsync(Guid pageId, Guid sectionId, CancellationToken cancellationToken);
    Task<bool> ReorderSectionsAsync(Guid pageId, IReadOnlyList<WebsitePageSectionOrderInput> sections, CancellationToken cancellationToken);
    Task<bool> PublishPageAsync(Guid pageId, CancellationToken cancellationToken);
    Task<bool> UnpublishPageAsync(Guid pageId, CancellationToken cancellationToken);
}

public sealed record WebsitePageItem(
    Guid Id,
    string Slug,
    string Title,
    string SeoTitle,
    string SeoDescription,
    bool IsPublished,
    DateTimeOffset? PublishedAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record WebsitePageSectionItem(
    Guid Id,
    Guid PageId,
    SectionType SectionType,
    int DisplayOrder,
    JsonDocument SectionData,
    bool IsPublished,
    DateTimeOffset? PublishedAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record CreateWebsitePageInput(
    string Slug,
    string Title,
    string SeoTitle,
    string SeoDescription);

public sealed record AddWebsitePageSectionInput(
    Guid PageId,
    SectionType SectionType,
    int DisplayOrder,
    JsonDocument SectionData,
    bool IsPublished);

public sealed record UpdateWebsitePageSectionInput(
    Guid PageId,
    Guid SectionId,
    int DisplayOrder,
    JsonDocument SectionData,
    bool IsPublished);

public sealed record WebsitePageSectionOrderInput(
    Guid SectionId,
    int DisplayOrder);
