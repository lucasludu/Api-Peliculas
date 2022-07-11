using System.ComponentModel.DataAnnotations;
using static ApiPelicula.Models.Pelicula;

namespace ApiPelicula.Models.Dtos
{
    public class PeliculaUpdateDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
          
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Duracion { get; set; }
        public TipoClasificacion Clasificacion { get; set; }

        public int categoriaId { get; set; }
    }
}
