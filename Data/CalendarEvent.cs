using MultiplayerCalendarPlanner.Constants;
using StardewValley;

namespace MultiplayerCalendarPlanner.Data
{
    public class CalendarEvent
    {
        private int _day;
        private Season _season;
        private long _playerId;
        private Activity _activity;

        public int Day
        {
            get => _day;
            set => _day = value;
        }

        public Season Season
        {
            get => _season;
            set => _season = value;
        }

        public long PlayerId
        {
            get => _playerId;
            set => _playerId = value;
        }

        public Activity Activity
        {
            get => _activity;
            set => _activity = value;
        }

        public CalendarEvent(int day, Season season, long playerId, Activity activity)
        {
            _day = day;
            _season = season;
            _playerId = playerId;
            _activity = activity;
        }

        public override string ToString()
        {
            var playerName = Game1.player.UniqueMultiplayerID == _playerId
                ? Game1.player.Name
                : Game1.otherFarmers.TryGetValue(_playerId, out var farmer)
                    ? farmer.Name
                    : _playerId.ToString();

            return ModEntry.StaticHelper.Translation.Get("event.string",
                new
                {
                    playerName,
                    activity = _activity,
                    day = _day,
                    season = _season
                });
        }
    }
}