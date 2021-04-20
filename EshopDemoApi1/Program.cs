using CatalogDemo.API;
using CatalogDemo.API.Extensions;
using CatalogDemo.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EshopDemoApi1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.MigrateDbContext<CatalogContext>((context, services) => {
                var settings = services.GetService<IOptions<CatalogSettings>>();
                var env = services.GetService<IWebHostEnvironment>();
                new CatalogContextSeed().Seed(context, env, settings);
            });
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
