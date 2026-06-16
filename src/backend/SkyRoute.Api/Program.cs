using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application;
using SkyRoute.Infrastructure;
using SkyRoute.Domain.Models;
using SkyRoute.Api.Contracts.Validations;
using SkyRoute.Api.Contracts.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using SkyRoute.Api.Filters;
using SkyRoute.Api.Endpoints;
using SkyRoute.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Registers all validators of the current assembly
builder.Services.AddValidatorsFromAssemblyContaining<FlightSearchRequestValidator>();

// Modularized Service Registry
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string AngularDevPolicy = "AngularDevCors";
const string AngularProdPolicy = "AngularProdCors";

// Registration and Policy Configuration
builder.Services.AddCors(options =>
{
    // PRD
    options.AddPolicy(AngularProdPolicy, policy =>
    {
        policy.WithOrigins("https://www.skyroute.com")
            .WithMethods("GET", "POST")
            .AllowAnyHeader();
    });

    // Dev
    options.AddPolicy(AngularDevPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
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

if (app.Environment.IsDevelopment())
{
    app.UseCors(AngularDevPolicy);
}
else
{
    app.UseCors(AngularProdPolicy);
}

// --- API Endpoints ---

// Health check endpoint
app.MapGet("/health", () => Results.Ok("Application is running"));

app.MapFlightEndpoints();

app.MapBookingEndpoints();

app.Run();
