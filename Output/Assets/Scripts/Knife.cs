using System;
using RagnarEngine;

public class Knife : RagnarComponent
{
	public GameObject player;
	// Components
	private AudioSource audioSourceComponent;
	private ParticleSystem particleComponent;

	private float force = 1700;
	public bool canReload = false;
	private bool pendingToDelete = false;
	private bool grabOnce = false;
	
	public void Start()
    {
        player = GameObject.Find("Player");
        AimMethod();

		// Get components
		particleComponent = gameObject.GetComponent<ParticleSystem>();
		audioSourceComponent = gameObject.GetComponent<AudioSource>();

		particleComponent.Play();

		player.GetComponent<Player>().PlayAudioClip("WPN_THORWINGKNIFETHROW");
	}

    private void AimMethod()
    {
        NavAgent agent = player.GetComponent<NavAgent>();

        Vector3 pos = player.transform.globalPosition;
        pos.y += 1;
        gameObject.transform.localPosition = pos;

        Vector3 direction = HitEnemy(agent, player);
		if(direction != null)
        {
			Rigidbody goRB = gameObject.GetComponent<Rigidbody>();
			goRB.SetBodyPosition(pos);

			Vector3 newForward = direction.normalized;
			double angle = Math.Atan2(newForward.x, newForward.z);
			Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
			goRB.SetBodyRotation(rot);

			goRB.IgnoreCollision(player, true);
			goRB.ApplyCentralForce(newForward * force);
		}
        else
        {
			pendingToDelete = true;
        }

 		agent.hitPosition = player.transform.globalPosition;
	}

    public void Update()
	{
		if (canReload) ReloadKnife();
		if (pendingToDelete) InternalCalls.Destroy(gameObject);

	}
	private void ReloadKnife()
	{
		GameObject player = GameObject.Find("Player");

		float xDiff = player.transform.globalPosition.x - gameObject.transform.globalPosition.x;
		float zDiff = player.transform.globalPosition.z - gameObject.transform.globalPosition.z;
		double distance = Math.Sqrt(zDiff * zDiff + xDiff * xDiff);

		if (distance < 1.0f)
		{
			if (!grabOnce)
			{
				grabOnce = true;
				GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].cooldown;
				audioSourceComponent.PlayClip("WPN_THORWINGKNIFERECOVERSAND");
			}

			pendingToDelete = true;
			canReload = false;
		}
	}

	private Vector3 HitEnemy(NavAgent agent, GameObject player)
	{
		GameObject enemy = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");
		if (enemy != null && Transform.GetDistanceBetween(player.transform.globalPosition, enemy.transform.globalPosition) < 15)
		{
			switch (enemy.GetComponent<BasicEnemy>().state)
			{
				case EnemyState.DEATH:
					GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].cooldown;
					return null;
				case EnemyState.IS_DYING:
					GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].cooldown;
					return null;
				default:
					return enemy.transform.globalPosition - player.transform.globalPosition;
			}
		}
		GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[2].cooldown;
		return null;
	}

	public void OnCollision(Rigidbody other)
	{
		particleComponent.Pause();
		canReload = true;
		gameObject.DeleteComponent<Rigidbody>(gameObject.GetComponent<Rigidbody>());
	}

}


