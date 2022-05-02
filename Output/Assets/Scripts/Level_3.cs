using System;
using RagnarEngine;

public class Level_3 : RagnarComponent
{
	public Characters[] characters;
	public Enemies[] enemies;

	public void Start()
	{
        // Camera Starting Position
        GameObject.Find("cameraController").transform.localPosition = new Vector3(2.42f, 0f, 30.47f);

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
            pos = new Vector3(0.31f, 0.5f, 25.15f)
        };
        characters[0].abilities[0] = new Abilities
        {
            name = "BackStab",
            prefabPath = "BackStab_2",
            transformY = 0.2f,
            intensity = 4.0f,
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
            intensity = 4.0f,
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
            intensity = 4.0f,
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
            intensity = 4.0f,
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
            pos = new Vector3(-1.87f, 0.5f, 27.44f)
        };
        characters[1].abilities[0] = new Abilities
        {
            name = "Backstab",
            prefabPath = "BackStab",
            transformY = 0.2f,
            intensity = 4.0f,
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
            intensity = 4.0f,
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
            intensity = 4.0f,
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
            intensity = 4.0f,
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
            pos = new Vector3(2.58f, 0.5f, 27.44f)
        };
        characters[2].abilities[0] = new Abilities
        {
            name = "Sword Slash",
            prefabPath = "SwordSlash",
            transformY = 0.2f,
            intensity = 4.0f,
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
            intensity = 4.0f,
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
            intensity = 4.0f,
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
            intensity = 4.0f,
            constant = 1.232f,
            linear = -0.201f,
            quadratic = 0f,
            charges = -1,
            cooldown = 6f
        }; // Whistle

        // ENEMIES
        enemies = new Enemies[18];

        enemies[0] = new Enemies
        {
            name = "Tank Enemy 1",
            prefabPath = "Tank Enemy",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
            pos = new Vector3(1.97f, 4.38f, 12.26f),
            waypoints = new GameObject[1]
        };
        enemies[0].waypoints[0] = GameObject.Find("1");

        enemies[1] = new Enemies
        {
            name = "Basic Enemy 1",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-1.94f, 5.29f, 9.29f),
            waypoints = new GameObject[1]
        };
        enemies[1].waypoints[0] = GameObject.Find("2");

        enemies[2] = new Enemies
        {
            name = "Basic Enemy 2",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-6.45f, 5.29f, 6.87f),
            waypoints = new GameObject[1]
        };
        enemies[2].waypoints[0] = GameObject.Find("3");

        enemies[3] = new Enemies
        {
            name = "Basic Enemy 3",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-1.48f, 5.29f, 6.95f),
            waypoints = new GameObject[1]
        };
        enemies[3].waypoints[0] = GameObject.Find("4");

        enemies[4] = new Enemies
        {
            name = "Basic Enemy 4",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(1.22f, 5.29f, 6.95f),
            waypoints = new GameObject[1]
        };
        enemies[4].waypoints[0] = GameObject.Find("5");

        enemies[5] = new Enemies
        {
            name = "Basic Enemy 5",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-2.09f, 5.29f, 2.67f),
            waypoints = new GameObject[1]
        };
        enemies[5].waypoints[0] = GameObject.Find("6");

        enemies[6] = new Enemies
        {
            name = "Basic Enemy 6",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-0.13f, 5.29f, 2.28f),
            waypoints = new GameObject[1]
        };
        enemies[6].waypoints[0] = GameObject.Find("7");

        enemies[7] = new Enemies
        {
            name = "Basic Enemy 7",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(2.6f, 5.29f, 2.96f),
            waypoints = new GameObject[1]
        };
        enemies[7].waypoints[0] = GameObject.Find("8");

        enemies[8] = new Enemies
        {
            name = "Basic Enemy 8",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(12.17f, 5.29f, 4.68f),
            waypoints = new GameObject[1]
        };
        enemies[8].waypoints[0] = GameObject.Find("9");

        enemies[9] = new Enemies
        {
            name = "Tank Enemy 2",
            prefabPath = "Tank Enemy",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
            pos = new Vector3(12.87f, 5.29f, 3.35f),
            waypoints = new GameObject[1]
        };
        enemies[9].waypoints[0] = GameObject.Find("10");

        //enemies[10] = new Enemies
        //{
        //    name = "Undistractable Enemy 1",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(12.31f, 5.29f, -7.62f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[10].waypoints[0] = GameObject.Find("11");

        enemies[10] = new Enemies
        {
            name = "Basic Enemy 9",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(5.67f, 5.29f, -3.86f),
            waypoints = new GameObject[1]
        };
        enemies[10].waypoints[0] = GameObject.Find("12");

        enemies[11] = new Enemies
        {
            name = "Basic Enemy 10",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(5.64f, 5.29f, -11.4f),
            waypoints = new GameObject[1]
        };
        enemies[11].waypoints[0] = GameObject.Find("13");

        enemies[12] = new Enemies
        {
            name = "Basic Enemy 11",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = new Vector3(-1.37f, 5.29f, -16.74f),
            waypoints = new GameObject[1]
        };
        enemies[12].waypoints[0] = GameObject.Find("14");

        //enemies[13] = new Enemies
        //{
        //    name = "Undistractable Enemy 2",
        //    prefabPath = "Undistractable Enemy",
        //    type = EnemyType.UNDISTRACTABLE,
        //    state = EnemyState.IDLE,
        //    pos = new Vector3(-3.29f, 5.29f, -21.28f),
        //    waypoints = new GameObject[1]
        //};
        //enemies[13].waypoints[0] = GameObject.Find("15");

        enemies[13] = new Enemies
        {
            name = "Tank Enemy 3",
            prefabPath = "Tank Enemy",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
            pos = GameObject.Find("16").transform.globalPosition,
            waypoints = new GameObject[2]
        };
        enemies[13].waypoints[0] = GameObject.Find("16");
        enemies[13].waypoints[1] = GameObject.Find("17");

        enemies[14] = new Enemies
        {
            name = "Basic Enemy 12",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("18").transform.globalPosition,
            waypoints = new GameObject[3]
        };
        enemies[14].waypoints[0] = GameObject.Find("18");
        enemies[14].waypoints[1] = GameObject.Find("19");
        enemies[14].waypoints[2] = GameObject.Find("20");

        enemies[15] = new Enemies
        {
            name = "Tank Enemy 4",
            prefabPath = "Tank Enemy",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
            pos = GameObject.Find("21").transform.globalPosition,
            waypoints = new GameObject[2]
        };
        enemies[15].waypoints[0] = GameObject.Find("21");
        enemies[15].waypoints[1] = GameObject.Find("22");

        enemies[16] = new Enemies
        {
            name = "Basic Enemy 13",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("23").transform.globalPosition,
            waypoints = new GameObject[5]
        };
        enemies[16].waypoints[0] = GameObject.Find("23");
        enemies[16].waypoints[1] = GameObject.Find("24");
        enemies[16].waypoints[2] = GameObject.Find("25");
        enemies[16].waypoints[3] = GameObject.Find("26");
        enemies[16].waypoints[4] = GameObject.Find("27");
        
        enemies[17] = new Enemies
        {
            name = "Basic Enemy 14",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
            pos = GameObject.Find("27").transform.globalPosition,
            waypoints = new GameObject[5]
        };
        enemies[17].waypoints[0] = GameObject.Find("23");
        enemies[17].waypoints[1] = GameObject.Find("24");
        enemies[17].waypoints[2] = GameObject.Find("25");
        enemies[17].waypoints[3] = GameObject.Find("26");
        enemies[17].waypoints[4] = GameObject.Find("27");
        ///////////////////////////////////////////////////////////////////

        InternalCalls.InstancePrefab("PlayerManager");
        GameObject.Find("PlayerManager").GetComponent<PlayerManager>().characters = characters;
        InternalCalls.InstancePrefab("EnemyManager");
        GameObject.Find("EnemyManager").GetComponent<EnemyManager>().enemies = enemies;
    }
    public void Update()
	{

	}

}