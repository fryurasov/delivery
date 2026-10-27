using DeliveryApp.Infrastructure.Adapters.Postgres;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeliveryApp.ComponentTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public TestWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Убираем реальный контекст
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            services.Remove(descriptor);

            // Регистрируем тестовый контекст
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(_connectionString));

            // Убираем Quartz-хостинг (AssignOrdersJob): его статический логгер ломается
            // при создании второго WebApplicationFactory в одном процессе. В тестах
            // Use Case "Назначить заказ" вызывается напрямую через IMediator, без таймера.
            var quartzHostedServices = services
                .Where(d => d.ServiceType == typeof(IHostedService) &&
                            d.ImplementationType?.Namespace?.StartsWith("Quartz") == true)
                .ToList();
            foreach (var quartzHostedService in quartzHostedServices)
                services.Remove(quartzHostedService);

            // Применяем миграции
            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        });
    }
}
