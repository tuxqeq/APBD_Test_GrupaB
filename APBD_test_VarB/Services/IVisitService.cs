using APBD_test_grupaB.DTOs;
using APBD_test_grupaB.models;

namespace APBD_test_grupaB.Services;

public interface IVisitService
{
    public Task<VisitResponseDto> GetVisitByIdAsync(int id);
    Task<VisitCreateResult> CreateVisitAsync(VisitCreateDto dto, CancellationToken cancellationToken);
}