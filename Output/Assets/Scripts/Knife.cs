using System;
using RagnarEngine;

public class Knife : RagnarComponent
{
	private float force = 1500;
	public bool canReload = false;
	private bool pendingToDelete = false;
	private bool grabOnce = false;
	
	public void Start()
    {
        AimMethod();
    }

    private void AimMethod()
    {
    	gameObject.GetComponent<ParticleSystem>().Play();
        GameObject player = GameObject.Find("Player");
        NavAgent agent = player.GetComponent<NavAgent>();

        Vector3 pos = player.transform.globalPosition;
        pos.y += 1;
        gameObject.transform.localPosition = pos;

        Vector3 direction = HitEnemy(agent, player);

        Rigidbody goRB = gameObject.GetComponent<Rigidbody>();
        goRB.SetBodyPosition(pos);
        goRB.IgnoreCollision(player, true);
        goRB.ApplyCentralForce(direction.normalized * force);

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
				gameObject.GetComponent<AudioSource>().PlayClip("WPN_THORWINGKNIFERECOVERSAND");
			}

			pendingToDelete = true;
			canReload = false;
		}
	}

	private Vector3 HitEnemy(NavAgent agent, GameObject player)
	{
		gameObject.GetComponent<ParticleSystem>().Pause();
			
		GameObject obj = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");

		if (obj != null)
		{
			//Debug.Log(obj.name.ToString());
			return obj.GetComponent<Transform>().globalPosition - player.transform.globalPosition;
		}

		return agent.hitPosition - player.transform.globalPosition;
	}

	public void OnCollision(Rigidbody other)
	{
		canReload = true;
	}

}


