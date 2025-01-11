using MultiplayerCalendarPlanner.Constants;
using StardewValley;

namespace MultiplayerCalendarPlanner.Data
{
    public class CalendarEvent
    {
        public int Day { get; }

        public Season Season { get; }

        public long PlayerId { get; }

        public Activity Activity { get; }

        public CalendarEvent(int day, Season season, long playerId, Activity activity)
        {
            Day = day;
            Season = season;
            PlayerId = playerId;
            Activity = activity;
        }

        public override bool Equals(object? obj)
        {
            return obj is CalendarEvent other &&
                   Day == other.Day &&
                   Season == other.Season &&
                   PlayerId == other.PlayerId &&
                   Activity == other.Activity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Day, Season, PlayerId, Activity);
        }

        public override string ToString()
        {
            var playerName = Game1.player.UniqueMultiplayerID == PlayerId
                ? Game1.player.Name
                : Game1.otherFarmers.TryGetValue(PlayerId, out var farmer)
                    ? farmer.Name
                    : PlayerId.ToString();

            return ModEntry.StaticHelper.Translation.Get("event.string",
                new
                {
                    playerName,
                    activity = Activity,
                    day = Day,
                    season = Season
                });
        }
    }
}