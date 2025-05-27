using DnkChangeHistoryService.Domain.ChangeHistoryManagement;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DnkChangeHistoryService.Infrastructure.Context;

[ExcludeFromCodeCoverage]
public abstract class ChangeHistoryContext<TContext>(DbContextOptions<TContext> options) : DbContext(options) where TContext : DbContext
{
    public DbSet<CommentHistory> CommentHistories { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("history");

        base.OnModelCreating(modelBuilder);

        ApplyConfigurations(modelBuilder);
    }

    private static void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        List<Type> configurationTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .ToList();

        foreach (object configuration in configurationTypes.Select(type => Activator.CreateInstance(type)!))
        {
            modelBuilder.ApplyConfiguration((dynamic)configuration);
        }
    }
}