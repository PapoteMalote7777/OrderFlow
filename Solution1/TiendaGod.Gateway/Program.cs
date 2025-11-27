using TiendaGod.Gateway.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddRedisClient("cache");
builder.Services.AddGatewayCors();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddGatewayAuthorizationPolicies();
builder.Services.AddRedisRateLimiting(builder.Configuration);
builder.Services.AddYarpReverseProxy(builder.Configuration);
var app = builder.Build();
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapReverseProxy();
app.Run();
