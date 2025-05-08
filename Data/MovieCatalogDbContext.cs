using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
namespace Data
{
    public class MovieCatalogDbContext : DbContext
    {

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Role> Roles { get; set; }

        public MovieCatalogDbContext(DbContextOptions<MovieCatalogDbContext> options)
     : base(options)
        { }

        public MovieCatalogDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //DESKTOP-U26QKOV - koray
                //Karina - KARINA\SQLEXPRESS 
                optionsBuilder.UseSqlServer(@"Server = KARINA\SQLEXPRESS; Database = MovieCatalog; Integrated security = true; TrustServerCertificate = true;");
            }
        }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Composite keys for Role and Review
            modelBuilder.Entity<Role>()
                .HasKey(r => new { r.MovieId, r.ActorId });

            modelBuilder.Entity<Review>()
                .HasKey(r => new { r.Id, r.MovieId });

            // ✅ Define Movie-Genre relationship (one-to-many)
            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Genre)
                .WithMany(g => g.Movies)
                .HasForeignKey(m => m.GenreId);

            // ✅ Define Movie-Actor relationship through Role (many-to-many via intermediate table)
            modelBuilder.Entity<Role>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Roles)
                .HasForeignKey(r => r.MovieId);

            modelBuilder.Entity<Role>()
                .HasOne(r => r.Actor)
                .WithMany(a => a.Roles)
                .HasForeignKey(r => r.ActorId);

            // ✅ Define Movie-Reviews relationship (one-to-many)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId);
        }*/
    }
}
    


