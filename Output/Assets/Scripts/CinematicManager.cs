using System;
using RagnarEngine;

public class CinematicManager : RagnarComponent
{
	GameObject cinematic;

	GameObject text;

    private int IdDialogue;
    private int indexLine;

    private bool endDialogue;
    private State state;
    public enum State
    {
		START,INCINEMATIC,END
    }
	
	public void Start()
	{
        indexLine = 0;
        Dialogue.LoadDialogueFile("");
        cinematic = GameObject.Find("LevelManager");
        text = GameObject.Find("Dialogue");
        state = State.START;

        Vector3 pos = new Vector3(0, 0, 0);
        float posY = InternalCalls.GetRegionGame().y, posX = InternalCalls.GetRegionGame().x;
        posY *= 0.33f;
        posX = 0;
        pos.Set(posX, posY + 10, text.GetComponent<Transform2D>().position2D.z + 20);
        text.GetComponent<Transform2D>().position2D = pos;
    }
	public void Update()
	{
        switch (state)
        {
            case State.START:
                indexLine = 0;
                state = State.INCINEMATIC;
                SetIDDialogue(0);
                break;
            case State.INCINEMATIC:
                Debug.Log("INCINEMATIC");

                InCinematic();
                break;
            case State.END:
                // Cambio de escena
                text.GetComponent<UIText>().text = "Fin de Cinematica";

                break;
            default:
                break;
        }
    }

    private void InCinematic(){
        // Next Line
        if (Input.GetKey(KeyCode.SPACE) == KeyState.KEY_UP)
        {
            //SceneAudio.GetComponent<AudioSource>().PlayClip("UI_DIALOGUEPASS");
            NextLine();
        }
    }

    void NextLine()
    {
        endDialogue = Dialogue.NextLine();
        if (endDialogue == false)
        {
            indexLine++;
            UpdateDialogue();
        }
        else
        {
            state = State.END;
        }
    }
    void UpdateDialogue()
    {
        text.GetComponent<UIText>().text = Dialogue.GetDialogueLine();
    }
    public void SetIDDialogue(int ID) {
        IdDialogue = ID;
        state = State.INCINEMATIC;
        StartNewDialogue(IdDialogue);
    }

    public void StartNewDialogue(int id)
    {
        endDialogue = false;
        Dialogue.StartDialogueById(id);
        UpdateDialogue();
        //Debug.Log(authId.ToString());
    }

}