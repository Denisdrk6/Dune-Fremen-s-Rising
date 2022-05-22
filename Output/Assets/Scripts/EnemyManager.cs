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

        colliders = GameObject.FindGameObjectsWithTag("Collider");

        for(int i = 0; i < enemies.Length; i++)
        {
            switch (enemies[i].type)
            {
                case EnemyType.BASIC:
                    enemyGOs.Add(InternalCalls.InstancePrefab("Basic Enemy"));
                    enemyGOs[i].name = enemies[i].name;
                    BasicEnemy basicEnemyScript = enemyGOs[i].GetComponent<BasicEnemy>();
                    basicEnemyScript.waypoints = enemies[i].waypoints;
                    basicEnemyScript.state = enemies[i].state;
                    basicEnemyScript.enemyType = enemies[i].type;
                    basicEnemyScript.colliders = colliders;
                    break;
                case EnemyType.TANK:
                    enemyGOs.Add(InternalCalls.InstancePrefab("Tank Enemy"));
                    enemyGOs[i].name = enemies[i].name;
                    TankEnemy tankEnemyScript = enemyGOs[i].GetComponent<TankEnemy>();
                    tankEnemyScript.waypoints = enemies[i].waypoints;
                    tankEnemyScript.state = enemies[i].state;
                    tankEnemyScript.enemyType = enemies[i].type;
                    tankEnemyScript.colliders = colliders;
                    break;
                case EnemyType.UNDISTRACTABLE:
                    enemyGOs.Add(InternalCalls.InstancePrefab("Undistractable Enemy"));
                    enemyGOs[i].name = enemies[i].name;
                    UndistractableEnemy undistractableEnemyScript = enemyGOs[i].GetComponent<UndistractableEnemy>();
                    undistractableEnemyScript.waypoints = enemies[i].waypoints;
                    undistractableEnemyScript.state = enemies[i].state;
                    undistractableEnemyScript.enemyType = enemies[i].type;
                    undistractableEnemyScript.colliders = colliders;
                    break;
                case EnemyType.AIR:
                    enemyGOs.Add(InternalCalls.InstancePrefab("Air Enemy"));
                    enemyGOs[i].name = enemies[i].name;
                    AirEnemy airEnemyScript = enemyGOs[i].GetComponent<AirEnemy>();
                    airEnemyScript.waypoints = enemies[i].waypoints;
                    airEnemyScript.state = enemies[i].state;
                    airEnemyScript.enemyType = enemies[i].type;
                    airEnemyScript.colliders = colliders;
                    break;
            }
            enemyGOs[i].SubmitOutlineDrawing(new Vector3(1, 0, 0));
            SetEnemyPositionAndRotation(enemyGOs[i], enemies[i]);
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
                    enemyGOs[i].transform.globalRotation = enemyGOs[i].GetComponent<Rigidbody>().GetBodyRotation();
                    enemyGOs[i].DeleteComponent<Rigidbody>(enemyGOs[i].GetComponent<Rigidbody>());

                    switch (enemyGOs[i].GetComponent<BasicEnemy>().enemyType)
                    {
                        case EnemyType.BASIC:
                            enemyGOs[i].ChangeMesh("1_modeldeath");
                            break;
                            //TODO: Check if drone destroyed
                        case EnemyType.AIR:
                            enemyGOs[i].ChangeMesh("4_modeldeath");
                            break;
                        case EnemyType.TANK:
                            enemyGOs[i].ChangeMesh("3_modeldeath");
                            break;
                        case EnemyType.UNDISTRACTABLE:
                            enemyGOs[i].ChangeMesh("2_modeldeath");
                            break;
                    };

                    GameObject sound = InternalCalls.InstancePrefab("SoundArea", true);
                    sound.GetComponent<Rigidbody>().SetRadiusSphere(10f);
                    sound.transform.globalPosition = enemyGOs[i].transform.globalPosition;

                    ChangeEnemyState(enemyGOs[i], EnemyState.DEATH);
                    enemies[i].state = EnemyState.DEATH;
                    enemyGOs[i].isInteractuable = true;
                    enemyGOs[i].interactuableColor = new Vector3(0, 0, 1);
                    enemyGOs[i].GetComponent<BasicEnemy>().pendingToDelete = false;
                }
            }
        }
    }

    private void SetEnemyPositionAndRotation(GameObject e, Enemies data)
    {
        Rigidbody rb = e.GetComponent<Rigidbody>();
        rb.SetBodyPosition(data.spawnPoint.transform.globalPosition);

        Quaternion rotation = GetFinalRotation(data);

        rb.SetBodyRotation(rotation);
    }

    private static Quaternion GetFinalRotation(Enemies data)
    {
        Vector3 newForward = data.spawnPoint.transform.forward;
        double angle = Math.Atan2(newForward.x, newForward.z);
        Quaternion rotation = new Quaternion(0, (float)(1 * Math.Sin(angle / 2)), 0, (float)Math.Cos(angle / 2));
        return rotation;
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

            enemies[i].state = data.state;
            enemies[i].type = data.type;

            SetEnemyPositionAndRotation(enemyGOs[i], enemies[i]);
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
                        enemyGOs[i].ChangeMesh("enemy3_modeldeath");
                        break;
                    case EnemyType.UNDISTRACTABLE:
                        enemyGOs[i].ChangeMesh("enemy2_modeldeath");
                        break;
                };
                enemyGOs[i].isInteractuable = true;
                enemyGOs[i].interactuableColor = new Vector3(0, 0, 1);
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