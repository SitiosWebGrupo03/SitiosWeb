using Microsoft.EntityFrameworkCore;
using SitiosWeb.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Colaborador> Colaboradores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Colaborador>()
            .ToTable("AsignacionPColaboradores")
            .HasKey(c => c.Identificacion);

        base.OnModelCreating(modelBuilder);
    }
}
