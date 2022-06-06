using System;
using RagnarEngine;

public class Cinematic_4 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;

    public int IdDialogue = 6;

    CinematicManager dialogues;

    GameObject stilgar;
    //GameObject jessica;
    Animation stilgarAnimation;
    //Animation jessicaAnimation;

    GameObject rabann;
    GameObject enemy1;
    GameObject enemy2;
    Animation rabannAnimation;
    Animation enemy1Animation;
    Animation enemy2Animation;
    
    GameObject hostageDiscussion;
    GameObject hostageFemale01;
    GameObject hostageFemale02;
    GameObject hostageFemale03;
    GameObject hostageFemale04;
    GameObject hostageFemale05;
    GameObject hostageFemale06;
    GameObject hostageFemale07;
    //Animation hostageDiscussionAnimation;
    Animation hostageFemale01Animation;
    Animation hostageFemale02Animation;
    Animation hostageFemale03Animation;
    Animation hostageFemale04Animation;
    Animation hostageFemale05Animation;
    Animation hostageFemale06Animation;
    Animation hostageFemale07Animation;

    public int IdLine = 0;
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

        // Find all the necessary GameObjects
        stilgar = GameObject.Find("Stilgar");
        //jessica = GameObject.Find("LadyJessica");

        rabann = GameObject.Find("Rabann");
        enemy1 = GameObject.Find("Basic Enemy 1");
        enemy2 = GameObject.Find("Basic Enemy 2");

        hostageDiscussion = GameObject.Find("Hostage M 1"); 
        hostageFemale01 = GameObject.Find("Hostage F 1");
        hostageFemale02 = GameObject.Find("Hostage F 2");
        hostageFemale03 = GameObject.Find("Hostage F 3");
        hostageFemale04 = GameObject.Find("Hostage F 4");
        hostageFemale05 = GameObject.Find("Hostage F 5");
        hostageFemale06 = GameObject.Find("Hostage F 6");
        hostageFemale07 = GameObject.Find("Hostage F 7");

        // Set and play default animations for all the GameObjects
        stilgarAnimation = stilgar.GetComponent<Animation>();
        stilgarAnimation.PlayAnimation("Idle");
        //jessicaAnimation = jessica.GetComponent<Animation>();
        //chaniAnimation.PlayAnimation("Idle");

        rabannAnimation = rabann.GetComponent<Animation>();
        rabannAnimation.PlayAnimation("WalkAngry");
        enemy1Animation = enemy1.GetComponent<Animation>();
        enemy1Animation.PlayAnimation("Idle");
        enemy2Animation = enemy2.GetComponent<Animation>();
        enemy2Animation.PlayAnimation("Idle");

        hostageFemale01Animation = hostageFemale01.GetComponent<Animation>();
        hostageFemale01Animation.PlayAnimation("Idle Sad");
        hostageFemale02Animation = hostageFemale02.GetComponent<Animation>();
        hostageFemale02Animation.PlayAnimation("Idle Sad");
        hostageFemale03Animation = hostageFemale03.GetComponent<Animation>();
        hostageFemale03Animation.PlayAnimation("Idle Sad");
        hostageFemale04Animation = hostageFemale04.GetComponent<Animation>();
        hostageFemale04Animation.PlayAnimation("Idle Sad");
        hostageFemale05Animation = hostageFemale05.GetComponent<Animation>();
        hostageFemale05Animation.PlayAnimation("Idle Sad");
        hostageFemale06Animation = hostageFemale06.GetComponent<Animation>();
        hostageFemale06Animation.PlayAnimation("Idle Sad");
        hostageFemale07Animation = hostageFemale07.GetComponent<Animation>();
        hostageFemale07Animation.PlayAnimation("Idle Sad");

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