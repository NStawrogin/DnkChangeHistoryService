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
        Guid targetCommentId = Guid.NewGuid();
        CancellationToken cancellationToken = CancellationToken.None;

        List<CommentHistory> fakeData = new()
        {
            new CommentHistory
            {
                Id = Guid.NewGuid(),
                CommentId = targetCommentId,
                UserId = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Reason = CommentHistoryReason.Created
            },
            new CommentHistory
            {
                Id = Guid.NewGuid(),
                CommentId = targetCommentId,
                UserId = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
                Reason = CommentHistoryReason.Created
            },
            new CommentHistory
            {
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
        IEnumerable<CommentHistory> result = await repository.GetCommentHistories(targetCommentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(h => h.CommentId == targetCommentId), Is.True);
    }

    [Test]
    public async Task GetCommentHistories_ShouldReturnEmptyList_WhenNoMatch()
    {
        // Arrange
        this.context = this.CreateContext();
        CommentHistoryQueryRepository repository = new(this.context);
        Guid targetCommentId = Guid.NewGuid();
        CancellationToken cancellationToken = CancellationToken.None;

        List<CommentHistory> fakeData = new()
        {
            new CommentHistory
            {
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
        IEnumerable<CommentHistory> result = await repository.GetCommentHistories(targetCommentId, cancellationToken);

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
