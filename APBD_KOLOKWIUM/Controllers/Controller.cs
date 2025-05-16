using APBD_KOLOKWIUM.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_KOLOKWIUM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Controller
{
    private readonly IService _Service;

    public Controller(IService Service)
    {
        _Service = Service;
    }
    
}