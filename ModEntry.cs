using HarmonyLib;
using MultiplayerCalendarPlanner.Data;
using MultiplayerCalendarPlanner.Patches;
using MultiplayerCalendarPlanner.Sync;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MultiplayerCalendarPlanner
{
    internal sealed class ModEntry : Mod
    {
        public static IModHelper StaticHelper = null!;
        public static IMonitor StaticMonitor = null!;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            BillboardDrawPatch.ApplyPatch(harmony);
            BillboardLeftClickPatch.ApplyPatch(harmony);

            StaticHelper = helper;
            StaticMonitor = Monitor;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
        }

        private void OnSaveLoaded(object? sender, EventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            if (Game1.dayOfMonth == 1)
            {
                CalendarManager.ClearData();
            }
            else
            {
                CalendarManager.LoadData();
            }
        }

        private void OnSaving(object? sender, EventArgs e)
        {
            if (Context.IsMainPlayer)
                CalendarManager.SaveData();
        }

        private void OnMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != ModManifest.UniqueID)
                return;

            MultiplayerManager.HandleReceivedMessage(e);
        }

        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            MultiplayerManager.AddHostCalendarDataToFarmHands(CalendarManager.GetCalendarData());
        }
    }
}