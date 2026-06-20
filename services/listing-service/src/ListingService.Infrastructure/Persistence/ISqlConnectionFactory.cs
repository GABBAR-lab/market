using System.Data;

namespace ListingService.Infrastructure.Persistence;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
