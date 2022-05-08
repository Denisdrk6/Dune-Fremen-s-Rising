using System;
using RagnarEngine;

public class Voice : RagnarComponent
{
	public GameObject player;
	public GameObject selectedEnemy;
	public GameObject[] enemies;
	public PlayerManager playerManager;
	NavAgent agent;
	public void Start()
	{
		player = GameObject.Find("Player");
		enemies = GameObject.FindGameObjectsWithTag("Enemies");
		playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
		agent = player.GetComponent<NavAgent>();
	}
	public void Update()
	{
		Debug.Log(enemies.Length.ToString());
		selectedEnemy = EnemyFound();
		if (selectedEnemy != null)
		{
			Debug.Log("A");
			AddNewEnemyToPlayer();
		}
		InternalCalls.Destroy(gameObject);
	}
	public GameObject EnemyFound()
	{
		//for (int i = 0; i < enemies.Length; i++)
		//{
		//	Vector3 enemyPos = enemies[i].transform.globalPosition;
		//	Vector3 distance = player.transform.globalPosition - enemyPos;
		//	distance.y = 0;
		//	if (distance.magnitude <= 5)
		//	{
		//		return enemies[i];
		//	}
		//}
		//return null;
		//index = RayCast.PerceptionCone(player.transform.globalPosition, player.transform.forward, 60, 10, 5, enemies, enemies.Length);
		//Debug.Log(index.ToString());
		return RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");
		
	}
	public void AddNewEnemyToPlayer()
    {
		Debug.Log("Is Changing");
		playerManager.players[playerManager.characterSelected].GetComponent<Player>().SetControled(false);
		if(selectedEnemy.GetComponent<BasicEnemy>().ToString() == "BasicEnemy")	selectedEnemy.GetComponent<BasicEnemy>().SetControled(true);
		if(selectedEnemy.GetComponent<TankEnemy>().ToString() == "TankEnemy")	selectedEnemy.GetComponent<TankEnemy>().SetControled(true);
		if(selectedEnemy.GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy")	selectedEnemy.GetComponent<UndistractableEnemy>().SetControled(true);
		Debug.Log("Is Changed");
	}
}

