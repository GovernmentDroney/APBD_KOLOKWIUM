using APBD_KOLOKWIUM.Models;
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

    [HttpPost]
    public async Task<IActionResult> PostAppointment(PostAppointment appointment)
    {
        if (await _appointmentsService.DoesAppointmentExist(appointment.appointmentId))
        {
            return Conflict();
        }
        if (!await _appointmentsService.DoesPatientExist(appointment.patientId))
        {
            return NotFound();
        }
        if (!await _appointmentsService.DoesDoctorExist(appointment.pwz))
        {
            return NotFound();
        }
        if (!await _appointmentsService.DoesServiceExist(appointment.appointmentServices))
        {
            return NotFound();
        }
        try
        {
            await _appointmentsService.PutAppoinment(appointment);
            return Created();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    

}