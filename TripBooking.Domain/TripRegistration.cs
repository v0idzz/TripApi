namespace TripBooking.Domain;

public class TripRegistration
{
    public int Id { get; private set; }
    public int TripId { get; private set; }
    public string EmailAddress { get; private set; }

    internal TripRegistration(Trip trip, string emailAddress)
    {
        ArgumentNullException.ThrowIfNull(trip);
        ArgumentException.ThrowIfNullOrEmpty(emailAddress);

        TripId = trip.Id;
        EmailAddress = emailAddress;
    }

    // EF constructor
    private TripRegistration()
    {
    }
}