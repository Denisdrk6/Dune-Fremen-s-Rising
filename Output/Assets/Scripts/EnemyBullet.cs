using System;
using RagnarEngine;

public class EnemyBullet : RagnarComponent
{
	public int vel = 100;

	public GameObject[] players;
	public int index = 0;
	public GameObject enemy;
	public Vector3 offset;

	public void Start()
	{
		players = GameObject.FindGameObjectsWithTag("Player");

		Vector3 pos = enemy.transform.globalPosition;
		pos.y += 0.5f;
		gameObject.transform.localPosition = pos;

		Rigidbody bulletRb = gameObject.GetComponent<Rigidbody>();
		bulletRb.IgnoreCollision(enemy, true);
		bulletRb.SetBodyPosition(pos);

		Vector3 diff = players[index].transform.globalPosition - gameObject.transform.globalPosition;
		diff.y = gameObject.transform.globalPosition.y;

		pos.x += gameObject.transform.forward.x * offset.x * 0.6f;
		pos.z += gameObject.transform.forward.z * offset.z * 0.6f;
		Vector3 objectivePos = players[index].transform.globalPosition;
		objectivePos.y += 1; 
		GameObject obj = RayCast.HitToTag(pos, objectivePos, "Player");
		if (obj != null)
		{
			//Debug.Log(obj.name.ToString());
			obj.GetComponent<Player>().GetHit(1);
			InternalCalls.RequestDamageFeedback();
		}

		bulletRb.linearVelocity = diff.normalized * vel;

		Vector3 dir = diff * -2;
		gameObject.GetComponent<ParticleSystem>().SetDirectionParticle(dir);
		gameObject.GetComponent<ParticleSystem>().Play();
	}
    public void Update()
	{
	}

	public void OnCollision()
	{
		InternalCalls.Destroy(gameObject);
	}
}