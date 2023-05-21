using Microsoft.EntityFrameworkCore;
using EventosApi.Data;
namespace EventosApi
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Organizador> Organizadores { get; set; }

        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
              modelBuilder.Entity<Evento>()
                .HasMany(e => e.Registrados)
                .WithMany(u => u.Historial)
                .UsingEntity(j => j.ToTable("EventoUsuarioRegistrado"));

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Favoritos)
                .WithMany(e => e.UsuarioFavoritos)
                .UsingEntity(j => j.ToTable("UsuarioEventoFavorito"));
        }
    }
}