using APBD_KOLOKWIUM.Models;

namespace APBD_KOLOKWIUM.Services;

public interface IAppointmentsService
{
    Task<bool> DoesAppointmentExist(int id);
    Task<bool> DoesPatientExist(int appointmentPatientId);
    Task<bool> DoesDoctorExist(string appointmentPwz);
    Task<bool> DoesServiceExist(List<PostAppoinment_Services> appointmentAppointmentServices);

    Task<Appointment> GetAppoinment(int id);
        Task PutAppoinment(PostAppointment appointment);
}