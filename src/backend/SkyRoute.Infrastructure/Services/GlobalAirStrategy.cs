using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Services
{
  public class GlobalAirStrategy : IAirlineProviderStrategy
  {
      public string ProviderName => "GlobalAir";

      public Task<IEnumerable<InternalFlightResult>> SearchFlightsAsync(FlightSearchParams searchParams)
      {
          // Simulamos datos realistas basados en la búsqueda. 
          // En un escenario real, acá se haría un HttpClient.GetAsync() a la API de GlobalAir.
          var mockFlights = new List<InternalFlightResult>
          {
              new("GA-742", searchParams.DepartureDate.AddHours(8), searchParams.DepartureDate.AddHours(14).AddMinutes(15), 375, searchParams.CabinClass, 250.00m),
              new("GA-901", searchParams.DepartureDate.AddHours(16), searchParams.DepartureDate.AddHours(22).AddMinutes(30), 390, searchParams.CabinClass, 310.00m)
          };

          return Task.FromResult<IEnumerable<InternalFlightResult>>(mockFlights);
      }
  }
}