using System;
using RagnarEngine;

public class winScren : RagnarComponent
{
	Vector3 Pos;
	Vector3 Size;
	GameObject Back;
	GameObject Menu;
	GameObject Next;
	GameObject Replay;

	GameObject MenuImage;
	GameObject NextImage;
	GameObject ReplayImage;
	GameObject AudioManager;

	bool isFirstM = false;
	bool isFirstA = false;
	bool isFirstR = false;
	bool isFirst = true;
	public void Start()
	{
		Back = GameObject.Find("Background");
		Menu = GameObject.Find("ButtonMenu");
		Next = GameObject.Find("ButtonNextLevel");
		Replay = GameObject.Find("ButtonReplay");

		MenuImage = GameObject.Find("Main Menu Image");
		NextImage = GameObject.Find("Next Level Image");
		ReplayImage = GameObject.Find("Replay Image");
		AudioManager = GameObject.Find("AudioWinMenu");

		Pos = new Vector3(0, 0, 0);
		Size = new Vector3(350, 94, 0);
		Pos.Set(InternalCalls.GetRegionGame().x / 2 + 200, Next.GetComponent<Transform2D>().position2D.y, 36.1f);
		Next.GetComponent<Transform2D>().position2D = Pos;

		Pos.Set(InternalCalls.GetRegionGame().x / 2 +200, NextImage.GetComponent<Transform2D>().position2D.y, 36.1f);
		NextImage.GetComponent<Transform2D>().position2D = Pos;
	}
	void MenuAction()
	{
		int a = Menu.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
		case 0:
			// disabled Mode
			break;
		case 1:
			if (!isFirstM)
			{
				Pos.Set(Menu.GetComponent<Transform2D>().position2D.x - 20, Menu.GetComponent<Transform2D>().position2D.y , 36.1f);
				Menu.GetComponent<Transform2D>().position2D = Pos;

				Pos.Set(MenuImage.GetComponent<Transform2D>().position2D.x - 20, MenuImage.GetComponent<Transform2D>().position2D.y , 36.1f);
				MenuImage.GetComponent<Transform2D>().position2D = Pos;
				isFirstM = true;
			}
			// normal Mode
			break;
		case 2:
			// focused mode
			if (isFirstM)
			{
				Pos.Set(Menu.GetComponent<Transform2D>().position2D.x + 20, Menu.GetComponent<Transform2D>().position2D.y, 36.1f);
				Menu.GetComponent<Transform2D>().position2D = Pos;

				Pos.Set(MenuImage.GetComponent<Transform2D>().position2D.x + 20, MenuImage.GetComponent<Transform2D>().position2D.y , 36.1f);
				MenuImage.GetComponent<Transform2D>().position2D = Pos;
				isFirstM = false;
				isFirst = false;
				//poner sonido
				AudioManager.GetComponent<AudioSource>().PlayClip("UI_HOVER");
			}
			break;
		case 3:
			// pressed mode
			AudioManager.GetComponent<AudioSource>().PlayClip("UI_SELECT");
			Menu.GetComponent<UIButton>().SetButtonAlpha(0.75f);
			SceneManager.LoadScene("MainMenu");
			//cambiar de escena
			break;
		}
	}
	void NextAction()
	{		
		int a = Next.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
		case 0:
			// disabled Mode
			break;
		case 1:
			if (!isFirstA)
			{
				Pos.Set((InternalCalls.GetRegionGame().x / 2)-100, Next.GetComponent<Transform2D>().position2D.y, 36.1f);
				Next.GetComponent<Transform2D>().position2D = Pos;

				Pos.Set((InternalCalls.GetRegionGame().x / 2)-100, NextImage.GetComponent<Transform2D>().position2D.y, 36.1f);
				NextImage.GetComponent<Transform2D>().position2D = Pos;
				isFirstA = true;
			}
			// normal Mode
			break;
		case 2:
			// focused mode
			if (isFirstA)
			{
				Pos.Set((InternalCalls.GetRegionGame().x / 2) - 120, Next.GetComponent<Transform2D>().position2D.y, 36.1f);
				Next.GetComponent<Transform2D>().position2D = Pos;
				isFirst = false;
				Pos.Set((InternalCalls.GetRegionGame().x / 2) - 120, NextImage.GetComponent<Transform2D>().position2D.y, 36.1f);
				NextImage.GetComponent<Transform2D>().position2D = Pos;
				AudioManager.GetComponent<AudioSource>().PlayClip("UI_HOVER");
				isFirstA = false;
				//poner sonido
			}
			break;
		case 3:
			// pressed mode
			Next.GetComponent<UIButton>().SetButtonAlpha(0.75f);
			if (SceneManager.lastSceneName == "build")
				SceneManager.LoadScene("build2");
			if (SceneManager.lastSceneName == "build2")
				SceneManager.LoadScene("build3");
			if (SceneManager.lastSceneName == "build3")
				SceneManager.LoadScene("Credits");
			//cambiar de escena
			break;
		}
	}

	void ReplayAction()
	{
		int a = Replay.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
		case 0:
			// disabled Mode
			break;
		case 1:
			if (!isFirstR)
			{

				Size.Set(301.5f - 20,51.5f, 0);
				Replay.GetComponent<Transform2D>().SetSize(Size);

				Size.Set(294 - 20, 46.5f, 0);
				ReplayImage.GetComponent<Transform2D>().SetSize(Size);
				isFirstR = true;
				
			}
			// normal Mode
			break;
		case 2:
			// focused mode
			if (isFirstR)
			{
				Size.Set(301.5f + 20, 51.5f, 0);
				Replay.GetComponent<Transform2D>().SetSize(Size);

				Size.Set(294 + 20, 46.5f, 0);
				ReplayImage.GetComponent<Transform2D>().SetSize(Size);
				isFirst = false;
				isFirstR = false;
				//poner sonido
				AudioManager.GetComponent<AudioSource>().PlayClip("UI_HOVER");
			}
			break;
		case 3:
			// pressed mode
			Replay.GetComponent<UIButton>().SetButtonAlpha(0.75f);
			if (SceneManager.lastSceneName == "build")
				SceneManager.LoadScene("build");
			if (SceneManager.lastSceneName == "build2")
				SceneManager.LoadScene("build2");
			if (SceneManager.lastSceneName == "build3")
				SceneManager.LoadScene("build3");
				//cambiar de escena
			break;
		}
	}
	public void Update()
	{
		MenuAction();
		NextAction();
		ReplayAction();
		if(isFirst)
			firstTime();
		Back.GetComponent<Transform2D>().SetSize(InternalCalls.GetRegionGame());
	}
	void firstTime()
    {
		Pos.Set((InternalCalls.GetRegionGame().x / 2) - 100, Next.GetComponent<Transform2D>().position2D.y, 36.1f);
		Next.GetComponent<Transform2D>().position2D = Pos;

		Pos.Set((InternalCalls.GetRegionGame().x / 2) - 100, NextImage.GetComponent<Transform2D>().position2D.y, 36.1f);
		NextImage.GetComponent<Transform2D>().position2D = Pos;
	}

}

