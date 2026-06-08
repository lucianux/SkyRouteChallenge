using System;

namespace SkyRoute.Domain.Models
{
    public record BookingResponse(
        string BookingReference,
        string Status,
        DateTime CreatedAt
    );
}