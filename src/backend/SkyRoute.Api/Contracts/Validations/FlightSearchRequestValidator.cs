using FluentValidation;
using SkyRoute.Api.Contracts.Requests;

namespace SkyRoute.Api.Contracts.Validations
{
    public class FlightSearchRequestValidator : AbstractValidator<FlightSearchRequest>
    {
        public FlightSearchRequestValidator()
        {
            RuleFor(x => x.Origin)
                .NotEmpty().WithMessage("Origin is required.")
                .Length(3).WithMessage("Origin must be a 3-letter airport code.");

            RuleFor(x => x.Destination)
                .NotEmpty().WithMessage("Destination is required.")
                .Length(3).WithMessage("Destination must be a 3-letter airport code.");

            RuleFor(x => x.Passengers)
                .InclusiveBetween(1, 9).WithMessage("Passengers count must be between 1 and 9.");

            RuleFor(x => x.DepartureDate)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Departure date cannot be in the past.");
        }
    }
}