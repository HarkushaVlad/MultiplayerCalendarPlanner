using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MultiplayerCalendarPlanner.Constants;
using MultiplayerCalendarPlanner.Data;
using MultiplayerCalendarPlanner.Utils;
using StardewModdingAPI;
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
            if (!Context.IsMultiplayer)
                return;

            var hoverTextField =
                typeof(Billboard).GetField("hoverText", BindingFlags.NonPublic | BindingFlags.Instance);
            if (hoverTextField == null)
                return;

            _originalHoverText = hoverTextField.GetValue(__instance) as string ?? string.Empty;
            hoverTextField.SetValue(__instance, string.Empty);
        }

        public static void Postfix(Billboard __instance, SpriteBatch b)
        {
            if (!Context.IsMultiplayer)
                return;

            if (!BillboardUtils.IsCalendar(__instance))
                return;

            List<ClickableTextureComponent> calendarDays = __instance.calendarDays;

            var x = Game1.getMouseX();
            var y = Game1.getMouseY();

            var customHoverText = "";

            const int rightActivityIconOffset = -Game1.tileSize / 2 + 6;
            const int leftActivityIconOffset = Game1.tileSize / 32 + 3;

            foreach (var day in calendarDays)
            {
                var season = (Season)Enum.Parse(typeof(Season), Game1.currentSeason, true);
                var dayEvents = CalendarManager.GetEventsForDay(day.myID, season);

                var iconOffset = rightActivityIconOffset;

                foreach (var calendarEvent in dayEvents)
                {
                    var iconX = day.bounds.X + Game1.tileSize - iconOffset;
                    var iconY = day.bounds.Y + Game1.tileSize / 10 - 2;

                    var alpha = day.myID < Game1.dayOfMonth ? 0.7f : 1f;

                    var activityIcon = calendarEvent.Activity switch
                    {
                        Activity.Robin => IconUtils.GetHammerTexture(iconX, iconY),
                        Activity.HardWood => IconUtils.GetHardWoodTexture(iconX, iconY),
                        _ => null
                    };

                    if (activityIcon == null)
                        continue;

                    activityIcon.draw(
                        b,
                        Color.White * alpha,
                        layerDepth: 0.91f
                    );

                    var farmerPortraitPosition = new Vector2(
                        iconX,
                        iconY + Game1.tileSize / 4
                    );

                    var farmer = Game1.GetPlayer(calendarEvent.PlayerId);

                    farmer?.FarmerRenderer.drawMiniPortrat(
                        b,
                        farmerPortraitPosition,
                        layerDepth: 0.91f,
                        scale: 1.5f,
                        facingDirection: 2,
                        who: farmer,
                        alpha: alpha
                    );

                    iconOffset = leftActivityIconOffset;

                    if (activityIcon.containsPoint(x, y))
                    {
                        var playerName = farmer != null
                            ? farmer.Name
                            : ModEntry.StaticHelper.Translation.Get(
                                "string.Player");

                        customHoverText = calendarEvent.Activity switch
                        {
                            Activity.Robin => ModEntry.StaticHelper.Translation.Get(
                                "event.hover.reserveRobin",
                                new { playerName }
                            ),
                            Activity.HardWood => ModEntry.StaticHelper.Translation.Get(
                                "event.hover.reserveSecretWoods",
                                new { playerName }
                            ),
                            _ => ModEntry.StaticHelper.Translation.Get(
                                "event.hover.unknown",
                                new { playerName }
                            )
                        };
                    }
                }

                // Draws a blue outline around the current day on the calendar
                if (Game1.dayOfMonth == day.myID)
                {
                    var num = (int)(4.0 * Game1.dialogueButtonScale / 8.0);
                    IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(379, 357, 3, 3),
                        day.bounds.X - num, day.bounds.Y - num, day.bounds.Width + num * 2, day.bounds.Height + num * 2,
                        Color.Blue, 4f, false);
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