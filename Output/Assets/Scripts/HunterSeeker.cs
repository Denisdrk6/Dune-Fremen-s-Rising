using System;
using RagnarEngine;

public class HunterSeeker : RagnarComponent
{
	NavAgent agent;
	public GameObject[] enemies;
	Rigidbody rb;
	GameObject player;
    public GameObject sceneAudio;
	ParticleSystem leftParticles;
	ParticleSystem rightParticles;

	public void Start()
	{
        agent = gameObject.GetComponent<NavAgent>();
		enemies = GameObject.FindGameObjectsWithTag("Enemies");
		player = GameObject.Find("Player_2");
		player.GetComponent<Player>().SetControled(false);
		Vector3 pos = player.transform.globalPosition + new Vector3(0, 0.5f, 0);
		rb = gameObject.GetComponent<Rigidbody>();
		rb.SetBodyPosition(pos);
		rb.IgnoreCollision(player, true);
        sceneAudio = GameObject.Find("AudioLevel1");
        sceneAudio.GetComponent<AudioSource>().PlayClip("WPN_HUNTERSEEKERNEEDLE");
		leftParticles = GameObject.Find("LeftWingParticles").GetComponent<ParticleSystem>();
		rightParticles = GameObject.Find("RightWingParticles").GetComponent<ParticleSystem>();
		leftParticles.Play();
		rightParticles.Play();
	}
	public void Update()
	{
		if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
		{
			agent.CalculatePath(agent.hitPosition);
		}
		agent.MovePath();
		if (Input.GetMouseClick(MouseButton.RIGHT) == KeyState.KEY_UP)
		{
			if (Attack())
			{
                sceneAudio.GetComponent<AudioSource>().PlayClip("WPN_HUNTERSEEKERNEEDLE");
                GameObject player = GameObject.Find("Player_2");
				player.GetComponent<Player>().SetControled(true);
				InternalCalls.Destroy(gameObject);
				leftParticles.Pause();
				rightParticles.Pause();
			}
		}
	}
	public bool Attack()
	{
		for (int i = 0; i < enemies.Length; i++)
		{
			float distance = Vector3.Magnitude(gameObject.transform.globalPosition - enemies[i].transform.globalPosition);
			if (distance <= 5.633)
			{
				if (enemies[i].GetComponent<BasicEnemy>().ToString() == "BasicEnemy")
				{
					enemies[i].GetComponent<BasicEnemy>().pendingToDelete = true;
				}
				if (enemies[i].GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy")
				{
					enemies[i].GetComponent<UndistractableEnemy>().pendingToDelete = true;
				}
				if (enemies[i].GetComponent<TankEnemy>().ToString() == "TankEnemy")
				{
					enemies[i].GetComponent<TankEnemy>().pendingToDelete = true;
				}
				//enemies[i].GetComponent<Animation>().PlayAnimation("Dying");
				return true;
			}
		}
		return false;
	}
	public void OnCollision(Rigidbody other)
	{
		if (other.gameObject.tag == "Enemies")
        {
			player.GetComponent<Player>().SetControled(true);
			InternalCalls.Destroy(gameObject);
			leftParticles.Pause();
			rightParticles.Pause();
		}
	}
}