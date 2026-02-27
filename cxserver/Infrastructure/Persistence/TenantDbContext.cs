using System.Linq.Expressions;
using cxserver.Application.Abstractions;
using cxserver.Domain.Common;
using cxserver.Domain.Configuration;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ConfigurationDocumentConfiguration());
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
