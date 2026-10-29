namespace CastleGuardian.Api.Models;

public class Visitor
{
    public int Id { get; set; }
    public int TrustLevel { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<VisitorActionHistory> ActionHistory { get; set; } = new();
}