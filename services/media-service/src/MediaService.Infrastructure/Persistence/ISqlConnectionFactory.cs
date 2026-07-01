using System.Data;

namespace MediaService.Infrastructure.Persistence;

public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
