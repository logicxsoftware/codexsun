namespace cxserver.Domain.AboutPage;

public sealed class Testimonial
{
    private Testimonial(
        Guid id,
        Guid sectionId,
        Guid tenantId,
        string name,
        string? company,
        string quote,
        int? rating,
        int order)
    {
        Id = id;
        SectionId = sectionId;
        TenantId = tenantId;
        Name = name;
        Company = company;
        Quote = quote;
        Rating = rating;
        Order = order;
    }

    private Testimonial()
    {
        Name = string.Empty;
        Quote = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid SectionId { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; }
    public string? Company { get; private set; }
    public string Quote { get; private set; }
    public int? Rating { get; private set; }
    public int Order { get; private set; }

    public static Testimonial Create(
        Guid id,
        Guid sectionId,
        Guid tenantId,
        string name,
        string? company,
        string quote,
        int? rating,
        int order)
    {
        return new Testimonial(
            id,
            sectionId,
            tenantId,
            name.Trim(),
            company?.Trim(),
            quote.Trim(),
            rating,
            order);
    }

    public void Update(
        string name,
        string? company,
        string quote,
        int? rating,
        int order)
    {
        Name = name.Trim();
        Company = company?.Trim();
        Quote = quote.Trim();
        Rating = rating;
        Order = order;
    }
}
