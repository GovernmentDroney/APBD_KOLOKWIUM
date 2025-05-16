using APBD_KOLOKWIUM.Models;

namespace APBD_KOLOKWIUM.Services;

public interface IAppointmentsService
{
    Task<bool> DoesAppointmentExist(int id);
    Task<Appointment> GetAppoinment(int id);
}