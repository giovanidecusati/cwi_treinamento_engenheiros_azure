using Meetup.WebApi.Infrastructure.Data.EntityConfigurations;
using Meetup.WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Meetup.WebApi.Infrastructure.Data
{
    public class MeetupDbcontext : DbContext
    {
        public MeetupDbcontext(DbContextOptions<MeetupDbcontext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }

        public DbSet<User> Users { get; set; }
    }
}
