using System;
using System.Collections.Generic;
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

    // Components
    private AudioSource audioSourceComponent;
    private Animation animationComponent;


    public int hitPoints;
    public float force = 100;
    private bool pendingToDelete = false;
    public bool paused = false;
    public bool invisible = false;
    public bool dead = false;
    public bool isHidden = false;
    public bool godMode = false;
    public bool stunned = false;
    private float speedBase = 0;

    Rigidbody rb;
    Material materialComponent;
    NavAgent agent;
    DialogueManager dialogue;
    GameObject sound;
    SoundAreaManager soundManag;

    GameObject uiCrouch;

    ParticleSystem walkPartSys;
    ParticleSystem runPartSys;
    ParticleSystem getHitPartSys;
    ParticleSystem deadPartSys;

    public bool controled = false;
    State abilityState = State.NONE;
    Actions action = Actions.NONE;
    Movement move = Movement.IDLE;

    pauseMenuButton pause;

    GameObject sword;
    GameObject stunner;
    public GameObject pointCharacter;
    Light pointerLight;

    /*
    DialogueManager dialogue;
    dialogue = GameObject.Find("Dialogue").GetComponent<DialogueManager>();
    if (!dialogue.GetInDialogue()){}
    */
    public void Start()
    {
        // Get components
        audioSourceComponent = gameObject.GetComponent<AudioSource>();
        animationComponent = gameObject.GetComponent<Animation>();

        rb = gameObject.GetComponent<Rigidbody>();
        materialComponent = gameObject.GetComponent<Material>();
        agent = gameObject.GetComponent<NavAgent>();
        speedBase = agent.speed;
        agent.ClearPath();
        dialogue = GameObject.Find("Dialogue").GetComponent<DialogueManager>();

        sound = InternalCalls.InstancePrefab("SoundArea", gameObject.transform.globalPosition);
        rb.IgnoreCollision(sound, true);
        gameObject.AddChild(sound);
        soundManag = sound.GetComponent<SoundAreaManager>();

        uiCrouch = GameObject.Find("UICrouch");
        pointerLight = pointCharacter.GetComponent<Light>();
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
            sword = GameObject.Find("Sword");
            stunner = GameObject.Find("Stunner");
        }
        getHitPartSys.Pause();
        pause = GameObject.Find("Background").GetComponent<pauseMenuButton>();
        ReloadState();
    }

    public void Update()
    {
        if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_DOWN)
            audioSourceComponent.PlayClip("UI_SANDCLICK");

        if (Input.GetKey(KeyCode.G) == KeyState.KEY_DOWN)
        {
            agent.CalculatePath(gameObject.transform.globalPosition);
            invisible = !invisible;
            godMode = !godMode;
        }
        
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
                    if (godMode)
                    {
                        if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
                        {
                            gameObject.transform.globalPosition = agent.hitPosition;
                            rb.SetBodyPosition(agent.hitPosition);
                            sound.transform.globalPosition = gameObject.transform.globalPosition;
                        }
                    }
                    else if (!godMode)
                    {
                        // Crouch
                        if (Input.GetKey(KeyCode.LSHIFT) == KeyState.KEY_UP)
                        {
                            if (action == Actions.NONE)
                            {
                                action = Actions.CROUCH;
                                rb.SetHeight(0.6f); // 0.6 = 60%
                                ReloadState();
                                uiCrouch.isActive = true;
                            }
                            else if (action == Actions.CROUCH && isHidden == false)
                            {
                                action = Actions.NONE;
                                rb.SetHeight(1); // 1 = 100% = Reset
                                ReloadState();
                                uiCrouch.isActive = false;
                            }
                            rb.IgnoreCollision(sound, true);
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


                        if (abilityState == State.NONE && Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP && !stunned)
                        {
                            GameObject.Find("PlayerManager").GetComponent<PlayerManager>().buffered = false;
                            if (agent.CalculatePath(agent.hitPosition).Length > 0)
                            {
                                pointCharacter.transform.globalPosition = agent.hitPosition;
                                pointerLight.intensity = 10;
                                ReloadState();
                                //Play audio when calculating movement to not repeat the same audio
                                audioSourceComponent.PlayClip("PAUL_WALKSAND");
                            }
                        }
                        else if (abilityState != State.NONE && agent.PathSize() > 0)
                        {
                            agent.ClearPath();
                            move = Movement.IDLE;
                            pointerLight.intensity = 0;
                            ReloadState();
                        }
                    }
                }
                if (agent.MovePath())
                {
                    move = Movement.IDLE;
                    pointerLight.intensity = 0;
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
                audioSourceComponent.PlayClip("WPN_RELOAD");
            }
            //////////////////////////

            //Effect PointerCharacter
            if (pointerLight.intensity > 0)
            {
                pointerLight.linear = Mathf.PingPongFloat(pointerLight.linear, Time.deltaTime / 5, -2.05f, -2.12f, true);
            }

            //SaveTest File for Debugging
            if (pendingToDelete && (animationComponent.GetLoopTime() > animationComponent.GetDuration() - 1))
            {
                deadPartSys.Play();
            }

            if (pendingToDelete && animationComponent.HasFinished())
            {
                Input.RestoreDefaultCursor();
                //String name = "";
                //if (gameObject.name == "Player") name = "Paul Atreides";
                //else if (gameObject.name == "Player_2") name = "Chani";
                //else if (gameObject.name == "Player_3") name = "Stilgar";
                //GameObject.Find("EnemyManager").GetComponent<EnemyManager>().SaveTest(name, gameObject.transform.globalPosition);
                SceneManager.LoadScene("LoseScene");
                pendingToDelete = false;
                //InternalCalls.Destroy(gameObject);
            }

            //Reset After PostCast
            if (abilityState == State.POSTCAST)
                abilityState = State.NONE;
        }

        if (paused|| pause.abiltyfocused != 0)
            Time.timeScale = 0.0f;
            
        else
            Time.timeScale = 1.0f;
    }

    public void PlayerPause()
    {
        agent.ClearPath();
        move = Movement.IDLE;
        animationComponent.PlayAnimation("Talk");
        if (gameObject.name == "Player_3")
        {
            stunner.isActive = false;
            sword.isActive = false;
        }
    }

    private void ReloadState()
    {
        switch (move)
        {
            case Movement.IDLE:
                switch (action)
                {
                    case Actions.NONE:
                        animationComponent.PlayAnimation("Idle");
                        break;
                    case Actions.CROUCH:
                        animationComponent.PlayAnimation("Crouch");
                        break;
                    case Actions.CARRY:
                        animationComponent.PlayAnimation("CorpseCarry");
                        break;
                }

                walkPartSys.Pause();
                runPartSys.Pause();
                soundManag.UpdateRadius(0f);
                audioSourceComponent.StopCurrentClip("PAUL_WALKSAND");
                break;

            case Movement.WALK:
                switch (action)
                {
                    case Actions.NONE:
                        animationComponent.PlayAnimation("Walk");
                        soundManag.UpdateRadius(2f);
                        break;
                    case Actions.CROUCH:
                        animationComponent.PlayAnimation("CrouchWalk");
                        soundManag.UpdateRadius(0f);
                        break;
                    case Actions.CARRY:
                        animationComponent.PlayAnimation("CorpseWalk");
                        soundManag.UpdateRadius(3f);
                        break;
                }

                walkPartSys.Play();
                break;

            case Movement.RUN:
                switch (action)
                {
                    case Actions.NONE:
                        animationComponent.PlayAnimation("Run");
                        soundManag.UpdateRadius(5f);
                        break;
                    case Actions.CROUCH:
                        //gameObject.GetComponent<Animation>().PlayAnimation("CrouchRun");
                        //soundManag.UpdateRadius(2f);

                        move = Movement.WALK;
                        agent.speed = speedBase;
                        ReloadState();
                        break;
                    case Actions.CARRY:
                        animationComponent.PlayAnimation("CorpseRun");
                        soundManag.UpdateRadius(6f);
                        break;
                }

                runPartSys.Play();
                break;
        }

        if (gameObject.name == "Player_3")
        {
            stunner.isActive = false;
            sword.isActive = false;
        }
    }

    private void Die()
    {
        audioSourceComponent.PlayClip("PAUL_DEATH");
        animationComponent.PlayAnimation("Death");
        agent.ClearPath();
        walkPartSys.Pause();
        runPartSys.Pause();
        pendingToDelete = true;
        if (GameObject.Find("Knife") != null)
        {
            InternalCalls.Destroy(GameObject.Find("Knife"));
        }
        soundManag.UpdateRadius(0f);
        InternalCalls.Destroy(sound);
        if (gameObject.name == "Player_3")
        {
            stunner.isActive = false;
            sword.isActive = false;
        }
    }

    public void OnCollision(Rigidbody other)
    {
        if (other.gameObject.tag == "RockThrown")
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
            InternalCalls.Destroy(other.gameObject);
            return;
        }
        if (other.gameObject.tag == "Hidden" && isHidden == false)
        {
            //action = Actions.CROUCH;
            //rb.SetHeight(0.6f); // 0.6 = 60%
            //ReloadState();

            isHidden = true;
        }

        if (other.gameObject.name == "Trigger1")
        {
            GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility1 = true;
            PlayerPause();
            pause.SetFocusedAbility(1);
            other.gameObject.name = "www";
            return;
        }
        if (other.gameObject.name == "Trigger2")
        {
            GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility3 = true;
            PlayerPause();
            pause.SetFocusedAbility(3);
            other.gameObject.name = "www";
            return;
        }
        if (other.gameObject.name == "Trigger3")
        {
            GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility2 = true;
            PlayerPause();
            pause.SetFocusedAbility(2);
            other.gameObject.name = "www";
            return;
        }
        if (other.gameObject.name == "Mision1")
        {
            GameObject.Find("Quest System").GetComponent<QuestSystem>().doorsLevel = true;
            other.gameObject.name = "www";
            return;
        }
        // Dialogues =========================================================
        /*if (other.gameObject.name == "DialogueTrigger0")
        {
            if(!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(0);
        }*/
        if (other.gameObject.name == "DialogueTrigger3")
        {
            GameObject.Find("Quest System").GetComponent<QuestSystem>().levelFinished = true;
            Input.RestoreDefaultCursor();
            if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveCinematic("Cinematic_3");
        }
        if (other.gameObject.name == "DialogueTrigger5")
        {
            if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(3);
        }
        if (other.gameObject.name == "DialogueTrigger6")
        {
            GameObject.Find("Quest System").GetComponent<QuestSystem>().midLevel = true;
            if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(4);
        }
        //if (other.gameObject.name == "DialogueTrigger9")
        //{
        //    if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
        //        PlayerPause();
        //    other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(9);
        //}
        if (other.gameObject.name == "DialogueTrigger10")
        {
            GameObject.Find("Quest System").GetComponent<QuestSystem>().levelFinished = true;
            Input.RestoreDefaultCursor();
            if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveCinematic("Cinematic_5");
        }
        if (other.gameObject.name == "DialogueTrigger11")
        {
            if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(6);
        }
        if (other.gameObject.name == "DialogueTrigger12")
        {
            if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(7);
        }
        if (other.gameObject.name == "DialogueTrigger13")
        {
            if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(13);
        }
        if (other.gameObject.name == "DialogueTrigger14")
        {
            if (!other.gameObject.GetComponent<DialogueTrigger>().isUsed)
                PlayerPause();
            other.gameObject.GetComponent<DialogueTrigger>().ActiveDialoguebyID(14);
        }
        // Boss ==============================================================
        if (other.gameObject.name == "BossTrigger")
        {
            GameObject.Find("Boss").GetComponent<Boss>().SetBattle();
        }
        // ===================================================================
    }

    public void OnTriggerExit(Rigidbody other)
    {
        if (other.gameObject.tag == "Hidden" && isHidden == true)
        {
            //action = Actions.NONE;
            //rb.SetHeight(1); // 1 = 100% = Reset
            //ReloadState();

            isHidden = false;
        }
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

        GameObject.Find("Quest System").GetComponent<QuestSystem>().damageRecieved = true;
    }

    public void PlayAudioClip(string clip)
    {
        gameObject.GetComponent<AudioSource>().PlayClip(clip.ToString());
    }
}







