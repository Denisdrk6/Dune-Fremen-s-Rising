using System;
using System.Collections.Generic;
using RagnarEngine;

public class UndistractableEnemy : RagnarComponent
{
    public int velocity = 1000;

    public NavAgent agents;
    public List<GameObject> waypoints;
    private int destPoint = 0;
    public EnemyState state;
    public EnemyType enemyType;

    public Vector3 initialPos;
    public Quaternion initialRot;

    // States
    public bool patrol;
    public bool stopState = false;

    // Timers
    public float stoppedTime = 0f;
    public float controlledCooldown = 10;

    // Player tracker
    public GameObject[] players;
    public GameObject[] colliders;
    private Vector3 offset;
    public int index = 0;

    // States
    public bool canShoot = true;
    public bool pendingToDelete = false;
    public bool controlled = false;
    public bool returning = false;
    public bool enterStunner = true;

    // Timers
    public float shootCooldown = 0f;
    public bool isDying = false;

    float initialSpeed;

    bool distracted = false;
    bool stunned = false;
    public bool backstab = false;
    
    public float distractedTimer = -1f;
    public float stunnedTimer = -1f;

    float coneTimer = 0.0f;
    int coneMaxTime = 1;
    private int radius = 12;
    private int angle = 60;

    Animation animation;
    Rigidbody rigidbody;
    AudioSource audioSource;

    GameObject[] childs;
    public ParticleSystem stunPartSys;
    public bool canLookOut = false;
    int retardedFrames;

    UIText buffCounter;
    float buffTemp;
    public GameObject circle;
    GameObject pointCharacter;
    Light pointerLight;
    public Light abilityLight;

    GameObject timerSlider;

    GameObject Ability1Bg;
    GameObject Ability2Bg;
    GameObject Ability3Bg;
    GameObject Ability4Bg;
    public void Start()
    {
        offset = gameObject.GetSizeAABB();
        animation = gameObject.GetComponent<Animation>();
        rigidbody = gameObject.GetComponent<Rigidbody>();

        if (state != EnemyState.DEATH)
        {
            animation.PlayAnimation("Walk");
            if (waypoints.Count != 0)
            {
                GotoNextPoint();
                patrol = false;
            }
            else
            {
                animation.PlayAnimation("Idle");
            }
        }

        initialSpeed = 6;

        childs = gameObject.childs;

        for (int i = 0; i < childs.Length; ++i)
        {
            if (childs[i].name == "StunParticles")
            {
                stunPartSys = childs[i].GetComponent<ParticleSystem>();
            }
            else if (childs[i].name == "StabParticles")
            {
                childs[i].GetComponent<ParticleSystem>().Pause();
            }
            else if (childs[i].name == "KnifeParticles")
            {
                childs[i].GetComponent<ParticleSystem>().Pause();
            }
            else if (childs[i].name == "SwordSlashParticles")
            {
                childs[i].GetComponent<ParticleSystem>().Pause();
            }
        }

        stunPartSys.Pause();
        retardedFrames = GameObject.Find("EnemyManager").GetComponent<EnemyManager>().retardedFrames;

        buffCounter = GameObject.Find("UIB").GetComponent<UIText>();

        pointCharacter = GameObject.Find("PlayerReminder").childs[3];
        pointerLight = pointCharacter.GetComponent<Light>();

        Ability1Bg = GameObject.Find("Ability1Bg");
        Ability2Bg = GameObject.Find("Ability2Bg");
        Ability3Bg = GameObject.Find("Ability3Bg");
        Ability4Bg = GameObject.Find("Ability4Bg");
    }

    public void OnCreation()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        agents = gameObject.GetComponent<NavAgent>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (state != EnemyState.DEATH && state != EnemyState.IS_DYING)
        {
            if (!controlled)
            {
                if (!pendingToDelete && !isDying && !stunned)
                {
                    if (returning)
                    {
                        agents.CalculatePath(initialPos);
                        if (agents.MovePath())
                        {
                            rigidbody.SetBodyRotation(initialRot);
                            returning = false;
                        }
                    }
                    if (!distracted && waypoints.Count != 0)
                    {
                        Patrol();
                    }
                    if (canLookOut)
                        LookOut(retardedFrames);
                }

                if (stunnedTimer >= 0)
                {
                    stunnedTimer -= Time.deltaTime;
                    if (stunnedTimer < 0)
                    {
                        stunPartSys.Pause();
                        if (waypoints.Count != 0)
                            animation.PlayAnimation("Walk");
                        else
                            animation.PlayAnimation("Idle");
                        stunned = false;
                        stunnedTimer = -1f;
                        enterStunner = true;
                    }
                }

                if (distractedTimer >= 0)
                {
                    distractedTimer -= Time.deltaTime;
                    if (distractedTimer < 0)
                    {
                        distracted = false;
                        distractedTimer = -1f;
                    }
                }
            }
            else
            {
                if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP && !backstab)
                {
                    if (agents.CalculatePath(agents.hitPosition).Length > 0)
                    {
                        pointCharacter.transform.globalPosition = agents.hitPosition;
                        pointerLight.intensity = 10;
                        animation.PlayAnimation("Walk");
                    }
                }
                if (agents.MovePath())
                {
                    pointerLight.intensity = 0;
                    animation.PlayAnimation("Idle");
                }

                if (!backstab && Input.GetKey(KeyCode.Z) == KeyState.KEY_REPEAT)
                {
                    backstab = true;
                    abilityLight.intensity = 1;
                }
                if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP && backstab)
                {
                    Debug.Log("BackStab enemy");
                    InternalCalls.InstancePrefab("BackStabEnemy", gameObject.transform.globalPosition);
                    abilityLight.intensity = 0;
                    backstab = false;
                }
                if (Input.GetMouseClick(MouseButton.RIGHT) == KeyState.KEY_DOWN && backstab)
                {
                    abilityLight.intensity = 0;
                    backstab = false;
                }
                buffTemp = controlledCooldown;
                buffTemp = (float)Math.Round((double)buffTemp, 0);
                buffCounter.text = buffTemp.ToString();

                Ability1Bg.GetComponent<UIImage>().SetImageGeneralColor(255, 0, 0);
                Ability2Bg.GetComponent<UIImage>().SetImageGeneralColor(255, 0, 0);
                Ability3Bg.GetComponent<UIImage>().SetImageGeneralColor(255, 0, 0);
                Ability4Bg.GetComponent<UIImage>().SetImageGeneralColor(255, 0, 0);
                GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility3 = false;
                GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility4 = false;

                if (Input.GetKey(KeyCode.ALPHA1) == KeyState.KEY_DOWN || (Input.GetKey(KeyCode.ALPHA2) == KeyState.KEY_DOWN && players.Length > 1) || (Input.GetKey(KeyCode.ALPHA3) == KeyState.KEY_DOWN && players.Length > 2))
                {
                    pointerLight.intensity = 0;
                    abilityLight.intensity = 0;
                    controlled = false;
                    returning = true;
                    gameObject.EraseChild(circle);
                    buffCounter.text = "";
                    GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility3 = true;
                    GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility4 = true;
                }

                controlledCooldown -= Time.deltaTime;
                if (controlledCooldown < 0)
                {
                    pointerLight.intensity = 0;
                    abilityLight.intensity = 0;
                    controlledCooldown = 0f;
                    buffCounter.text = "";
                    GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility3 = true;
                    GameObject.Find("PlayerManager").GetComponent<PlayerManager>().canDoAbility4 = true;
                    controlled = false;
                    gameObject.EraseChild(circle);
                    GameObject.Find("PlayerManager").GetComponent<PlayerManager>().ChangeCharacter(0);
                    if (waypoints.Count != 0) agents.CalculatePath(waypoints[destPoint].transform.globalPosition);
                    else returning = true;
                }
                else if (controlledCooldown < 3)
                {
                    float value = circle.GetComponent<Material>().emissiveColor.x;
                    circle.GetComponent<Material>().emissiveColor = new Vector3(Mathf.PingPongFloat(value, Time.deltaTime * 2, 1, 0.1f, false), 0, 0);
                }

                //Effect PointerCharacter
                if (pointerLight.intensity > 0)
                {
                    pointerLight.linear = Mathf.PingPongFloat(pointerLight.linear, Time.deltaTime / 5, -2.05f, -2.12f, true);
                }
            }
        }
        if (isDying)
        {
            state = EnemyState.IS_DYING;
            if (animation.HasFinished())
            {
                audioSource.PlayClip("EMALE_DEATH3");
                isDying = false;
                pendingToDelete = true;
            }
        }
    }

    public void SetControled(bool flag)
    {
        controlled = flag;
        if (flag){ 
            
            controlledCooldown = 10;
            timerSlider = InternalCalls.InstancePrefab("TimerP", gameObject.transform.globalPosition);
            timerSlider.GetComponent<TimerSlider>().getGa(gameObject, controlledCooldown, (int)enemyType, "controlledCooldown");
        }
    }

    public void OnCollision(Rigidbody other)
    {
        if (state != EnemyState.DEATH && state != EnemyState.IS_DYING)
        {
            if (other.gameObject.name == "Knife")
            {
                if (!isDying)
                {
                    isDying = true;
                    for (int i = 0; i < childs.Length; ++i)
                    {
                        if (childs[i].name == "KnifeParticles")
                        {
                            childs[i].GetComponent<ParticleSystem>().Play();
                            break;
                        }
                    }
                    animation.PlayAnimation("Dying");
                    QuestSystem system = GameObject.Find("Quest System").GetComponent<QuestSystem>();
                    system.hasKilledEnemies = true;
                    system.killWithPaul = true;
                    if (system.camouflageActive)
                        system.enemiesCamouflage++;
                }
            }
            if (other.gameObject.tag == "Player")
            {
                Distraction(other.gameObject.transform.globalPosition);
            }
            if (other.gameObject.name == "StunnerShot")
            {
                if (!isDying)
                {
                    audioSource.PlayClip("EBASIC_BULLETHIT");
                    isDying = true;
                    for (int i = 0; i < childs.Length; ++i)
                    {
                        if (childs[i].name == "KnifeParticles")
                        {
                            childs[i].GetComponent<ParticleSystem>().Play();
                            break;
                        }
                    }
                    animation.PlayAnimation("Dying");
                    QuestSystem system = GameObject.Find("Quest System").GetComponent<QuestSystem>();
                    system.hasKilledEnemies = true;
                    system.killWithStilgar = true;
                    if (system.camouflageActive)
                        system.enemiesCamouflage++;
                }
            }
            if (other.gameObject.name == "HunterSeeker")
            {
                if (!isDying)
                {
                    isDying = true;
                    for (int i = 0; i < childs.Length; ++i)
                    {
                        if (childs[i].name == "StabParticles")
                        {
                            childs[i].GetComponent<ParticleSystem>().Play();
                            break;
                        }
                    }
                    animation.PlayAnimation("Dying");
                    QuestSystem system = GameObject.Find("Quest System").GetComponent<QuestSystem>();
                    system.hasKilledEnemies = true;
                    system.killWithChani = true;
                    if (system.camouflageActive)
                        system.enemiesCamouflage++;
                }
            }
        }
    }

    public void OnTriggerEnter(Rigidbody other)
    {
        if (state != EnemyState.DEATH && state != EnemyState.IS_DYING)
        {
            //// Chani =======================================
            if (other.gameObject.name == "SpiceGrenade")
            {
                // STUN (BLIND)
                audioSource.PlayClip("EBASIC_SCREAM");
                Stun(5f);
                stunPartSys.Play();
                if (enterStunner)
                {
                    GameObject.Find("Quest System").GetComponent<QuestSystem>().enemiesGrenade++;
                    enterStunner = false;
                }
            }


            //// Stilgar =====================================
            if (other.gameObject.name == "SwordSlash")
            {
                audioSource.PlayClip("WPN_SWORDHIT");
                isDying = true;
                for (int i = 0; i < childs.Length; ++i)
                {
                    if (childs[i].name == "SwordSlashParticles")
                    {
                        childs[i].GetComponent<ParticleSystem>().Play();
                        break;
                    }
                }
                animation.PlayAnimation("Dying");
                QuestSystem system = GameObject.Find("Quest System").GetComponent<QuestSystem>();
                system.hasKilledEnemies = true;
                system.killWithStilgar = true;
                if (system.camouflageActive)
                    system.enemiesCamouflage++;
            }
            if (other.gameObject.name == "Trap")
            {
                // STUN (BLIND)
                audioSource.PlayClip("EBASIC_SCREAM");
                Stun(5f);
                GameObject.Find("ElectricParticles").GetComponent<ParticleSystem>().Play();
                stunPartSys.Play();
                GameObject.Find("Quest System").GetComponent<QuestSystem>().enemiesTrap++;
            }
        }
    }

    public void LookOut(int frames)
    {
        if (state != EnemyState.DEATH && state != EnemyState.IS_DYING && !controlled && !stunned)
        {
            if ((frames == retardedFrames) ? PerceptionCone() : PlayerIsNear())
            {
                coneTimer += Time.deltaTime * retardedFrames;
                if (coneTimer >= coneMaxTime)
                {
                    agents.speed = initialSpeed * 1.2f;
                    Shoot();
                }
            }
            else
            {
                agents.speed = initialSpeed;
                coneTimer -= Time.deltaTime * retardedFrames;
                if (coneTimer < 0) coneTimer = 0;
            }
            if (!canShoot && shootCooldown >= 0)
            {
                shootCooldown -= Time.deltaTime * retardedFrames;
                if (shootCooldown < 0)
                {
                    shootCooldown = 0f;
                    canShoot = true;
                }
            }
        }        
        canLookOut = false;
    }
    private bool PerceptionCone()
    {
        Vector3 enemyPos = gameObject.transform.globalPosition;
        Vector3 enemyForward = gameObject.transform.forward;
        Vector3 initPos = new Vector3(enemyPos.x + (enemyForward.x * offset.x * 0.6f), enemyPos.y + 0.1f, enemyPos.z + (enemyForward.z * offset.z * 0.6f));

        index = RayCast.PerceptionCone(initPos, enemyForward, 60, 10, 12, players, players.Length, "Collider", coneTimer / coneMaxTime);
        if (index != -1 && (players[index].GetComponent<Player>().invisible || players[index].GetComponent<Player>().dead || players[index].GetComponent<Player>().isHidden)) return false;
        return (index == -1) ? false : true;
    }

    public bool PlayerIsNear()
    {
        for (int i = 0; i < players.Length; i++)
        {
            Vector3 vecDir = players[i].transform.globalPosition - gameObject.transform.globalPosition;
            if ((vecDir).magnitude <= radius)
            {
                if (Transform.GetAngleBetween(gameObject.transform.forward, vecDir) <= angle * 0.5f)
                {
                    if (players[i].GetComponent<Player>().invisible || players[i].GetComponent<Player>().dead || players[i].GetComponent<Player>().isHidden)
                        return false;

                    if (RayCast.HitToTag(gameObject.transform.globalPosition, players[i].transform.globalPosition, "Player") != null)
                        return true;
                }
            }
        }
        return false;
    }

    private void Shoot()
    {
        audioSource.SetState("MUSIC", "LEVEL1_BATTLE");

        if (canShoot)
        {
            //TODO_AUDIO
            audioSource.PlayClip("EBASIC_SHOTGUN");
            //animation.PlayAnimation("Shoot");
            canShoot = false;
            shootCooldown = 1f;
            Vector3 pos = gameObject.transform.globalPosition;
            pos.y += 0.5f;

            GameObject bullet = InternalCalls.InstancePrefab("EnemyBullet", pos);
            bullet.GetComponent<Rigidbody>().IgnoreCollision(gameObject, true);
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            bulletScript.enemy = gameObject;
            bulletScript.index = index;
            bulletScript.offset = offset;
        }
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetPendingToDelete()
    {
        pendingToDelete = true;
    }

    public void GotoNextPoint()
    {
        //audioSource.PlayClip("FOOTSTEPS");
        animation.PlayAnimation("Walk");
        agents.CalculatePath(waypoints[destPoint].transform.globalPosition);
        destPoint = (destPoint + 1) % waypoints.Count;
    }

    public void Patrol()
    {
        if (agents.MovePath())
        {
            stopState = true;
        }

        if (stopState)
        {
            if (stoppedTime >= 0)
            {
                audioSource.StopCurrentClip("ETANK_WALKSAND");
                stoppedTime -= Time.deltaTime;
                if (stoppedTime < 0)
                {
                    stoppedTime = 0f;
                    stopState = false;
                    if (waypoints.Count != 0)
                    {
                        patrol = true;
                        GotoNextPoint();
                    }
                }
            }
        }

        if (agents.MovePath() && waypoints.Count != 0 && patrol && !stopState)
        {
            GotoNextPoint();
        }
    }

    public void Distraction(Vector3 distractionItem)
    {
        Vector3 newForward = (distractionItem - gameObject.transform.globalPosition).normalized;

        double angle = Math.Atan2(newForward.x, newForward.z);

        Quaternion newRot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));

        rigidbody.SetBodyRotation(newRot);

        animation.PlayAnimation("Idle");
    }

    public void Stun(float timeStunned)
    {
        stunned = true;
        stunnedTimer = timeStunned;
        animation.PlayAnimation("Stun");

        timerSlider = InternalCalls.InstancePrefab("TimerP", gameObject.transform.globalPosition);
        timerSlider.GetComponent<TimerSlider>().getGa(gameObject, stunnedTimer, (int)enemyType, "stunnedTimer");
    }
}