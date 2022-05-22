using System;
using RagnarEngine;

public class DialogueTrigger : RagnarComponent
{
    // Components
    private DialogueManager dialogueManager;
    ///////////

    GameObject manager;
    public int dialogueId;
    public bool isUsed = false;
    public bool nexLevel = false;
	public void Start()
	{
        manager = GameObject.Find("Dialogue");
        dialogueManager = manager.GetComponent<DialogueManager>();
    }
    public void Update()
	{
        if (nexLevel == true) return;
        switch (dialogueId)
        {
            case 0:
                if (isUsed)
                    if (dialogueManager.GetEndDialogue())
                    {
                        isUsed = false;
                        ActiveDialoguebyID(2);
                    }
                    break;
            case 1:
                // Gameplay
                break;
            case 2:
                // Gameplay
                break;

            case 3:
                if (isUsed)
                    if (dialogueManager.GetEndDialogue())
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
                if (dialogueManager.GetEndDialogue())
                {
                    isUsed = false;
                    ActiveDialoguebyID(7);
                }
                break;

            case 7:
                if (dialogueManager.GetEndDialogue())
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

            case 11:
                // Gameplay
                break;

            case 12:
                // Gameplay
                break;

            default:
                break;
        }
       
	}

    void LoadSceneWin()
    {
        if (dialogueManager.GetEndDialogue())
        {
            nexLevel = true;
            isUsed = false;
            GameObject.Find("EnemyManager").GetComponent<EnemyManager>().SaveTest("WIIIIIN", gameObject.transform.globalPosition);
            SceneManager.LoadScene("WinScene");
        }
    }
    public void ActiveDialogue()
    {
        if (!isUsed)
        {
            isUsed = true;
            dialogueManager.StartNewDialogue(dialogueId);
        }
    }

    public void ActiveDialoguebyID(int id)
    {
        if (!isUsed)
        {
            isUsed = true;
            dialogueId = id;
            dialogueManager.StartNewDialogue(dialogueId);
        }
    }
    public bool GetUsed() { return isUsed; }
    public void SetUsed(bool used) { isUsed = used; }
}