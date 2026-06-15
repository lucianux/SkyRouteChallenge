using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application;
using SkyRoute.Infrastructure;
using SkyRoute.Domain.Models;
using SkyRoute.Api.Contracts.Validations;
using SkyRoute.Api.Contracts.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using SkyRoute.Api.Filters;

var builder = WebApplication.CreateBuilder(args);

// Registers all validators of the current assembly
builder.Services.AddValidatorsFromAssemblyContaining<FlightSearchRequestValidator>();

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
  });
}

app.UseCors();

// --- API Endpoints ---

// 0. Health check
app.MapGet("/health", () => Results.Ok("Application is running"));

// 1. Flight Search Endpoint
app.MapGet("/api/flights", async (
    [AsParameters] FlightSearchRequest request,
    IFlightSearchService searchService) =>
{
    var searchParams = new FlightSearchParams(
        request.Origin,
        request.Destination,
        request.DepartureDate,
        request.Passengers,
        request.CabinClass
    );
    var results = await searchService.SearchAndConsolidateAsync(searchParams);
    return Results.Ok(results);
})
.AddEndpointFilter<ValidationFilter<FlightSearchRequest>>();

// 2. Booking Confirmation Endpoint
app.MapPost("/api/bookings", async (
    [FromBody] BookingRequest request,
    IBookingService bookingService) =>
{
    try
    {
        var response = await bookingService.CreateBookingAsync(request);
        // We return a response with the HTTP code 201, which means "Created" along with the booking information.
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
