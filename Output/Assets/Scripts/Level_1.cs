using System;
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
        GameObject.Find("cameraController").transform.localPosition = new Vector3(-52.79f, 0f, 89.05f);
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
            pos = new Vector3(-55.52f, 0f, 89.29f)
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
            pos = new Vector3(-50.72f, 0f, 92f)
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
        enemies = new Enemies[25];

        enemies[0] = new Enemies
        {
            name = "Basic Enemy 1",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-47.95f, 0f, 70.62f),
            waypoints = new GameObject[1]
        };
        enemies[0].waypoints[0] = GameObject.Find("1");

        enemies[1] = new Enemies
        {
            name = "Basic Enemy 2",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-41.9f, 0f, 76.72f),
            waypoints = new GameObject[1]
        };
        enemies[1].waypoints[0] = GameObject.Find("2");

        enemies[2] = new Enemies
        {
            name = "Basic Enemy 3",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-36.09f, 0f, 78.52f),
            waypoints = new GameObject[1]
        };
        enemies[2].waypoints[0] = GameObject.Find("3");

        enemies[3] = new Enemies
        {
            name = "Basic Enemy 4",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-32.31f, 0f, 83.42f),
            waypoints = new GameObject[1]
        };
        enemies[3].waypoints[0] = GameObject.Find("4");

        enemies[4] = new Enemies
        {
            name = "Basic Enemy 5",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-14.31f, 0f, 71.04f),
            waypoints = new GameObject[1]
        };
        enemies[4].waypoints[0] = GameObject.Find("5");

        enemies[5] = new Enemies
        {
            name = "Basic Enemy 6",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-14.84f, 0f, 66.57f),
            waypoints = new GameObject[1]
        };
        enemies[5].waypoints[0] = GameObject.Find("6");

        enemies[6] = new Enemies
        {
            name = "Basic Enemy 7",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-23.29f, 0f, 49.22f),
            waypoints = new GameObject[1]
        };
        enemies[6].waypoints[0] = GameObject.Find("7");

        enemies[7] = new Enemies
        {
            name = "Basic Enemy 8",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-0.96f, 0f, 55.66f),
            waypoints = new GameObject[1]
        };
        enemies[7].waypoints[0] = GameObject.Find("8");

        enemies[8] = new Enemies
        {
            name = "Basic Enemy 9",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-39.77f, 0f, 57.01f),
            waypoints = new GameObject[1]
        };
        enemies[8].waypoints[0] = GameObject.Find("9");

        enemies[9] = new Enemies
        {
            name = "Basic Enemy 10",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-7.03f, 0f, 17.20f),
            waypoints = new GameObject[1]
        };
        enemies[9].waypoints[0] = GameObject.Find("10");

        enemies[10] = new Enemies
        {
            name = "Basic Enemy 11",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(11.87f, 0f, 28.13f),
            waypoints = new GameObject[1]
        };
        enemies[10].waypoints[0] = GameObject.Find("11");

        //enemies[11] = new Enemies
        //{
        //    name = "Undistractable Enemy 1",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(-38.13f, 0f, -2.73f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[11].waypoints[0] = GameObject.Find("12");

        enemies[11] = new Enemies
        {
            name = "Basic Enemy 12",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(11.40f, 0f, 11.64f),
            waypoints = new GameObject[1]
        };
        enemies[11].waypoints[0] = GameObject.Find("13");

        enemies[12] = new Enemies
        {
            name = "Basic Enemy 13",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-3.54f, 0f, -5.34f),
            waypoints = new GameObject[1]
        };
        enemies[12].waypoints[0] = GameObject.Find("14");

        //enemies[14] = new Enemies
        //{
        //    name = "Undistractable Enemy 2",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(11.91f, 0f, -6.25f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[14].waypoints[0] = GameObject.Find("15");

        enemies[13] = new Enemies
        {
            name = "Basic Enemy 14",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(8.02f, 0f, -18.9f),
            waypoints = new GameObject[1]
        };
        enemies[13].waypoints[0] = GameObject.Find("16");

        enemies[14] = new Enemies
        {
            name = "Basic Enemy 15",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-15.74f, 0f, -36.37f),
            waypoints = new GameObject[1]
        };
        enemies[14].waypoints[0] = GameObject.Find("17");

        enemies[15] = new Enemies
        {
            name = "Basic Enemy 16",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(1.08f, 0f, -37.19f),
            waypoints = new GameObject[1]
        };
        enemies[15].waypoints[0] = GameObject.Find("18");

        enemies[16] = new Enemies
        {
            name = "Basic Enemy 17",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(40.21f, 0f, 0.84f),
            waypoints = new GameObject[1]
        };
        enemies[16].waypoints[0] = GameObject.Find("19");

        enemies[17] = new Enemies
        {
            name = "Basic Enemy 18",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(42.43f, 0f, -11.37f),
            waypoints = new GameObject[1]
        };
        enemies[17].waypoints[0] = GameObject.Find("20");

        enemies[18] = new Enemies
        {
            name = "Basic Enemy 19",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(44.22f, 0f, -35.61f),
            waypoints = new GameObject[1]
        };
        enemies[18].waypoints[0] = GameObject.Find("21");

        enemies[19] = new Enemies
        {
            name = "Basic Enemy 20",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("22").transform.globalPosition,
            waypoints = new GameObject[1]
        };
        enemies[19].waypoints[0] = GameObject.Find("22");

        enemies[20] = new Enemies
        {
            name = "Basic Enemy 21",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("23").transform.globalPosition,
            waypoints = new GameObject[1]
        };
        enemies[20].waypoints[0] = GameObject.Find("23");

        enemies[21] = new Enemies
        {
            name = "Basic Enemy 22",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("24").transform.globalPosition,
            waypoints = new GameObject[1]
        };
        enemies[21].waypoints[0] = GameObject.Find("24");

        enemies[22] = new Enemies
        {
            name = "Basic Enemy 23",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("25").transform.globalPosition,
            waypoints = new GameObject[2]
        };
        enemies[22].waypoints[0] = GameObject.Find("25");
        enemies[22].waypoints[1] = GameObject.Find("26");

        enemies[23] = new Enemies
        {
            name = "Basic Enemy 24",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("27").transform.globalPosition,
            waypoints = new GameObject[4]
        };
        enemies[23].waypoints[0] = GameObject.Find("27");
        enemies[23].waypoints[1] = GameObject.Find("28");
        enemies[23].waypoints[2] = GameObject.Find("29");
        enemies[23].waypoints[3] = GameObject.Find("30");

        enemies[24] = new Enemies
        {
            name = "Basic Enemy 25",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("31").transform.globalPosition,
            waypoints = new GameObject[2]
        };
        enemies[24].waypoints[0] = GameObject.Find("31");
        enemies[24].waypoints[1] = GameObject.Find("32");
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