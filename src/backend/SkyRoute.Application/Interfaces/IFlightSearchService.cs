using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces
{
    public interface IFlightSearchService
    {
        Task<IEnumerable<FlightResponse>> SearchAndConsolidateAsync(FlightSearchParams searchParams);
    }
}