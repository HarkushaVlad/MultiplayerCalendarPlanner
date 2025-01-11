using MultiplayerCalendarPlanner.Constants;
using MultiplayerCalendarPlanner.Data;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MultiplayerCalendarPlanner.Sync;

public static class MultiplayerManager
{
    private static bool IsAllowedToSend(bool isMainPlayerRequired, string actionName)
    {
        if (Context.IsMainPlayer != isMainPlayerRequired)
        {
            ModEntry.StaticMonitor.Log(
                $"Attempted to send '{actionName}' from an invalid context. This operation is ignored.",
                LogLevel.Warn
            );
            return false;
        }

        return true;
    }

    private static void SendMessage<T>(T messageData, MultiplayerMessage messageType)
    {
        ModEntry.StaticHelper.Multiplayer.SendMessage(
            messageData,
            messageType.ToString(),
            new[] { ModEntry.StaticHelper.ModRegistry.ModID }
        );
    }

    public static void AddFarmHandEventToAll(CalendarEvent calendarEvent)
    {
        if (!IsAllowedToSend(isMainPlayerRequired: false, "AddFarmHandEventToAll"))
            return;

        SendMessage(calendarEvent, MultiplayerMessage.AddFarmHandEventToAll);
    }

    public static void RemoveFarmHandEventFromAll(CalendarEvent calendarEvent)
    {
        if (!IsAllowedToSend(isMainPlayerRequired: false, "RemoveFarmHandEventFromAll"))
            return;

        SendMessage(calendarEvent, MultiplayerMessage.RemoveFarmHandEventFromAll);
    }

    public static void AddHostEventToFarmHands(CalendarEvent calendarEvent)
    {
        if (!IsAllowedToSend(isMainPlayerRequired: true, "AddHostEventToFarmHands"))
            return;

        SendMessage(calendarEvent, MultiplayerMessage.AddHostEventToFarmHands);
    }

    public static void RemoveHostEventFromFarmHands(CalendarEvent calendarEvent)
    {
        if (!IsAllowedToSend(isMainPlayerRequired: true, "RemoveHostEventFromFarmHands"))
            return;

        SendMessage(calendarEvent, MultiplayerMessage.RemoveHostEventFromFarmHands);
    }

    public static void AddHostCalendarDataToFarmHands(CalendarData calendarData)
    {
        if (!IsAllowedToSend(isMainPlayerRequired: true, "AddHostCalendarDataToFarmHands"))
            return;

        SendMessage(calendarData, MultiplayerMessage.AddHostCalendarDataToFarmHands);
    }

    public static void HandleReceivedMessage(ModMessageReceivedEventArgs e)
    {
        if (!Enum.TryParse(e.Type, out MultiplayerMessage messageType))
        {
            ModEntry.StaticMonitor.Log($"Unknown message type received: {e.Type}", LogLevel.Warn);
            return;
        }

        switch (messageType)
        {
            case MultiplayerMessage.AddFarmHandEventToAll:
                var farmHandEvent = e.ReadAs<CalendarEvent>();
                CalendarManager.AddEvent(farmHandEvent);

                if (Context.IsMainPlayer)
                    CalendarManager.SaveData();
                break;

            case MultiplayerMessage.RemoveFarmHandEventFromAll:
                var farmHandRemoveEvent = e.ReadAs<CalendarEvent>();
                CalendarManager.RemoveEvent(farmHandRemoveEvent);

                if (Context.IsMainPlayer)
                    CalendarManager.SaveData();
                break;

            case MultiplayerMessage.AddHostEventToFarmHands:
                if (Context.IsMainPlayer)
                    break;

                var hostEvent = e.ReadAs<CalendarEvent>();
                CalendarManager.AddEvent(hostEvent);
                break;

            case MultiplayerMessage.RemoveHostEventFromFarmHands:
                if (Context.IsMainPlayer)
                    break;

                var hostRemoveEvent = e.ReadAs<CalendarEvent>();
                CalendarManager.RemoveEvent(hostRemoveEvent);
                break;

            case MultiplayerMessage.AddHostCalendarDataToFarmHands:
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