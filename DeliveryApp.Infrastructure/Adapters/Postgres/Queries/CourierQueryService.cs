using Dapper;
using DeliveryApp.Core;
using DeliveryApp.Core.Application.Queries.Common.Dto;
using DeliveryApp.Core.Application.Queries.GetAllCouriers.Response;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Queries;

public class CourierQueryService : ICourierQueryService
{
    private readonly string _connectionString;
    
    public CourierQueryService(IOptions<Settings> options)
    {
        _connectionString = options.Value.ConnectionString 
                            ?? throw new ArgumentException("Connection string is required");
    }
    
    public async Task<GetAllCouriersResponse> GetAllCouriers()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        
        var couriers = await connection.QueryAsync<CourierDto, LocationDto, CourierDto>(
            """
                SELECT
                    c.Id AS "Id",               
                    c.name AS "Name",
                    c.x AS "X",
                    c.y AS "Y"
                FROM courier c
            """,
            (courier, location) =>
            {
                courier.Location = location;
                return courier;
            },
            splitOn: "X"
        );
        
        return new GetAllCouriersResponse { Couriers = couriers.ToList() };
    }
}