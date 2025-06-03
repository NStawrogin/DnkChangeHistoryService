using DnkChangeHistoryService.Application.CommentHistories.Mapper;
using DnkChangeHistoryService.Application.CommentHistories.Queries.History;
using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;
using DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;
using FakeItEasy;
using Microsoft.Extensions.Logging.Testing;
using NUnit.Framework;

namespace DnkChangeHistoryService.Tests.Application.CommentHistories.Queries;

[TestFixture]
public class CommentHistoryQueryHandlerTests
{
    private CommentHistoryQueryHandler handler;
    private FakeLogger<CommentHistoryQueryHandler> logger;
    private ICommentHistoryQueryRepository queryRepository;
    private ICommentHistoryMapper mapper;

    [SetUp]
    public void SetUp()
    {
        this.logger = new FakeLogger<CommentHistoryQueryHandler>();
        this.queryRepository = A.Fake<ICommentHistoryQueryRepository>();
        this.mapper = new CommentHistoryMapper();
        this.handler = new CommentHistoryQueryHandler(this.logger, this.queryRepository, this.mapper);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenNoHistoryExists()
    {
        // Arrange
        Guid commentId = Guid.NewGuid();
        CommentHistoryQuery request = new() { CommentId = commentId, Language = "en" };
        A.CallTo(() => this.queryRepository.GetCommentHistories(commentId, A<CancellationToken>.Ignored)).Returns(Task.FromResult<IEnumerable<CommentHistory>>(null));

        // Act
        List<CommentHistoryResponse> result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_ShouldReturnMappedResponse_WhenHistoryExists()
    {
        // Arrange
        Guid commentId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        CommentHistoryQuery request = new() { CommentId = commentId, Language = "de" };

        List<CommentHistory> fakeHistories = new()
        {
            new CommentHistory
            {
                CommentId = commentId,
                UserId = userId,
                Reason = CommentHistoryReason.Created,
                CreationDate = DateTime.UtcNow
            }
        };

        A.CallTo(() => this.queryRepository.GetCommentHistories(commentId, A<CancellationToken>.Ignored))
            .Returns(fakeHistories);

        // Act
        List<CommentHistoryResponse> result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result[0].CommentId, Is.EqualTo(commentId));
        Assert.That(result[0].UserId, Is.EqualTo(userId));
        Assert.That(result[0].Event, Is.EqualTo("Kommentar erstellt"));
    }
}