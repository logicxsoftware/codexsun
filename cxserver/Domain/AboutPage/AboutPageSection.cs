using cxserver.Domain.Common;

namespace cxserver.Domain.AboutPage;

public sealed class AboutPageSection : AggregateRoot
{
    private readonly List<TeamMember> _teamMembers;
    private readonly List<Testimonial> _testimonials;
    private readonly List<RoadmapMilestone> _roadmapMilestones;

    private AboutPageSection(
        Guid id,
        Guid tenantId,
        string heroTitle,
        string heroSubtitle,
        string aboutTitle,
        string aboutSubtitle,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc) : base(id)
    {
        TenantId = tenantId;
        HeroTitle = heroTitle;
        HeroSubtitle = heroSubtitle;
        AboutTitle = aboutTitle;
        AboutSubtitle = aboutSubtitle;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        _teamMembers = new List<TeamMember>();
        _testimonials = new List<Testimonial>();
        _roadmapMilestones = new List<RoadmapMilestone>();
    }

    private AboutPageSection() : base(Guid.NewGuid())
    {
        HeroTitle = string.Empty;
        HeroSubtitle = string.Empty;
        AboutTitle = string.Empty;
        AboutSubtitle = string.Empty;
        _teamMembers = new List<TeamMember>();
        _testimonials = new List<Testimonial>();
        _roadmapMilestones = new List<RoadmapMilestone>();
    }

    public Guid TenantId { get; private set; }
    public string HeroTitle { get; private set; }
    public string HeroSubtitle { get; private set; }
    public string AboutTitle { get; private set; }
    public string AboutSubtitle { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public IReadOnlyCollection<TeamMember> TeamMembers => _teamMembers.AsReadOnly();
    public IReadOnlyCollection<Testimonial> Testimonials => _testimonials.AsReadOnly();
    public IReadOnlyCollection<RoadmapMilestone> RoadmapMilestones => _roadmapMilestones.AsReadOnly();

    public static AboutPageSection Create(
        Guid id,
        Guid tenantId,
        string heroTitle,
        string heroSubtitle,
        string aboutTitle,
        string aboutSubtitle,
        DateTimeOffset nowUtc)
    {
        return new AboutPageSection(
            id,
            tenantId,
            heroTitle.Trim(),
            heroSubtitle.Trim(),
            aboutTitle.Trim(),
            aboutSubtitle.Trim(),
            nowUtc,
            nowUtc);
    }

    public void Update(
        string heroTitle,
        string heroSubtitle,
        string aboutTitle,
        string aboutSubtitle,
        DateTimeOffset nowUtc)
    {
        HeroTitle = heroTitle.Trim();
        HeroSubtitle = heroSubtitle.Trim();
        AboutTitle = aboutTitle.Trim();
        AboutSubtitle = aboutSubtitle.Trim();
        UpdatedAtUtc = nowUtc;
    }
}
