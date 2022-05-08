using System;
using RagnarEngine;

public class DialogueManager : RagnarComponent
{
	//UIText toxt;
	GameObject box;
	GameObject text;
	GameObject image;
    GameObject name;
	Vector3 pos;
	string auth;
    int authId;
    // 0=Paul / 1=Chani / 2=Rehen Fremen / 3=Rabban / 
    // 4=Soldado Harkonnen / 5=Soldado / 
    // 6=Lady Jessica / 7=Stilgar
    //int idDialogue;
	bool firstTime;

	bool endDialogue;
    bool inDialogue;

	public void Start()
	{
		Dialogue.LoadDialogueFile("");
		box = GameObject.Find("DialogueBox");
		text = GameObject.Find("DialogueText");
		image = GameObject.Find("DialogueAuthImg");
        name = GameObject.Find("DialogueAuthName");
		auth = "";
        authId = -1;
        //idDialogue = 0;
		pos = new Vector3(0, 0, 0);
		endDialogue = false;
        firstTime = true;
        inDialogue = false;
        DisableDialogue();
        
    }

    public void Update()
    {
        if (firstTime)
        {

            pos.Set(192.0f, text.GetComponent<Transform2D>().position2D.y + 25, text.GetComponent<Transform2D>().position2D.z + 20);
            text.GetComponent<Transform2D>().position2D = pos;
            pos.Set(0.0f, image.GetComponent<Transform2D>().position2D.y-2, image.GetComponent<Transform2D>().position2D.z + 20);
            image.GetComponent<Transform2D>().position2D = pos;
            pos.Set(185.0f, name.GetComponent<Transform2D>().position2D.y + 25, image.GetComponent<Transform2D>().position2D.z + 20);
            name.GetComponent<Transform2D>().position2D = pos;
            pos.Set(box.GetComponent<Transform2D>().position2D.x, box.GetComponent<Transform2D>().position2D.y, box.GetComponent<Transform2D>().position2D.z - 10);
            box.GetComponent<Transform2D>().position2D = pos;


            firstTime = false;
        }

        // Next Line
        if (Input.GetKey(KeyCode.SPACE) == KeyState.KEY_UP && gameObject.isActive)
        {
            NextLine();
        }
        //End Dialogue
        if (Input.GetKey(KeyCode.P) == KeyState.KEY_UP)
        {
            EndDialogue();
        }
        /*
        // Active Dialogue ID = 0 (firt dialogue)
        if (Input.GetKey(KeyCode.J) == KeyState.KEY_UP)
        {
            StartNewDialogue(0);
        }
        // Next Dialogue
        if (Input.GetKey(KeyCode.K) == KeyState.KEY_UP)
        {
            idDialogue++;
            StartNewDialogue(idDialogue);
        }
        //Desactive Dialogue
        if (Input.GetKey(KeyCode.O) == KeyState.KEY_UP)
        {
            DisableDialogue();
        }
        // Continue dialogue
        if (Input.GetKey(KeyCode.I) == KeyState.KEY_UP)
        {
            ContinueDialogue();
        }*/
    }
    private void DrawAuthor()
    {
        // 0=Paul / 1=Chani / 2=Rehen Fremen / 3=Rabban / 
        // 4=Soldado Harkonnen / 5=Soldado / 
        // 6=Lady Jessica / 7=Stilgar
        switch (authId)
        {
            case 0:
                image.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_dialog_paul.png");
                break;
            case 1:
                image.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_dialog_chani.png");
                break;
            case 2:
                image.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_dialog_fremen_hostage.png");
                break;
            case 3:
                image.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_dialog_rabann.png");
                break;
            case 4:
                image.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_dialog_soldier_1.png");
                break;
            case 5:
                image.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_dialog_soldier_2.png");
                break;
            case 6:
                image.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_dialog_jessica.png");
                break;
            case 7:
                image.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_dialog_stilgar.png");
                break;

            default:
                break;
        }
    }

    void NextLine()
	{
		endDialogue = Dialogue.NextLine();
		if (endDialogue == false)
		{
            UpdateDialogue();
            Debug.Log(authId.ToString());
        }
        else
		{
            EndDialogue();
        }
	}

    void EndDialogue()
    {
        endDialogue = true;
        inDialogue = false;
        DisableDialogue();
    }

    void UpdateDialogue() 
    {
        text.GetComponent<UIText>().text = Dialogue.GetDialogueLine();
        auth = Dialogue.GetDialogueLineAuthor();
        name.GetComponent<UIText>().text = auth;
        authId = Dialogue.GetDialogueLineAuthorId();
        DrawAuthor();
    }

	public void StartNewDialogue(int id)
    {
        gameObject.isActive = true;
        endDialogue = false;
        inDialogue = true;
        Dialogue.StartDialogueById(id);
        UpdateDialogue();
        Debug.Log(authId.ToString());
    }

    public void DisableDialogue()
    {
        gameObject.isActive = false;
    }

    public void ContinueDialogue()
    {
        gameObject.isActive = true;
    }

    public bool GetInDialogue() { return inDialogue; }

    public bool GetEndDialogue() { return endDialogue; }
}