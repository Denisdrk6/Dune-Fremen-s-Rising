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

	UIButton menuButton;
	Transform2D menuTransform;
	Transform2D menuImageTransform;

	GameObject MenuImage;
	GameObject NextImage;
	GameObject ReplayImage;
	GameObject AudioManager;

	AudioSource managerAudioSource;

	Transform2D nextTransform;
	Transform2D nextImageTransform;
	UIButton nextButton;

	Transform2D replayTransform;
	Transform2D replayImageTransform;
	UIButton replayButton;

	Transform2D backTransform;

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
		
		managerAudioSource = AudioManager.GetComponent<AudioSource>();

		menuButton = Menu.GetComponent<UIButton>();
		menuTransform = Menu.GetComponent<Transform2D>();
		
		menuImageTransform = MenuImage.GetComponent<Transform2D>();
		backTransform = Back.GetComponent<Transform2D>();

		nextButton = Next.GetComponent<UIButton>();
		nextTransform = Next.GetComponent<Transform2D>();
		nextImageTransform = NextImage.GetComponent<Transform2D>();

		replayTransform = Replay.GetComponent<Transform2D>();
		replayImageTransform = ReplayImage.GetComponent<Transform2D>();
		replayButton = Replay.GetComponent<UIButton>();

		Pos = new Vector3(0, 0, 0);
		Size = new Vector3(350, 94, 0);
		Pos.Set(InternalCalls.GetRegionGame().x / 2 + 200, nextTransform.position2D.y, 36.1f);
		nextTransform.position2D = Pos;

		Pos.Set(InternalCalls.GetRegionGame().x / 2 +200, nextImageTransform.position2D.y, 36.1f);
		nextImageTransform.position2D = Pos;
	}
	void MenuAction()
	{
		int a = menuButton.GetButtonState();
		switch (a)
		{
		case 0:
			// disabled Mode
			break;
		case 1:
			if (!isFirstM)
			{
				Pos.Set(menuTransform.position2D.x - 20, menuTransform.position2D.y , 36.1f);
				menuTransform.position2D = Pos;

				Pos.Set(menuImageTransform.position2D.x - 20, menuImageTransform.position2D.y , 36.1f);
				menuImageTransform.position2D = Pos;
				isFirstM = true;
			}
			// normal Mode
			break;
		case 2:
			// focused mode
			if (isFirstM)
			{
				Pos.Set(menuTransform.position2D.x + 20, menuTransform.position2D.y, 36.1f);
				menuTransform.position2D = Pos;

				Pos.Set(menuImageTransform.position2D.x + 20, menuImageTransform.position2D.y , 36.1f);
				menuImageTransform.position2D = Pos;
				isFirstM = false;
				isFirst = false;
				//poner sonido
				managerAudioSource.PlayClip("UI_HOVER");
			}
			break;
		case 3:
				// pressed mode
			managerAudioSource.PlayClip("UI_SELECT");
			menuButton.SetButtonAlpha(0.75f);
			SceneManager.LoadScene("MainMenu");
			//cambiar de escena
			break;
		}
	}
	void NextAction()
	{		
		int a = nextButton.GetButtonState();
		switch (a)
		{
		case 0:
			// disabled Mode
			break;
		case 1:
			if (!isFirstA)
			{
				Pos.Set((InternalCalls.GetRegionGame().x / 2)-100, nextTransform.position2D.y, 36.1f);
				nextTransform.position2D = Pos;

				Pos.Set((InternalCalls.GetRegionGame().x / 2)-100, nextImageTransform.position2D.y, 36.1f);
				nextImageTransform.position2D = Pos;
				isFirstA = true;
			}
			// normal Mode
			break;
		case 2:
			// focused mode
			if (isFirstA)
			{
				Pos.Set((InternalCalls.GetRegionGame().x / 2) - 120, nextTransform.position2D.y, 36.1f);
				nextTransform.position2D = Pos;
				isFirst = false;
				Pos.Set((InternalCalls.GetRegionGame().x / 2) - 120, nextImageTransform.position2D.y, 36.1f);
				nextImageTransform.position2D = Pos;
				managerAudioSource.PlayClip("UI_HOVER");
				isFirstA = false;
				//poner sonido
			}
			break;
		case 3:
			// pressed mode
			nextButton.SetButtonAlpha(0.75f);
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
		int a = replayButton.GetButtonState();
		switch (a)
		{
		case 0:
			// disabled Mode
			break;
		case 1:
			if (!isFirstR)
			{

				Size.Set(301.5f - 20,51.5f, 0);
				replayTransform.SetSize(Size);

				Size.Set(294 - 20, 46.5f, 0);
				replayImageTransform.SetSize(Size);
				isFirstR = true;
				
			}
			// normal Mode
			break;
		case 2:
			// focused mode
			if (isFirstR)
			{
				Size.Set(301.5f + 20, 51.5f, 0);
				replayTransform.SetSize(Size);

				Size.Set(294 + 20, 46.5f, 0);
				replayImageTransform.SetSize(Size);
				isFirst = false;
				isFirstR = false;
				//poner sonido
				managerAudioSource.PlayClip("UI_HOVER");
			}
			break;
		case 3:
			// pressed mode
			replayButton.SetButtonAlpha(0.75f);
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
		backTransform.SetSize(InternalCalls.GetRegionGame());
	}
	void firstTime()
    {
		Pos.Set((InternalCalls.GetRegionGame().x / 2) - 100, nextTransform.position2D.y, 36.1f);
		nextTransform.position2D = Pos;

		Pos.Set((InternalCalls.GetRegionGame().x / 2) - 100, nextImageTransform.position2D.y, 36.1f);
		nextImageTransform.position2D = Pos;
	}

}

