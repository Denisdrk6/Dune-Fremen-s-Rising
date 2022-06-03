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
    private Vector3 offset;
    public int index = 0;

    // States
    public bool canShoot = true;
    public bool pendingToDelete = false;
    public bool controlled = false;
    public bool returning = false;

    // Timers
    public float shootCooldown = 0f;
    public bool isDying = false;

    float initialSpeed;

    bool distracted = false;
    bool stunned = false;
    public bool backstab = false;
    
    float distractedTimer = -1f;
    float stunnedTimer = -1f;

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

    public void Start()
    {
        offset = gameObject.GetSizeAABB();
        animation = gameObject.GetComponent<Animation>();
        rigidbody = gameObject.GetComponent<Rigidbody>();

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
        retardedFrames = GameObject.Find("EnemyManager").GetComponent<EnemyManager>().retardedFrames;
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
                    InternalCalls.InstancePrefab("BackStabEnemy", gameObject.transform.globalPosition);
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
        if (flag) controlledCooldown = 10;
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
                }
            }
            if (other.gameObject.name == "HunterSeeker")
            {
                if (!isDying)
                {
                    isDying = true;
                    animation.PlayAnimation("Dying");
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
            }
            if (other.gameObject.name == "Trap")
            {
                // STUN (BLIND)
                audioSource.PlayClip("EBASIC_SCREAM");
                Stun(5f);
                GameObject.Find("ElectricParticles").GetComponent<ParticleSystem>().Play();
                stunPartSys.Play();
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