using cxserver.Domain.Common;

namespace cxserver.Domain.SliderEngine;

public sealed class SlideLayer : ISoftDeletable
{
    private SlideLayer(
        Guid id,
        Guid slideId,
        int order,
        SliderLayerType type,
        string content,
        string? mediaUrl,
        decimal positionX,
        decimal positionY,
        string width,
        SliderLayerAnimationFrom animationFrom,
        int animationDelay,
        int animationDuration,
        string animationEasing,
        string responsiveVisibility,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc,
        bool isDeleted,
        DateTimeOffset? deletedAtUtc)
    {
        Id = id;
        SlideId = slideId;
        Order = order;
        Type = type;
        Content = content;
        MediaUrl = mediaUrl;
        PositionX = positionX;
        PositionY = positionY;
        Width = width;
        AnimationFrom = animationFrom;
        AnimationDelay = animationDelay;
        AnimationDuration = animationDuration;
        AnimationEasing = animationEasing;
        ResponsiveVisibility = responsiveVisibility;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        IsDeleted = isDeleted;
        DeletedAtUtc = deletedAtUtc;
    }

    private SlideLayer()
    {
        Content = string.Empty;
        Width = "100%";
        AnimationEasing = "ease-out";
        ResponsiveVisibility = "all";
    }

    public Guid Id { get; private set; }
    public Guid SlideId { get; private set; }
    public int Order { get; private set; }
    public SliderLayerType Type { get; private set; }
    public string Content { get; private set; }
    public string? MediaUrl { get; private set; }
    public decimal PositionX { get; private set; }
    public decimal PositionY { get; private set; }
    public string Width { get; private set; }
    public SliderLayerAnimationFrom AnimationFrom { get; private set; }
    public int AnimationDelay { get; private set; }
    public int AnimationDuration { get; private set; }
    public string AnimationEasing { get; private set; }
    public string ResponsiveVisibility { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAtUtc { get; private set; }

    public static SlideLayer Create(
        Guid id,
        Guid slideId,
        int order,
        SliderLayerType type,
        string content,
        string? mediaUrl,
        decimal positionX,
        decimal positionY,
        string width,
        SliderLayerAnimationFrom animationFrom,
        int animationDelay,
        int animationDuration,
        string animationEasing,
        string responsiveVisibility,
        DateTimeOffset nowUtc)
    {
        Validate(content, width, animationEasing, responsiveVisibility);

        return new SlideLayer(
            id,
            slideId,
            order,
            type,
            content.Trim(),
            mediaUrl?.Trim(),
            positionX,
            positionY,
            width.Trim(),
            animationFrom,
            animationDelay,
            animationDuration,
            animationEasing.Trim(),
            responsiveVisibility.Trim(),
            nowUtc,
            nowUtc,
            false,
            null);
    }

    public void Update(
        int order,
        SliderLayerType type,
        string content,
        string? mediaUrl,
        decimal positionX,
        decimal positionY,
        string width,
        SliderLayerAnimationFrom animationFrom,
        int animationDelay,
        int animationDuration,
        string animationEasing,
        string responsiveVisibility,
        DateTimeOffset nowUtc)
    {
        Validate(content, width, animationEasing, responsiveVisibility);
        Order = order;
        Type = type;
        Content = content.Trim();
        MediaUrl = mediaUrl?.Trim();
        PositionX = positionX;
        PositionY = positionY;
        Width = width.Trim();
        AnimationFrom = animationFrom;
        AnimationDelay = animationDelay;
        AnimationDuration = animationDuration;
        AnimationEasing = animationEasing.Trim();
        ResponsiveVisibility = responsiveVisibility.Trim();
        UpdatedAtUtc = nowUtc;
    }

    public void Reorder(int order, DateTimeOffset nowUtc)
    {
        Order = order;
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

    private static void Validate(string content, string width, string easing, string visibility)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content is required.", nameof(content));
        }

        if (string.IsNullOrWhiteSpace(width))
        {
            throw new ArgumentException("Width is required.", nameof(width));
        }

        if (string.IsNullOrWhiteSpace(easing))
        {
            throw new ArgumentException("Animation easing is required.", nameof(easing));
        }

        if (string.IsNullOrWhiteSpace(visibility))
        {
            throw new ArgumentException("Responsive visibility is required.", nameof(visibility));
        }
    }
}
