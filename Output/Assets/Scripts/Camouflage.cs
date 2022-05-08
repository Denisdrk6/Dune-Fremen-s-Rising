using System;
using RagnarEngine;

public class Camouflage : RagnarComponent
{
	public GameObject player;
	float time1 = 5f;
	public void Start()
	{
		player = GameObject.Find("Player_2");
	}
	public void Update()
	{
		SetMaterialTransparent("Assets/Resources/UI/transparent_tex.png");
		if(Timer())
        {
			SetMaterialTransparent("Assets/Resources/CharacterTex/char_chani_albedo.png");
			player.GetComponent<Player>().invisible = false;
			InternalCalls.Destroy(gameObject);
		}
	}
	private void SetMaterialTransparent(string path)
    {
		player.GetComponent<Material>().SetTexturePath(path);
		player.GetComponent<Player>().invisible = true;
    }
	public bool Timer()//moltes gracies Isaac
	{
		if (time1 > 0)
		{
			time1 -= Time.deltaTime;
			return false;
		}
		else
		{
			return true;
		}
	}

}