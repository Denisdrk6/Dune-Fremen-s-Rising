using System;
using RagnarEngine;

public class Credits : RagnarComponent
{
	Vector3 Pos;
	GameObject Back;
	GameObject Menu;
	GameObject TextJob;
	GameObject TextName;
	GameObject TextTitles;
	GameObject MenuImage;
	GameObject RagnarLogo;
	GameObject UPCLogo;
	GameObject AudioManager;
	float textCounter = 0;
	float newDelta=0;
	float velocity = 30;
	bool isFirstM = true;
	public void Start()
	{
		Back = GameObject.Find("Background");
		Menu = GameObject.Find("ButtonMenu");


		MenuImage = GameObject.Find("Main Menu Image");
		TextJob = GameObject.Find("Jobs List");
		TextName= GameObject.Find("Names List");
		TextTitles = GameObject.Find("Tiles List");
		RagnarLogo = GameObject.Find("Ragnar Logo");
		UPCLogo = GameObject.Find("UPC Logo");
		AudioManager = GameObject.Find("AudioWinMenu");
		
		Pos = new Vector3(0, 0, 0);

		Pos.Set(-150, -(InternalCalls.GetRegionGame().y / 2)+210, 36.1f);
		TextJob.GetComponent<Transform2D>().position2D = Pos;
		Pos.Set(-40, -(InternalCalls.GetRegionGame().y / 2)+210, 36.1f);
		TextName.GetComponent<Transform2D>().position2D = Pos;
		Pos.Set(-140, -(InternalCalls.GetRegionGame().y / 2) + 230, 36.1f);
		TextTitles.GetComponent<Transform2D>().position2D = Pos;

		Pos.Set(0, -(InternalCalls.GetRegionGame().y / 2) - 4300, -10.4f);
		UPCLogo.GetComponent<Transform2D>().position2D = Pos;
		Pos.Set(0, -(InternalCalls.GetRegionGame().y / 2) - 4450, -10.4f);
		RagnarLogo.GetComponent<Transform2D>().position2D = Pos;
	}
	public void Update()
	{
		newDelta = Time.deltaTime;

		BackgroundImageAction();

		MenuAction();

		textAction();
	}
	void textAction()
    {
		if (textCounter >= 5)
        {			
			float vel = newDelta * velocity;
			Pos.Set(-150, TextJob.GetComponent<Transform2D>().position2D.y + vel, 36.1f);
			TextJob.GetComponent<Transform2D>().position2D = Pos;

			Pos.Set(30, TextName.GetComponent<Transform2D>().position2D.y + vel, 36.1f);
			TextName.GetComponent<Transform2D>().position2D = Pos;

			Pos.Set(-100, TextTitles.GetComponent<Transform2D>().position2D.y + vel, 36.1f);
			TextTitles.GetComponent<Transform2D>().position2D = Pos;

			Pos.Set(0, UPCLogo.GetComponent<Transform2D>().position2D.y + (vel*2), 36.1f);
			UPCLogo.GetComponent<Transform2D>().position2D = Pos;

			Pos.Set(0, RagnarLogo.GetComponent<Transform2D>().position2D.y + (vel * 2), 36.1f);
			RagnarLogo.GetComponent<Transform2D>().position2D = Pos;

		}
        else
        {
			textCounter += newDelta;
		}
		
	}
	void BackgroundImageAction()
	{

		Back.GetComponent<Transform2D>().SetSize(InternalCalls.GetRegionGame());

		Pos = Back.GetComponent<Transform2D>().position2D;
		Pos.Set(0.0f, 0.0f, 36.0f);
		Back.GetComponent<Transform2D>().position2D = Pos;
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
					Pos.Set(Menu.GetComponent<Transform2D>().position2D.x - 20, Menu.GetComponent<Transform2D>().position2D.y, 36.1f);
					Menu.GetComponent<Transform2D>().position2D = Pos;


					Pos.Set(MenuImage.GetComponent<Transform2D>().position2D.x - 20, MenuImage.GetComponent<Transform2D>().position2D.y, 36.1f);
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

					Pos.Set(MenuImage.GetComponent<Transform2D>().position2D.x + 20, MenuImage.GetComponent<Transform2D>().position2D.y, 36.1f);
					MenuImage.GetComponent<Transform2D>().position2D = Pos;
					isFirstM = false;
					//poner sonido
					AudioManager.GetComponent<AudioSource>().PlayClip("UIHOVER");
				}
				break;
			case 3:
				// pressed mode
				AudioManager.GetComponent<AudioSource>().PlayClip("UISELECT");
				Menu.GetComponent<UIButton>().SetButtonAlpha(0.75f);
				SceneManager.LoadScene("MainMenu");
				//cambiar de escena
				break;


		}
	}
}

