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
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().ToContainer("Games");
            modelBuilder.Entity<Game>().HasKey(_ => _.Key);
            modelBuilder.Entity<Game>().HasPartitionKey(_ => _.Key);
            modelBuilder.Entity<Game>().OwnsMany(_ => _.Players).OwnsMany(b => b.PlayerCards);
            modelBuilder.Entity<Game>().OwnsMany(_ => _.CardsPlayed);
            modelBuilder.Entity<Game>().OwnsMany(_ => _.Deck);

            modelBuilder.Entity<Card>().ToContainer("Cards");
            modelBuilder.Entity<Card>().HasKey(_ => _.Key);
            modelBuilder.Entity<Card>().HasPartitionKey(_ => _.Type);


            base.OnModelCreating(modelBuilder);
        }
    }
}
