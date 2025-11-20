using Aspire.Hosting;
using Projects;
using StackExchange.Redis;

var builder = DistributedApplication.CreateBuilder(args);

// ============================================
// INFRASTRUCTURE - PostgreSQL
// ============================================
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume(isReadOnly: false)
    .WithLifetime(ContainerLifetime.Persistent);

//se añade una base de datos especifica
var cositasdb = postgres.AddDatabase("cositas");

//redis - Distributed cache for rate limiting, sessions, and caching 
var redis = builder.AddRedis("cache")
    .WithDataVolume("tiendagod-redis-data")
    .WithHostPort(6379)
    .WithLifetime(ContainerLifetime.Persistent);

// ============================================
// MICROSERVICIOS
// API IDENTITY(BACKEND)s
// ============================================
var identityApi = builder.AddProject<Projects.TiendaGod_Identity>("tiendagod-identity")
    .WaitFor(postgres)
    .WithReference(cositasdb);

// ============================================
// API GATEWAY
// ============================================
// API Gateway acts as the single entry point for all client requests
// It handles authentication, authorization, rate limiting, and routes to microservices
var apiGateway = builder.AddProject<Projects.TiendaGod_Gateway>("tiendagod-gateway")
    .WithReference(redis)
    .WithReference(identityApi)
    .WaitFor(identityApi);

// ============================================
// REACT - FRONTEND
// ============================================
var webApp = builder.AddNpmApp("TiendaGodFrontend", "../TiendaGod.Frontend", "dev") // nombre y ruta de tu app React
                    .WithReference(apiGateway)
                    .WithEnvironment("VITE_API_GATEWAY_URL", apiGateway.GetEndpoint("https"))
                    .WithHttpEndpoint(port: 59210, env: "PORT")
                    .WithExternalHttpEndpoints()
                    .PublishAsDockerFile();

builder.AddProject<Projects.TiendaGod_Catalogo>("tiendagod-catalogo");

builder.Build().Run();
