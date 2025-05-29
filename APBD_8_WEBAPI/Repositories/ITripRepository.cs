using APBD_8_WEBAPI.Models;

namespace APBD_8_WEBAPI.Repositories;

public interface ITripRepository
{
    Task<IEnumerable<Trip>> GetAllTripsWithCountriesAsync(CancellationToken cancellationToken);
    Task<bool> TripExistsAsync(int id, CancellationToken cancellationToken);
}