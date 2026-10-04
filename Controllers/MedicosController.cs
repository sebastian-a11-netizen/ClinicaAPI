using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ClinicaAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace ClinicaAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MedicosController : ControllerBase
{
    private readonly IMongoCollection<Medico> _medicos;

    public MedicosController(IMongoDatabase database)
    {
        _medicos = database.GetCollection<Medico>("medicos");
    }

    [HttpGet]
    public async Task<ActionResult<List<Medico>>> ObtenerMedicos()
    {
        var medicos = await _medicos
            .Find(_ => true)
            .ToListAsync();

        return Ok(medicos);
    }

    [HttpGet("{documento}")]
    public async Task<ActionResult<Medico>> ObtenerMedicoPorDocumento(string documento)
    {
        var medico = await _medicos
            .Find(m => m.Documento == documento)
            .FirstOrDefaultAsync();

        if (medico == null)
        {
            return NotFound($"No se encontró un médico con el documento {documento}");
        }

        return Ok(medico);
    }

    [HttpPost]
    public async Task<ActionResult<Medico>> CrearMedico(Medico medico)
    {
        await _medicos.InsertOneAsync(medico);

        return Ok(medico);
    }

    [HttpPut("{documento}")]
    public async Task<ActionResult<Medico>> ActualizarMedico(
        string documento,
        Medico medico)
    {
        var medicoExistente = await _medicos
            .Find(m => m.Documento == documento)
            .FirstOrDefaultAsync();

        if (medicoExistente == null)
        {
            return NotFound($"No se encontró un médico con el documento {documento}");
        }

        medico.Id = medicoExistente.Id;
        medico.Documento = documento;

        var resultado = await _medicos.ReplaceOneAsync(
            m => m.Documento == documento,
            medico);

        if (resultado.ModifiedCount == 0)
        {
            return BadRequest("No se pudo actualizar el médico");
        }

        return Ok(medico);
    }

    [HttpDelete("{documento}")]
    public async Task<ActionResult> DesactivarMedico(string documento)
    {
        var filtro = Builders<Medico>.Filter.Eq(m => m.Documento, documento);

        var actualizacion = Builders<Medico>.Update
            .Set(m => m.Activo, false);

        var resultado = await _medicos.UpdateOneAsync(
            filtro,
            actualizacion);

        if (resultado.MatchedCount == 0)
        {
            return NotFound(
                $"No se encontró un médico con el documento {documento}");
        }

        return Ok(
            $"El médico {documento} fue desactivado correctamente");
    }
}