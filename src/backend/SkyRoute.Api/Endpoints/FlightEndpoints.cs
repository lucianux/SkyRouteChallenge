using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.Interfaces;
using SkyRoute.Api.Contracts.Requests;
using SkyRoute.Api.Filters;
using SkyRoute.Domain.Models;

namespace SkyRoute.Api.Endpoints;

public static class FlightEndpoints
{
  public static void MapFlightEndpoints(this IEndpointRouteBuilder app)
  {
    // We group all endpoints under the prefix "/api/flights"
    var group = app.MapGroup("/api/flights");

    // 1. Flight Search Endpoint
    group.MapGet("/", async (
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
    .WithName("SearchFlights")
    .AddEndpointFilter<ValidationFilter<FlightSearchRequest>>();
  }
}