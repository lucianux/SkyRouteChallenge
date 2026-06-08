using System;

namespace SkyRoute.Domain.Models
{
    public record FlightResponse(
        string FlightId,
        string Provider,
        string FlightNumber,
        DateTime DepartureTime,
        DateTime ArrivalTime,
        int DurationMinutes,
        string CabinClass,
        decimal PricePerPassenger,
        decimal PriceTotal
    );
}