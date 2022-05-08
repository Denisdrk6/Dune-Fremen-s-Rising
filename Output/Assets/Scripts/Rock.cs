using System;
using RagnarEngine;

public class Rock : RagnarComponent
{
	private float force = 1050;
	public float soundRadius = 6f;
	private float cooldown = 0f;
	private bool pendingToDelete = false;
	private bool hitOnce = false;
	Rigidbody goRB;

	public void Start()
	{
		AimMethod();
		gameObject.GetComponent<ParticleSystem>().Play();
	}

	private void AimMethod()
	{
		GameObject player = GameObject.Find("Player");
		NavAgent agent = player.GetComponent<NavAgent>();

		Vector3 pos = player.transform.globalPosition;
		pos.y += 1;
		gameObject.transform.localPosition = pos;

		Vector3 direction = HitEnemy(agent, player);
		direction.y = 0;

		goRB = gameObject.GetComponent<Rigidbody>();
		goRB.SetBodyPosition(pos);
		goRB.IgnoreCollision(player, true);
		goRB.ApplyCentralForce(direction.normalized * force);

		agent.hitPosition = player.transform.globalPosition;
	}
	private Vector3 HitEnemy(NavAgent agent, GameObject player)
	{
		GameObject obj = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");

		if (obj != null)
		{
			Debug.Log(obj.name.ToString());
			return obj.GetComponent<Transform>().globalPosition - player.transform.globalPosition;
		}

		return agent.hitPosition - player.transform.globalPosition;
	}

	public void Update()
	{
		if (cooldown > 0 && gameObject != null)
		{
			cooldown -= Time.deltaTime;
			if (cooldown < 0)
			{
				cooldown = 0f;
				pendingToDelete = true;
			}
		}
		if (pendingToDelete) InternalCalls.Destroy(gameObject);
	}

	public void OnCollisionEnter(Rigidbody other)
	{
		if (other.gameObject.tag == "Ground" && !hitOnce)
        {
			gameObject.GetComponent<AudioSource>().PlayClip("WEAPONROCKHITGROUND");
			hitOnce = true;
		}

		goRB.SetAsStatic();
		Rigidbody area = gameObject.CreateComponent<Rigidbody>();
		CreateSphereTrigger(area, soundRadius, gameObject.transform.globalPosition);
		cooldown = 2f;
	}

	private static void CreateSphereTrigger(Rigidbody rb, float radius, Vector3 pos)
	{
		rb.SetCollisionSphere(radius, pos.x, pos.y, pos.z);
		rb.SetAsTrigger();
	}
}