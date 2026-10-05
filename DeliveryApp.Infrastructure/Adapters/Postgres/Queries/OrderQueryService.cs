using Dapper;
using DeliveryApp.Core;
using DeliveryApp.Core.Application.Queries.Common.Dto;
using DeliveryApp.Core.Application.Queries.GetNotCompletedOrders.Response;
using DeliveryApp.Core.Domain.Models.Order;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Queries;

public class OrderQueryService : IOrderQueryService
{
    private readonly string _connectionString;
    
    public OrderQueryService(IOptions<Settings> options)
    {
        _connectionString = options.Value.ConnectionString 
                            ?? throw new ArgumentException("Connection string is required");
    }
    
    public async Task<GetNotCompletedOrdersResponse> GetNotCompletedOrders()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var orders = await connection.QueryAsync<OrderDto, LocationDto, OrderDto>(
            """
                SELECT
                    o.Id AS "Id",               
                    o.location_x AS "X",
                    o.location_y AS "Y"
                FROM "order" o
                WHERE o.status != @status
            """,
            (order, location) =>
            {
                order.Location = location;
                return order;
            },
            param: new { status = OrderStatusVo.OrderStatusEnum.Completed },
            splitOn: "X"
        );
        
        return new GetNotCompletedOrdersResponse { Orders = orders.ToList() };
    }
}