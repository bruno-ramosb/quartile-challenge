using Quartile.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.ConfigureApplication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
