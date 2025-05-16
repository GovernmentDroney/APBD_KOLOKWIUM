namespace APBD_KOLOKWIUM.Services;

public class Service : IService
{
    private readonly IConfiguration _configuration;

    public Service(IConfiguration configuration)
    {
        _configuration = configuration;
    }  
}