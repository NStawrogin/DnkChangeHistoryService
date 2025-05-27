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
        //builder.ToTable("CommentHistories");

        //SeedData(builder);
    }

    //private static void SeedData(EntityTypeBuilder<CommentHistory> builder)
    //{
    //    builder.HasData(
    //        new List<CommentHistory>
    //        {
    //            new()
    //            {
    //                CreationDate = new DateTime(2024, 10, 11),
    //                Id = Guid.Parse("a70e9d17-78d1-45e4-9f1f-58b587edcc7c"),
    //                LanguageTwoLetterCode = "de"
    //            }
    //        }
    //    );
    //}
}