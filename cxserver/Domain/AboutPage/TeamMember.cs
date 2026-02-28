namespace cxserver.Domain.AboutPage;

public sealed class TeamMember
{
    private TeamMember(
        Guid id,
        Guid sectionId,
        Guid tenantId,
        string name,
        string role,
        string bio,
        string image,
        int order)
    {
        Id = id;
        SectionId = sectionId;
        TenantId = tenantId;
        Name = name;
        Role = role;
        Bio = bio;
        Image = image;
        Order = order;
    }

    private TeamMember()
    {
        Name = string.Empty;
        Role = string.Empty;
        Bio = string.Empty;
        Image = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid SectionId { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string Role { get; private set; }
    public string Bio { get; private set; }
    public string Image { get; private set; }
    public int Order { get; private set; }

    public static TeamMember Create(
        Guid id,
        Guid sectionId,
        Guid tenantId,
        string name,
        string role,
        string bio,
        string image,
        int order)
    {
        return new TeamMember(
            id,
            sectionId,
            tenantId,
            name.Trim(),
            role.Trim(),
            bio.Trim(),
            image.Trim(),
            order);
    }

    public void Update(
        string name,
        string role,
        string bio,
        string image,
        int order)
    {
        Name = name.Trim();
        Role = role.Trim();
        Bio = bio.Trim();
        Image = image.Trim();
        Order = order;
    }
}
