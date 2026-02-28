using System.Text.Json;
using cxserver.Domain.AboutPage;
using cxserver.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Seeding;

internal sealed class TenantAboutPageSeeder
{
    private readonly Application.Abstractions.IDateTimeProvider _dateTimeProvider;

    public TenantAboutPageSeeder(Application.Abstractions.IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task SeedAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;
        var tenantId = await ResolveTenantIdAsync(dbContext, cancellationToken);

        var section = await dbContext.AboutPageSections
            .Include(x => x.TeamMembers)
            .Include(x => x.Testimonials)
            .Include(x => x.RoadmapMilestones)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

        if (section is null)
        {
            section = AboutPageSection.Create(
                Guid.NewGuid(),
                tenantId,
                "About Techmedia Retail",
                "Delivering Trusted IT Solutions in Tiruppur Since 2002",
                "Our Story",
                "Two decades of innovation and customer trust",
                now);
            await dbContext.AboutPageSections.AddAsync(section, cancellationToken);
        }
        else
        {
            section.Update(
                "About Techmedia Retail",
                "Delivering Trusted IT Solutions in Tiruppur Since 2002",
                "Our Story",
                "Two decades of innovation and customer trust",
                now);
        }

        SyncTeamMembers(dbContext, section, tenantId);
        SyncTestimonials(dbContext, section, tenantId);
        SyncRoadmap(dbContext, section, tenantId);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void SyncTeamMembers(TenantDbContext dbContext, AboutPageSection section, Guid tenantId)
    {
        var definitions = new[]
        {
            new TeamSeed("Sundar Raj", "Founder & Managing Director", "Leads strategic growth and customer-first technology initiatives across retail and enterprise business.", "https://images.unsplash.com/photo-1560250097-0b93528c311a?w=400&q=80&auto=format&fit=crop", 1),
            new TeamSeed("Meena Prakash", "Enterprise Solutions Head", "Designs workstation, networking, and enterprise procurement solutions for business clients.", "https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=400&q=80&auto=format&fit=crop", 2),
            new TeamSeed("Arun Kumar", "Gaming & Custom Build Specialist", "Builds high-performance gaming rigs and creator systems optimized for airflow and reliability.", "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=400&q=80&auto=format&fit=crop", 3),
            new TeamSeed("Vidhya N", "Service Operations Manager", "Oversees diagnostics, repairs, and onsite setup delivery for retail and corporate customers.", "https://images.unsplash.com/photo-1580489944761-15a19d654956?w=400&q=80&auto=format&fit=crop", 4),
        };

        var existing = section.TeamMembers.ToDictionary(x => x.Order, x => x);

        foreach (var definition in definitions)
        {
            if (existing.TryGetValue(definition.Order, out var member))
            {
                member.Update(definition.Name, definition.Role, definition.Bio, definition.Image, definition.Order);
                continue;
            }

            dbContext.TeamMembers.Add(TeamMember.Create(
                Guid.NewGuid(),
                section.Id,
                tenantId,
                definition.Name,
                definition.Role,
                definition.Bio,
                definition.Image,
                definition.Order));
        }

        var stale = section.TeamMembers.Where(x => !definitions.Any(d => d.Order == x.Order)).ToList();
        if (stale.Count > 0)
        {
            dbContext.TeamMembers.RemoveRange(stale);
        }
    }

    private static void SyncTestimonials(TenantDbContext dbContext, AboutPageSection section, Guid tenantId)
    {
        var definitions = new[]
        {
            new TestimonialSeed("Karthik S", "Nexa Garments", "Techmedia helped us standardize 120+ office systems with smooth deployment and dependable after-sales support.", 5, 1),
            new TestimonialSeed("Priya R", "Skyline Studios", "Our editing workstations were delivered and tuned exactly as promised. Performance and reliability are excellent.", 5, 2),
            new TestimonialSeed("Aravind M", "Velocity Esports Club", "From gaming rigs to monitors, every recommendation was practical and value-focused for our team setup.", 4, 3),
        };

        var existing = section.Testimonials.ToDictionary(x => x.Order, x => x);

        foreach (var definition in definitions)
        {
            if (existing.TryGetValue(definition.Order, out var testimonial))
            {
                testimonial.Update(definition.Name, definition.Company, definition.Quote, definition.Rating, definition.Order);
                continue;
            }

            dbContext.Testimonials.Add(Testimonial.Create(
                Guid.NewGuid(),
                section.Id,
                tenantId,
                definition.Name,
                definition.Company,
                definition.Quote,
                definition.Rating,
                definition.Order));
        }

        var stale = section.Testimonials.Where(x => !definitions.Any(d => d.Order == x.Order)).ToList();
        if (stale.Count > 0)
        {
            dbContext.Testimonials.RemoveRange(stale);
        }
    }

    private static void SyncRoadmap(TenantDbContext dbContext, AboutPageSection section, Guid tenantId)
    {
        var definitions = new[]
        {
            new RoadmapSeed("2002", "Founded", "Started as a local computer retail and service center in Tiruppur.", 1),
            new RoadmapSeed("2008", "Expanded Showroom", "Scaled inventory and introduced dedicated hardware zones for business and home users.", 2),
            new RoadmapSeed("2015", "Enterprise Division Launched", "Launched bulk procurement and managed IT solutions for institutions and companies.", 3),
            new RoadmapSeed("2020", "Online Expansion", "Expanded digital channels for product discovery, support, and remote consultations.", 4),
            new RoadmapSeed("2024", "12,000+ Products Milestone", "Crossed 12,000+ active products with stronger logistics and service response.", 5),
        };

        var existing = section.RoadmapMilestones.ToDictionary(x => x.Order, x => x);

        foreach (var definition in definitions)
        {
            if (existing.TryGetValue(definition.Order, out var milestone))
            {
                milestone.Update(definition.Year, definition.Title, definition.Description, definition.Order);
                continue;
            }

            dbContext.RoadmapMilestones.Add(RoadmapMilestone.Create(
                Guid.NewGuid(),
                section.Id,
                tenantId,
                definition.Year,
                definition.Title,
                definition.Description,
                definition.Order));
        }

        var stale = section.RoadmapMilestones.Where(x => !definitions.Any(d => d.Order == x.Order)).ToList();
        if (stale.Count > 0)
        {
            dbContext.RoadmapMilestones.RemoveRange(stale);
        }
    }

    private static async Task<Guid> ResolveTenantIdAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        var bootstrap = await dbContext.ConfigurationDocuments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NamespaceKey == "tenant" && x.DocumentKey == "bootstrap", cancellationToken);

        if (bootstrap is not null &&
            bootstrap.Payload.RootElement.TryGetProperty("tenantId", out var tenantIdElement) &&
            tenantIdElement.ValueKind == JsonValueKind.String &&
            Guid.TryParse(tenantIdElement.GetString(), out var parsedTenantId))
        {
            return parsedTenantId;
        }

        return Guid.Empty;
    }

    private sealed record TeamSeed(string Name, string Role, string Bio, string Image, int Order);
    private sealed record TestimonialSeed(string Name, string Company, string Quote, int? Rating, int Order);
    private sealed record RoadmapSeed(string Year, string Title, string Description, int Order);
}
