using Microsoft.EntityFrameworkCore;
using TiendaGod.Productos.Data;

var builder = WebApplication.CreateBuilder(args);

// SERVICIOS POR DEFECTO
builder.AddServiceDefaults();

builder.AddNpgsqlDbContext<ProductDbContext>("productos");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ No se pudo aplicar migraciones: {ex.Message}");
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
