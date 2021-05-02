using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace Webuno.API.Models
{
    public sealed class WebunoDbContext : DbContext
    {
        public WebunoDbContext(DbContextOptions<WebunoDbContext> options) : base(options)
        {

        }
        public WebunoDbContext()
        {
        
        }
        public DbSet<Game> Games { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Game>().ToContainer("Games");
            modelBuilder.Entity<Game>().HasKey(_ => _.Key);
            modelBuilder.Entity<Game>().HasPartitionKey(_ => _.Key);
            modelBuilder.Entity<Game>().OwnsMany(_ => _.Players);
            modelBuilder.Entity<Game>().OwnsMany(_ => _.CardsPlayed);


            base.OnModelCreating(modelBuilder);
        }
    }
}
