using System;
using System.Collections.Generic;
using RagnarEngine;

public class Rock : RagnarComponent
{
	private Vector3 relativePos;
	private float cooldown = -1f;
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
	}

	public void OnCollisionEnter(Rigidbody other)
	{
		if (other.gameObject.tag == "Ground")
		{
			gameObject.GetComponent<AudioSource>().PlayClip("BG_SANDHITROCK");

			goRB.SetAsStatic();

			GameObject sound = InternalCalls.InstancePrefab("SoundArea", true);
			sound.GetComponent<Rigidbody>().SetRadiusSphere(6f);
			sound.transform.globalPosition = gameObject.transform.globalPosition;
			sound.GetComponent<SoundAreaManager>().stablishedTimer = 2f;

			cooldown = 2f;
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