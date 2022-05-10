using System;
using RagnarEngine;

public class Eagle : RagnarComponent
{
	public GameObject player;
	public PlayerManager playerManager;
    public bool controled = false;
    private float cooldown = -1f;
    Rigidbody goRB;

    NavAgent agent;
    public void Start()
	{
        //gameObject.GetComponent<AudioSource>().PlayClip("WPN_EAGLEORDER");
        agent = gameObject.GetComponent<NavAgent>();
        controled = true;
        player = GameObject.Find("Player");
        goRB = gameObject.GetComponent<Rigidbody>();
        goRB.SetBodyPosition(player.transform.globalPosition);
        goRB.IgnoreCollision(player, true);
        agent.CalculatePath(agent.hitPosition);
    }
	public void Update()
	{
        if (agent.MovePath())
        {
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
            }
        }
    }

}