using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

public class EscuelaContext : DbContext
{
    public EscuelaContext(DbContextOptions<EscuelaContext> options) : base(options)
    {
    }

    public DbSet<Asunto> Asuntos { get; set; }
    public DbSet<NivelEducativo> NivelesEducativos { get; set; }
    public DbSet<Domicilio> Domicilios { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<Estudiante> Estudiantes { get; set; }
    public DbSet<Municipio> Municipios { get; set; }
    public DbSet<Pais> Paises { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de las claves principales
        modelBuilder.Entity<Asunto>().HasKey(a => a.IdAsunto);
        modelBuilder.Entity<NivelEducativo>().HasKey(ne => ne.IdNivelEducativo);
        modelBuilder.Entity<Domicilio>().HasKey(d => d.IdDomicilio);
        modelBuilder.Entity<Estado>().HasKey(e => e.IdEstado);
        modelBuilder.Entity<Estudiante>().HasKey(e => e.CURP);
        modelBuilder.Entity<Municipio>().HasKey(m => m.IdMunicipio);
        modelBuilder.Entity<Pais>().HasKey(p => p.IdPais);
        modelBuilder.Entity<Ticket>().HasKey(t => t.Folio);
        modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);

        // Configuración de las relaciones de claves foráneas
        modelBuilder.Entity<Domicilio>()
            .HasOne<Municipio>(d => d.Municipio)
            .WithMany()
            .HasForeignKey(d => d.IdMunicipio);
        modelBuilder.Entity<Domicilio>()
            .HasOne<Estado>(d => d.Estado)
            .WithMany()
            .HasForeignKey(d => d.IdEstado);
        modelBuilder.Entity<Domicilio>()
            .HasOne<Pais>(d => d.Pais)
            .WithMany()
            .HasForeignKey(d => d.IdPais);

        modelBuilder.Entity<Estado>()
            .HasOne<Pais>(e => e.Pais)
            .WithMany()
            .HasForeignKey(e => e.IdPais);

        modelBuilder.Entity<Estudiante>()
            .HasOne<Domicilio>(e => e.Domicilio)
            .WithMany()
            .HasForeignKey(e => e.IdDomicilio);
        modelBuilder.Entity<Estudiante>()
            .HasOne<NivelEducativo>(e => e.NivelEducativo)
            .WithMany()
            .HasForeignKey(e => e.IdNivelEducativo);

        modelBuilder.Entity<Municipio>()
            .HasOne<Estado>(m => m.Estado)
            .WithMany()
            .HasForeignKey(m => m.IdEstado);

        modelBuilder.Entity<Ticket>()
            .HasOne<Municipio>(t => t.Municipio)
            .WithMany()
            .HasForeignKey(t => t.IdMunicipio);
        modelBuilder.Entity<Ticket>()
            .HasOne<Estudiante>(t => t.Estudiante)
            .WithMany()
            .HasForeignKey(t => t.CURP);
        modelBuilder.Entity<Ticket>()
            .HasOne<Asunto>(t => t.Asunto)
            .WithMany()
            .HasForeignKey(t => t.IdAsunto);



    }
}


public enum TipoUsuario
{
    Normal, // Será convertido a "Normal" como cadena
    Administrador // Será convertido a "Administrador" como cadena
}

public class Asunto
{
    public int IdAsunto { get; set; }
    public string Descripcion { get; set; }
}

public class NivelEducativo
{
    public int IdNivelEducativo { get; set; }
    public string Descripcion { get; set; }
}

public class Domicilio
{
    public int IdDomicilio { get; set; }
    public string Calle { get; set; }
    public string Colonia { get; set; }
    public int IdMunicipio { get; set; }
    public int IdEstado { get; set; }
    public int IdPais { get; set; }

    // Propiedades de navegación
    public Municipio Municipio { get; set; }
    public Estado Estado { get; set; }
    public Pais Pais { get; set; }
}

public class Estado
{
    public int IdEstado { get; set; }
    public string NombreEstado { get; set; }
    public int IdPais { get; set; }

    // Propiedad de navegación
    public Pais Pais { get; set; }
}

public class Estudiante
{
    public string CURP { get; set; }
    public string Nombre { get; set; }
    public string Paterno { get; set; }
    public string Materno { get; set; }
    public int? IdNivelEducativo { get; set; }
    public int? IdDomicilio { get; set; }
    public string TelefonoEstudiante { get; set; }

    // Propiedades de navegación
    public NivelEducativo NivelEducativo { get; set; }
    public Domicilio Domicilio { get; set; }
}

public class Municipio
{
    public int IdMunicipio { get; set; }
    public string NombreMunicipio { get; set; }
    public int IdEstado { get; set; }

    // Propiedad de navegación
    public Estado Estado { get; set; }
}

public class Pais
{
    public int IdPais { get; set; }
    public string NombrePais { get; set; }
}

public class Ticket
{
    public int Folio { get; set; }
    public string CURP { get; set; }
    public int? IdAsunto { get; set; }
    public string Estatus { get; set; }
    public int? NumeroTurno { get; set; }
    public DateTime? FechaRegistroTramite { get; set; }
    public DateTime? FechaDeTramite { get; set; }
    public int? IdMunicipio { get; set; }

    // Propiedades de navegación
    public Municipio Municipio { get; set; }
    public Estudiante Estudiante { get; set; }
    public Asunto Asunto { get; set; }
}

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string TipoUsuario { get; set; }
    public string Email { get; set; }
    public string NombreCompleto { get; set; }
    public string TelefonoUsuario { get; set; }
}
