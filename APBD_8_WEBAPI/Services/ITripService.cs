using APBD_8_WEBAPI.Models;

namespace APBD_8_WEBAPI.Services;

public interface ITripService
{
    Task<IEnumerable<Trip>> GetAllTripsAsync(CancellationToken cancellationToken);
}