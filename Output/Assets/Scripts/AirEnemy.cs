using System;
using System.Collections.Generic;
using RagnarEngine;

public class AirEnemy : RagnarComponent
{
    // Components

    private AudioSource audioComponent;
    private Animation animationComponent;
    private ParticleSystem particleComponent;
    private EnemyBullet bulletScript;
    private Rigidbody rb;

    /////////////

    public int velocity = 1000;

    public NavAgent agents;
    public List<GameObject> waypoints;
    private int destPoint = 0;
    public EnemyState state;
    public EnemyType enemyType;

    // States
    public bool patrol;
    public bool stopState = false;

    // Timers
    public float stoppedTime = 0f;

    // Player tracker
    public GameObject[] players;
    public GameObject[] colliders;
    GameObject SceneAudio;
    private Vector3 offset;
    public int index = 0;

    // States
    public bool canShoot = true;
    public bool pendingToDelete = false;

    // Timers
    public float shootCooldown = 0f;
    float deathTimer = -1f;

    float initialSpeed;

    bool distracted = false;
    float distractedTimer = -1f;
    bool stunned = false;
    float stunnedTimer = -1f;

    GameObject[] childs;
    ParticleSystem deathPartSys;
    public void Start()
    {
        // Get components
        audioComponent = gameObject.GetComponent<AudioSource>();
        animationComponent = gameObject.GetComponent<Animation>();
        particleComponent = gameObject.GetComponent<ParticleSystem>();
        rb = gameObject.GetComponent<Rigidbody>();
 
        players = GameObject.FindGameObjectsWithTag("Player");
        SceneAudio = GameObject.Find("AudioLevel1");
        offset = gameObject.GetSizeAABB();

        agents = gameObject.GetComponent<NavAgent>();

        if (state != EnemyState.DEATH)
        {
            gameObject.GetComponent<Animation>().PlayAnimation("Idle");
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
            if (childs[i].name == "DronDestParticles")
            {
                deathPartSys = childs[i].GetComponent<ParticleSystem>();
                break;
            }
        }

        deathPartSys.Pause();
    }

    public void Update()
    {
        if(state != EnemyState.DEATH)
        {
            if (!pendingToDelete && deathTimer == -1)
            {
                if (!pendingToDelete && deathTimer == -1)
                {
                    if (!stunned)
                    {
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
            }

            if (deathTimer >= 0)
            {
                state = EnemyState.IS_DYING;
                deathTimer -= Time.deltaTime;
                if (deathTimer < 0)
                {
                    audioComponent.PlayClip("EDRONE_DESTROYED");
                    deathTimer = -1f;
                    pendingToDelete = true;
                }
            }

            if (stunnedTimer >= 0)
            {
                stunnedTimer -= Time.deltaTime;
                if (stunnedTimer < 0)
                {
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
    }

    public void OnCollision(Rigidbody other)
    {
        if (state != EnemyState.DEATH)
        {
            if (other.gameObject.name == "Knife")
            {
                if (deathTimer == -1f)
                {
                    audioComponent.PlayClip("EDRONE_GETDAMAGE");
                    deathTimer = 4f;
                    deathPartSys.Play();
                    animationComponent.PlayAnimation("Dying");
                }

                // WHEN RUNES FUNCTIONAL
                // deathTimer = 0f;
            }
            if (other.gameObject.name == "StunnerShot")
            {
                if (deathTimer == -1f)
                {
                    audioComponent.PlayClip("EDRONE_GETDAMAGE");
                    deathTimer = 2f;
                    deathPartSys.Play();
                    animationComponent.PlayAnimation("Dying");
                }
            }
            if (other.gameObject.name == "HunterSeeker")
            {
                // WHEN RUNES FUNCTIONAL

            }
            // EXPLOSION AREA
        }
    }

    public void OnTrigger(Rigidbody other)
    {
        if (state != EnemyState.DEATH || state != EnemyState.IS_DYING)
        {
            //// Stilgar =====================================
            if (other.gameObject.name == "Trap")
            {
                if (pendingToDelete == false)
                {
                    audioComponent.PlayClip("EDRONE_GETDAMAGE");
                    pendingToDelete = true;
                    deathPartSys.Play();
                    GameObject.Find("ElectricParticles").GetComponent<ParticleSystem>().Play();
                    animationComponent.PlayAnimation("Dying");
                }
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
            audioComponent.PlayClip("EDRONE_SHOOT");
            canShoot = false;
            shootCooldown = 4f;

            Vector3 pos = gameObject.transform.globalPosition;
            pos.y += 0.5f;
            GameObject bullet = InternalCalls.InstancePrefab("EnemyBullet", pos, true);
            bullet.GetComponent<Rigidbody>().IgnoreCollision(gameObject, true);
            bulletScript = bullet.GetComponent<EnemyBullet>();
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
        //gameObject.GetComponent<AudioSource>().PlayClip("EDRONE_FLYING");
        animationComponent.PlayAnimation("Walk");
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
                audioComponent.StopCurrentClip("EDRONE_FLYING");
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

        rb.SetBodyRotation(newRot);

        animationComponent.PlayAnimation("Idle");
    }

    public void Stun(float timeStunned)
    {
        stunned = true;
        stunnedTimer = timeStunned;
        animationComponent.PlayAnimation("Idle");
    }
}