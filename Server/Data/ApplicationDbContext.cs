using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Визначення взаємозв'язку між Advertisement і Photo
            modelBuilder.Entity<Advertisement>()
                .HasMany(a => a.Photos)
                .WithOne(b => b.Advertisement)
                .HasForeignKey(b => b.AdvertisementId);
        }
    }
}
