using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SitiosWeb.Model;

public partial class Tiusr22plProyectoContext : DbContext
{
    public Tiusr22plProyectoContext()
    {
    }

    public Tiusr22plProyectoContext(DbContextOptions<Tiusr22plProyectoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AsignacionPcolaboradores> AsignacionPcolaboradores { get; set; }

    public virtual DbSet<BloqueoDias> BloqueoDias { get; set; }

    public virtual DbSet<Colaboradores> Colaboradores { get; set; }

    public virtual DbSet<Configuraciones> Configuraciones { get; set; }

    public virtual DbSet<Departamentos> Departamentos { get; set; }

    public virtual DbSet<FechasReposicion> FechasReposicion { get; set; }

    public virtual DbSet<HorariosXPuesto> HorariosXPuesto { get; set; }

    public virtual DbSet<HorasExtra> HorasExtra { get; set; }

    public virtual DbSet<Inconsistencias> Inconsistencias { get; set; }

    public virtual DbSet<JustificacionesInconsistencias> JustificacionesInconsistencias { get; set; }

    public virtual DbSet<Marcas> Marcas { get; set; }

    public virtual DbSet<Permisos> Permisos { get; set; }

    public virtual DbSet<Puestos> Puestos { get; set; }

    public virtual DbSet<Rebajos> Rebajos { get; set; }

    public virtual DbSet<RegistroActividades> RegistroActividades { get; set; }

    public virtual DbSet<ReporteMarcas> ReporteMarcas { get; set; }

    public virtual DbSet<ReporteVacaciones> ReporteVacaciones { get; set; }

    public virtual DbSet<ReposicionTercero> ReposicionTercero { get; set; }

    public virtual DbSet<Reposiciones> Reposiciones { get; set; }

    public virtual DbSet<SolicitudHorasExtra> SolicitudHorasExtra { get; set; }

    public virtual DbSet<SolicitudPermiso> SolicitudPermiso { get; set; }

    public virtual DbSet<SolicitudVacaciones> SolicitudVacaciones { get; set; }

    public virtual DbSet<SolicitudeRebajo> SolicitudeRebajo { get; set; }

    public virtual DbSet<TipoActividades> TipoActividades { get; set; }

    public virtual DbSet<TipoUsuario> TipoUsuario { get; set; }

    public virtual DbSet<TiposInconsistencias> TiposInconsistencias { get; set; }

    public virtual DbSet<TiposPermisos> TiposPermisos { get; set; }

    public virtual DbSet<TiposRebajos> TiposRebajos { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<Vacaciones> Vacaciones { get; set; }

    public virtual DbSet<VacacionesColectivas> VacacionesColectivas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=Host");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Grupo03");

        modelBuilder.Entity<AsignacionPcolaboradores>(entity =>
        {
            entity.HasKey(e => e.Identificacion);

            entity.ToTable("AsignacionPColaboradores");

            entity.Property(e => e.Identificacion)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Apellido)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Departamento)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Puesto)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<BloqueoDias>(entity =>
        {
            entity.HasKey(e => e.DayId);

            entity.HasIndex(e => e.Day, "IX_BloqueoDias").IsUnique();

            entity.Property(e => e.DayId).HasColumnName("DayID");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Colaboradores>(entity =>
        {
            entity.HasKey(e => e.Identificacion).HasName("PK__colabora__C196DEC638065473");

            entity.ToTable("colaboradores");

            entity.Property(e => e.Identificacion)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("identificacion");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.FechaContratacion).HasColumnName("fechaContratacion");
            entity.Property(e => e.FechaFinContrato).HasColumnName("fechaFinContrato");
            entity.Property(e => e.FechaNaciento).HasColumnName("fechaNaciento");
            entity.Property(e => e.IdPuesto)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("id_puesto");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono).HasColumnName("telefono");

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.Colaboradores)
                .HasForeignKey(d => d.IdPuesto)
                .HasConstraintName("FK__colaborad__id_pu__3F466844");
        });

        modelBuilder.Entity<Configuraciones>(entity =>
        {
            entity.HasKey(e => e.IdConfiguraciones).HasName("PK__configur__D16D0A942C96E6A6");

            entity.ToTable("configuraciones");

            entity.Property(e => e.IdConfiguraciones).HasColumnName("id_configuraciones");
            entity.Property(e => e.NomConfiguracion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nom_configuracion");
            entity.Property(e => e.NumConfig).HasColumnName("num_config");
            entity.Property(e => e.ValorConfig)
                .HasColumnType("text")
                .HasColumnName("valor_config");
        });

        modelBuilder.Entity<Departamentos>(entity =>
        {
            entity.HasKey(e => e.IdDepartamento).HasName("PK__departam__64F37A1654E919DA");

            entity.ToTable("departamentos");

            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.NomDepartamento)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nom_departamento");
        });

        modelBuilder.Entity<FechasReposicion>(entity =>
        {
            entity.HasKey(e => new { e.IdReposicion, e.DiasReposicion, e.HorasReposicion });

            entity.ToTable("fechas_reposicion");

            entity.Property(e => e.IdReposicion).HasColumnName("idReposicion");
            entity.Property(e => e.DiasReposicion).HasColumnName("diasReposicion");
            entity.Property(e => e.HorasReposicion).HasColumnName("horasReposicion");

            entity.HasOne(d => d.IdReposicionNavigation).WithMany(p => p.FechasReposicion)
                .HasForeignKey(d => d.IdReposicion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_fechas_reposicion_Reposiciones");
        });

        modelBuilder.Entity<HorariosXPuesto>(entity =>
        {
            entity.HasKey(e => e.Idhorario).HasName("PK__Horarios__9EC73E1599F09D91");

            entity.ToTable("Horarios_x_puesto");

            entity.Property(e => e.Idhorario).HasColumnName("idhorario");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdPuesto)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("id_puesto");
            entity.Property(e => e.Jueves)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Lunes)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Martes)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Miercoles)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sabado)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Viernes)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.HorariosXPuesto)
                .HasForeignKey(d => d.IdPuesto)
                .HasConstraintName("FK__Horarios___id_pu__47DBAE45");
        });

        modelBuilder.Entity<HorasExtra>(entity =>
        {
            entity.HasKey(e => e.IdHoras).HasName("PK__horas_ex__ECAD337C7FEB5B0D");

            entity.ToTable("horas_extra");

            entity.Property(e => e.IdHoras).HasColumnName("id_horas");
            entity.Property(e => e.CantidadHoras)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("cantidad_horas");
            entity.Property(e => e.CantidadPago)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cantidad_pago");
            entity.Property(e => e.EstadoDelPago).HasColumnName("estado_del_pago");
            entity.Property(e => e.FechaRealizacion).HasColumnName("fecha_realizacion");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.IdSolicitante)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_solicitante");
            entity.Property(e => e.IdTipoActividad).HasColumnName("idTipoActividad");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.HorasExtraIdEmpleadoNavigation)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__horas_ext__id_em__70DDC3D8");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.HorasExtraIdSolicitanteNavigation)
                .HasForeignKey(d => d.IdSolicitante)
                .HasConstraintName("FK__horas_ext__id_so__72C60C4A");

            entity.HasOne(d => d.IdTipoActividadNavigation).WithMany(p => p.HorasExtra)
                .HasForeignKey(d => d.IdTipoActividad)
                .HasConstraintName("FK__horas_ext__idTip__71D1E811");
        });

        modelBuilder.Entity<Inconsistencias>(entity =>
        {
            entity.HasKey(e => e.IdInconsistencia).HasName("PK__inconsis__FD32FC0E1ED1AC66");

            entity.ToTable("inconsistencias");

            entity.Property(e => e.IdInconsistencia).HasColumnName("id_inconsistencia");
            entity.Property(e => e.FechaInconsistencia).HasColumnName("fecha_inconsistencia");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.IdJustificacion).HasColumnName("id_Justificacion");
            entity.Property(e => e.IdTipoInconsistencia).HasColumnName("id_tipoInconsistencia");
            entity.Property(e => e.Mostrar).HasColumnName("mostrar");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Inconsistencias)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__inconsist__id_em__5165187F");

            entity.HasOne(d => d.IdJustificacionNavigation).WithMany(p => p.Inconsistencias)
                .HasForeignKey(d => d.IdJustificacion)
                .HasConstraintName("FK__inconsist__id_Ju__534D60F1");

            entity.HasOne(d => d.IdTipoInconsistenciaNavigation).WithMany(p => p.Inconsistencias)
                .HasForeignKey(d => d.IdTipoInconsistencia)
                .HasConstraintName("FK__inconsist__id_ti__52593CB8");
        });

        modelBuilder.Entity<JustificacionesInconsistencias>(entity =>
        {
            entity.HasKey(e => e.IdJustificacion).HasName("PK__justific__5E1594065508465E");

            entity.ToTable("justificaciones_inconsistencias");

            entity.Property(e => e.IdJustificacion).HasColumnName("id_Justificacion");
            entity.Property(e => e.Evidencias).HasColumnName("evidencias");
            entity.Property(e => e.HorarioId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("HorarioID");
            entity.Property(e => e.IdColaborador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_colaborador");
            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.IdPuesto)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("id_puesto");
            entity.Property(e => e.IdTipoInconsistencia).HasColumnName("idTipoInconsistencia");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(350)
                .IsUnicode(false);
            entity.Property(e => e.Reposicion).HasColumnName("reposicion");
            entity.Property(e => e.Validacion).HasColumnName("validacion");

            entity.HasOne(d => d.IdColaboradorNavigation).WithMany(p => p.JustificacionesInconsistencias)
                .HasForeignKey(d => d.IdColaborador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_justificaciones_inconsistencias_colaboradores");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.JustificacionesInconsistencias)
                .HasForeignKey(d => d.IdDepartamento)
                .HasConstraintName("FK_justificaciones_inconsistencias_departamentos");

            entity.HasOne(d => d.IdPuestoNavigation).WithMany(p => p.JustificacionesInconsistencias)
                .HasForeignKey(d => d.IdPuesto)
                .HasConstraintName("FK_justificaciones_inconsistencias_puestos");

            entity.HasOne(d => d.IdTipoInconsistenciaNavigation).WithMany(p => p.JustificacionesInconsistencias)
                .HasForeignKey(d => d.IdTipoInconsistencia)
                .HasConstraintName("FK_justificaciones_inconsistencias_tipos_inconsistencias");

            entity.HasOne(d => d.ReposicionNavigation).WithMany(p => p.JustificacionesInconsistencias)
                .HasForeignKey(d => d.Reposicion)
                .HasConstraintName("FK_justificaciones_inconsistencias_Reposiciones");
        });

        modelBuilder.Entity<Marcas>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("PK__marcas__7E43E99EE68AE58D");

            entity.ToTable("marcas");

            entity.Property(e => e.IdMarca).HasColumnName("id_marca");
            entity.Property(e => e.FinJornada)
                .HasColumnType("datetime")
                .HasColumnName("finJornada");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.InicioJornada)
                .HasColumnType("datetime")
                .HasColumnName("inicioJornada");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Marcas)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__marcas__id_emple__656C112C");
        });

        modelBuilder.Entity<Permisos>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PK__permisos__ED14A36F77BFBC23");

            entity.ToTable("permisos");

            entity.Property(e => e.IdPermiso).HasColumnName("id_Permiso");
            entity.Property(e => e.Aprobado).HasColumnName("aprobado");
            entity.Property(e => e.DOH).HasColumnName("d_o_h");
            entity.Property(e => e.DiasHorasFuera).HasColumnName("dias_horas_fuera");
            entity.Property(e => e.FechaFin)
                .HasColumnType("datetime")
                .HasColumnName("fechaFin");
            entity.Property(e => e.FechaInicio)
                .HasColumnType("datetime")
                .HasColumnName("fechaInicio");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.IdTipoPermiso).HasColumnName("id_tipoPermiso");
            entity.Property(e => e.IdValidador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_validador");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.PermisosIdEmpleadoNavigation)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__permisos__id_emp__7C4F7684");

            entity.HasOne(d => d.IdTipoPermisoNavigation).WithMany(p => p.Permisos)
                .HasForeignKey(d => d.IdTipoPermiso)
                .HasConstraintName("FK__permisos__id_tip__7D439ABD");

            entity.HasOne(d => d.IdValidadorNavigation).WithMany(p => p.PermisosIdValidadorNavigation)
                .HasForeignKey(d => d.IdValidador)
                .HasConstraintName("FK__permisos__id_val__7F2BE32F");
        });

        modelBuilder.Entity<Puestos>(entity =>
        {
            entity.HasKey(e => e.IdPuesto).HasName("pk_puestos");

            entity.ToTable("puestos");

            entity.Property(e => e.IdPuesto)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("id_puesto");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.NombrePuesto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_puesto");
            entity.Property(e => e.Salario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("salario");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.Puestos)
                .HasForeignKey(d => d.IdDepartamento)
                .HasConstraintName("FK__puestos__id_depa__3B75D760");
        });

        modelBuilder.Entity<Rebajos>(entity =>
        {
            entity.HasKey(e => e.IdRebajo).HasName("PK__rebajos__094418102E528F44");

            entity.ToTable("rebajos");

            entity.Property(e => e.IdRebajo).HasColumnName("id_rebajo");
            entity.Property(e => e.Aprobacion).HasColumnName("aprobacion");
            entity.Property(e => e.FechaRebajo).HasColumnName("fechaRebajo");
            entity.Property(e => e.IdColaborador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_colaborador");
            entity.Property(e => e.IdTipoRebajo).HasColumnName("idTipoRebajo");
            entity.Property(e => e.IdValidador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_validador");
            entity.Property(e => e.Inconsistencia).HasColumnName("inconsistencia");

            entity.HasOne(d => d.IdColaboradorNavigation).WithMany(p => p.RebajosIdColaboradorNavigation)
                .HasForeignKey(d => d.IdColaborador)
                .HasConstraintName("FK__rebajos__id_cola__5FB337D6");

            entity.HasOne(d => d.IdTipoRebajoNavigation).WithMany(p => p.Rebajos)
                .HasForeignKey(d => d.IdTipoRebajo)
                .HasConstraintName("FK__rebajos__idTipoR__628FA481");

            entity.HasOne(d => d.IdValidadorNavigation).WithMany(p => p.RebajosIdValidadorNavigation)
                .HasForeignKey(d => d.IdValidador)
                .HasConstraintName("FK__rebajos__id_vali__60A75C0F");

            entity.HasOne(d => d.InconsistenciaNavigation).WithMany(p => p.Rebajos)
                .HasForeignKey(d => d.Inconsistencia)
                .HasConstraintName("FK__rebajos__inconsi__619B8048");
        });

        modelBuilder.Entity<RegistroActividades>(entity =>
        {
            entity.HasKey(e => e.IdRegistro).HasName("PK__registro__C0A94D77D6D345EF");

            entity.ToTable("registro_actividades");

            entity.Property(e => e.IdRegistro).HasColumnName("id_Registro");
            entity.Property(e => e.Aprobado).HasColumnName("aprobado");
            entity.Property(e => e.DuracionEnHoras)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("duracion_en_horas");
            entity.Property(e => e.IdColaborador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_colaborador");
            entity.Property(e => e.IdTipoActividad).HasColumnName("idTipoActividad");
            entity.Property(e => e.IdValidador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_validador");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.IdColaboradorNavigation).WithMany(p => p.RegistroActividadesIdColaboradorNavigation)
                .HasForeignKey(d => d.IdColaborador)
                .HasConstraintName("FK__registro___id_co__59063A47");

            entity.HasOne(d => d.IdTipoActividadNavigation).WithMany(p => p.RegistroActividades)
                .HasForeignKey(d => d.IdTipoActividad)
                .HasConstraintName("FK__registro___idTip__5812160E");

            entity.HasOne(d => d.IdValidadorNavigation).WithMany(p => p.RegistroActividadesIdValidadorNavigation)
                .HasForeignKey(d => d.IdValidador)
                .HasConstraintName("FK__registro___id_va__59FA5E80");
        });

        modelBuilder.Entity<ReporteMarcas>(entity =>
        {
            entity.HasKey(e => e.IdReporte).HasName("PK__reporte___40D65D3CDA24EB46");

            entity.ToTable("reporte_marcas");

            entity.Property(e => e.IdReporte).HasColumnName("idReporte");
            entity.Property(e => e.FechaRegistro).HasColumnName("fechaRegistro");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.IdMarca).HasColumnName("id_marca");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.ReporteMarcas)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__reporte_m__id_em__68487DD7");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.ReporteMarcas)
                .HasForeignKey(d => d.IdMarca)
                .HasConstraintName("FK__reporte_m__id_ma__693CA210");
        });

        modelBuilder.Entity<ReporteVacaciones>(entity =>
        {
            entity.HasKey(e => e.IdReporte).HasName("PK__reporte___87E4F5CBDE4D26A1");

            entity.ToTable("reporte_vacaciones");

            entity.Property(e => e.IdReporte).HasColumnName("id_reporte");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.FechaAprobacion).HasColumnName("fechaAprobacion");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.IdValidador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_validador");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(350)
                .IsUnicode(false)
                .HasColumnName("observaciones");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.ReporteVacacionesIdEmpleadoNavigation)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__reporte_v__id_em__07C12930");

            entity.HasOne(d => d.IdValidadorNavigation).WithMany(p => p.ReporteVacacionesIdValidadorNavigation)
                .HasForeignKey(d => d.IdValidador)
                .HasConstraintName("FK__reporte_v__id_va__08B54D69");
        });

        modelBuilder.Entity<ReposicionTercero>(entity =>
        {
            entity.HasKey(e => new { e.Idtercero, e.Idsolicitante, e.Justificacion });

            entity.Property(e => e.Idtercero)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("IDtercero");
            entity.Property(e => e.Idsolicitante)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("IDSolicitante");

            entity.HasOne(d => d.IdsolicitanteNavigation).WithMany(p => p.ReposicionTerceroIdsolicitanteNavigation)
                .HasForeignKey(d => d.Idsolicitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReposicionTercero_colaboradores1");

            entity.HasOne(d => d.IdterceroNavigation).WithMany(p => p.ReposicionTerceroIdterceroNavigation)
                .HasForeignKey(d => d.Idtercero)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReposicionTercero_colaboradores");

            entity.HasOne(d => d.JustificacionNavigation).WithMany(p => p.ReposicionTercero)
                .HasForeignKey(d => d.Justificacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReposicionTercero_justificaciones_inconsistencias");
        });

        modelBuilder.Entity<Reposiciones>(entity =>
        {
            entity.HasKey(e => e.IdReposicion);

            entity.Property(e => e.IdReposicion).HasColumnName("idReposicion");
            entity.Property(e => e.AprobadasPor)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Idcolaborador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("IDColaborador");

            entity.HasOne(d => d.AprobadasPorNavigation).WithMany(p => p.ReposicionesAprobadasPorNavigation)
                .HasForeignKey(d => d.AprobadasPor)
                .HasConstraintName("FK_Reposiciones_colaboradores1");

            entity.HasOne(d => d.IdcolaboradorNavigation).WithMany(p => p.ReposicionesIdcolaboradorNavigation)
                .HasForeignKey(d => d.Idcolaborador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reposiciones_colaboradores");
        });

        modelBuilder.Entity<SolicitudHorasExtra>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__solicitu__5C0C31F3B760F025");

            entity.ToTable("solicitud_horas_extra");

            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.AprobadaPor).HasMaxLength(450);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaSolicitud).HasColumnName("fecha_solicitud");
            entity.Property(e => e.Horas)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("horas");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.IdSolicitante)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_solicitante");
            entity.Property(e => e.IdTipoActividad).HasColumnName("idTipoActividad");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.SolicitudHorasExtraIdEmpleadoNavigation)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__solicitud__id_em__6D0D32F4");

            entity.HasOne(d => d.IdSolicitanteNavigation).WithMany(p => p.SolicitudHorasExtraIdSolicitanteNavigation)
                .HasForeignKey(d => d.IdSolicitante)
                .HasConstraintName("FK__solicitud__id_so__6C190EBB");

            entity.HasOne(d => d.IdTipoActividadNavigation).WithMany(p => p.SolicitudHorasExtra)
                .HasForeignKey(d => d.IdTipoActividad)
                .HasConstraintName("FK__solicitud__idTip__6E01572D");
        });

        modelBuilder.Entity<SolicitudPermiso>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__solicitu__5C0C31F33821F86F");

            entity.ToTable("solicitud_permiso");

            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.Comentarios)
                .HasMaxLength(250)
                .HasColumnName("comentarios");
            entity.Property(e => e.DOH).HasColumnName("d_o_h");
            entity.Property(e => e.DiasHorasFuera).HasColumnName("dias_horas_fuera");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.IdTipoPermiso).HasColumnName("id_tipoPermiso");
            entity.Property(e => e.PuestoLaboral)
                .HasMaxLength(100)
                .HasColumnName("puestoLaboral");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.SolicitudPermiso)
                .HasForeignKey(d => d.IdEmpleado)
                .HasConstraintName("FK__solicitud__id_em__778AC167");

            entity.HasOne(d => d.IdTipoPermisoNavigation).WithMany(p => p.SolicitudPermiso)
                .HasForeignKey(d => d.IdTipoPermiso)
                .HasConstraintName("FK__solicitud__id_ti__797309D9");
        });

        modelBuilder.Entity<SolicitudVacaciones>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__solicitu__5C0C31F33D7958E7");

            entity.ToTable("solicitud_vacaciones");

            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.Aprobadas).HasColumnName("aprobadas");
            entity.Property(e => e.AprobadasPor)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("aprobadas_por");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.IdEmpleado)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_empleado");
            entity.Property(e => e.TotalDias).HasColumnName("total_dias");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.SolicitudVacaciones)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__solicitud__id_em__04E4BC85");
        });

        modelBuilder.Entity<SolicitudeRebajo>(entity =>
        {
            entity.HasKey(e => e.IdSolicitud).HasName("PK__solicitu__D801DDB88E38041D");

            entity.ToTable("solicitude_rebajo");

            entity.Property(e => e.IdSolicitud).HasColumnName("idSolicitud");
            entity.Property(e => e.IdInconsistencia).HasColumnName("idInconsistencia");
            entity.Property(e => e.IdSolicitante)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("idSolicitante");
            entity.Property(e => e.Mostrar).HasColumnName("mostrar");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoActividades>(entity =>
        {
            entity.HasKey(e => e.IdTipoActividad).HasName("PK__tipo_act__A3477EC5CAB61501");

            entity.ToTable("tipo_actividades");

            entity.Property(e => e.IdTipoActividad).HasColumnName("idTipoActividad");
            entity.Property(e => e.NomActividad)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nom_Actividad");
            entity.Property(e => e.Nombre).HasMaxLength(255);
        });

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.HasKey(e => e.IdTipo).HasName("PK__tipo_usu__CF901089BC77D8E9");

            entity.ToTable("tipo_usuario");

            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.NomTipo)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("nom_tipo");
        });

        modelBuilder.Entity<TiposInconsistencias>(entity =>
        {
            entity.HasKey(e => e.IdTipoInconsistencia).HasName("PK__tipos_in__F909D7022F09C3EF");

            entity.ToTable("tipos_inconsistencias");

            entity.Property(e => e.IdTipoInconsistencia).HasColumnName("id_tipoInconsistencia");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasColumnName("estado");
        });

        modelBuilder.Entity<TiposPermisos>(entity =>
        {
            entity.HasKey(e => e.IdTipoPermiso).HasName("PK__tipos_pe__AA886AABAEB53886");

            entity.ToTable("tipos_permisos");

            entity.Property(e => e.IdTipoPermiso).HasColumnName("id_tipoPermiso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado).HasColumnName("estado");
        });

        modelBuilder.Entity<TiposRebajos>(entity =>
        {
            entity.HasKey(e => e.IdTipoRebajo).HasName("PK__tipos_re__43B87CF246AF95CA");

            entity.ToTable("tipos_rebajos");

            entity.Property(e => e.IdTipoRebajo).HasColumnName("id_tipo_rebajo");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasColumnName("estado");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.CodUsuario).HasName("PK__usuarios__EA3C9B1A03BEA64E");

            entity.ToTable("usuarios");

            entity.Property(e => e.CodUsuario)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("cod_usuario");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("contrasena");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.IdColaborador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("idColaborador");
            entity.Property(e => e.IdTipoUsuario).HasColumnName("idTipoUsuario");

            entity.HasOne(d => d.IdColaboradorNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdColaborador)
                .HasConstraintName("FK__usuarios__idCola__4316F928");

            entity.HasOne(d => d.IdTipoUsuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdTipoUsuario)
                .HasConstraintName("FK__usuarios__idTipo__4222D4EF");
        });

        modelBuilder.Entity<Vacaciones>(entity =>
        {
            entity.HasKey(e => new { e.IdSolicitud, e.Fecha }).HasName("PK__vacacion__3F50DB1ECA79E337");

            entity.ToTable("vacaciones");

            entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
            entity.Property(e => e.Fecha).HasColumnName("fecha");

            entity.HasOne(d => d.IdSolicitudNavigation).WithMany(p => p.Vacaciones)
                .HasForeignKey(d => d.IdSolicitud)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_vacaciones_solicitud_vacaciones");
        });

        modelBuilder.Entity<VacacionesColectivas>(entity =>
        {
            entity.HasKey(e => e.IdVacaciones).HasName("PK__vacacion__0B46BB6B53674148");

            entity.ToTable("vacaciones_colectivas");

            entity.Property(e => e.IdVacaciones).HasColumnName("id_vacaciones");
            entity.Property(e => e.Aprobado).HasColumnName("aprobado");
            entity.Property(e => e.IdDepartamento).HasColumnName("id_departamento");
            entity.Property(e => e.IdSolicitador)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("id_solicitador");

            entity.HasOne(d => d.IdDepartamentoNavigation).WithMany(p => p.VacacionesColectivas)
                .HasForeignKey(d => d.IdDepartamento)
                .HasConstraintName("FK__vacacione__id_de__0C85DE4D");

            entity.HasOne(d => d.IdSolicitadorNavigation).WithMany(p => p.VacacionesColectivas)
                .HasForeignKey(d => d.IdSolicitador)
                .HasConstraintName("FK__vacacione__id_so__0B91BA14");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
