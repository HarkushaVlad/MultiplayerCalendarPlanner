using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerCalendarPlanner.Constants;
using MultiplayerCalendarPlanner.Data;
using MultiplayerCalendarPlanner.Utils;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerCalendarPlanner.Patches
{
    internal class BillboardDrawPatch
    {
        private static string _originalHoverText = string.Empty;

        public static void ApplyPatch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(
                    typeof(Billboard),
                    "draw",
                    new[] { typeof(SpriteBatch) }
                ),
                prefix: new HarmonyMethod(typeof(BillboardDrawPatch), nameof(Prefix)),
                postfix: new HarmonyMethod(typeof(BillboardDrawPatch), nameof(Postfix))
            );
        }

        public static void Prefix(Billboard __instance, ref bool __state)
        {
            var hoverTextField =
                typeof(Billboard).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance);
            if (hoverTextField == null)
                return;

            _originalHoverText = hoverTextField.GetValue(__instance) as string ?? string.Empty;
            hoverTextField.SetValue(__instance, string.Empty);
        }

        public static void Postfix(Billboard __instance, SpriteBatch b)
        {
            if (!BillboardUtils.IsCalendar(__instance))
                return;

            List<ClickableTextureComponent> calendarDays = __instance.calendarDays;

            var x = Game1.getMouseX();
            var y = Game1.getMouseY();

            var customHoverText = "";

            foreach (var day in calendarDays)
            {
                var season = (Season)Enum.Parse(typeof(Season), Game1.currentSeason, true);
                var dayEvents = CalendarManager.GetEventsForDay(day.myID, season);

                foreach (var calendarEvent in dayEvents)
                {
                    var iconPosition = new Vector2(
                        day.bounds.X + Game1.tileSize / 4,
                        day.bounds.Y + Game1.tileSize / 4
                    );

                    var activityIcon = calendarEvent.Activity switch
                    {
                        Activity.Robin => IconUtils.GetHammerTexture(
                            day.bounds.X + Game1.tileSize - Game1.tileSize / 32,
                            day.bounds.Y + Game1.tileSize / 10
                        ),
                        Activity.HardWood => IconUtils.GetHardWoodTexture(
                            day.bounds.X + Game1.tileSize + Game1.tileSize / 2,
                            day.bounds.Y + Game1.tileSize / 10
                        ),
                        _ => null
                    };

                    if (activityIcon == null)
                        return;

                    activityIcon.draw(b);

                    var farmerPortraitPosition = new Vector2(
                        iconPosition.X + Game1.tileSize / 2,
                        iconPosition.Y
                    );

                    var farmer = Game1.GetPlayer(calendarEvent.PlayerId);

                    farmer?.FarmerRenderer.drawMiniPortrat(
                        b,
                        farmerPortraitPosition,
                        layerDepth: 0.91f,
                        scale: 1.5f,
                        facingDirection: 2,
                        who: farmer
                    );

                    if (activityIcon.containsPoint(x, y))
                    {
                        var playerName = farmer?.Name;

                        customHoverText = calendarEvent.Activity switch
                        {
                            Activity.Robin => ModEntry.StaticHelper.Translation.Get(
                                "event.reserveRobin",
                                new { playerName }
                            ),
                            Activity.HardWood => ModEntry.StaticHelper.Translation.Get(
                                "event.reserveSecretWoods",
                                new { playerName }
                            ),
                            _ => ModEntry.StaticHelper.Translation.Get(
                                "event.unknown",
                                new { playerName }
                            )
                        };
                    }
                }
            }

            if (!string.IsNullOrEmpty(customHoverText))
            {
                IClickableMenu.drawHoverText(b, customHoverText, Game1.dialogueFont);
            }

            if (!string.IsNullOrEmpty(_originalHoverText) && string.IsNullOrEmpty(customHoverText))
            {
                IClickableMenu.drawHoverText(b, _originalHoverText, Game1.dialogueFont);
            }

            __instance.drawMouse(b);
        }
    }
}