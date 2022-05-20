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
			BasicEnemy enemyScript = selectedEnemy.GetComponent<BasicEnemy>();
			AddNewEnemyToPlayer();
			enemyScript.initialPos = selectedEnemy.transform.globalPosition;
			enemyScript.initialRot = selectedEnemy.transform.globalRotation;
		}
		InternalCalls.Destroy(gameObject);
	}
	public GameObject EnemyFound()
	{
		GameObject enemy = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");
		if (enemy != null && Transform.GetDistanceBetween(player.transform.globalPosition, enemy.transform.globalPosition) < 15)
        {
			switch(enemy.GetComponent<BasicEnemy>().state)
            {
				case EnemyState.DEATH:
					GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[1].cooldown = 0;
					return null;
				case EnemyState.IS_DYING:
					GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[1].cooldown = 0;
					return null;
				default:
					return enemy;
			}			
        }
		GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters[0].abilities[1].cooldown = 0;
		return null;

	}
	public void ActivateVoice()
    {
		Debug.Log("Is Changing");
		
		playerManager.players[playerManager.characterSelected].GetComponent<Player>().SetControled(false);
		if (selectedEnemy.GetComponent<BasicEnemy>().ToString() == "BasicEnemy") selectedEnemy.GetComponent<BasicEnemy>().SetControled(true);
		if (selectedEnemy.GetComponent<TankEnemy>().ToString() == "TankEnemy") selectedEnemy.GetComponent<TankEnemy>().SetControled(true);
		if (selectedEnemy.GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy") selectedEnemy.GetComponent<UndistractableEnemy>().SetControled(true);
		
		Debug.Log("Is Changed");
	}
}

