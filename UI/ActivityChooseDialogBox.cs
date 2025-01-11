using Microsoft.Xna.Framework.Input;
using MultiplayerCalendarPlanner.Constants;
using MultiplayerCalendarPlanner.Data;
using MultiplayerCalendarPlanner.Sync;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerCalendarPlanner.UI;

public class ActivityChooseDialogBox : DialogueBox
{
    private readonly int _selectedDay;

    public ActivityChooseDialogBox(string dialogue, Response[] responses, int selectedDay) : base(dialogue, responses)
    {
        _selectedDay = selectedDay;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        for (var i = 0; i < responseCC.Count; i++)
        {
            if (!responseCC[i].containsPoint(x, y))
                continue;

            selectedResponse = i;
            break;
        }

        if (selectedResponse != -1)
            HandleSelectedResponse();

        CustomCloseDialogue();
    }

    private void HandleSelectedResponse()
    {
        if (selectedResponse == -1)
            return;

        var isReserveRobin = responses[selectedResponse].responseKey == MenuResponse.ReserveRobin.ToString();
        var isReserveHardWood = responses[selectedResponse].responseKey == MenuResponse.ReserveHardWood.ToString();

        var isUnreserveRobin = responses[selectedResponse].responseKey == MenuResponse.UnreserveRobin.ToString();
        var isUnreserveHardWood = responses[selectedResponse].responseKey == MenuResponse.UnreserveHardWood.ToString();

        if (isReserveRobin || isReserveHardWood)
        {
            var season = (Season)Enum.Parse(typeof(Season), Game1.currentSeason, true);
            var playerId = Game1.player.UniqueMultiplayerID;

            var calendarEvent = new CalendarEvent(_selectedDay, season, playerId,
                isReserveRobin ? Activity.Robin : Activity.HardWood);

            CalendarManager.AddEvent(calendarEvent);

            if (Context.IsMainPlayer)
            {
                CalendarManager.SaveData();
                MultiplayerManager.AddHostEventToFarmHands(calendarEvent);
            }
            else
            {
                MultiplayerManager.AddFarmHandEventToAll(calendarEvent);
            }

            return;
        }

        if (isUnreserveRobin || isUnreserveHardWood)
        {
            var deleteEvent = CalendarManager.FindEvent(_selectedDay, Game1.season,
                Game1.player.UniqueMultiplayerID, isUnreserveRobin ? Activity.Robin : Activity.HardWood);

            if (deleteEvent == null)
                return;

            var isDeleted = CalendarManager.RemoveEvent(deleteEvent);

            if (!isDeleted)
                return;

            if (Context.IsMainPlayer)
            {
                CalendarManager.SaveData();
                MultiplayerManager.RemoveHostEventFromFarmHands(deleteEvent);
            }
            else
            {
                MultiplayerManager.RemoveFarmHandEventFromAll(deleteEvent);
            }
        }
    }

    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        receiveLeftClick(x, y, playSound);
    }

    public override void receiveKeyPress(Keys key)
    {
        base.receiveKeyPress(key);

        if (key is Keys.Escape or Keys.E)
            CustomCloseDialogue();
    }

    private void CustomCloseDialogue()
    {
        closeDialogue();
        exitThisMenuNoSound();

        Game1.dialogueUp = false;
        Game1.eventUp = false;
        Game1.player.canMove = true;

        responses = Array.Empty<Response>();
        characterDialoguesBrokenUp.Clear();
        dialogues.Clear();

        dialogueIcon = null;
        aboveDialogueImage = null;

        safetyTimer = 0;
        characterAdvanceTimer = 0;
        newPortaitShakeTimer = 0;

        transitioning = false;
        transitioningBigger = false;

        Game1.activeClickableMenu.SetChildMenu(null);
    }
}