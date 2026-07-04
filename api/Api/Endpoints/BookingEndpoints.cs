using Api.Dtos;
using Api.Services;

namespace Api.Endpoints;

public static class BookingEndpoints
{
    public static void MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1");

        group.MapGet("/sessions", (BookingService bookings) =>
        {
            var sessions = bookings.GetSessions().Select(s => new SessionResponse(
                s.Id,
                s.Name,
                s.StartsAtUtc,
                s.MaxCapacity,
                SpotsLeft: s.MaxCapacity - bookings.CountBookings(s.Id),
                s.IsPeak,
                CostCredits: s.IsPeak ? s.CreditsPeak : s.CreditsOffPeak));

            return Results.Ok(sessions);
        });

        group.MapPost("/bookings", (CreateBookingRequest request, BookingService bookings) =>
        {
            var errors = CreateBookingRequestValidator.Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var result = bookings.CreateBooking(request.ClassSessionId, request.MemberName!.Trim());

            return result.Status switch
            {
                BookingResultStatus.Success => Results.Created(
                    $"/api/v1/bookings/{result.Booking!.Id}",
                    new BookingResponse(
                        result.Booking.Id,
                        result.Booking.ClassSessionId,
                        result.Booking.MemberName,
                        result.Booking.CostCredits)),
                BookingResultStatus.ClassNotFound => Results.NotFound(new { error = "Class session not found." }),
                BookingResultStatus.ClassInactive => Results.Conflict(new { error = "This class is not open for booking." }),
                BookingResultStatus.ClassStarted => Results.Conflict(new { error = "This class has already started." }),
                BookingResultStatus.CapacityFull => Results.Conflict(new { error = "This class is full." }),
                BookingResultStatus.AlreadyBooked => Results.Conflict(new { error = "You have already booked this class." }),
                _ => Results.Problem("Unexpected booking result."),
            };
        });
    }
}
