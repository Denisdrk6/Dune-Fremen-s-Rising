using System;
using RagnarEngine;

public class Boss : RagnarComponent
{
	public struct BarrelSpawnLocation
	{
		public BarrelSpawnLocation(int num, Vector3 location, bool destroyed)
		{
			barrelNum = num;
			spawnLocation = location;
			isDestroyed = destroyed;
		}

		public bool isDestroyed;
		public int barrelNum;
		public Vector3 spawnLocation;
	}
	public enum BossState
	{
		PHASE1,
		PHASE2,
		PHASE3,
		PHASE4
	};

	// Components
	private Animation animationComponent;
	private ParticleSystem stabParticles;
	private ParticleSystem shieldParticles;
	private Rigidbody rb;

	////////////

	public Material material;
	public Rigidbody rigidbody;

	public BossState state;

	// NavMesh behaviour
	NavAgent agent;
	float calculatePathCooldown = 0.0f;

	// Percetion variables
	private Vector3 offset;
	public int index = 0;

	// Arrays for the players and colliders
	GameObject[] players = new GameObject[3];
	public GameObject[] colliders;

	// Phase 2 mechanics
	int indexPlayerTarget;
	bool jumping = false;
	bool phase2Location = false;

	// Phase 3 mechanics

	GameObject[] barrels = new GameObject[3];
	public BarrelSpawnLocation[] barrelLocations = new BarrelSpawnLocation[3];
	public int barrelCount = 0;
	public int stunnedHits = 0;
	float barrelCooldown = 0.0f;
	bool shieldInmunity = false;
	bool phase3Location = false;

	private int destPoint = 0;

	// Phase 4 mechanics
	// Cinematic for throwing paul
	bool finishedCinematic = false;
	bool grabbedPaul = false;
	bool throwedPaul = false;
	Vector3 throwingLocation;
	Vector3 newBossLocation;

	int throwedRocks = 0;
	float hitGroundCooldown = 10.0f;
	float shieldCooldown = 10.0f;
	float tiredCooldown = 10.0f;
	bool rocksAvailable = false;
	bool throwingRocks = false;
	bool throwing = false;
	bool tired = false;
	GameObject nextRock;
	float sweepAttackCooldown = 5.0f;

	GameObject[] waypoints = new GameObject[3];
	bool bossStop = false;
	float countDown = 5.0f;


	string[] animations = new string[14];
	//int indexAnim = 0;
	public void Start()
	{

		// Get Components
		animationComponent = gameObject.GetComponent<Animation>();
		stabParticles = GameObject.Find("StabParticlesBoss").GetComponent<ParticleSystem>();
		shieldParticles = GameObject.Find("BossShieldParticles").GetComponent<ParticleSystem>();
		rb = gameObject.GetComponent<Rigidbody>();


		animations[0] = "CallBackup";
		animations[1] = "Die";
		animations[2] = "Execdie";
		animations[3] = "Exhausted";
		animations[4] = "HitGround";
		animations[5] = "Ntoa";
		animations[6] = "Run";
		animations[7] = "Shield";
		animations[8] = "ShieldDestroy";
		animations[9] = "ShotCrazy";
		animations[10] = "SitIdle";
		animations[11] = "Throw";
		animations[12] = "WalkAngry";
		animations[13] = "WalkNormal";

		throwingLocation = new Vector3(0.0f, 9.14f, -63.49f);
		newBossLocation = new Vector3(0.0f, 9.14f, -75.35f);
		colliders = GameObject.FindGameObjectsWithTag("Collider");
		material = gameObject.GetComponent<Material>();
		state = BossState.PHASE1;

		offset = gameObject.GetSizeAABB();

		waypoints[0] = GameObject.Find("28");
		waypoints[1] = GameObject.Find("29");
		waypoints[2] = GameObject.Find("30");

		rigidbody = gameObject.GetComponent<Rigidbody>();

		agent = gameObject.GetComponent<NavAgent>();

		animationComponent.PlayAnimation("SitIdle");

		barrelLocations[0] = new BarrelSpawnLocation(0, new Vector3(1.60f, 9.27f, -54.91f), true);
		barrelLocations[1] = new BarrelSpawnLocation(1, new Vector3(-12.60f, 9.27f, -41.12f), true);
		barrelLocations[2] = new BarrelSpawnLocation(2, new Vector3(10.79f, 9.27f, -41.12f), true);

		stabParticles.Pause();
		shieldParticles.Pause();
	}
	public void Update()
	{
		if (Input.GetKey(KeyCode.N) == KeyState.KEY_DOWN)
		{
			state += 1;
			NextState();
		}

		if (state == BossState.PHASE2)
		{
			Phase2();
		}
		else if (state == BossState.PHASE3)
		{
			Phase3();
		}
		else if (state == BossState.PHASE4)
		{
			Phase4();
		}

		//if (Input.GetKey(KeyCode.M) == KeyState.KEY_DOWN)
		//{
		//	string anim = animations[indexAnim];
		//	gameObject.GetComponent<Animation>().PlayAnimation(anim);
		//	indexAnim++;
		//	//GenerateEnemies();
		//	if (indexAnim == 14)
		//	{
		//		indexAnim = 0;
		//	}

		//}

	}

	private void NextState()
	{
		switch (state)
		{
			case BossState.PHASE2:
				//GenerateEnemies();
				players = GameObject.FindGameObjectsWithTag("Player");
				rigidbody.linearVelocity = GameObject.Find("Player").GetComponent<Rigidbody>().linearVelocity * 0.5f;
				GotoNextPoint();
				break;
			case BossState.PHASE3:
				GameObject.Find("BossShieldParticles").GetComponent<ParticleSystem>().Play();
				rigidbody.linearVelocity = GameObject.Find("Player").GetComponent<Rigidbody>().linearVelocity * 0.75f;
				barrelCooldown = 0.0f;
				GenerateBarrels();
				break;
			case BossState.PHASE4:
				//if (!shieldInmunity)
				//{
				rigidbody.linearVelocity = GameObject.Find("Player").GetComponent<Rigidbody>().linearVelocity * 1.2f;
				animationComponent.PlayAnimation("Run");
				//}
				//else state--;
				break;
			default:
				state--;
				break;
		}
	}

	private void GenerateEnemies()
	{
		animationComponent.PlayAnimation("CallBackup");

		GameObject enemy1 = InternalCalls.InstancePrefab("Basic Enemy 16", new Vector3(2.07f, 6.66f, -19.0f));
		enemy1.GetComponent<EnemyBoss>().colliders = colliders;

		GameObject enemy2 = InternalCalls.InstancePrefab("Basic Enemy 17", new Vector3(0.61f, 6.66f, -8.0f));
		enemy2.GetComponent<EnemyBoss>().colliders = colliders;
	}
	public void GenerateRocks()
	{
		for (int i = 0; i < 5; ++i)
		{
			string rockPrefab = "Rock" + (i + 1);
			GameObject rock = GameObject.Find(rockPrefab);
			if (rock != null)
				InternalCalls.Destroy(rock);
		}
		Vector3 bossPos = gameObject.transform.localPosition;
		for (int i = 0; i < 5; ++i)
		{
			string rockPrefab = "Rock" + (i + 1);

			Vector3 pos;
			if (i % 2 == 0) pos = new Vector3(bossPos.x, 15.0f, bossPos.z + i);
			else pos = new Vector3(bossPos.x + i, 15.0f, bossPos.z);

			InternalCalls.InstancePrefab(rockPrefab, pos);
		}
	}

	// Boss behaviour for the phases

	// Phase 2
	public void Phase2()
	{
		// this instakills
		if (phase2Location == false)
		{
			if (animationComponent.HasFinished())
			{
				// Move to new location as soon as phase 2 starts
				Vector3 destination = new Vector3(0.0f, 9.24f, 18.53f);

				animationComponent.PlayAnimation("WalkNormal");
				agent.MoveTo(destination);

				if (Math.Abs((gameObject.transform.localPosition - destination).magnitude) < 1.0f) phase2Location = true;
			}
		}
		else
		{
			if (PerceptionCone(90))
			{
				Vector3 jumpTo = new Vector3(100.0f, 100.0f, 100.0f);
				Vector3 area = new Vector3(1.0f, 1.0f, 1.0f);
				if (!jumping)
				{
					for (int i = 0; i < players.Length; ++i)
					{
						if (players[i].transform.globalPosition.magnitude <= area.magnitude &&
							players[i].transform.globalPosition.magnitude < jumpTo.magnitude)
						{
							jumpTo = players[i].transform.globalPosition;
							indexPlayerTarget = i;
						}
					}
					agent.speed = 10.0f;
				}

				if (jumpTo != new Vector3(100.0f, 100.0f, 100.0f) && players[indexPlayerTarget] != null)
				{
					agent.MoveTo(jumpTo);
					if ((players[indexPlayerTarget].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude) <= 2.0f)
					{
						// Play sweep attack animation

						// Play sweep attack sound

						// Hit player, lower his HP
						players[indexPlayerTarget].GetComponent<Player>().GetHit(100);
					}
				}
			}

			Patrol();
		}
	}

	// Phase 3
	public void Phase3()
	{
        if (phase3Location == false)
        {
            // Move to new location as soon as phase 3 starts
            Vector3 destination = new Vector3(0.0f, 9.24f, -58.0f);

            animationComponent.PlayAnimation("WalkNormal");
            agent.MoveTo(destination);

            if (Math.Abs((gameObject.transform.localPosition - destination).magnitude) < 2.0f) phase3Location = true;

            shieldInmunity = true;
		}
        else
        {
            Debug.Log("Needs to be stunned");
			ExplodeBarrels();
			if (shieldInmunity) GenerateBarrels();
			if (stunnedHits < 3 && barrelCount < 3)
			{
				barrelCooldown -= Time.deltaTime;
			}

			if (shieldInmunity)
			{
				FollowPlayer();
			}
			else
			{
				shieldCooldown -= Time.deltaTime;
				if (shieldCooldown <= 0.0f)
				{
					shieldCooldown = 10.0f;
					shieldInmunity = true;
					shieldParticles.Play();
					stunnedHits = 0;
				}
			}
		}
	}

	// Phase 4
	public void Phase4()
	{
		if (finishedCinematic)
		{
			if (tired == false && hitGroundCooldown <= 0.0f && rocksAvailable == false)
			{
				animationComponent.PlayAnimation("HitGround");
				GenerateRocks();
				hitGroundCooldown = 10.0f;
				throwingRocks = true;
			}
			else if (!throwingRocks && hitGroundCooldown > 0.0f && !tired)
			{
				FollowPlayer();
				hitGroundCooldown -= Time.deltaTime;
			}
			else if (tired == false && rocksAvailable == true)
			{
                if (!throwing)
                {
                    animationComponent.PlayAnimation("Throw");
					throwing = true;
                }
				else if (animationComponent.HasFinished())
                {
					ThrowRock();
					throwedRocks++;
					Debug.Log("Finished Throwing");
					throwing = false;
					rocksAvailable = false;
				}
            }
			else if (tired == false && throwedRocks < 5 && rocksAvailable == false && !throwing)
			{
				if (nextRock == null)
				{
					for (int i = 0; i < 5; ++i)
					{
						string rockPrefab = "Rock" + (i + 1);
						nextRock = GameObject.Find(rockPrefab);
						if (nextRock != null)
						{
							animationComponent.PlayAnimation("WalkAngry");
							break;
						}
					}
				}
				else
				{
					Vector3 dest = nextRock.transform.localPosition;
					dest.y = gameObject.transform.localPosition.y;
					agent.MoveTo(dest);
				}
			}
			else if (tired == false && throwedRocks == 5)
            {
				tired = true;
				animationComponent.PlayAnimation("Exhausted");
			}
			else
			{
				tiredCooldown -= Time.deltaTime;
				if (tiredCooldown <= 0.0f)
				{
					throwingRocks = false;
					tired = false;
					throwedRocks = 0;
					tiredCooldown = 10.0f;
					animationComponent.PlayAnimation("WalkAngry");
				}
			}

			if (!tired && sweepAttackCooldown <= 0.0f) SweepAttack();

			sweepAttackCooldown -= Time.deltaTime;
		}
		else
		{
			if (!grabbedPaul)
			{
				FollowPlayer();
				float magnitude = Math.Abs((players[0].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude));
				if (magnitude <= 1.5f)
				{
					grabbedPaul = true;
					players[0].GetComponent<Player>().stunned = true;
				}
			}
			else
			{
				if (!throwedPaul)
				{
					agent.CalculatePath(throwingLocation);
					agent.MovePath();
					if (animationComponent.HasFinished())
                    {
						animationComponent.PlayAnimation("WalkAngry");
                    }
					rb.IgnoreCollision(players[0], true);
					players[0].GetComponent<Rigidbody>().SetBodyPosition(gameObject.transform.globalPosition + new Vector3(0.5f, 0.5f, 0.5f));
				}
				else
				{
					// Go to next zone after throwing Paul
					//if (gameObject.GetComponent<Animation>().HasFinished())
					//{
					//	gameObject.GetComponent<Animation>().PlayAnimation("WalkAngry");
					//}
					agent.CalculatePath(newBossLocation);
					agent.MovePath();
					if (Math.Abs((newBossLocation.magnitude - gameObject.transform.globalPosition.magnitude)) <= 1.0f)
						finishedCinematic = true;
				}

				if (Math.Abs((throwingLocation.magnitude - gameObject.transform.globalPosition.magnitude)) <= 1.0f)
				{
					// Location to throw the player at
					//gameObject.GetComponent<Animation>().PlayAnimation("Throw");
					Vector3 desiredLocation = new Vector3(0.0f, 9.14f, -105.63f);
					Vector3 endDirection;
					endDirection = (desiredLocation - gameObject.transform.globalPosition).normalized;

					rb.IgnoreCollision(players[0], false);
					players[0].GetComponent<Rigidbody>().ApplyCentralImpulse(endDirection * 50.0f);
					throwedPaul = true;
					animationComponent.PlayAnimation("WalkAngry");
					players[0].GetComponent<Player>().stunned = false;
				}
			}
		}
	}

	private void ThrowRock()
	{
		//gameObject.GetComponent<AudioSource>().PlayClip("EBOSS_THROWOBJECT");
		GameObject boss = GameObject.Find("Boss");
		Vector3 pos = boss.transform.localPosition;
		pos.y += 0.5f;
		GameObject r = InternalCalls.InstancePrefab("RockBoss", pos);
		r.transform.localPosition = pos;

		Vector3 direction = players[0].transform.localPosition - gameObject.transform.localPosition;
		Rigidbody rockBossRb = r.GetComponent<Rigidbody>();
		rockBossRb.IgnoreCollision(this.gameObject, true);
		rockBossRb.ApplyCentralForce(direction.normalized * 2000.0f);

	}
	private void GenerateBarrels()
	{
		if (barrelCooldown <= 0.0f && barrelCount < 3)
		{
			for (int i = 0; i < 3; ++i)
			{
				if (barrelLocations[i].isDestroyed)
				{
					string barrelName = "Barrel" + (i + 1);
					// Need to know the name of the prefab
					GameObject barrel = InternalCalls.InstancePrefab(barrelName, barrelLocations[i].spawnLocation);
					barrel.GetComponent<Barrel>().barrelIndex = i;
					barrelLocations[i].isDestroyed = false;
					barrelCount++;
				}
			}
			barrelCooldown = 15.0f;
		}
	}

	private void ExplodeBarrels()
	{
		if (stunnedHits == 3)
		{
			if (animationComponent.HasFinished())
			{
				animationComponent.PlayAnimation("ShieldDestroy");
			}
			shieldInmunity = false;
			shieldParticles.Pause();
		}
	}

	private bool PerceptionCone(int angleDegrees)
	{
		Vector3 bossPos = gameObject.transform.globalPosition;
		Vector3 bossForward = gameObject.transform.forward;
		Vector3 initPos = new Vector3(bossPos.x + (bossForward.x * offset.x * 0.6f), bossPos.y + 0.1f, bossPos.z + (bossForward.z * offset.z * 0.6f));

		index = RayCast.PerceptionCone(initPos, bossForward, angleDegrees, 16, 8, players, players.Length, "Collider", Time.deltaTime);
		return (index == -1) ? false : true;
	}

	public void GotoNextPoint()
	{
		//gameObject.GetComponent<AudioSource>().PlayClip("FOOTSTEPS");
		//gameObject.GetComponent<Animation>().PlayAnimation("WalkNormal");

		//Debug.Log(waypoints[destPoint].transform.globalPosition.ToString());
		agent.CalculatePath(waypoints[destPoint].transform.globalPosition);
		destPoint = (destPoint + 1) % waypoints.Length;
	}

	private void Patrol()
	{
		agent.MovePath();
		if (!bossStop && Math.Abs((waypoints[destPoint].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude)) < 1.5f)
		{
			Debug.Log("Stopped");
			bossStop = true;
		}

		if (bossStop)
		{
			countDown -= Time.deltaTime;
			if (countDown <= 0.0f)
			{
				bossStop = false;
				countDown = 5.0f;
				GotoNextPoint();
			}
		}
	}

	private void FollowPlayer()
	{
		if (calculatePathCooldown <= 0.0f)
		{
			agent.CalculatePath(players[0].transform.localPosition);
			calculatePathCooldown = 2.0f;
		}

		agent.MovePath();
		calculatePathCooldown -= Time.deltaTime;
	}
	public void SweepAttack()
	{
		if (players[0] != null && Math.Abs((players[0].transform.globalPosition - gameObject.transform.globalPosition).magnitude) <= 1.5f)
		{
			// Play sweep attack animation

			// Play sweep attack sound
			Debug.Log("SWEEP ATTACK");
			// Hit player, lower his HP
			players[0].GetComponent<Player>().GetHit(1);
			// Reset Cooldown
			sweepAttackCooldown = 5.0f;
		}
	}
	public void GetBackstabbed()
	{
		if (!shieldInmunity)
		{
			state++;
			stabParticles.Play();
			NextState();
		}
	}
	public void OnCollision(Rigidbody other)
	{
		if (other.gameObject.name == "BackStab" && !shieldInmunity)
		{
			state++;
			NextState();
			if (state == BossState.PHASE4)
			{
				SceneManager.LoadScene("WinScene");
			}
		}
		else if (other.gameObject.tag == "Rocks" && rocksAvailable == false)
		{
			Debug.Log("Found rock" + nextRock.name.ToString());
			if (nextRock != null && other.gameObject.name == nextRock.name)
			{
				rocksAvailable = true;
				InternalCalls.Destroy(other.gameObject);
				nextRock = null;
			}
		}
	}
}