using System;
using RagnarEngine;

public class BackStab_2 : RagnarComponent
{
	public GameObject player;
	public Vector3 pos;
	public GameObject[] enemies;
	public GameObject selectedEnemy;
	public bool backstabed;
	NavAgent agent;
	public GameObject boss;
    private GameObject sceneAudio;

	public GameObject[] barrels;
	public void Start()
	{
		Debug.Log("Start Knife");
        sceneAudio = GameObject.Find("AudioLevel1");
        player = GameObject.Find("Player");
		pos = player.transform.globalPosition;
		pos.y += 1;
		gameObject.transform.localPosition = pos;
		enemies = GameObject.FindGameObjectsWithTag("Enemies");
		agent = player.GetComponent<NavAgent>();
		selectedEnemy = null;
		backstabed = false;
		boss = GameObject.Find("Boss");
	}
	public void Update()
	{
		selectedEnemy = CalculateDistancePlayerEnemies();
		if (selectedEnemy != null && backstabed == false)
		{
			backstabed = true;

			Vector3 newForward = selectedEnemy.transform.globalPosition - player.transform.globalPosition;
			double angle = Math.Atan2(newForward.x, newForward.z);
			Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
			player.GetComponent<Rigidbody>().SetBodyRotation(rot);
			player.GetComponent<Animation>().PlayAnimation("Ability1");

			player.GetComponent<Player>().PlayAudioClip("WPN_CRYSKNIFESTAB");
			//Vector3 behind = selectedEnemy.transform.globalPosition - (selectedEnemy.transform.forward * 1);
			//player.GetComponent<Rigidbody>().SetBodyPosition(behind);
			if (selectedEnemy.GetComponent<BasicEnemy>().ToString() == "BasicEnemy" && (selectedEnemy.GetComponent<BasicEnemy>().state != EnemyState.IS_DYING || selectedEnemy.GetComponent<BasicEnemy>().state != EnemyState.DEATH))
			{ 
                selectedEnemy.GetComponent<BasicEnemy>().isDying = true;
			}
			else if (selectedEnemy.GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy" && (selectedEnemy.GetComponent<UndistractableEnemy>().state != EnemyState.IS_DYING || selectedEnemy.GetComponent<UndistractableEnemy>().state != EnemyState.DEATH))
			{
                selectedEnemy.GetComponent<UndistractableEnemy>().isDying = true;
			}
			else if (selectedEnemy.GetComponent<TankEnemy>().ToString() == "TankEnemy" && (selectedEnemy.GetComponent<TankEnemy>().state != EnemyState.IS_DYING || selectedEnemy.GetComponent<TankEnemy>().state != EnemyState.DEATH))
			{
                selectedEnemy.GetComponent<TankEnemy>().isDying = true;
			}
			GameObject[] childs = selectedEnemy.childs;
			for (int i = 0; i < childs.Length; ++i)
			{
				if (childs[i].name == "StabParticles")
				{
					childs[i].GetComponent<ParticleSystem>().Play();
					break;
				}
			}
			selectedEnemy.GetComponent<Animation>().PlayAnimation("Dying");
			QuestSystem system = GameObject.Find("Quest System").GetComponent<QuestSystem>();
			system.hasKilledEnemies = true;
			system.killWithPaul = true;
			if (system.camouflageActive)
				system.enemiesCamouflage++;
		}
		if (boss != null)
		{
			Vector3 newForward = boss.transform.globalPosition - player.transform.globalPosition;
			double angle = Math.Atan2(newForward.x, newForward.z);
			Quaternion rot = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
			player.GetComponent<Rigidbody>().SetBodyRotation(rot);
			player.GetComponent<Animation>().PlayAnimation("Ability1");

			player.GetComponent<Player>().PlayAudioClip("WPN_CRYSKNIFESTAB");
			if (Math.Abs(boss.transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude) < 1.5f)
			{
				Debug.Log("Backstabbing");
				boss.GetComponent<Boss>().GetBackstabbed();
			}
		}
		InternalCalls.Destroy(gameObject);

	}
	public GameObject CalculateDistancePlayerEnemies()
	{
		GameObject enemy = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");
		if (enemy != null && Transform.GetDistanceBetween(player.transform.globalPosition, enemy.transform.globalPosition) < 3)
		{
			switch (enemy.GetComponent<BasicEnemy>().state)
			{
				case EnemyState.DEATH:
					return null;
				case EnemyState.IS_DYING:
					return null;
				default:
					return enemy;
			}
		}

		barrels = GameObject.FindGameObjectsWithTag("Barrels");
		Debug.Log(barrels.Length);
		for (int i = 0; i < barrels.Length; ++i)
		{
			if (Math.Abs((barrels[i].transform.globalPosition - gameObject.transform.globalPosition).magnitude) <= 3.0f)
			{
				barrels[i].GetComponent<Barrel>().Explode();
			}
		}

		return null;
	}

}