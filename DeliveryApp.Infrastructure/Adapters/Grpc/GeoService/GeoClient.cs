using Clients.Geo;
using CSharpFunctionalExtensions;
using DeliveryApp.Core;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using Errs;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Options;

namespace DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;

public class GeoClient : IGeoClient
{
    private readonly MethodConfig _methodConfig;
    private readonly SocketsHttpHandler _socketsHttpHandler;
    private readonly string _url;

    public GeoClient(IOptions<Settings> options)
    {
        if (string.IsNullOrWhiteSpace(options.Value.GeoServiceGrpcHost))
            throw new ArgumentException(nameof(options.Value.GeoServiceGrpcHost));
        _url = options.Value.GeoServiceGrpcHost;

        _socketsHttpHandler = new SocketsHttpHandler
        {
            PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
            KeepAlivePingDelay = TimeSpan.FromSeconds(60),
            KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
            EnableMultipleHttp2Connections = true
        };

        _methodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };
    }

    public async Task<Result<LocationVo, Error>> GetGeolocationAsync(AddressVo address, CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress(_url, new GrpcChannelOptions
        {
            HttpHandler = _socketsHttpHandler,
            ServiceConfig = new ServiceConfig { MethodConfigs = { _methodConfig } }
        });

        var client = new Geo.GeoClient(channel);
        var reply = await client.GetGeolocationAsync(new GetGeolocationRequest
        {
            Street = address.Street
        }, null, DateTime.UtcNow.AddSeconds(5), cancellationToken);

        return LocationVo.Create(reply.Location.X, reply.Location.Y);
    }
}
