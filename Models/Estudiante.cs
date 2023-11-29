using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project2.Models
{
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
}
