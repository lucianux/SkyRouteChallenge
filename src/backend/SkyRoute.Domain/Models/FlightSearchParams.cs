using System;

namespace SkyRoute.Domain.Models
{
    public record FlightSearchParams(
        string Origin,
        string Destination,
        DateTime DepartureDate,
        int Passengers,
        string CabinClass
    );
}