using System;
using RagnarEngine;

public class Cinematic_2 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;

    public int IdDialogue = 0;

    CinematicManager dialogues;

    public Characters[] characters;

    public int IdLine = 0;
    enum CinematicState
    {
        FIRST, TRANSITION, ANIMATIONS
    }
    private CinematicState state;

    public void Start()
    {
        Debug.Log("Cinematic 2 Start");

        //Set UI Bands
        //bands = new GameObject[2];
        //bands[0] = GameObject.Find("High_Band");
        //bands[1] = GameObject.Find("Low_Band");
        //bands[0].transform.globalPosition = new Vector3(0f, 449f, -10.4f);
        //bands[1].transform.globalPosition = new Vector3(0f, -447f, -10.4f);

        characters = new Characters[1];
        characters[0] = new Characters
        {
            name = "Paul Atreides",
            prefabPath = "Player",
            state = State.NONE,
            abilities = new Abilities[0],
            hitPoints = 4,
            pos = new Vector3(-43.69f, 0f, 199.77f)
        };

        //paul = InternalCalls.InstancePrefab("Player", new Vector3(-43.69f, 0f, 199.77f));
        //chani = InternalCalls.InstancePrefab("Player_2", new Vector3(-42, 3, 200));

        GameObject pm = InternalCalls.InstancePrefab("PlayerManager", Vector3.zero);
        pm.GetComponent<PlayerManager>().characters = characters;


        //paul.GetComponent<Animation>().PlayAnimation("Idle");
        //chani.GetComponent<Animation>().PlayAnimation("Idle");

        //// Deberian ser variables publicas para poder ponerles las animaciones en la funcion: Animations()
        //GameObject char_boss = GameObject.Find("char_boss");
        //GameObject char_enemy1 = GameObject.Find("char_enemy1");
        //GameObject char_enemy2 = GameObject.Find("char_enemy2");
        //GameObject char_enemy3 = GameObject.Find("char_enemy3");
        //GameObject char_enemy4 = GameObject.Find("char_enemy4");
        //GameObject char_hostage = GameObject.Find("char_hostage");
        //
        //char_boss.GetComponent<Animation>().PlayAnimation("Run");
        //char_enemy1.GetComponent<Animation>().PlayAnimation("Idle");
        //char_enemy2.GetComponent<Animation>().PlayAnimation("Idle");
        //char_enemy3.GetComponent<Animation>().PlayAnimation("Idle");
        //char_enemy4.GetComponent<Animation>().PlayAnimation("Idle");
        //char_hostage.GetComponent<Animation>().PlayAnimation("Knee Idle");

        dialogues = GameObject.Find("Cinematic2").GetComponent<CinematicManager>();

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
        //var3: Numera de la cinematica (Ej: Cinematica_1 = 1)
        dialogues.SetIDDialogue(IdDialogue, "build", 2);
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
}