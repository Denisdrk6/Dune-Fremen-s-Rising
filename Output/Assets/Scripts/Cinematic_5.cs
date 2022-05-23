using System;
using RagnarEngine;

public class Cinematic_5 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;

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

        GameObject characterBoss = GameObject.Find("char_boss");

        characterBoss.GetComponent<Animation>().PlayAnimation("WalkAngry");

        dialogues = GameObject.Find("CinematicDialogue").GetComponent<CinematicManager>();



        first = false;
    }
    public void Update()
    {
        if (!first)
        {

            dialogues.SetIDDialogue(IdDialogue, "build2");

            first = true;
        }
    }

}