using SkyRoute.Domain.Models;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Infrastructure.Services;

public class AirportService : IAirportService
{
    // Static dictionary that simulates our Airports database table
    private static readonly Dictionary<string, (string Name, string Country)> _airports = new(StringComparer.OrdinalIgnoreCase)
    {
        { "EZE", ("Ministro Pistarini International Airport", "Argentina") },
        { "AEP", ("Aeroparque Jorge Newbery", "Argentina") },
        { "JFK", ("John F. Kennedy International Airport", "USA") },
        { "MIA", ("Miami International Airport", "USA") },
        { "MAD", ("Adolfo Suárez Madrid-Barajas Airport", "Spain") },
        { "BCN", ("Josep Tarradellas Barcelona-El Prat Airport", "Spain") }
    };

    public AirportDetails GetDetailsByCode(string iataCode)
    {
        var code = iataCode?.Trim().ToUpper() ?? string.Empty;

        if (_airports.TryGetValue(code, out var data))
        {
            return new AirportDetails(code, data.Name, data.Country);
        }

        // If they send code we don't know, we return a safe generic object instead of breaking
        return new AirportDetails(code, "Unknown Airport", "Unknown");
    }
}