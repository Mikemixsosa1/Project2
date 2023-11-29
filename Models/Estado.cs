namespace Project2.Models
{
    public class Estado
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int PaisId { get; set; }
        // Relación con Pais
        public virtual Pais Pais { get; set; }
        // Relación con Municipios
        public virtual ICollection<Municipio> Municipios { get; set; }
    }
}
