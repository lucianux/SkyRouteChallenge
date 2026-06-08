namespace SkyRoute.Domain.Models
{
    public record PassengerDetails(
        string FullName,
        string Email,
        string DocumentNumber
    );
}