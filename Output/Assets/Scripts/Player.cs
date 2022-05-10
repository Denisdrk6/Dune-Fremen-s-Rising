using System;
using RagnarEngine;

public class Player : RagnarComponent
{
    enum Movement
    {
        IDLE,
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
    GameObject sound;

    ParticleSystem walkPartSys;
    ParticleSystem runPartSys;
    ParticleSystem getHitPartSys;
    ParticleSystem deadPartSys;

    public bool controled = false;
    State abilityState = State.NONE;
    Actions action = Actions.NONE;
    Movement move = Movement.IDLE;

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
        dialogue = GameObject.Find("Dialogue").GetComponent<DialogueManager>();

        sound = InternalCalls.InstancePrefab("SoundArea");
        gameObject.AddChild(sound);

        // Asignation of particles depending of the character
        if (gameObject.name == "Player")
        {
            walkPartSys = GameObject.Find("WalkParticles").GetComponent<ParticleSystem>();
            runPartSys = GameObject.Find("RunParticles").GetComponent<ParticleSystem>();
            getHitPartSys = GameObject.Find("GetHitParticles").GetComponent<ParticleSystem>();
            deadPartSys = GameObject.Find("FallDeadParticles_1").GetComponent<ParticleSystem>();
        }
        else if (gameObject.name == "Player_2")
        {
            walkPartSys = GameObject.Find("WalkParticles_2").GetComponent<ParticleSystem>();
            runPartSys = GameObject.Find("RunParticles_2").GetComponent<ParticleSystem>();
            getHitPartSys = GameObject.Find("GetHitParticles_2").GetComponent<ParticleSystem>();
            deadPartSys = GameObject.Find("FallDeadParticles_2").GetComponent<ParticleSystem>();
        }
        else if (gameObject.name == "Player_3")
        {
            walkPartSys = GameObject.Find("WalkParticles_3").GetComponent<ParticleSystem>();
            runPartSys = GameObject.Find("RunParticles_3").GetComponent<ParticleSystem>();
            getHitPartSys = GameObject.Find("GetHitParticles_3").GetComponent<ParticleSystem>();
            deadPartSys = GameObject.Find("FallDeadParticles_3").GetComponent<ParticleSystem>();
        }
        getHitPartSys.Pause();

        ReloadState();
    }

    public void Update()
    {
        if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_DOWN)
            gameObject.GetComponent<AudioSource>().PlayClip("UI_SANDCLICK");

        if (!dialogue.GetInDialogue())
        {
            if (!dead)
            {
                if (hitPoints <= 0)
                {
                    dead = true;
                    Die();
                }

                if (controled)
                //if (controled && hitPoints > 0 && dialogue.GetInDialogue())
                {
                    // Crouch
                    if (Input.GetKey(KeyCode.LSHIFT) == KeyState.KEY_DOWN)
                    {
                        if (action == Actions.NONE)
                        {
                            action = Actions.CROUCH;
                            rb.SetHeight(0.6f); // 0.6 = 60%
                            ReloadState();
                        }
                        else if (action == Actions.CROUCH)
                        {
                            action = Actions.NONE;
                            rb.SetHeight(1); // 1 = 100% = Reset
                            ReloadState();
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
                            ReloadState();
                            //Play audio when calculating movement to not repeat the same audio
                            gameObject.GetComponent<AudioSource>().PlayClip("PAUL_WALKSAND");
                        }
                    }
                    else if (abilityState != State.NONE && agent.PathSize() > 0)
                    {
                        agent.ClearPath();
                        move = Movement.IDLE;
                        ReloadState();
                    }
                }
                if (agent.MovePath())
                {
                    move = Movement.IDLE;
                    ReloadState();
                }
                else if (agent.PathSize() > 0)
                    sound.transform.globalPosition = gameObject.transform.globalPosition;
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
                deadPartSys.Play();
                String name = "";
                if (gameObject.name == "Player") name = "Paul Atreides";
                else if (gameObject.name == "Player_2") name = "Chani";
                else if (gameObject.name == "Player_3") name = "Stilgar";
                GameObject.Find("EnemyManager").GetComponent<EnemyManager>().SaveTest(name, gameObject.transform.globalPosition);
                SceneManager.LoadScene("LoseScene");
                pendingToDelete = false;
                //InternalCalls.Destroy(gameObject);
            }

            //Reset After PostCast
            if (abilityState == State.POSTCAST)
                abilityState = State.NONE;
        }

        if (paused)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    private void PlayerPause()
    {
        agent.ClearPath();
        move = Movement.IDLE;
        action = Actions.NONE;
        ReloadState();
    }

    private void ReloadState()
    {
        switch (move)
        {
            case Movement.IDLE:
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
                sound.GetComponent<Rigidbody>().SetRadiusSphere(0f);
                gameObject.GetComponent<AudioSource>().StopCurrentClip("PAUL_WALKSAND");
                break;

            case Movement.WALK:
                switch (action)
                {
                    case Actions.NONE:
                        gameObject.GetComponent<Animation>().PlayAnimation("Walk");
                        sound.GetComponent<Rigidbody>().SetRadiusSphere(4f);
                        break;
                    case Actions.CROUCH:
                        gameObject.GetComponent<Animation>().PlayAnimation("CrouchWalk");
                        sound.GetComponent<Rigidbody>().SetRadiusSphere(0f);
                        break;
                    case Actions.CARRY:
                        gameObject.GetComponent<Animation>().PlayAnimation("CorpseWalk");
                        sound.GetComponent<Rigidbody>().SetRadiusSphere(6f);
                        break;
                }

                walkPartSys.Play();
                break;

            case Movement.RUN:
                switch (action)
                {
                    case Actions.NONE:
                        gameObject.GetComponent<Animation>().PlayAnimation("Run");
                        sound.GetComponent<Rigidbody>().SetRadiusSphere(8f);
                        break;
                    case Actions.CROUCH:
                        gameObject.GetComponent<Animation>().PlayAnimation("CrouchRun");
                        sound.GetComponent<Rigidbody>().SetRadiusSphere(2f);

                        //move = Movement.Walk;
                        //agent.speed = speedBase;
                        //ReloadState();
                        break;
                    case Actions.CARRY:
                        gameObject.GetComponent<Animation>().PlayAnimation("CorpseRun");
                        sound.GetComponent<Rigidbody>().SetRadiusSphere(10f);
                        break;
                }

                runPartSys.Play();
                break;
        }
    }

    private void Die()
    {
        gameObject.GetComponent<AudioSource>().PlayClip("PAUL_DEATH");
        gameObject.GetComponent<Animation>().PlayAnimation("Death");
        agent.ClearPath();
        walkPartSys.Pause();
        runPartSys.Pause();
        pendingToDelete = true;
        if (GameObject.Find("Knife") != null)
        {
            InternalCalls.Destroy(GameObject.Find("Knife"));
        }
        InternalCalls.Destroy(sound);
    }

    public void OnCollision(Rigidbody other)
    {
        if (other.gameObject.name == "Rocks")
            GetHit(1);
    }
    //public void OnTrigger(Rigidbody other)
    //{
    //}

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

        // Dialogues =========================================================
        if (other.gameObject.name == "DialogueTrigger0")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(0);
            PlayerPause();
        }
        if (other.gameObject.name == "DialogueTrigger3")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(3);
            PlayerPause();
        }
        if (other.gameObject.name == "DialogueTrigger5")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(5);
            PlayerPause();
        }
        if (other.gameObject.name == "DialogueTrigger6")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(6);
            PlayerPause();
        }
        if (other.gameObject.name == "DialogueTrigger9")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(9);
            PlayerPause();
        }
        if (other.gameObject.name == "DialogueTrigger10")
        {
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(10);
            PlayerPause();
        }
        // ===================================================================
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







