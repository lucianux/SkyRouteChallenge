namespace SkyRoute.Api.Contracts.Requests
{
    public record FlightSearchRequest(
        string Origin,
        string Destination,
        DateTime DepartureDate,
        int Passengers,
        string CabinClass
    );
}