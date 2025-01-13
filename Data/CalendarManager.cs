using MultiplayerCalendarPlanner.Constants;
using MultiplayerCalendarPlanner.Sync;
using StardewModdingAPI;
using StardewValley;

namespace MultiplayerCalendarPlanner.Data;

public static class CalendarManager
{
    private static CalendarData _calendarData = new();
    private const string ModDataKey = "calendarActivityData";
    private const int DaysInSeason = 28;

    public static void LoadData()
    {
        _calendarData = ModEntry.StaticHelper.Data.ReadSaveData<CalendarData>(ModDataKey) ?? new CalendarData();
    }

    public static void SaveData()
    {
        ModEntry.StaticHelper.Data.WriteSaveData(ModDataKey, _calendarData);
    }

    public static void ClearData()
    {
        ModEntry.StaticHelper.Data.WriteSaveData<CalendarData>(ModDataKey, null);
    }

    public static void AddEvent(CalendarEvent calendarEvent)
    {
        _calendarData.Events.Add(calendarEvent);
    }

    public static void AddEvents(List<CalendarEvent> calendarEvents)
    {
        foreach (var calendarEvent in calendarEvents)
        {
            AddEvent(calendarEvent);
        }
    }

    public static bool RemoveEvent(CalendarEvent calendarEvent)
    {
        return _calendarData.Events.Remove(calendarEvent);
    }

    public static bool RemoveEvents(List<CalendarEvent> calendarEvents)
    {
        foreach (var calendarEvent in calendarEvents)
        {
            var res = RemoveEvent(calendarEvent);
            if (!res)
                return false;
        }

        return true;
    }

    public static void ReplaceCalendarData(CalendarData calendarData)
    {
        _calendarData = calendarData;
    }

    public static void SetBuildingActivities(int buildDays, long playerId)
    {
        var today = Game1.dayOfMonth;
        var currentSeason = Game1.season;
        var eventsToAdd = new List<CalendarEvent>();
        var eventsToRemove = new List<CalendarEvent>();

        for (var i = 0; i <= buildDays; i++)
        {
            var targetDay = today + i;
            var targetSeason = currentSeason;

            while (targetDay > DaysInSeason)
            {
                targetDay -= DaysInSeason;
                targetSeason = GetNextSeason(targetSeason);
            }

            eventsToRemove.AddRange(GetEventsForDay(targetDay, targetSeason)
                .Where(e => e.Activity == Activity.Robin)
                .ToList());

            var newEvent = new CalendarEvent(targetDay, targetSeason, playerId, Activity.Robin);
            eventsToAdd.Add(newEvent);
        }

        RemoveEvents(eventsToRemove);
        AddEvents(eventsToAdd);

        if (Context.IsMainPlayer)
        {
            MultiplayerManager.RemoveHostEventsFromFarmHands(eventsToRemove);
            MultiplayerManager.AddHostEventsToFarmHands(eventsToAdd);
            SaveData();
        }
        else
        {
            MultiplayerManager.RemoveFarmHandEventsFromAll(eventsToRemove);
            MultiplayerManager.AddFarmHandEventsToAll(eventsToAdd);
        }
    }


    public static List<CalendarEvent> GetEventsForDay(int day, Season season)
    {
        return _calendarData.Events.Where(e => e.Day == day && e.Season == season).ToList();
    }

    public static CalendarEvent? FindEvent(int day, Season season, long playerId, Activity activity)
    {
        var exampleEvent = new CalendarEvent(day, season, playerId, activity);

        return _calendarData.Events
            .FirstOrDefault(e => e.Equals(exampleEvent));
    }

    public static CalendarData GetCalendarData()
    {
        return _calendarData;
    }

    private static Season GetNextSeason(Season current)
    {
        return current switch
        {
            Season.Spring => Season.Summer,
            Season.Summer => Season.Fall,
            Season.Fall => Season.Winter,
            Season.Winter => Season.Spring,
            _ => throw new ArgumentOutOfRangeException(nameof(current), current, null)
        };
    }
}