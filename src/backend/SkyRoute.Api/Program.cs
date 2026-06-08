using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application;
using SkyRoute.Infrastructure;
using SkyRoute.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Modularized Service Registry
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Friendly CORS configuration for local development with Angular
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => 
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Activating the Swagger UI in a Development Environment
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkyRoute API v1");
    // This makes the Swagger tool appear on the home page (http://localhost:5122/)
    c.RoutePrefix = string.Empty;
  });
}

app.UseCors();

// --- API Endpoints ---

// 1. Flight Search Endpoint
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

// 2. Booking Confirmation Endpoint
app.MapPost("/api/bookings", async (
    [FromBody] BookingRequest request,
    IBookingService bookingService) =>
{
    try
    {
        var response = await bookingService.CreateBookingAsync(request);
        // We return a response with the HTTP code 201, which means "Created," along with the booking information.
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

app.Run();
