using MultiplayerCalendarPlanner.Constants;
using StardewValley;

namespace MultiplayerCalendarPlanner.Data;

public static class CalendarManager
{
    private static CalendarData _calendarData = new();
    private const string ModDataKey = "calendarActivityData";

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
        ModEntry.StaticHelper.Data.WriteSaveData<>(ModDataKey, null);
    }

    public static void AddEvent(CalendarEvent calendarEvent)
    {
        _calendarData.Events.Add(calendarEvent);
    }

    public static bool RemoveEvent(CalendarEvent calendarEvent)
    {
        return _calendarData.Events.Remove(calendarEvent);
    }

    public static void ReplaceCalendarData(CalendarData calendarData)
    {
        _calendarData = calendarData;
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
}