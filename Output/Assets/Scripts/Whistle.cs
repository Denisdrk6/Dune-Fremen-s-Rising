using System;
using RagnarEngine;

public class Whistle : RagnarComponent
{
	public float timer = 0f;
	
	public void Start()
	{
		GameObject player = GameObject.Find("Player_3");

		Vector3 pos = player.transform.globalPosition;
		pos.y += 1;
		gameObject.transform.localPosition = pos;

		Rigidbody goRB = gameObject.GetComponent<Rigidbody>();
		goRB.SetBodyPosition(pos);
		goRB.IgnoreCollision(player, true);

		player.GetComponent<Player>().PlayAudioClip("WPN_WHISTLE");
	}
	public void Update()
	{
		if(timer > 1.5f)
        {
			timer = 0f;
			InternalCalls.Destroy(gameObject);
		}
        else
        {
			timer += Time.deltaTime;
        }
	}

}