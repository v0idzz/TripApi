using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TripBooking.Api.Models;
using TripBooking.Data;
using TripBooking.Domain;

namespace TripBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly TripContext _tripContext;
    private readonly ITripConstraintsCheckService _tripConstraintsCheckService;

    public TripsController(TripContext tripContext, ITripConstraintsCheckService tripConstraintsCheckService)
    {
        _tripContext = tripContext;
        _tripConstraintsCheckService = tripConstraintsCheckService;
    }

    [HttpGet]
    public IAsyncEnumerable<TripListItemModel> GetTripsListAsync([FromQuery] string? country)
    {
        IQueryable<Trip> queryable = _tripContext.Trips;

        if (!string.IsNullOrEmpty(country))
        {
            queryable = queryable.Where(x => x.Country == country);
        }

        return queryable
            .Select(x => new TripListItemModel
            {
                Id = x.Id,
                Country = x.Country,
                Name = x.Name,
                StartDate = x.StartDate
            })
            .AsAsyncEnumerable();
    }

    [HttpGet("{tripId:int}")]
    public async Task<ActionResult<TripDetailsModel>> GetTripAsync(int tripId)
    {
        var trip = await _tripContext.Trips.FindAsync(tripId);

        if (trip is null)
        {
            return NotFound(new ErrorModel(TripNotFound));
        }

        return new TripDetailsModel
        {
            Id = trip.Id,
            Country = trip.Country,
            Description = trip.Description,
            Name = trip.Name,
            NumberOfSeats = trip.NumberOfSeats,
            StartDate = trip.StartDate
        };
    }

    [HttpPost]
    public async Task<ActionResult> CreateTripAsync([FromBody] CreateOrEditTripModel model)
    {
        var trip = await Trip.CreateNewAsync(
            model.Name,
            model.Country,
            model.Description,
            model.StartDate,
            model.NumberOfSeats,
            _tripConstraintsCheckService);

        await _tripContext.AddAsync(trip);
        await _tripContext.SaveChangesAsync();

        return CreatedAtAction("GetTrip", new {tripId = trip.Id}, null);
    }

    [HttpPut("{tripId:int}")]
    public async Task<ActionResult> EditTripAsync(int tripId, [FromBody] CreateOrEditTripModel model)
    {
        var trip = await _tripContext.Trips.FindAsync(tripId);

        if (trip is null)
        {
            return NotFound(new ErrorModel(TripNotFound));
        }

        await trip.EditDetailsAsync(
            model.Name,
            model.Country,
            model.Description,
            model.StartDate,
            _tripConstraintsCheckService);

        await trip.UpdateNumberOfSeatsAsync(model.NumberOfSeats, _tripConstraintsCheckService);

        await _tripContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{tripId:int}")]
    public async Task<ActionResult> DeleteTripAsync(int tripId)
    {
        var trip = await _tripContext.Trips.FindAsync(tripId);

        if (trip is null)
        {
            return NotFound(new ErrorModel(TripNotFound));
        }

        _tripContext.Trips.Remove(trip);

        await _tripContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{tripId:int}/registrations")]
    public async Task<ActionResult> RegisterForTripAsync(int tripId, [FromBody] RegisterForTripModel model)
    {
        var trip = await _tripContext.Trips.FindAsync(tripId);

        if (trip is null)
        {
            return NotFound(new ErrorModel(TripNotFound));
        }

        var registration = await trip.CreateRegistrationAsync(model.EmailAddress, _tripConstraintsCheckService);

        await _tripContext.TripRegistrations.AddAsync(registration);

        await _tripContext.SaveChangesAsync();

        return Ok();
    }

    private const string TripNotFound = "Trip was not found";
}