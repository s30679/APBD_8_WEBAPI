using APBD_8_WEBAPI.Models;
using Microsoft.Data.SqlClient;

namespace APBD_8_WEBAPI.Repositories;

public class TripRepository : ITripRepository
{
    private readonly string _connectionString;

    public TripRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Trip>> GetAllTripsWithCountriesAsync(CancellationToken cancellationToken)
    {
        await using var con = new SqlConnection(_connectionString);
        await using var com = new SqlCommand(@"
            SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName
            FROM Trip t
            LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip 
            LEFT JOIN Country c ON ct.IdCountry = c.IdCountry;", con);
        await con.OpenAsync(cancellationToken);
        SqlDataReader odczyt=await com.ExecuteReaderAsync(cancellationToken);
        var wycieczki=new Dictionary<int, Trip>();
        while (await odczyt.ReadAsync(cancellationToken))
        {
            int idTrip=(int)odczyt["IdTrip"];
            if(!wycieczki.TryGetValue(idTrip, out var trip))
            {
                trip=new Trip
                {
                    IdTrip = idTrip,
                    Name = (string)odczyt["Name"],
                    Description = (string)odczyt["Description"],
                    DateFrom = (DateTime)odczyt["DateFrom"],
                    DateTo = (DateTime)odczyt["DateTo"],
                    MaxPeople = (int)odczyt["MaxPeople"],
                    Countries = new List<string>()
                };
                wycieczki.Add(idTrip, trip);
            }
            string countryName = odczyt["CountryName"] as string;
            if(!string.IsNullOrEmpty(countryName) && !trip.Countries.Contains(countryName))
            {
                trip.Countries.Add(countryName);
            }
        }
        return wycieczki.Values.ToList();
    }

    public async Task<bool> TripExistsAsync(int id, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("SELECT 1 FROM Trip WHERE IdTrip = @id;", con);
        com.Parameters.AddWithValue("@id", id);
        await con.OpenAsync(cancellationToken);
        return (await com.ExecuteScalarAsync(cancellationToken)) != null;
    }
}