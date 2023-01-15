using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using TripBooking.Api.Models;
using Xunit;

namespace TripBooking.Api.IntegrationTests;

public class TripsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TripsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_WhenTripHasValidData_CreatesTrip()
    {
        // Arrange
        const string tripName = "My trip";
        const string tripCountry = "Egypt";
        const string tripDescription = "Some description...";
        const int numberOfSeats = 50;
        var startDate = DateTime.Now;

        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/trips", new CreateOrEditTripModel
        {
            Name = tripName,
            Country = tripCountry,
            Description = tripDescription,
            NumberOfSeats = numberOfSeats,
            StartDate = startDate
        });

        // Assert
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location;
        var trip = await client.GetFromJsonAsync<TripDetailsModel>(location);
        
        Assert.Equal(tripName, trip!.Name);
        Assert.Equal(tripCountry, trip.Country);
        Assert.Equal(tripDescription, trip.Description);
        Assert.Equal(numberOfSeats, trip.NumberOfSeats);
        Assert.Equal(startDate, trip.StartDate);
    }

    [Fact]
    public async Task Get_WhenExecuted_ReturnsTripsList()
    {
        // Arrange
        const string tripName = "My Egypt trip";
        const string tripCountry = "Egypt";
        var startDate = DateTime.Now;

        var client = _factory.CreateClient();

        await client.PostAsJsonAsync("/api/trips", new CreateOrEditTripModel
        {
            Name = tripName,
            Country = tripCountry,
            Description = "Some description...",
            NumberOfSeats = 50,
            StartDate = startDate
        });

        // Act
        var trips = await client.GetFromJsonAsync<IEnumerable<TripListItemModel>>("/api/trips");

        // Assert
        var createdTrip = trips!.Single(x => x.Name == tripName);

        Assert.Equal(tripName, createdTrip.Name);
        Assert.Equal(tripCountry, createdTrip.Country);
        Assert.Equal(startDate, createdTrip.StartDate);
    }

    [Fact]
    public async Task PostRegistrations_UniqueEmailAddress_RegistersForTrip()
    {
        // Arrange
        var client = _factory.CreateClient();

        var result = await client.PostAsJsonAsync("/api/trips", new CreateOrEditTripModel
        {
            Name = "Some trip",
            Country = "Egypt",
            Description = "Some description...",
            NumberOfSeats = 50,
            StartDate = DateTime.Now
        });

        var resourceUri = result.Headers.Location;
        
        // Act
        var response = await client.PostAsJsonAsync(resourceUri!.AbsolutePath + "/registrations", new RegisterForTripModel
        {
            EmailAddress = "example@example.com"
        });
        
        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Delete_WhenExecuted_DeletesTrip()
    {
        // Arrange
        var client = _factory.CreateClient();

        var result = await client.PostAsJsonAsync("/api/trips", new CreateOrEditTripModel
        {
            Name = "Some other trip",
            Country = "Egypt",
            Description = "Some description...",
            NumberOfSeats = 50,
            StartDate = DateTime.Now
        });

        var resourceUri = result.Headers.Location;
        
        // Act
        var response = await client.DeleteAsync(resourceUri);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var tripResponse = await client.GetAsync(resourceUri);
        
        Assert.Equal(HttpStatusCode.NotFound, tripResponse.StatusCode);
    }

    [Fact]
    public async Task Put_WhenExecuted_UpdatesTrip()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        const string newTripName = "The other trip 2";
        const string newTripCountry = "France";
        const string newTripDescription = "France trip description";
        const int newNumberOfSeats = 77;
        var newStartDate = DateTime.Now.AddDays(-7);

        var tripModel = new CreateOrEditTripModel
        {
            Name = "The other trip",
            Country = "Egypt",
            Description = "Some description...",
            NumberOfSeats = 50,
            StartDate = DateTime.Now
        };

        var result = await client.PostAsJsonAsync("/api/trips", tripModel);

        var resourceUri = result.Headers.Location;

        tripModel = new CreateOrEditTripModel
        {
            Name = newTripName,
            Country = newTripCountry,
            Description = newTripDescription,
            NumberOfSeats = newNumberOfSeats,
            StartDate = newStartDate
        };
        
        // Act
        result = await client.PutAsJsonAsync(resourceUri, tripModel);
        
        // Assert
        result.EnsureSuccessStatusCode();
        
        var trip = await client.GetFromJsonAsync<TripDetailsModel>(resourceUri);
        
        Assert.Equal(newTripName, trip!.Name);
        Assert.Equal(newTripCountry, trip.Country);
        Assert.Equal(newTripDescription, trip.Description);
        Assert.Equal(newNumberOfSeats, trip.NumberOfSeats);
        Assert.Equal(newStartDate, trip.StartDate);
    }
}