using System;
using RagnarEngine;

public class Cinematic_4 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;

    public int IdDialogue = 4;
    public int IdLine = 0;

    CinematicManager dialogues;

    GameObject audio;

    enum CinematicState
    {
        FIRST, TRANSITION, ANIMATIONS
    }
    private CinematicState state;

    public void Start()
    {
        // Set UI Bands
        bands = new GameObject[2];
        bands[0] = GameObject.Find("High_Band");
        bands[1] = GameObject.Find("Low_Band");

        bands[0].transform.globalPosition = new Vector3(0f, 449f, -10.4f);
        bands[1].transform.globalPosition = new Vector3(0f, -447f, -10.4f);

        audio = GameObject.Find("Audio");

        GameObject.Find("Stilgar").GetComponent<Animation>().PlayAnimation("Idle");
        //GameObject.Find("LadyJessica").GetComponent<Animation>().PlayAnimation("Idle");

        GameObject.Find("Rabann").GetComponent<Animation>().PlayAnimation("WalkAngry");
        GameObject.Find("Basic Enemy 1").GetComponent<Animation>().PlayAnimation("Idle");
        GameObject.Find("Basic Enemy 2").GetComponent<Animation>().PlayAnimation("Idle");

        GameObject.Find("Hostage M 1").GetComponent<Animation>().PlayAnimation("Idle");
        GameObject.Find("Hostage F 1").GetComponent<Animation>().PlayAnimation("IdleSad");
        GameObject.Find("Hostage F 2").GetComponent<Animation>().PlayAnimation("IdleSad");
        GameObject.Find("Hostage F 3").GetComponent<Animation>().PlayAnimation("IdleSad");
        GameObject.Find("Hostage F 4").GetComponent<Animation>().PlayAnimation("IdleSad");
        GameObject.Find("Hostage F 5").GetComponent<Animation>().PlayAnimation("IdleSad");
        GameObject.Find("Hostage F 6").GetComponent<Animation>().PlayAnimation("IdleSad");
        GameObject.Find("Hostage F 7").GetComponent<Animation>().PlayAnimation("IdleSad");             

        dialogues = GameObject.Find("CinematicDialogue").GetComponent<CinematicManager>();

        //-----------
        state = CinematicState.FIRST;
        //-----------

    }

    public void Update()
    {
        switch (state)
        {
            case CinematicState.FIRST:

                StartCinematic();
                state = CinematicState.TRANSITION;

                break;
            case CinematicState.TRANSITION:
                //Aqui no pasara nada

                break;
            case CinematicState.ANIMATIONS:

                Animations();
                state = CinematicState.TRANSITION;

                break;
            default:
                break;
        }
    }

    // IMPORTATNTE CAMBIAR ESTO SEGUN TU CINEMATICA
    private void StartCinematic()
    {
        //var1: ID del dialogo que se hara en la cinematica(variable arriba)
        //var2: Nombre de la escena a la que se irá cuando acabe el dialogo
        dialogues.SetIDDialogue(IdDialogue, "build2");
    }

    private void Animations()
    {
        switch (IdLine)
        {   // EL dialogo puede tener mas o menos lineas
            // Tened en cuenta que por aqui solo pasara...
            // cuando se pase a la siguiente linea de dialogo
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
            case 3:

                break;
            case 4:

                break;
            case 5:

                break;
            case 6:

                break;

            default:
                break;
        }
    }

    public void SetLine(int line)
    {
        IdLine = line;
        state = CinematicState.ANIMATIONS;
    }

    void EscortJessica()
    {
        //jessicaNavAgent.speed = 5;
        Animation anim = GameObject.Find("Player_2").GetComponent<Animation>();
        anim.PlayAnimation("Walk");

        //moving = true;
    }
}