using System.Collections;
using RagnarEngine;

[System.Serializable]
public class DialogueData
{
    public bool used;

    public DialogueData(DialogueTrigger trigger)
    {
        used = trigger.GetUsed();
    }
}