using Microsoft.EntityFrameworkCore;
using MyMindset.Cache.WebAPI.Models;

namespace MyMindset.Cache.WebAPI.Infrastracture
{
    public class KeyAndValueContext : DbContext
    {
        public KeyAndValueContext(DbContextOptions<KeyAndValueContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KeyValueModel>()
                .HasKey(k => k.Key);
        }

        public DbSet<KeyValueModel>? KeyAndValues { get; set; }
    }
}
