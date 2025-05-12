namespace APBD_test_grupaB.models;

public class Visit
{
    public int visit_id { get; set; }
    public int client_id { get; set; }
    public int mechanic_id { get; set; }
    public DateTime date { get; set; }

    public Client Client { get; set; }
    public Mechanic Mechanic { get; set; }
    public ICollection<Visit1Service> VisitServices { get; set; }
}