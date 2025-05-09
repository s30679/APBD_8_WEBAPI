using APBD_8_WEBAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace APBD_8_WEBAPI.Controllers;

[ApiController]
[Route("api/clients")]
public class Client_TripController : ControllerBase
{
    //Ten endpoint zwraca wszystkie wycieczki powiązane z konretnym klientem wskazanym przez użytkownika po id
    //Zwracane są dokładne szczegóły wycieczki, czyli wartości wszystkich pól klasy trip oraz 
    //zwracane są wartości pól klasy client_trip
    //W przypadku gdy klient nie istnieje lub nie ma przypisanych wycieczek są zwracane odpowiednie komunikaty HTTP
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetClientsTripsAsync(CancellationToken cancellationToken, int id)
    {
        await using var con =
            new SqlConnection(
                "Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True;MultipleActiveResultSets=True");
        await using var com = new SqlCommand();
        com.Connection = con;
        await con.OpenAsync(cancellationToken);
        
        using (var com2 = new SqlCommand("SELECT 1 FROM Client WHERE IdClient = @id;", con))
        {
            com2.Parameters.AddWithValue("@id", id);
            var result = await com2.ExecuteScalarAsync(cancellationToken);
            if (result == null)
            {
                return NotFound("Nie znaleziono klienta o takim id");
            }
        }
        com.CommandText = @"
        SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, ct.RegisteredAt, ct.PaymentDate
        FROM Client_Trip ct
        JOIN Trip t ON ct.IdTrip = t.IdTrip
        WHERE ct.IdClient = @id;";
        
        com.Parameters.AddWithValue("@id", id);
        SqlDataReader reader = await com.ExecuteReaderAsync(cancellationToken);
        
        var trips = new List<List<object>>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var pom = new List<object>();
            int IdTrip = (int)reader["IdTrip"];
            string Name = (string)reader["Name"];
            string Description = (string)reader["Description"];
            DateTime DateFrom = (DateTime)reader["DateFrom"];
            DateTime DateTo = (DateTime)reader["DateTo"];
            int MaxPeople = (int)reader["MaxPeople"];
            int RegisteredAt = (int)reader["RegisteredAt"];
            int PaymentDate = (int)reader["PaymentDate"];
            
            pom.Add("IdTrip: "+IdTrip);
            pom.Add("Name: "+Name);
            pom.Add("Description: "+Description);
            pom.Add("DateFrom: "+DateFrom);
            pom.Add("DateTo: "+DateTo);
            pom.Add("MaxPeople: "+MaxPeople);
            pom.Add("RegisteredAt: "+RegisteredAt);
            pom.Add("PaymentDate: "+PaymentDate);
            
            trips.Add(pom);
        }

        if (trips.Count == 0)
        {
            return NotFound("Nie znaleziono wycieczek dla tego klienta");
        }
        
        com.DisposeAsync();
        reader.Close();
        return Ok(trips);
    }
    //Ten endpoint tworzy rekord klienta przy podanych przez użytkownika danych w tabeli Client, dane muszą wszystkie zostać podane bez psutych pól
    //oraz bez wartości null, dane muszą być poprawne
    [HttpPost]
    public async Task<IActionResult> AddClient(Client client, CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True");
        await using var com = new SqlCommand();
        com.Connection = con;

        if (string.IsNullOrWhiteSpace(client.FirstName) ||
            string.IsNullOrWhiteSpace(client.LastName) ||
            string.IsNullOrWhiteSpace(client.Email) ||
            string.IsNullOrWhiteSpace(client.Telephone) ||
            string.IsNullOrWhiteSpace(client.Pesel)) {
            return BadRequest("Wszystkie pola muszą być wypełnione danymi poza Id");
        }
        
        com.CommandText = @"
        INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
        VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
        SELECT SCOPE_IDENTITY();";
        
        com.Parameters.AddWithValue("@FirstName", client.FirstName);
        com.Parameters.AddWithValue("@LastName", client.LastName);
        com.Parameters.AddWithValue("@Email", client.Email);
        com.Parameters.AddWithValue("@Telephone", client.Telephone);
        com.Parameters.AddWithValue("@Pesel", client.Pesel);

        await con.OpenAsync(cancellationToken);
        var pom = await com.ExecuteScalarAsync(cancellationToken);
        if (pom != DBNull.Value)
        {
            client.IdClient = Convert.ToInt32(pom);    
        }
        else
        {
            return BadRequest("Nie poprawne dodawanie klienta");
        }
        
        return Created($"/api/clients/{client.IdClient}", client);
    }
}