using System;
using System.Collections.Generic;
using RagnarEngine;

public class EnemyManager : RagnarComponent
{
    public Enemies[] enemies;
    public List<GameObject> enemyGOs = new List<GameObject>();
    public List<GameObject> deadEnemies = new List<GameObject>();
    public GameObject[] colliders;
    public int enemyCount = 0;

    public void Start()
    {
        enemyGOs.Clear();
        deadEnemies.Clear();
        foreach (Enemies e in enemies)
        {
            enemyGOs.Add(InternalCalls.InstancePrefab(e.prefabPath));
        }

        colliders = GameObject.FindGameObjectsWithTag("Collider");

        for(int i = 0; i < enemyGOs.Count; i++)
        {
            enemyGOs[i].name = enemies[i].name;
            enemyGOs[i].SubmitOutlineDrawing(new Vector3(1, 0, 0));
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

            enemyGOs[i].GetComponent<Rigidbody>().SetBodyPosition(enemies[i].pos);
        }

        if (SaveSystem.fromContinue)
        {
            LoadEnemy();
        }
    }
    public void Update()
    {
        // Death Control
        if(enemyGOs.Count > 0)
        {
            foreach(GameObject go in enemyGOs)
            {
                if((go.GetComponent<BasicEnemy>().pendingToDelete && go.GetComponent<BasicEnemy>().ToString() == "BasicEnemy") || (go.GetComponent<AirEnemy>().pendingToDelete && go.GetComponent<AirEnemy>().ToString() == "AirEnemy") || (go.GetComponent<TankEnemy>().pendingToDelete && go.GetComponent<TankEnemy>().ToString() == "TankEnemy") || (go.GetComponent<UndistractableEnemy>().pendingToDelete && go.GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy"))
                {
                    for (int i = 0; i < enemyGOs.Count; i++)
                    {
                        if (enemyGOs[i] == go)
                        {
                            deadEnemies.Add(enemyGOs[i]);
                            enemyGOs[i].DeleteComponent<Rigidbody>(enemyGOs[i].GetComponent<Rigidbody>());
                            ChangeEnemyState(enemyGOs[i], EnemyState.DEATH);
                            enemyCount++;
                            enemies[i].state = EnemyState.DEATH;
                            enemyGOs[i].isInteractuable = true;
                            enemyGOs[i].interactuableColor = new Vector3(0, 0, 1);
                        }
                    }
                }
            }
        }

        foreach (GameObject de in deadEnemies)
        {
            Vector3 maxPoint = de.GetMaxAABB();
            maxPoint.x *= 1.08f;
            maxPoint.y *= 0.25f;
            maxPoint.z *= 1f;

            Vector3 minPoint = de.GetMinAABB();
            minPoint.x *= 0.98f;
            minPoint.y *= 1f;
            minPoint.z *= 1.015f;

            de.SetSizeAABB(minPoint, maxPoint);
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

    public void SaveEnemies()
    {
        SaveSystem.DeleteDirectoryFiles("Library/SavedGame/Enemies");
        for (int i = 0; i < enemies.Length; ++i)
        {
            SaveSystem.SaveEnemy(enemies[i]);
        }
    }

    public void LoadEnemy()
    {
        for (int i = 0; i < enemies.Length; ++i)
        {
            EnemyData data = SaveSystem.LoadEnemy(enemies[i].name);

            Vector3 pos = new Vector3(data.position[0], data.position[1], data.position[2]);
            enemies[i].pos = pos;
            enemies[i].state = data.state;
            enemies[i].type = data.type;

            enemyGOs[i].GetComponent<Rigidbody>().SetBodyPosition(pos);
            switch (enemies[i].type)
            {
                case EnemyType.BASIC:
                    enemyGOs[i].GetComponent<BasicEnemy>().state = data.state;
                    break;
                case EnemyType.TANK:
                    enemyGOs[i].GetComponent<TankEnemy>().state = data.state;
                    break;
                case EnemyType.UNDISTRACTABLE:
                    enemyGOs[i].GetComponent<UndistractableEnemy>().state = data.state;
                    break;
                case EnemyType.AIR:
                    enemyGOs[i].GetComponent<AirEnemy>().state = data.state;
                    break;
            }

            if (enemies[i].state == EnemyState.DEATH)
            {
                enemyGOs[i].GetComponent<Animation>().PlayAnimation("Dying");
                deadEnemies.Add(enemyGOs[i]);
            }
        }
    }
    public void SaveTest(String name, Vector3 pos)
    {
        // Change deadEnemies[] to a list, if not deadCount is not really
        float time = GameObject.Find("LevelManager").GetComponent<Level_1>().timer.timer;
        SceneManager.SaveTest(deadEnemies.Count, name, pos, time);
    }
}