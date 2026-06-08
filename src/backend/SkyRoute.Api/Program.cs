using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application;
using SkyRoute.Infrastructure;
using SkyRoute.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// --- Registro de Servicios Modularizado ---
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORS amigable para el desarrollo local con Angular
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => 
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Activación de la UI de Swagger en entorno de Desarrollo
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkyRoute API v1");
    // Esto hace que Swagger sea la página de inicio (http://localhost:5000/)
    c.RoutePrefix = string.Empty;
  });
}

app.UseCors();

// --- Endpoints de la API ---

// 1. Endpoint de Búsqueda de Vuelos
app.MapGet("/api/flights", async (
    [FromQuery] string origin,
    [FromQuery] string destination,
    [FromQuery] DateTime departureDate,
    [FromQuery] int passengers,
    [FromQuery] string cabinClass,
    IFlightSearchService searchService) =>
{
    if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(destination))
        return Results.BadRequest("Origin and Destination are required.");

    if (passengers < 1 || passengers > 9)
        return Results.BadRequest("Passengers count must be between 1 and 9.");

    var searchParams = new FlightSearchParams(origin, destination, departureDate, passengers, cabinClass);
    var results = await searchService.SearchAndConsolidateAsync(searchParams);
    
    return Results.Ok(results);
});

// 2. Endpoint de Confirmación de Reservas
app.MapPost("/api/bookings", async (
    [FromBody] BookingRequest request,
    IBookingService bookingService) =>
{
    try
    {
        var response = await bookingService.CreateBookingAsync(request);
        // Devolvemos un 21 Created con la información de la reserva
        return Results.Json(response, statusCode: StatusCodes.Status201Created);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception)
    {
        return Results.StatusCode(500);
    }
});

app.MapGet("/", () => "App is running");

app.Run();
