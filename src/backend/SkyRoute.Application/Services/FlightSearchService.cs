using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Services
{
  public class FlightSearchService : IFlightSearchService
  {
    private readonly IEnumerable<IAirlineProviderStrategy> _providers;

    public FlightSearchService(IEnumerable<IAirlineProviderStrategy> providers)
    {
      _providers = providers;
    }

    public async Task<IEnumerable<FlightResponse>> SearchAndConsolidateAsync(FlightSearchParams searchParams)
    {
      // 1. We triggered all searches in parallel at the same time
      var tasks = _providers.Select(async provider =>
      {
        var rawFlights = await provider.SearchFlightsAsync(searchParams);

        // Artificial delay - Network Latency Simulation
        //await Task.Delay(2000);

        // Each provider maps and calculates its prices in isolation and independently.
        return rawFlights.Select(rawFlight =>
        {
          decimal finalPriceTotal = provider.CalculateFinalPrice(rawFlight.BaseFare, searchParams.Passengers);

          string flightId = $"{provider.ProviderName[..3].ToUpper()}-{rawFlight.FlightNumber}-{searchParams.DepartureDate:yyyyMMdd}";

          return new FlightResponse(
            flightId,
            provider.ProviderName,
            rawFlight.FlightNumber,
            rawFlight.DepartureTime,
            rawFlight.ArrivalTime,
            rawFlight.DurationMinutes,
            rawFlight.CabinClass,
            finalPriceTotal / searchParams.Passengers,
            finalPriceTotal);
        });
      });
        
      // 2. We wait for all tasks to finish in parallel.
      var resultsMatrix = await Task.WhenAll(tasks);

      // 3. We flatten the results matrix (SelectMany) into a single linear list
      return resultsMatrix.SelectMany(flight => flight);
    }
  }
}