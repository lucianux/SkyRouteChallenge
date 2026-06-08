using Microsoft.Extensions.DependencyInjection;
using SkyRoute.Application.Services;

namespace SkyRoute.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registramos los servicios propios de la capa de aplicación
            services.AddScoped<IFlightSearchService, FlightSearchService>();
            services.AddScoped<IBookingService, BookingService>();

            return services;
        }
    }
}