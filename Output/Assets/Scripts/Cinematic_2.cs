using System;
using RagnarEngine;

public class Cinematic_2 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;

    public int IdDialogue = 0;

    CinematicManager dialogues;

    GameObject paul;
    Animation paulAnimation;
    GameObject chani;
    Animation chaniAnimation;


    public int IdLine = 0;
    enum CinematicState
    {
        FIRST, TRANSITION, ANIMATIONS
    }
    private CinematicState state;

    public void Start()
    {
        Vector3 size = InternalCalls.GetRegionGame();
        //GameObject.Find("High_Band").transform.globalPosition = new Vector3(0f, -size.y - 100, -10.4f);
        //GameObject.Find("Low_Band").transform.globalPosition = new Vector3(0f, size.y + 100, -10.4f);

        //GameObject.Find("High_Band").transform.globalPosition = new Vector3(0f, -447f, -10.4f);
        //GameObject.Find("Low_Band").transform.globalPosition = new Vector3(0f, 449f, -10.4f);

        paul = GameObject.Find("Player");
        chani = GameObject.Find("Player_2");
        paulAnimation = paul.GetComponent<Animation>();
        chaniAnimation = chani.GetComponent<Animation>();

        //GameObject.Find("Camera").transform.globalPosition = new Vector3(0f, 37f, -50f);
        //GameObject.Find("cameraController").transform.globalPosition = new Vector3(-43f, 0f, 144f);

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
                //state = CinematicState.ANIMATIONS;
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
        dialogues.SetIDDialogue(IdDialogue, "build");
    }

    private void Animations()
    {
        switch (IdLine)
        {   // EL dialogo puede tener mas o menos lineas
            // Tened en cuenta que por aqui solo pasara...
            // cuando se pase a la siguiente linea de dialogo
            case 0:
                paul.GetComponent<Animation>().PlayAnimation("Idle");
                // If chani var is used this crashes
                GameObject.Find("Player_2").GetComponent<Animation>().PlayAnimation("Idle");

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
}