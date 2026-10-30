using EventosApi.Data;
using EventosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// DEGM - 28/10/2026
// Este archivo atiende las peticiones para consultar, crear, modificar
// y eliminar eventos.
namespace EventosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly AppDbContext _contexto;

        public EventosController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        // GET api/eventos  -> 200
        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Evento>>> ObtenerTodos()
        {
            var eventos = await _contexto.Eventos.ToListAsync();
            return Ok(eventos);
        }

        [HttpGet("buscar/{id}")]
        public async Task<ActionResult<Evento>> ObtenerPorId(int id)
        {
            var evento = await _contexto.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound($"No existe el evento con Id {id}.");

            return Ok(evento);
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<Evento>> Crear(Evento evento)
        {
            if (string.IsNullOrWhiteSpace(evento.Nombre))
                return BadRequest("No se permiten eventos sin nombre.");

            if (evento.CupoDisponible < 0)
                return BadRequest("No se permiten cupos negativos.");

            evento.Id = 0;
            _contexto.Eventos.Add(evento);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = evento.Id }, evento);
        }

        [HttpPut("modificar/{id}")]
        public async Task<IActionResult> Actualizar(int id, Evento evento)
        {
            if (id != evento.Id)
                return BadRequest("El Id de la ruta no coincide con el Id del evento.");

            if (string.IsNullOrWhiteSpace(evento.Nombre))
                return BadRequest("No se permiten eventos sin nombre.");

            if (evento.CupoDisponible < 0)
                return BadRequest("No se permiten cupos negativos.");

            var existente = await _contexto.Eventos.FindAsync(id);
            if (existente == null)
                return NotFound($"No existe el evento con Id {id}.");

            existente.Nombre = evento.Nombre;
            existente.Lugar = evento.Lugar;
            existente.Fecha = evento.Fecha;
            existente.CupoDisponible = evento.CupoDisponible;

            await _contexto.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var evento = await _contexto.Eventos.FindAsync(id);
            if (evento == null)
                return NotFound($"No existe el evento con Id {id}.");

            bool tieneParticipantes = await _contexto.Participantes.AnyAsync(p => p.EventoId == id);
            if (tieneParticipantes)
                return Conflict("No se puede eliminar un evento que tiene participantes inscritos.");

            _contexto.Eventos.Remove(evento);
            await _contexto.SaveChangesAsync();
            return NoContent();
        }
    }
}