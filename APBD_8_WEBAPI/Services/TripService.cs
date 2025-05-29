using APBD_8_WEBAPI.Models;
using APBD_8_WEBAPI.Repositories;

namespace APBD_8_WEBAPI.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepository;

    public TripService(ITripRepository tripRepository)
    {
        _tripRepository=tripRepository;
    }

    public async Task<IEnumerable<Trip>> GetAllTripsAsync(CancellationToken cancellationToken)
    {
        return await _tripRepository.GetAllTripsWithCountriesAsync(cancellationToken);
    }
}