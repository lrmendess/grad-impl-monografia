using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SCAP.Configurations
{
    public static class SchedulerConfig
    {
        public static IServiceCollection AddSchedulerConfig(this IServiceCollection service, IConfiguration configuration)
        {
            service.AddHangfire(config =>
            {
                config.UseStorage(new MySqlStorage(
                    configuration.GetConnectionString("DefaultConnection"),
                    new MySqlStorageOptions
                    {
                        TablesPrefix = "Hangfire_"
                    }
                ));
            });

            return service;
        }

        public static IApplicationBuilder UseScheduler(this IApplicationBuilder app)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard("/scheduler", new DashboardOptions
            {
                Authorization = new [] { new SchedulerAuthorizationFilter() }
            });
            
            return app;
        }
    }

    public class SchedulerAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.User.Identity.IsAuthenticated &&
                   httpContext.User.IsInRole("Secretario");
        }
    }
}
