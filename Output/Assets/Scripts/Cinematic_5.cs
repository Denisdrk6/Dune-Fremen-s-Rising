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

        stilgarA.PlayAnimation("Talk");

        paulA.PlayAnimation("Idle");
        chaniA.PlayAnimation("Idle");

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
        dialogues.SetIDDialogue(IdDialogue, "WinScene");
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
                Animation anim1 = GameObject.Find("Player").GetComponent<Animation>();
                anim1.PlayAnimation("Talk");
                Animation anim2 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim2.PlayAnimation("Idle");

                break;
            case 2:
                Animation anim3 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim3.PlayAnimation("Talk");
                Animation anim4 = GameObject.Find("Player").GetComponent<Animation>();
                anim4.PlayAnimation("Idle");

                break;
            case 3:
                Animation anim5 = GameObject.Find("Player").GetComponent<Animation>();
                anim5.PlayAnimation("Talk");
                Animation anim6 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim6.PlayAnimation("Idle");

                break;
            case 4:
                Animation anim = GameObject.Find("Player_2").GetComponent<Animation>();
                anim.PlayAnimation("Talk");

                Animation anim7 = GameObject.Find("Player").GetComponent<Animation>();
                anim7.PlayAnimation("Talk");
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
}