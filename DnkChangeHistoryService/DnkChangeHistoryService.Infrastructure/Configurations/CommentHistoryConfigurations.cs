using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Infrastructure.Configurations;

[ExcludeFromCodeCoverage]
public class CommentHistoryConfigurations : IEntityTypeConfiguration<CommentHistory>
{
    public void Configure(EntityTypeBuilder<CommentHistory> builder)
    {
        builder.ToTable("CommentHistories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CreationDate).IsRequired();
        builder.Property(c => c.UserId).IsRequired();
        builder.Property(c => c.CommentId).IsRequired();
        builder.Property(c => c.Reason).IsRequired();
    }
}
