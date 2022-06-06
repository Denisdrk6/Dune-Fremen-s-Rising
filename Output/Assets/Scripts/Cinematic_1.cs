using System;
using RagnarEngine;

public class Cinematic_1 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;

    public int IdDialogue = 0;

    CinematicManager dialogues;


    GameObject boss;
    Animation bossA;
    GameObject  Enemy1;
    Animation   EnemyA1;
    GameObject  Enemy2;
    Animation   EnemyA2;
    GameObject  Enemy3;
    Animation   EnemyA3;

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

        boss = GameObject.Find("Boss");
        //paulAnimation.PlayAnimation("Idle");
        Enemy1 = GameObject.Find("BEnemy1");
        Enemy2 = GameObject.Find("BEnemy2");
        Enemy3 = GameObject.Find("BEnemy3");
        bossA   = boss.GetComponent<Animation>();
        EnemyA1 = Enemy1.GetComponent<Animation>();
        EnemyA2 = Enemy2.GetComponent<Animation>();
        EnemyA3 = Enemy3.GetComponent<Animation>();

        bossA.PlayAnimation("Talk");
        EnemyA1.PlayAnimation("Talk");
        EnemyA2.PlayAnimation("Talk");
        EnemyA3.PlayAnimation("Talk");

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