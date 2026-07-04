namespace Api.Dtos;

// DTOs define the shape of data crossing the API boundary.
// The client never sends or receives domain models directly.

public sealed record CreateBookingRequest(int ClassSessionId, string? MemberName);

public sealed record BookingResponse(int Id, int ClassSessionId, string MemberName, int CostCredits);

public sealed record SessionResponse(
    int Id,
    string Name,
    DateTime StartsAtUtc,
    int MaxCapacity,
    int SpotsLeft,
    bool IsPeak,
    int CostCredits);

public static class CreateBookingRequestValidator
{
    // Input is untrusted until validated here, at the boundary.
    public static Dictionary<string, string[]> Validate(CreateBookingRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.ClassSessionId <= 0)
        {
            errors[nameof(request.ClassSessionId)] = ["ClassSessionId must be a positive integer."];
        }

        var name = request.MemberName?.Trim();
        if (string.IsNullOrEmpty(name))
        {
            errors[nameof(request.MemberName)] = ["MemberName is required."];
        }
        else if (name.Length is < 2 or > 50)
        {
            errors[nameof(request.MemberName)] = ["MemberName must be between 2 and 50 characters."];
        }

        return errors;
    }
}
