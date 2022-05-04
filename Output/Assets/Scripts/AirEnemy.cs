using System;
using RagnarEngine;

public class AirEnemy : RagnarComponent
{
    public int velocity = 1000;

    public NavAgent agents;
    public GameObject[] waypoints;
    private int destPoint = 0;
    public EnemyState state;

    // States
    public bool patrol;
    public bool stopState = false;
    private bool stay = false;

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
    public void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        SceneAudio = GameObject.Find("AudioLevel1");
        offset = gameObject.GetSizeAABB();

        agents = gameObject.GetComponent<NavAgent>();
        gameObject.GetComponent<Animation>().PlayAnimation("Idle");
        if (waypoints.Length != 0)
        {
            GotoNextPoint();
            patrol = false;
        }

        initialSpeed = agents.speed;
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
                        if (!distracted)
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
                deathTimer -= Time.deltaTime;
                if (deathTimer < 0)
                {
                    gameObject.GetComponent<AudioSource>().PlayClip("ENEMY1DEATH");
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
                deathTimer = 4f;
                gameObject.GetComponent<Animation>().PlayAnimation("Dying");

                // WHEN RUNES FUNCTIONAL
                // deathTimer = 0f;
            }
            if (other.gameObject.name == "StunnerShot")
            {
                deathTimer = 2f;
                gameObject.GetComponent<Animation>().PlayAnimation("Dying");
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
        if (state != EnemyState.DEATH)
        {
            //// Stilgar =====================================
            if (other.gameObject.name == "Trap")
            {
                pendingToDelete = true;
                gameObject.GetComponent<Animation>().PlayAnimation("Dying");
            }
        }
    }

    private bool PerceptionCone()
    {
        Vector3 enemyPos = gameObject.transform.globalPosition;
        Vector3 enemyForward = gameObject.transform.forward;
        Vector3 initPos = new Vector3(enemyPos.x + (enemyForward.x * offset.x * 0.6f), enemyPos.y + 0.1f, enemyPos.z + (enemyForward.z * offset.z * 0.6f));

        index = RayCast.PerceptionCone(initPos, enemyForward, 60, 10, 8, players, players.Length, colliders, colliders.Length);
        if (players[index].GetComponent<Player>().invisible || players[index].GetComponent<Player>().dead) return false;
        return (index == -1) ? false : true;
    }

    private void Shoot()
    {
        SceneAudio.GetComponent<AudioSource>().SetState("MUSIC", "LEVEL1_BATTLE");

        if (canShoot)
        {
            //TODO_AUDIO
            gameObject.GetComponent<AudioSource>().PlayClip("ENEMY1SHOOT");
            canShoot = false;
            shootCooldown = 4f;
            //GameObject bullet = InternalCalls.InstancePrefab("EnemyBullet", true);
            //bullet.GetComponent<EnemyBullet>().enemy = gameObject;
            //bullet.GetComponent<EnemyBullet>().index = index;
            //bullet.GetComponent<EnemyBullet>().offset = offset;

            InternalCalls.InstancePrefab("EnemyBullet", true);
            GameObject.Find("EnemyBullet").GetComponent<EnemyBullet>().enemy = gameObject;
            GameObject.Find("EnemyBullet").GetComponent<EnemyBullet>().index = index;
            GameObject.Find("EnemyBullet").GetComponent<EnemyBullet>().offset = offset;
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
        if(!stay)
        {
            if (waypoints.Length == 1)
            {
                stay = true;
                gameObject.GetComponent<Animation>().PlayAnimation("Idle");
            }
            gameObject.GetComponent<AudioSource>().PlayClip("FOOTSTEPS");
            gameObject.GetComponent<Animation>().PlayAnimation("Walk");
            agents.CalculatePath(waypoints[destPoint].transform.globalPosition);
            destPoint = (destPoint + 1) % waypoints.Length;
        }
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
                gameObject.GetComponent<AudioSource>().StopCurrentClip("FOOTSTEPS");
                stoppedTime -= Time.deltaTime;
                if (stoppedTime < 0)
                {
                    stoppedTime = 0f;
                    stopState = false;
                    if (waypoints.Length != 0)
                    {
                        patrol = true;
                        GotoNextPoint();
                    }
                }
            }
        }

        if (agents.MovePath() && waypoints.Length != 0 && patrol && !stopState)
        {
            GotoNextPoint();
        }
    }
    public void Distraction(Vector3 distractionItem)
    {
        Vector3 newForward = (distractionItem - gameObject.transform.globalPosition).normalized;

        double angle = Math.Atan2(newForward.x, newForward.z);

        Quaternion newRot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));

        gameObject.GetComponent<Rigidbody>().SetBodyRotation(newRot);

        gameObject.GetComponent<Animation>().PlayAnimation("Idle");
    }

    public void Stun(float timeStunned)
    {
        stunned = true;
        stunnedTimer = timeStunned;
        gameObject.GetComponent<Animation>().PlayAnimation("Idle");
    }
}