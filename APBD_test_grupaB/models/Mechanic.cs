namespace APBD_test_grupaB.models;

public class Mechanic
{
    public int mechanic_id { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string licence_number { get; set; }

    public ICollection<Visit> Visits { get; set; }
}
