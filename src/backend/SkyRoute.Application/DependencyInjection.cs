using Microsoft.Extensions.DependencyInjection;
using SkyRoute.Application.Services;

namespace SkyRoute.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // We register the services specific to the application layer
            services.AddScoped<IFlightSearchService, FlightSearchService>();
            services.AddScoped<IBookingService, BookingService>();

            return services;
        }
    }
}