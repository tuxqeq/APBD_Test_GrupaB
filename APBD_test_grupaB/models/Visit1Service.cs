namespace APBD_test_grupaB.models;

public class VisitService
{
    public int VisitId { get; set; }
    public int ServiceId { get; set; }
    public decimal ServiceFee { get; set; }

    public Visit Visit { get; set; }
    public Service Service { get; set; }
}