using APBD_KOLOKWIUM.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_KOLOKWIUM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentsService _appointmentsService;

    public AppointmentsController(IAppointmentsService appointmentsService)
    {
        _appointmentsService = appointmentsService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointments(int id)
    {
        if (!await _appointmentsService.DoesAppointmentExist(id))
        {
            return NotFound();
        }

        var getResult = await _appointmentsService.GetAppoinment(id);
        return Ok(getResult);
    }

}