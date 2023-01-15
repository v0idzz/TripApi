using System.ComponentModel.DataAnnotations;

namespace TripBooking.Api.Models;

public class RegisterForTripModel
{
    [EmailAddress]
    public required string EmailAddress { get; init; }
}