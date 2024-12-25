using MultiplayerCalendarPlanner.Constants;
using StardewModdingAPI;
using StardewValley;

namespace MultiplayerCalendarPlanner.Data
{
    public class CalendarEvent
    {
        private int _day;
        private Season _season;
        private string _playerName;
        private Activity _activity;
        private readonly IModHelper _helper;

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

        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        public Activity Activity
        {
            get => _activity;
            set => _activity = value;
        }

        public CalendarEvent(int day, Season season, string playerName, Activity activity, IModHelper helper)
        {
            _day = day;
            _season = season;
            _playerName = playerName;
            _activity = activity;
            _helper = helper;
        }

        public override string ToString()
        {
            return _helper.Translation.Get("event.string",
                new
                {
                    playerName = _playerName,
                    activity = _activity,
                    day = _day,
                    season = _season
                });
        }
    }
}