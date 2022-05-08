using System;
using System.Collections.Generic;
using RagnarEngine;

public class EnemyManager : RagnarComponent
{
    public Enemies[] enemies;
    public List<GameObject> enemyGOs = new List<GameObject>();
    public List<GameObject> deadEnemies = new List<GameObject>();
    public GameObject[] colliders;

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
            enemies[i].state = EnemyState.IDLE;
            enemyGOs[i].SubmitOutlineDrawing(new Vector3(1, 0, 0));
            switch (enemies[i].type)
            {
                case EnemyType.BASIC:
                    enemyGOs[i].GetComponent<BasicEnemy>().waypoints = enemies[i].waypoints;
                    enemyGOs[i].GetComponent<BasicEnemy>().state = enemies[i].state;
                    enemyGOs[i].GetComponent<BasicEnemy>().enemyType = enemies[i].type;
                    enemyGOs[i].GetComponent<BasicEnemy>().colliders = colliders;
                    break;
                case EnemyType.TANK:
                    enemyGOs[i].GetComponent<TankEnemy>().waypoints = enemies[i].waypoints;
                    enemyGOs[i].GetComponent<TankEnemy>().state = enemies[i].state;
                    enemyGOs[i].GetComponent<TankEnemy>().enemyType = enemies[i].type;
                    enemyGOs[i].GetComponent<TankEnemy>().colliders = colliders;
                    break;
                case EnemyType.UNDISTRACTABLE:
                    enemyGOs[i].GetComponent<UndistractableEnemy>().waypoints = enemies[i].waypoints;
                    enemyGOs[i].GetComponent<UndistractableEnemy>().state = enemies[i].state;
                    enemyGOs[i].GetComponent<UndistractableEnemy>().enemyType = enemies[i].type;
                    enemyGOs[i].GetComponent<UndistractableEnemy>().colliders = colliders;
                    break;
                case EnemyType.AIR:
                    enemyGOs[i].GetComponent<AirEnemy>().waypoints = enemies[i].waypoints;
                    enemyGOs[i].GetComponent<AirEnemy>().state = enemies[i].state;
                    enemyGOs[i].GetComponent<AirEnemy>().enemyType = enemies[i].type;
                    enemyGOs[i].GetComponent<AirEnemy>().colliders = colliders;
                    break;
            }
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
            for (int i = 0; i < enemyGOs.Count; i++)
            {
                if ((enemyGOs[i].GetComponent<BasicEnemy>().pendingToDelete && enemyGOs[i].GetComponent<BasicEnemy>().ToString() == "BasicEnemy") || (enemyGOs[i].GetComponent<AirEnemy>().pendingToDelete && enemyGOs[i].GetComponent<AirEnemy>().ToString() == "AirEnemy") || (enemyGOs[i].GetComponent<TankEnemy>().pendingToDelete && enemyGOs[i].GetComponent<TankEnemy>().ToString() == "TankEnemy") || (enemyGOs[i].GetComponent<UndistractableEnemy>().pendingToDelete && enemyGOs[i].GetComponent<UndistractableEnemy>().ToString() == "UndistractableEnemy"))
                {
                    deadEnemies.Add(enemyGOs[i]);
                    enemyGOs[i].DeleteComponent<Rigidbody>(enemyGOs[i].GetComponent<Rigidbody>());

                    switch (enemyGOs[i].GetComponent<BasicEnemy>().enemyType)
                    {
                        case EnemyType.BASIC:
                            enemyGOs[i].ChangeMesh("enemy1_modeldeath");
                            break;
                            //TODO: Check if drone destroyed
                        case EnemyType.AIR:
                            enemyGOs[i].ChangeMesh("enemy4_modeldeath");
                            break;
                        case EnemyType.TANK:
                            enemyGOs[i].ChangeMesh("enemy2_modeldeath");
                            break;
                        case EnemyType.UNDISTRACTABLE:
                            enemyGOs[i].ChangeMesh("enemy3_modeldeath");
                            break;
                    };

                    ChangeEnemyState(enemyGOs[i], EnemyState.DEATH);
                    enemies[i].state = EnemyState.DEATH;
                    enemyGOs[i].isInteractuable = true;
                    enemyGOs[i].interactuableColor = new Vector3(0, 0, 1);
                    enemyGOs[i].GetComponent<BasicEnemy>().pendingToDelete = false;
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