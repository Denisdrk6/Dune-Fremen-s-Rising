using System;
using System.Collections.Generic;
using RagnarEngine;

public class PlayerManager : RagnarComponent
{
    private Camera camComponent;

    public GameObject[] players;
    public int characterSelected = 0;

    public Characters[] characters;
    public Characters playableCharacter;

    GameObject[] area = null;
    GameObject lightHab = null;
    DialogueManager dialogue;

    UIText cd1;
    UIText cd2;
    UIText cd3;
    UIText cd4;

    UIImage ability1Bg;
    UIImage ability2Bg;
    UIImage ability3Bg;
    UIImage ability4Bg;

    public bool canDoAbility1 = true;
    public bool canDoAbility2 = true;
    public bool canDoAbility3 = true;
    public bool canDoAbility4 = true;

    public float radius;
    public void Start()
	{
        foreach (Characters c in characters)
        {
            InternalCalls.InstancePrefab(c.prefabPath, c.pos);
        }

        players = GameObject.FindGameObjectsWithTag("Player");
        Vector3[] outlineColors = new Vector3[3] { new Vector3(0,1,0), new Vector3(0.5f,0,0.5f), new Vector3(0,0,1) };
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Rigidbody>().SetBodyPosition(characters[i].pos);
            if(i == 0 && SceneManager.currentSceneName == "build")
                players[i].GetComponent<Rigidbody>().SetBodyRotation(Quaternion.RotateAroundAxis(new Vector3(0,1,0), 180));
            players[i].SubmitOutlineDrawing(outlineColors[i]);
        }

        // IgnoreCollision with other players
        for (int i = 0; i < players.Length; i++)
        {
            for (int j = 0; j < players.Length; j++)
            {
                players[i].GetComponent<Rigidbody>().IgnoreCollision(players[j], true);
            }
        }

        ChangeCharacter(characterSelected);
        playableCharacter = characters[characterSelected];
        for(int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Player>().hitPoints = characters[i].hitPoints;
        }

        area = GameObject.FindGameObjectsWithTag("AbilityRange");
        GameObject[] aux = new GameObject[area.Length];
        for (int i = 0, j = area.Length - 1; i < area.Length; i++, j--)
        {
            aux[j] = area[i];
        }
        area = aux;
        dialogue = GameObject.Find("Dialogue").GetComponent<DialogueManager>();

        cd1 = GameObject.Find("cd1").GetComponent<UIText>();
        cd2 = GameObject.Find("cd2").GetComponent<UIText>();
        cd3 = GameObject.Find("cd3").GetComponent<UIText>();
        cd4 = GameObject.Find("cd4").GetComponent<UIText>();

        lightHab = GameObject.Find("ControllableLight");
        if (SaveSystem.fromContinue)
        {
            LoadPlayer();
        }

        camComponent = GameObject.Find("Camera").GetComponent<Camera>();
        GameObject.Find("Background").GetComponent<pauseMenuButton>().FillPlayers();

        // Abilities UI
        ability1Bg = GameObject.Find("Ability1Bg").GetComponent<UIImage>();
        ability2Bg = GameObject.Find("Ability2Bg").GetComponent<UIImage>();
        ability3Bg = GameObject.Find("Ability3Bg").GetComponent<UIImage>();
        ability4Bg = GameObject.Find("Ability4Bg").GetComponent<UIImage>();

        if (SceneManager.currentSceneName == "build" && !SaveSystem.fromContinue)
        {
            canDoAbility1 = false;
            ability1Bg.SetImageGeneralColor(128, 128, 128);
            canDoAbility2 = false;
            ability2Bg.SetImageGeneralColor(128, 128, 128);
            canDoAbility3 = false;
            ability3Bg.SetImageGeneralColor(128, 128, 128);
        }        
    }

	public void Update()
    {
        if (Input.GetKey(KeyCode.F6) == KeyState.KEY_DOWN)
        {
            SaveSystem.LoadScene();
            //LoadPlayer();
            //GameObject.Find("EnemyManager").GetComponent<EnemyManager>().LoadEnemy();
            SaveSystem.fromContinue = true;
        }
        if (Input.GetKey(KeyCode.F5) == KeyState.KEY_DOWN)
        {
            GameObject.Find("EnemyManager").GetComponent<EnemyManager>().SaveEnemies();
            SavePlayer();
        }
        if (!dialogue.GetInDialogue())
        {
            PlayerCases();

            /*Cambiador de estados para saber que habilidad estas o no casteando (Basicamente hace que el personaje entre en un estado donde si clickas una tecla
            muestre el rango de habilidad, y entre en un estado de castear o cancelar la habilidad seleccionada (Click derecho cancel/click izquierdo casteo)).
            Aqu� deber�a ir la zona de rango de cada habilidad.*/
            if(players[characterSelected].GetComponent<Player>().controled)
                AbilityStateChanger();
            

            /*Contador de cooldown para cada habilidad
            Funciona en todos los casos con todos los pjs.*/
            CooldownCounter();

        }

    }

    private void CooldownCounter()
    {
        for (int j = 0; j < characters.Length; j++)
        {
            for (int i = 0; i < characters[j].abilities.Length; i++)
            {
                if (characters[j].abilities[i].onCooldown == true)
                {
                    characters[j].abilities[i].counter += Time.deltaTime;
                    if (characters[j].abilities[i].counter >= characters[j].abilities[i].cooldown)
                    {
                        characters[j].abilities[i].onCooldown = false;
                        characters[j].abilities[i].counter = 0f;
                    }
                }
            }
        }
    }

    // LETRA Z --> HABILIDAD 1 DE TODOS LOS PJS
    public void Ability1()
    {
        if (players[characterSelected].GetComponent<Player>().controled && playableCharacter.pickedEnemy == null && !players[characterSelected].GetComponent<Player>().dead&&canDoAbility1)
            SpawnArea(State.ABILITY_1);
    }

    // LETRA X --> HABILIDAD 2 DE TODOS LOS PJS
    public void Ability2()
    {
        if (players[characterSelected].GetComponent<Player>().controled && playableCharacter.pickedEnemy == null && !players[characterSelected].GetComponent<Player>().dead&&canDoAbility2)
            SpawnArea(State.ABILITY_2);
    }

    // LETRA C --> HABILIDAD 3 DE TODOS LOS PJS
    public void Ability3()
    {
        if (players[characterSelected].GetComponent<Player>().controled && playableCharacter.pickedEnemy == null && !players[characterSelected].GetComponent<Player>().dead&&canDoAbility3)
            SpawnArea(State.ABILITY_3);
    }

    // LETRA V --> HABILIDAD 4 DE TODOS LOS PJS
    public void Ability4()
    {
        if (players[characterSelected].GetComponent<Player>().controled && playableCharacter.pickedEnemy == null && !players[characterSelected].GetComponent<Player>().dead&&canDoAbility4)
            SpawnArea(State.ABILITY_4);
    }

    // LETRA B --> ARRASTRAR CUERPOS
    public void Carrying()
    {
        if (players[characterSelected].GetComponent<Player>().controled && !players[characterSelected].GetComponent<Player>().dead)
        {
            playableCharacter.state = State.CARRYING;
            players[characterSelected].GetComponent<Player>().SetState(State.CARRYING);
        }
    }

    private void AbilityStateChanger()
    {
        // Change Condition to all players
        if (((playableCharacter == characters[0]) && (playableCharacter.state == State.ABILITY_4)) || (players.Length != 1 && playableCharacter == characters[1] && (playableCharacter.state == State.ABILITY_4)))
        {
            radius = 0f;
            if (playableCharacter == characters[0]) radius = 20.2f;
            else if (playableCharacter == characters[1]) radius = 12.7f;

            lightHab.GetComponent<Light>().intensity = 6;

            Vector3 hit;
            if (SceneManager.currentSceneName == "build")
                hit = GameObject.Find("LevelManager").GetComponent<Level_1>().hitPoint;
            else if (SceneManager.currentSceneName == "build2")
                hit = GameObject.Find("LevelManager").GetComponent<Level_2>().hitPoint;
            else
                hit = GameObject.Find("LevelManager").GetComponent<Level_3>().hitPoint;

            if (Transform.GetDistanceBetween(players[characterSelected].transform.globalPosition, hit) < radius)
            {
                hit.y += 0.2f;
                lightHab.transform.globalPosition = hit;
            }
            else
            {
                // No BORRAR
                //Vector3 a = players[characterSelected].transform.globalPosition;
                //Vector3 b = (hit - a).normalized * 11.5f + a;
                //b.y = lightHab.transform.globalPosition.y;
                //lightHab.transform.globalPosition = b;
            }
        }

        // Si el estado no es NONE, significa que la habilidad est� lista para ser casteada, y entrar� en esta funci�n.
        if (playableCharacter.state != State.NONE) CastOrCancel();
    }

    private void SpawnArea(State ability)
    {
        // Comprobador de cargas de habilidad. Si entra aqu�, significa que la habilidad no tiene cargas
        if (playableCharacter.abilities[(int)ability - 1].charges == 0)
        {
            playableCharacter.state = State.NONE;
        }
        // Entra aqu� si la habilidad tiene cargas o las cargas son -1 (Habilidad infinita (Solo cooldown)). Cambia el estado del player al de la habilidad que haya marcado.
        else if (!playableCharacter.abilities[(int)ability - 1].onCooldown)
        {

            SetCursor(ability);

            playableCharacter.state = (State)ability;
            lightHab.GetComponent<Light>().intensity = 0f;
            // Dibujado del �rea de rango.
            DrawArea((int)ability);

            players[characterSelected].GetComponent<Player>().SetState(ability);
        }
        // Si la habilidad est� en cooldown y tiene cargas, entrar� aqu� y pondr� el state del player en NONE.
        else
        {
            //Debug.Log("Ability on Cooldown! You have" + (playableCharacter.abilities[(int)ability - 1].cooldown - playableCharacter.abilities[(int)ability - 1].counter) + "seconds left to use it again!");
            
            playableCharacter.state = State.NONE;
        }
    }

    private void SetCursor(State ability)
    {
        switch (characterSelected)
        {
            case 0:
                if (ability == State.ABILITY_1)
                {
                    Input.SetCursorState((int)CursorState.PAUL_1);
                }
                else if (ability == State.ABILITY_2)
                {
                    Input.SetCursorState((int)CursorState.PAUL_2);
                }
                else if (ability == State.ABILITY_3)
                {
                    Input.SetCursorState((int)CursorState.PAUL_3);
                }
                else if (ability == State.ABILITY_4)
                {
                    Input.SetCursorState((int)CursorState.PAUL_4);
                }
                break;
            case 1:
                if (ability == State.ABILITY_1)
                {
                    Input.SetCursorState((int)CursorState.CHANI_1);
                }
                else if (ability == State.ABILITY_2)
                {
                    Input.SetCursorState((int)CursorState.CHANI_2);
                }
                else if (ability == State.ABILITY_3)
                {
                    Input.SetCursorState((int)CursorState.CHANI_3);
                }
                else if (ability == State.ABILITY_4)
                {
                    Input.SetCursorState((int)CursorState.CHANI_4);
                }
                break;
            case 2:
                if (ability == State.ABILITY_1)
                {
                    Input.SetCursorState((int)CursorState.STILGAR_1);
                }
                else if (ability == State.ABILITY_2)
                {
                    Input.SetCursorState((int)CursorState.STILGAR_2);
                }
                else if (ability == State.ABILITY_3)
                {
                    Input.SetCursorState((int)CursorState.STILGAR_3);
                }
                else if (ability == State.ABILITY_4)
                {
                    Input.SetCursorState((int)CursorState.STILGAR_4);
                }
                break;
            default:
                break;
        }
    }

    private void DrawArea(int ability)
    {
        area[characterSelected].transform.localPosition = new Vector3(0, playableCharacter.abilities[ability - 1].transformY, 0);
        Light areaLightSelected = area[characterSelected].GetComponent<Light>();
        areaLightSelected.intensity = playableCharacter.abilities[ability - 1].intensity;
        areaLightSelected.constant = playableCharacter.abilities[ability - 1].constant;
        areaLightSelected.linear = playableCharacter.abilities[ability - 1].linear;
        areaLightSelected.quadratic = playableCharacter.abilities[ability - 1].quadratic;
    }

    private void CastOrCancel()
    {

        if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
        {
            Input.SetCursorState((int)CursorState.NORMAL);

            switch (playableCharacter.state)
            {
                case State.CARRYING:
                    {
                        if (playableCharacter.pickedEnemy != null && players[characterSelected].GetComponent<Player>().GetAction() == 2)
                        {
                            GameObject.ReparentToRoot(playableCharacter.pickedEnemy);

                            players[characterSelected].GetComponent<Animation>().PlayAnimation("CorpseDrop");
                            playableCharacter.pickedEnemy.transform.localPosition = players[characterSelected].transform.globalPosition;
                            playableCharacter.pickedEnemy.transform.localRotation = players[characterSelected].transform.globalRotation;

                            //Debug.Log("Dropping the corpse of" + playableCharacter.pickedEnemy.name.ToString());
                            playableCharacter.pickedEnemy = null;
                        }
                        else
                        {
                            NavAgent agent = players[characterSelected].GetComponent<NavAgent>();
                            GameObject obj = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");

                            if (obj != null && obj.GetComponent<BasicEnemy>().state == EnemyState.DEATH && Transform.GetDistanceBetween(obj.transform.globalPosition, players[characterSelected].transform.globalPosition) < 3)
                            {
                                players[characterSelected].AddChild(obj);

                                obj.transform.localPosition = new Vector3(0, 2, 0);
                                obj.transform.localRotation = Quaternion.identity;

                                obj.GetComponent<Animation>().PlayAnimation("CorpsePicked");
                                players[characterSelected].GetComponent<Animation>().PlayAnimation("CorpsePick");

                                playableCharacter.pickedEnemy = obj;
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            if (playableCharacter.state != State.CARRYING)
            {
                // Instancia la habilidad en cuesti�n. 
                InternalCalls.InstancePrefab(playableCharacter.abilities[(int)playableCharacter.state - 1].prefabPath, playableCharacter.pos);

                // Al haberse instanciado una habilidad, comprueba si funciona por cargas. Si lo hace resta una carga a la habilidad.
                if (playableCharacter.abilities[(int)playableCharacter.state - 1].charges != -1 && playableCharacter.abilities[(int)playableCharacter.state - 1].charges != 0)
                {
                    playableCharacter.abilities[(int)playableCharacter.state - 1].charges -= 1;
                }

                // Pone la habilidad en cooldown y el player en estado de NONE
                playableCharacter.abilities[(int)playableCharacter.state - 1].onCooldown = true;
                playableCharacter.state = State.NONE;

                // Se cambia el estado a POSTCAST para evitar que se mueva directamente despu�s de castear la habilidad. En el update de los players se cambiar� a NONE nuevamente para que se pueda mover (Tras un ciclo de update). 
                players[characterSelected].GetComponent<Player>().SetState(State.POSTCAST);

                area[characterSelected].GetComponent<Light>().intensity = 0f;
                lightHab.GetComponent<Light>().intensity = 0f;
            }
            else if (playableCharacter.state == State.CARRYING)
            {
                if (players[characterSelected].GetComponent<Animation>().HasFinished())
                {
                    playableCharacter.state = State.NONE;
                    players[characterSelected].GetComponent<Player>().SetState(State.POSTCAST);

                    if (playableCharacter.pickedEnemy != null)
                    {
                        playableCharacter.pickedEnemy.GetComponent<Animation>().PlayAnimation("CorpseCarry");
                        players[characterSelected].GetComponent<Player>().SetAction(2);
                    }
                    else
                        players[characterSelected].GetComponent<Player>().SetAction(0);
                }
            }
        }

        // Se cancela el estado de la habilidad para que el �rea de rango deje de mostrarse.
        if (Input.GetMouseClick(MouseButton.RIGHT) == KeyState.KEY_DOWN)
        {
            Input.SetCursorState((int)CursorState.NORMAL);
            playableCharacter.state = State.POSTCAST;
            players[characterSelected].GetComponent<Player>().SetState(State.POSTCAST);

            area[characterSelected].GetComponent<Light>().intensity = 0f;
            lightHab.GetComponent<Light>().intensity = 0f;

            
        }
    }

    private void PlayerCases()
    {
        for (int i = 0; i < playableCharacter.abilities.Length; i++)
        {
            CooldownTimer(i);
        }
        switch (players.Length)
        {
            case 4:
                if (Input.GetKey(KeyCode.ALPHA4) == KeyState.KEY_DOWN)
                {
                    if (characterSelected == 3 && camComponent != null)
                    {
                        Vector3 pos = players[characterSelected].GetComponent<Transform>().globalPosition;
                        camComponent.ScriptMovement(pos.x, pos.y, pos.z);
                    }

                    players[characterSelected].GetComponent<Player>().SetState(State.NONE);
                    characterSelected = 3;
                    playableCharacter.state = State.NONE;
                    if (area != null) area[characterSelected].GetComponent<Light>().intensity = 0f;
                    lightHab.GetComponent<Light>().intensity = 0f;
                    playableCharacter = characters[characterSelected];
                    ChangeCharacter(characterSelected);
                }
                goto case 3;
            case 3:
                if (Input.GetKey(KeyCode.ALPHA3) == KeyState.KEY_DOWN)
                {
                    if (characterSelected == 2 && camComponent != null)
                    {
                        Vector3 pos = players[characterSelected].GetComponent<Transform>().globalPosition;
                        camComponent.ScriptMovement(pos.x, pos.y, pos.z);
                    }

                    players[characterSelected].GetComponent<Player>().SetState(State.NONE);
                    characterSelected = 2;
                    playableCharacter.state = State.NONE;
                    if(area != null) area[characterSelected].GetComponent<Light>().intensity = 0f;
                    lightHab.GetComponent<Light>().intensity = 0f;
                    playableCharacter = characters[characterSelected];
                    ChangeCharacter(characterSelected);
                }
                goto case 2;
            case 2:
                if (Input.GetKey(KeyCode.ALPHA2) == KeyState.KEY_DOWN)
                {
                    if (characterSelected == 1 && camComponent != null)
                    {
                        Vector3 pos = players[characterSelected].GetComponent<Transform>().globalPosition;
                        camComponent.ScriptMovement(pos.x, pos.y, pos.z);
                    }

                    players[characterSelected].GetComponent<Player>().SetState(State.NONE);
                    characterSelected = 1;
                    playableCharacter.state = State.NONE;
                    if (area != null) area[characterSelected].GetComponent<Light>().intensity = 0f;
                    lightHab.GetComponent<Light>().intensity = 0f;
                    playableCharacter = characters[characterSelected];
                    ChangeCharacter(characterSelected);
                }
                goto case 1;
            case 1:
                if (Input.GetKey(KeyCode.ALPHA1) == KeyState.KEY_DOWN)
                {
                    if (characterSelected == 0 && camComponent != null)
                    {
                        Vector3 pos = players[characterSelected].GetComponent<Transform>().globalPosition;
                        camComponent.ScriptMovement(pos.x, pos.y, pos.z);
                    }

                    players[characterSelected].GetComponent<Player>().SetState(State.NONE);
                    characterSelected = 0;
                    playableCharacter.state = State.NONE;
                    if (area != null) area[characterSelected].GetComponent<Light>().intensity = 0f;
                    lightHab.GetComponent<Light>().intensity = 0f;
                    playableCharacter = characters[characterSelected];
                    ChangeCharacter(characterSelected);
                }
                break;
        }
    }

    void ChangeCharacter(int id)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (area != null) area[i].GetComponent<Light>().intensity = 0f;
            players[i].GetComponent<Player>().SetControled(false);
        }
        players[id].GetComponent<Player>().SetControled(true);
        Input.SetCursorState(0);
    }

    public void SavePlayer()
    {
        SaveSystem.DeleteDirectoryFiles("Library/SavedGame/Players");
        SaveSystem.SaveScene();
        bool[] ret = { true, true, true};
        Transform cam = GameObject.Find("cameraController").transform;
        switch (SceneManager.currentSceneName)
        {
            case "build":
                bool[] abi = { canDoAbility1, canDoAbility2 ,canDoAbility3};
                SaveSystem.SaveLevel(GameObject.Find("LevelManager").GetComponent<Level_1>().timer.timer, cam.globalPosition, cam.globalRotation, camComponent.horizontalAngle, abi);
                GameObject.Find("Dialogue").GetComponent<DialogueManager>().SaveDialogue();
                break;
            case "build2":
                SaveSystem.SaveLevel(GameObject.Find("LevelManager").GetComponent<Level_2>().timer.timer, cam.globalPosition, cam.globalRotation, camComponent.horizontalAngle, ret);
                GameObject.Find("Dialogue").GetComponent<DialogueManager>().SaveDialogue();
                break;
            case "build3":
                SaveSystem.SaveLevel(GameObject.Find("LevelManager").GetComponent<Level_3>().timer.timer, cam.globalPosition, cam.globalRotation, camComponent.horizontalAngle, ret);
                GameObject.Find("Dialogue").GetComponent<DialogueManager>().SaveDialogue();
                break;
        }

        for (int i = 0; i < players.Length; ++i)
        { 
            SaveSystem.SavePlayer(players[i].GetComponent<Player>());
        }
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer(players[0].name);
        if (data != null)
        {
            for (int i = 0; i < players.Length; ++i)
            {
                data = SaveSystem.LoadPlayer(players[i].name);

                players[i].GetComponent<Player>().hitPoints = data.hitPoints;

                Vector3 pos = new Vector3(data.position[0], data.position[1], data.position[2]);
                players[i].GetComponent<Rigidbody>().SetBodyPosition(pos);

                Quaternion rot = new Quaternion(data.rotation[0], data.rotation[1], data.rotation[2], data.rotation[3]);
                players[i].GetComponent<Rigidbody>().SetBodyRotation(rot);
            }
        }
    }

    public void CooldownTimer(int abilityID)
    {
        float temp;
        temp = playableCharacter.abilities[abilityID].cooldown - playableCharacter.abilities[abilityID].counter;

        temp = (float)Math.Round((double)temp, 0);

        switch(abilityID)
        {
            case 0:
                cd1.text = temp.ToString();

                if (playableCharacter.abilities[abilityID].onCooldown || !canDoAbility1)
                {
                    ability1Bg.SetImageGeneralColor(128, 128, 128);
                }
                if (temp <= 0.0f || (playableCharacter.abilities[abilityID].counter <= 0.0f))
                    cd1.text = "";

                if (!playableCharacter.abilities[abilityID].onCooldown && canDoAbility1)
                {
                    if (playableCharacter.name == "Paul Atreides")
                    {
                        ability1Bg.SetImageGeneralColor(11, 212, 0);
                    }
                    else if (playableCharacter.name == "Chani")
                    {
                        ability1Bg.SetImageGeneralColor(244, 60, 255);
                    }
                    else if (playableCharacter.name == "Stilgar")
                    {
                        ability1Bg.SetImageGeneralColor(0, 40, 255);
                    }
                }
                break;
            case 1:
                cd2.text = temp.ToString();

                if (playableCharacter.abilities[abilityID].onCooldown || !canDoAbility2)
                    ability2Bg.SetImageGeneralColor(128, 128, 128);

                if (temp <= 0.0f || (playableCharacter.abilities[abilityID].counter <= 0.0f))
                    cd2.text = "";

                if (!playableCharacter.abilities[abilityID].onCooldown && canDoAbility2)
                {
                    if (playableCharacter.name == "Paul Atreides")
                    {
                        ability2Bg.SetImageGeneralColor(11, 212, 0);
                    }
                    else if (playableCharacter.name == "Chani")
                    {
                        ability2Bg.SetImageGeneralColor(244, 60, 255);
                    }
                    else if (playableCharacter.name == "Stilgar")
                    {
                        ability2Bg.SetImageGeneralColor(0, 40, 255);
                    }
                }
                break;
            case 2:
                cd3.text = temp.ToString();

                if (playableCharacter.abilities[abilityID].onCooldown || !canDoAbility3)
                    ability3Bg.SetImageGeneralColor(128, 128, 128);

                if (temp <= 0.0f || (playableCharacter.abilities[abilityID].counter <= 0.0f))
                    cd3.text = "";

                if (!playableCharacter.abilities[abilityID].onCooldown && canDoAbility3)
                {
                    if (playableCharacter.name == "Paul Atreides")
                    {
                        ability3Bg.SetImageGeneralColor(11, 212, 0);
                    }
                    else if (playableCharacter.name == "Chani")
                    {
                        ability3Bg.SetImageGeneralColor(244, 60, 255);
                    }
                    else if (playableCharacter.name == "Stilgar")
                    {
                        ability3Bg.SetImageGeneralColor(0, 40, 255);
                    }
                }
                break;
            case 3:
                cd4.text = temp.ToString();

                if (playableCharacter.abilities[abilityID].onCooldown || !canDoAbility4)
                {
                    ability4Bg.SetImageGeneralColor(128, 128, 128);
                }

                if (temp <= 0.0f || (playableCharacter.abilities[abilityID].counter <= 0.0f))
                {
                    cd4.text = "";
                }

                if (!playableCharacter.abilities[abilityID].onCooldown && canDoAbility4)
                {
                    if (playableCharacter.name == "Paul Atreides")
                    {
                        ability4Bg.SetImageGeneralColor(11, 212, 0);
                    }
                    else if (playableCharacter.name == "Chani")
                    {
                        ability4Bg.SetImageGeneralColor(244, 60, 255);
                    }
                   else if (playableCharacter.name == "Stilgar")
                   {
                        ability4Bg.SetImageGeneralColor(0, 40, 255);
                   }
                }
                break;
        }
    }
}
