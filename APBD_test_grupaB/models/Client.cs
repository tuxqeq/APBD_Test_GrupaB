namespace APBD_test_grupaB.models;

public class Client
{
    public int client_id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public DateTime date_of_birth { get; set; }

    public ICollection<Visit> Visits { get; set; }
}