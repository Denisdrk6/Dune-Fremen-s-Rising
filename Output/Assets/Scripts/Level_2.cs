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
        //GameObject.Find("cameraController").transform.globalPosition = new Vector3(-13.87f, 0f, -376.10f);
        GameObject.Find("UI Counter").GetComponent<Transform2D>().position2D = new Vector3(0, (0.5f * InternalCalls.GetRegionGame().y) - 28, 0);
        chrono = GameObject.Find("UI Counter").GetComponent<UIButton>();
        chrono.SetTextPosition(-26, -4);
        timer = new Chronometer();

        preClick = GameObject.Find("preClick");
        preNonClick = GameObject.Find("preNonClick");
        camera = GameObject.Find("Camera").transform;

        if (SaveSystem.fromContinue)
        {
            TimerData data = SaveSystem.LoadTimer();
            timer.timer = data.timer;
        }

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
            pos = new Vector3(-0.25f, 1f, -345.38f)
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
            pos = new Vector3(3.29f, 0f, -343.98f)
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
            pos = new Vector3(-4.82f, 0f, -347.26f)
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
        enemies = new Enemies[70];

        enemies[0] = new Enemies
        {
            name = "Basic Enemy 1",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_1")
        };

        enemies[1] = new Enemies
        {
            name = "Basic Enemy 2",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_2")
        };

        enemies[2] = new Enemies
        {
            name = "Basic Enemy 3",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_3")
        };

        enemies[3] = new Enemies
        {
            name = "Basic Enemy 4",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_4")
        };

        enemies[4] = new Enemies
        {
            name = "Basic Enemy 5",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_5")
        };

        enemies[5] = new Enemies
        {
            name = "Basic Enemy 6",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_6_patrol")
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
            spawnPoint = GameObject.Find("basic_enemy_7")
        };

        enemies[7] = new Enemies
        {
            name = "Basic Enemy 8",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_8")
        };

        enemies[8] = new Enemies
        {
            name = "Basic Enemy 9",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_9")
        };

        enemies[9] = new Enemies
        {
            name = "Basic Oficial Enemy 1",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_oficial_enemy_1_patrol")
        };
        enemies[9].waypoints.Add(GameObject.Find("5"));
        enemies[9].waypoints.Add(GameObject.Find("6"));
        enemies[9].waypoints.Add(GameObject.Find("7"));
        enemies[9].waypoints.Add(GameObject.Find("8"));

        enemies[10] = new Enemies
        {
            name = "Basic Enemy 10",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_10")
        };

        enemies[11] = new Enemies
        {
            name = "Basic Enemy 11",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_11")
        };

        enemies[12] = new Enemies
        {
            name = "Basic Enemy 12",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_12")
        };

        enemies[13] = new Enemies
        {
            name = "Basic Enemy 13",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_13")
        };

        enemies[14] = new Enemies
        {
            name = "Basic Enemy 14",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_14")
        };

        enemies[15] = new Enemies
        {
            name = "Basic Enemy 15",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_15")
        };

        enemies[16] = new Enemies
        {
            name = "Undistractable Enemy 1",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_enemy_1")
        };

        enemies[17] = new Enemies
        {
            name = "Basic Enemy 16",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_16")
        };

        enemies[18] = new Enemies
        {
            name = "Basic Enemy 17",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_17")
        };

        enemies[19] = new Enemies
        {
            name = "Basic Enemy 18",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_18")
        };

        enemies[20] = new Enemies
        {
            name = "Basic Enemy 19",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_19")
        };

        enemies[21] = new Enemies
        {
            name = "Basic Enemy 20",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_20")
        };

        enemies[22] = new Enemies
        {
            name = "Dron Enemy 1",
            type = EnemyType.AIR,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("dron_enemy_1_patrol")
        };
        enemies[22].waypoints.Add(GameObject.Find("9"));
        enemies[22].waypoints.Add(GameObject.Find("10"));
        enemies[22].waypoints.Add(GameObject.Find("11"));
        enemies[22].waypoints.Add(GameObject.Find("12"));
        enemies[22].waypoints.Add(GameObject.Find("13"));
        enemies[22].waypoints.Add(GameObject.Find("14"));
        enemies[22].waypoints.Add(GameObject.Find("15"));

        enemies[23] = new Enemies
        {
            name = "Basic Enemy 21",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_21")
        };

        enemies[24] = new Enemies
        {
            name = "Basic Enemy 22",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_22")
        };

        enemies[25] = new Enemies
        {
            name = "Basic Enemy 23",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_23")
        };

        enemies[26] = new Enemies
        {
            name = "Basic Enemy 24",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_24")
        };

        enemies[27] = new Enemies
        {
            name = "Tank Enemy 1",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("tank_enemy_1_patrol")
        };
        enemies[27].waypoints.Add(GameObject.Find("16"));
        enemies[27].waypoints.Add(GameObject.Find("17"));
        enemies[27].waypoints.Add(GameObject.Find("18"));
        enemies[27].waypoints.Add(GameObject.Find("19"));

        enemies[28] = new Enemies
        {
            name = "Basic Enemy 25",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_25")
        };

        enemies[29] = new Enemies
        {
            name = "Basic Enemy 26",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_26")
        };

        enemies[30] = new Enemies
        {
            name = "Undistractable Enemy 2",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_enemy_2")
        };

        enemies[31] = new Enemies
        {
            name = "Basic Enemy 27",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_27")
        };

        enemies[32] = new Enemies
        {
            name = "Basic Enemy 28",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_28")
        };

        enemies[33] = new Enemies
        {
            name = "Basic Enemy 29",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_29")
        };

        enemies[34] = new Enemies
        {
            name = "Undistractable Enemy 3",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_enemy_3_patrol")
        };
        enemies[34].waypoints.Add(GameObject.Find("20"));
        enemies[34].waypoints.Add(GameObject.Find("21"));
        enemies[34].waypoints.Add(GameObject.Find("22"));

        enemies[35] = new Enemies
        {
            name = "Undistractable Enemy 4",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_enemy_4_patrol")
        };
        enemies[35].waypoints.Add(GameObject.Find("23"));
        enemies[35].waypoints.Add(GameObject.Find("24"));
        enemies[35].waypoints.Add(GameObject.Find("25"));
        enemies[35].waypoints.Add(GameObject.Find("26"));

        enemies[36] = new Enemies
        {
            name = "Undistractable Enemy 5",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_enemy_5_patrol")
        };
        enemies[36].waypoints.Add(GameObject.Find("23"));
        enemies[36].waypoints.Add(GameObject.Find("24"));
        enemies[36].waypoints.Add(GameObject.Find("25"));
        enemies[36].waypoints.Add(GameObject.Find("26"));

        enemies[37] = new Enemies
        {
            name = "Basic Enemy 30",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_30_patrol")
        };
        enemies[37].waypoints.Add(GameObject.Find("23"));
        enemies[37].waypoints.Add(GameObject.Find("24"));
        enemies[37].waypoints.Add(GameObject.Find("25"));
        enemies[37].waypoints.Add(GameObject.Find("26"));

        enemies[38] = new Enemies
        {
            name = "Basic Enemy 31",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_31_patrol")
        };
        enemies[38].waypoints.Add(GameObject.Find("23"));
        enemies[38].waypoints.Add(GameObject.Find("24"));
        enemies[38].waypoints.Add(GameObject.Find("25"));
        enemies[38].waypoints.Add(GameObject.Find("26"));

        enemies[39] = new Enemies
        {
            name = "Dron Enemy 2",
            type = EnemyType.AIR,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("dron_enemy_2_patrol")
        };
        enemies[39].waypoints.Add(GameObject.Find("27"));
        enemies[39].waypoints.Add(GameObject.Find("28"));

        enemies[40] = new Enemies
        {
            name = "Basic Enemy 32",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_32")
        };

        enemies[41] = new Enemies
        {
            name = "Basic Enemy 33",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_33")
        };

        enemies[42] = new Enemies
        {
            name = "Basic Enemy 34",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_34_patrol")
        };
        enemies[42].waypoints.Add(GameObject.Find("29"));
        enemies[42].waypoints.Add(GameObject.Find("30"));

        enemies[43] = new Enemies
        {
            name = "Basic Enemy 35",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_35")
        };

        enemies[44] = new Enemies
        {
            name = "Basic Enemy 36",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_36")
        };

        enemies[45] = new Enemies
        {
            name = "Basic Enemy 37",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_37")
        };

        enemies[46] = new Enemies
        {
            name = "Basic Enemy 38",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_38")
        };

        enemies[47] = new Enemies
        {
            name = "Basic Enemy 39",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_39_patrol")
        };
        enemies[47].waypoints.Add(GameObject.Find("31"));
        enemies[47].waypoints.Add(GameObject.Find("32"));
        enemies[47].waypoints.Add(GameObject.Find("33"));
        enemies[47].waypoints.Add(GameObject.Find("32"));


        enemies[48] = new Enemies
        {
            name = "Basic Enemy 40",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_40")
        };

        enemies[49] = new Enemies
        {
            name = "Basic Enemy 41",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_41")
        };

        enemies[50] = new Enemies
        {
            name = "Basic Enemy 42",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_42")
        };

        enemies[51] = new Enemies
        {
            name = "Basic Enemy 43",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_43")
        };

        enemies[52] = new Enemies
        {
            name = "Basic Enemy 44",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_44")
        };

        enemies[53] = new Enemies
        {
            name = "Basic Enemy 45",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_45")
        };

        enemies[54] = new Enemies
        {
            name = "Basic Enemy 46",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_46")
        };

        enemies[55] = new Enemies
        {
            name = "Basic Enemy 47",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_47_patrol")
        };
        enemies[55].waypoints.Add(GameObject.Find("34"));
        enemies[55].waypoints.Add(GameObject.Find("35"));
        enemies[55].waypoints.Add(GameObject.Find("36"));
        enemies[55].waypoints.Add(GameObject.Find("37"));
        enemies[55].waypoints.Add(GameObject.Find("38"));
        enemies[55].waypoints.Add(GameObject.Find("39"));

        enemies[56] = new Enemies
        {
            name = "Basic Enemy 48",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_48_patrol")
        };
        enemies[56].waypoints.Add(GameObject.Find("34"));
        enemies[56].waypoints.Add(GameObject.Find("35"));
        enemies[56].waypoints.Add(GameObject.Find("36"));
        enemies[56].waypoints.Add(GameObject.Find("37"));
        enemies[56].waypoints.Add(GameObject.Find("38"));
        enemies[56].waypoints.Add(GameObject.Find("39"));

        enemies[57] = new Enemies
        {
            name = "Basic Enemy 49",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_49_patrol")
        };
        enemies[57].waypoints.Add(GameObject.Find("34"));
        enemies[57].waypoints.Add(GameObject.Find("35"));
        enemies[57].waypoints.Add(GameObject.Find("36"));
        enemies[57].waypoints.Add(GameObject.Find("37"));
        enemies[57].waypoints.Add(GameObject.Find("38"));
        enemies[57].waypoints.Add(GameObject.Find("39"));

        enemies[58] = new Enemies
        {
            name = "Dron Enemy 3",
            type = EnemyType.AIR,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("dron_enemy_3_patrol")
        };
        enemies[58].waypoints.Add(GameObject.Find("40"));
        enemies[58].waypoints.Add(GameObject.Find("41"));

        enemies[59] = new Enemies
        {
            name = "Basic Enemy 50",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_50_patrol")
        };
        enemies[59].waypoints.Add(GameObject.Find("42"));
        enemies[59].waypoints.Add(GameObject.Find("43"));

        enemies[60] = new Enemies
        {
            name = "Basic Enemy 51",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_51")
        };

        enemies[61] = new Enemies
        {
            name = "Basic Enemy 52",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_52")
        };

        enemies[62] = new Enemies
        {
            name = "Basic Enemy 53",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_53")
        };

        enemies[63] = new Enemies
        {
            name = "Basic Enemy 54",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_54")
        };

        enemies[64] = new Enemies
        {
            name = "Basic Enemy 55",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_55")
        };

        enemies[65] = new Enemies
        {
            name = "Basic Enemy 56",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_56")
        };

        enemies[66] = new Enemies
        {
            name = "Undistractable Enemy 6",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_enemy_6_patrol")
        };
        enemies[66].waypoints.Add(GameObject.Find("25"));
        enemies[66].waypoints.Add(GameObject.Find("26"));
        enemies[66].waypoints.Add(GameObject.Find("23"));
        enemies[66].waypoints.Add(GameObject.Find("24"));

        enemies[67] = new Enemies
        {
            name = "Undistractable Enemy 7",
            type = EnemyType.UNDISTRACTABLE,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("undistractable_enemy_7_patrol")
        };
        enemies[67].waypoints.Add(GameObject.Find("25"));
        enemies[67].waypoints.Add(GameObject.Find("26"));
        enemies[67].waypoints.Add(GameObject.Find("23"));
        enemies[67].waypoints.Add(GameObject.Find("24"));

        enemies[68] = new Enemies
        {
            name = "Basic Enemy 57",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_57_patrol")
        };
        enemies[68].waypoints.Add(GameObject.Find("25"));
        enemies[68].waypoints.Add(GameObject.Find("26"));
        enemies[68].waypoints.Add(GameObject.Find("23"));
        enemies[68].waypoints.Add(GameObject.Find("24"));

        enemies[69] = new Enemies
        {
            name = "Basic Enemy 58",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            spawnPoint = GameObject.Find("basic_enemy_58_patrol")
        };
        enemies[69].waypoints.Add(GameObject.Find("25"));
        enemies[69].waypoints.Add(GameObject.Find("26"));
        enemies[69].waypoints.Add(GameObject.Find("23"));
        enemies[69].waypoints.Add(GameObject.Find("24"));
        ///////////////////////////////////////////////////////////////////

        InternalCalls.InstancePrefab("PlayerManager");
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters = characters;
        InternalCalls.InstancePrefab("EnemyManager");
        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemies = enemies;

        InternalCalls.InstancePrefab("Dialogue");
        InternalCalls.InstancePrefab("DialoguesLevel2");
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