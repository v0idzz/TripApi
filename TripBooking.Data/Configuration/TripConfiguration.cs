using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripBooking.Domain;

namespace TripBooking.Data.Configuration;

internal class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(50);
        builder.Property(x => x.Country).HasMaxLength(20);

        builder.HasMany<TripRegistration>()
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}