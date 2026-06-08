using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            // 1. Ejecución en paralelo de todas las estrategias de aerolíneas disponibles
            var tasks = _providers.Select(async provider =>
            {
                try
                {
                    var results = await provider.SearchFlightsAsync(searchParams);
                    return (Provider: provider.ProviderName, Flights: results);
                }
                catch
                {
                    // Tolerancia a fallos: si un proveedor externo cae, el agregador sigue funcionando
                    return (Provider: provider.ProviderName, Flights: Enumerable.Empty<InternalFlightResult>());
                }
            });

            var rawResults = await Task.WhenAll(tasks);
            var consolidatedFlights = new List<FlightResponse>();

            // 2. Motor de Pricing unificado
            foreach (var result in rawResults)
            {
                foreach (var flight in result.Flights)
                {
                    decimal pricePerPassenger = CalculatePrice(result.Provider, flight.BaseFare);
                    decimal totalPrice = pricePerPassenger * searchParams.Passengers;

                    // Identificador sintético único para rastrear este vuelo específico en el flujo
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
                    // Base + 15% de recargo por combustible, redondeado a 2 decimales
                    Math.Round(baseFare * 1.15m, 2),

                "BUDGETWINGS" => 
                    // Base - 10% de descuento promocional, con un piso mínimo de $29.99
                    Math.Max(baseFare * 0.90m, 29.99m),

                _ => baseFare
            };
        }
    }
}