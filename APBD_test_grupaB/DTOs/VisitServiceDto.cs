using System.Text.Json.Serialization;

namespace APBD_test_grupaB.DTOs;

public class VisitServiceDto
{
    [JsonPropertyName("serviceName")]
    public string Name { get; set; }
    [JsonPropertyName("serviceFee")]
    public decimal ServiceFee { get; set; }
}