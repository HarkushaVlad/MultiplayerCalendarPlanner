using HarmonyLib;
using Microsoft.Xna.Framework;
using MultiplayerCalendarPlanner.Constants;
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
                if (!day.bounds.Contains(x, y))
                    continue;
                
                __instance.SetChildMenu(new ActivityChooseDialogBox("",
                    new[]
                    {
                        new Response(Activity.Robin.ToString(), "Reserve Robin for a project"),
                        new Response(Activity.HardWood.ToString(), "Reserve the Secret Woods for hardwood gathering")
                    }));
                
                Game1.dialogueUp = true;

                return false;
            }

            return true;
        }
    }
}