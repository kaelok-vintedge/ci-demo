using Api.Models;
using Api.Services;

namespace Api.Tests;

public class BookingServiceTests
{
    private static ClassSession Session(
        int id = 1,
        int maxCapacity = 10,
        bool isPeak = false,
        DateTime? startsAtUtc = null,
        bool isActive = true) => new()
    {
        Id = id,
        Name = "Test Class",
        StartsAtUtc = startsAtUtc ?? DateTime.UtcNow.AddDays(1),
        MaxCapacity = maxCapacity,
        IsPeak = isPeak,
        CreditsPeak = 8,
        CreditsOffPeak = 5,
        IsActive = isActive,
    };

    [Fact]
    public void CreateBooking_UnknownSession_ReturnsClassNotFound()
    {
        var service = new BookingService([Session(id: 1)]);

        var result = service.CreateBooking(999, "Alice");

        Assert.Equal(BookingResultStatus.ClassNotFound, result.Status);
    }

    [Fact]
    public void CreateBooking_SessionAlreadyStarted_ReturnsClassStarted()
    {
        var service = new BookingService([Session(startsAtUtc: DateTime.UtcNow.AddHours(-1))]);

        var result = service.CreateBooking(1, "Alice");

        Assert.Equal(BookingResultStatus.ClassStarted, result.Status);
    }

    [Fact]
    public void CreateBooking_AtCapacity_ReturnsCapacityFull()
    {
        var service = new BookingService([Session(maxCapacity: 2)]);
        service.CreateBooking(1, "Alice");
        service.CreateBooking(1, "Bob");

        var result = service.CreateBooking(1, "Carol");

        Assert.Equal(BookingResultStatus.CapacityFull, result.Status);
        Assert.Equal(2, service.CountBookings(1));
    }

    [Fact]
    public void CreateBooking_SameMemberTwice_ReturnsAlreadyBooked()
    {
        var service = new BookingService([Session()]);
        service.CreateBooking(1, "Alice");

        var result = service.CreateBooking(1, "alice");

        Assert.Equal(BookingResultStatus.AlreadyBooked, result.Status);
        Assert.Equal(1, service.CountBookings(1));
    }

    [Fact]
    public void CreateBooking_PeakSession_ChargesPeakCredits()
    {
        var service = new BookingService([Session(isPeak: true)]);

        var result = service.CreateBooking(1, "Alice");

        Assert.Equal(BookingResultStatus.Success, result.Status);
        Assert.Equal(8, result.Booking!.CostCredits);
    }

    [Fact]
    public void CreateBooking_OffPeakSession_ChargesOffPeakCredits()
    {
        var service = new BookingService([Session(isPeak: false)]);

        var result = service.CreateBooking(1, "Alice");

        Assert.Equal(BookingResultStatus.Success, result.Status);
        Assert.Equal(5, result.Booking!.CostCredits);
    }

    [Fact]
    public void CreateBooking_Valid_PersistsBooking()
    {
        var service = new BookingService([Session()]);

        var result = service.CreateBooking(1, "Alice");

        Assert.Equal(BookingResultStatus.Success, result.Status);
        Assert.NotNull(result.Booking);
        Assert.Equal("Alice", result.Booking.MemberName);
        Assert.Equal(1, service.CountBookings(1));
    }
}
