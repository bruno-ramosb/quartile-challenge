using Quartile.Api.Extensions;
using Quartile.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

await ConfigureServices.ApplyMigrationsAsync(app.Services);

app.ConfigureApplication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
