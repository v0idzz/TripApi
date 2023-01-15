using Microsoft.EntityFrameworkCore;
using TripBooking.Api;
using TripBooking.Data;
using TripBooking.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITripConstraintsCheckService, TripConstraintsCheckService>();
builder.Services.AddDbContext<TripContext>(
    options => options.UseInMemoryDatabase("Db"));

var app = builder.Build();

app.ConfigureExceptionHandler();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// for integration tests
public partial class Program { }