using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using ClinicaAPI.Models;

namespace ClinicaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IMongoCollection<Paciente> _pacientes;

    public PacientesController(IMongoDatabase database)
    {
        _pacientes = database.GetCollection<Paciente>("pacientes");
    }

    [HttpGet]
    public async Task<ActionResult<List<Paciente>>> ObtenerPacientes()
    {
        var pacientes = await _pacientes
            .Find(_ => true)
            .ToListAsync();

        return Ok(pacientes);
    }

    [HttpPost]
    public async Task<ActionResult<Paciente>> CrearPaciente(Paciente paciente)
    {
        await _pacientes.InsertOneAsync(paciente);

        return Ok(paciente);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Paciente>> ObtenerPacientePorId(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest("El ID proporcionado no es válido");
        }

        var paciente = await _pacientes
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();

        if (paciente == null)
        {
            return NotFound($"No se encontró un paciente con el ID {id}");
        }

        return Ok(paciente);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Paciente>> ActualizarPaciente(
        string id,
        Paciente paciente)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest("El ID proporcionado no es válido");
        }

        var pacienteExistente = await _pacientes
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync();

        if (pacienteExistente == null)
        {
            return NotFound($"No se encontró un paciente con el ID {id}");
        }

        paciente.Id = pacienteExistente.Id;

        var resultado = await _pacientes.ReplaceOneAsync(
            p => p.Id == id,
            paciente);

        if (resultado.MatchedCount == 0)
        {
            return NotFound($"No se encontró un paciente con el ID {id}");
        }

        return Ok(paciente);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DesactivarPaciente(string id)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return BadRequest("El ID proporcionado no es válido");
        }

        var filtro = Builders<Paciente>.Filter.Eq(p => p.Id, id);

        var actualizacion = Builders<Paciente>.Update
            .Set(p => p.Activo, false);

        var resultado = await _pacientes.UpdateOneAsync(
            filtro,
            actualizacion);

        if (resultado.MatchedCount == 0)
        {
            return NotFound($"No se encontró un paciente con el ID {id}");
        }

        return Ok($"El paciente {id} fue desactivado correctamente");
    }
}