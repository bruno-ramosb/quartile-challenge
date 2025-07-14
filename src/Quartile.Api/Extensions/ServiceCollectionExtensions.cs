using Microsoft.EntityFrameworkCore;
using Quartile.Application;
using Quartile.Domain.Interfaces.Repositories;
using Quartile.Infrastructure.Context;
using Quartile.Infrastructure.Repositories;

namespace Quartile.Api.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection ConfigureServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddRepositories();
            services.AddInfrastructure();
            services.AddCommandHandlers();
            return services;
        }

        internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<QuartileContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("QuartileConnection"));
            });

            return services;
        }

        internal static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<ICompanyRepository, CompanyRepository>();
        }

        internal static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            return services
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }

    }
}
