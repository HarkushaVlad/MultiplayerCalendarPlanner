using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace MultiplayerCalendarPlanner.UI;

public class ActivityChooseDialogBox : DialogueBox
{
    public ActivityChooseDialogBox(string dialogue, Response[] responses) : base(dialogue, responses)
    {
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
        {
            var responseKey = responses[selectedResponse].responseKey;
            Game1.chatBox.addMessage($"Selected response: {responseKey}", Color.White);
        }

        CustomCloseDialogue();
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