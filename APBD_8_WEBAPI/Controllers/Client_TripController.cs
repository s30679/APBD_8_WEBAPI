using APBD_8_WEBAPI.Models;
using APBD_8_WEBAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_8_WEBAPI.Controllers;

[ApiController]
[Route("api/clients")]
public class Client_TripController : ControllerBase
{
    private readonly IClientService _clientService;

    public Client_TripController(IClientService clientService)
    {
        _clientService=clientService;
    }
    //Ten endpoint zwraca wszystkie wycieczki powiązane z konretnym klientem wskazanym przez użytkownika po id
    //Zwracane są dokładne szczegóły wycieczki, czyli wartości wszystkich pól klasy trip oraz 
    //zwracane są wartości pól klasy client_trip
    //W przypadku gdy klient nie istnieje lub nie ma przypisanych wycieczek są zwracane odpowiednie komunikaty HTTP
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientsTripsAsync(CancellationToken cancellationToken, int id)
    {
        var(trips, odp)=await _clientService.GetClientsTripsAsync(id, cancellationToken);
        if(trips==null)
        {
            return NotFound(odp);
        }
        return Ok(trips);
    }

    //Ten endpoint tworzy rekord klienta przy podanych przez użytkownika danych w tabeli Client, dane muszą wszystkie zostać podane bez psutych pól
    //oraz bez wartości null, dane muszą być poprawne
    [HttpPost]
    public async Task<IActionResult> AddClient(Client client, CancellationToken cancellationToken)
    {
        var(newClient, odp)=await _clientService.AddClientAsync(client, cancellationToken);
        if(newClient==null)
        {
            return BadRequest(odp);
        }
        return Created($"/api/clients/{newClient.IdClient}", newClient);
    }
    
    //Ten endpoint przypisuje klienta do wskazanej wycieczki, sprawdzając na początku czy taki klient i taka wycieczka istnieją w bazie
    //bieże również pod uwagę liczbę uczestników biorących udział w wycieczce i jeśli liczba przekracza dopuszczalną to nie zostanie dodany
    //kolejny klient, po odpowiednim sprawdzeniu endpoint dodaje klienta do wycieczki.
    [HttpPut("{IdClient}/trips/{IdTrip}")]
    public async Task<IActionResult> AddClientToTripAsync(int IdClient, int IdTrip, CancellationToken cancellationToken)
    {
        var odp=await _clientService.AddClientToTripAsync(IdClient, IdTrip, cancellationToken);
        if(odp.Contains("Nie ma w bazie"))
        {
            return NotFound(odp);
        }
        if(odp.Contains("już na liście") || odp.Contains("Nie dodam"))
        {
            return BadRequest(odp);
        }
        
        return Ok(odp);
    }

    //Ten endpoint na podstawie danych podanych przez użytkownika usuwa rezerwację wycieczki przez klienta uprzednio sprawdzając czy taka istnieje
    [HttpDelete("{IdClient}/trips/{IdTrip}")]
    public async Task<IActionResult> DeleteClientFromTripAsync(int IdClient, int IdTrip, CancellationToken cancellationToken)
    {
        var odp=await _clientService.DeleteClientFromTripAsync(IdClient, IdTrip, cancellationToken);
        if(odp.Contains("Nie ma w bazie"))
        {
            return NotFound(odp);
        }
        return Ok(odp);
    }
}