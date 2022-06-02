using System;
using RagnarEngine;

public class Eagle : RagnarComponent
{
	public GameObject player;
	public PlayerManager playerManager;
    public bool controled = false;
    private float cooldown = 6f;
    private bool pendingToDelete = false;
    Rigidbody goRB;
    //ParticleSystem leftParticles;
    //ParticleSystem rightParticles;
    bool hasArrived = false;
    NavAgent agent;
    public void Start()
	{
        //gameObject.GetComponent<AudioSource>().PlayClip("WPN_EAGLEORDER");
        goRB = gameObject.GetComponent<Rigidbody>();
        agent = gameObject.GetComponent<NavAgent>();
        controled = true;
        player = GameObject.Find("Player");
        Vector3 pos = player.transform.globalPosition + new Vector3(0, 4, 0);
        gameObject.transform.globalPosition = pos;

        Vector3 newForward = agent.hitPosition - pos;
        double angle = Math.Atan2(newForward.x, newForward.z);
        Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
        goRB.SetBodyRotation(rot);
        goRB.SetBodyPosition(pos);
        for (int i = 0; i < GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters.Length; i++)
        {
            goRB.IgnoreCollision(GameObject.Find("PlayerManager").GetComponent<PlayerManager>().players[i], true);
        }
        agent.CalculatePath(agent.hitPosition);

        player.GetComponent<Player>().PlayAudioClip("EBOSS_THROWOBJECT");
        //leftParticles = GameObject.Find("LeftWingParticles").GetComponent<ParticleSystem>();
        //rightParticles = GameObject.Find("RightWingParticles").GetComponent<ParticleSystem>();
        //leftParticles.Play();
        //rightParticles.Play();

        gameObject.GetComponent<Animation>().PlayAnimation("Idle");
        gameObject.isInteractuable = false;
    }
	public void Update()
	{
        agent.MovePath();
        if (((agent.hitPosition - gameObject.transform.globalPosition).magnitude < 4.0f) && !hasArrived)
        {
            //leftParticles.Pause();
            //rightParticles.Pause();
            hasArrived = true;
            GameObject sound = InternalCalls.InstancePrefab("SoundArea", gameObject.transform.globalPosition);
            sound.GetComponent<Rigidbody>().SetRadiusSphere(6.7f);
            //sound.transform.globalPosition = gameObject.transform.globalPosition;
            sound.GetComponent<SoundAreaManager>().stablishedTimer = 6f;

            cooldown = 6f;
            pendingToDelete = true;
        }

        if (pendingToDelete)
        {
            cooldown -= Time.deltaTime;
            if (cooldown < 0)
            {
                //leftParticles.Pause();
                //rightParticles.Pause();
                InternalCalls.Destroy(gameObject);
            }
        }
    }

}