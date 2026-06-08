using SkyRoute.Domain.Models;

namespace SkyRoute.Domain.Interfaces
{
    public interface IAirlineProviderStrategy
    {
        string ProviderName { get; }
        Task<IEnumerable<InternalFlightResult>> SearchFlightsAsync(FlightSearchParams searchParams);
    }
}