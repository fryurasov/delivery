using DeliveryApp.Infrastructure.Adapters.Postgres;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Infrastructure;

/// <summary>
///     Фикстура для интеграционных тестов с PostgreSQL
/// </summary>
/// <remarks>
///     Управляет жизненным циклом PostgreSQL контейнера и ApplicationDbContext
/// </remarks>
public class PostgresTestFixture : IAsyncLifetime
{
    /// <summary>
    ///     Настройка Postgres из библиотеки TestContainers
    /// </summary>
    /// <remarks>По сути это Docker контейнер с Postgres</remarks>
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14.7")
        .WithDatabase("delivery")
        .WithUsername("username")
        .WithPassword("secret")
        .WithCleanUp(true)
        .Build();

    protected ApplicationDbContext Context { get; private set; } = null!;

    /// <summary>
    ///     Инициализируем окружение
    /// </summary>
    /// <remarks>Вызывается перед каждым тестом</remarks>
    public async Task InitializeAsync()
    {
        //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
        await _postgreSqlContainer.StartAsync();

        //Накатываем миграции и справочники
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); })
            .Options;
        Context = new ApplicationDbContext(contextOptions);
        await Context.Database.MigrateAsync();
    }

    /// <summary>
    ///     Уничтожаем окружение
    /// </summary>
    /// <remarks>Вызывается после каждого теста</remarks>
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }
}