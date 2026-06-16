using System;

namespace SkyRoute.Domain.Models
{
    public record FlightResponse(
        string FlightId,
        AirportDetails Origin,
        AirportDetails Destination,
        string Provider,
        string FlightNumber,
        DateTime DepartureTime,
        DateTime ArrivalTime,
        int DurationMinutes,
        string CabinClass,
        decimal PricePerPassenger,
        decimal PriceTotal,
        bool IsInternational
    );
}