using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingRequest request)
        {
            // 1. Basic business consistency validations
            if (request == null || request.Passengers == null || request.Passengers.Count == 0)
            {
                throw new ArgumentException("The reservation must contain at least one passenger.");
            }

            // 2. Extract data from the synthetic FlightId (e.g., "GBL-GA-742-20260615")
            // This allows us to know which provider originated the flight for internal audit purposes.
            var parts = request.FlightId.Split('-');
            if (parts.Length < 2)
            {
                throw new ArgumentException("The provided flight identifier (FlightId) is invalid.");
            }
            
            string providerCode = parts[0]; // GBL o BUD

            // In a real, full production workflow, we would use the providerCode to internally re-query the provider's tier price and ensure that the rate has not expired or been tampered with on the web client.

            // 3. Persisting the reserve by delegating to the abstraction of the repository
            var response = await _bookingRepository.SaveBookingAsync(request);

            return response;
        }
    }
}