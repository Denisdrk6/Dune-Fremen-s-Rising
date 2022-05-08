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
	int throwedRocks = 0;
	float hitGroundCooldown = 10.0f;
	float tiredCooldown = 10.0f;
	bool rocksAvailable = false;
	bool throwingRocks = false;
	bool tired = false;
	GameObject nextRock;
	float sweepAttackCooldown = 0.0f;

	GameObject[] waypoints = new GameObject[3];
	bool bossStop = false;
	float countDown = 5.0f;

	public void Start()
	{
		colliders = GameObject.FindGameObjectsWithTag("Collider");
		material = gameObject.GetComponent<Material>();
		state = BossState.PHASE1;

		offset = gameObject.GetSizeAABB();

		waypoints[0] = GameObject.Find("28");
		waypoints[1] = GameObject.Find("29");
		waypoints[2] = GameObject.Find("30");

		rigidbody = gameObject.GetComponent<Rigidbody>();

		agent = gameObject.GetComponent<NavAgent>();

		barrelLocations[0] = new BarrelSpawnLocation(0, new Vector3(-1.10f, 6.66f, -7.0f), true);
		barrelLocations[1] = new BarrelSpawnLocation(1, new Vector3(3.5f, 6.66f, -4.0f), true);
		barrelLocations[2] = new BarrelSpawnLocation(2, new Vector3(-3.10f, 6.66f, -4.0f), true);
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

		if (Input.GetKey(KeyCode.M) == KeyState.KEY_DOWN)
		{
			GenerateEnemies();
		}
	}

	private void NextState()
	{
		switch (state)
		{
			case BossState.PHASE2:
				GenerateEnemies();
				players = GameObject.FindGameObjectsWithTag("Player");
				rigidbody.linearVelocity = GameObject.Find("Player").GetComponent<Rigidbody>().linearVelocity * 0.5f;
				GotoNextPoint();
				break;
			case BossState.PHASE3:
				rigidbody.linearVelocity = GameObject.Find("Player").GetComponent<Rigidbody>().linearVelocity * 0.75f;
				barrelCooldown = 0.0f;
				GenerateBarrels();
				break;
			case BossState.PHASE4:
				if (!shieldInmunity)
				{
					rigidbody.linearVelocity = GameObject.Find("Player").GetComponent<Rigidbody>().linearVelocity * 1.2f;
				}
				else state--;
				break;
			default:
				state--;
				break;
		}
	}

	private void GenerateEnemies()
	{
		InternalCalls.InstancePrefab("Basic Enemy 15");
		GameObject enemy1 = GameObject.Find("Basic Enemy 15");
		enemy1.GetComponent<Rigidbody>().SetBodyPosition(new Vector3(2.07f, 6.66f, -19.0f));
		enemy1.GetComponent<EnemyBoss>().colliders = colliders;

		InternalCalls.InstancePrefab("Basic Enemy 16");
		enemy1 = GameObject.Find("Basic Enemy 16");
		enemy1.GetComponent<Rigidbody>().SetBodyPosition(new Vector3(0.61f, 6.66f, -8.0f));
		enemy1.GetComponent<EnemyBoss>().colliders = colliders;
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
			InternalCalls.InstancePrefab(rockPrefab);
			GameObject rock = GameObject.Find(rockPrefab);

			if (i % 2 == 0) rock.GetComponent<Rigidbody>().SetBodyPosition(new Vector3(bossPos.x, 25.0f, bossPos.z + i));
			else rock.GetComponent<Rigidbody>().SetBodyPosition(new Vector3(bossPos.x + i, 25.0f, bossPos.z));
		}
	}

	// Boss behaviour for the phases

	// Phase 2
	public void Phase2()
	{
		// this instakills
		if (phase2Location == false)
		{
				// Move to new location as soon as phase 2 starts
				Vector3 destination = new Vector3(0.0f, 6.66f, 2.50f);

				agent.MoveTo(destination);

				if (this.gameObject.transform.localPosition == destination) phase2Location = true;
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
			Vector3 destination = new Vector3(0.0f, 6.66f, -20.0f);

			agent.MoveTo(destination);

			if (this.gameObject.transform.localPosition == destination) phase3Location = true;

			shieldInmunity = true;
		}
		else
		{
			ExplodeBarrels();
			if (shieldInmunity) GenerateBarrels();
			if (barrelCount < 3) barrelCooldown -= Time.deltaTime;
		}

		if (phase3Location) FollowPlayer();
	}

	// Phase 4
	public void Phase4()
	{
		if (tired == false && hitGroundCooldown <= 0.0f && rocksAvailable == false)
		{
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
			ThrowRock();
			throwedRocks++;
			rocksAvailable = false;
		}
		else if (tired == false && throwedRocks < 5 && rocksAvailable == false)
		{
			if (nextRock == null)
			{
				for (int i = 0; i < 5; ++i)
				{
					string rockPrefab = "Rock" + (i + 1);
					nextRock = GameObject.Find(rockPrefab);
					if (nextRock != null)
						break;
				}
			}
			else
			{
				agent.MoveTo(nextRock.transform.localPosition);
			}
		}
		else
		{
			tired = true;
			tiredCooldown -= Time.deltaTime;
			if (tiredCooldown <= 0.0f)
			{
				throwingRocks = false;
				tired = false;
				throwedRocks = 0;
				tiredCooldown = 10.0f;
			}
		}

		if (!tired) SweepAttack();
	}

	private void ThrowRock()
	{
        gameObject.GetComponent<AudioSource>().PlayClip("EBOSS_THROWOBJECT");
        InternalCalls.InstancePrefab("RockBoss");
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
					InternalCalls.InstancePrefab(barrelName);
					GameObject barrel = GameObject.Find(barrelName);
					barrel.GetComponent<Rigidbody>().SetBodyPosition(barrelLocations[i].spawnLocation);
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
			Debug.Log("shield down");
			shieldInmunity = false;
		}
	}

	private bool PerceptionCone(int angleDegrees)
	{
		Vector3 bossPos = gameObject.transform.globalPosition;
		Vector3 bossForward = gameObject.transform.forward;
		Vector3 initPos = new Vector3(bossPos.x + (bossForward.x * offset.x * 0.6f), bossPos.y + 0.1f, bossPos.z + (bossForward.z * offset.z * 0.6f));

		index = RayCast.PerceptionCone(initPos, bossForward, angleDegrees, 16, 8, players, players.Length, colliders, colliders.Length);
		return (index == -1) ? false : true;
	}

	public void GotoNextPoint()
	{
		//gameObject.GetComponent<AudioSource>().PlayClip("FOOTSTEPS");
		//gameObject.GetComponent<Animation>().PlayAnimation("Walk");

		//Debug.Log(waypoints[destPoint].transform.globalPosition.ToString());
		agent.CalculatePath(waypoints[destPoint].transform.globalPosition);
		destPoint = (destPoint + 1) % waypoints.Length;
	}

	private void Patrol()
	{
		agent.MovePath();
		if (!bossStop && (waypoints[destPoint].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude) < 1.5f)
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
			calculatePathCooldown = 3.0f;
		}

		agent.MovePath();
		calculatePathCooldown -= Time.deltaTime;
	}
	public void SweepAttack()
	{
		for (int i = 0; i < players.Length; ++i)
		{
			if (players[i] != null && (players[i].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude) <= 2.0f)
			{
				// Play sweep attack animation

				// Play sweep attack sound

				// Hit player, lower his HP
				players[i].GetComponent<Player>().GetHit(1);
				// Reset Cooldown
				sweepAttackCooldown = 5.0f;
			}
		}
		sweepAttackCooldown -= Time.deltaTime;
	}
	public void GetBackstabbed()
    {
        if (!shieldInmunity)
        {
            state++;
            NextState(); 
        }
	}
	public void OnCollision(Rigidbody other)
	{
		if (other.gameObject.name == "BackStab" && !shieldInmunity)
		{
			state++;
			NextState();
		}
		else if (other.gameObject.tag == "Rocks" && rocksAvailable == false)
		{
            gameObject.GetComponent<AudioSource>().PlayClip("EBOSS_HITGROUND");
            rocksAvailable = true;
			InternalCalls.Destroy(other.gameObject);
			nextRock = null;
		}
	}
}