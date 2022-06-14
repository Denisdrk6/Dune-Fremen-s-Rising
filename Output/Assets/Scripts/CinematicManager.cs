using System;
using RagnarEngine;

public class CinematicManager : RagnarComponent
{
	GameObject text;
	GameObject cinematic;
    
    private int IdDialogue;
    private int indexLine;

    private bool endDialogue;
    private AudioSource SceneAudio;

    private bool inMove = false;

    enum State
    {
		START,
        POST_START,
        INCINEMATIC,
        END,
        NONE
    }
	
    int state;
    String nextScene;
    String lenguage;


    public void Start()
	{
        indexLine = 0;
        text = GameObject.Find("Dialogue");
        SceneAudio = GameObject.Find("Audio").GetComponent<AudioSource>();

        state = 0;
        
        //Pos Text
        Vector3 pos = new Vector3(0, 0, 0);
        float posY = InternalCalls.GetRegionGame().y, posX = InternalCalls.GetRegionGame().x;
        posY -= 300;
        posX = -(InternalCalls.GetRegionGame().x / 4);
        /*pos.Set(posX, posY + 10, text.GetComponent<Transform2D>().position2D.z + 20);
        text.GetComponent<Transform2D>().position2D = pos;*/
        inMove = false;

        pos.Set(posX, -(InternalCalls.GetRegionGame().y / 2)+80, text.GetComponent<Transform2D>().position2D.z + 20);
        text.GetComponent<Transform2D>().position2D = pos;
    }

	public void Update()
	{
        switch (state)
        {
            case 0:// Start

                indexLine = 0;

                state = 1;
                
                break;
            case 1:// Update

                if (IdDialogue == 2 && (indexLine == 0 || indexLine == 6) && inMove == true)
                {
                    state = 4;
                    break;
                }

                state = 2;
                UpdateDialogue();
                break;
            case 2:// Imputs
                
                InCinematic();
                break;
            case 3:// End

                // Load new scene
                SceneManager.LoadScene(nextScene);
                state = 4;
                break;
            case 4:// NONE

                text.GetComponent<UIText>().text = " ";
                if (!inMove)
                {
                    state = 1;
                }
                break;
            default:
                break;
        }

    }

    private void InCinematic(){
        // Next Line
        if (Input.GetKey(KeyCode.SPACE) == KeyState.KEY_UP)
        {
            state = 1;
            NextLine();
        }

        if (Input.GetKey(KeyCode.P) == KeyState.KEY_UP)
        {
            state = 3;
        }
    }

    void NextLine()
    {
        endDialogue = Dialogue.NextLine();

        if (endDialogue == false)
            indexLine++;
        else // End dialogue
            state = 3;

        if (IdDialogue == 2 && (indexLine == 0 || indexLine == 6))
        {
            inMove = true;
            state = 4;
        }

    }

    void UpdateDialogue()
    {
        //Stop Previous Dialogue
        if (indexLine > 0)
        {
            int lastIndex = indexLine - 1;
            string lastVoicePath = "VOICE_" + lenguage + "_" + IdDialogue.ToString() + "_" + lastIndex.ToString();
            SceneAudio.StopCurrentClip(lastVoicePath);
        }

        //Text
        text.GetComponent<UIText>().text = Dialogue.GetDialogueLine().ToString();
        //Debug.Log(text.GetComponent<UIText>().text);

        //Voice  
        // SP=0 / ING=1
        switch (Dialogue.GetDialogueLenguage())
        {
            case 0:
                lenguage = "SP";
                break;
            case 1:
                lenguage = "ENG";
                break;
            default:
                break;
        }

        // VOICE
        String voicePath = "VOICE_" + lenguage + "_" + IdDialogue.ToString() + "_" + indexLine.ToString();
        //Debug.Log(voicePath);
        SceneAudio.PlayClip2(voicePath);

        // For animations
        cinematic.GetComponent<Cinematic_1>().SetLine(indexLine);
        cinematic.GetComponent<Cinematic_2>().SetLine(indexLine);
        cinematic.GetComponent<Cinematic_3>().SetLine(indexLine);
        cinematic.GetComponent<Cinematic_4>().SetLine(indexLine);
        cinematic.GetComponent<Cinematic_5>().SetLine(indexLine);
        cinematic.GetComponent<Cinematic_6>().SetLine(indexLine);
        cinematic.GetComponent<Cinematic_7>().SetLine(indexLine);

    }

    public void SetInMove(bool x) { inMove = x; }

    public bool GetInMove() { return inMove; }

    public void SetIDDialogue(int ID,String _nextScene) {
        endDialogue = false;
        IdDialogue = ID;

        Dialogue.LoadDialogueFile("");
        Dialogue.StartDialogueById(IdDialogue);
        nextScene = _nextScene;
        cinematic = GameObject.Find("Cinematic");
    }
}