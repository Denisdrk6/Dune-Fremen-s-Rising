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
		rb = gameObject.GetComponent<Rigidbody>();
        agent = gameObject.GetComponent<NavAgent>();
		enemies = GameObject.FindGameObjectsWithTag("Enemies");
		player = GameObject.Find("Player_2");
		player.GetComponent<Player>().SetControled(false);
		Vector3 pos = player.transform.globalPosition + new Vector3(0, 0.5f, 0);
		gameObject.transform.globalPosition = pos;

		Vector3 newForward = agent.hitPosition - pos;
		double angle = Math.Atan2(newForward.x, newForward.z);
		Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
		player.GetComponent<Rigidbody>().SetBodyRotation(rot);
		rb.SetBodyRotation(rot);
		rb.SetBodyPosition(pos);

        for (int i = 0; i < GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters.Length; i++)
        {
			rb.IgnoreCollision(GameObject.Find("PlayerManager").GetComponent<PlayerManager>().players[i], true);
		}

		player.GetComponent<Animation>().PlayAnimation("Ability3");
		sceneAudio = GameObject.Find("AudioLevel1");
        sceneAudio.GetComponent<AudioSource>().PlayClip("WPN_HUNTERSEEKERNEEDLE");

		leftParticles = GameObject.Find("LeftHunterParticles").GetComponent<ParticleSystem>();
		rightParticles = GameObject.Find("RightHunterParticles").GetComponent<ParticleSystem>();
		leftParticles.Play();
		rightParticles.Play();
	}
	public void Update()
	{
		if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
		{
			GameObject enemy = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");
			if (enemy != null)
				agent.CalculatePath(enemy.transform.globalPosition);
			else
				agent.CalculatePath(agent.hitPosition);
			leftParticles.Play();
			rightParticles.Play();
		}
		if(agent.PathSize() == 0)
        {
			leftParticles.Pause();
			rightParticles.Pause();
		}
		agent.MovePath();
		if (Input.GetMouseClick(MouseButton.RIGHT) == KeyState.KEY_UP)
		{
			if (Attack())
			{
                sceneAudio.GetComponent<AudioSource>().PlayClip("WPN_HUNTERSEEKERNEEDLE");
				// TODO: Must check if more players need to be found, if it's only player 2, it is already stored in "player" at the start
                // GameObject player = GameObject.Find("Player_2");
				player.GetComponent<Player>().SetControled(true);
				InternalCalls.Destroy(gameObject);
				leftParticles.Pause();
				rightParticles.Pause();
			}
		}
		if (Input.GetKey(KeyCode.ALPHA1) == KeyState.KEY_DOWN || Input.GetKey(KeyCode.ALPHA2) == KeyState.KEY_DOWN || Input.GetKey(KeyCode.ALPHA3) == KeyState.KEY_DOWN)
		{
			GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[1].abilities[2].counter = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[1].abilities[2].cooldown;
			leftParticles.Pause();
			rightParticles.Pause();
			InternalCalls.Destroy(gameObject);
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
			GameObject.Find("Quest System").GetComponent<QuestSystem>().enemiesHunterSeeker++;
			player.GetComponent<Player>().SetControled(true);
			leftParticles.Pause();
			rightParticles.Pause();
			InternalCalls.Destroy(gameObject);
		}
	}
}