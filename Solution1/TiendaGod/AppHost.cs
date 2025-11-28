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

var cositasdb = postgres.AddDatabase("cositas");
var productosdb = postgres.AddDatabase("productos");
 
var redis = builder.AddRedis("cache")
    .WithDataVolume("tiendagod-redis-data")
    .WithHostPort(6379)
    .WithLifetime(ContainerLifetime.Persistent);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume("orderflow-rabbitmq-data")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

// ============================================
// MICROSERVICIOS
// ============================================
var identityApi = builder.AddProject<Projects.TiendaGod_Identity>("tiendagod-identity")
    .WaitFor(postgres)
    .WithReference(rabbitmq)
    .WithReference(cositasdb)
    .WaitFor(rabbitmq);

var productApi = builder.AddProject<Projects.TiendaGod_Productos>("tiendagod-productos")
    .WaitFor(postgres)
    .WithReference(rabbitmq)
    .WithReference(productosdb)
    .WaitFor(rabbitmq);

// ============================================
// API GATEWAY
// ============================================
var apiGateway = builder.AddProject<Projects.TiendaGod_Gateway>("tiendagod-gateway")
    .WithReference(redis)
    .WithReference(identityApi)
    .WithReference(productApi)
    .WaitFor(identityApi)
    .WaitFor(productApi);

// ============================================
// REACT - FRONTEND
// ============================================
var webApp = builder.AddNpmApp("TiendaGodFrontend", "../TiendaGod.Frontend", "dev")
                    .WithReference(apiGateway)
                    .WithEnvironment("VITE_API_GATEWAY_URL", apiGateway.GetEndpoint("https"))
                    .WithHttpEndpoint(port: 59210, env: "PORT")
                    .WithExternalHttpEndpoints()
                    .PublishAsDockerFile();

builder.AddProject<Projects.TiendaGod_Notifications>("tiendagod-notifications")
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.Build().Run();
