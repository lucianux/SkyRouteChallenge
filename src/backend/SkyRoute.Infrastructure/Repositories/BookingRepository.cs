using System.Collections.Concurrent;
using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        // Thread-Safe memory storage
        private static readonly ConcurrentDictionary<string, BookingRequest> _storage = new();

        public Task<BookingResponse> SaveBookingAsync(BookingRequest bookingRequest)
        {
            // We generate a unique reference code for the Challenge (e.g., SR-2026-XF93)
            string referenceCode = $"SR-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

            // We saved the reservation indexed by its reference code
            bool isSaved = _storage.TryAdd(referenceCode, bookingRequest);

            if (!isSaved)
            {
                throw new InvalidOperationException("There was a conflict of concurrency when generating the reservation reference.");
            }

            // We return the response with the status Confirmed and the current date
            var response = new BookingResponse(
                referenceCode,
                "Confirmed",
                DateTime.UtcNow
            );

            return Task.FromResult(response);
        }
    }
}