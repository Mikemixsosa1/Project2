namespace Project2.Models
{
    public class _Estudiante
    {
        public string CURP { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public int? IdNivelEducativo { get; set; }
        public string TelefonoEstudiante { get; set; }
        public _Domicilio Domicilio { get; set; }
    }

}
