using APBD_8_WEBAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_8_WEBAPI.Controllers;

[ApiController]
[Route("api/trips")]
public class TripController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripController(ITripService tripService)
    {
        _tripService = tripService;
    }
    //Ten endpoint zwraca wszystkie wycieczki czyli wartości wszystkich parametrów obiektu trip oraz dodatkowo dodaje
    //wszystkie państwa do wycieczek
    //http://localhost:5141/api/trips
    [HttpGet]
    public async Task<IActionResult> GetTripsAsync(CancellationToken cancellationToken)
    {
        var wycieczki = await _tripService.GetAllTripsAsync(cancellationToken);
        return Ok(wycieczki);
    }
}