using cxserver.Domain.Common;

namespace cxserver.Domain.SliderEngine;

public sealed class SlideHighlight : ISoftDeletable
{
    private SlideHighlight(
        Guid id,
        Guid slideId,
        string text,
        string variant,
        int order,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc)
    {
        Id = id;
        SlideId = slideId;
        Text = text;
        Variant = variant;
        Order = order;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private SlideHighlight()
    {
        Text = string.Empty;
        Variant = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid SlideId { get; private set; }
    public string Text { get; private set; }
    public string Variant { get; private set; }
    public int Order { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public static SlideHighlight Create(Guid id, Guid slideId, string text, string variant, int order, DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text is required.", nameof(text));
        }

        if (string.IsNullOrWhiteSpace(variant))
        {
            throw new ArgumentException("Variant is required.", nameof(variant));
        }

        return new SlideHighlight(
            id,
            slideId,
            text.Trim(),
            variant.Trim(),
            order,
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void Delete(DateTimeOffset deletedAtUtc)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAtUtc;
        UpdatedAtUtc = deletedAtUtc;
    }

    public void Update(string text, string variant, int order, DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text is required.", nameof(text));
        }

        if (string.IsNullOrWhiteSpace(variant))
        {
            throw new ArgumentException("Variant is required.", nameof(variant));
        }

        Text = text.Trim();
        Variant = variant.Trim();
        Order = order;
        UpdatedAtUtc = nowUtc;
    }

    public void Restore(DateTimeOffset restoredAtUtc)
    {
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = restoredAtUtc;
    }
}
