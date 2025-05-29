using APBD_8_WEBAPI.DTOs;
using APBD_8_WEBAPI.Models;

namespace APBD_8_WEBAPI.Services;

public interface IClientService
{
    Task<(IEnumerable<Clinet_TripDetails> trips, string message)> GetClientsTripsAsync(int id, CancellationToken cancellationToken);
    Task<(Client client, string message)> AddClientAsync(Client client, CancellationToken cancellationToken);
    Task<string> AddClientToTripAsync(int IdClient, int IdTrip, CancellationToken cancellationToken);
    Task<string> DeleteClientFromTripAsync(int IdClient, int IdTrip, CancellationToken cancellationToken);
}