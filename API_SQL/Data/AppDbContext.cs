using EventosApi.Models;
using Microsoft.EntityFrameworkCore;

// DEGM - 28/10/2026
// Este archivo conecta la API con la base de datos SQLite y define
// las tablas de eventos y participantes y su relación.
namespace EventosApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Participante> Participantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                     modelBuilder.Entity<Participante>()
                .HasOne(p => p.Evento)
                .WithMany(e => e.Participantes)
                .HasForeignKey(p => p.EventoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}