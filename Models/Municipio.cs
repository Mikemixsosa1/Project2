namespace Project2.Models
{
    public class Municipio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int EstadoId { get; set; }
        // Relación con Estado
        public virtual Estado Estado { get; set; }
    }
}
