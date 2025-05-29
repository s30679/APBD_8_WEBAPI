using APBD_8_WEBAPI.DTOs;
using APBD_8_WEBAPI.Models;
using Microsoft.Data.SqlClient;

namespace APBD_8_WEBAPI.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly string _connectionString;

    public ClientRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> ClientExistsAsync(int id, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("SELECT 1 FROM Client WHERE IdClient = @id;", con);
        com.Parameters.AddWithValue("@id", id);
        await con.OpenAsync(cancellationToken);
        return (await com.ExecuteScalarAsync(cancellationToken))!=null;
    }

    public async Task<IEnumerable<Clinet_TripDetails>> GetClientTripsAsync(int id, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand(@"
            SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, ct.RegisteredAt, ct.PaymentDate
            FROM Client_Trip ct
            JOIN Trip t ON ct.IdTrip = t.IdTrip
            WHERE ct.IdClient = @id;", con);
        com.Parameters.AddWithValue("@id", id);
        await con.OpenAsync(cancellationToken);
        SqlDataReader odczyt=await com.ExecuteReaderAsync(cancellationToken);
        var wycieczki=new List<Clinet_TripDetails>();
        while(await odczyt.ReadAsync(cancellationToken))
        {
            wycieczki.Add(new Clinet_TripDetails
            {
                IdTrip = (int)odczyt["IdTrip"],
                Name = (string)odczyt["Name"],
                Description = (string)odczyt["Description"],
                DateFrom = (DateTime)odczyt["DateFrom"],
                DateTo = (DateTime)odczyt["DateTo"],
                MaxPeople = (int)odczyt["MaxPeople"],
                RegisteredAt = (int)odczyt["RegisteredAt"],
                PaymentDate = odczyt["PaymentDate"] as int?
            });
        }
        return wycieczki;
    }

    public async Task<int> AddClientAsync(Client client, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand(@"
            INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
            VALUES (@FirstName, @LastName, @Email, @Telephone, @Pesel);
            SELECT SCOPE_IDENTITY();", con);
        com.Parameters.AddWithValue("@FirstName", client.FirstName);
        com.Parameters.AddWithValue("@LastName", client.LastName);
        com.Parameters.AddWithValue("@Email", client.Email);
        com.Parameters.AddWithValue("@Telephone", client.Telephone);
        com.Parameters.AddWithValue("@Pesel", client.Pesel);
        await con.OpenAsync(cancellationToken);
        var noweId = await com.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(noweId);
    }

    public async Task<bool> IsClientRegisteredForTripAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("SELECT 1 FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip;", con);
        com.Parameters.AddWithValue("@IdClient", clientId);
        com.Parameters.AddWithValue("@IdTrip", tripId);
        await con.OpenAsync(cancellationToken);
        return (await com.ExecuteScalarAsync(cancellationToken)) != null;
    }

    public async Task<int> GetTripMaxPeopleAsync(int tripId, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("SELECT MaxPeople FROM Trip WHERE IdTrip = @IdTrip;", con);
        com.Parameters.AddWithValue("@IdTrip", tripId);
        await con.OpenAsync(cancellationToken);
        return (int)(await com.ExecuteScalarAsync(cancellationToken));
    }

    public async Task<int> GetCurrentTripParticipantsCountAsync(int tripId, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @IdTrip;", con);
        com.Parameters.AddWithValue("@IdTrip", tripId);
        await con.OpenAsync(cancellationToken);
        return (int)(await com.ExecuteScalarAsync(cancellationToken));
    }

    public async Task AddClientToTripAsync(int clientId, int tripId, int registeredAt, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt) VALUES (@IdClient, @IdTrip, @RegisteredAt)", con);
        com.Parameters.AddWithValue("@IdClient", clientId);
        com.Parameters.AddWithValue("@IdTrip", tripId);
        com.Parameters.AddWithValue("@RegisteredAt", registeredAt);
        await con.OpenAsync(cancellationToken);
        await com.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<bool> ClientTripRegistrationExistsAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("SELECT 1 FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip;", con);
        com.Parameters.AddWithValue("@IdClient", clientId);
        com.Parameters.AddWithValue("@IdTrip", tripId);
        await con.OpenAsync(cancellationToken);
        return (await com.ExecuteScalarAsync(cancellationToken)) != null;
    }

    public async Task DeleteClientFromTripAsync(int clientId, int tripId, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("DELETE FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip", con);
        com.Parameters.AddWithValue("@IdClient", clientId);
        com.Parameters.AddWithValue("@IdTrip", tripId);
        await con.OpenAsync(cancellationToken);
        await com.ExecuteNonQueryAsync(cancellationToken);
    }
    
    public async Task<bool> PeselExistsAsync(string pesel, CancellationToken cancellationToken)
    {
        await using var con=new SqlConnection(_connectionString);
        await using var com=new SqlCommand("SELECT 1 FROM Client WHERE Pesel = @Pesel;", con);
        com.Parameters.AddWithValue("@Pesel", pesel);
        await con.OpenAsync(cancellationToken);
        return (await com.ExecuteScalarAsync(cancellationToken))!=null;
    }
}