using System.Data;
using System.Reflection;
using Dapper;
using ListingService.Infrastructure.Persistence.Seeding;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ListingService.Infrastructure.Persistence;

public class DatabaseInitializer
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly string _connectionString;

    public DatabaseInitializer(ISqlConnectionFactory connectionFactory, IConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    public async Task InitializeAsync()
    {
        await EnsureDatabaseExistsAsync();

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        await ApplyScriptAsync(connection, "001_InitialSchema.sql");
        await DatabaseSeeder.SeedAsync(connection);
    }

    private async Task EnsureDatabaseExistsAsync()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        var databaseName = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        await using var connection = new SqlConnection(builder.ConnectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName) CREATE DATABASE [{databaseName}]",
            new { DatabaseName = databaseName });
    }

    private static async Task ApplyScriptAsync(IDbConnection connection, string scriptFileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"ListingService.Infrastructure.Persistence.Scripts.{scriptFileName}";
        await using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"SQL script not found: {resourceName}");

        using var reader = new StreamReader(stream);
        var script = await reader.ReadToEndAsync();

        foreach (var batch in script.Split("GO", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!string.IsNullOrWhiteSpace(batch))
            {
                using var command = connection.CreateCommand();
                command.CommandText = batch;
                if (command is SqlCommand sqlCommand)
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                }
                else
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        var schemaTableExists = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'SchemaVersions'");

        if (schemaTableExists > 0)
        {
            await connection.ExecuteAsync(
                "IF NOT EXISTS (SELECT 1 FROM SchemaVersions WHERE ScriptName = @ScriptName) INSERT INTO SchemaVersions (ScriptName, AppliedAt) VALUES (@ScriptName, @AppliedAt)",
                new { ScriptName = scriptFileName, AppliedAt = DateTime.UtcNow });
        }
    }
}
