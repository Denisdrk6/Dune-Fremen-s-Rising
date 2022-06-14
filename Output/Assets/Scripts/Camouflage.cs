using System;
using RagnarEngine;

public class Camouflage : RagnarComponent
{
	// Components
	private Player playerScript;

	///////////////
	public GameObject player;
    private GameObject SceneAudio;
	float time1 = 5f;
	UIText buffCounter;
	float buffTemp;

	GameObject buffImg;

	public void Start()
	{
		player = GameObject.Find("Player_2");
		playerScript = player.GetComponent<Player>();

		SceneAudio = GameObject.Find("AudioLevel1");
		//player.GetComponent<Animation>().PlayAnimation("Ability2");
        player.GetComponent<Player>().PlayAudioClip("WPN_CAMOUFLAGEACTIVATE");
		buffCounter = GameObject.Find("UIBCamoNum").GetComponent<UIText>();
		buffImg = GameObject.Find("UIBCamoImg");
		SetMaterial("Assets/Resources/UI/options_rect.png");
    }
	public void Update()
	{
		if(Timer())
        {
            SceneAudio.GetComponent<AudioSource>().PlayClip("WPN_CAMOUFLAGEDEACTIVATE");
			SetMaterial("Assets/Resources/CharacterTex/char_chani_basecolor.png");
			playerScript.invisible = false;
			InternalCalls.Destroy(gameObject);
		}
	}
	private void SetMaterial(string path)
    {
		player.GetComponent<Material>().SetTexturePath(path);
		playerScript.invisible = true;
    }
	public bool Timer()
	{
		QuestSystem system = GameObject.Find("Quest System").GetComponent<QuestSystem>();
		if (time1 > 0)
		{
			system.camouflageActive = true;

			buffImg.isActive = true;

			buffTemp = time1;
			buffTemp = (float)Math.Round((double)buffTemp, 0);

			buffCounter.text = buffTemp.ToString();
			time1 -= Time.deltaTime;
			return false;
		}
		else
		{
			system.camouflageActive = false;
			buffImg.isActive = false;
			buffCounter.text = "";
			return true;
		}
	}

}