using Dapper;
using ListingService.Application.Interfaces;

namespace ListingService.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public ReportRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(Guid listingId, Guid? reporterUserId, string reason, string? comment)
    {
        var id = Guid.NewGuid();
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(
            @"INSERT INTO ListingReports (Id, ListingId, ReporterUserId, Reason, Comment, Status, CreatedAt)
              VALUES (@Id, @ListingId, @ReporterUserId, @Reason, @Comment, 'Pending', @CreatedAt)",
            new
            {
                Id = id,
                ListingId = listingId,
                ReporterUserId = reporterUserId,
                Reason = reason,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            });
        return id;
    }
}
