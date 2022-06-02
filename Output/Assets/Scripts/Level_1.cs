using System;
using System.Collections.Generic;
using RagnarEngine;

public class Level_1 : RagnarComponent
{
    public Characters[] characters;
    public Enemies[] enemies;
    public Chronometer timer = null;
    public bool runGame = true;
    public UIButton chrono;
    public Vector3 hitPoint;

    private GameObject SceneAudio;
    private Transform camera;
    private Transform cameraController;
    private pauseMenuButton pause;

    public void Start()
	{
        Input.SetEagleCursor(false);
        Input.SetCursorState(0);
        // Camera Starting Position
        //GameObject.Find("cameraController").transform.globalPosition = new Vector3(-52.79f, 0f, 89.05f);
        GameObject.Find("UI Counter").GetComponent<Transform2D>().position2D = new Vector3(0, (0.5f * InternalCalls.GetRegionGame().y) -28, 0);
        chrono = GameObject.Find("UI Counter").GetComponent<UIButton>();
        chrono.SetTextPosition(-26, -4);
        timer = new Chronometer();

        //Play Level Soundtrack
        SceneAudio = GameObject.Find("AudioLevel1");
        SceneAudio.GetComponent<AudioSource>().PlayClip("MUSICPLAY");
        SceneAudio.GetComponent<AudioSource>().SetState("MUSIC", "LEVEL1_BASE");
        camera = GameObject.Find("Camera").transform;
        cameraController = GameObject.Find("cameraController").transform;
        pause = GameObject.Find("Background").GetComponent<pauseMenuButton>();

        // PLAYERS
        characters = new Characters[1];
        // Player 1
        characters[0] = new Characters
        {
            name = "Paul Atreides",
            prefabPath = "Player",
            state = State.NONE,
            abilities = new Abilities[4],
            hitPoints = 4,
            pos = new Vector3(-43.69f, 0f, 199.77f)
        };
        characters[0].abilities[0] = new Abilities
        {
            name = "BackStab",
            prefabPath = "BackStab_2",
            transformY = 0.2f,
            intensity = 1.250f,
            constant = 0.1f,
            linear = -0.574f,
            quadratic = 0f,
            charges = -1,
            cooldown = 0f
        }; // BackStab
        characters[0].abilities[1] = new Abilities
        {
            name = "The Voice",
            prefabPath = "Voice",
            transformY = 1.32f,
            intensity = 1.250f,
            constant = 1.232f,
            linear = -0.172f,
            quadratic = 0f,
            charges = -1,
            cooldown = 20f
        }; // The Voice
        characters[0].abilities[2] = new Abilities
        {
            name = "Knife Throw",
            prefabPath = "Knife",
            transformY = 1.32f,
            intensity = 1.250f,
            constant = 1.232f,
            linear = -0.172f,
            quadratic = 0f,
            charges = -1,
            cooldown = 25f
        }; // Throwing Knife
        characters[0].abilities[3] = new Abilities
        {
            name = "Rock Throw",
            prefabPath = "Rock",
            transformY = 1.15f,
            intensity = 1.250f,
            constant = 2.5f,
            linear = -0.172f,
            quadratic = 0f,
            charges = -1,
            cooldown = 3f
        }; // Rock/Eagle

        // ENEMIES
        enemies = new Enemies[26];
        enemies[0] = new Enemies
        {
            name = "Basic Enemy 1",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_0")
        };

        enemies[1] = new Enemies
        {
            name = "Basic Enemy 2",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_1")
        };
        enemies[1].waypoints.Add(GameObject.Find("1"));
        enemies[1].waypoints.Add(GameObject.Find("2"));
        enemies[1].waypoints.Add(GameObject.Find("3"));

        enemies[2] = new Enemies
        {
            name = "Basic Enemy 3",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_2")
        };

        enemies[3] = new Enemies
        {
            name = "Basic Enemy 23",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_3")
        };

        enemies[4] = new Enemies
        {
            name = "Basic Enemy 4",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_4")
        };
        enemies[4].waypoints.Add(GameObject.Find("4"));
        enemies[4].waypoints.Add(GameObject.Find("5"));

        enemies[5] = new Enemies
        {
            name = "Basic Enemy 5",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_5")
        };

        enemies[6] = new Enemies
        {
            name = "Basic Enemy 6",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_6")
        };

        enemies[7] = new Enemies
        {
            name = "Basic Oficial Enemy 2",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_7")
        };

        enemies[8] = new Enemies
        {
            name = "Basic Enemy 7",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_8")
        };

        enemies[9] = new Enemies
        {
            name = "Basic Enemy 8",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_9")
        };

        enemies[10] = new Enemies
        {
            name = "Basic Enemy 9",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_10")
        };

        enemies[11] = new Enemies
        {
            name = "Basic Oficial Enemy 3",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_11")
        };

        enemies[12] = new Enemies
        {
            name = "Basic Enemy 10",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_12")
        };

        enemies[13] = new Enemies
        {
            name = "Undistractable Enemy 1",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_13")
        };

        enemies[14] = new Enemies
        {
            name = "Undistractable Enemy 2",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_14")
        };

        enemies[15] = new Enemies
        {
            name = "Basic Enemy 11",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_15")
        };
        enemies[15].waypoints.Add(GameObject.Find("6"));
        enemies[15].waypoints.Add(GameObject.Find("7"));
        enemies[15].waypoints.Add(GameObject.Find("8"));
        enemies[15].waypoints.Add(GameObject.Find("9"));

        enemies[16] = new Enemies
        {
            name = "Basic Enemy 12",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_static_16")
        };

        enemies[17] = new Enemies
        {
            name = "Basic Enemy 13",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_static_17")
        };

        enemies[18] = new Enemies
        {
            name = "Basic Enemy 14",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_18")
        };

        enemies[19] = new Enemies
        {
            name = "Basic Enemy 15",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_19")
        };

        enemies[20] = new Enemies
        {
            name = "Basic Enemy 16",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_20")
        };

        enemies[21] = new Enemies
        {
            name = "Basic Enemy 17",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_21")
        };

        enemies[22] = new Enemies
        {
            name = "Basic Enemy 18",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_22")
        };
        enemies[22].waypoints.Add(GameObject.Find("10"));
        enemies[22].waypoints.Add(GameObject.Find("11"));
        enemies[22].waypoints.Add(GameObject.Find("12"));

        enemies[23] = new Enemies
        {
            name = "Basic Enemy 19",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_23"),
            coneRotate = false
        };
        enemies[24] = new Enemies
        {
            name = "Basic Enemy 20",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_24"),
            coneRotate = false
        };
        enemies[25] = new Enemies
        {
            name = "Basic Enemy 21",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_25"),
            coneRotate = false
        };
        /////////////////////////////////////////////////

        GameObject pm = InternalCalls.InstancePrefab("PlayerManager", Vector3.zero);
        pm.GetComponent<PlayerManager>().characters = characters;
        GameObject em = InternalCalls.InstancePrefab("EnemyManager", Vector3.zero);
        em.GetComponent<EnemyManager>().enemies = enemies;

        InternalCalls.InstancePrefab("Dialogue", Vector3.zero);
        InternalCalls.InstancePrefab("DialogueLevel1", Vector3.zero);

        if (SaveSystem.fromContinue)
        {
            LevelData data = SaveSystem.LoadLevel();
            timer.timer = data.timer;
            cameraController.globalPosition = new Vector3(data.posCam[0], data.posCam[1], data.posCam[2]);
            cameraController.globalRotation = new Quaternion(data.rotCam[0], data.rotCam[1], data.rotCam[2], data.rotCam[3]);
            GameObject.Find("Camera").GetComponent<Camera>().horizontalAngle = data.angle;

            PlayerManager pmm = pm.GetComponent<PlayerManager>();
            pmm.canDoAbility1 = data.abilities[0];
            if (pmm.canDoAbility1)
                InternalCalls.Destroy(GameObject.Find("Trigger1"));
            pmm.canDoAbility2 = data.abilities[1];
            if (pmm.canDoAbility2)
                InternalCalls.Destroy(GameObject.Find("Trigger2"));
            pmm.canDoAbility3 = data.abilities[2];
            if (pmm.canDoAbility3)
                InternalCalls.Destroy(GameObject.Find("Trigger3"));
        }
        else
        {
            SaveSystem.SaveScene();
            bool[] ret = { false, false, false };
            SaveSystem.SaveLevel(timer.timer, cameraController.globalPosition, cameraController.globalRotation, GameObject.Find("Camera").GetComponent<Camera>().horizontalAngle, ret);
        }
    }

	public void Update()
	{
        if (runGame) timer.Update();
        chrono.text = timer.GetTimeToString();

        if (Input.GetKey(KeyCode.L) == KeyState.KEY_DOWN)
        {
            SceneManager.LoadScene("Cinematic_1");
        }
        hitPoint = RayCast.ReturnHitpoint();

        //if(!pause.isSowing || !pause.isOptions)
        //{
        //    GameObject hittedGO = RayCast.HitToTag(camera.globalPosition, hitPoint, "Ground");

        //    if (hittedGO != null)
        //        Input.SetCursorState((int)CursorState.CLICKABLE);
        //    else
        //        Input.SetCursorState((int)CursorState.NON_CLICKABLE);
        //}        
    }
}