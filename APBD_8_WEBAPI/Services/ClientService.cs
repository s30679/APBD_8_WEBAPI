using APBD_8_WEBAPI.DTOs;
using APBD_8_WEBAPI.Models;
using APBD_8_WEBAPI.Repositories;

namespace APBD_8_WEBAPI.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly ITripRepository _tripRepository;

    public ClientService(IClientRepository clientRepository, ITripRepository tripRepository)
    {
        _clientRepository=clientRepository;
        _tripRepository=tripRepository;
    }

    public async Task<(IEnumerable<Clinet_TripDetails> trips, string message)> GetClientsTripsAsync(int id, CancellationToken cancellationToken)
    {
        if(!await _clientRepository.ClientExistsAsync(id, cancellationToken))
        {
            return (null, "Nie znaleziono klienta o takim id");
        }

        var wycieczki=await _clientRepository.GetClientTripsAsync(id, cancellationToken);
        if(!wycieczki.Any())
        {
            return (null, "Nie znaleziono wycieczek dla tego klienta");
        }

        return (wycieczki, null);
    }

    public async Task<(Client client, string message)> AddClientAsync(Client client, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(client.FirstName) || string.IsNullOrWhiteSpace(client.LastName) || string.IsNullOrWhiteSpace(client.Email) || string.IsNullOrWhiteSpace(client.Telephone) || string.IsNullOrWhiteSpace(client.Pesel))
        {
            return (null, "Wszystkie pola muszą być wypełnione danymi poza Id");
        }
        
        if(await _clientRepository.PeselExistsAsync(client.Pesel, cancellationToken))
        {
            return (null, "Klient z podanym numerem PESEL już istnieje.");
        }

        int nowyKlientID=await _clientRepository.AddClientAsync(client, cancellationToken);
        client.IdClient=nowyKlientID;
        
        return (client, null);
    }

    public async Task<string> AddClientToTripAsync(int IdClient, int IdTrip, CancellationToken cancellationToken)
    {
        if(!await _clientRepository.ClientExistsAsync(IdClient, cancellationToken))
        {
            return "Nie ma w bazie klienta o takim id";
        }
        else if(!await _tripRepository.TripExistsAsync(IdTrip, cancellationToken))
        {
            return "Nie ma w bazie wycieczki o takim id";
        }
        else if(await _clientRepository.IsClientRegisteredForTripAsync(IdClient, IdTrip, cancellationToken))
        {
            return "Klient jest już na liście wycieczkowiczów";
        }
        int maxOsoby = await _clientRepository.GetTripMaxPeopleAsync(IdTrip, cancellationToken);
        int obecniUczestnicy = await _clientRepository.GetCurrentTripParticipantsCountAsync(IdTrip, cancellationToken);
        if(obecniUczestnicy>=maxOsoby)
        {
            return "Nie dodam klienta, bo nie ma wolnych miejsc na wycieczce";
        }
        int zarejestrowany=int.Parse(DateTime.Today.ToString("yyyyMMdd"));
        await _clientRepository.AddClientToTripAsync(IdClient, IdTrip, zarejestrowany, cancellationToken);
        return "Pomyślnie dodano klienta do wycieczki";
    }

    public async Task<string> DeleteClientFromTripAsync(int IdClient, int IdTrip, CancellationToken cancellationToken)
    {
        if(!await _clientRepository.ClientTripRegistrationExistsAsync(IdClient, IdTrip, cancellationToken))
        {
            return "Nie ma w bazie takiej rejestracji";
        }
        
        await _clientRepository.DeleteClientFromTripAsync(IdClient, IdTrip, cancellationToken);
        return "Klient został usunięty z wycieczki";
    }
}