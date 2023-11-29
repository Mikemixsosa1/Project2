namespace Project2.Models
{
    public class _Ticket
    {
        public int Folio { get; set; }
        public string CURP { get; set; }
        public int IdAsunto { get; set; }
        public string Estatus { get; set; }
        public int NumeroTurno { get; set; }
        public DateTime FechaRegistroTramite { get; set; }
        public DateTime FechaDeTramite { get; set; }
        public int IdMunicipio { get; set; }
    }
}
