using Microsoft.EntityFrameworkCore;
using TripBooking.Data;
using TripBooking.Domain;

namespace TripBooking.Api;

public class TripConstraintsCheckService : ITripConstraintsCheckService
{
    private readonly TripContext _context;

    public TripConstraintsCheckService(TripContext context)
    {
        _context = context;
    }

    public async Task<bool> IsReservationEmailUniqueAsync(int tripId, string emailAddress)
    {
        return !await _context.TripRegistrations.AnyAsync(x => x.TripId == tripId && x.EmailAddress == emailAddress);
    }

    public async Task<int> GetSeatsTakenAsync(int tripId)
    {
        return await _context.TripRegistrations.CountAsync(x => x.TripId == tripId);
    }

    public async Task<bool> IsNameUniqueAsync(string name, int? tripId = null)
    {
        return await _context.Trips.AllAsync(x => x.Name != name || x.Id == tripId);
    }
}