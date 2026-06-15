using SkyRoute.Domain.Models;

namespace SkyRoute.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(BookingRequest request);
    }
}