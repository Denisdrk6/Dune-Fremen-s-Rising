using System;
using RagnarEngine;

public class Player : RagnarComponent
{
    enum Movement
    {
        WALK,
        RUN
    }
    enum Actions
    {
        NONE,
        CROUCH,
        CARRY
    }

    public int hitPoints;
    public float force = 100;
    private bool pendingToDelete = false;
    public bool paused = false;
    public bool invisible = false;
    public bool dead = false;
    public bool isHidden = false;
    private float speedBase = 0;

    Rigidbody rb;
    Material materialComponent;
    NavAgent agent;
    DialogueManager dialogue;

    ParticleSystem walkPartSys;
    ParticleSystem runPartSys;
    ParticleSystem getHitPartSys;

    public bool controled = false;
    State abilityState = 0;
    Actions action = Actions.NONE;
    Movement move = Movement.WALK;

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
        speedBase = agent.speed;
        agent.ClearPath();
        gameObject.GetComponent<Animation>().PlayAnimation("Idle");
        dialogue = GameObject.Find("Dialogue").GetComponent<DialogueManager>();

        // Asignation of particles depending of the character
        if (gameObject.name == "Player")
        {
            walkPartSys = GameObject.Find("WalkParticles").GetComponent<ParticleSystem>();
            runPartSys = GameObject.Find("RunParticles").GetComponent<ParticleSystem>();
            getHitPartSys = GameObject.Find("GetHitParticles").GetComponent<ParticleSystem>();
        }
        else if (gameObject.name == "Player_2")
        {
            walkPartSys = GameObject.Find("WalkParticles_2").GetComponent<ParticleSystem>();
            runPartSys = GameObject.Find("RunParticles_2").GetComponent<ParticleSystem>();
            getHitPartSys = GameObject.Find("GetHitParticles_2").GetComponent<ParticleSystem>();
        }
        else if (gameObject.name == "Player_3")
        {
            walkPartSys = GameObject.Find("WalkParticles_3").GetComponent<ParticleSystem>();
            runPartSys = GameObject.Find("RunParticles_3").GetComponent<ParticleSystem>();
            getHitPartSys = GameObject.Find("GetHitParticles_3").GetComponent<ParticleSystem>();
        }
        runPartSys.Pause();
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
                // Crouch
                if (Input.GetKey(KeyCode.LSHIFT) == KeyState.KEY_DOWN)
                {
                    if (action == Actions.NONE)
                    {
                        action = Actions.CROUCH;
                        rb.SetHeight(0.6f); // 0.6 = 60%
                        gameObject.GetComponent<Animation>().PlayAnimation("Crouch");
                        gameObject.GetComponent<AudioSource>().PlayClip("PAUL_CROUCH");
                    }
                    else if (action == Actions.CROUCH)
                    {
                        action = Actions.NONE;
                        rb.SetHeight(1); // 1 = 100% = Reset
                        gameObject.GetComponent<Animation>().PlayAnimation("Idle");
                    }
                }

                // Run
                if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_DOWN)
                {
                    agent.speed = speedBase;
                    move = Movement.WALK;
                }
                else if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_TWICE)
                {
                    agent.speed *= 2;
                    move = Movement.RUN;
                }

                if (abilityState == State.NONE && Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
                {
                    if (agent.CalculatePath(agent.hitPosition).Length > 0)
                    {
                        switch (move)
                        {
                            case Movement.WALK:
                                switch (action)
                                {
                                    case Actions.NONE:
                                        gameObject.GetComponent<Animation>().PlayAnimation("Walk");
                                        walkPartSys.Play();
                                        break;
                                    case Actions.CROUCH:
                                        gameObject.GetComponent<Animation>().PlayAnimation("CrouchWalk");
                                        walkPartSys.Play();
                                        break;
                                    case Actions.CARRY:
                                        gameObject.GetComponent<Animation>().PlayAnimation("CorpseWalk");
                                        walkPartSys.Play();
                                        break;
                                }
                                break;

                            case Movement.RUN:
                                switch (action)
                                {
                                    case Actions.NONE:
                                        gameObject.GetComponent<Animation>().PlayAnimation("Run");
                                        runPartSys.Play();
                                        break;
                                    case Actions.CROUCH:
                                        gameObject.GetComponent<Animation>().PlayAnimation("CrouchRun");
                                        runPartSys.Play();
                                        break;
                                    case Actions.CARRY:
                                        gameObject.GetComponent<Animation>().PlayAnimation("CorpseRun");
                                        runPartSys.Play();
                                        break;
                                }
                                break;
                        }

                        gameObject.GetComponent<AudioSource>().PlayClip("PAUL_WALKSAND");
                    }
                }
                else if (abilityState != State.NONE && agent.PathSize() > 0)
                {
                    agent.ClearPath();
                    switch (action)
                    {
                        case Actions.NONE:
                            gameObject.GetComponent<Animation>().PlayAnimation("Idle");
                            break;
                        case Actions.CROUCH:
                            gameObject.GetComponent<Animation>().PlayAnimation("Crouch");
                            break;
                        case Actions.CARRY:
                            gameObject.GetComponent<Animation>().PlayAnimation("CorpseCarry");
                            break;
                    }
                    walkPartSys.Pause();
                    runPartSys.Pause();
                }
            }
            if (agent.MovePath())
            {
                switch (action)
                {
                    case Actions.NONE:
                        gameObject.GetComponent<Animation>().PlayAnimation("Idle");
                        break;
                    case Actions.CROUCH:
                        gameObject.GetComponent<Animation>().PlayAnimation("Crouch");
                        break;
                    case Actions.CARRY:
                        gameObject.GetComponent<Animation>().PlayAnimation("CorpseCarry");
                        break;
                }

                walkPartSys.Pause();
                runPartSys.Pause();
                gameObject.GetComponent<AudioSource>().StopCurrentClip("PAUL_WALKSAND");
            }
            if (action == Actions.CROUCH)
            {
                Vector3 maxPoint = gameObject.GetMaxAABB();
                maxPoint.y *= 0.6f;
                gameObject.SetSizeAABB(gameObject.GetMinAABB(), maxPoint);
            }

            ///////// SOUNDS /////////
            // Reload Sound
            if (Input.GetKey(KeyCode.R) == KeyState.KEY_DOWN)
            {
                gameObject.GetComponent<AudioSource>().PlayClip("WPN_RELOAD");
            }
            //////////////////////////

            //SaveTest File for Debugging
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

            //Reset After PostCast
            if (abilityState == State.POSTCAST)
                abilityState = State.NONE;
        }
        else
        {
            gameObject.GetComponent<Animation>().PlayAnimation("Idle");
            walkPartSys.Pause();
            runPartSys.Pause();
            gameObject.GetComponent<AudioSource>().StopCurrentClip("PAUL_WALKSAND");
        }

        if (paused)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    private void Die()
    {
        gameObject.GetComponent<AudioSource>().PlayClip("PAUL_DEATH");
        gameObject.GetComponent<Animation>().PlayAnimation("Death");
        walkPartSys.Pause();
        runPartSys.Pause();
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

    public void SetState(State var)
    {
        abilityState = var;
    }

    public void SetAction(int act)
    {
        action = (Actions)act;
    }

    public int GetAction()
    {
        return (int)action;
    }

    public void GetHit(int dmg)
    {
        gameObject.GetComponent<AudioSource>().PlayClip("EBASIC_BULLETHIT");
        hitPoints -= dmg;
        getHitPartSys.Play();
    }
}







