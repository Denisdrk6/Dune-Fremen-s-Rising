using System;
using RagnarEngine;

public class SpiceGranade : RagnarComponent
{
	public GameObject player;
	Rigidbody goRB;
	public float explosionRadius = 6.7f;
	private float cooldown = 0f;
	private Vector3 relativePos;

	public bool pendingToDelete = false;
    private GameObject sceneAudio;
	public void Start()
	{
		player = GameObject.Find("Player_2");
		goRB = gameObject.GetComponent<Rigidbody>();
        sceneAudio = GameObject.Find("AudioLevel1");
		AimMethod();

		player.GetComponent<Player>().PlayAudioClip("SMOKEGRENADE_ACTIVATE");
		//player.GetComponent<Animation>().PlayAnimation("Ability4");
	}
	public void Update()
	{
		goRB.ApplyVelocity(relativePos.normalized * 25);
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
		Vector3 pos = player.transform.globalPosition;
		pos.y += 1.5f;
		goRB.SetBodyPosition(pos);
		gameObject.transform.globalPosition = pos;

		float radius = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().radius;
		GameObject LevelManager = GameObject.Find("LevelManager");
		Vector3 hitPoint = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().tempPosition;
		
		relativePos = hitPoint - pos;
		if (relativePos.magnitude > radius)
		{
			relativePos.y = hitPoint.y;
			pos.y -= 1.5f;
			Vector3 newPos = pos + relativePos.normalized * radius;
			pos.y += 1.5f;
			relativePos = newPos - pos;
		}
		Vector3 newForward = relativePos.normalized;
		double angle = Math.Atan2(newForward.x, newForward.z);
		Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
		player.GetComponent<Rigidbody>().SetBodyRotation(rot);

		goRB.IgnoreCollision(player, true);
	}
	public void OnCollisionEnter(Rigidbody other)
	{
        sceneAudio.GetComponent<AudioSource>().PlayClip("SMOKEGRENADE_EXPLOSION");

		goRB.SetAsStatic();
		
		gameObject.GetComponent<ParticleSystem>().Play();

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