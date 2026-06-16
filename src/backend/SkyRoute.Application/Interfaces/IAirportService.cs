using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces;

public interface IAirportService
{
    AirportDetails GetDetailsByCode(string iataCode);
}