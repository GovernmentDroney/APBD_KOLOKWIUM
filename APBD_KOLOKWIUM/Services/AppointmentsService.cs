using System.Data.Common;
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

    public async Task<bool> DoesPatientExist(int appointmentPatientId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();
        command.CommandText = "Select 1 from Patient where patient_id = @id";
        command.Parameters.AddWithValue("@id", appointmentPatientId);
        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<bool> DoesDoctorExist(string appointmentPwz)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();
        command.CommandText = "Select 1 from Doctor where PWZ = @id";
        command.Parameters.AddWithValue("@id", appointmentPwz);
        var result = await command.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<bool> DoesServiceExist(List<PostAppoinment_Services> appointmentAppointmentServices)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        await connection.OpenAsync();
        foreach (var AppSer in appointmentAppointmentServices)
        {
            command.CommandText = "Select 1 from Service where name = @id";
            command.Parameters.AddWithValue("@id", AppSer.serviceName);
            var result = await command.ExecuteScalarAsync();
            return result != null;
        }

        return false;
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

    public async Task PutAppoinment(PostAppointment appointment)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.CommandText = "Select doctor_id from doctor where PWZ = @pwz";
            command.Parameters.AddWithValue("@pwz", appointment.pwz);
            var docId = (int)await command.ExecuteScalarAsync();
            command.Parameters.Clear();
            command.CommandText = "Insert into Appointment VALUES (@appointment_id,@patient_id,@doctor_id,@date) ";
            command.Parameters.AddWithValue("@appointment_id",appointment.appointmentId);
            command.Parameters.AddWithValue("@patient_id",appointment.patientId);
            command.Parameters.AddWithValue("@doctor_id",docId);
            command.Parameters.AddWithValue("@date",DateTime.Now);
            await command.ExecuteNonQueryAsync();
            command.Parameters.Clear();
            foreach (var AppSer in appointment.appointmentServices)
            {
                command.Parameters.AddWithValue("@serviceName",AppSer.serviceName);
                command.CommandText = "Select service_id from Service where name = @serviceName";
                var ServiceId = (int)await command.ExecuteScalarAsync();
                command.Parameters.Clear();

                command.CommandText = "Insert Into Appoinment_Service VALUES (@appointment_id,@service_id,@service_fee)";
                command.Parameters.AddWithValue("@appointment_id", appointment.appointmentId);
                command.Parameters.AddWithValue("@service_id", ServiceId);
                command.Parameters.AddWithValue("@service_fee", AppSer.serviceFee);
                await command.ExecuteNonQueryAsync();

            }
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}