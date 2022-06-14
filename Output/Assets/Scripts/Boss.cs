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

	// All-purpose mechanics
	private bool inmunity = false;
	private bool bossAttack = false;

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
	bool barrelHit = false;

	private int destPoint = 0;

	// Phase 4 mechanics
	// Cinematic for throwing paul
	bool finishedCinematic = false;
	bool grabbedPaul = false;
	bool throwedPaul = false;
	bool reachedMiddleLocation = false;
	float waitToStart = 2.5f;
	Vector3 paulLocation;
	Vector3 throwingLocation;
	Vector3 throwingLocationMiddle;
	Vector3 newBossLocation;

	int throwedRocks = 0;
	float hitGroundCooldown = 10.0f;
	float shieldCooldown = 10.0f;
	float tiredCooldown = 10.0f;
	float throwRockTiming = 2.5f;
	
	bool rocksAvailable = false;
	bool rocksFalling = false;
	bool throwingRocks = false;
	bool throwing = false;
	bool tired = false;
	GameObject nextRock;
	float sweepAttackCooldown = 10.0f;

	GameObject[] waypoints = new GameObject[3];
	bool bossStop = false;
	float countDown = 5.0f;

	bool hitted = false;

	float cooldownCharge = 4.0f;
	public bool charging = false;
	Vector3 playerPos = new Vector3(0.0f, 0.0f, 0.0f);

	float enemiesCooldown = 20.0f;
	bool attacking = false;

	string[] animations = new string[14];
	int indexAnim = 0;

	// Boss UI
	bool bossActive = false;
	GameObject life;
	GameObject shieldBar;

	float coneTimer = 0.0f;
	float coneMaxTime = 0.5f;
	int frames = 0;

	public void Start()
	{
		// Get Components
		animationComponent = gameObject.GetComponent<Animation>();
		stabParticles = GameObject.Find("StabParticlesBoss").GetComponent<ParticleSystem>();
		shieldParticles = GameObject.Find("BossShieldParticles").GetComponent<ParticleSystem>();
		rb = gameObject.GetComponent<Rigidbody>();

		life = GameObject.Find("Life");
		life.GetComponent<Transform2D>().position2D = new Vector3(0, (0.5f * InternalCalls.GetRegionGame().y) - 70, 0);
		shieldBar = GameObject.Find("ShieldBar");
		shieldBar.GetComponent<Transform2D>().position2D = new Vector3(0, (0.5f * InternalCalls.GetRegionGame().y) - 90, 0);

		animations[0] = "Attack";
		animations[1] = "ChargeStart";
		animations[2] = "ChargeIdle";
		animations[3] = "SprintHit";
		animations[4] = "Hit";
		animations[5] = "Idle";
		animations[6] = "Standup";
		//animations[3] = "Exhausted";
		//animations[4] = "HitGround";
		//animations[5] = "Ntoa";
		//animations[6] = "Run";
		//animations[7] = "Shield";
		//animations[8] = "ShieldDestroy";
		//animations[9] = "ShotCrazy";
		//animations[10] = "SitIdle";
		//animations[11] = "Throw";
		//animations[12] = "WalkAngry";
		//animations[13] = "WalkNormal";
		//animations[13] = "";

		throwingLocation = new Vector3(0.0f, 9.14f, -63.49f);
		paulLocation = new Vector3(0.0f, 9.14f, -26.24f);
		throwingLocationMiddle = new Vector3(0.0f, 9.14f, -39.74f);
		newBossLocation = new Vector3(0.0f, 9.14f, -70.80f);
		colliders = GameObject.FindGameObjectsWithTag("Collider");
		material = gameObject.GetComponent<Material>();
		state = BossState.PHASE1;

		offset = gameObject.GetSizeAABB();

		waypoints[0] = GameObject.Find("28");
		waypoints[1] = GameObject.Find("29");
		waypoints[2] = GameObject.Find("30");

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
		frames++;
		if (Input.GetKey(KeyCode.N) == KeyState.KEY_DOWN)
		{
			state += 1;
			NextState();
		}

		if (!hitted)
		{
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
		}
        else
		{
			inmunity = true;
			if (animationComponent.HasFinished())
			{
				hitted = false;
				if (state == BossState.PHASE4)
				{
					GameObject.Find("Quest System").GetComponent<QuestSystem>().lastPhaseCompleted = true;
                    GameObject.Find("Quest System").GetComponent<QuestSystem>().levelFinished = true;
                    GameObject.Find("DialogueTrigger11").GetComponent<DialogueTrigger>().SetUsed(false);
                    GameObject.Find("DialogueTrigger11").GetComponent<DialogueTrigger>().ActiveDialoguebyID(9);
                    life.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_boss_lifebar0.png");
					//InternalCalls.Destroy(gameObject);
				}
				else
				{
					state++;
					NextState();
				}
			}
		}

		if (Input.GetKey(KeyCode.M) == KeyState.KEY_DOWN)
		{
			string anim = animations[indexAnim];
			gameObject.GetComponent<Animation>().PlayAnimation(anim);
			indexAnim++;
			//GenerateEnemies();
			if (indexAnim == 7)
			{
				indexAnim = 0;
			}

		}

	}

	private void NextState()
	{
		switch (state)
		{
			case BossState.PHASE2:
				GameObject.Find("Quest System").GetComponent<QuestSystem>().firstPhaseCompleted = true;
				life.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_boss_lifebar3.png");
				//GenerateEnemies();
				players = GameObject.FindGameObjectsWithTag("Player");
				agent.speed = GameObject.Find("Player").GetComponent<NavAgent>().speed * 0.5f;
				GotoNextPoint();
				break;
			case BossState.PHASE3:
				GameObject.Find("BossShieldParticles").GetComponent<ParticleSystem>().Play();
				agent.speed = GameObject.Find("Player").GetComponent<NavAgent>().speed * 0.75f;
				barrelCooldown = 0.0f;
				GenerateBarrels();
				
				life.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_boss_lifebar2.png");
				shieldBar.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_boss_shieldbar1.png");
				shieldBar.GetComponent<UIImage>().SetImageAlpha(1.0f);

				break;
			case BossState.PHASE4:
				if (!shieldInmunity)
				{
					agent.speed = GameObject.Find("Player").GetComponent<NavAgent>().speed * 1.2f;
					animationComponent.PlayAnimation("Run");
					sweepAttackCooldown = 5.0f;
					life.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_boss_lifebar1.png");
				}
				else state--;
				break;
			default:
				InternalCalls.Destroy(gameObject);
				break;
		}
	}

	private void GenerateEnemies()
	{
		animationComponent.PlayAnimation("CallBackup");

		if (GameObject.Find("Basic Enemy 15") == null)
		{
			GameObject enemy1 = InternalCalls.InstancePrefab("Basic Enemy 15", new Vector3(5.56f, 9.34f, -53.60f));
			enemy1.GetComponent<BasicEnemy>().state = EnemyState.IDLE;
			enemy1.GetComponent<BasicEnemy>().enemyType = EnemyType.BASIC;
			enemy1.GetComponent<BasicEnemy>().colliders = GameObject.FindGameObjectsWithTag("Collider");
			enemy1.GetComponent<BasicEnemy>().coneRotate = true;
			enemy1.GetComponent<BasicEnemy>().waypoints = GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemies[0].waypoints;
		}

		if (GameObject.Find("Basic Enemy 16") == null)
		{
			GameObject enemy2 = InternalCalls.InstancePrefab("Basic Enemy 16", new Vector3(-5.56f, 9.34f, -53.60f));
			enemy2.GetComponent<BasicEnemy>().state = EnemyState.IDLE;
			enemy2.GetComponent<BasicEnemy>().enemyType = EnemyType.BASIC;
			enemy2.GetComponent<BasicEnemy>().colliders = GameObject.FindGameObjectsWithTag("Collider");
			enemy2.GetComponent<BasicEnemy>().coneRotate = false;
			enemy2.GetComponent<BasicEnemy>().waypoints = GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemies[0].waypoints;
		}
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
		if (phase2Location == false)
		{
			// Move to new location as soon as phase 2 starts
			Vector3 destination = waypoints[2].transform.globalPosition;

			if (animationComponent.HasFinished())
            {
				animationComponent.PlayAnimation("Run");
				agent.speed = 15.0f;
			}
			agent.MoveTo(destination);

			if (Math.Abs((gameObject.transform.localPosition - destination).magnitude) < 1.0f)
			{
				Debug.Log("INmunity false phase 2");
				agent.speed = GameObject.Find("Player").GetComponent<NavAgent>().speed * 0.5f;
				animationComponent.PlayAnimation("WalkNormal");
				inmunity = false;
				phase2Location = true;
			}

		}
		else
		{
			enemiesCooldown -= Time.deltaTime;
			if (enemiesCooldown <= 0.0f)
            {
				//GenerateEnemies();
				enemiesCooldown = 20.0f;
            }

			sweepAttackCooldown -= Time.deltaTime;

			if(frames % 4 == 0)
            {
				if (PerceptionCone(90))
				{
					coneTimer += Time.deltaTime * 4;
					if (!jumping && sweepAttackCooldown <= 0.0f && coneTimer >= coneMaxTime)
					{
						float furthest = 1000.0f;

						for (int i = 0; i < players.Length; ++i)
						{
							// Calculate closest player
							float distance = Math.Abs(players[i].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude);
							if (distance < furthest)
							{
								indexPlayerTarget = i;
								furthest = distance;
							}
						}
						jumping = true;
						agent.speed = 25.0f;
						animationComponent.PlayAnimation("Run");
					}
					else if (jumping && !attacking)
					{
						if (players[indexPlayerTarget] != null)
						{
							agent.MoveTo(players[indexPlayerTarget].transform.globalPosition);
							if (Math.Abs(players[indexPlayerTarget].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude) <= 2.0f)
							{
								// Play sweep attack animation

								// Play sweep attack sound
								// Hit player, lower his HP
								players[indexPlayerTarget].GetComponent<Rigidbody>().SetBodyPosition(gameObject.transform.globalPosition + (gameObject.transform.forward * 1.5f));

								players[indexPlayerTarget].GetComponent<Player>().stunned = true;
								players[indexPlayerTarget].GetComponent<NavAgent>().ClearPath();
								agent.ClearPath();
								// TODO: Needs attack animation
								animationComponent.PlayAnimation("Attack");
								attacking = true;
							}
						}
					}
				}
				else
				{
					coneTimer -= Time.deltaTime * 4;
					if (coneTimer < 0) coneTimer = 0;
				}
			}			

			if (attacking && animationComponent.HasFinished())
            {
				sweepAttackCooldown = 10.0f;
				if (!bossStop) agent.CalculatePath(waypoints[destPoint].transform.globalPosition);
				agent.speed = GameObject.Find("Player").GetComponent<NavAgent>().speed * 0.5f;
				players[indexPlayerTarget].GetComponent<Player>().GetHit(1);
				players[indexPlayerTarget].GetComponent<Player>().stunned = false;
				animationComponent.PlayAnimation("WalkNormal");
				attacking = false;
				jumping = false;
			}

			if (!jumping && !attacking) Patrol();
		}
	}

	// Phase 3
	public void Phase3()
	{
        if (phase3Location == false)
        {
            // Move to new location as soon as phase 3 starts
            Vector3 destination = new Vector3(0.0f, 9.24f, -58.0f);

			if (animationComponent.HasFinished())
			{
				animationComponent.PlayAnimation("Run");
				agent.speed = 15.0f;
			}
			agent.MoveTo(destination);

			if (Math.Abs((gameObject.transform.localPosition - destination).magnitude) < 2.0f)
			{
				Debug.Log("INmunity false phase 3");
				inmunity = false;
				phase3Location = true;
				agent.speed = GameObject.Find("Player").GetComponent<NavAgent>().speed * 0.5f;
			}

            shieldInmunity = true;
		}
        else
        {
            //Debug.Log("Needs to be stunned");

			ExplodeBarrels();
			if (shieldInmunity) GenerateBarrels();
			if (stunnedHits < 3 && barrelCount < 3)
			{
				barrelCooldown -= Time.deltaTime;
			}

			if (shieldInmunity)
			{
				if (!charging && !attacking && !barrelHit)
				{
					//FollowPlayer();
					//Vector3 lastPos = playerPos;
					playerPos = players[0].transform.localPosition;

					Vector3 direction = players[0].transform.localPosition - gameObject.transform.localPosition;
					double angle = Math.Atan2(direction.x, direction.z);
					Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
					rb.SetBodyRotation(rot);
					//bool samePosition = (lastPos.x == playerPos.x && lastPos.y == playerPos.y && lastPos.z == playerPos.z);
					//float distance = Math.Abs(gameObject.transform.globalPosition.magnitude - playerPos.magnitude);

					//if (samePosition && distance <= 30.0f)
					//{
						cooldownCharge -= Time.deltaTime;
						//Debug.Log(cooldownCharge.ToString());
						if (cooldownCharge <= 0.0f)
						{
							charging = true;
							agent.speed = 30.0f;
							animationComponent.PlayAnimation("ChargeStart");
							agent.CalculatePath(playerPos);
						}
					//}
     //               else if (!samePosition)
     //               {
					//	cooldownCharge = 1.0f;
     //               }
				}
				else if (charging && !attacking)
				{
					if (animationComponent.HasFinished()) animationComponent.PlayAnimation("ChargeIdle");
					agent.MovePath();
					//agent.MoveTo(playerPos);
					Vector3 playerCurrentPos = players[0].transform.globalPosition;
					playerCurrentPos.y = 0;

					Vector3 bossCurrentPos = gameObject.transform.globalPosition;
					bossCurrentPos.y = 0;

					
					if (Math.Abs((playerCurrentPos - bossCurrentPos).magnitude) <= 3.0f)
					{
						float testing2 = Math.Abs((playerCurrentPos - bossCurrentPos).magnitude);
						Debug.Log("Grabbed Paul, distance is  " + testing2.ToString());
						// Reset charging
						charging = false;
						agent.ClearPath();
						cooldownCharge = 4.0f;
						agent.speed = players[0].GetComponent<NavAgent>().speed * 0.75f;

						players[0].GetComponent<Rigidbody>().SetBodyPosition(gameObject.transform.globalPosition + (gameObject.transform.forward * 1.5f));

						players[0].GetComponent<Player>().GetHit(1);
						players[0].GetComponent<NavAgent>().ClearPath();
						attacking = true;
						animationComponent.PlayAnimation("Attack");
					}
					playerPos.y = 0;
					if (Math.Abs((playerPos - bossCurrentPos).magnitude) <= 1.2f)
					{
						float testing = Math.Abs((playerPos - bossCurrentPos).magnitude);
						Debug.Log("Distance is " + testing.ToString());
						agent.speed = players[0].GetComponent<NavAgent>().speed * 0.75f;
						charging = false;
						agent.ClearPath();
						animationComponent.PlayAnimation("ChargeEnd");
						cooldownCharge = 4.0f;	
					}
				}

				if (attacking && animationComponent.HasFinished())
                {
					attacking = false;	
				}
			}
			else
			{
				shieldCooldown -= Time.deltaTime;
				if (shieldCooldown <= 0.0f)
				{
					shieldCooldown = 10.0f;
					shieldBar.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_boss_shieldbar1.png");
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
				if (!rocksFalling)
				{
					animationComponent.PlayAnimation("HitGround");
					rocksFalling = true;
				}
				else if (animationComponent.HasFinished())
				{
					GenerateRocks();
					hitGroundCooldown = 10.0f;
					throwingRocks = true;
					rocksFalling = false;
				}
			}
			else if (!throwingRocks && hitGroundCooldown > 0.0f && !tired)
			{
				FollowPlayer();
				hitGroundCooldown -= Time.deltaTime;
			}
			else if (tired == false && rocksAvailable == true)
			{
				Vector3 direction = players[0].transform.localPosition - gameObject.transform.localPosition;
				double angle = Math.Atan2(direction.x, direction.z);
				Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
				rb.SetBodyRotation(rot);
				if (!throwing)
                {
					animationComponent.PlayAnimation("Throw");
					throwing = true;
                }
				else if (throwRockTiming <= 0.0f)
                {
					ThrowRock();
					throwedRocks++;
					Debug.Log("Finished Throwing");
					throwing = false;
					rocksAvailable = false;
					throwRockTiming = 3.5f;
				}

				throwRockTiming -= Time.deltaTime;
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
			players[0].GetComponent<Player>().stunned = true;
			if (!grabbedPaul)
			{
				if (Math.Abs(players[0].transform.globalPosition.magnitude - paulLocation.magnitude) >= 1.5f)
                {
					Animation paulAnim = players[0].GetComponent<Animation>();
					if (paulAnim.HasFinished()) paulAnim.PlayAnimation("Walk");
					players[0].GetComponent<NavAgent>().CalculatePath(paulLocation);
					players[0].GetComponent<NavAgent>().MovePath();
				}
				else
                {
					if (animationComponent.HasFinished()) animationComponent.PlayAnimation("WalkAngry");
					FollowPlayer();
					float magnitude = Math.Abs((players[0].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude));
					if (magnitude <= 1.5f) grabbedPaul = true;
				}	
			}
			else
			{
				if (!throwedPaul)
				{
					if (!reachedMiddleLocation) agent.CalculatePath(throwingLocationMiddle);
					else agent.CalculatePath(throwingLocation);

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
                    {
						if (waitToStart <= 0.0f)
                        {
							players[0].GetComponent<Player>().stunned = false;
							finishedCinematic = true;
							inmunity = false;
							InternalCalls.InstancePrefab("BossColliderPhase4", new Vector3(-0.18f, 12.55f, -66.08f));
						}
						waitToStart -= Time.deltaTime;
					}
				}
				if (Math.Abs((throwingLocationMiddle.magnitude - gameObject.transform.globalPosition.magnitude)) <= 1.0f) reachedMiddleLocation = true;
				else if (Math.Abs((throwingLocation.magnitude - gameObject.transform.globalPosition.magnitude)) <= 1.0f)
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
		if (stunnedHits == 1)
		{
			if (!barrelHit && animationComponent.HasFinished())
			{
				animationComponent.PlayAnimation("SprintHit");
				barrelHit = true;
				charging = false;
				attacking = false;
			}
			else if (barrelHit && animationComponent.HasFinished())
			{
				animationComponent.PlayAnimation("ShieldDestroy");
				shieldInmunity = false;
				shieldParticles.Pause();
				stunnedHits = 0;
				cooldownCharge = 4.0f;
			}
		}
	}

	public void SetBattle()
    {
		if (bossActive) return;

		bossActive = true;
		life.GetComponent<UIImage>().SetImageAlpha(1.0f);
	}

	private bool PerceptionCone(int angleDegrees)
	{
		Vector3 bossPos = gameObject.transform.globalPosition;
		Vector3 bossForward = gameObject.transform.forward;
		Vector3 initPos = new Vector3(bossPos.x + (bossForward.x * offset.x * 0.6f), bossPos.y + 0.1f, bossPos.z + (bossForward.z * offset.z * 0.6f));

		index = RayCast.PerceptionCone(initPos, bossForward, angleDegrees, 16, 8, players, players.Length, "Collider", coneTimer / coneMaxTime);
		if (index != -1 && (players[index].GetComponent<Player>().invisible || players[index].GetComponent<Player>().dead || players[index].GetComponent<Player>().isHidden)) return false;
		return (index == -1) ? false : true;
	}

	public void GotoNextPoint()
	{	
		destPoint = (destPoint + 1) % waypoints.Length;
		agent.CalculatePath(waypoints[destPoint].transform.globalPosition);
	}

	private void Patrol()
	{
		if (!bossStop && Math.Abs((waypoints[destPoint].transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude)) < 1.5f)
		{
			Debug.Log("Stopped");
			bossStop = true;
			animationComponent.PlayAnimation("Idle");
		}
		else if (!bossStop)
		{
			agent.MovePath();
		}

		if (bossStop)
		{
			countDown -= Time.deltaTime;
			if (countDown <= 0.0f)
			{
				bossStop = false;
				countDown = 5.0f;
				GotoNextPoint();
				animationComponent.PlayAnimation("WalkNormal");
			}
		}
	}

	private void FollowPlayer()
	{
		if (!bossAttack)
		{
			if (calculatePathCooldown <= 0.0f)
			{
				agent.CalculatePath(players[0].transform.localPosition);
				calculatePathCooldown = 2.0f;
			}

			agent.MovePath();
			calculatePathCooldown -= Time.deltaTime;
		}
		else if (animationComponent.HasFinished()) bossAttack = false;

	}
	public void SweepAttack()
	{
		if (players[0] != null && Math.Abs((players[0].transform.globalPosition - gameObject.transform.globalPosition).magnitude) <= 1.5f)
		{
			// Play sweep attack animation
			animationComponent.PlayAnimation("Attack");
			// Play sweep attack sound
			Debug.Log("SWEEP ATTACK");
			// Hit player, lower his HP
			players[0].GetComponent<Player>().GetHit(1);
			// Reset Cooldown
			sweepAttackCooldown = 5.0f;

			InternalCalls.RequestDamageFeedback();

			bossAttack = true;
		}
	}
	public void GetBackstabbed()
	{
		Debug.Log(shieldInmunity.ToString());
		Debug.Log(inmunity.ToString());
		if (!shieldInmunity && !inmunity)
		{
			if (state == BossState.PHASE1)
			{
				animationComponent.PlayAnimation("Standup");
			}
			else if (state == BossState.PHASE4)
            {
				Debug.Log("EEEEE");
				animationComponent.PlayAnimation("Execdie");
			}
			else
            {
				Debug.Log("BACKSTABBED");
				animationComponent.PlayAnimation("Hit");
			}

			if (state == BossState.PHASE3)
            {
				shieldBar.GetComponent<Material>().SetTexturePath("Assets/Resources/UI/ui_boss_shieldbar0.png");
			}
			stabParticles.Play();
			hitted = true;
		}
	}
	public void OnCollision(Rigidbody other)
	{
		if (other.gameObject.name == "BackStab" && !shieldInmunity)
		{
			state++;
			animationComponent.PlayAnimation("Hit");
			hitted = true;
			if (state == BossState.PHASE4)
			{
				Input.RestoreDefaultCursor();
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