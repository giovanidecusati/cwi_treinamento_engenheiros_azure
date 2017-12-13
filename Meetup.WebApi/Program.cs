using Meetup.WebApi.Infrastructure.Data;
using Meetup.WebApi.Infrastructure.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Meetup.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args)
                .MigrateDbContext<MeetupDbcontext>((context, services) =>
                {
                    var env = services.GetService<IHostingEnvironment>();
                    var logger = services.GetService<ILogger<MeetupDbcontext>>();
                    new MeetupDbcontextSeed()
                       .SeedAsync(context, env, logger)
                       .Wait();
                })
                .Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
    }
}
