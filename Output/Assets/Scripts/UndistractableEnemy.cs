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
    float controlledCooldown = 10;

    // Player tracker
    public GameObject[] players;
    public GameObject[] colliders;
    GameObject SceneAudio;
    private Vector3 offset;
    public int index = 0;

    // States
    public bool canShoot = true;
    public bool pendingToDelete = false;
    public bool controlled = false;
    public bool returning = false;

    // Timers
    public float shootCooldown = 0f;
    float deathTimer = -1f;

    float initialSpeed;

    bool distracted = false;
    bool stunned = false;
    public bool backstab = false;
    
    float distractedTimer = -1f;
    float stunnedTimer = -1f;

    Animation animation;
    Rigidbody rigidbody;
    AudioSource audioSource;

    GameObject[] childs;
    ParticleSystem stunPartSys;
    public void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        SceneAudio = GameObject.Find("AudioLevel1");
        offset = gameObject.GetSizeAABB();

        agents = gameObject.GetComponent<NavAgent>();
        animation = gameObject.GetComponent<Animation>();

        rigidbody = gameObject.GetComponent<Rigidbody>();
        audioSource = gameObject.GetComponent<AudioSource>();

        if (state != EnemyState.DEATH)
        {
            animation.PlayAnimation("Idle");
            if (waypoints.Count != 0)
            {
                GotoNextPoint();
                patrol = false;
            }
        }

        initialSpeed = agents.speed;

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
    }

    public void Update()
    {
        if (state != EnemyState.DEATH)
        {
            if (!controlled)
            {
                if (!pendingToDelete && deathTimer == -1)
                {
                    if (!stunned)
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
                        if (PerceptionCone())
                        {
                            agents.speed = initialSpeed * 1.2f;
                            Shoot();
                        }
                        else
                        {
                            agents.speed = initialSpeed;
                        }
                    }
                }

                if (deathTimer >= 0)
                {
                    deathTimer -= Time.deltaTime;
                    if (deathTimer < 0)
                    {
                        audioSource.PlayClip("EMALE_DEATH3");
                        deathTimer = -1f;
                        pendingToDelete = true;
                    }
                }

                if (stunnedTimer >= 0)
                {
                    stunnedTimer -= Time.deltaTime;
                    if (stunnedTimer < 0)
                    {
                        stunPartSys.Pause();
                        stunned = false;
                        stunnedTimer = -1f;
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
                if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
                {
                    agents.CalculatePath(agents.hitPosition);

                }
                agents.MovePath();
                if (!backstab && Input.GetKey(KeyCode.Z) == KeyState.KEY_REPEAT)
                {
                    backstab = true;
                    //area de luz
                }
                if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_DOWN && backstab)
                {
                    Debug.Log("BackStab enemy");
                    InternalCalls.InstancePrefab("BackStabEnemy");
                    backstab = false;
                }
                if (Input.GetMouseClick(MouseButton.RIGHT) == KeyState.KEY_DOWN && backstab)
                {
                    backstab = false;
                }
                if (Input.GetKey(KeyCode.ALPHA1) == KeyState.KEY_DOWN || Input.GetKey(KeyCode.ALPHA2) == KeyState.KEY_DOWN || Input.GetKey(KeyCode.ALPHA3) == KeyState.KEY_DOWN)
                {
                    controlled = false;
                    returning = true;
                }
                controlledCooldown -= Time.deltaTime;
                if (controlledCooldown < 0)
                {
                    controlledCooldown = 0f;
                    controlled = false;
                    players[0].GetComponent<Player>().SetControled(true);
                    if (waypoints.Count != 0) agents.CalculatePath(waypoints[destPoint].transform.globalPosition);
                    else returning = true;
                }
            }
        }
    }
    public void SetControled(bool flag)
    {
        controlled = flag;
        if (flag) controlledCooldown = 10;
    }

    public void OnCollision(Rigidbody other)
    {
        if (state != EnemyState.DEATH)
        {
            if (other.gameObject.name == "Knife")
            {
                if (deathTimer == -1f)
                {
                    deathTimer = 4f;
                    for (int i = 0; i < childs.Length; ++i)
                    {
                        if (childs[i].name == "KnifeParticles")
                        {
                            childs[i].GetComponent<ParticleSystem>().Play();
                            break;
                        }
                    }
                    animation.PlayAnimation("Dying");
                }

                // WHEN RUNES FUNCTIONAL
                // deathTimer = 0f;
            }
            if (other.gameObject.name == "StunnerShot")
            {
                if (deathTimer == -1f)
                {
                    deathTimer = 2f;
                    for (int i = 0; i < childs.Length; ++i)
                    {
                        if (childs[i].name == "KnifeParticles")
                        {
                            childs[i].GetComponent<ParticleSystem>().Play();
                            break;
                        }
                    }
                    animation.PlayAnimation("Dying");
                }
            }
            if (other.gameObject.name == "HunterSeeker")
            {
                if (deathTimer == -1f)
                {
                    deathTimer = 5f;
                    animation.PlayAnimation("Dying");
                }

                // WHEN RUNES FUNCTIONAL
                // EXPLOSION AREA
            }
        }
    }

    public void OnTrigger(Rigidbody other)
    {
        if (state != EnemyState.DEATH)
        {
            //// Chani =======================================
            if (other.gameObject.name == "SpiceGrenade")
            {
                // STUN (BLIND)
                Stun(5f);
                stunPartSys.Play();
            }


            //// Stilgar =====================================
            if (other.gameObject.name == "SwordSlash")
            {
                deathTimer = 2f;
                for (int i = 0; i < childs.Length; ++i)
                {
                    if (childs[i].name == "SwordSlashParticles")
                    {
                        childs[i].GetComponent<ParticleSystem>().Play();
                        break;
                    }
                }
                animation.PlayAnimation("Dying");
            }
            if (other.gameObject.name == "Whistle")
            {
                // NEED TO CREATE FUNCTION TO INITIATE STOPPED TIME WHEN ARRIVES TO THE POSITION
                patrol = false;
                stoppedTime = 5f;
                agents.CalculatePath(other.gameObject.transform.globalPosition);

                // WHEN RUNES FUNCTIONAL
                // STUN (BLIND) 3s
            }
            if (other.gameObject.name == "Trap")
            {
                // STUN (BLIND)
                Stun(5f);
                GameObject.Find("ElectricParticles").GetComponent<ParticleSystem>().Play();
                stunPartSys.Play();
            }
        }
    }

    private bool PerceptionCone()
    {
        Vector3 enemyPos = gameObject.transform.globalPosition;
        Vector3 enemyForward = gameObject.transform.forward;
        Vector3 initPos = new Vector3(enemyPos.x + (enemyForward.x * offset.x * 0.6f), enemyPos.y + 0.1f, enemyPos.z + (enemyForward.z * offset.z * 0.6f));

        index = RayCast.PerceptionCone(initPos, enemyForward, 60, 10, 12, players, players.Length, "Collider");
        if (index != -1 && (players[index].GetComponent<Player>().invisible || players[index].GetComponent<Player>().dead || players[index].GetComponent<Player>().isHidden)) return false;
        return (index == -1) ? false : true;
    }

    private void Shoot()
    {
        SceneAudio.GetComponent<AudioSource>().SetState("MUSIC", "LEVEL1_BATTLE");

        if (canShoot)
        {
            //TODO_AUDIO
            audioSource.PlayClip("EBASIC_SHOTGUN");
            canShoot = false;
            shootCooldown = 4f;
            InternalCalls.InstancePrefab("EnemyBullet", true);
            EnemyBullet bulletScript = GameObject.Find("EnemyBullet").GetComponent<EnemyBullet>();
            bulletScript.enemy = gameObject;
            bulletScript.index = index;
            bulletScript.offset = offset;
        }

        if (!canShoot)
        {
            if (shootCooldown >= 0)
            {
                shootCooldown -= Time.deltaTime;
                if (shootCooldown < 0)
                {
                    shootCooldown = 0f;
                    canShoot = true;
                }
            }
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
        audioSource.PlayClip("FOOTSTEPS");
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
        animation.PlayAnimation("Idle");
    }
}