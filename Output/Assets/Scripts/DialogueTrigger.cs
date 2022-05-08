using System;
using RagnarEngine;

public class DialogueTrigger : RagnarComponent
{
    GameObject manager;
    public int dialogueId;
    bool isUsed;
	public void Start()
	{
        manager = GameObject.Find("Dialogue");
        isUsed = false;
    }
    public void Update()
	{
        switch (dialogueId)
        {
            case 0:
                if (isUsed)
                    if (manager.GetComponent<DialogueManager>().GetEndDialogue())
                    {
                        isUsed = false;
                        ActiveDialoguebyID(2);
                    }
                    break;
            case 1:
                if (isUsed)
                    LoadSceneWin();
                break;
            case 2:
                // Gameplay
                break;

            case 3:
                if (isUsed)
                    if (manager.GetComponent<DialogueManager>().GetEndDialogue())
                    {
                        isUsed = false;
                        ActiveDialoguebyID(4);
                    }
                break;

            case 4:
                if (isUsed)
                    LoadSceneWin();
                break;

            case 5:
                // Gameplay
                break;

            case 6:
                if (manager.GetComponent<DialogueManager>().GetEndDialogue())
                {
                    isUsed = false;
                    ActiveDialoguebyID(7);
                }
                break;

            case 7:
                if (manager.GetComponent<DialogueManager>().GetEndDialogue())
                {
                    isUsed = false;
                    ActiveDialoguebyID(8);
                }
                break;

            case 8:
                // Gameplay
                break;

            case 9:
                // Gameplay
                break;

            case 10:
                if (isUsed)
                    LoadSceneWin();
                break;
            default:
                break;
        }
       
	}

    void LoadSceneWin()
    {
        if (manager.GetComponent<DialogueManager>().GetEndDialogue())
            SceneManager.LoadScene("WinScene");
    }
    public void ActiveDialogue()
    {
        if (!isUsed)
        {
            isUsed = true;
            manager.GetComponent<DialogueManager>().StartNewDialogue(dialogueId);
        }
    }

    public void ActiveDialoguebyID(int id)
    {
        if (!isUsed)
        {
            isUsed = true;
            dialogueId = id;
            manager.GetComponent<DialogueManager>().StartNewDialogue(dialogueId);
        }
    }
}