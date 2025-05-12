namespace APBD_test_grupaB.DTOs;

public class VisitResponseDto
{
    public DateTime Date { get; set; }
    public ClientDto Client { get; set; }
    public MechanicDto Mechanic { get; set; }
    public List<VisitServiceDto> VisitServices { get; set; }
}