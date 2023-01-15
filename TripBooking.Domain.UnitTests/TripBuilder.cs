namespace TripBooking.Domain.UnitTests;

internal class TripBuilder
{
    private string _name = "My trip";
    private string _country = "Egypt";
    private string _description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
    private DateTime _startDate = DateTime.Now.AddDays(7);
    private int _numberOfSeats = 25;
    
    public TripBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public TripBuilder WithCountry(string country)
    {
        _country = country;
        return this;
    }

    public TripBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TripBuilder WithStartDate(DateTime startDate)
    {
        _startDate = startDate;
        return this;
    }

    public TripBuilder WithNumberOfSeats(int numberOfSeats)
    {
        _numberOfSeats = numberOfSeats;
        return this;
    }

    public Trip Build()
    {
        return new Trip(_name, _country, _description, _startDate, _numberOfSeats);
    }
}