using System;
using RagnarEngine;

public class Cinematic_1 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;
    public Characters[] characters;

    public int IdDialogue = 0;

    CinematicManager dialogues;

    private bool first;

    public void Start()
	{
        //Set UI Bands
        bands = new GameObject[2];
        bands[0] = GameObject.Find("High_Band");
        bands[1] = GameObject.Find("Low_Band");
        
        bands[0].transform.globalPosition = new Vector3(0f, 449f, -10.4f);
        bands[1].transform.globalPosition = new Vector3(0f, -447f, -10.4f);

        GameObject characterPaul = GameObject.Find("char_paul");
        GameObject characterChani = GameObject.Find("char_chani");

        characterPaul.GetComponent<Animation>().PlayAnimation("Talk");
        characterChani.GetComponent<Animation>().PlayAnimation("Talk");

        dialogues = GameObject.Find("CinematicDialogue").GetComponent<CinematicManager>();



        first = false;
    }
	public void Update()
	{
        if (!first)
        {

            dialogues.SetIDDialogue(IdDialogue,"build");

            first = true;
        }
    }

}