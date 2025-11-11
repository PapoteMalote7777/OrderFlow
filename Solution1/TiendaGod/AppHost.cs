var builder = DistributedApplication.CreateBuilder(args);

//registra un servidor de postgres
var postgres = builder.AddPostgres("postgres");

//se añade una base de datos especifica
var identitydb = postgres.AddDatabase("identitydb");

//añadir la base de datos al proyecto
builder.AddProject<Projects.TiendaGod_Identity>("tiendagod-identity").WithReference(identitydb);

builder.Build().Run();
