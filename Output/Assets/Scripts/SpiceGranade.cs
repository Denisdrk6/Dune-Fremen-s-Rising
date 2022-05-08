using System;
using RagnarEngine;

public class SpiceGranade : RagnarComponent
{
	Rigidbody goRB;
	private float force = 1050;
	public float explosionRadius = 6f;
	private float cooldown = 0f;
	public bool pendingToDelete = false;
	public void Start()
	{
		AimMethod();
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
	private void AimMethod()
	{
		GameObject player = GameObject.Find("Player_2");
		NavAgent agent = player.GetComponent<NavAgent>();

		Vector3 pos = player.transform.globalPosition;
		pos.y += 1;
		gameObject.transform.localPosition = pos;

		Vector3 direction = agent.hitPosition - player.transform.globalPosition;
		direction.y = 0;

		goRB = gameObject.GetComponent<Rigidbody>();
		goRB.SetBodyPosition(pos);
		goRB.IgnoreCollision(player, true);
		goRB.ApplyCentralForce(direction.normalized * force);

		agent.hitPosition = player.transform.globalPosition;
	}
	public void OnCollisionEnter(Rigidbody other)
	{
		gameObject.GetComponent<ParticleSystem>().Play();
		goRB.SetAsStatic();
		Rigidbody area = gameObject.CreateComponent<Rigidbody>();
		CreateSphereTrigger(area, explosionRadius, gameObject.transform.globalPosition);
		cooldown = 1;
	}

	private static void CreateSphereTrigger(Rigidbody rb, float radius, Vector3 pos)
	{
		rb.SetCollisionSphere(radius, pos.x, pos.y, pos.z);
		rb.SetAsTrigger();
	}
}