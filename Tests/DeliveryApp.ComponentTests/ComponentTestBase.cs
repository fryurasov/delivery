using CntFixtures;
using Xunit;

namespace DeliveryApp.ComponentTests;

public abstract class ComponentTestBase : IAsyncLifetime
{
    protected readonly PostgresFixture Postgres = new();
    protected HttpClient Client = null!;
    protected TestWebApplicationFactory Factory = null!;

    public async Task InitializeAsync()
    {
        await Postgres.InitializeAsync();

        // CourierQueryService/OrderQueryService читают строку подключения через IOptions<Settings>
        // (Dapper, мимо DbContext), поэтому подмены DbContextOptions в TestWebApplicationFactory
        // недостаточно - им отдельно нужна переменная окружения.
        Environment.SetEnvironmentVariable("CONNECTION_STRING", Postgres.ConnectionString);

        Factory = new TestWebApplicationFactory(Postgres.ConnectionString);
        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();

        await Postgres.DisposeAsync();
    }
}
