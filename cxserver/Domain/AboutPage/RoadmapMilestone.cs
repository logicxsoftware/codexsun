namespace cxserver.Domain.AboutPage;

public sealed class RoadmapMilestone
{
    private RoadmapMilestone(
        Guid id,
        Guid sectionId,
        Guid tenantId,
        string year,
        string title,
        string description,
        int order)
    {
        Id = id;
        SectionId = sectionId;
        TenantId = tenantId;
        Year = year;
        Title = title;
        Description = description;
        Order = order;
    }

    private RoadmapMilestone()
    {
        Year = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid SectionId { get; private set; }
    public Guid TenantId { get; private set; }
    public string Year { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Order { get; private set; }

    public static RoadmapMilestone Create(
        Guid id,
        Guid sectionId,
        Guid tenantId,
        string year,
        string title,
        string description,
        int order)
    {
        return new RoadmapMilestone(
            id,
            sectionId,
            tenantId,
            year.Trim(),
            title.Trim(),
            description.Trim(),
            order);
    }

    public void Update(
        string year,
        string title,
        string description,
        int order)
    {
        Year = year.Trim();
        Title = title.Trim();
        Description = description.Trim();
        Order = order;
    }
}
