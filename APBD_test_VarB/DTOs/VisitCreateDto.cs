namespace APBD_test_grupaB.DTOs;

public class VisitCreateDto
{
    public int VisitId { get; set; }
    public int ClientId { get; set; }
    public string MechanicLicenceNumber { get; set; } = null!;
    public List<VisitServiceDto> Services { get; set; } = new();
}