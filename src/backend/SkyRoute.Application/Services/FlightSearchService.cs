using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Services
{
  public class FlightSearchService : IFlightSearchService
  {
    private readonly IEnumerable<IAirlineProviderStrategy> _providers;
    private readonly IAirportService _airportService;

    public FlightSearchService(IEnumerable<IAirlineProviderStrategy> providers, IAirportService airportService)
    {
      _providers = providers;
      _airportService = airportService;
    }

    public async Task<IEnumerable<FlightResponse>> SearchAndConsolidateAsync(FlightSearchParams searchParams)
    {
      // 1. We triggered all searches in parallel at the same time
      var tasks = _providers.Select(async provider =>
      {
        var rawFlights = await provider.SearchFlightsAsync(searchParams);

        var originDetails = _airportService.GetDetailsByCode(searchParams.Origin);
        var destinationDetails = _airportService.GetDetailsByCode(searchParams.Destination);

        bool isInternational = !originDetails.Country.Equals(destinationDetails.Country, StringComparison.OrdinalIgnoreCase);

        // Artificial delay - Network Latency Simulation
        //await Task.Delay(2000);

        // Each provider maps and calculates its prices in isolation and independently.
        return rawFlights.Select(rawFlight =>
        {
          decimal finalPriceTotal = provider.CalculateFinalPrice(rawFlight.BaseFare, searchParams.Passengers);

          string flightId = $"{provider.ProviderName[..3].ToUpper()}-{rawFlight.FlightNumber}-{searchParams.DepartureDate:yyyyMMdd}";

          return new FlightResponse(
            flightId,
            originDetails,
            destinationDetails,
            provider.ProviderName,
            rawFlight.FlightNumber,
            rawFlight.DepartureTime,
            rawFlight.ArrivalTime,
            rawFlight.DurationMinutes,
            rawFlight.CabinClass,
            finalPriceTotal / searchParams.Passengers,
            finalPriceTotal,
            isInternational);
        });
      });
        
      // 2. We wait for all tasks to finish in parallel.
      var resultsMatrix = await Task.WhenAll(tasks);

      // 3. We flatten the results matrix (SelectMany) into a single linear list
      return resultsMatrix.SelectMany(flight => flight);
    }
  }
}