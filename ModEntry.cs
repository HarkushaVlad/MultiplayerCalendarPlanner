using HarmonyLib;
using MultiplayerCalendarPlanner.Data;
using MultiplayerCalendarPlanner.Patches;
using MultiplayerCalendarPlanner.Sync;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MultiplayerCalendarPlanner
{
    internal sealed class ModEntry : Mod
    {
        public static IModHelper StaticHelper = null!;

        public override void Entry(IModHelper helper)
        {
            var harmony = new Harmony(ModManifest.UniqueID);
            BillboardDrawPatch.ApplyPatch(harmony);
            BillboardLeftClickPatch.ApplyPatch(harmony);

            StaticHelper = helper;

            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.Multiplayer.ModMessageReceived += OnMessageReceived;
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
    }
}