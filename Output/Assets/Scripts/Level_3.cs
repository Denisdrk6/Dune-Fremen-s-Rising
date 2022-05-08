using System;
using RagnarEngine;

public class Level_3 : RagnarComponent
{
	public Characters[] characters;
	public Enemies[] enemies;
    private Chronometer timer = null;
    public bool runGame = true;
    public UIButton chrono;
    public Vector3 hitPoint;

    private GameObject SceneAudio;
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
        GameObject.Find("cameraController").transform.localPosition = new Vector3(2.42f, 0f, 30.47f);
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
            pos = new Vector3(0.31f, 0.5f, 25.15f)
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
            pos = new Vector3(-1.87f, 0.5f, 27.44f)
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
            pos = new Vector3(2.58f, 0.5f, 27.44f)
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
        enemies = new Enemies[18];

        enemies[0] = new Enemies
        {
            name = "Tank Enemy 1",
            prefabPath = "Tank Enemy",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
        };

        enemies[1] = new Enemies
        {
            name = "Basic Enemy 1",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[2] = new Enemies
        {
            name = "Basic Enemy 2",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[3] = new Enemies
        {
            name = "Basic Enemy 3",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[4] = new Enemies
        {
            name = "Basic Enemy 4",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[5] = new Enemies
        {
            name = "Basic Enemy 5",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[6] = new Enemies
        {
            name = "Basic Enemy 6",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[7] = new Enemies
        {
            name = "Basic Enemy 7",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[8] = new Enemies
        {
            name = "Basic Enemy 8",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[9] = new Enemies
        {
            name = "Tank Enemy 2",
            prefabPath = "Tank Enemy",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
        };

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
        };

        enemies[11] = new Enemies
        {
            name = "Basic Enemy 10",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

        enemies[12] = new Enemies
        {
            name = "Basic Enemy 11",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };

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
        };
        enemies[13].waypoints.Add(GameObject.Find("16"));
        enemies[13].waypoints.Add(GameObject.Find("16"));

        enemies[14] = new Enemies
        {
            name = "Basic Enemy 12",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };
        enemies[14].waypoints.Add(GameObject.Find("16")); 
        enemies[14].waypoints.Add(GameObject.Find("16"));
        enemies[14].waypoints.Add(GameObject.Find("16"));

        enemies[15] = new Enemies
        {
            name = "Tank Enemy 4",
            prefabPath = "Tank Enemy",
            type = EnemyType.TANK,
            state = EnemyState.IDLE,
        };
        enemies[15].waypoints.Add(GameObject.Find("16")); 
        enemies[15].waypoints.Add(GameObject.Find("16"));

        enemies[16] = new Enemies
        {
            name = "Basic Enemy 13",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };
        enemies[16].waypoints.Add(GameObject.Find("16")); 
        enemies[16].waypoints.Add(GameObject.Find("16")); 
        enemies[16].waypoints.Add(GameObject.Find("16")); 
        enemies[16].waypoints.Add(GameObject.Find("16")); 
        enemies[16].waypoints.Add(GameObject.Find("16"));

        enemies[17] = new Enemies
        {
            name = "Basic Enemy 14",
            prefabPath = "Basic Enemy",
            type = EnemyType.BASIC,
            state = EnemyState.IDLE,
        };
        enemies[17].waypoints.Add(GameObject.Find("16")); 
        enemies[17].waypoints.Add(GameObject.Find("16")); 
        enemies[17].waypoints.Add(GameObject.Find("16")); 
        enemies[17].waypoints.Add(GameObject.Find("16")); 
        enemies[17].waypoints.Add(GameObject.Find("16"));
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