using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClinicaAPI.Models;

public class Medico
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Documento { get; set; } = "";
    public string RegistroMedico { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Correo { get; set; } = "";
    public string Especialidad { get; set; } = "";
    public string Consultorio { get; set; } = "";
    public bool Activo { get; set; }
}