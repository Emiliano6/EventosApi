using Microsoft.EntityFrameworkCore;
using EventosApi.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EventosApi
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });

            modelBuilder.Entity<Evento>()
              .HasMany(e => e.Registrados)
              .WithMany(u => u.Historial)
              .UsingEntity(j => j.ToTable("EventoUsuarioRegistrado"));

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Favoritos)
                .WithMany(e => e.UsuarioFavoritos)
                .UsingEntity(j => j.ToTable("UsuarioEventoFavorito"));
        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Organizador> Organizadores { get; set; }

        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
    }
}