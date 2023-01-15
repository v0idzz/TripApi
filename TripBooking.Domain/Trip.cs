namespace TripBooking.Domain;

public class Trip
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Country { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public int NumberOfSeats { get; private set; }

    internal Trip(
        string name,
        string country,
        string description,
        DateTime startDate,
        int numberOfSeats)
    {
        Name = name;
        Country = country;
        Description = description;
        StartDate = startDate;
        NumberOfSeats = numberOfSeats;
    }

    public static async Task<Trip> CreateNewAsync(
        string name,
        string country,
        string description,
        DateTime startDate,
        int numberOfSeats,
        ITripConstraintsCheckService constraintsCheckService)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(country);
        ArgumentException.ThrowIfNullOrEmpty(description);

        if (!await constraintsCheckService.IsNameUniqueAsync(name))
        {
            throw new BusinessRuleViolationException("The trip name must be unique");
        }

        return new Trip(name, country, description, startDate, numberOfSeats);
    }

    public async Task<TripRegistration> CreateRegistrationAsync(string userEmailAddress,
        ITripConstraintsCheckService tripConstraintsCheckService)
    {
        if (!await tripConstraintsCheckService.IsReservationEmailUniqueAsync(Id, userEmailAddress))
        {
            throw new BusinessRuleViolationException("A single email can be registered for the trip only once");
        }

        var seatsTaken = await tripConstraintsCheckService.GetSeatsTakenAsync(Id);

        if (seatsTaken >= NumberOfSeats)
        {
            throw new BusinessRuleViolationException("Can't register more users for this trip");
        }

        return new TripRegistration(this, userEmailAddress);
    }

    public async Task EditDetailsAsync(
        string name,
        string country,
        string description,
        DateTime startDate,
        ITripConstraintsCheckService tripConstraintsCheckService)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(country);
        ArgumentException.ThrowIfNullOrEmpty(description);

        if (!await tripConstraintsCheckService.IsNameUniqueAsync(name, Id))
        {
            throw new BusinessRuleViolationException("The trip name must be unique");
        }

        Name = name;
        Country = country;
        Description = description;
        StartDate = startDate;
    }

    public async Task UpdateNumberOfSeatsAsync(int newMaxSeats, ITripConstraintsCheckService tripConstraintsCheckService)
    {
        var seatsTaken = await tripConstraintsCheckService.GetSeatsTakenAsync(Id);

        if (newMaxSeats < seatsTaken)
        {
            throw new BusinessRuleViolationException("Can't reduce seats number below number of seats taken already");
        }

        NumberOfSeats = newMaxSeats;
    }
}