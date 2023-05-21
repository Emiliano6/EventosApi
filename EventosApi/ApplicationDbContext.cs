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

    }
}