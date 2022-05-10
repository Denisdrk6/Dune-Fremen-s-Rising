using System;
using RagnarEngine;

public class Eagle : RagnarComponent
{
	public GameObject player;
	public PlayerManager playerManager;
    public bool controled = false;
    private float cooldown = -1f;
    Rigidbody goRB;
    ParticleSystem leftParticles;
    ParticleSystem rightParticles;
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
        goRB.IgnoreCollision(player, true);
        agent.CalculatePath(agent.hitPosition);

        leftParticles = GameObject.Find("LeftWingParticles").GetComponent<ParticleSystem>();
        rightParticles = GameObject.Find("RightWingParticles").GetComponent<ParticleSystem>();
        leftParticles.Play();
        rightParticles.Play();
    }
	public void Update()
	{
        agent.MovePath();
        if (((agent.hitPosition - gameObject.transform.globalPosition).magnitude < 3.0f) && !hasArrived)
        {
            leftParticles.Pause();
            rightParticles.Pause();
            hasArrived = true;
            GameObject sound = InternalCalls.InstancePrefab("SoundArea", true);
            sound.GetComponent<Rigidbody>().SetRadiusSphere(6f);
            sound.transform.globalPosition = gameObject.transform.globalPosition;
            sound.GetComponent<SoundAreaManager>().stablishedTimer = 6f;

            cooldown = 6f;
        }

        if (cooldown != -1f)
        {
            cooldown -= Time.deltaTime;
            if (cooldown < 0)
            {
                InternalCalls.Destroy(gameObject);
                leftParticles.Pause();
                rightParticles.Pause();
            }
        }
    }

}