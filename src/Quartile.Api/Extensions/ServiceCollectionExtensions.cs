using Quartile.Application;
using Quartile.Infrastructure;

namespace Quartile.Api.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructureServices(configuration);
            services.AddApplicationServices();
            return services;
        }
    }
}
