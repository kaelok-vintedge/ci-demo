namespace Api.Models;

public sealed class ClassSession
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public DateTime StartsAtUtc { get; init; }
    public int MaxCapacity { get; init; }
    public bool IsPeak { get; init; }
    public int CreditsPeak { get; init; }
    public int CreditsOffPeak { get; init; }
    public bool IsActive { get; init; } = true;
}
