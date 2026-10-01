using CntFixtures;
using DeliveryApp.Core;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace DeliveryApp.IntegrationTests.Postgres;

public abstract class PostgresTestBase : IAsyncLifetime
{
    private readonly PostgresFixture _postgres = new();
    
    public string ConnectionString => _postgres.ConnectionString;
    
    private DbContextOptions<ApplicationDbContext>? _options;

    public IOptions<Settings> CreateOptions()
    {
        return Options.Create(new Settings 
        { 
            ConnectionString = ConnectionString 
        });
    }
    
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