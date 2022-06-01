using System;
using RagnarEngine;

public class SwordSlash : RagnarComponent
{
	public float timeAlive = 0f;
	public bool pendingToDelete = false;
    GameObject player;
	public void Start()
    {
        timeAlive = 0.1f;

        player = GameObject.Find("Player_3");

        Vector3 pos = player.transform.globalPosition;
        pos.y += 1;
        gameObject.transform.localPosition = pos;

        gameObject.transform.forward.Set(player.transform.forward.x, player.transform.forward.y, player.transform.forward.z);

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.SetBodyPosition(pos);
        rb.IgnoreCollision(player, true);
        rb.SetAsTrigger();

        player.GetComponent<Player>().PlayAudioClip("WPN_SWORDHIT");
        GameObject.Find("SlashParticles").GetComponent<ParticleSystem>().Play();
    }

    public void Update()
	{
		if(timeAlive > 0 && gameObject != null)
        {
			timeAlive -= Time.deltaTime;
			if(timeAlive < 0)
            {
				timeAlive = 0f;
				pendingToDelete = true;
            }
        }
		if (pendingToDelete) InternalCalls.Destroy(gameObject);
	}

}