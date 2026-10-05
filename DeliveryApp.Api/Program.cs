using CSharpFunctionalExtensions;
using Ddd;
using DeliveryApp.Api;
using DeliveryApp.Api.Adapters.BackgroundJobs;
using DeliveryApp.Core.Application.Commands.AssignOrder;
using DeliveryApp.Core.Application.Commands.CompleteOrder;
using DeliveryApp.Core.Application.Commands.CreateCourier;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Application.Commands.MoveCourier;
using DeliveryApp.Core.Application.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;
using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders;
using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders.Response;
using DeliveryApp.Core.Domain.Services.Complete;
using DeliveryApp.Core.Domain.Services.Dispatch;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Queries;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using Errs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin(); // Не делайте так в проде!
        });
});

// Configuration
builder.Services.ConfigureOptions<SettingsSetup>();
var connectionString = builder.Configuration["CONNECTION_STRING"];

builder.Services.AddSingleton<IDispatchService, DispatchService>();
builder.Services.AddSingleton<ICompleteService, CompleteService>();

// БД, ORM 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString,
            sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
        options.EnableSensitiveDataLogging();
    }
);

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<ICourierAggregateRepository, CourierAggregateRepository>();
builder.Services.AddScoped<IOrderAggregateRepository, OrderAggregateRepository>();

builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();
builder.Services.AddScoped<ICourierQueryService, CourierQueryService>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

// Commands
builder.Services.AddTransient<IRequestHandler<AssignOrderCommand, UnitResult<Error>>, AssignOrderCommandHandler>();
builder.Services.AddTransient<IRequestHandler<CompleteOrderCommand, UnitResult<Error>> , CompleteOrderCommandHandler>();
builder.Services.AddTransient<IRequestHandler<CreateCourierCommand, Result<Guid, Error>>, CreateCourierCommandHandler>();
builder.Services.AddTransient<IRequestHandler<CreateOrderCommand, Result<Guid, Error>>, CreateOrderCommandHandler>();
builder.Services.AddTransient<IRequestHandler<MoveCourierCommand, UnitResult<Error>>, MoveCourierCommandHandler>();
builder.Services.AddTransient<IRequestHandler<CreateOrderCommand, Result<Guid, Error>>, CreateOrderCommandHandler>();

// Queries
builder.Services.AddTransient<IRequestHandler<GetAllCouriersQuery, GetAllCouriersResponse>, GetAllCouriersQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetNotCompletedOrdersQuery, GetNotCompletedOrdersResponse>, GetNotCompletedOrdersQueryHandler>();

builder.Services.AddQuartz(configure =>
{
    var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
    configure
        .AddJob<AssignOrdersJob>(job => job.WithIdentity(assignOrdersJobKey))
        .AddTrigger(
            trigger => trigger.ForJob(assignOrdersJobKey)
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInSeconds(1)
                        .RepeatForever()));
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

// Apply Migrations
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     db.Database.Migrate();
// }

app.Run();