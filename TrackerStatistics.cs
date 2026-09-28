namespace DEATHTRACKERARCHIPELAGO;

public enum StatisticsSort { MostDeaths, LeastDeaths, Alphabetical, HighestStreak }
public record PlayerStatistics(string Name, int Deaths, int CurrentStreak, int HighestStreak, int Rank);

public sealed class StreakTracker
{
    public string ActivePlayer { get; private set; } = "";
    public int ActiveCount { get; private set; }
    public Dictionary<string, int> Highest { get; private set; } = new();

    public void Record(string player)
    {
        ActiveCount = ActivePlayer == player ? ActiveCount + 1 : 1;
        ActivePlayer = player;
        Highest[player] = Math.Max(Highest.GetValueOrDefault(player), ActiveCount);
    }

    public void Restore(DeathTrackerState state)
    {
        ActivePlayer = "";
        ActiveCount = 0;
        Highest = new();
        // Old servers/clients only know history. Recover the streaks available in that window.
        foreach (var death in state.History.AsEnumerable().Reverse()) Record(death.Player);
        if (state.HighestStreaks != null)
            foreach (var pair in state.HighestStreaks)
                Highest[pair.Key] = Math.Max(Highest.GetValueOrDefault(pair.Key), pair.Value);
        if (state.ActiveStreak > 0 && !string.IsNullOrEmpty(state.ActiveStreakPlayer))
        {
            ActivePlayer = state.ActiveStreakPlayer;
            ActiveCount = state.ActiveStreak;
            Highest[ActivePlayer] = Math.Max(Highest.GetValueOrDefault(ActivePlayer), ActiveCount);
        }
    }

    public List<PlayerStatistics> Snapshot(Dictionary<string, int> counts, StatisticsSort sort)
    {
        var ranked = counts.OrderByDescending(p => p.Value).ThenBy(p => p.Key, StringComparer.OrdinalIgnoreCase).ToList();
        var rows = ranked.Select(p => new PlayerStatistics(p.Key, p.Value,
            p.Value == 0 ? 0 : p.Key == ActivePlayer ? ActiveCount : 1,
            Math.Max(p.Value > 0 ? 1 : 0, Highest.GetValueOrDefault(p.Key)),
            1 + ranked.Count(other => other.Value > p.Value)));
        return (sort switch
        {
            StatisticsSort.LeastDeaths => rows.OrderBy(p => p.Deaths).ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase),
            StatisticsSort.Alphabetical => rows.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase),
            StatisticsSort.HighestStreak => rows.OrderByDescending(p => p.HighestStreak).ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase),
            _ => rows
        }).ToList();
    }
}
