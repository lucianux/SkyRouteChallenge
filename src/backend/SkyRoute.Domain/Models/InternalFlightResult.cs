using System;

namespace SkyRoute.Domain.Models
{
    public record InternalFlightResult(
        string FlightNumber,
        DateTime DepartureTime,
        DateTime ArrivalTime,
        int DurationMinutes,
        string CabinClass,
        decimal BaseFare
    );
}