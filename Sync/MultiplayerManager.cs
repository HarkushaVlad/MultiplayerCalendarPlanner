using MultiplayerCalendarPlanner.Constants;
using MultiplayerCalendarPlanner.Data;
using StardewModdingAPI.Events;

namespace MultiplayerCalendarPlanner.Sync;

public static class MultiplayerManager
{
    public static void SendEventToHost(CalendarEvent calendarEvent)
    {
        ModEntry.StaticHelper.Multiplayer.SendMessage(calendarEvent, ModMessageType.AddEvent.ToString(),
            new[] { ModEntry.StaticHelper.ModRegistry.ModID });
    }

    public static void HandleReceivedMessage(ModMessageReceivedEventArgs e)
    {
        if (e.Type != ModMessageType.AddEvent.ToString())
            return;

        var calendarEvent = e.ReadAs<CalendarEvent>();
        CalendarManager.AddEvent(calendarEvent);
    }
}