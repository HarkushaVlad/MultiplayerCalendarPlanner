using System.Reflection;
using StardewValley.Menus;

namespace MultiplayerCalendarPlanner.Utils
{
    public static class BillboardUtils
    {
        /// <summary>
        /// Checks if the provided Billboard instance is a Calendar (not a Daily Quest Board).
        /// </summary>
        /// <param name="billboard">Instance of the Billboard.</param>
        /// <returns>True if the Billboard is a Calendar; otherwise, false.</returns>
        public static bool IsCalendar(Billboard billboard)
        {
            if (billboard == null)
                throw new ArgumentNullException(nameof(billboard));

            var fieldInfo =
                typeof(Billboard).GetField("dailyQuestBoard", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
                return false;

            var value = fieldInfo.GetValue(billboard);
            return value is bool isDailyQuestBoard && !isDailyQuestBoard;
        }
    }
}