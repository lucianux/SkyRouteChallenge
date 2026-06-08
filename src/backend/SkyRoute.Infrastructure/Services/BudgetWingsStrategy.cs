using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Services
{
  public class BudgetWingsStrategy : IAirlineProviderStrategy
  {
      public string ProviderName => "BudgetWings";

      public Task<IEnumerable<InternalFlightResult>> SearchFlightsAsync(FlightSearchParams searchParams)
      {
          var mockFlights = new List<InternalFlightResult>
          {
              new("BW-109", searchParams.DepartureDate.AddHours(11), searchParams.DepartureDate.AddHours(17).AddMinutes(45), 405, searchParams.CabinClass, 180.00m),
              new("BW-312", searchParams.DepartureDate.AddHours(20), searchParams.DepartureDate.AddHours(23).AddMinutes(50), 230, searchParams.CabinClass, 120.00m)
          };

          return Task.FromResult<IEnumerable<InternalFlightResult>>(mockFlights);
      }
  }
}