using System;
using RagnarEngine;

public class Cinematic_5 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;

    public int IdDialogue = 5;

    CinematicManager dialogues;

    GameObject paul;
    Animation paulA;
    GameObject chani;
    Animation chaniA;
    GameObject stilgar;
    Animation stilgarA;

    public int IdLine = 0;
    enum CinematicState
    {
        FIRST, TRANSITION, ANIMATIONS
    }
    private CinematicState state;

    public void Start()
    {
        //Set UI Bands
        bands = new GameObject[2];
        bands[0] = GameObject.Find("High_Band");
        bands[1] = GameObject.Find("Low_Band");

        bands[0].transform.globalPosition = new Vector3(0f, 449f, -10.4f);
        bands[1].transform.globalPosition = new Vector3(0f, -447f, -10.4f);

        paul = GameObject.Find("Player");
        chani = GameObject.Find("Player_2");
        stilgar = GameObject.Find("Player_3");
        paulA = paul.GetComponent<Animation>();
        chaniA = chani.GetComponent<Animation>();
        stilgarA = stilgar.GetComponent<Animation>();


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
        dialogues.SetIDDialogue(IdDialogue, "Cinematic_6");
    }

    private void Animations()
    {
        /*switch (IdLine)
        {   // EL dialogo puede tener mas o menos lineas
            // Tened en cuenta que por aqui solo pasara...
            // cuando se pase a la siguiente linea de dialogo
            case 0:
                stilgarA.PlayAnimation("Talk");

                paulA.PlayAnimation("Idle");
                chaniA.PlayAnimation("Idle");

                break;
            case 1:
                paulA.PlayAnimation("Talk");
                stilgarA.PlayAnimation("Idle");

                break;
            case 2:
                stilgarA.PlayAnimation("Talk");
                paulA.PlayAnimation("Idle");

                break;
            case 3:
                paulA.PlayAnimation("Talk");
                stilgarA.PlayAnimation("Idle");

                break;
            case 4:
                chaniA.PlayAnimation("Talk");

                paulA.PlayAnimation("Idle");
                stilgarA.PlayAnimation("Idle");
                break;

            default:
                break;
        }*/
    }

    public void SetLine(int line)
    {
        IdLine = line;
        state = CinematicState.ANIMATIONS;
    }
}