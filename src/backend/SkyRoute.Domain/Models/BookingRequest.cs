using System.Collections.Generic;

namespace SkyRoute.Domain.Models
{
    public record BookingRequest(
        string FlightId,
        List<PassengerDetails> Passengers
    );
}