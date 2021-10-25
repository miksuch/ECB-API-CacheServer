using Challenge.CacheServer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Challenge.CacheServer.Infrastructure.Database
{
    public class CacheServerDbContext : DbContext
    {
        public DbSet<ExchangeRate> ExchangeRate { get; set; }
        public DbSet<CacheRecord> CacheRecord { get; set; }
        public DbSet<CurrencyPair> CurrencyPair { get; set; }

        public CacheServerDbContext() : base()
        {
        }

        public CacheServerDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB; Initial Catalog = aveneo.Challenge.Database;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder
                .Entity<ExchangeRate>()
                .HasOne(rate => rate.CurrencyPair)
                .WithMany(pair => pair.ExchangeRates)
                .HasForeignKey(rate => rate.CurrencyPairId);

            modelBuilder
                .Entity<ExchangeRate>()
                .HasIndex(rate => new { rate.CurrencyPairId, rate.Date })
                .IsUnique();

            modelBuilder
                .Entity<CacheRecord>()
                .HasOne(record => record.CurrencyPair)
                .WithMany(pair => pair.CacheRecords)
                .HasForeignKey(record => record.CurrencyPairId);
        }
    }
}
