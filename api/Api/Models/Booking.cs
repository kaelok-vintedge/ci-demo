namespace Api.Models;

public sealed class Booking
{
    public int Id { get; init; }
    public int ClassSessionId { get; init; }
    public required string MemberName { get; init; }
    public int CostCredits { get; init; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}
