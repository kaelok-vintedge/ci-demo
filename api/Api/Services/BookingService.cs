using Api.Models;

namespace Api.Services;

public enum BookingResultStatus
{
    Success,
    ClassNotFound,
    ClassInactive,
    ClassStarted,
    CapacityFull,
    AlreadyBooked,
    TooCloseToStart
}

public sealed record BookingResult(BookingResultStatus Status, Booking? Booking = null);

// In-memory stand-in for the real database so the demo needs no infrastructure.
// The booking rules mirror the real platform: no booking a missing/inactive/
// started class, no overbooking capacity, no double-booking, and the credit
// cost depends on whether the session is peak.
public sealed class BookingService
{
    public const int BookingCutoffMinutes = 30;

    private readonly Lock _lock = new();
    private readonly List<ClassSession> _sessions;
    private readonly List<Booking> _bookings = [];
    private int _nextBookingId = 1;

    public BookingService(IEnumerable<ClassSession> sessions)
    {
        _sessions = [.. sessions];
    }

    public IReadOnlyList<ClassSession> GetSessions()
    {
        lock (_lock)
        {
            return [.. _sessions];
        }
    }

    public int CountBookings(int classSessionId)
    {
        lock (_lock)
        {
            return _bookings.Count(b => b.ClassSessionId == classSessionId);
        }
    }

    public BookingResult CreateBooking(int classSessionId, string memberName)
    {
        lock (_lock)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == classSessionId);
            if (session is null)
            {
                return new BookingResult(BookingResultStatus.ClassNotFound);
            }

            if (!session.IsActive)
            {
                return new BookingResult(BookingResultStatus.ClassInactive);
            }

            if (session.StartsAtUtc < DateTime.UtcNow)
            {
                return new BookingResult(BookingResultStatus.ClassStarted);
            }

            if (session.StartsAtUtc <= DateTime.UtcNow.AddMinutes(BookingCutoffMinutes))
            {
                return new BookingResult(BookingResultStatus.TooCloseToStart);
            }

            var currentCount = _bookings.Count(b => b.ClassSessionId == classSessionId);
            if (currentCount >= session.MaxCapacity)
            {
                return new BookingResult(BookingResultStatus.CapacityFull);
            }

            var alreadyBooked = _bookings.Any(b =>
                b.ClassSessionId == classSessionId &&
                b.MemberName.Equals(memberName, StringComparison.OrdinalIgnoreCase));
            if (alreadyBooked)
            {
                return new BookingResult(BookingResultStatus.AlreadyBooked);
            }

            var cost = session.IsPeak ? session.CreditsPeak : session.CreditsOffPeak;
            var booking = new Booking
            {
                Id = _nextBookingId++,
                ClassSessionId = classSessionId,
                MemberName = memberName,
                CostCredits = cost,
            };
            _bookings.Add(booking);

            return new BookingResult(BookingResultStatus.Success, booking);
        }
    }
}

public static class SeedData
{
    public static List<ClassSession> Sessions() =>
    [
        new()
        {
            Id = 1,
            Name = "Morning Yoga",
            StartsAtUtc = DateTime.UtcNow.AddDays(1).Date.AddHours(7),
            MaxCapacity = 12,
            IsPeak = false,
            CreditsPeak = 6,
            CreditsOffPeak = 4,
        },
        new()
        {
            Id = 2,
            Name = "Evening HIIT",
            StartsAtUtc = DateTime.UtcNow.AddDays(1).Date.AddHours(19),
            MaxCapacity = 8,
            IsPeak = true,
            CreditsPeak = 8,
            CreditsOffPeak = 5,
        },
        new()
        {
            Id = 3,
            Name = "Lunchtime Spin (already started)",
            StartsAtUtc = DateTime.UtcNow.AddHours(-2),
            MaxCapacity = 10,
            IsPeak = false,
            CreditsPeak = 6,
            CreditsOffPeak = 4,
        },
    ];
}
