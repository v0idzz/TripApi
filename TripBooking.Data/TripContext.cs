using Microsoft.EntityFrameworkCore;
using TripBooking.Domain;

namespace TripBooking.Data;

public class TripContext : DbContext
{
    public DbSet<Trip> Trips { get; private set; }
    public DbSet<TripRegistration> TripRegistrations { get; private set; }

    public TripContext(DbContextOptions<TripContext> options) : base(options)
    {
    }
}