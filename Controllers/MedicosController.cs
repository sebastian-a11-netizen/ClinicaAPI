using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using ClinicaAPI.Models;

namespace ClinicaAPI.Controllers;

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

[HttpGet("{id}")]
public async Task<ActionResult<Medico>> ObtenerMedicoPorId(string id)
{
    if (!ObjectId.TryParse(id, out _))
        return BadRequest("El ID no tiene un formato válido.");

    var medico = await _medicos.Find(m => m.Id == id).FirstOrDefaultAsync();

    if (medico == null)
        return NotFound($"No se encontró un médico con el Id: {id}.");

    return Ok(medico);
}

[HttpPost]
public async Task<ActionResult<Medico>> CrearMedico(Medico medico)
{
    medico.Id = null;

    await _medicos.InsertOneAsync(medico);

    return Ok(medico);
}

[HttpPut("{id}")]
public async Task<ActionResult<Medico>> ActualizarMedico(
    string id,
    Medico medico)
{
    if (!ObjectId.TryParse(id, out _))
        return BadRequest("El ID no tiene un formato válido.");

    var medicoExistente = await _medicos
        .Find(m => m.Id == id)
        .FirstOrDefaultAsync();

    if (medicoExistente == null)
        return NotFound("No se encontró el médico.");

    medico.Id = id;

    var resultado = await _medicos.ReplaceOneAsync(
        m => m.Id == id,
        medico);

    if (resultado.MatchedCount == 0)
        return NotFound("No se encontró el médico.");

    return Ok(medico);
}


[HttpDelete("{id}")]
public async Task<ActionResult> EliminarMedico(string id)
{
    if (!ObjectId.TryParse(id, out _))
        return BadRequest("El ID no tiene un formato válido.");

    var resultado = await _medicos.DeleteOneAsync(
        m => m.Id == id
    );

    if (resultado.DeletedCount == 0)
        return NotFound("No se encontró el médico.");

    return Ok("El médico fue eliminado correctamente.");
}



}
