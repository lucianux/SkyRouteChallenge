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
            // 1. Parallel execution of all available airline strategies
            var tasks = _providers.Select(async provider =>
            {
                try
                {
                    var results = await provider.SearchFlightsAsync(searchParams);
                    return (Provider: provider.ProviderName, Flights: results);
                }
                catch
                {
                    // Fault tolerance: if an external provider goes down, the aggregator continues to function
                    return (Provider: provider.ProviderName, Flights: Enumerable.Empty<InternalFlightResult>());
                }
            });

            var rawResults = await Task.WhenAll(tasks);
            var consolidatedFlights = new List<FlightResponse>();

            // 2. Unified Pricing Engine
            foreach (var result in rawResults)
            {
                foreach (var flight in result.Flights)
                {
                    decimal pricePerPassenger = CalculatePrice(result.Provider, flight.BaseFare);
                    decimal totalPrice = pricePerPassenger * searchParams.Passengers;

                    // Unique synthetic identifier to track this specific flight in the flow
                    string flightId = $"{result.Provider[..3].ToUpper()}-{flight.FlightNumber}-{searchParams.DepartureDate:yyyyMMdd}";

                    consolidatedFlights.Add(new FlightResponse(
                        flightId,
                        result.Provider,
                        flight.FlightNumber,
                        flight.DepartureTime,
                        flight.ArrivalTime,
                        flight.DurationMinutes,
                        flight.CabinClass,
                        pricePerPassenger,
                        totalPrice
                    ));
                }
            }

            return consolidatedFlights;
        }

        private static decimal CalculatePrice(string provider, decimal baseFare)
        {
            return provider.ToUpper() switch
            {
                "GLOBALAIR" => 
                    // Base + 15% fuel surcharge, rounded to 2 decimal places
                    Math.Round(baseFare * 1.15m, 2),

                "BUDGETWINGS" => 
                    // Base - 10% promotional discount, with a minimum purchase of $29.99
                    Math.Max(baseFare * 0.90m, 29.99m),

                _ => baseFare
            };
        }
    }
}