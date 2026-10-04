using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClinicaAPI.Models;

public class Paciente
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Documento { get; set; } = "";
    public string FechaNacimiento { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Direccion { get; set; } = "";
    public bool Activo { get; set; }
}