namespace TripBooking.Api.Models;

public class TripListItemModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Country { get; init; }
    public required DateTime StartDate { get; init; }
}