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
        paul = GameObject.Find("Player");
        chani = GameObject.Find("Player_2");
        paulAnimation = paul.GetComponent<Animation>();
        chaniAnimation = chani.GetComponent<Animation>();
        GameObject.Find("WalkParticles").GetComponent<ParticleSystem>().Pause();
        GameObject.Find("RunParticles").GetComponent<ParticleSystem>().Pause();

        GameObject.Find("WalkParticles_2").GetComponent<ParticleSystem>().Pause();
        GameObject.Find("RunParticles_2").GetComponent<ParticleSystem>().Pause();

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
        dialogues.SetIDDialogue(IdDialogue, "build");
    }

    private void Animations()
    {
        switch (IdLine)
        {   // EL dialogo puede tener mas o menos lineas
            // Tened en cuenta que por aqui solo pasara...
            // cuando se pase a la siguiente linea de dialogo
            case 0:
                paul.GetComponent<Animation>().PlayAnimation("Crouch");
                // If chani var is used this crashes
                GameObject.Find("Player_2").GetComponent<Animation>().PlayAnimation("Crouch");

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