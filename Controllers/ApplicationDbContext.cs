using Microsoft.EntityFrameworkCore;
using SitiosWeb.Model;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Colaboradores> Colaboradores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Colaboradores>()
            .ToTable("AsignacionPColaboradores")
            .HasKey(c => c.Identificacion);

        base.OnModelCreating(modelBuilder);
    }
}
