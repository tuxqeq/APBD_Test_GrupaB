using APBD_test_grupaB.DTOs;
using APBD_test_grupaB.models;
using APBD_test_grupaB.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_test_grupaB.Controllers;

using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitsController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVisit(int id)
    {
        var visit = await _visitService.GetVisitByIdAsync(id);

        if (visit == null)
        {
            return NotFound();
        }

        return Ok(visit);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVisit([FromBody] VisitCreateDto visitDto, CancellationToken cancellationToken)
    {
        var result = await _visitService.CreateVisitAsync(visitDto, cancellationToken);

        return result switch
        {
            VisitCreateResult.AlreadyExists => Conflict($"Visit with ID {visitDto.VisitId} already exists."),
            VisitCreateResult.ClientNotFound => NotFound($"Client with ID {visitDto.ClientId} not found."),
            VisitCreateResult.MechanicNotFound => NotFound($"Mechanic with license number '{visitDto.MechanicLicenceNumber}' not found."),
            VisitCreateResult.ServiceNotFound => BadRequest("One or more services do not exist."),
            VisitCreateResult.InvalidData => BadRequest("Invalid visit data."),
            VisitCreateResult.Success => CreatedAtAction(nameof(CreateVisit), new { id = visitDto.VisitId }, null),
            _ => StatusCode(500, "An unknown error occurred.")
        };
    }
}