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

    NavAgent chaniNavAgent;
    ParticleSystem walkPartSys;

    GameObject audio;
    GameObject exitPoint;

    bool moving = false;

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

        GameObject.Find("WalkParticles").GetComponent<ParticleSystem>().Pause();
        GameObject.Find("RunParticles").GetComponent<ParticleSystem>().Pause();

        GameObject.Find("WalkParticles_2").GetComponent<ParticleSystem>().Pause();
        walkPartSys = GameObject.Find("RunParticles_2").GetComponent<ParticleSystem>();
        walkPartSys.Pause();

        audio = GameObject.Find("Audio");
        exitPoint = GameObject.Find("ExitPoint");

        chaniNavAgent = GameObject.Find("Player_2").GetComponent<NavAgent>();
        chaniNavAgent.speed = 0;

        dialogues = GameObject.Find("CinematicDialogue").GetComponent<CinematicManager>();
        //-----------
        state = CinematicState.FIRST;
        //-----------

    }

    public void Update()
    {
        if (moving)
        {
            chaniNavAgent.CalculatePath(GameObject.Find("ExitPoint").transform.globalPosition);
            chaniNavAgent.MovePath();
        }

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
                //Animation anim = GameObject.Find("Player_2").GetComponent<Animation>();
                //anim.PlayAnimation("Idle");
                break;

            case 5:
                MoveChani();
                
                break;

            case 6:

                break;

            case 7:

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

    void MoveChani()
    {
        walkPartSys.Play();
        chaniNavAgent.speed = 5;
        Animation anim = GameObject.Find("Player_2").GetComponent<Animation>();
        anim.PlayAnimation("Walk");

        moving = true;
    }
}