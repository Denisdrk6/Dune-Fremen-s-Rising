using System;
using RagnarEngine;

public class EnemyManager : RagnarComponent
{
    public Enemies[] enemies;
    public GameObject[] enemyGOs;
    public GameObject[] deadEnemies;
    public GameObject[] colliders;

    public void Start()
    {
        foreach (Enemies e in enemies)
        {
            InternalCalls.InstancePrefab(e.prefabPath);
        }

        enemyGOs = GameObject.FindGameObjectsWithTag("Enemies");
        colliders = GameObject.FindGameObjectsWithTag("Collider");

        for(int i = 0; i < enemyGOs.Length; i++)
        {
            enemyGOs[i].name = enemies[i].name;
            if (enemies[i].waypoints.Length != 0)
            {
                switch (enemies[i].type)
                {
                    case EnemyType.BASIC:
                        enemyGOs[i].GetComponent<BasicEnemy>().waypoints = enemies[i].waypoints;
                        enemyGOs[i].GetComponent<BasicEnemy>().state = EnemyState.IDLE;
                        enemyGOs[i].GetComponent<BasicEnemy>().colliders = colliders;
                        break;
                    case EnemyType.TANK:
                        enemyGOs[i].GetComponent<TankEnemy>().waypoints = enemies[i].waypoints;
                        enemyGOs[i].GetComponent<TankEnemy>().state = EnemyState.IDLE;
                        enemyGOs[i].GetComponent<TankEnemy>().colliders = colliders;
                        break;
                    case EnemyType.UNDISTRACTABLE:
                        enemyGOs[i].GetComponent<UndistractableEnemy>().waypoints = enemies[i].waypoints;
                        enemyGOs[i].GetComponent<UndistractableEnemy>().state = EnemyState.IDLE;
                        enemyGOs[i].GetComponent<UndistractableEnemy>().colliders = colliders;
                        break;
                    case EnemyType.AIR:
                        enemyGOs[i].GetComponent<AirEnemy>().waypoints = enemies[i].waypoints;
                        enemyGOs[i].GetComponent<AirEnemy>().state = EnemyState.IDLE;
                        enemyGOs[i].GetComponent<AirEnemy>().colliders = colliders;
                        break;
                }

                enemies[i].state = EnemyState.IDLE;
            }
            enemyGOs[i].GetComponent<Rigidbody>().SetBodyPosition(enemies[i].pos);
        }

        deadEnemies = new GameObject[enemyGOs.Length];
    }
    public void Update()
    {
        // Death Control
        if(enemyGOs.Length > 0)
        {
            foreach(GameObject go in enemyGOs)
            {
                if((go.GetComponent<BasicEnemy>().pendingToDelete && go.GetComponent<BasicEnemy>().ToString() == "BasicEnemy") || (go.GetComponent<AirEnemy>().pendingToDelete && go.GetComponent<AirEnemy>().ToString() == "AirEnemy") || (go.GetComponent<TankEnemy>().pendingToDelete && go.GetComponent<TankEnemy>().ToString() == "TankEnemy") || (go.GetComponent<UndistractableEnemy>().pendingToDelete && go.GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy"))
                {
                    GameObject[] aux = new GameObject[enemyGOs.Length-1];
                    Enemies[] aux2 = new Enemies[enemies.Length-1];
                    for (int i = 0, j = 0; i < enemyGOs.Length; i++, j++)
                    {
                        if (enemyGOs[i] == go)
                        {
                            j--;
                            for(int k = 0; k < deadEnemies.Length; k++)
                            {
                                if (deadEnemies[k] == null) deadEnemies[k] = go;
                            }
                            enemies[i].state = EnemyState.DEATH;
                            ChangeEnemyState(enemyGOs[i], EnemyState.DEATH);
                        }
                        else
                        {
                            aux[j] = enemyGOs[i];
                            aux2[j] = enemies[i];
                        }
                    }
                    enemyGOs = aux;
                    enemies = aux2;
                }
            }
        }
    }

    private void ChangeEnemyState(GameObject go, EnemyState newState)
    {
        if (go.GetComponent<AirEnemy>().state != newState && go.GetComponent<AirEnemy>().ToString() == "AirEnemy")
        {
            go.GetComponent<AirEnemy>().state = newState;
        }
        if (go.GetComponent<BasicEnemy>().state != newState && go.GetComponent<BasicEnemy>().ToString() == "BasicEnemy")
        {
            go.GetComponent<BasicEnemy>().state = newState;
        }
        if (go.GetComponent<TankEnemy>().state != newState && go.GetComponent<TankEnemy>().ToString() == "TankEnemy")
        {
            go.GetComponent<TankEnemy>().state = newState;
        }
        if (go.GetComponent<UndistractableEnemy>().state != newState && go.GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy")
        {
            go.GetComponent<UndistractableEnemy>().state = newState;
        }
    }
}