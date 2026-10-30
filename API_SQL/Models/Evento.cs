using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// DEGM - 28/10/2026
// Este archivo describe un evento de la empresa: su nombre, dónde y cuándo
// se realiza y cuántos lugares quedan disponibles.
namespace EventosApi.Models
{
    public class Evento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del evento es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El lugar es obligatorio.")]
        [StringLength(150, ErrorMessage = "El lugar no puede exceder 150 caracteres.")]
        public string Lugar { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime Fecha { get; set; }

        [Range(0, 100000, ErrorMessage = "El cupo disponible debe estar entre 0 y 100000.")]
        public int CupoDisponible { get; set; }

        // Un evento tiene muchos participantes (se oculta en el JSON para evitar ciclos)
        [JsonIgnore]
        public List<Participante> Participantes { get; set; } = new List<Participante>();
    }
}