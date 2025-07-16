using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartile.Domain.Interfaces.Repositories;
using Quartile.Infrastructure.Context;
using Quartile.Infrastructure.Repositories;

namespace Quartile.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("QuartileConnection");
            
            if (connectionString?.Contains("Server=tcp:") == true)
            {
                services.AddDbContext<QuartileContext>(options =>
                    options.UseSqlServer(connectionString));
            }
            else
            {
                services.AddDbContext<QuartileContext>(options =>
                    options.UseSqlite(connectionString ?? "Data Source=sqlite.db"));
            }

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
} 