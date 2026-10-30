using EventosApi.Data;
using EventosApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// DEGM - 28/10/2026
// Este archivo atiende las peticiones para consultar, inscribir, modificar
// y dar de baja participantes, cuidando que haya lugares disponibles.
namespace EventosApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantesController : ControllerBase
    {
        private readonly AppDbContext _contexto;

        public ParticipantesController(AppDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<Participante>>> ObtenerTodos()
        {
            var participantes = await _contexto.Participantes.ToListAsync();
            return Ok(participantes);
        }

        [HttpGet("buscar/{id}")]
        public async Task<ActionResult<Participante>> ObtenerPorId(int id)
        {
            var participante = await _contexto.Participantes.FindAsync(id);
            if (participante == null)
                return NotFound($"No existe el participante con Id {id}.");

            return Ok(participante);
        }

        [HttpPost("inscribir")]
        public async Task<ActionResult<Participante>> Crear(Participante participante)
        {
            if (string.IsNullOrWhiteSpace(participante.CorreoElectronico))
                return BadRequest("No se permiten participantes sin correo electrónico.");

            var evento = await _contexto.Eventos.FindAsync(participante.EventoId);
            if (evento == null)
                return NotFound($"No existe el evento con Id {participante.EventoId}.");

            if (evento.CupoDisponible <= 0)
                return Conflict("El evento no tiene cupo disponible.");

            bool repetido = await _contexto.Participantes.AnyAsync(p =>
                p.EventoId == participante.EventoId &&
                p.CorreoElectronico == participante.CorreoElectronico);
            if (repetido)
                return Conflict("Ese correo ya está inscrito en este evento.");

            participante.Id = 0;
            evento.CupoDisponible--; 
            _contexto.Participantes.Add(participante);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerPorId), new { id = participante.Id }, participante);
        }

        [HttpPut("modificar/{id}")]
        public async Task<IActionResult> Actualizar(int id, Participante participante)
        {
            if (id != participante.Id)
                return BadRequest("El Id de la ruta no coincide con el Id del participante.");

            if (string.IsNullOrWhiteSpace(participante.CorreoElectronico))
                return BadRequest("No se permiten participantes sin correo electrónico.");

            var existente = await _contexto.Participantes.FindAsync(id);
            if (existente == null)
                return NotFound($"No existe el participante con Id {id}.");

            if (existente.EventoId != participante.EventoId)
            {
                var eventoNuevo = await _contexto.Eventos.FindAsync(participante.EventoId);
                if (eventoNuevo == null)
                    return NotFound($"No existe el evento con Id {participante.EventoId}.");

                if (eventoNuevo.CupoDisponible <= 0)
                    return Conflict("El evento destino no tiene cupo disponible.");

                var eventoAnterior = await _contexto.Eventos.FindAsync(existente.EventoId);
                if (eventoAnterior != null)
                    eventoAnterior.CupoDisponible++; 

                eventoNuevo.CupoDisponible--; 
                existente.EventoId = participante.EventoId;
            }

            existente.NombreCompleto = participante.NombreCompleto;
            existente.CorreoElectronico = participante.CorreoElectronico;

            await _contexto.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var participante = await _contexto.Participantes.FindAsync(id);
            if (participante == null)
                return NotFound($"No existe el participante con Id {id}.");

            var evento = await _contexto.Eventos.FindAsync(participante.EventoId);
            if (evento != null)
                evento.CupoDisponible++;

            _contexto.Participantes.Remove(participante);
            await _contexto.SaveChangesAsync();
            return NoContent();
        }
    }
}