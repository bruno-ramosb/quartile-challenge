using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Quartile.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return services
                .AddValidatorsFromAssembly(assembly)
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        }
    }
}
