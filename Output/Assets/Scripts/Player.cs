using System;
using RagnarEngine;

public class Player : RagnarComponent
{
    public int hitPoints;
    public float force = 100;
    private bool pendingToDelete = false;
    public bool paused = false;
    private bool crouched = false;
    public bool invisible = false;
    private bool firstTime = false;
    public bool dead = false;
    public GameObject pickedEnemy = null;
    public bool isHidden = false;

    Rigidbody rb;
    Material materialComponent;
    NavAgent agent;
    DialogueManager dialogue;

    ParticleSystem walkPartSys;

    public bool controled = false;
    int state = 0;

    /*
    DialogueManager dialogue;
    dialogue = GameObject.Find("Dialogue").GetComponent<DialogueManager>();
    if (!dialogue.GetInDialogue()){}
    */
    public void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        materialComponent = gameObject.GetComponent<Material>();
        agent = gameObject.GetComponent<NavAgent>();
        gameObject.GetComponent<Animation>().PlayAnimation("Idle");
        dialogue = GameObject.Find("Dialogue").GetComponent<DialogueManager>();

        if (gameObject.name == "Player") walkPartSys = GameObject.Find("WalkParticles").GetComponent<ParticleSystem>();
        else if (gameObject.name == "Player_2") walkPartSys = GameObject.Find("WalkParticles_2").GetComponent<ParticleSystem>();
        else if (gameObject.name == "Player_3") walkPartSys = GameObject.Find("WalkParticles_3").GetComponent<ParticleSystem>();
        walkPartSys.Pause();
    }

    public void Update()
    {
        if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_DOWN)
            gameObject.GetComponent<AudioSource>().PlayClip("UI_SANDCLICK");

        if (!dialogue.GetInDialogue())
        {
            if (hitPoints <= 0 && !dead)
            {
                dead = true;
                Die();
            }

            if (controled && hitPoints > 0)
            //if (controled && hitPoints > 0 && dialogue.GetInDialogue())
            {
                if (state == (int)State.NONE && Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
                {
                    if (agent.CalculatePath(agent.hitPosition).Length > 0)
                    {
                        gameObject.GetComponent<Animation>().PlayAnimation("Walk");
                        walkPartSys.Play();
                    }

                    if (firstTime)
                    {
                        //gameObject.GetComponent<AudioSource>().PlayClip("PAUL_WALKSAND");
                    }
                    else
                    {
                        firstTime = true;
                    }
                }

                // Crouch
                switch(Input.GetKey(KeyCode.LSHIFT))
                {
                    case KeyState.KEY_DOWN:
                        {
                            gameObject.GetComponent<AudioSource>().PlayClip("PAUL_CROUCH");
                            crouched = true;
                            gameObject.GetComponent<Animation>().PlayAnimation("Crouch");
                            rb.SetHeight(0.6f); // 0.6 = 60%
                            break;
                        }
                    case KeyState.KEY_REPEAT:
                        {
                            Vector3 maxPoint = gameObject.GetMaxAABB();
                            maxPoint.y *= 0.6f;
                            gameObject.SetSizeAABB(gameObject.GetMinAABB(), maxPoint);
                            break;
                        }
                    case KeyState.KEY_UP:
                        {
                            crouched = false;
                            gameObject.GetComponent<Animation>().PlayAnimation("Idle");
                            rb.SetHeight(1); // 1 = 100% = Reset
                            break;
                        }
                }
            }
            if (state == (int)State.ABILITY_1 || state == (int)State.ABILITY_2 || state == (int)State.ABILITY_3 || state == (int)State.ABILITY_4 || state == (int)State.CARRYING)
            {
                agent.CalculatePath(new Vector3(gameObject.transform.globalPosition.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z));
                agent.CalculatePath(agent.hitPosition);
                if (crouched)
                {
                    gameObject.GetComponent<Animation>().PlayAnimation("CrouchWalk");
                    walkPartSys.Play();
                }
                else
                {
                    gameObject.GetComponent<Animation>().PlayAnimation("Walk");
                    walkPartSys.Play();
                }
                //gameObject.GetComponent<AudioSource>().PlayClip("FOOTSTEPS");
            }
            if (agent.MovePath())
            {
                gameObject.GetComponent<Animation>().PlayAnimation("Idle");
                walkPartSys.Pause();

                gameObject.GetComponent<AudioSource>().StopCurrentClip("PAUL_WALKSAND");
            }

            ///////// SOUNDS /////////
            // Reload Sound
            if (Input.GetKey(KeyCode.R) == KeyState.KEY_DOWN)
            {
                gameObject.GetComponent<AudioSource>().PlayClip("WPN_RELOAD");
            }
            //////////////////////////

            if (pendingToDelete && gameObject.GetComponent<Animation>().HasFinished())
            {
                String name = "";
                if (gameObject.name == "Player") name = "Paul Atreides";
                else if (gameObject.name == "Player_2") name = "Chani";
                else if (gameObject.name == "Player_3") name = "Stilgar";
                GameObject.Find("EnemyManager").GetComponent<EnemyManager>().SaveTest(name, gameObject.transform.globalPosition);
                SceneManager.LoadScene("LoseScene");
                //InternalCalls.Destroy(gameObject);
            }

            if (state == (int)State.POSTCAST)
            {
                state = (int)State.NONE;
            }

        }
        else
        {
            gameObject.GetComponent<Animation>().PlayAnimation("Idle");
            walkPartSys.Pause();

            gameObject.GetComponent<AudioSource>().StopCurrentClip("PAUL_WALKSAND");
        }

        if (paused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
            
        }
    }

    private void Die()
    {
        gameObject.GetComponent<AudioSource>().PlayClip("PAUL_DEATH");
        gameObject.GetComponent<Animation>().PlayAnimation("Death");
        walkPartSys.Pause();
        pendingToDelete = true;
        if (GameObject.Find("Knife") != null)
        {
            InternalCalls.Destroy(GameObject.Find("Knife"));
        }
    }
    public void OnCollision(Rigidbody other)
    {
        if (other.gameObject.name == "Rocks")
            GetHit(1);
    }
    public void OnTrigger(Rigidbody other)
    {
        if (other.gameObject.name == "DialogueTrigger0")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(0);
        }
        if (other.gameObject.name == "DialogueTrigger3")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(3);
        }
        if (other.gameObject.name == "DialogueTrigger5")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(5);
        }
        if (other.gameObject.name == "DialogueTrigger6")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(6);
        }
        if (other.gameObject.name == "DialogueTrigger9")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(9);
        }
        if (other.gameObject.name == "DialogueTrigger10")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(10);
        }
    }

    public void OnTriggerEnter(Rigidbody other)
    {
        if (other.gameObject.tag == "CheckPoint")
        {
            SaveSystem.SaveScene();
            GameObject.Find("PlayerManager").GetComponent<PlayerManager>().SavePlayer();
            GameObject.Find("EnemyManager").GetComponent<EnemyManager>().SaveEnemies();
        }
        if (other.gameObject.tag == "Hidde")
            isHidden = true;
    }

    public void OnTriggerExit(Rigidbody other)
    {
        if (other.gameObject.tag == "Hidde")
            isHidden = false;
    }

    public void SetControled(bool var)
    {
        controled = var;
    }

    public void SetState(int var)
    {
        state = var;
    }

    public void GetHit(int dmg)
    {
        gameObject.GetComponent<AudioSource>().PlayClip("EBASIC_BULLETHIT");
        hitPoints -= dmg;
    }
}







