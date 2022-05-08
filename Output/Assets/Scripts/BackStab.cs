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
	public void Start()
	{
		Debug.Log("Start Knife");
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
			Vector3 behind = selectedEnemy.transform.globalPosition - (selectedEnemy.transform.forward * 1);
			behind.y = -0.8f;
			player.GetComponent<Rigidbody>().SetBodyPosition(behind);
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
			if ((boss.transform.globalPosition.magnitude - gameObject.transform.globalPosition.magnitude) < 3.0f)
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
		
		return RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");
	}

}