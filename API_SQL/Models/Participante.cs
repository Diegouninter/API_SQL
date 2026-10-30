using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// DEGM - 28/10/2026
// Este archivo describe a una persona inscrita en un evento: su nombre,
// su correo y el evento al que pertenece.
namespace EventosApi.Models
{
    public class Participante
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(120, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 120 caracteres.")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [StringLength(150)]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un EventoId válido.")]
        public int EventoId { get; set; }

        // Cada participante pertenece a un solo evento (se oculta en el JSON)
        [JsonIgnore]
        public Evento? Evento { get; set; }
    }
}