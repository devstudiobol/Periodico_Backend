using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PeriodicoUpdate.Models
{
    public class Publicacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string? ImagenUrl { get; set; }
        public string? ImagenPublicId { get; set; }
        public int Visualizacion { get; set; }
        public string Estado { get; set; }

        [ForeignKey("Usuario")]
        public int idusuario { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }

        [ForeignKey("Categoria")]
        public int idcategoria { get; set; }
        [JsonIgnore]
        public Categoria Categoria { get; set; }
    }
}
