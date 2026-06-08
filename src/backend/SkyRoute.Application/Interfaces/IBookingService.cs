using SkyRoute.Domain.Models;

namespace SkyRoute.Application
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(BookingRequest request);
    }
}