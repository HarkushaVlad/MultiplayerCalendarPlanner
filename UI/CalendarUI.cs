using Microsoft.Xna.Framework;
using StardewValley;

namespace MultiplayerCalendarPlanner.UI;

public static class CalendarUI
{
    public static void DrawCalendarOverlay()
    {
        Game1.chatBox.addMessage("Open calendar overlay", Color.Blue);
    }

    public static void OpenEventEditor(int day, string season)
    {
        Game1.chatBox.addMessage($"Open event editor {day} {season}", Color.Aqua);
    }
}