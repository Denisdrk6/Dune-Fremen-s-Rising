using System;
using RagnarEngine;

public class Level_2 : RagnarComponent
{
	public Characters[] characters;
	public Enemies[] enemies;
    private Chronometer timer = null;
    private GameObject SceneAudio;
    public bool runGame = true;
    public UIButton chrono;
    public Vector3 hitPoint;

    private GameObject preClick;
    private GameObject preNonClick;
    private Transform camera;
    public void Start()
	{
        //Play Level Soundtrack
        SceneAudio = GameObject.Find("AudioLevel1");
        SceneAudio.GetComponent<AudioSource>().PlayClip("MUSICPLAY");
        SceneAudio.GetComponent<AudioSource>().SetState("MUSIC", "LEVEL1_BASE");
        
        // Camera Starting Position
        GameObject.Find("cameraController").transform.localPosition = new Vector3(-23.76f, 0f, -199.01f);
        GameObject.Find("UI Counter").GetComponent<Transform2D>().position2D = new Vector3(0, (0.5f * InternalCalls.GetRegionGame().y) - 28, 0);
        chrono = GameObject.Find("UI Counter").GetComponent<UIButton>();
        chrono.SetTextPosition(-26, -4);
        timer = new Chronometer();

        preClick = GameObject.Find("preClick");
        preNonClick = GameObject.Find("preNonClick");
        camera = GameObject.Find("Camera").transform;

        // PLAYERS
        characters = new Characters[3];
        // Player 1
        characters[0] = new Characters
        {
            name = "Paul Atreides",
            prefabPath = "Player",
            state = State.NONE,
            abilities = new Abilities[4],
            hitPoints = 3,
            pos = new Vector3(21.70f, 0f, -160.8f)
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
            name = "Eagle",
            prefabPath = "Eagle",
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
            pos = new Vector3(29.58f, 0f, -164.22f)
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

        // Player 3
        characters[2] = new Characters
        {
            name = "Stilgar",
            prefabPath = "Player_3",
            state = State.NONE,
            abilities = new Abilities[4],
            hitPoints = 4,
            pos = new Vector3(19.31f, 0f, -167.67f)
        };
        characters[2].abilities[0] = new Abilities
        {
            name = "Sword Slash",
            prefabPath = "SwordSlash",
            transformY = 0.2f,
            intensity = 1.250f,
            constant = 0.1f,
            linear = -0.574f,
            quadratic = 0f,
            charges = -1,
            cooldown = 0f
        }; // Sword Slash
        characters[2].abilities[1] = new Abilities
        {
            name = "Stunner",
            prefabPath = "StunnerShot",
            transformY = 1.32f,
            intensity = 1.250f,
            constant = 1.232f,
            linear = -0.172f,
            quadratic = 0f,
            charges = 4,
            cooldown = 5f
        }; // Stunner Shot
        characters[2].abilities[2] = new Abilities
        {
            name = "Trap",
            prefabPath = "Trap",
            transformY = 0.12f,
            intensity = 1.250f,
            constant = 0.100f,
            linear = -0.942f,
            quadratic = 0f,
            charges = 1,
            cooldown = 0f
        }; // Trap
        characters[2].abilities[3] = new Abilities
        {
            name = "Whistle",
            prefabPath = "Whistle",
            transformY = 1.12f,
            intensity = 1.250f,
            constant = 1.232f,
            linear = -0.201f,
            quadratic = 0f,
            charges = -1,
            cooldown = 6f
        }; // Whistle

        // ENEMIES
        enemies = new Enemies[42];

        enemies[0] = new Enemies
        {
            name = "Basic Enemy 1",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(25.81f, 0f, -149.29f),
            waypoints = new GameObject[1]
        };
        enemies[0].waypoints[0] = GameObject.Find("1");

        enemies[1] = new Enemies
        {
            name = "Basic Enemy 2",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(12.4f, 0f, -150.71f),
            waypoints = new GameObject[1]
        };
        enemies[1].waypoints[0] = GameObject.Find("2");

        enemies[2] = new Enemies
        {
            name = "Basic Enemy 3",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(6.13f, 0f, -135.06f),
            waypoints = new GameObject[1]
        };
        enemies[2].waypoints[0] = GameObject.Find("3");

        enemies[3] = new Enemies
        {
            name = "Basic Enemy 4",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-16.31f, 0f, -138.17f),
            waypoints = new GameObject[1]
        };
        enemies[3].waypoints[0] = GameObject.Find("4");

        enemies[4] = new Enemies
        {
            name = "Basic Enemy 5",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-30.45f, 0f, -163.85f),
            waypoints = new GameObject[1]
        };
        enemies[4].waypoints[0] = GameObject.Find("5");

        enemies[5] = new Enemies
        {
            name = "Basic Enemy 6",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-32.4f, 0f, -166.61f),
            waypoints = new GameObject[1]
        };
        enemies[5].waypoints[0] = GameObject.Find("6");

        enemies[6] = new Enemies
        {
            name = "Basic Enemy 7",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-19.34f, 0f, -110.44f),
            waypoints = new GameObject[1]
        };
        enemies[6].waypoints[0] = GameObject.Find("7");

        enemies[7] = new Enemies
        {
            name = "Basic Enemy 8",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-41.22f, 0f, -120.72f),
            waypoints = new GameObject[1]
        };
        enemies[7].waypoints[0] = GameObject.Find("8");

        enemies[8] = new Enemies
        {
            name = "Basic Enemy 9",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-21.95f, 0f, -126.75f),
            waypoints = new GameObject[1]
        };
        enemies[8].waypoints[0] = GameObject.Find("9");

        enemies[9] = new Enemies
        {
            name = "Basic Enemy 10",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-41.73f, 0f, -92.8f),
            waypoints = new GameObject[1]
        };
        enemies[9].waypoints[0] = GameObject.Find("10");

        enemies[10] = new Enemies
        {
            name = "Basic Enemy 11",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-45.46f, 0f, -92.28f),
            waypoints = new GameObject[1]
        };
        enemies[10].waypoints[0] = GameObject.Find("11");

        enemies[11] = new Enemies
        {
            name = "Basic Enemy 12",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-46.17f, 0f, -95.98f),
            waypoints = new GameObject[1]
        };
        enemies[11].waypoints[0] = GameObject.Find("12");

        //enemies[12] = new Enemies
        //{
        //    name = "Undistractable Enemy 1",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(-69.01f, 0f, -92.53f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[12].waypoints[0] = GameObject.Find("13");

        enemies[12] = new Enemies
        {
            name = "Basic Enemy 13",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-72.34f, 0f, -84.86f),
            waypoints = new GameObject[1]
        };
        enemies[12].waypoints[0] = GameObject.Find("14");

        enemies[13] = new Enemies
        {
            name = "Basic Enemy 14",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-58.97f, 0f, -72.96f),
            waypoints = new GameObject[1]
        };
        enemies[13].waypoints[0] = GameObject.Find("15");

        enemies[14] = new Enemies
        {
            name = "Basic Enemy 15",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-53.75f, 0f, -82.53f),
            waypoints = new GameObject[1]
        };
        enemies[14].waypoints[0] = GameObject.Find("16");

        enemies[15] = new Enemies
        {
            name = "Basic Enemy 16",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-46.91f, 0f, -76.17f),
            waypoints = new GameObject[1]
        };
        enemies[15].waypoints[0] = GameObject.Find("17");

        enemies[16] = new Enemies
        {
            name = "Basic Enemy 17",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-88.25f, 0f, -80.21f),
            waypoints = new GameObject[1]
        };
        enemies[16].waypoints[0] = GameObject.Find("18");

        enemies[17] = new Enemies
        {
            name = "Basic Enemy 18",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-104.68f, 0f, -75.70f),
            waypoints = new GameObject[1]
        };
        enemies[17].waypoints[0] = GameObject.Find("19");

        enemies[18] = new Enemies
        {
            name = "Basic Enemy 19",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-106.18f, 0f, -72.36f),
            waypoints = new GameObject[1]
        };
        enemies[18].waypoints[0] = GameObject.Find("20");

        enemies[19] = new Enemies
        {
            name = "Basic Enemy 20",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-91.69f, 0f, -64.14f),
            waypoints = new GameObject[1]
        };
        enemies[19].waypoints[0] = GameObject.Find("21");

        enemies[20] = new Enemies
        {
            name = "Basic Enemy 21",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-47.41f, 0f, -58.41f),
            waypoints = new GameObject[1]
        };
        enemies[20].waypoints[0] = GameObject.Find("22");

        enemies[21] = new Enemies
        {
            name = "Basic Enemy 22",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-54.86f, 0f, -55.51f),
            waypoints = new GameObject[1]
        };
        enemies[21].waypoints[0] = GameObject.Find("23");

        enemies[22] = new Enemies
        {
            name = "Basic Enemy 23",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-54.78f, 0f, -40.8f),
            waypoints = new GameObject[1]
        };
        enemies[22].waypoints[0] = GameObject.Find("24");

        enemies[23] = new Enemies
        {
            name = "Basic Enemy 24",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-43.86f, 0f, -40.12f),
            waypoints = new GameObject[1]
        };
        enemies[23].waypoints[0] = GameObject.Find("25");

        //enemies[25] = new Enemies
        //{
        //    name = "Undistractable Enemy 2",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(-24.83f, 0f, -28.42f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[25].waypoints[0] = GameObject.Find("26");

        //enemies[24] = new Enemies
        //{
        //    name = "Undistractable Enemy 3",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(33.6f, 0f, 27.78f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[24].waypoints[0] = GameObject.Find("27");

        //enemies[25] = new Enemies
        //{
        //    name = "Undistractable Enemy 4",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(29.89f, 0f, 27.59f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[25].waypoints[0] = GameObject.Find("28");

        //enemies[26] = new Enemies
        //{
        //    name = "Undistractable Enemy 5",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(26.07f, 0f, 27.18f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[26].waypoints[0] = GameObject.Find("29");

        enemies[24] = new Enemies
        {
            name = "Basic Enemy 25",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(2.18f, 0f, 53.00f),
            waypoints = new GameObject[1]
        };
        enemies[24].waypoints[0] = GameObject.Find("30");

        enemies[25] = new Enemies
        {
            name = "Basic Enemy 26",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(13.01f, 0f, 76.35f),
            waypoints = new GameObject[1]
        };
        enemies[25].waypoints[0] = GameObject.Find("31");

        enemies[26] = new Enemies
        {
            name = "Basic Enemy 27",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(0.36f, 8.10f, 78.50f),
            waypoints = new GameObject[1]
        };
        enemies[26].waypoints[0] = GameObject.Find("32");

        enemies[27] = new Enemies
        {
            name = "Basic Enemy 28",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-9.66f, 0f, 84.53f),
            waypoints = new GameObject[1]
        };
        enemies[27].waypoints[0] = GameObject.Find("33");

        enemies[28] = new Enemies
        {
            name = "Basic Enemy 29",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-16.33f, 0f, 84.66f),
            waypoints = new GameObject[1]
        };
        enemies[28].waypoints[0] = GameObject.Find("34");

        enemies[29] = new Enemies
        {
            name = "Basic Enemy 30",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(62.67f, 0f, 59.19f),
            waypoints = new GameObject[1]
        };
        enemies[29].waypoints[0] = GameObject.Find("35");

        enemies[30] = new Enemies
        {
            name = "Basic Enemy 31",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(83.51f, 0f, 73.53f),
            waypoints = new GameObject[1]
        };
        enemies[30].waypoints[0] = GameObject.Find("36");

        enemies[31] = new Enemies
        {
            name = "Basic Enemy 32",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(92.41f, 0f, 87.59f),
            waypoints = new GameObject[1]
        };
        enemies[31].waypoints[0] = GameObject.Find("37");

        enemies[32] = new Enemies
        {
            name = "Basic Enemy 33",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(83.42f, 0f, 108.89f),
            waypoints = new GameObject[1]
        };
        enemies[32].waypoints[0] = GameObject.Find("38");

        enemies[33] = new Enemies
        {
            name = "Basic Enemy 34",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(80.2f, 0f, 111.65f),
            waypoints = new GameObject[1]
        };
        enemies[33].waypoints[0] = GameObject.Find("39");

        enemies[34] = new Enemies
        {
            name = "Basic Enemy 35",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(63.37f, 0f, 117.25f),
            waypoints = new GameObject[1]
        };
        enemies[34].waypoints[0] = GameObject.Find("40");

        //enemies[38] = new Enemies
        //{
        //    name = "Undistractable Enemy 6",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(33.97f, 0f, 131.15f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[38].waypoints[0] = GameObject.Find("41");

        //enemies[39] = new Enemies
        //{
        //    name = "Undistractable Enemy 7",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(30.48f, 0f, 132.19f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[39].waypoints[0] = GameObject.Find("42");

        //enemies[40] = new Enemies
        //{
        //    name = "Undistractable Enemy 8",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(26.43f, 0f, 132.45f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[40].waypoints[0] = GameObject.Find("43");

        enemies[35] = new Enemies
        {
            name = "Basic Enemy 36",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(12.95f, 0f, 131.32f),
            waypoints = new GameObject[1]
        };
        enemies[35].waypoints[0] = GameObject.Find("44");

        enemies[36] = new Enemies
        {
            name = "Basic Enemy 37",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(16.85f, 0f, 113.25f),
            waypoints = new GameObject[1]
        };
        enemies[36].waypoints[0] = GameObject.Find("45");

        enemies[37] = new Enemies
        {
            name = "Basic Enemy 38",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(2.91f, 0f, 124.5f),
            waypoints = new GameObject[1]
        };
        enemies[37].waypoints[0] = GameObject.Find("46");

        enemies[38] = new Enemies
        {
            name = "Basic Enemy 39",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-7.75f, 0f, 113.32f),
            waypoints = new GameObject[1]
        };
        enemies[38].waypoints[0] = GameObject.Find("47");

        enemies[39] = new Enemies
        {
            name = "Basic Enemy 40",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-8.21f, 0f, 110.61f),
            waypoints = new GameObject[1]
        };
        enemies[39].waypoints[0] = GameObject.Find("48");

        //enemies[46] = new Enemies
        //{
        //    name = "Undistractable Enemy 9",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = GameObject.Find("49").transform.globalPosition,
        //    waypoints = new GameObject[2]
        //};
        //enemies[46].waypoints[0] = GameObject.Find("49");
        //enemies[46].waypoints[1] = GameObject.Find("50");

        enemies[40] = new Enemies
        {
            name = "Tank Enemy 1",
            prefabPath = "Tank Enemy",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
            pos = new Vector3(-47.95f, 0f, 70.62f),
            waypoints = new GameObject[4]
        };
        enemies[40].waypoints[0] = GameObject.Find("51");
        enemies[40].waypoints[1] = GameObject.Find("52");
        enemies[40].waypoints[2] = GameObject.Find("53");
        enemies[40].waypoints[3] = GameObject.Find("54");

        enemies[41] = new Enemies
        {
            name = "Basic Enemy 41",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-47.95f, 0f, 70.62f),
            waypoints = new GameObject[2]
        };
        enemies[41].waypoints[0] = GameObject.Find("55");
        enemies[41].waypoints[1] = GameObject.Find("56");
        ///////////////////////////////////////////////////////////////////

        InternalCalls.InstancePrefab("PlayerManager");
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters = characters;
        InternalCalls.InstancePrefab("EnemyManager");
        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemies = enemies;
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
        if (preClick.isActive) preClick.GetComponent<Transform>().globalPosition = hitPoint;
        if (preNonClick.isActive) preNonClick.GetComponent<Transform>().globalPosition = hitPoint;
    }

}