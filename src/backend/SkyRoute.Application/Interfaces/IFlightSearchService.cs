using SkyRoute.Domain.Models;

namespace SkyRoute.Application
{
    public interface IFlightSearchService
    {
        Task<IEnumerable<FlightResponse>> SearchAndConsolidateAsync(FlightSearchParams searchParams);
    }
}