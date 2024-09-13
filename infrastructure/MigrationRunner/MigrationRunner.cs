using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace infrastructure.MigrationRunner;
public class MigrationRunner
{
    private readonly NpgsqlDataSource _dataSource;

    public MigrationRunner(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task ApplyMigrationsAsync()
    {
        await using var connection = await _dataSource.OpenConnectionAsync();

        // Ensure the appliedmigrations table exists
        await EnsureAppliedMigrationsTableExistsAsync(connection);

        // Get the already applied migrations
        var appliedMigrations = await GetAppliedMigrationsAsync(connection);

        // Get all migration scripts from the Migrations folder
        var migrationFiles = Directory.GetFiles("Migrations", "*.sql").OrderBy(f => f);

        foreach (var file in migrationFiles)
        {
            var migrationName = Path.GetFileName(file);
            Console.WriteLine($"Applying migration: {migrationName}");
            if (!appliedMigrations.Contains(migrationName))
            {
                var script = await File.ReadAllTextAsync(file);

                await using var command = new NpgsqlCommand(script, connection);
                await command.ExecuteNonQueryAsync();

                // Insert the migration name into the appliedmigrations table
                var insertCommand = new NpgsqlCommand("INSERT INTO appliedmigrations (MigrationName) VALUES (@MigrationName)", connection);
                insertCommand.Parameters.AddWithValue("MigrationName", migrationName);
                await insertCommand.ExecuteNonQueryAsync();
            }
        }
    }

    private async Task EnsureAppliedMigrationsTableExistsAsync(NpgsqlConnection connection)
    {
        var query = @"
            CREATE TABLE IF NOT EXISTS appliedmigrations (
                MigrationName VARCHAR(255) PRIMARY KEY
            )";
        await using var command = new NpgsqlCommand(query, connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<HashSet<string>> GetAppliedMigrationsAsync(NpgsqlConnection connection)
    {
        var appliedMigrations = new HashSet<string>();

        var query = "SELECT MigrationName FROM appliedmigrations";
        await using var command = new NpgsqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            appliedMigrations.Add(reader.GetString(0));
        }

        return appliedMigrations;
    }
}