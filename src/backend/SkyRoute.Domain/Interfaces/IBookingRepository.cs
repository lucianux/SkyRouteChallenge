using System.Threading.Tasks;
using SkyRoute.Domain.Models;

namespace SkyRoute.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<BookingResponse> SaveBookingAsync(BookingRequest bookingRequest);
    }
}