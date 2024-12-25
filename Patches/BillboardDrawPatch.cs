using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            var fieldInfo =
                typeof(Billboard).GetField("dailyQuestBoard", BindingFlags.NonPublic | BindingFlags.Instance);

            var value = fieldInfo?.GetValue(__instance);

            if (value is null)
                return;

            var isDailyQuestBoard = (bool)value;

            if (isDailyQuestBoard)
                return;

            List<ClickableTextureComponent> calendarDays = __instance.calendarDays;

            var x = Game1.getMouseX();
            var y = Game1.getMouseY();

            var customHoverText = "";

            foreach (var day in calendarDays)
            {
                var hammerIcon = new ClickableTextureComponent(
                    new Rectangle(
                        day.bounds.X + Game1.tileSize - Game1.tileSize / 32,
                        day.bounds.Y + Game1.tileSize / 10,
                        Game1.tileSize / 2,
                        Game1.tileSize / 2
                    ),
                    Game1.content.Load<Texture2D>("LooseSprites/Cursors"),
                    new Rectangle(366, 373, 16, 16),
                    2f
                );

                hammerIcon.draw(b);

                if (hammerIcon.containsPoint(x, y))
                {
                    customHoverText =
                        ModEntry.StaticHelper.Translation.Get("event.reserveRobin", new { playerName = Game1.player.Name });
                }

                var farmer = Game1.player;
                var farmerPortrait = farmer.FarmerRenderer;

                var portraitPosition = new Vector2(
                    hammerIcon.bounds.X + Game1.tileSize / 8,
                    hammerIcon.bounds.Y + Game1.tileSize / 6
                );

                farmerPortrait.drawMiniPortrat(
                    b,
                    portraitPosition,
                    layerDepth: 0.89f,
                    scale: 1.8f,
                    facingDirection: 2,
                    who: farmer
                );

                var hardWoodIcon = new ClickableTextureComponent(
                    new Rectangle(
                        day.bounds.X + Game1.tileSize + Game1.tileSize / 2,
                        day.bounds.Y + Game1.tileSize / 10,
                        Game1.tileSize / 2,
                        Game1.tileSize / 2
                    ),
                    Game1.content.Load<Texture2D>("Maps/springobjects"),
                    new Rectangle(210, 466, 13, 14),
                    2f
                );

                hardWoodIcon.draw(b);

                if (hardWoodIcon.containsPoint(x, y))
                {
                    customHoverText =
                        ModEntry.StaticHelper.Translation.Get("event.reserveSecretWoods", new { playerName = Game1.player.Name });
                }

                var portraitPositionHardwood = new Vector2(
                    hardWoodIcon.bounds.X + Game1.tileSize / 32,
                    hardWoodIcon.bounds.Y + Game1.tileSize / 6
                );

                farmerPortrait.drawMiniPortrat(
                    b,
                    portraitPositionHardwood,
                    layerDepth: 0.89f,
                    scale: 1.8f,
                    facingDirection: 2,
                    who: farmer
                );
            }

            if (!string.IsNullOrEmpty(customHoverText))
            {
                IClickableMenu.drawHoverText(b, customHoverText, Game1.dialogueFont);
            }

            if (
                !string.IsNullOrEmpty(_originalHoverText) && string.IsNullOrEmpty(customHoverText)
            )
            {
                IClickableMenu.drawHoverText(b, _originalHoverText, Game1.dialogueFont);
            }

            __instance.drawMouse(b);
        }
    }
}