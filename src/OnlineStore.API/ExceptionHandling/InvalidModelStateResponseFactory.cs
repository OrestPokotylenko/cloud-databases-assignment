using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.API.ExceptionHandling;

public static class InvalidModelStateResponseFactory
{
    public static IActionResult Create(ActionContext context)
    {
        var messages = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct();

        var errors = string.Join(" ", messages);
        if (string.IsNullOrWhiteSpace(errors))
        {
            errors = "The request contains invalid data.";
        }

        return new BadRequestObjectResult(
            new ApiErrorResponse(
                "Validation failed",
                errors,
                Activity.Current?.Id ?? context.HttpContext.TraceIdentifier));
    }
}
