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

		GameObject sound = InternalCalls.InstancePrefab("SoundArea", gameObject.transform.globalPosition);
		goRB.IgnoreCollision(sound, true);
		sound.GetComponent<SoundAreaManager>().stablishedTimer = 1.5f;
		sound.GetComponent<SoundAreaManager>().UpdateRadius(11.0f);
	}
	public void Update()
	{
		if(timer > 2f)
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