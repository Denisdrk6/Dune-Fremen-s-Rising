using System;
using RagnarEngine;

public class BackStab : RagnarComponent
{
	public GameObject player;
	public Vector3 pos;
	public GameObject[] enemies;
	public GameObject selectedEnemy;
	public bool backstabed;
	NavAgent agent;
	public GameObject boss;
	
	public GameObject[] barrels;

	// Component list
    private GameObject sceneAudio;
	public void Start()
	{
        Debug.Log("Start Knife");
        sceneAudio = GameObject.Find("AudioLevel1");
        player = GameObject.Find("Player_2");
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
			player.GetComponent<Player>().PlayAudioClip("WPN_CRYSKNIFESTAB");
			Vector3 behind = selectedEnemy.transform.globalPosition - (selectedEnemy.transform.forward * 1);
			behind.y = -0.8f;
			player.GetComponent<Rigidbody>().SetBodyPosition(behind);
			if (selectedEnemy.GetComponent<BasicEnemy>().ToString() == "BasicEnemy" && (selectedEnemy.GetComponent<BasicEnemy>().state != EnemyState.IS_DYING || selectedEnemy.GetComponent<BasicEnemy>().state != EnemyState.DEATH))
			{
                selectedEnemy.GetComponent<BasicEnemy>().pendingToDelete = true;
			}
			if (selectedEnemy.GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy" && (selectedEnemy.GetComponent<UndistractableEnemy>().state != EnemyState.IS_DYING || selectedEnemy.GetComponent<UndistractableEnemy>().state != EnemyState.DEATH))
			{
                selectedEnemy.GetComponent<UndistractableEnemy>().pendingToDelete = true;
			}
			if (selectedEnemy.GetComponent<TankEnemy>().ToString() == "TankEnemy" && (selectedEnemy.GetComponent<TankEnemy>().state != EnemyState.IS_DYING || selectedEnemy.GetComponent<TankEnemy>().state != EnemyState.DEATH))
			{
                selectedEnemy.GetComponent<TankEnemy>().pendingToDelete = true;
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
		}
		if (boss != null)
		{
			player.GetComponent<Player>().PlayAudioClip("WPN_CRYSKNIFESTAB");
			if ((boss.transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude) < 0.5f)
			{
				boss.GetComponent<Boss>().GetBackstabbed();
			}
		}
		InternalCalls.Destroy(gameObject);
  
	}
	public bool Timer() //moltes gracies Isaac
    {
		float time1 = 0.5f;

		if(time1 > 0)
        {
			time1 -= Time.deltaTime;
			return false;
		}
		else
        {
			return true;
		}
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