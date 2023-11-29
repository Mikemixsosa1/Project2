namespace Project2.Models
{
    public class Pais
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        // Relación con Estados
        public virtual ICollection<Estado> Estados { get; set; }
    }
}
