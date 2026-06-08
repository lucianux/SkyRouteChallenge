using System;
using System.Threading.Tasks;
using SkyRoute.Domain.Interfaces;
using SkyRoute.Domain.Models;

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
            // 1. Validaciones de consistencia de negocio básicas
            if (request == null || request.Passengers == null || request.Passengers.Count == 0)
            {
                throw new ArgumentException("La reserva debe contener al menos un pasajero.");
            }

            // 2. Extraer datos del FlightId sintético (ej: "GBL-GA-742-20260615")
            // Esto nos permite saber qué proveedor originó el vuelo para auditoría interna
            var parts = request.FlightId.Split('-');
            if (parts.Length < 2)
            {
                throw new ArgumentException("El identificador de vuelo (FlightId) provisto es inválido.");
            }
            
            string providerCode = parts[0]; // GBL o BUD

            // [Nota de Senior]: En un flujo real de producción completa, aquí usaríamos el providerCode 
            // para volver a consultar de manera interna el precio del tramo del proveedor y asegurar 
            // que la tarifa no expiró ni fue manipulada en el cliente Web.

            // 3. Persistir la reserva delegando en la abstracción del repositorio
            var response = await _bookingRepository.SaveBookingAsync(request);

            return response;
        }
    }
}