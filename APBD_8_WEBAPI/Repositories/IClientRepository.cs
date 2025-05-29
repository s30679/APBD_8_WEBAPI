using APBD_8_WEBAPI.Models;
using APBD_8_WEBAPI.DTOs;

namespace APBD_8_WEBAPI.Repositories;


public interface IClientRepository
{
    Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Clinet_TripDetails>> GetClientTripsAsync(int id, CancellationToken cancellationToken);
    Task<int> AddClientAsync(Client client, CancellationToken cancellationToken);
    Task<bool> IsClientRegisteredForTripAsync(int clientId, int tripId, CancellationToken cancellationToken);
    Task<int> GetTripMaxPeopleAsync(int tripId, CancellationToken cancellationToken);
    Task<int> GetCurrentTripParticipantsCountAsync(int tripId, CancellationToken cancellationToken);
    Task AddClientToTripAsync(int clientId, int tripId, int registeredAt, CancellationToken cancellationToken);
    Task<bool> ClientTripRegistrationExistsAsync(int clientId, int tripId, CancellationToken cancellationToken);
    Task DeleteClientFromTripAsync(int clientId, int tripId, CancellationToken cancellationToken);
    Task<bool> PeselExistsAsync(string pesel, CancellationToken cancellationToken);
}