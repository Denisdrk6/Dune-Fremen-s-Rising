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
		//Debug.Log(enemies.Length.ToString());
		selectedEnemy = EnemyFound();
		if (selectedEnemy != null)
		{
			AddNewEnemyToPlayer();
			selectedEnemy.GetComponent<BasicEnemy>().initialPos = selectedEnemy.transform.globalPosition;
			selectedEnemy.GetComponent<BasicEnemy>().initialRot = selectedEnemy.transform.globalRotation;
		}
		InternalCalls.Destroy(gameObject);
	}
	public GameObject EnemyFound()
	{
		GameObject enemy = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");
		if (enemy != null && Transform.GetDistanceBetween(player.transform.globalPosition, enemy.transform.globalPosition) < 15) return enemy;
		GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[1].cooldown = 0;
		return null;

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

