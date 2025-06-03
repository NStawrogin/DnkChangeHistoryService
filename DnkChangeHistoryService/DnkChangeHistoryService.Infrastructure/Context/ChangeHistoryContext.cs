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
    }
}