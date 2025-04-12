using Microsoft.EntityFrameworkCore;
using Calendario.Models;

namespace Calendario.Models.Data
{
    public class CalendarioContext : DbContext
    {
        public CalendarioContext(DbContextOptions<CalendarioContext> options)
            : base(options)
        {
        }

        public DbSet<Global> DataGlobal { get; set; }
        public DbSet<Especifico> DataEspecifico { get; set; }
        public DbSet<RespAcademico> DataRespAcademico { get; set; }
        public DbSet<RespOperador> DataRespOperador { get; set; }
        public DbSet<Principal> DataPrincipal { get; set; }
        public DbSet<AmbienteA> DataAmbienteA { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for Principal
            modelBuilder.Entity<Principal>()
                .HasOne(p => p.Global)
                .WithMany()
                .HasForeignKey(p => p.GlobalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Principal>()
                .HasOne(p => p.Especifico)
                .WithMany()
                .HasForeignKey(p => p.EspecificoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Principal>()
                .HasOne(p => p.RespAcademico)
                .WithMany()
                .HasForeignKey(p => p.RespAcademicoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Principal>()
                .HasOne(p => p.RespOperador)
                .WithMany()
                .HasForeignKey(p => p.RespOperadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Principal>()
            .HasMany(p => p.AmbienteA)
            .WithOne(e => e.Principal)
            .HasForeignKey(e => e.PrincipalId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}