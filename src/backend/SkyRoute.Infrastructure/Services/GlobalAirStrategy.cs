using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Services
{
  public class GlobalAirStrategy : IAirlineProviderStrategy
  {
      public string ProviderName => "GlobalAir";

      public Task<IEnumerable<InternalFlightResult>> SearchFlightsAsync(FlightSearchParams searchParams)
      {
          // We simulate realistic data based on the search. 
          // In a real-world scenario, this would involve using HttpClient.GetAsync() on the GlobalAir API.
          var mockFlights = new List<InternalFlightResult>
          {
              new("GA-742", searchParams.DepartureDate.AddHours(8), searchParams.DepartureDate.AddHours(14).AddMinutes(15), 375, searchParams.CabinClass, 250.00m),
              new("GA-901", searchParams.DepartureDate.AddHours(16), searchParams.DepartureDate.AddHours(22).AddMinutes(30), 390, searchParams.CabinClass, 310.00m)
          };

          return Task.FromResult<IEnumerable<InternalFlightResult>>(mockFlights);
      }
      
      public decimal CalculateFinalPrice(decimal basePrice, int passengers)
      {
          // Base fare + 15% fuel surcharge
          decimal pricePerPassengerWithSurcharges = basePrice * 1.15m;

          decimal finalPrice = pricePerPassengerWithSurcharges * passengers;

          // Strict requirement: Always round the final price to 2 decimal places.
          return Math.Round(finalPrice, 2, MidpointRounding.AwayFromZero);
      }
  }
}