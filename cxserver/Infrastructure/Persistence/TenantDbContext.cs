using System.Linq.Expressions;
using cxserver.Domain.AboutPage;
using cxserver.Application.Abstractions;
using cxserver.Domain.BlogEngine;
using cxserver.Domain.ContactEngine;
using cxserver.Domain.Common;
using cxserver.Domain.Configuration;
using cxserver.Domain.MenuEngine;
using cxserver.Domain.NavigationEngine;
using cxserver.Domain.ProductCatalog;
using cxserver.Domain.SliderEngine;
using cxserver.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace cxserver.Infrastructure.Persistence;

public sealed class TenantDbContext : DbContext
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public TenantDbContext(DbContextOptions<TenantDbContext> options, IDateTimeProvider dateTimeProvider) : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public DbSet<ConfigurationDocument> ConfigurationDocuments => Set<ConfigurationDocument>();
    public DbSet<cxserver.Domain.WebEngine.Page> WebsitePages => Set<cxserver.Domain.WebEngine.Page>();
    public DbSet<cxserver.Domain.WebEngine.PageSection> WebsitePageSections => Set<cxserver.Domain.WebEngine.PageSection>();
    public DbSet<MenuGroup> MenuGroups => Set<MenuGroup>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<WebNavigationConfig> WebNavigationConfigs => Set<WebNavigationConfig>();
    public DbSet<FooterConfig> FooterConfigs => Set<FooterConfig>();
    public DbSet<SliderConfig> SliderConfigs => Set<SliderConfig>();
    public DbSet<Slide> Slides => Set<Slide>();
    public DbSet<SlideLayer> SlideLayers => Set<SlideLayer>();
    public DbSet<SlideHighlight> SlideHighlights => Set<SlideHighlight>();
    public DbSet<AboutPageSection> AboutPageSections => Set<AboutPageSection>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<RoadmapMilestone> RoadmapMilestones => Set<RoadmapMilestone>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();
    public DbSet<BlogTag> BlogTags => Set<BlogTag>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<BlogPostTag> BlogPostTags => Set<BlogPostTag>();
    public DbSet<BlogComment> BlogComments => Set<BlogComment>();
    public DbSet<BlogLike> BlogLikes => Set<BlogLike>();
    public DbSet<BlogPostImage> BlogPostImages => Set<BlogPostImage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ConfigurationDocumentConfiguration());
        modelBuilder.ApplyConfiguration(new WebsitePageConfiguration());
        modelBuilder.ApplyConfiguration(new WebsitePageSectionConfiguration());
        modelBuilder.ApplyConfiguration(new MenuGroupConfiguration());
        modelBuilder.ApplyConfiguration(new MenuConfiguration());
        modelBuilder.ApplyConfiguration(new MenuItemConfiguration());
        modelBuilder.ApplyConfiguration(new WebNavigationConfigConfiguration());
        modelBuilder.ApplyConfiguration(new FooterConfigConfiguration());
        modelBuilder.ApplyConfiguration(new SliderConfigConfiguration());
        modelBuilder.ApplyConfiguration(new SlideConfiguration());
        modelBuilder.ApplyConfiguration(new SlideLayerConfiguration());
        modelBuilder.ApplyConfiguration(new SlideHighlightConfiguration());
        modelBuilder.ApplyConfiguration(new AboutPageSectionConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMemberConfiguration());
        modelBuilder.ApplyConfiguration(new AboutTestimonialConfiguration());
        modelBuilder.ApplyConfiguration(new RoadmapMilestoneConfiguration());
        modelBuilder.ApplyConfiguration(new ContactMessageConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
        modelBuilder.ApplyConfiguration(new BlogCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new BlogTagConfiguration());
        modelBuilder.ApplyConfiguration(new BlogPostConfiguration());
        modelBuilder.ApplyConfiguration(new BlogPostTagConfiguration());
        modelBuilder.ApplyConfiguration(new BlogCommentConfiguration());
        modelBuilder.ApplyConfiguration(new BlogLikeConfiguration());
        modelBuilder.ApplyConfiguration(new BlogPostImageConfiguration());
        ApplySoftDeleteFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = _dateTimeProvider.UtcNow;

        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>().Where(x => x.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.Delete(now);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            if (!typeof(ISoftDeletable).IsAssignableFrom(clrType))
            {
                continue;
            }

            var parameter = Expression.Parameter(clrType, "e");
            var isDeleted = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
            var notDeleted = Expression.Equal(isDeleted, Expression.Constant(false));
            var filter = Expression.Lambda(notDeleted, parameter);

            modelBuilder.Entity(clrType).HasQueryFilter(filter);
        }
    }
}
