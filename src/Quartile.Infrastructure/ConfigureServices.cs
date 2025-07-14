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
            services.AddDbContext<QuartileContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("QuartileConnection")));

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
} 