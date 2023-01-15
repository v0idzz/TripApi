namespace TripBooking.Api.Models;

public class TripDetailsModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required string Description { get; init; }
    public required DateTime StartDate { get; init; }
    public required int NumberOfSeats { get; init; }
}