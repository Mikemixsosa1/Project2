namespace Project2.Models
{
    public class Ticket2
    {
        public int Folio { get; set; }
        public string CURP { get; set; }
        public int IdAsunto { get; set; }
        public string Estatus { get; set; }
        public int NumeroTurno { get; set; }
        public DateTime FechaRegistroTramite { get; set; }
        public DateTime? FechaDeTramite { get; set; } // Nullable porque se asignará después
        public int IdMunicipio { get; set; }
    }
}
