using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarbonFooter.Api.Models;

public class RegistroEsg
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Departamento { get; set; } = string.Empty;

    public double? ConsumoKwh { get; set; }

    public double? EmissoesCo2Kg { get; set; }

    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

    public RegistroEsg() { }

    public RegistroEsg(string departamento, double? consumoKwh, double? emissoesCo2Kg)
    {
        Departamento = departamento;
        ConsumoKwh = consumoKwh;
        EmissoesCo2Kg = emissoesCo2Kg;
        DataRegistro = DateTime.UtcNow;
    }
}