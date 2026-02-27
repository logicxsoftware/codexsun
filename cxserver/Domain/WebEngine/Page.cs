using System.Text.Json;
using cxserver.Domain.Common;

namespace cxserver.Domain.WebEngine;

public sealed class Page : AggregateRoot, ISoftDeletable
{
    private readonly List<PageSection> _sections;

    private Page(
        Guid id,
        string slug,
        string title,
        string seoTitle,
        string seoDescription,
        bool isPublished,
        DateTimeOffset? publishedAtUtc,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc) : base(id)
    {
        Slug = slug;
        Title = title;
        SeoTitle = seoTitle;
        SeoDescription = seoDescription;
        IsPublished = isPublished;
        PublishedAtUtc = publishedAtUtc;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
        _sections = new List<PageSection>();
    }

    private Page() : base(Guid.NewGuid())
    {
        Slug = string.Empty;
        Title = string.Empty;
        SeoTitle = string.Empty;
        SeoDescription = string.Empty;
        _sections = new List<PageSection>();
    }

    public string Slug { get; private set; }
    public string Title { get; private set; }
    public string SeoTitle { get; private set; }
    public string SeoDescription { get; private set; }
    public bool IsPublished { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }
    public IReadOnlyCollection<PageSection> Sections => _sections.AsReadOnly();

    public static Page Create(
        Guid id,
        string slug,
        string title,
        string seoTitle,
        string seoDescription,
        DateTimeOffset nowUtc)
    {
        ValidateSlug(slug);
        ValidateTitle(title);
        ValidateSeoTitle(seoTitle);
        ValidateSeoDescription(seoDescription);

        return new Page(
            id,
            slug.Trim().ToLowerInvariant(),
            title.Trim(),
            seoTitle.Trim(),
            seoDescription.Trim(),
            false,
            null,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void Update(string title, string seoTitle, string seoDescription, DateTimeOffset nowUtc)
    {
        ValidateTitle(title);
        ValidateSeoTitle(seoTitle);
        ValidateSeoDescription(seoDescription);

        Title = title.Trim();
        SeoTitle = seoTitle.Trim();
        SeoDescription = seoDescription.Trim();
        UpdatedAtUtc = nowUtc;
    }

    public PageSection AddSection(
        Guid sectionId,
        SectionType sectionType,
        int displayOrder,
        JsonDocument sectionData,
        bool isPublished,
        DateTimeOffset nowUtc)
    {
        EnsureDisplayOrderUnique(displayOrder, null);
        var section = PageSection.Create(sectionId, Id, sectionType, displayOrder, sectionData, nowUtc, isPublished);
        _sections.Add(section);
        UpdatedAtUtc = nowUtc;
        NormalizeOrder(nowUtc);
        return section;
    }

    public PageSection UpdateSection(
        Guid sectionId,
        int displayOrder,
        JsonDocument sectionData,
        bool isPublished,
        DateTimeOffset nowUtc)
    {
        var section = _sections.FirstOrDefault(x => x.Id == sectionId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Section not found.");

        EnsureDisplayOrderUnique(displayOrder, sectionId);
        section.Update(displayOrder, sectionData, nowUtc);

        if (isPublished)
        {
            section.Publish(nowUtc);
        }
        else
        {
            section.Unpublish(nowUtc);
        }

        UpdatedAtUtc = nowUtc;
        NormalizeOrder(nowUtc);
        return section;
    }

    public void RemoveSection(Guid sectionId, DateTimeOffset nowUtc)
    {
        var section = _sections.FirstOrDefault(x => x.Id == sectionId && !x.IsDeleted)
            ?? throw new InvalidOperationException("Section not found.");

        section.Remove(nowUtc);
        UpdatedAtUtc = nowUtc;
        NormalizeOrder(nowUtc);
    }

    public void ReorderSections(IReadOnlyList<(Guid SectionId, int DisplayOrder)> orderMap, DateTimeOffset nowUtc)
    {
        var activeSections = _sections.Where(x => !x.IsDeleted).ToList();
        if (orderMap.Count != activeSections.Count)
        {
            throw new InvalidOperationException("Section reorder request is invalid.");
        }

        var distinctIds = orderMap.Select(x => x.SectionId).Distinct().Count();
        if (distinctIds != orderMap.Count)
        {
            throw new InvalidOperationException("Section reorder request contains duplicate section ids.");
        }

        var distinctOrders = orderMap.Select(x => x.DisplayOrder).Distinct().Count();
        if (distinctOrders != orderMap.Count)
        {
            throw new InvalidOperationException("Section reorder request contains duplicate display orders.");
        }

        foreach (var mapping in orderMap)
        {
            var section = activeSections.FirstOrDefault(x => x.Id == mapping.SectionId)
                ?? throw new InvalidOperationException("Section reorder request contains invalid section id.");
            section.Reorder(mapping.DisplayOrder, nowUtc);
        }

        UpdatedAtUtc = nowUtc;
        NormalizeOrder(nowUtc);
    }

    public void Publish(DateTimeOffset nowUtc)
    {
        IsPublished = true;
        PublishedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public void Unpublish(DateTimeOffset nowUtc)
    {
        IsPublished = false;
        PublishedAtUtc = null;
        UpdatedAtUtc = nowUtc;
    }

    public void Delete(DateTimeOffset deletedAtUtc)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAtUtc;
        UpdatedAtUtc = deletedAtUtc;
    }

    public void Restore(DateTimeOffset restoredAtUtc)
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = restoredAtUtc;
    }

    private void EnsureDisplayOrderUnique(int displayOrder, Guid? currentSectionId)
    {
        if (_sections.Any(x => !x.IsDeleted && x.DisplayOrder == displayOrder && x.Id != currentSectionId))
        {
            throw new InvalidOperationException("Display order must be unique per page.");
        }
    }

    private void NormalizeOrder(DateTimeOffset nowUtc)
    {
        var ordered = _sections
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.CreatedAtUtc)
            .ToList();

        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].Reorder(index, nowUtc);
        }
    }

    private static void ValidateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("Slug is required.", nameof(slug));
        }
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }
    }

    private static void ValidateSeoTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("SEO title is required.", nameof(title));
        }
    }

    private static void ValidateSeoDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("SEO description is required.", nameof(description));
        }
    }
}
