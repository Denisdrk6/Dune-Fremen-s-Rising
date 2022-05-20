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
    private GameObject preClick;
    private GameObject preNonClick;
    private Transform camera;
    public void Start()
	{
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
        preClick = GameObject.Find("preClick");
        preNonClick = GameObject.Find("preNonClick");
        camera = GameObject.Find("Camera").transform;

        if (SaveSystem.fromContinue)
        {
            TimerData data = SaveSystem.LoadTimer();
            timer.timer = data.timer;
        }

        // PLAYERS
        characters = new Characters[2];
        // Player 1
        characters[0] = new Characters
        {
            name = "Paul Atreides",
            prefabPath = "Player",
            state = State.NONE,
            abilities = new Abilities[4],
            hitPoints = 3,
            pos = new Vector3(-58.45f, 0f, 106.37f)
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
            constant = 1.129f,
            linear = -0.188f,
            quadratic = 0f,
            charges = -1,
            cooldown = 20f
        }; // Rock/Eagle

        // Player 2
        characters[1] = new Characters
        {
            name = "Chani",
            prefabPath = "Player_2",
            state = State.NONE,
            abilities = new Abilities[4],
            hitPoints = 2,
            pos = new Vector3(-57.10f, 0f, 106.17f)
        };
        characters[1].abilities[0] = new Abilities
        {
            name = "Backstab",
            prefabPath = "BackStab",
            transformY = 0.2f,
            intensity = 1.250f,
            constant = 0.1f,
            linear = -0.574f,
            quadratic = 0f,
            charges = -1,
            cooldown = 0f
        }; // BackStab
        characters[1].abilities[1] = new Abilities
        {
            name = "Camouflage",
            prefabPath = "Camouflage",
            transformY = 0.2f,
            intensity = 1.250f,
            constant = 0.1f,
            linear = -0.574f,
            quadratic = 0f,
            charges = -1,
            cooldown = 30f
        }; // Camouflage
        characters[1].abilities[2] = new Abilities
        {
            name = "Hunter Seeker",
            prefabPath = "HunterSeeker",
            transformY = 1.32f,
            intensity = 1.250f,
            constant = 1.232f,
            linear = -0.172f,
            quadratic = 0f,
            charges = 3,
            cooldown = 40f
        }; // Hunter Seeker
        characters[1].abilities[3] = new Abilities
        {
            name = "Spice Grenade",
            prefabPath = "SpiceGrenade",
            transformY = 1.32f,
            intensity = 1.250f,
            constant = 1.232f,
            linear = -0.172f,
            quadratic = 0f,
            charges = -1,
            cooldown = 30f
        }; // Spice Bomb

        // ENEMIES
        enemies = new Enemies[23];
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
        /////////////////////////////////////////////////

        InternalCalls.InstancePrefab("PlayerManager");
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters = characters;
        InternalCalls.InstancePrefab("EnemyManager");
        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemies = enemies;

        InternalCalls.InstancePrefab("Dialogue");
        InternalCalls.InstancePrefab("DialogueLevel1");
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
        hitPoint.y -= 0.5f;
        GameObject hittedGO = RayCast.HitToTag(camera.globalPosition, hitPoint, "Ground");
        if (hittedGO != null)
        {
            preClick.isActive = true;
            preNonClick.isActive = false;
        }
        else
        {
            preClick.isActive = false;
            preNonClick.isActive = true;
        }

        hitPoint.y += 0.54f;
        if (preClick.isActive) preClick.transform.globalPosition = hitPoint;
        if (preNonClick.isActive) preNonClick.transform.globalPosition = hitPoint;
    }
}