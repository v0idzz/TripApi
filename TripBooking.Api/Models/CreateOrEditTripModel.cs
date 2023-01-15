using System.ComponentModel.DataAnnotations;

namespace TripBooking.Api.Models;

public class CreateOrEditTripModel
{
    [Required]
    [MaxLength(50)]
    public required string Name { get; init; }

    [Required]
    [MaxLength(20)]
    public required string Country { get; init; }

    [Required]
    public required string Description { get; init; }

    public required DateTime StartDate { get; init; }

    [Range(1, 100)]
    public required int NumberOfSeats { get; init; }
}