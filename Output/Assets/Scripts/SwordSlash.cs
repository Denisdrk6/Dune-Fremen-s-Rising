using System;
using RagnarEngine;

public class SwordSlash : RagnarComponent
{
	public float timeAlive = 0f;
	public bool pendingToDelete = false;
    GameObject player;
    GameObject sword;
	public void Start()
    {
        timeAlive = 0.42f;

        player = GameObject.Find("Player_3");

        Vector3 pos = player.transform.globalPosition;
        pos.y += 1;
        gameObject.transform.localPosition = pos;

        gameObject.transform.forward.Set(player.transform.forward.x, player.transform.forward.y, player.transform.forward.z);

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.SetBodyPosition(pos);
        rb.IgnoreCollision(player, true);
        rb.SetAsTrigger();

        Vector3 direction = GameObject.Find("LevelManager").GetComponent<Level_3>().hitPoint - pos;
        Vector3 newForward = direction.normalized;
        double angle = Math.Atan2(newForward.x, newForward.z);
        Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
        player.GetComponent<Rigidbody>().SetBodyRotation(rot);

        player.GetComponent<Player>().PlayAudioClip("WPN_SWORDHIT");
        //player.GetComponent<Animation>().PlayAnimation("Ability1");
        sword = GameObject.Find("Sword");
        sword.GetComponent<Animation>().PlayAnimation("Attack");
    }

    public void Update()
	{
		if(timeAlive > 0 && gameObject != null)
        {
			timeAlive -= Time.deltaTime;
			if(timeAlive < 0)
            {
				timeAlive = 0f;
                GameObject.Find("SlashParticles").GetComponent<ParticleSystem>().Play();
                pendingToDelete = true;
            }
        }
		if (pendingToDelete) InternalCalls.Destroy(gameObject);
	}

}