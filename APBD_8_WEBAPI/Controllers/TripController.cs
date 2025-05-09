using APBD_8_WEBAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace APBD_8_WEBAPI.Controllers;

[ApiController]
[Route("api/trips")]
public class TripController : ControllerBase
{
    //Ten endpoint zwraca wszystkie wycieczki czyli wartości wszystkich parametrów obiektu trip oraz dodatkowo dodaje
    //wszystkie państwa do wycieczek
    
    //http://localhost:5141/api/trips
    [HttpGet]
    public async Task<IActionResult> GetClientsAsync(CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=2019SBD;Integrated Security=True;Trust Server Certificate=True");
        
        await using var com = new SqlCommand();
        com.Connection = con;
        com.CommandText = @"SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,c.Name AS CountryName
            FROM Trip t
            LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip 
            LEFT JOIN Country c ON ct.IdCountry = c.IdCountry;";
        
        await con.OpenAsync(cancellationToken);
        
        SqlDataReader reader = await com.ExecuteReaderAsync(cancellationToken);
        
        var trips = new List<Trip>();
        while (await reader.ReadAsync(cancellationToken))
        {
            int IdTrip = (int)reader["IdTrip"];
            string Name = (string)reader["Name"];
            string Description = (string)reader["Description"];
            DateTime DateFrom = (DateTime)reader["DateFrom"];
            DateTime DateTo = (DateTime)reader["DateTo"];
            int MaxPeople = (int)reader["MaxPeople"];
            string CountryName = (string)reader["CountryName"];
            
            var Trip = new Trip();
            
            Trip.IdTrip = IdTrip;
            Trip.Name = Name;
            Trip.Description = Description;
            Trip.DateFrom = DateFrom;
            Trip.DateTo = DateTo;
            Trip.MaxPeople = MaxPeople;
            Trip.Countries.Add(CountryName);
            
            if (trips.Exists(t => t.IdTrip == reader.GetInt32(0)))
            {
                if (trips.First(t => t.IdTrip == reader.GetInt32(0)).Countries.Contains(CountryName))
                { }
                else
                {
                    trips.First(t => t.IdTrip == reader.GetInt32(0)).Countries.Add(CountryName);
                }
            }
            else
            {
                trips.Add(Trip);
            }
        }
        
        com.DisposeAsync();
        
        return Ok(trips);
    }
}