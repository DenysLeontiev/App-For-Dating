using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope()) // to resolve scoped services
            {
                var services = scope.ServiceProvider; // gets all dependencies
                try
                {
                    var context = services.GetRequiredService<DataContext>(); // getting the specifiec(DataContext) dependency
                    await context.Database.MigrateAsync(); // insted of updating database manually every time we need
                    await Seed.SeedUsers(context);
                }
                catch (Exception  ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>(); // getting ILogger<> dependency
                    logger.LogError(ex, "An error occured during migration");
                }
            }

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