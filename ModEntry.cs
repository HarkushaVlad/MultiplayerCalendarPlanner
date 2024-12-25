using System.Reflection;
using MultiplayerCalendarPlanner.Data;
using MultiplayerCalendarPlanner.Sync;
using MultiplayerCalendarPlanner.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerCalendarPlanner
{
    internal sealed class ModEntry : Mod
    {
        public static IModHelper StaticHelper = null!;
        private static bool _calendarOpened;

        public override void Entry(IModHelper helper)
        {
            StaticHelper = helper;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        }

        private void OnSaveLoaded(object? sender, EventArgs e)
        {
            CalendarManager.LoadData();
        }

        private void OnSaving(object? sender, EventArgs e)
        {
            CalendarManager.SaveData();
        }

        private void OnMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID)
                return;

            if (Context.IsMainPlayer)
                MultiplayerManager.HandleReceivedMessage(e);
        }

        private static void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (_calendarOpened && Game1.activeClickableMenu == null)
                _calendarOpened = false;
        }

        private static void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (Game1.activeClickableMenu is not Billboard || _calendarOpened)
                return;

            var fieldInfo =
                typeof(Billboard).GetField("dailyQuestBoard", BindingFlags.NonPublic | BindingFlags.Instance);

            var value = fieldInfo?.GetValue(Game1.activeClickableMenu);

            if (value is null)
                return;

            var isDailyQuestBoard = (bool)value;

            if (isDailyQuestBoard)
                return;

            CalendarUI.DrawCalendarOverlay();
            _calendarOpened = true;
        }
    }
}