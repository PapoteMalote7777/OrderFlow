using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

//SERVIDOR DE POSTGRES
//registra un servidor de postgres
var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);
//se añade una base de datos especifica
var cositasdb = postgres.AddDatabase("cositas");

//API (BACKEND)
//añadir la base de datos al proyecto
var identityApi = builder.AddProject<Projects.TiendaGod_Identity>("tiendagod-identity")
    .WaitFor(postgres)
    .WithReference(cositasdb);

//REACT FRONTEND
var webApp = builder.AddNpmApp("TiendaGodFrontend", "../TiendaGod.Frontend", "dev") // nombre y ruta de tu app React
                    .WithReference(identityApi) // el frontend "conoce" la API
                    .WithHttpEndpoint(port: 59210, env: "PORT")
                    .WithExternalHttpEndpoints()
                    .PublishAsDockerFile()
                    //.WithEnvironment("VITE_API_URL", identityApi.GetEndpoint("http")) // pasa la URL del backend a React
                    .WaitFor(identityApi); // espera a que la API esté lista antes de arrancar el frontend

builder.Build().Run();
