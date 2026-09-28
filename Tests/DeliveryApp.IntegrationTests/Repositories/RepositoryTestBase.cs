using CntFixtures;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public abstract class RepositoryTestBase : IAsyncLifetime
{
    private readonly PostgresFixture _postgres = new();
    
    private DbContextOptions<ApplicationDbContext>? _options;

    public ApplicationDbContext CreateDbContext()
    {
        if (_options == null)
            throw new InvalidOperationException("DbContext options are not initialized.");

        return new ApplicationDbContext(_options);
    }

    public async Task InitializeAsync()
    {
        // 1. Поднимаем Postgres через fixture
        await _postgres.InitializeAsync();

        // 2. Создаём DbContext поверх fixture
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                _postgres.ConnectionString,
                x => x.MigrationsAssembly("DeliveryApp.Infrastructure"))
            .Options;

        // 3. Гарантируем схему
        await using var dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}