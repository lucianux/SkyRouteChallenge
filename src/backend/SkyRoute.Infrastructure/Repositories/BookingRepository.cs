using System.Collections.Concurrent;
using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;

namespace SkyRoute.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        // Almacenamiento en memoria Thread-Safe
        private static readonly ConcurrentDictionary<string, BookingRequest> _storage = new();

        public Task<BookingResponse> SaveBookingAsync(BookingRequest bookingRequest)
        {
            // Generamos un código de referencia único para el Challenge (ej: SR-2026-XF93)
            string referenceCode = $"SR-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString()[..4].ToUpper()}";

            // Guardamos la reserva indexada por su código de referencia
            bool isSaved = _storage.TryAdd(referenceCode, bookingRequest);

            if (!isSaved)
            {
                throw new InvalidOperationException("Hubo un conflicto de concurrencia al generar la referencia de la reserva.");
            }

            // Retornamos la respuesta con estado Confirmado y la fecha actual
            var response = new BookingResponse(
                referenceCode,
                "Confirmed",
                DateTime.UtcNow
            );

            return Task.FromResult(response);
        }
    }
}