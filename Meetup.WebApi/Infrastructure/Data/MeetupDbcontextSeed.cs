using Meetup.WebApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Meetup.WebApi.Infrastructure.Data
{
    public class MeetupDbcontextSeed
    {
        public async Task SeedAsync(MeetupDbcontext context, IHostingEnvironment env, ILogger<MeetupDbcontext> logger)
        {
            using (context)
            {
                await context.Database.MigrateAsync();

                if (!await context.Users.AnyAsync())
                {
                    await context.Users.AddAsync(new User("root", "root", "123456"));
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
