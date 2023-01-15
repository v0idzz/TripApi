using Moq;

namespace TripBooking.Domain.UnitTests;

public class TripTests
{
    [Fact]
    public async Task CreateRegistration_GivenNonUniqueEmailAddress_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        var trip = new TripBuilder().Build();

        var constraintsCheckService = new Mock<ITripConstraintsCheckService>();

        constraintsCheckService
            .Setup(x => x.GetSeatsTakenAsync(trip.Id))
            .ReturnsAsync(trip.NumberOfSeats + 1);

        constraintsCheckService
            .Setup(x => x.IsReservationEmailUniqueAsync(trip.Id, It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var fn = () => trip.CreateRegistrationAsync("example@example.com", constraintsCheckService.Object);

        // Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(fn);
    }

    [Fact]
    public async Task CreateRegistration_WhenNoSeatsAreLeft_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        const int numberOfSeats = 12;
        const int bookedSeats = 12;

        var trip = new TripBuilder()
            .WithNumberOfSeats(numberOfSeats)
            .Build();

        var constraintsCheckService = new Mock<ITripConstraintsCheckService>();

        constraintsCheckService
            .Setup(x => x.GetSeatsTakenAsync(trip.Id))
            .ReturnsAsync(bookedSeats);

        constraintsCheckService
            .Setup(x => x.IsReservationEmailUniqueAsync(trip.Id, It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var fn = () => trip.CreateRegistrationAsync("example@example.com", constraintsCheckService.Object);
        
        // Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(fn);
    }

    [Fact]
    public async Task CreateRegistration_WhenEmailIsUniqueAndSeatsAreFree_CreatesTripRegistration()
    {
        // Arrange
        const string emailAddress = "example@example.com";
        var trip = new TripBuilder().Build();

        var constraintsCheckService = new Mock<ITripConstraintsCheckService>();

        constraintsCheckService
            .Setup(x => x.GetSeatsTakenAsync(trip.Id))
            .ReturnsAsync(trip.NumberOfSeats - 1);

        constraintsCheckService
            .Setup(x => x.IsReservationEmailUniqueAsync(trip.Id, emailAddress))
            .ReturnsAsync(true);
        
        // Act
        var registration = await trip.CreateRegistrationAsync(emailAddress, constraintsCheckService.Object);

        // Assert
        Assert.Equal(emailAddress, registration.EmailAddress);
        Assert.Equal(trip.Id, registration.TripId);
    }

    [Fact]
    public async Task UpdateNumberOfSeatsAsync_WhenNewNumberOfSeatsIsBelowBooked_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        const int numberOfSeats = 10;
        const int bookedSeats = 8;
        const int newNumberOfSeats = 7;

        var trip = new TripBuilder().WithNumberOfSeats(numberOfSeats).Build();

        var constraintsCheckService = new Mock<ITripConstraintsCheckService>();

        constraintsCheckService
            .Setup(x => x.GetSeatsTakenAsync(trip.Id))
            .ReturnsAsync(bookedSeats);
        
        // Act
        var fn = () => trip.UpdateNumberOfSeatsAsync(newNumberOfSeats, constraintsCheckService.Object);
        
        // Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(fn);
    }
    
    [Fact]
    public async Task UpdateNumberOfSeatsAsync_WhenNewNumberOfSeatsIsAboveBooked_UpdatesNumberOfSeats()
    {
        // Arrange
        const int numberOfSeats = 10;
        const int bookedNumberOfSeats = 5;
        const int newNumberOfSeats = 8;

        var trip = new TripBuilder()
            .WithNumberOfSeats(numberOfSeats)
            .Build();

        var constraintsCheckService = new Mock<ITripConstraintsCheckService>();

        constraintsCheckService
            .Setup(x => x.GetSeatsTakenAsync(trip.Id))
            .ReturnsAsync(bookedNumberOfSeats);
        
        // Act
        await trip.UpdateNumberOfSeatsAsync(newNumberOfSeats, constraintsCheckService.Object);
        
        // Assert
        Assert.Equal(newNumberOfSeats, trip.NumberOfSeats);
    }

    [Fact]
    public async Task CreateNewTrip_WhenTripNameIsNonUnique_ThrowsBusinessRoleViolationException()
    {
        // Arrange
        const string tripName = "Egypt trip";
        var constraintsCheckService = Mock.Of<ITripConstraintsCheckService>(
            x => x.IsNameUniqueAsync(tripName, null) == Task.FromResult(false));

        // Act
        var fn = () =>
            Trip.CreateNewAsync(tripName, "Egypt", "Some description", DateTime.Now, 5, constraintsCheckService);
        
        // Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(fn);
    }

    [Fact]
    public async Task EditDetails_WhenNewTripNameIsNonUnique_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        const string newTripName = "Trip";
        var trip = new TripBuilder().Build();

        var constraintsCheckService = Mock.Of<ITripConstraintsCheckService>(
            x => x.IsNameUniqueAsync(newTripName, trip.Id) == Task.FromResult(false));
        
        // Act
        var fn = () => trip.EditDetailsAsync(newTripName,
            trip.Country,
            trip.Description,
            DateTime.Now,
            constraintsCheckService);
        
        // Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(fn);
    }
    
    [Fact]
    public async Task EditDetails_WhenNewTripNameIsUnique_EditsTripDetails()
    {
        // Arrange
        const string newTripName = "Trip";
        const string newTripCountry = "France";
        const string newTripDescription = "My trip to France";
        var newTripDate = DateTime.Now.AddDays(-7);

        var trip = new TripBuilder()
            .WithName("Old trip name")
            .WithCountry("Poland")
            .WithDescription("Trip to Poland")
            .WithStartDate(DateTime.Now)
            .Build();

        var constraintsCheckService = Mock.Of<ITripConstraintsCheckService>(
            x => x.IsNameUniqueAsync(newTripName, trip.Id) == Task.FromResult(true));
        
        // Act
        await trip.EditDetailsAsync(newTripName,
            newTripCountry,
            newTripDescription,
            newTripDate,
            constraintsCheckService);
        
        // Assert
        Assert.Equal(newTripName, trip.Name);
        Assert.Equal(newTripCountry, trip.Country);
        Assert.Equal(newTripDescription, trip.Description);
        Assert.Equal(newTripDate, trip.StartDate);
    }
}