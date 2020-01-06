namespace IRunes.Data
{
    using IRunes.Models;
    using Microsoft.EntityFrameworkCore;

    public class RunesDbContext : DbContext
    {
        public RunesDbContext(DbContextOptions options) : base(options)
        {
        }

        public RunesDbContext()
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DatabaseConfiguration.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(u => u.Id);
            });

            modelBuilder.Entity<Album>(album =>
            {
                album.HasKey(a => a.Id);
                album.HasMany(a => a.Tracks);
            });

            modelBuilder.Entity<Track>(track =>
            {
                track.HasKey(t => t.Id);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
