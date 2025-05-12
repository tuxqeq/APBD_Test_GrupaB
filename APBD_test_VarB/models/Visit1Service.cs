namespace APBD_test_grupaB.models;

public class Visit1Service
{
    public int visit_id { get; set; }
    public int service_id { get; set; }
    public decimal service_fee { get; set; }

    public Visit Visit { get; set; }
    public Service Service { get; set; }
}