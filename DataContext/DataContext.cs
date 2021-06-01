using Microsoft.EntityFrameworkCore;
using Angeloid.Models;

namespace Angeloid.DataContext
{
    public class Context : DbContext
    {
        public Context(DbContextOptions options) : base(options) { }

        public DbSet<Anime> Animes { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Studio> Studios { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Seiyuu> Seiyuus { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AnimeTag> AnimeTags { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Thread> Threads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Season and Anime : many to one
            modelBuilder.Entity<Season>()
                        .HasMany(s => s.Animes)
                        .WithOne(a => a.Season)
                        .OnDelete(DeleteBehavior.SetNull);

            //Studio and Anime : many to one
            modelBuilder.Entity<Studio>()
                        .HasMany(s => s.Animes)
                        .WithOne(a => a.Studio)
                        .OnDelete(DeleteBehavior.SetNull);

            //Seiyuu and Character : many to one
            modelBuilder.Entity<Seiyuu>()
                        .HasMany(s => s.Characters)
                        .WithOne(c => c.Seiyuu)
                        .OnDelete(DeleteBehavior.SetNull);

            //Anime and Character : many to one
            modelBuilder.Entity<Anime>()
                        .HasMany(a => a.Characters)
                        .WithOne(c => c.Anime)
                        .OnDelete(DeleteBehavior.SetNull);

            //Tag and Anime: many to many
            modelBuilder.Entity<Anime>()
                        .HasMany(a => a.Tags)
                        .WithMany(t => t.Animes)
                        .UsingEntity<AnimeTag>(
                            j => j
                                .HasOne(at => at.Tag)
                                .WithMany(t => t.AnimeTags)
                                .HasForeignKey(at => at.TagId),
                            j => j
                                .HasOne(at => at.Anime)
                                .WithMany(a => a.AnimeTags)
                                .HasForeignKey(at => at.AnimeId),
                            j =>
                            {
                                j.HasKey(at => new { at.AnimeId, at.TagId });
                            }
                        );

            //User and Anime: many to many (Favorites)
            modelBuilder.Entity<Anime>()
                        .HasMany(a => a.UsersFavorite)
                        .WithMany(u => u.FavoriteAnimes)
                        .UsingEntity<Favorite>(
                            j => j
                                .HasOne(fa => fa.User)
                                .WithMany(u => u.Favorites)
                                .HasForeignKey(fa => fa.UserId),
                            j => j
                                .HasOne(fa => fa.Anime)
                                .WithMany(a => a.Favorites)
                                .HasForeignKey(fa => fa.AnimeId),
                            j =>
                            {
                                j.HasKey(fa => new { fa.AnimeId, fa.UserId });
                            }
                        );

            //User and Anime: many to many (Review)
            modelBuilder.Entity<Anime>()
                        .HasMany(a => a.UsersReview)
                        .WithMany(u => u.ReviewAnimes)
                        .UsingEntity<Review>(
                            j => j
                                .HasOne(r => r.User)
                                .WithMany(u => u.Reviews)
                                .HasForeignKey(r => r.UserId),
                            j => j
                                .HasOne(r => r.Anime)
                                .WithMany(a => a.Reviews)
                                .HasForeignKey(r => r.AnimeId),
                            j =>
                            {
                                j.HasKey(r => new { r.AnimeId, r.UserId });
                            }
                        );

            //User and Thread : many to one
            modelBuilder.Entity<User>()
                        .HasMany(u => u.Threads)
                        .WithOne(r => r.User)
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}