using System;
using RagnarEngine;

public class PlayerManager : RagnarComponent
{
    public GameObject[] players;
    public int characterSelected = 0;

    public Characters[] characters;
    public Characters playableCharacter;

    GameObject[] area = null;
    public bool drawnArea = false;
    bool crouched = false;
    DialogueManager dialogue;

    public void Start()
	{
        foreach (Characters c in characters)
        {
            InternalCalls.InstancePrefab(c.prefabPath);
            //c.abilities[4] = new Abilities
            //{
            //    name = "CorpseCarrier",
            //    prefabPath = "",
            //    transformY = 0.0f,
            //    intensity = 4.0f,
            //    constant = 0.0f,
            //    linear = 0.0f,
            //    quadratic = 0f,
            //    charges = -1,
            //    cooldown = 0f
            //}; // Corpse Carrier Ability
        }

        players = GameObject.FindGameObjectsWithTag("Player");
        for(int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Rigidbody>().SetBodyPosition(characters[i].pos);
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
    }

	public void Update()
    {
        if (!dialogue.GetInDialogue())
        {
            if (Input.GetKey(KeyCode.LSHIFT) == KeyState.KEY_DOWN)
            {
                crouched = !crouched;
            }

            PlayerCases();

            /*Cambiador de estados para saber qué habilidad estás o no casteando (Básicamente hace que el personaje entre en un estado donde si clickas una tecla
            muestre el rango de habilidad, y entre en un estado de castear o cancelar la habilidad seleccionada (Click derecho cancel/click izquierdo casteo)).
            Aquí debería ir la zona de rango de cada habilidad.*/
            AbilityStateChanger();

            /*Contador de cooldown para cada habilidad
            Funciona en todos los casos con todos los pjs.*/
            CooldownCounter();
        }

    }

    private void CooldownCounter()
    {
        foreach (Abilities a in playableCharacter.abilities)
        {
            if (a.onCooldown == true)
            {
                a.counter += Time.deltaTime;
                if (a.counter >= a.cooldown)
                {
                    a.onCooldown = false;
                    a.counter = 0f;
                }
            }
        }
    }

    private void AbilityStateChanger()
    {
        // LETRA Z --> HABILIDAD 1 DE TODOS LOS PJS
        if (Input.GetKey(KeyCode.Z) == KeyState.KEY_DOWN)
        {
            SpawnArea((int)State.ABILITY_1);
        }

        // LETRA X --> HABILIDAD 2 DE TODOS LOS PJS
        if (Input.GetKey(KeyCode.X) == KeyState.KEY_DOWN)
        {
            SpawnArea((int)State.ABILITY_2);
        }

        // LETRA C --> HABILIDAD 3 DE TODOS LOS PJS
        if (Input.GetKey(KeyCode.C) == KeyState.KEY_DOWN)
        {
            SpawnArea((int)State.ABILITY_3);
        }

        // LETRA V --> HABILIDAD 4 DE TODOS LOS PJS
        if (Input.GetKey(KeyCode.V) == KeyState.KEY_DOWN)
        {
            SpawnArea((int)State.ABILITY_4);
        }

        // LETRA B --> ARRASTRAR CUERPOS
        if (Input.GetKey(KeyCode.B) == KeyState.KEY_DOWN)
        {
            //SpawnArea((int)State.CARRYING);
            playableCharacter.state = State.CARRYING;
            players[characterSelected].GetComponent<Player>().SetState((int)State.CARRYING);
        }

        // Si el estado no es NONE, significa que la habilidad está lista para ser casteada, y entrará en esta función.
        if (playableCharacter.state != State.NONE) CastOrCancel();
    }

    private void SpawnArea(int ability)
    {
        // Comprobador de cargas de habilidad. Si entra aquí, significa que la habilidad no tiene cargas
        if (playableCharacter.abilities[ability - 1].charges == 0)
        {
            playableCharacter.state = State.NONE;
        }
        // Entra aquí si la habilidad tiene cargas o las cargas son -1 (Habilidad infinita (Solo cooldown)). Cambia el estado del player al de la habilidad que haya marcado.
        else if (!playableCharacter.abilities[ability - 1].onCooldown)
        {
            playableCharacter.state = (State)ability;

            // Dibujado del área de rango.
            if (!drawnArea)
            {
                drawnArea = true;
                DrawArea(ability);
            }

            players[characterSelected].GetComponent<Player>().SetState(ability);
        }
        // Si la habilidad está en cooldown y tiene cargas, entrará aquí y pondrá el state del player en NONE.
        else
        {
            Debug.Log("Ability on Cooldown! You have" + (playableCharacter.abilities[ability - 1].cooldown - playableCharacter.abilities[ability - 1].counter) + "seconds left to use it again!");
            playableCharacter.state = State.NONE;
        }
    }

    private void DrawArea(int ability)
    {
        area[characterSelected].transform.localPosition = new Vector3(0, playableCharacter.abilities[ability - 1].transformY, 0);
        area[characterSelected].GetComponent<Light>().intensity = playableCharacter.abilities[ability - 1].intensity;
        area[characterSelected].GetComponent<Light>().constant = playableCharacter.abilities[ability - 1].constant;
        area[characterSelected].GetComponent<Light>().linear = playableCharacter.abilities[ability - 1].linear;
        area[characterSelected].GetComponent<Light>().quadratic = playableCharacter.abilities[ability - 1].quadratic;
    }

    private void CastOrCancel()
    {
        if (Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP)
        {
            if (crouched)
            {
                players[characterSelected].GetComponent<Animation>().PlayAnimation("CrouchShoot");
            }
            else
            {
                players[characterSelected].GetComponent<Animation>().PlayAnimation("Shoot");
            }

            switch(playableCharacter.state)
            {
                case State.ABILITY_1:
                    {
                        if (playableCharacter == characters[0])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONTHROWINGKNIFETHROW");
                        }
                        else if (playableCharacter == characters[1])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONCRYSKNIFESTAB");
                        }
                        else if (playableCharacter == characters[2])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONSWORDHIT");
                        }
                        break;
                    }
                case State.ABILITY_2:
                    {
                        if (playableCharacter == characters[0])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("THROWROCK");
                        }
                        else if (playableCharacter == characters[1])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONCAMOUFLAGEACTIVATE");
                        }
                        else if (playableCharacter == characters[2])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONSTUNNERSHOT");
                        }
                        break;
                    }
                case State.ABILITY_3:
                    {
                        if (playableCharacter == characters[0])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("THROWROCK");
                        }
                        else if (playableCharacter == characters[1])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONCAMOUFLAGEACTIVATE");
                        }
                        else if (playableCharacter == characters[2])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONSTUNNERSHOT");
                        }
                        break;
                    }
                case State.ABILITY_4:
                    {
                        if (playableCharacter == characters[0])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("THROWROCK");
                        }
                        else if (playableCharacter == characters[1])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONCAMOUFLAGEACTIVATE");
                        }
                        else if (playableCharacter == characters[2])
                        {
                            players[characterSelected].GetComponent<AudioSource>().PlayClip("WEAPONSTUNNERSHOT");
                        }
                        break;
                    }
                case State.CARRYING:
                    {
                        if (playableCharacter.pickedEnemy != null)
                        {
                            GameObject.ReparentToRoot(playableCharacter.pickedEnemy);

                            playableCharacter.pickedEnemy.transform.localPosition = players[characterSelected].transform.globalPosition;

                            Debug.Log("Dropping the corpse of" + playableCharacter.pickedEnemy.name.ToString());
                            playableCharacter.pickedEnemy = null;
                        }
                        else
                        {
                            NavAgent agent = players[characterSelected].GetComponent<NavAgent>();
                            GameObject obj = RayCast.HitToTag(agent.rayCastA, agent.rayCastB, "Enemies");

                            if (obj != null && Transform.GetDistanceBetween(obj.transform.globalPosition, players[characterSelected].transform.globalPosition) < 3)
                            {
                                GameObject[] enemiesDead = GameObject.Find("EnemyManager").GetComponent<EnemyManager>().deadEnemies;
                                foreach (GameObject g in enemiesDead)
                                {
                                    if (g != null && obj.name == g.name)
                                    {
                                        players[characterSelected].AddChild(obj);

                                        //setear position, animation, whatever de obj
                                        obj.transform.localPosition = new Vector3(0,2,0);

                                        Debug.Log("Carrying the corpse of" + obj.name.ToString());
                                        playableCharacter.pickedEnemy = obj;
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            if (playableCharacter.state != State.CARRYING)
            {
                // Instancia la habilidad en cuestión. 
                InternalCalls.InstancePrefab(playableCharacter.abilities[(int)playableCharacter.state - 1].prefabPath);

                // Al haberse instanciado una habilidad, comprueba si funciona por cargas. Si lo hace resta una carga a la habilidad.
                if (playableCharacter.abilities[(int)playableCharacter.state - 1].charges != -1 && playableCharacter.abilities[(int)playableCharacter.state - 1].charges != 0)
                {
                    playableCharacter.abilities[(int)playableCharacter.state - 1].charges -= 1;
                }

                // Pone la habilidad en cooldown y el player en estado de NONE
                playableCharacter.abilities[(int)playableCharacter.state - 1].onCooldown = true;
            }
            playableCharacter.state = State.NONE;

            // Se cambia el estado a POSTCAST para evitar que se mueva directamente después de castear la habilidad. En el update de los players se cambiará a NONE nuevamente para que se pueda mover (Tras un ciclo de update). 
            players[characterSelected].GetComponent<Player>().SetState((int)State.POSTCAST);

            area[characterSelected].GetComponent<Light>().intensity = 0f;
            drawnArea = false;
        }
        // Se cancela el estado de la habilidad para que el área de rango deje de mostrarse.
        if (Input.GetMouseClick(MouseButton.RIGHT) == KeyState.KEY_DOWN)
        {
            playableCharacter.state = State.NONE;
            players[characterSelected].GetComponent<Player>().SetState((int)State.NONE);

            area[characterSelected].GetComponent<Light>().intensity = 0f;
            drawnArea = false;
        }
    }

    private void PlayerCases()
    {
        switch (players.Length)
        {
            case 4:
                if (Input.GetKey(KeyCode.F4) == KeyState.KEY_DOWN)
                {
                    players[characterSelected].GetComponent<Player>().SetState((int)State.NONE);
                    characterSelected = 3;
                    playableCharacter.state = State.NONE;
                    if (area != null) area[characterSelected].GetComponent<Light>().intensity = 0f;
                    playableCharacter = characters[characterSelected];
                    ChangeCharacter(characterSelected);
                    Debug.Log("Character Changed");
                }
                goto case 3;
            case 3:
                if (Input.GetKey(KeyCode.F3) == KeyState.KEY_DOWN)
                {
                    players[characterSelected].GetComponent<Player>().SetState((int)State.NONE);
                    characterSelected = 2;
                    playableCharacter.state = State.NONE;
                    if(area != null) area[characterSelected].GetComponent<Light>().intensity = 0f;
                    playableCharacter = characters[characterSelected];
                    ChangeCharacter(characterSelected);
                    Debug.Log("Character Changed");
                }
                goto case 2;
            case 2:
                if (Input.GetKey(KeyCode.F2) == KeyState.KEY_DOWN)
                {
                    players[characterSelected].GetComponent<Player>().SetState((int)State.NONE);
                    characterSelected = 1;
                    playableCharacter.state = State.NONE;
                    if (area != null) area[characterSelected].GetComponent<Light>().intensity = 0f;
                    playableCharacter = characters[characterSelected];
                    ChangeCharacter(characterSelected);
                    Debug.Log("Character Changed");
                }
                goto case 1;
            case 1:
                if (Input.GetKey(KeyCode.F1) == KeyState.KEY_DOWN)
                {
                    players[characterSelected].GetComponent<Player>().SetState((int)State.NONE);
                    characterSelected = 0;
                    playableCharacter.state = State.NONE;
                    if (area != null) area[characterSelected].GetComponent<Light>().intensity = 0f;
                    playableCharacter = characters[characterSelected];
                    ChangeCharacter(characterSelected);
                    Debug.Log("Character Changed");
                }
                break;
        }
    }

    void ChangeCharacter(int id)
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<Player>().SetControled(false);

        }
        players[id].GetComponent<Player>().SetControled(true);
        
    }
}


