#nullable disable

using Microsoft.EntityFrameworkCore;
using Film_Catalogue.Domain.Entity;

namespace Film_Catalogue.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Film> Films { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<FilmCategory> FilmCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Film>(builder =>
            {
                builder.ToTable("Films").HasKey(x => x.Id);

                builder.Property(x => x.Name).HasColumnType("varchar").HasMaxLength(200);

                builder.Property(x => x.Director).HasColumnType("varchar").HasMaxLength(200);

                builder.Property(x => x.Release).HasColumnType("datetime");
            });

            modelBuilder.Entity<Category>(builder =>
            {
                builder.ToTable("Categories").HasKey(x => x.Id);

                builder.Property(x => x.Name).HasColumnType("varchar").HasMaxLength(200);
            });

            modelBuilder.Entity<FilmCategory>(builder =>
            {
                builder.ToTable("Film_Categories").HasKey(x => x.Id);

                builder.HasOne(x => x.Film).WithMany(x => x.FilmCategories).HasForeignKey(x => x.FilmId);

                builder.HasOne(x => x.Category).WithMany(x => x.FilmCategories).HasForeignKey(x => x.CategoryId);
            });
        }
    }
}