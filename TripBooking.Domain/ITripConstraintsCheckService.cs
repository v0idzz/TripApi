namespace TripBooking.Domain;

public interface ITripConstraintsCheckService
{
    Task<bool> IsReservationEmailUniqueAsync(int tripId, string emailAddress);

    Task<int> GetSeatsTakenAsync(int tripId);

    Task<bool> IsNameUniqueAsync(string name, int? tripId = null);
}