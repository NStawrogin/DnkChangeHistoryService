using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;
using DnkChangeHistoryService.Infrastructure.Context;
using DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;
using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using MockQueryable.FakeItEasy;
using NUnit.Framework;

namespace DnkChangeHistoryService.Tests.Infrastructure.Repositories.CommentHistories.Queries;

[TestFixture]
public class CommentHistoryQueryRepositoryTests
{
    private ChangeHistoryWorkContext context = null!;

    [Test]
    public async Task GetCommentHistories_ShouldReturnMatchingHistories()
    {
        // Arrange
        this.context = this.CreateContext();
        CommentHistoryQueryRepository repository = new(this.context);
        Guid targetReportId = Guid.Parse("0fc5e6ef-a22a-46ff-be50-4881c16062eb");
        string targetContextId = "SomeContextId"; // Provide a valid context ID
        CancellationToken cancellationToken = CancellationToken.None;

        List<CommentHistory> fakeData = new()
    {
        new CommentHistory
        {
            Id = Guid.NewGuid(),
            ReportId = targetReportId,
            ContextId = targetContextId,
            DisclosureRequirementId = "Requirement1",
            CommentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CreationDate = DateTime.UtcNow,
            Reason = CommentHistoryReason.Created
        },
        new CommentHistory
        {
            Id = Guid.NewGuid(),
            ReportId = targetReportId,
            ContextId = targetContextId,
            DisclosureRequirementId = "Requirement2",
            CommentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CreationDate = DateTime.UtcNow,
            Reason = CommentHistoryReason.Created
        },
        new CommentHistory
        {
            Id = Guid.NewGuid(),
            ReportId = Guid.NewGuid(),
            ContextId = "DifferentContextId",
            DisclosureRequirementId = "Requirement3",
            CommentId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CreationDate = DateTime.UtcNow,
            Reason = CommentHistoryReason.Created
        }
    };

        IQueryable<CommentHistory> queryableData = fakeData.AsQueryable();
        DbSet<CommentHistory> dbSet = queryableData.BuildMockDbSet();
        this.context.CommentHistories = dbSet;

        // Act
        IEnumerable<CommentHistory> result = await repository.GetCommentHistories(targetReportId, targetContextId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(h => h.ReportId == targetReportId && h.ContextId == targetContextId), Is.True);
    }

    [Test]
    public async Task GetCommentHistories_ShouldReturnEmptyList_WhenNoMatch()
    {
        // Arrange
        this.context = this.CreateContext();
        CommentHistoryQueryRepository repository = new(this.context);
        string targetContextId = "targetContextId";
        Guid targetReportId = Guid.Parse("0fc5e6ef-a22a-46ff-be50-4881c16062eb");
        CancellationToken cancellationToken = CancellationToken.None;

        List<CommentHistory> fakeData = new()
        {
            new CommentHistory
            {
                ReportId = Guid.NewGuid(),
                ContextId = "DifferentContextId",
                DisclosureRequirementId = "Requirement3",
                Id = Guid.NewGuid(),
                CommentId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Reason = CommentHistoryReason.Created
            }
        };

        IQueryable<CommentHistory> queryableData = fakeData.AsQueryable();
        DbSet<CommentHistory> dbSet = queryableData.BuildMockDbSet();
        this.context.CommentHistories = dbSet;

        // Act
        IEnumerable<CommentHistory> result = await repository.GetCommentHistories(targetReportId, targetContextId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    private ChangeHistoryWorkContext CreateContext()
    {
        DbContextOptionsBuilder<ChangeHistoryWorkContext> builder = new();
        ChangeHistoryWorkContext context = A.Fake<ChangeHistoryWorkContext>(options =>
        {
            options.WithArgumentsForConstructor(() => new ChangeHistoryWorkContext(builder.Options));
        });

        return context;
    }
}
