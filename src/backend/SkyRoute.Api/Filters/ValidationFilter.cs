using FluentValidation;

namespace SkyRoute.Api.Filters
{
  public class ValidationFilter<T> : IEndpointFilter where T : class
  {
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
      // We look for the endpoint argument that matches type T (our DTO)
      var argument = context.Arguments.FirstOrDefault(x => x is T) as T;

      if (argument is null) return await next(context);

      // We require the validator registered from the DI container
      var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

      if (validator is not null)
      {
        var validationResult = await validator.ValidateAsync(argument);
        if (!validationResult.IsValid)
        {
          // We return a clean error dictionary compatible with ValidationProblem
          return Results.ValidationProblem(validationResult.ToDictionary());
        }
      }

      return await next(context);
    }
  }
}