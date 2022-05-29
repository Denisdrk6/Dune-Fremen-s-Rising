using System;
using RagnarEngine;

public class BackStabEnemy : RagnarComponent
{
	public GameObject enemyPlayer;
	public Vector3 pos;
	public GameObject[] enemies;
	public GameObject selectedEnemy;
	public bool backstabed;
	NavAgent agent;
	public GameObject boss;
	public void Start()
	{
		Debug.Log("Start Knife");
		enemies = GameObject.FindGameObjectsWithTag("Enemies");
		enemyPlayer = GetEnemy();
		enemyPlayer.tag = "wwwwwwww";
		//Debug.Log(enemyPlayer.name.ToString());
		pos = enemyPlayer.transform.globalPosition;
		pos.y += 1;
		gameObject.transform.localPosition = pos;
		agent = enemyPlayer.GetComponent<NavAgent>();
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
			Vector3 behind = selectedEnemy.transform.globalPosition - (selectedEnemy.transform.forward * 1);
			behind.y = -0.8f;
			enemyPlayer.GetComponent<Rigidbody>().SetBodyPosition(behind);
			if (selectedEnemy.GetComponent<BasicEnemy>().ToString() == "BasicEnemy")
			{
				selectedEnemy.GetComponent<BasicEnemy>().pendingToDelete = true;
			}
			if (selectedEnemy.GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy")
			{
				selectedEnemy.GetComponent<UndistractableEnemy>().pendingToDelete = true;
			}
			if (selectedEnemy.GetComponent<TankEnemy>().ToString() == "TankEnemy")
			{
				selectedEnemy.GetComponent<TankEnemy>().pendingToDelete = true;
			}
			selectedEnemy.GetComponent<Animation>().PlayAnimation("Dying");
		}
		if (boss != null)
		{
			if ((boss.transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude) < 0.5f)
			{
				boss.GetComponent<Boss>().GetBackstabbed();
			}
		}
		enemyPlayer.tag = "Enemies";
		InternalCalls.Destroy(gameObject);

	}
	public GameObject GetEnemy()
    {
		for (int i = 0; i < enemies.Length; i++)
		{
			if (enemies[i].GetComponent<BasicEnemy>().ToString() == "BasicEnemy" && enemies[i].GetComponent<BasicEnemy>().controlled) return enemies[i];
			else if (enemies[i].GetComponent<TankEnemy>().ToString() == "TankEnemy" && enemies[i].GetComponent<TankEnemy>().controlled) return enemies[i];
			else if (enemies[i].GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy" && enemies[i].GetComponent<UndistractableEnemy>().controlled) return enemies[i];
		}
		return null;
	}
	public bool Timer() //moltes gracies Isaac
	{
		float time1 = 0.5f;

		if (time1 > 0)
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
		if (enemy != null && Transform.GetDistanceBetween(enemyPlayer.transform.globalPosition, enemy.transform.globalPosition) < 3)
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
		return null;
	}

}
