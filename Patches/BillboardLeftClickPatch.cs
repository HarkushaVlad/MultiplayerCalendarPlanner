using HarmonyLib;
using MultiplayerCalendarPlanner.Constants;
using MultiplayerCalendarPlanner.Data;
using MultiplayerCalendarPlanner.UI;
using MultiplayerCalendarPlanner.Utils;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerCalendarPlanner.Patches
{
    internal class BillboardLeftClickPatch
    {
        public static void ApplyPatch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), "receiveLeftClick",
                    new[] { typeof(int), typeof(int), typeof(bool) }),
                prefix: new HarmonyMethod(typeof(BillboardLeftClickPatch), nameof(Prefix))
            );
        }

        public static bool Prefix(Billboard __instance, int x, int y, bool playSound)
        {
            if (!BillboardUtils.IsCalendar(__instance))
                return true;

            List<ClickableTextureComponent> calendarDays = __instance.calendarDays;

            foreach (var day in calendarDays)
            {
                if (!day.bounds.Contains(x, y) || day.myID < Game1.dayOfMonth)
                    continue;

                var eventsForDay = CalendarManager.GetEventsForDay(day.myID, Game1.season);

                var isRobinReserved = IsReserved(Activity.Robin, eventsForDay);
                var isHardWoodReserved = IsReserved(Activity.HardWood, eventsForDay);

                var isRobinReservedByPlayer = IsReservedByPlayer(Activity.Robin, eventsForDay);
                var isHardWoodReservedByPlayer = IsReservedByPlayer(Activity.HardWood, eventsForDay);

                var responses = new List<Response>();

                if (isRobinReservedByPlayer)
                {
                    responses.Add(new Response(
                        MenuResponse.UnreserveRobin.ToString(),
                        ModEntry.StaticHelper.Translation.Get("event.unreserveRobin")
                    ));
                }
                else if (!isRobinReserved)
                {
                    responses.Add(new Response(
                        MenuResponse.ReserveRobin.ToString(),
                        ModEntry.StaticHelper.Translation.Get("event.reserveRobin")
                    ));
                }

                if (isHardWoodReservedByPlayer)
                {
                    responses.Add(new Response(
                        MenuResponse.UnreserveHardWood.ToString(),
                        ModEntry.StaticHelper.Translation.Get("event.unreserveSecretWoods")
                    ));
                }
                else if (!isHardWoodReserved)
                {
                    responses.Add(new Response(
                        MenuResponse.ReserveHardWood.ToString(),
                        ModEntry.StaticHelper.Translation.Get("event.reserveSecretWoods")
                    ));
                }


                if (responses.Count == 0)
                    continue;

                __instance.SetChildMenu(new ActivityChooseDialogBox
                (
                    ModEntry.StaticHelper.Translation.Get("event.reserve", new { day = day.myID }),
                    responses.ToArray(),
                    day.myID
                ));

                Game1.dialogueUp = true;

                return false;
            }

            return true;
        }

        private static bool IsReserved(Activity activity, List<CalendarEvent> eventsForDay)
        {
            return eventsForDay.Any(e => e.Activity == activity);
        }

        private static bool IsReservedByPlayer(Activity activity, List<CalendarEvent> eventsForDay)
        {
            return eventsForDay.Any(e => e.Activity == activity && e.PlayerId == Game1.player.UniqueMultiplayerID);
        }
    }
}