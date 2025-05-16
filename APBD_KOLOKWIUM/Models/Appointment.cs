namespace APBD_KOLOKWIUM.Models;

public class Appointment
{
    public DateTime date { get; set; }
    public Patient patient { get; set; }
    public Doctor doctor { get; set; }
    public List<Appoinment_Services> appointmentServices { get; set; }
    
}

public class Patient
{
    public string firstName { get; set; }
    public string lastName { get; set; }
    public DateTime dateOfBirth { get; set; }
}

public class Doctor
{
    public int doctorId { get; set; }
    public string pwz { get; set; }
}

public class Appoinment_Services
{
    public string name { get; set; }
    public Decimal serviceFee { get; set; }
}