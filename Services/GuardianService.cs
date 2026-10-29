using CastleGuardian.Api.Data;
using CastleGuardian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CastleGuardian.Api.Services;

public class GuardianService
{
    private readonly AppDbContext _db;

    public GuardianService(AppDbContext db)
    {
        _db = db;
    }

    public static int? GetTrustDelta(string action) => action switch
    {
        "Introduce" => 20,
        "OfferHelp" => 30,
        "Threaten" => -20,
        _ => null
    };

    public async Task<(Visitor? visitor, string? error, int? newTrust)> ExecuteActionAsync(int visitorId, string action)
    {
        var visitor = await _db.Visitors
            .Include(v => v.ActionHistory)
            .FirstOrDefaultAsync(v => v.Id == visitorId);

        if (visitor is null)
            return (null, "ѕосетитель не найден", null);

        var delta = GetTrustDelta(action);
        if (delta is null)
            return (visitor, "Ќедопустимое действие", null);

        if (visitor.ActionHistory.Any(h => h.ActionName == action))
            return (visitor, "Ёто действие уже было выполнено", null);

        visitor.TrustLevel = Math.Clamp(visitor.TrustLevel + delta.Value, 0, 100);

        var historyRecord = new VisitorActionHistory
        {
            VisitorId = visitor.Id,
            ActionName = action,
            TrustChange = delta.Value
        };

        _db.VisitorActionHistories.Add(historyRecord);
        _db.Visitors.Update(visitor);
        await _db.SaveChangesAsync();

        return (visitor, null, visitor.TrustLevel);
    }
}