using APBD_KOLOKWIUM.Models;
using Microsoft.Data.SqlClient;

namespace APBD_KOLOKWIUM.Services;

public class AppointmentsService : IAppointmentsService
{
    private readonly IConfiguration _configuration;

    public AppointmentsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesAppointmentExist(int id)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();
        command.CommandText = "Select 1 from Appointment where appoitment_id = @id";
        command.Parameters.AddWithValue("@id", id);
        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<Appointment> GetAppoinment(int id)
    {
        var appointment = new Appointment();
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();
        command.CommandText = "SELECT date from Appointment where appoitment_id = @id";
        command.Parameters.AddWithValue("@id", id);
        using (var appointment_reader = await command.ExecuteReaderAsync())
        {
            while (await appointment_reader.ReadAsync())
            {
                appointment = new Appointment()
                {
                    date = appointment_reader.GetDateTime(appointment_reader.GetOrdinal("date")),
                    patient = new Patient(),
                    doctor = new Doctor(),
                    appointmentServices = new List<Appoinment_Services>()
                };
            }
        }

        command.Parameters.Clear();
        command.CommandText =
            "SELECT first_name,last_name,date_of_birth from Patient Join dbo.Appointment A on Patient.patient_id = A.patient_id where appoitment_id = @id";
        command.Parameters.AddWithValue("@id", id);
        using (var patient_reader = await command.ExecuteReaderAsync())
        {
            while (await patient_reader.ReadAsync())
            {
                appointment.patient = new Patient()
                {
                    firstName = patient_reader.GetString(patient_reader.GetOrdinal("first_name")),
                    lastName = patient_reader.GetString(patient_reader.GetOrdinal("last_name")),
                    dateOfBirth = patient_reader.GetDateTime(patient_reader.GetOrdinal("date_of_birth"))
                };
            }
        }

        command.Parameters.Clear();
        command.CommandText =
            "Select Doctor.doctor_id, PWZ from Doctor JOIN dbo.Appointment A on Doctor.doctor_id = A.doctor_id WHERE appoitment_id = @id";
        command.Parameters.AddWithValue("@id", id);
        using (var doctor_reader = await command.ExecuteReaderAsync())
        {
            while (await doctor_reader.ReadAsync())
            {
                appointment.doctor = new Doctor()
                {
                    doctorId = doctor_reader.GetInt32(doctor_reader.GetOrdinal("doctor_id")),
                    pwz = doctor_reader.GetString(doctor_reader.GetOrdinal("PWZ"))
                };
            }
        }

        command.Parameters.Clear();
        command.CommandText =
            "Select name,base_fee from Service Join dbo.Appointment_Service SA on Service.service_id = SA.service_id join dbo.Appointment A on A.appoitment_id = SA.appoitment_id where A.appoitment_id = @id";
        command.Parameters.AddWithValue("@id", id);
        var listOfServices = new List<Appoinment_Services>();
        using (var services_reader = await command.ExecuteReaderAsync())
        {
            while (await services_reader.ReadAsync())
            {
                var appointment_service = new Appoinment_Services()
                {
                    name = services_reader.GetString(services_reader.GetOrdinal("name")),
                    serviceFee = services_reader.GetDecimal(services_reader.GetOrdinal("base_fee"))
                };
                listOfServices.Add(appointment_service);
            }
        }
        appointment.appointmentServices = listOfServices;
        return appointment;
    }
}