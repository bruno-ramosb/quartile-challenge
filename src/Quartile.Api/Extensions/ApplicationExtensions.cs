using Quartile.Api.Middlewares;

namespace Quartile.Api.Extensions
{
    internal static class ApplicationExtensions
    {
        internal static void ConfigureApplication(this IApplicationBuilder app)
        {
            app.ConfigureSwagger();
            app.ConfigureMiddlewares();
        }

        internal static void ConfigureMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        internal static void ConfigureSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }
    }
}
