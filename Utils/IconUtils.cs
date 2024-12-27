using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerCalendarPlanner.Utils
{
    public static class IconUtils
    {
        /// <summary>
        /// Creates a hammer icon at the given position with the default scale of 2f.
        /// </summary>
        /// <param name="x">The X position for the icon.</param>
        /// <param name="y">The Y position for the icon.</param>
        /// <param name="scale">The scale of the icon, default is 2f.</param>
        /// <returns>A ClickableTextureComponent for the hammer icon.</returns>
        public static ClickableTextureComponent GetHammerTexture(int x, int y, float scale = 2f)
        {
            var texture = Game1.content.Load<Texture2D>("LooseSprites/Cursors");
            var sourceRect = new Rectangle(366, 373, 16, 16);

            return new ClickableTextureComponent(
                new Rectangle(x, y, Game1.tileSize / 2, Game1.tileSize / 2),
                texture,
                sourceRect,
                scale
            );
        }

        /// <summary>
        /// Creates a hardwood icon at the given position with the default scale of 2f.
        /// </summary>
        /// <param name="x">The X position for the icon.</param>
        /// <param name="y">The Y position for the icon.</param>
        /// <param name="scale">The scale of the icon, default is 2f.</param>
        /// <returns>A ClickableTextureComponent for the hardwood icon.</returns>
        public static ClickableTextureComponent GetHardWoodTexture(int x, int y, float scale = 2f)
        {
            var texture = Game1.content.Load<Texture2D>("Maps/springobjects");
            var sourceRect = new Rectangle(210, 466, 13, 14);

            return new ClickableTextureComponent(
                new Rectangle(x, y, Game1.tileSize / 2, Game1.tileSize / 2),
                texture,
                sourceRect,
                scale
            );
        }
    }
}