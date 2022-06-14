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
        if (!isUsed) return;
        switch (dialogueId)
        {
            case 9:
                LoadSceneWin();
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
            InternalCalls.Destroy(gameObject);
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

    public void ActiveCinematic(string cinematic)
    {
        if (!isUsed)
        {
            nexLevel = true;
            isUsed = true;
            Input.RestoreDefaultCursor();
            GameObject.Find("Quest System").GetComponent<QuestSystem>().SaveMissions();
            SceneManager.LoadScene(cinematic);
            //InternalCalls.Destroy(gameObject);
        }
    }

    public bool GetUsed() { return isUsed; }
    public void SetUsed(bool used) { isUsed = used; }
}

/* ESTO DEBE ESTAR EN EL C�DIGO DEL BOSS CUANDO MUERA
   GameObject.Find("Quest System").GetComponent<QuestSystem>().levelFinished = true;
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(9);
 */