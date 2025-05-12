using System.Text.Json.Serialization;

namespace APBD_test_grupaB.models;

public class Service
{
    public int service_id { get; set; }
    public string name { get; set; }
    public decimal base_fee { get; set; }

    public ICollection<Visit1Service> VisitServices { get; set; }
}