using System;
using RagnarEngine;

public class Level_2 : RagnarComponent
{
	public Characters[] characters;
	public Enemies[] enemies;
    public Chronometer timer = null;
    private GameObject SceneAudio;
    public bool runGame = true;
    public UIButton chrono;
    public Vector3 hitPoint;
    private Transform camera;
    private Transform cameraController;
    public void Start()
	{
        Input.SetEagleCursor(true);
        Input.SetCursorState(0);
        //Play Level Soundtrack
        SceneAudio = GameObject.Find("AudioLevel1");
        SceneAudio.GetComponent<AudioSource>().PlayClip("MUSICPLAY");
        SceneAudio.GetComponent<AudioSource>().SetState("MUSIC", "LEVEL1_BASE");
        
        // Camera Starting Position
        //GameObject.Find("cameraController").transform.globalPosition = new Vector3(-13.87f, 0f, -376.10f);
        GameObject.Find("UI Counter").GetComponent<Transform2D>().position2D = new Vector3(0, (0.5f * InternalCalls.GetRegionGame().y) - 28, 0);
        chrono = GameObject.Find("UI Counter").GetComponent<UIButton>();
        chrono.SetTextPosition(-26, -4);
        timer = new Chronometer();
        camera = GameObject.Find("Camera").transform;
        cameraController = GameObject.Find("cameraController").transform;

        if (SaveSystem.fromContinue)
        {
            LevelData data = SaveSystem.LoadLevel();
            timer.timer = data.timer;
            cameraController.globalPosition = new Vector3(data.posCam[0], data.posCam[1], data.posCam[2]);
            cameraController.globalRotation = new Quaternion(data.rotCam[0], data.rotCam[1], data.rotCam[2], data.rotCam[3]);
            GameObject.Find("Camera").GetComponent<Camera>().horizontalAngle = data.angle;
        }
        else
        {
            SaveSystem.SaveScene();
            bool[] ret = { true, true, true };
            SaveSystem.SaveLevel(timer.timer, cameraController.globalPosition, cameraController.globalRotation, GameObject.Find("Camera").GetComponent<Camera>().horizontalAngle, ret);
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
            hitPoints = 4,
            pos = new Vector3(-90.12f, 1f, 95.10f)
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
            charges = 1,
            cooldown = 0f
        }; // Throwing Knife
        characters[0].abilities[3] = new Abilities
        {
            name = "Eagle Throw",
            prefabPath = "Eagle",
            transformY = 1.15f,
            intensity = 1.250f,
            constant = 1.350f,
            linear = -0.172f,
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
            hitPoints = 3,
            pos = new Vector3(-92.83f, 1f, 89.41f)
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
        enemies = new Enemies[58];

        enemies[0] = new Enemies
        {
            name = "Basic Enemy 1",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_1")
        };

        enemies[1] = new Enemies
        {
            name = "Basic Enemy 2",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_2")
        };

        enemies[2] = new Enemies
        {
            name = "Basic Enemy 3",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_3")
        };

        enemies[3] = new Enemies
        {
            name = "Basic Enemy 4",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_4")
        };

        enemies[4] = new Enemies
        {
            name = "Basic Enemy 5",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_5")
        };

        enemies[5] = new Enemies
        {
            name = "Basic Enemy 6",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_partrol_6")
        };
        enemies[5].waypoints.Add(GameObject.Find("1"));
        enemies[5].waypoints.Add(GameObject.Find("2"));
        enemies[5].waypoints.Add(GameObject.Find("3"));
        enemies[5].waypoints.Add(GameObject.Find("4"));

        enemies[6] = new Enemies
        {
            name = "Basic Enemy 7",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_7")
        };

        enemies[7] = new Enemies
        {
            name = "Basic Enemy 8",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_8")
        };

        enemies[8] = new Enemies
        {
            name = "Basic Enemy 9",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_9")
        };

        enemies[9] = new Enemies
        {
            name = "Basic Enemy 10",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_10")
        };
        enemies[9].waypoints.Add(GameObject.Find("5"));
        enemies[9].waypoints.Add(GameObject.Find("6"));
        enemies[9].waypoints.Add(GameObject.Find("7"));
        enemies[9].waypoints.Add(GameObject.Find("8"));

        enemies[10] = new Enemies
        {
            name = "Basic Enemy 11",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_11")
        };

        enemies[11] = new Enemies
        {
            name = "Basic Enemy 12",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_12")
        };

        enemies[12] = new Enemies
        {
            name = "Basic Enemy 13",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_13")
        };

        enemies[13] = new Enemies
        {
            name = "Basic Enemy 14",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_14")
        };

        //enemies[14] = new Enemies
        //{
        //    name = "Basic Enemy 15",
        //    type = EnemyType.BASIC,
        //    state = EnemyState.IDLE,
        //    spawnPoint = GameObject.Find("basic_static_14")
        //};

        enemies[14] = new Enemies
        {
            name = "Undistractable Enemy 15",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_static_15")
        };

        enemies[15] = new Enemies
        {
            name = "Basic Enemy 16",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_16")
        };

        enemies[16] = new Enemies
        {
            name = "Basic Enemy 17",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_17")
        };

        enemies[17] = new Enemies
        {
            name = "Basic Enemy 18",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_18")
        };

        enemies[18] = new Enemies
        {
            name = "Dron Enemy 19",
            type = EnemyType.AIR,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("dron_patrol_19")
        };
        enemies[18].waypoints.Add(GameObject.Find("9"));
        enemies[18].waypoints.Add(GameObject.Find("10"));
        enemies[18].waypoints.Add(GameObject.Find("11"));
        enemies[18].waypoints.Add(GameObject.Find("12"));
        enemies[18].waypoints.Add(GameObject.Find("13"));

        enemies[19] = new Enemies
        {
            name = "Basic Enemy 20",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_20")
        };

        enemies[20] = new Enemies
        {
            name = "Basic Enemy 21",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_21")
        };

        enemies[21] = new Enemies
        {
            name = "Basic Enemy 22",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_22")
        };

        enemies[22] = new Enemies
        {
            name = "Basic Enemy 23",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_23")
        };

        enemies[23] = new Enemies
        {
            name = "Basic Enemy 24",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_24")
        };

        enemies[24] = new Enemies
        {
            name = "Basic Enemy 25",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_25")
        };

        enemies[25] = new Enemies
        {
            name = "Undistractable Enemy 26",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_patrol_26")
        };
        enemies[25].waypoints.Add(GameObject.Find("29"));
        enemies[25].waypoints.Add(GameObject.Find("30"));

        enemies[26] = new Enemies
        {
            name = "Undistractable Enemy 27",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_patrol_27")
        };
        enemies[26].waypoints.Add(GameObject.Find("27"));
        enemies[26].waypoints.Add(GameObject.Find("28"));

        enemies[27] = new Enemies
        {
            name = "Undistractable Enemy 28",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_patrol_28")
        };
        enemies[27].waypoints.Add(GameObject.Find("25"));
        enemies[27].waypoints.Add(GameObject.Find("26"));

        enemies[28] = new Enemies
        {
            name = "Undistractable Enemy 29",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_patrol_29")
        };
        enemies[28].waypoints.Add(GameObject.Find("23"));
        enemies[28].waypoints.Add(GameObject.Find("24"));

        enemies[29] = new Enemies
        {
            name = "Dron Enemy 30",
            type = EnemyType.AIR,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("dron_patrol_30")
        };
        enemies[29].waypoints.Add(GameObject.Find("31"));
        enemies[29].waypoints.Add(GameObject.Find("32"));

        enemies[30] = new Enemies
        {
            name = "Basic Enemy 31",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_31")
        };

        enemies[31] = new Enemies
        {
            name = "Basic Enemy 32",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_32")
        };

        enemies[32] = new Enemies
        {
            name = "Basic Enemy 33",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_33")
        };
        enemies[32].waypoints.Add(GameObject.Find("33"));
        enemies[32].waypoints.Add(GameObject.Find("34"));

        enemies[33] = new Enemies
        {
            name = "Basic Enemy 34",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_34")
        };
        enemies[33].waypoints.Add(GameObject.Find("35"));
        enemies[33].waypoints.Add(GameObject.Find("36"));

        enemies[34] = new Enemies
        {
            name = "Basic Enemy 35",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_35")
        };

        enemies[35] = new Enemies
        {
            name = "Basic Enemy 36",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_36")
        };

        enemies[36] = new Enemies
        {
            name = "Basic Enemy 37",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_37")
        };

        enemies[37] = new Enemies
        {
            name = "Basic Enemy 38",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_38")
        };

        enemies[38] = new Enemies
        {
            name = "Basic Enemy 39",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_39")
        };


        enemies[39] = new Enemies
        {
            name = "Basic Enemy 40",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_40")
        };

        enemies[40] = new Enemies
        {
            name = "Basic Enemy 41",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_41")
        };

        enemies[41] = new Enemies
        {
            name = "Basic Enemy 42",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_42")
        };

        enemies[42] = new Enemies
        {
            name = "Undistractable Enemy 43",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_patrol_43")
        };
        enemies[42].waypoints.Add(GameObject.Find("37"));
        enemies[42].waypoints.Add(GameObject.Find("38"));

        enemies[43] = new Enemies
        {
            name = "Undistractable Enemy 44",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_patrol_44")
        };
        enemies[43].waypoints.Add(GameObject.Find("39"));
        enemies[43].waypoints.Add(GameObject.Find("40"));

        enemies[44] = new Enemies
        {
            name = "Undistractable Enemy 45",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_patrol_45")
        };
        enemies[44].waypoints.Add(GameObject.Find("41"));
        enemies[44].waypoints.Add(GameObject.Find("42"));

        enemies[45] = new Enemies
        {
            name = "Undistractable Enemy 46",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_patrol_46")
        };
        enemies[45].waypoints.Add(GameObject.Find("43"));
        enemies[45].waypoints.Add(GameObject.Find("44"));

        enemies[46] = new Enemies
        {
            name = "Basic Enemy 47",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_47")
        };

        enemies[47] = new Enemies
        {
            name = "Basic Enemy 48",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_48")
        };

        enemies[48] = new Enemies
        {
            name = "Dron Enemy 49",
            type = EnemyType.AIR,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("dron_patrol_49")
        };
        enemies[48].waypoints.Add(GameObject.Find("45"));
        enemies[48].waypoints.Add(GameObject.Find("46"));

        enemies[49] = new Enemies
        {
            name = "Basic Enemy 50",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_50")
        };
        enemies[49].waypoints.Add(GameObject.Find("47"));
        enemies[49].waypoints.Add(GameObject.Find("48"));
        enemies[49].waypoints.Add(GameObject.Find("49"));
        enemies[49].waypoints.Add(GameObject.Find("50"));
        enemies[49].waypoints.Add(GameObject.Find("51"));

        enemies[50] = new Enemies
        {
            name = "Basic Enemy 51",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_51")
        };
        enemies[50].waypoints.Add(GameObject.Find("47"));
        enemies[50].waypoints.Add(GameObject.Find("48"));
        enemies[50].waypoints.Add(GameObject.Find("49"));
        enemies[50].waypoints.Add(GameObject.Find("50"));
        enemies[50].waypoints.Add(GameObject.Find("51"));

        enemies[51] = new Enemies
        {
            name = "Basic Enemy 52",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_52")
        };
        enemies[51].waypoints.Add(GameObject.Find("47"));
        enemies[51].waypoints.Add(GameObject.Find("48"));
        enemies[51].waypoints.Add(GameObject.Find("49"));
        enemies[51].waypoints.Add(GameObject.Find("50"));
        enemies[51].waypoints.Add(GameObject.Find("51"));

        enemies[52] = new Enemies
        {
            name = "Basic Enemy 53",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_patrol_53")
        };
        enemies[52].waypoints.Add(GameObject.Find("52"));
        enemies[52].waypoints.Add(GameObject.Find("53"));

        enemies[53] = new Enemies
        {
            name = "Basic Enemy 54",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_54")
        };

        enemies[54] = new Enemies
        {
            name = "Basic Enemy 55",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_55")
        };

        enemies[55] = new Enemies
        {
            name = "Basic Enemy 56",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_56")
        };

        enemies[56] = new Enemies
        {
            name = "Basic Enemy 57",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_57")
        };

        enemies[57] = new Enemies
        {
            name = "Basic Enemy 58",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_static_58")
        };
        ///////////////////////////////////////////////////////////////////

        GameObject pm = InternalCalls.InstancePrefab("PlayerManager", Vector3.zero);
        pm.GetComponent<PlayerManager>().characters = characters;
        GameObject em = InternalCalls.InstancePrefab("EnemyManager", Vector3.zero);
        em.GetComponent<EnemyManager>().enemies = enemies;

        InternalCalls.InstancePrefab("Dialogue", Vector3.zero);
        InternalCalls.InstancePrefab("DialoguesLevel2", Vector3.zero);
    }
    public void Update()
	{
        if (runGame) timer.Update();
        chrono.text = timer.GetTimeToString();
        hitPoint = RayCast.ReturnHitpoint();

        //GameObject hittedGO = RayCast.HitToTag(camera.globalPosition, hitPoint, "Ground");
    }

}