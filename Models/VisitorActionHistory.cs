using System.Text.Json.Serialization;
namespace CastleGuardian.Api.Models;


public class VisitorActionHistory
{
    public int Id { get; set; }
    public int VisitorId { get; set; }

    [JsonIgnore]
    public Visitor Visitor { get; set; } = null!;

    public string ActionName { get; set; } = string.Empty;
    public int TrustChange { get; set; }
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
}
