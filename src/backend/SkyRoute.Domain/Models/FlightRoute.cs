namespace SkyRoute.Domain.Models
{
    public record FlightRoute(string OriginAirport, string DestinationAirport)
    {
        // En un escenario real, mapearíamos los códigos IATA a una base de datos de países.
        // Para el mock, podemos inferir el país según la primera letra o con un diccionario simple de 6 aeropuertos.
        public bool IsInternational => GetCountry(OriginAirport) != GetCountry(DestinationAirport);

        public string RequiredDocumentLabel => IsInternational ? "Passport Number" : "National ID";

        private static string GetCountry(string airportCode)
        {
            return airportCode.ToUpper() switch
            {
                "EZE" or "AEP" => "Argentina",
                "JFK" or "MIA" => "USA",
                "MAD" or "BCN" => "Spain",
                _ => "Unknown" // Fallback seguro
            };
        }
    }
}