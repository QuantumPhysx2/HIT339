using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tennis339.Models;

namespace Tennis339
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var _serviceProvider = services.GetRequiredService<IServiceProvider>();
                    var _configuration = services.GetRequiredService<IConfiguration>();

                    SeedData.CreateRoles(_serviceProvider, _configuration).Wait();
                }
                catch (Exception err)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(err, "Seeding data failed");
                };
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
