using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SCAP.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using SCAP.Configurations;
using Microsoft.AspNetCore.Identity;
using SCAP.Models;
using SCAP.Data.Seeds;
using KissLog.AspNetCore;
using Rotativa.AspNetCore;
using System.Threading.Tasks;

namespace SCAP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>    
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentityConfiguration(Configuration)
                .AddAutoMapper(typeof(Startup))
                .AddMvcConfiguration()
                .ResolveDependencies()
                .ResolveEmailSender(Configuration)
                .AddSchedulerConfig(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<Pessoa> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();

                UserSeeder.SeedDev(userManager);
            }
            else
            {
                app.UseExceptionHandler("/error/500");
                app.UseStatusCodePagesWithReExecute("/error/{0}");
                app.UseHsts();

                UserSeeder.SeedStagingOrProduction(userManager);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseGlobalizationConfig("pt-BR");

            if (!env.IsDevelopment())
            {
                app.UseKissLogMiddleware(options => options
                    .ConfigureKissLog()
                    .RegisterKissLogListeners(Configuration));
            }

            app.UseScheduler();

            RotativaConfiguration.Setup(env.WebRootPath, "rotativa");
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Afastamentos}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
