using System;
using RagnarEngine;

public class Cinematic_2 : RagnarComponent
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

        GameObject char_boss = GameObject.Find("char_boss");
        GameObject char_enemy1 = GameObject.Find("char_enemy1");
        GameObject char_enemy2 = GameObject.Find("char_enemy2");
        GameObject char_enemy3 = GameObject.Find("char_enemy3");
        GameObject char_enemy4 = GameObject.Find("char_enemy4");

        char_boss.GetComponent<Animation>().PlayAnimation("Run");
        char_enemy1.GetComponent<Animation>().PlayAnimation("Idle");
        char_enemy2.GetComponent<Animation>().PlayAnimation("Idle");
        char_enemy3.GetComponent<Animation>().PlayAnimation("Idle");
        char_enemy4.GetComponent<Animation>().PlayAnimation("Idle");

        dialogues = GameObject.Find("CinematicDialogue").GetComponent<CinematicManager>();

        first = false;
    }
    public void Update()
    {
        if (!first)
        {

            dialogues.SetIDDialogue(IdDialogue, "build");

            first = true;
        }
    }

}