using HarmonyLib;
using MultiplayerCalendarPlanner.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerCalendarPlanner.Patches
{
    internal class CarpenterMenuTryToBuildPatch
    {
        public static void ApplyPatch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CarpenterMenu), "tryToBuild"),
                postfix: new HarmonyMethod(typeof(CarpenterMenuTryToBuildPatch), nameof(Postfix))
            );
        }

        public static void Postfix(CarpenterMenu __instance, bool __result)
        {
            if (!__result)
                return;

            var buildDays = __instance.Blueprint.BuildDays;
            CalendarManager.SetBuildingActivities(buildDays, Game1.player.UniqueMultiplayerID);
        }
    }
}