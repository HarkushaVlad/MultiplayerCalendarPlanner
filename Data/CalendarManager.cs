using StardewModdingAPI;
using StardewValley;

namespace MultiplayerCalendarPlanner.Data;

public static class CalendarManager
{
    private static CalendarData _calendarData = new();

    public static void LoadData()
    {
        _calendarData = ModEntry.StaticHelper.Data.ReadSaveData<CalendarData>("calendar-data") ?? new CalendarData();
    }

    public static void SaveData()
    {
        ModEntry.StaticHelper.Data.WriteSaveData("calendar-data", _calendarData);
    }

    public static void AddEvent(CalendarEvent calendarEvent)
    {
        _calendarData.Events.Add(calendarEvent);
    }

    public static List<CalendarEvent> GetEventsForDay(int day, Season season)
    {
        return _calendarData.Events.Where(e => e.Day == day && e.Season == season).ToList();
    }
}