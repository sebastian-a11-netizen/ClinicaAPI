using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
        var pacientes = await _pacientes.Find(_ => true).ToListAsync(); // Obtener todos los documentos

        return Ok(pacientes);
    }

    [HttpPost]
    public async Task<ActionResult<Paciente>> CrearPaciente(Paciente paciente)
    {
        await _pacientes.InsertOneAsync(paciente);

        return Ok(paciente);
    }

    [HttpGet("{documento}")]
    public async Task<ActionResult<Paciente>> ObtenerPacientePorDocumento(string documento)
    {
        var paciente = await _pacientes
            .Find(p => p.Documento == documento)
            .FirstOrDefaultAsync();

        if (paciente == null)
        {
            return NotFound($"No se encontró un paciente con el documento {documento}");
        }

        return Ok(paciente);
    }

    [HttpPut("{documento}")]
    public async Task<ActionResult<Paciente>> ActualizarPaciente(
    string documento,
    Paciente paciente)
    {
        var pacienteExistente = await _pacientes
            .Find(p => p.Documento == documento)
            .FirstOrDefaultAsync();

        if (pacienteExistente == null)
        {
            return NotFound($"No se encontró un paciente con el documento {documento}");
        }

        paciente.Id = pacienteExistente.Id;
        paciente.Documento = documento;

        var resultado = await _pacientes.ReplaceOneAsync(
            p => p.Documento == documento,
            paciente);

        if (resultado.ModifiedCount == 0)
        {
            return BadRequest("No se pudo actualizar el paciente");
        }

        return Ok(paciente);
    }

    [HttpDelete("{documento}")]
    public async Task<ActionResult> DesactivarPaciente(string documento)
    {
        var filtro = Builders<Paciente>.Filter.Eq(p => p.Documento, documento);

        var actualizacion = Builders<Paciente>.Update
            .Set(p => p.Activo, false);

        var resultado = await _pacientes.UpdateOneAsync(filtro, actualizacion);

        if (resultado.MatchedCount == 0)
        {
            return NotFound($"No se encontró un paciente con el documento {documento}");
        }

        return Ok($"El paciente {documento} fue desactivado correctamente");
    }
}