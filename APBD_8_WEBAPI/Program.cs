using APBD_8_WEBAPI.Repositories;
using APBD_8_WEBAPI.Services;

namespace APBD_8_WEBAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
        
        builder.Services.AddAuthorization();
        builder.Services.AddControllers();
        
        builder.Services.AddScoped<IClientRepository>(provider => new ClientRepository(connectionString));
        builder.Services.AddScoped<ITripRepository>(provider => new TripRepository(connectionString));
        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<ITripService, TripService>();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}