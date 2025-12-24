using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ReTube.Models;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;

namespace ReTube.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ReTube.Models.Video> Video { get; set; }
        public DbSet<ReTube.Models.FavoriteVideo> FavoriteVideo { get; set; }
        public DbSet<ReTube.Models.FavoriteVideoToVideo> FavoriteVideoToVideo { get; set; }
        public DbSet<ReTube.Models.Playlist> Playlist { get; set; }
        public DbSet<ReTube.Models.PlaylistVideo> PlaylistVideo { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER",
                }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Videos)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Playlists)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(e => e.FavoriteVideos)
                .WithOne(e => e.User)
                .HasForeignKey<FavoriteVideo>(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Video>()
                .HasMany(e => e.FavoriteVideos)
                .WithMany(e => e.Videos)
                .UsingEntity<FavoriteVideoToVideo>();

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Video>()
                .HasMany(e => e.Playlists)
                .WithMany(e => e.Videos)
                .UsingEntity<PlaylistVideo>();
        }
    }
}
