using System;
using RagnarEngine;

public class Cinematic_3 : RagnarComponent
{
    public bool runGame = true;
    public GameObject[] bands;

    public int IdDialogue = 2;

    CinematicManager dialogues;

    GameObject paul;
    //Animation paulAnimation;
    GameObject chani;
    //Animation chaniAnimation;
    GameObject stilgar;
    GameObject rabban;
    GameObject basicEnemy;
    GameObject undistractable;
    GameObject undistractable2;

    //GameObject exitPoint;

    NavAgent paulNavAgent;
    ParticleSystem walkPartSys;

    //bool movingPaul = false;
    bool fightPaul = false;
    bool fightChani = false;
    bool hitPaul = false;
    bool kneePaul = false;
    bool moveChani = false;

    //bool inMove = true;

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
        stilgar = GameObject.Find("Player_3");
        rabban = GameObject.Find("Boss");
        undistractable = GameObject.Find("Undistractable Enemy");
        undistractable2 = GameObject.Find("Undistractable Enemy 2");

        //paul.GetComponent<Animation>().PlayAnimation("Run");
        chani.GetComponent<Animation>().PlayAnimation("Crouch");
        stilgar.GetComponent<Animation>().PlayAnimation("Crouch");
        undistractable.GetComponent<Animation>().PlayAnimation("Idle");
        undistractable2.GetComponent<Animation>().PlayAnimation("Idle");
        rabban.GetComponent<Animation>().PlayAnimation("Idle");

        GameObject.Find("WalkParticles").GetComponent<ParticleSystem>().Pause();
        GameObject.Find("WalkParticles_2").GetComponent<ParticleSystem>().Pause();
        GameObject.Find("WalkParticles_3").GetComponent<ParticleSystem>().Pause();
        GameObject.Find("RunParticles_3").GetComponent<ParticleSystem>().Pause();
        GameObject.Find("RunParticles_2").GetComponent<ParticleSystem>().Pause();
        walkPartSys = GameObject.Find("RunParticles").GetComponent<ParticleSystem>();
        walkPartSys.Pause();

        //exitPoint = GameObject.Find("ExitPoint");

        paulNavAgent = GameObject.Find("Player").GetComponent<NavAgent>();
        paulNavAgent.speed = 0;

        dialogues = GameObject.Find("CinematicDialogue").GetComponent<CinematicManager>();

        dialogues.SetInMove(true);


        //-----------
        state = CinematicState.FIRST;
        //-----------

    }

    public void Update()
    {
        if (IdLine == 0)
        {
            if (!fightPaul)
            {
                paulNavAgent.CalculatePath(GameObject.Find("Undistractable Enemy 2").transform.globalPosition);
                paulNavAgent.MovePath();
                if (paul.transform.globalPosition.x < -44.6)
                {
                    FightPaul();
                    fightPaul = true;
                }
            }
            if (!hitPaul && paul.GetComponent<Animation>().HasFinished())
            {
                HitPaul();
                hitPaul = true;
            }
            if (hitPaul && !kneePaul && GameObject.Find("Undistractable Enemy 2").GetComponent<Animation>().HasFinished())
            {
                KneePaul();
                kneePaul = true;
            }
        }
        if(IdLine == 6)
        {
            //if (!moveChani)
            //{
            // Empeza liena 6
            dialogues.SetInMove(true);

            GameObject.Find("Boss").GetComponent<NavAgent>().CalculatePath(GameObject.Find("ExitPoint").transform.globalPosition);
                GameObject.Find("Boss").GetComponent<NavAgent>().MovePath();
                GameObject.Find("Undistractable Enemy 2").GetComponent<NavAgent>().CalculatePath(GameObject.Find("ExitPoint").transform.globalPosition);
                GameObject.Find("Undistractable Enemy 2").GetComponent<NavAgent>().MovePath();
                if (GameObject.Find("Boss").transform.localPosition.x < -60)
                {
                    MoveChani();
                    moveChani = true;
                }
            //}
            if(!fightChani && moveChani)
            {
                GameObject.Find("Player_2").GetComponent<NavAgent>().CalculatePath(GameObject.Find("Undistractable Enemy").transform.globalPosition);
                GameObject.Find("Player_2").GetComponent<NavAgent>().MovePath();
                GameObject.Find("Player_3").GetComponent<NavAgent>().CalculatePath(GameObject.Find("Player").transform.globalPosition);
                GameObject.Find("Player_3").GetComponent<NavAgent>().MovePath();
                if (GameObject.Find("Player_2").transform.localPosition.x < -47)
                {
                    FightChani();
                    fightChani = true;
                }
            }
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
        dialogues.SetIDDialogue(IdDialogue, "WinScene");
    }

    private void Animations()
    {
        switch (IdLine)
        {   // EL dialogo puede tener mas o menos lineas
            // Tened en cuenta que por aqui solo pasara...
            // cuando se pase a la siguiente linea de dialogo
            case 0:
                if (!fightPaul) MovePaul();
                if (fightPaul) FightPaul();
                break;
            case 1:
                
                break;
            case 2:
                Animation anim = GameObject.Find("Boss").GetComponent<Animation>();
                anim.PlayAnimation("Shout");
                break;
            case 3:
                break;
            case 4:

                break;
            case 5:
                Animation anim2 = GameObject.Find("Boss").GetComponent<Animation>();
                anim2.PlayAnimation("Shout");
                break;
            case 6:
                if (!fightChani)
                {
                    MoveRabban();
                    Animation anim3 = GameObject.Find("Player_2").GetComponent<Animation>();
                    anim3.PlayAnimation("Walk");
                    Animation anim4 = GameObject.Find("Player_3").GetComponent<Animation>();
                    anim4.PlayAnimation("Walk");
                }
                if (fightChani) FightChani();
                break;
            case 7:
                GameObject player = GameObject.Find("Player");
                GameObject player2 = GameObject.Find("Player_2");
                Vector3 dir3 = player2.transform.globalPosition - player.transform.globalPosition;
                double angle3 = Math.Atan2(dir3.x, dir3.z);
                Quaternion rot3 = new Quaternion(0, (float)(1 * Math.Sin(angle3 / 2)), 0, (float)Math.Cos(angle3 / 2));
                player.GetComponent<Rigidbody>().SetBodyRotation(rot3);
                Animation anim5 = GameObject.Find("Player_2").GetComponent<Animation>();
                anim5.PlayAnimation("Ability1");
                Animation anim6 = GameObject.Find("Undistractable Enemy").GetComponent<Animation>();
                anim6.PlayAnimation("Die");
                Animation anim8 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim8.PlayAnimation("Idle");
                break;
            case 8:
                Animation anim7 = GameObject.Find("Player_2").GetComponent<Animation>();
                anim7.PlayAnimation("Talk");
                Animation anim11 = GameObject.Find("Player").GetComponent<Animation>();
                anim11.PlayAnimation("Talk");
                
                break;
            case 9:
                Animation anim10 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim10.PlayAnimation("Talk");
                break;
            case 10:
                Animation anim13 = GameObject.Find("Player").GetComponent<Animation>();
                anim13.PlayAnimation("Talk");
                break;
            case 11:
                Animation anim9 = GameObject.Find("Player_2").GetComponent<Animation>();
                anim9.PlayAnimation("Talk");
                Animation anim14 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim14.PlayAnimation("Talk");
                Animation anim12 = GameObject.Find("Player").GetComponent<Animation>();
                anim12.PlayAnimation("ArmMove");
                break;
            case 12:
                Animation anim15 = GameObject.Find("Player_2").GetComponent<Animation>();
                anim15.PlayAnimation("Talk");
                break;
            case 13:
                Animation anim16 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim16.PlayAnimation("Talk");
                break;
            case 14:
                Animation anim17 = GameObject.Find("Player").GetComponent<Animation>();
                anim17.PlayAnimation("ArmMove");
                break;
            case 15:
                Animation anim18 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim18.PlayAnimation("Talk");
                break;
            case 16:
                Animation anim19 = GameObject.Find("Player").GetComponent<Animation>();
                anim19.PlayAnimation("Celebrate");
                break;
            case 17:
                break;
            case 18:
                break;
            case 19:
                Animation anim20 = GameObject.Find("Player_3").GetComponent<Animation>();
                anim20.PlayAnimation("Talk");
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

    void MovePaul()
    {
        paulNavAgent.speed = 5;
        Animation anim = GameObject.Find("Player").GetComponent<Animation>();
        anim.PlayAnimation("Run");
    }
    
    void FightPaul()
    {
        paulNavAgent.speed = 0;
        paulNavAgent.ClearPath();
        Animation anim = GameObject.Find("Player").GetComponent<Animation>();
        anim.PlayAnimation("Ability1");
        
    }

    void HitPaul()
    {
        GameObject enemy = GameObject.Find("Undistractable Enemy");
        GameObject enemy2 = GameObject.Find("Undistractable Enemy 2");
        GameObject boss = GameObject.Find("Boss");
        GameObject player = GameObject.Find("Player");
        Vector3 dir = player.transform.globalPosition - enemy.transform.globalPosition;
        double angle = Math.Atan2(dir.x, dir.z);
        Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
        enemy.GetComponent<Rigidbody>().SetBodyRotation(rot);
        
        Vector3 dir2 = player.transform.globalPosition - enemy2.transform.globalPosition;
        double angle2 = Math.Atan2(dir2.x, dir2.z);
        Quaternion rot2 = new Quaternion(0, (float)(1 * Math.Sin(angle2 / 2)), 0, (float)Math.Cos(angle2 / 2));
        enemy2.GetComponent<Rigidbody>().SetBodyRotation(rot2);

        Vector3 dir3 = player.transform.globalPosition - boss.transform.globalPosition;
        double angle3 = Math.Atan2(dir3.x, dir3.z);
        Quaternion rot3 = new Quaternion(0, (float)(1 * Math.Sin(angle3 / 2)), 0, (float)Math.Cos(angle3 / 2));
        boss.GetComponent<Rigidbody>().SetBodyRotation(rot3);

        Animation anim2 = GameObject.Find("Undistractable Enemy 2").GetComponent<Animation>();
        anim2.PlayAnimation("RifleHit");
        Animation anim3 = GameObject.Find("Undistractable Enemy").GetComponent<Animation>();
        anim3.PlayAnimation("Aim");
        Animation anim4 = GameObject.Find("Boss").GetComponent<Animation>();
        anim4.PlayAnimation("Shout");
        Animation anim = GameObject.Find("Player").GetComponent<Animation>();
        anim.PlayAnimation("HeadHit");
        //Comienza linea 0
        dialogues.SetInMove(false);

    }
    void KneePaul()
    {
        Animation anim2 = GameObject.Find("Undistractable Enemy 2").GetComponent<Animation>();
        anim2.PlayAnimation("PutOnKnee");
        Animation anim = GameObject.Find("Player").GetComponent<Animation>();
        anim.PlayAnimation("FallKnee");
    }

    void MoveRabban()
    {
        GameObject.Find("Boss").GetComponent<NavAgent>().speed = 5;
        GameObject.Find("Undistractable Enemy 2").GetComponent<NavAgent>().speed = 5;
        Animation anim = GameObject.Find("Boss").GetComponent<Animation>();
        anim.PlayAnimation("WalkAngry");
    }

    void MoveChani()
    {
        GameObject.Find("Player_3").GetComponent<NavAgent>().speed = 10;
        Animation anim = GameObject.Find("Player_2").GetComponent<Animation>();
        anim.PlayAnimation("Walk");
        Animation anim2 = GameObject.Find("Player_3").GetComponent<Animation>();
        anim2.PlayAnimation("Walk");
        moveChani = true;
    }

    void FightChani()
    {
        //Acaba linea 6
        dialogues.SetInMove(false);
        GameObject.Find("Player_2").GetComponent<NavAgent>().ClearPath();
        GameObject.Find("Player_3").GetComponent<NavAgent>().speed = 0;
        GameObject.Find("Player_3").GetComponent<NavAgent>().ClearPath();
        Animation anim = GameObject.Find("Player_2").GetComponent<Animation>();
        anim.PlayAnimation("Idle");
        Animation anim2 = GameObject.Find("Player_3").GetComponent<Animation>();
        anim2.PlayAnimation("Idle");
        Animation anim3 = GameObject.Find("Undistractable Enemy").GetComponent<Animation>();
        anim3.PlayAnimation("Idle");
    }
}