using MultiplayerCalendarPlanner.Data;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MultiplayerCalendarPlanner.Sync;

public static class MultiplayerManager
{
    private const string AddFarmHandEventToAllMessage = "AddFarmHandEventToAll";
    private const string AddHostEventToFarmHandsMessage = "AddHostEventToFarmHands";
    private const string AddHostCalendarDataToFarmHandsMessage = "AddHostCalendarDataToFarmHands";

    public static void AddFarmHandEventToAll(CalendarEvent calendarEvent)
    {
        if (Context.IsMainPlayer)
        {
            ModEntry.StaticMonitor.Log(
                "Attempted to send 'AddFarmHandEventToAll' from the main player. This operation is ignored.",
                LogLevel.Warn
            );
            return;
        }

        ModEntry.StaticHelper.Multiplayer.SendMessage(calendarEvent, AddFarmHandEventToAllMessage,
            new[] { ModEntry.StaticHelper.ModRegistry.ModID });
    }

    public static void AddHostEventToFarmHands(CalendarEvent calendarEvent)
    {
        if (!Context.IsMainPlayer)
        {
            ModEntry.StaticMonitor.Log(
                "Attempted to send 'AddHostEventToFarmHands' from a farmhand. This operation is ignored.",
                LogLevel.Warn
            );
            return;
        }

        ModEntry.StaticHelper.Multiplayer.SendMessage(calendarEvent, AddHostEventToFarmHandsMessage,
            new[] { ModEntry.StaticHelper.ModRegistry.ModID });
    }

    public static void AddHostCalendarDataToFarmHands(CalendarData calendarData)
    {
        if (!Context.IsMainPlayer)
        {
            ModEntry.StaticMonitor.Log(
                "Attempted to send 'AddHostCalendarDataToFarmHands' from a farmhand. This operation is ignored.",
                LogLevel.Warn
            );
            return;
        }

        ModEntry.StaticHelper.Multiplayer.SendMessage(calendarData, AddHostCalendarDataToFarmHandsMessage,
            new[] { ModEntry.StaticHelper.ModRegistry.ModID });
    }

    public static void HandleReceivedMessage(ModMessageReceivedEventArgs e)
    {
        switch (e.Type)
        {
            case AddFarmHandEventToAllMessage:
                var farmHandEvent = e.ReadAs<CalendarEvent>();
                CalendarManager.AddEvent(farmHandEvent);

                if (Context.IsMainPlayer)
                    CalendarManager.SaveData();
                break;

            case AddHostEventToFarmHandsMessage:
                if (Context.IsMainPlayer)
                    break;

                var hostEvent = e.ReadAs<CalendarEvent>();
                CalendarManager.AddEvent(hostEvent);
                break;

            case AddHostCalendarDataToFarmHandsMessage:
                if (Context.IsMainPlayer)
                    break;

                var hostCalendarData = e.ReadAs<CalendarData>();
                CalendarManager.ReplaceCalendarData(hostCalendarData);
                break;

            default:
                ModEntry.StaticMonitor.Log($"Unknown message type received: {e.Type}", LogLevel.Warn);
                break;
        }
    }
}