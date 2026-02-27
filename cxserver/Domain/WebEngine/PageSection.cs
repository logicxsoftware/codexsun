using System.Text.Json;
using cxserver.Domain.Common;

namespace cxserver.Domain.WebEngine;

public sealed class PageSection : ISoftDeletable
{
    private PageSection(
        Guid id,
        Guid pageId,
        SectionType sectionType,
        int displayOrder,
        JsonDocument sectionData,
        bool isPublished,
        DateTimeOffset? publishedAtUtc,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc)
    {
        Id = id;
        PageId = pageId;
        SectionType = sectionType;
        DisplayOrder = displayOrder;
        SectionData = sectionData;
        IsPublished = isPublished;
        PublishedAtUtc = publishedAtUtc;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private PageSection()
    {
        SectionData = JsonDocument.Parse("{}");
    }

    public Guid Id { get; private set; }
    public Guid PageId { get; private set; }
    public SectionType SectionType { get; private set; }
    public int DisplayOrder { get; private set; }
    public JsonDocument SectionData { get; private set; }
    public bool IsPublished { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    internal static PageSection Create(
        Guid id,
        Guid pageId,
        SectionType sectionType,
        int displayOrder,
        JsonDocument sectionData,
        DateTimeOffset nowUtc,
        bool isPublished)
    {
        ValidateDisplayOrder(displayOrder);
        ArgumentNullException.ThrowIfNull(sectionData);

        return new PageSection(
            id,
            pageId,
            sectionType,
            displayOrder,
            sectionData,
            isPublished,
            isPublished ? nowUtc : null,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    internal void Update(int displayOrder, JsonDocument sectionData, DateTimeOffset nowUtc)
    {
        ValidateDisplayOrder(displayOrder);
        ArgumentNullException.ThrowIfNull(sectionData);

        DisplayOrder = displayOrder;
        SectionData = sectionData;
        UpdatedAtUtc = nowUtc;
    }

    internal void Publish(DateTimeOffset nowUtc)
    {
        IsPublished = true;
        PublishedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    internal void Unpublish(DateTimeOffset nowUtc)
    {
        IsPublished = false;
        PublishedAtUtc = null;
        UpdatedAtUtc = nowUtc;
    }

    internal void Remove(DateTimeOffset nowUtc)
    {
        Delete(nowUtc);
    }

    internal void Reorder(int displayOrder, DateTimeOffset nowUtc)
    {
        ValidateDisplayOrder(displayOrder);
        DisplayOrder = displayOrder;
        UpdatedAtUtc = nowUtc;
    }

    private static void ValidateDisplayOrder(int displayOrder)
    {
        if (displayOrder < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(displayOrder), "Display order must be greater than or equal to zero.");
        }
    }

    public void Delete(DateTimeOffset deletedAtUtc)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAtUtc;
        UpdatedAtUtc = deletedAtUtc;
    }

    public void Restore(DateTimeOffset restoredAtUtc)
    {
        RestoreInternal(restoredAtUtc);
    }

    private void RestoreInternal(DateTimeOffset restoredAtUtc)
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = restoredAtUtc;
    }
}
