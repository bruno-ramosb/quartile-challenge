using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Quartile.Application.Interfaces;
using Quartile.Application.Services;

namespace Quartile.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return services
                .AddValidatorsFromAssembly(assembly)
                .AddScoped<IStoreService, StoreService>();
        }
    }
}
