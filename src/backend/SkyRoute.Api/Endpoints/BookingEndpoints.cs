using SkyRoute.Domain.Models;
using SkyRoute.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SkyRoute.Api.Endpoints;

public static class BookingEndpoints
{
  public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/bookings");

    // 1. Booking Confirmation Endpoint
    group.MapPost("/", async (
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
    })
    .WithName("BookingFlights");
  }
}
