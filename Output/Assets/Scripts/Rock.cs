using System;
using System.Collections.Generic;
using RagnarEngine;

public class Rock : RagnarComponent
{
	private Vector3 relativePos;
	private bool pendingToDelete = false;
	private float cooldown = 2f;
	GameObject player;
	Rigidbody goRB;

	public void Start()
	{
		player = GameObject.Find("Player");
		goRB = gameObject.GetComponent<Rigidbody>();
		AimMethod();
		gameObject.GetComponent<ParticleSystem>().Play();

		player.GetComponent<Player>().PlayAudioClip("EBOSS_THROWOBJECT");
		player.GetComponent<Animation>().PlayAnimation("Ability3");
	}

	private void AimMethod()
	{
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
			pos.y += 1.31f;
			relativePos = newPos - pos;
        }

		Vector3 newForward = relativePos.normalized;
		double angle = Math.Atan2(newForward.x, newForward.z);
		Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
		player.GetComponent<Rigidbody>().SetBodyRotation(rot);

		goRB.IgnoreCollision(player, true);
	}
	public void Update()
	{
		goRB.ApplyVelocity(relativePos.normalized * 30);

		if (pendingToDelete)
		{
			cooldown -= Time.deltaTime;
			if (cooldown < 0)
			{
				InternalCalls.Destroy(gameObject);
			}
		}
	}

	public void OnCollisionEnter(Rigidbody other)
	{
		if (other.gameObject.tag == "Ground")
		{
			gameObject.GetComponent<AudioSource>().PlayClip("BG_SANDHITROCK");

			goRB.SetAsStatic();

			GameObject sound = InternalCalls.InstancePrefab("SoundArea", gameObject.transform.globalPosition);
			goRB.IgnoreCollision(sound, true);
			sound.GetComponent<SoundAreaManager>().stablishedTimer = 2f;
			sound.GetComponent<SoundAreaManager>().UpdateRadius(7.0f);

			cooldown = 2f;
			pendingToDelete = true;
		}
	}
}