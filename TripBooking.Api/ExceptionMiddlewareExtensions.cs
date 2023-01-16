using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using TripBooking.Api.Models;
using TripBooking.Domain;

namespace TripBooking.Api;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(error =>
            error.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.GetRequiredFeature<IExceptionHandlerFeature>();

                if (exceptionHandlerFeature.Error is BusinessRuleViolationException businessException)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;

                    await context.Response.WriteAsJsonAsync(new ErrorModel(businessException.Message));
                }
            }));
    }
}