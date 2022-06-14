using System;
using RagnarEngine;

public class Cinematic_7 : RagnarComponent
{
    public bool runGame = true;

    public int IdDialogue = 9;

    CinematicManager dialogues;

    public int IdLine = 0;

    GameObject paul;
    GameObject chani;
    GameObject stilgar;
    GameObject boss;


    enum CinematicState
    {
        FIRST, TRANSITION, ANIMATIONS
    }
    private CinematicState state;

    public void Start()
    {
        paul = GameObject.Find("Player");
        chani = GameObject.Find("Player_2");
        stilgar = GameObject.Find("Player_3");
        boss = GameObject.Find("Boss");

        paul.GetComponent<Animation>().PlayAnimation("Talk");
        chani.GetComponent<Animation>().PlayAnimation("Idle");
        stilgar.GetComponent<Animation>().PlayAnimation("Idle");
        boss.GetComponent<Animation>().PlayAnimation("Dead");
        boss.GetComponent<Material>().outlineNormals = 0.0f;

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
                chani.GetComponent<Animation>().PlayAnimation("Talk");
                paul.GetComponent<Animation>().PlayAnimation("Idle");
                stilgar.GetComponent<Animation>().PlayAnimation("Idle");
                break;

            case 2:
                stilgar.GetComponent<Animation>().PlayAnimation("Talk");
                paul.GetComponent<Animation>().PlayAnimation("Idle");
                chani.GetComponent<Animation>().PlayAnimation("Idle");

                break;

            case 3:
                paul.GetComponent<Animation>().PlayAnimation("Talk");
                chani.GetComponent<Animation>().PlayAnimation("Idle");
                stilgar.GetComponent<Animation>().PlayAnimation("Idle");
                break;

            case 4:
                stilgar.GetComponent<Animation>().PlayAnimation("Talk");
                paul.GetComponent<Animation>().PlayAnimation("Idle");
                chani.GetComponent<Animation>().PlayAnimation("Idle");
                break;

            case 5:
                chani.GetComponent<Animation>().PlayAnimation("Talk");
                paul.GetComponent<Animation>().PlayAnimation("Idle");
                stilgar.GetComponent<Animation>().PlayAnimation("Idle");
                break;

            case 6:
                paul.GetComponent<Animation>().PlayAnimation("Talk");
                chani.GetComponent<Animation>().PlayAnimation("Idle");
                stilgar.GetComponent<Animation>().PlayAnimation("Idle");
                break;

            case 7:
                chani.GetComponent<Animation>().PlayAnimation("Talk");
                paul.GetComponent<Animation>().PlayAnimation("Idle");
                stilgar.GetComponent<Animation>().PlayAnimation("Idle");
                break;

            case 8:
                stilgar.GetComponent<Animation>().PlayAnimation("Talk");
                paul.GetComponent<Animation>().PlayAnimation("Idle");
                chani.GetComponent<Animation>().PlayAnimation("Idle");
                break;

            case 9:
                chani.GetComponent<Animation>().PlayAnimation("Talk");
                paul.GetComponent<Animation>().PlayAnimation("Idle");
                stilgar.GetComponent<Animation>().PlayAnimation("Idle");
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