using System;
using System.Collections.Generic;
using RagnarEngine;

public class Rock : RagnarComponent
{
	public float soundRadius = 6f;
	private float cooldown = 0f;
	private Vector3 relativePos;

	private bool pendingToDelete = false;
	private bool hitOnce = false;

	GameObject player;
	Rigidbody goRB;

	public void Start()
	{
		goRB = gameObject.GetComponent<Rigidbody>();
		AimMethod();
		gameObject.GetComponent<ParticleSystem>().Play();
	}

	private void AimMethod()
	{
		player = GameObject.Find("Player");

		Vector3 pos = player.transform.globalPosition;
		pos.y += 1.5f;
		goRB.SetBodyPosition(pos);
		gameObject.transform.globalPosition = pos;

		float radius = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().radius;
		Vector3 hitPoint = GameObject.Find("LevelManager").GetComponent<Level_1>().hitPoint;
		relativePos = hitPoint - pos;
		if (relativePos.magnitude > radius)
        {
			relativePos.y = hitPoint.y;
			pos.y -= 1.5f;
			Vector3 newPos = pos + relativePos.normalized * radius;
			pos.y += 1.5f;
			relativePos = newPos - pos;
        }

		goRB.IgnoreCollision(player, true);
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

	public void OnCollisionEnter(Rigidbody other)
	{
		if (other.gameObject.tag == "Ground" && !hitOnce)
        {
			gameObject.GetComponent<AudioSource>().PlayClip("BG_SANDHITROCK");
			hitOnce = true;
		}

		goRB.SetAsStatic();

		Rigidbody area = gameObject.CreateComponent<Rigidbody>();
		CreateSphereTrigger(area, soundRadius, gameObject.transform.globalPosition);
		cooldown = 2;
	}

	private static void CreateSphereTrigger(Rigidbody rb, float radius, Vector3 pos)
	{
		rb.SetCollisionSphere(radius, pos.x, pos.y, pos.z);
		rb.SetAsTrigger();
	}
}