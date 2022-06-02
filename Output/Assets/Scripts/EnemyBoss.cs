using System;
using RagnarEngine;

public class EnemyBoss : RagnarComponent
{
    // Components
    private Animation animationComponent;
    private AudioSource audioComponent;
    private Rigidbody rb;

    ////////
    
    public int velocity = 1000;

    public NavAgent agents;
    public GameObject[] waypoints = new GameObject[4];
    private int destPoint = 0;
    public EnemyState state;

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
    public bool controlled = false;

    // Timers
    public float shootCooldown = 0f;
    float deathTimer = -1f;
    float controlledCooldown = 10;

    float initialSpeed;

    bool distracted = false;
    float distractedTimer = -1f;
    bool stunned = false;
    float stunnedTimer = -1f;

    float coneTimer = 0.0f;
    int coneMaxTime = 4;

    public void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        SceneAudio = GameObject.Find("AudioLevel1");
        offset = gameObject.GetSizeAABB();

        agents = gameObject.GetComponent<NavAgent>();

        // Get components
        animationComponent = gameObject.GetComponent<Animation>();
        audioComponent = gameObject.GetComponent<AudioSource>();
        rb = gameObject.GetComponent<Rigidbody>();

        animationComponent.PlayAnimation("Idle");

        waypoints[0] = GameObject.Find("23");
        waypoints[1] = GameObject.Find("24");
        waypoints[2] = GameObject.Find("25");
        waypoints[3] = GameObject.Find("26");

        if (waypoints.Length != 0)
        {
            GotoNextPoint();
            patrol = false;
        }

        initialSpeed = agents.speed;
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
                        if (!distracted)
                        {
                            Patrol();
                        }
                        if (PerceptionCone())
                        {
                            coneTimer += Time.deltaTime;

                            if (coneTimer >= coneMaxTime)
                            {
                                agents.speed = initialSpeed * 1.2f;
                                Shoot();
                            }
                        }
                        else
                        {
                            agents.speed = initialSpeed;
                            coneTimer -= Time.deltaTime;
                        }
                    }
                }

                if (deathTimer >= 0)
                {
                    deathTimer -= Time.deltaTime;
                    if (deathTimer < 0)
                    {
                        audioComponent.PlayClip("EBASIC_SCREAM");
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
            else
            {
                if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
                {
                    agents.CalculatePath(agents.hitPosition);

                }
                agents.MovePath();
                controlledCooldown -= Time.deltaTime;
                if (controlledCooldown < 0)
                {
                    controlledCooldown = 0f;
                    controlled = false;
                    players[0].GetComponent<Player>().SetControled(true);
                    agents.CalculatePath(waypoints[destPoint].transform.globalPosition);
                }

            }
        }
    }

    public void OnCollision(Rigidbody other)
    {
        if (state != EnemyState.DEATH)
        {
            //gameObject.GetComponent<AudioSource>().PlayClip("EBASIC_SCREAM");
            if (other.gameObject.name == "Knife")
            {
                deathTimer = 4f;
                animationComponent.PlayAnimation("Dying");

                // WHEN RUNES FUNCTIONAL
                // deathTimer = 0f;
            }
            if (other.gameObject.name == "StunnerShot")
            {
                audioComponent.PlayClip("EBASIC_BULLETHIT");
                deathTimer = 2f;
                animationComponent.PlayAnimation("Dying");
            }
            if (other.gameObject.name == "HunterSeeker")
            {
                deathTimer = 5f;
                animationComponent.PlayAnimation("Dying");

                // WHEN RUNES FUNCTIONAL
                // EXPLOSION AREA
            }
        }
    }

    public void OnTrigger(Rigidbody other)
    {
        if (state != EnemyState.DEATH)
        {
            //// Paul ========================================
            if (other.gameObject.name == "Rock")
            {
                // DISTRACTION (ROTATE VISION, NO MOVEMENT TO THE DISTRACTION)
                distracted = true;
                distractedTimer = 5f;
                Distraction(other.gameObject.transform.globalPosition);
            }
            if (other.gameObject.name == "Eagle")
            {
                // DISTRACTION (ROTATE VISION, NO MOVEMENT TO THE DISTRACTION)
                distracted = true;
                distractedTimer = 6f;
                Distraction(other.gameObject.transform.globalPosition);
            }

            //// Chani =======================================
            if (other.gameObject.name == "SpiceGrenade")
            {
                // STUN (BLIND)
                audioComponent.PlayClip("EBASIC_SCREAM");
                Stun(5f);
            }


            //// Stilgar =====================================
            if (other.gameObject.name == "SwordSlash")
            {
                audioComponent.PlayClip("WPN_SWORDHIT");
                deathTimer = 2f;
                animationComponent.PlayAnimation("Dying");
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
                audioComponent.PlayClip("EBASIC_SCREAM");
                Stun(5f);
            }
        }
    }

    private bool PerceptionCone()
    {
        Vector3 enemyPos = gameObject.transform.globalPosition;
        Vector3 enemyForward = gameObject.transform.forward;
        Vector3 initPos = new Vector3(enemyPos.x + (enemyForward.x * offset.x * 0.6f), enemyPos.y + 0.1f, enemyPos.z + (enemyForward.z * offset.z * 0.6f));

        index = RayCast.PerceptionCone(initPos, enemyForward, 60, 10, 8, players, players.Length, "Collider", Time.deltaTime);
        if (players[index].GetComponent<Player>().invisible || players[index].GetComponent<Player>().dead) return false;
        return (index == -1) ? false : true;
    }
    public void SetControled(bool flag)
    {
        controlled = flag;
        if (flag) controlledCooldown = 10;
    }

    private void Shoot()
    {
        SceneAudio.GetComponent<AudioSource>().SetState("MUSIC", "LEVEL1_BATTLE");

        if (canShoot)
        {
            //TODO_AUDIO
            audioComponent.PlayClip("EBASIC_SHOTGUN");
            canShoot = false;
            shootCooldown = 4f;
            
            Vector3 pos = gameObject.transform.globalPosition;
            pos.y += 0.5f;
            pos.x += gameObject.transform.forward.x * offset.x * 0.6f;
            pos.z += gameObject.transform.forward.z * offset.z * 0.6f;
            EnemyBullet bulletScript = InternalCalls.InstancePrefab("EnemyBullet", pos).GetComponent<EnemyBullet>();
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
        //gameObject.GetComponent<AudioSource>().PlayClip("EBASIC_WALKSAND");
        animationComponent.PlayAnimation("Walk");
        agents.CalculatePath(waypoints[destPoint].transform.globalPosition);
        destPoint = (destPoint + 1) % waypoints.Length;
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
               audioComponent.StopCurrentClip("EBASIC_WALKSAND");
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