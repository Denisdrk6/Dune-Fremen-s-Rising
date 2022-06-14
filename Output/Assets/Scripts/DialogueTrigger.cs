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
                
                break;
            case 1:
                // Gameplay
                
                break;
            case 2:
                // Gameplay
                if (isUsed)
                    LoadSceneWin();
                break;
            case 3:
                
                break;
            case 4:

                break;
            case 5:
                // Gameplay
                if (isUsed)
                    LoadSceneWin();
                break;
            case 6:
             
                break;
            case 7:
               
                break;
            case 8:
                // Gameplay
                
                break;
            case 9:
                // Gameplay
                if (isUsed)
                    LoadSceneWin();
                break;
            case 10:

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
            //GameObject.Find("EnemyManager").GetComponent<EnemyManager>().SaveTest("WIIIIIN", gameObject.transform.globalPosition);
            Input.RestoreDefaultCursor();

            GameObject.Find("Quest System").GetComponent<QuestSystem>().SaveMissions();
            Debug.Log("Holaaaaa");
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



/* ESTO DEBE ESTAR EN EL CÓDIGO DEL BOSS CUANDO MUERA
   GameObject.Find("Quest System").GetComponent<QuestSystem>().levelFinished = true;
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(9);
 */