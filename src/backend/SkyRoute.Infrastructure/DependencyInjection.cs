using Microsoft.Extensions.DependencyInjection;
using SkyRoute.Domain.Interfaces;
using SkyRoute.Infrastructure.Repositories;
using SkyRoute.Infrastructure.Services;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<IAirportService, AirportService>();
            services.AddSingleton<IBookingRepository, BookingRepository>();
            services.AddScoped<IAirlineProviderStrategy, GlobalAirStrategy>();
            services.AddScoped<IAirlineProviderStrategy, BudgetWingsStrategy>();

            return services;
        }
    }
}