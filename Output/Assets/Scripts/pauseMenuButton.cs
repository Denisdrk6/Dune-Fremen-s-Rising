using System;
using RagnarEngine;

public class pauseMenuButton : RagnarComponent
{
	string actualOption = "Screen";
	Vector3 pos;
	Vector3 mouseLastposition;
	Vector3 bounds;
	bool isOptions = false;
	bool isSowing = false;
	bool isFirstS = true;
	bool isFirstE = true;
	bool isFirstO = true;
	GameObject[] players;

	//////////////PAUSE//////////////
	GameObject Image;
	GameObject Resume;
	GameObject MainM;
	GameObject Opt;
	GameObject restart;

	GameObject ResumeImage;
	GameObject MainMImage;
	GameObject OptImage;
	GameObject restartImage;

	GameObject CharFocusedImage;
	GameObject CharFocusedText;
	//////////////OPTIONS//////////////
	bool isFirstR = true;
	bool isFirstOBackB = true;
	bool isFirstOScreenB = true;
	bool isFirstOSoundB = true;
	bool isFirstOControlsB = true;

	bool isFirstOcontrolsL = true;
	bool isFirstOcontrolsR = true;
	float lastWindowW = 0;
	GameObject optionsBack;
	GameObject optionsBackImage;
	GameObject optionsBackButton;
	GameObject optionsTransCuad;
	GameObject optionsTitle;
	GameObject optionsScreenButton;
	GameObject optionsScreenImage;
	GameObject optionsSoundButton;
	GameObject optionsSoundImage;
	GameObject optionsControlsButton;
	GameObject optionsControlsImage;

	GameObject optionsLanguaje;
	GameObject optionsScreenSDCH;
	GameObject optionsScreenVSCH;
	GameObject optionsScreenFSCH;
	GameObject optionsSreenText;

	GameObject optionsSoundText;
    GameObject optionsVoicesSound;
    GameObject optionsFXcSound;
    GameObject optionsMusicSound;
    GameObject optionsGeneralSound;

	GameObject optionsControlText;
	GameObject optionsControlText1;
	GameObject optionsControlText2;
	GameObject optionsControl1;
	GameObject optionsControl2;
	GameObject optionsControl3;
	GameObject optionsControl4;
	GameObject optionsControl5;
	GameObject optionsControl6;
	GameObject optionsControl7;
	GameObject optionsControl8;
	GameObject optionsControl9;
	GameObject optionsControl10;
	GameObject optionsControl11;
	GameObject optionsControl12;
	GameObject optionsControl13;
	GameObject optionsControl14;
	GameObject optionsControl15;
	GameObject optionsControl16;
	GameObject optionsControl17;
	GameObject optionsControl18;
	GameObject optionsControl19;
	GameObject optionsControl20;
	GameObject optionsControl21;
	GameObject optionsControl22;

	GameObject optionsControlR;
	GameObject optionsControlL;
	int actualControlOption = 1;
	//////////////AUDIO//////////////
	GameObject SceneAudio;
	float currVolume = 0.0f;

	//////////////GAME//////////////
	GameObject selectedPlayer;
    GameObject playerManager;
	string lastPlayerSelected;
	bool isFirstA1 = true;
	bool isFirstA2 = true;
	bool isFirstA3 = true;
	bool isFirstA4 = true;
	bool isFirstA5 = true;
	DialogueManager dialogue;
	
	GameObject CharacterPhotoBord;
	GameObject AbilityBord;
	GameObject AbilityImage;
	GameObject Ability1;
	GameObject Ability2;
	GameObject Ability3;
	GameObject Ability4;
	GameObject Ability5;

	GameObject UICharPhoto;
	GameObject UIAbilityArray;
	GameObject UICharBor1;
	GameObject UICharBor2;
	GameObject UICharBor3;
	GameObject UISelector;
	GameObject UICharacterName;

	GameObject UIPaulImage;
	GameObject UIChaniImage;
	GameObject UIStilgarImage;

	GameObject AbilityImageApmliate;
	GameObject abilityLeters;
	public void Start()
	{
		pos = new Vector3(0.0f, 0.0f, 0.0f);
		mouseLastposition = new Vector3(0.0f, 0.0f, 0.0f);
		bounds = new Vector3(0.0f, 0.0f, 0.0f);
		//////////////AUDIO//////////////
		SceneAudio = GameObject.Find("AudioLevel1");
		SceneAudio.GetComponent<AudioSource>().PlayClip("LEVEL1BGMUSIC");

		//////////////PAUSE//////////////
		Image = GameObject.Find("Background");
		Resume = GameObject.Find("Button Resume");
		MainM = GameObject.Find("Button MainMenu"); 
		Opt = GameObject.Find("Button Options");
		restart = GameObject.Find("Button Restart");

		ResumeImage = GameObject.Find("ResumeImage");
		MainMImage = GameObject.Find("MainMenuImage");
		OptImage = GameObject.Find("optionsImage");
		restartImage = GameObject.Find("restartImage");

		pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, -(InternalCalls.GetRegionGame().y / 2) + 660, 36.1f);
		restart.GetComponent<Transform2D>().position2D = pos;
		pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, -(InternalCalls.GetRegionGame().y / 2) + 660, 36.1f);
		restartImage.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, -(InternalCalls.GetRegionGame().y / 2) + 600, 36.1f);
		Resume.GetComponent<Transform2D>().position2D = pos;
		pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, -(InternalCalls.GetRegionGame().y / 2) + 600, 36.1f);
		ResumeImage.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, -(InternalCalls.GetRegionGame().y / 2) + 540, 36.1f);
		MainM.GetComponent<Transform2D>().position2D = pos;
		pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, -(InternalCalls.GetRegionGame().y / 2) + 540, 36.1f);
		MainMImage.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, -(InternalCalls.GetRegionGame().y / 2) + 480, 36.1f);
		Opt.GetComponent<Transform2D>().position2D = pos;
		pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, -(InternalCalls.GetRegionGame().y / 2) + 480, 36.1f);
		OptImage.GetComponent<Transform2D>().position2D = pos;

		ImageHide();
		RestartButtonHide();
		ResumeButtonHide();
		MainMenuButtonHide();
		OptionsButtonHide();
		//////////////OPTIONS//////////////
		



        optionsBack = GameObject.Find("optionsBack");
        optionsBackImage = GameObject.Find("optionsBackImage");
        optionsBackButton = GameObject.Find("optionsBackButton");
		optionsTransCuad = GameObject.Find("optionsTransCuad");
		optionsTitle = GameObject.Find("optionsTitle");
		optionsScreenButton = GameObject.Find("optionsScreenButton");
		optionsScreenImage = GameObject.Find("optionsScreenImage");
		optionsSoundButton = GameObject.Find("optionsSoundButton");
		optionsSoundImage = GameObject.Find("optionsSoundImage");
		optionsControlsButton = GameObject.Find("optionsControlsButton");
		optionsControlsImage = GameObject.Find("optionsControlsImage");

		optionsScreenSDCH = GameObject.Find("optionsScreenSDCH");
		optionsScreenVSCH = GameObject.Find("optionsScreenVSCH");
		optionsScreenFSCH = GameObject.Find("optionsScreenFSCH"); 
		optionsSreenText = GameObject.Find("optionsSreenText");
		optionsLanguaje = GameObject.Find("optionsLanguaje");

		optionsSoundText = GameObject.Find("optionsSoundText");

        optionsVoicesSound = GameObject.Find("optionsVoicesSound");
        optionsFXcSound = GameObject.Find("optionsFXcSound");
        optionsMusicSound = GameObject.Find("optionsMusicSound");
        optionsGeneralSound = GameObject.Find("optionsGeneralSound");
		lastWindowW = (InternalCalls.GetRegionGame().x / 2);
		//optionsScreenSDCH.GetComponent<UICheckbox>().SetCheckboxState(Light.shadowsEnabled);

		optionsScreenSDCH.GetComponent<UICheckbox>().SetCheckboxState(Light.shadowsEnabled);
		optionsScreenFSCH.GetComponent<UICheckbox>().SetCheckboxState(InternalCalls.GetFullScreen());
		//InternalCalls.SetFullScreen(true);
		optionsScreenVSCH.GetComponent<UICheckbox>().SetCheckboxState(InternalCalls.GetVSync());

		optionsControlText = GameObject.Find("optionsControlText");
		optionsControlText1 = GameObject.Find("optionsControlText1");
		optionsControlText2 = GameObject.Find("optionsControlText2");

		optionsControl1 = GameObject.Find("optionsControl1");
		optionsControl2 = GameObject.Find("optionsControl2");
		optionsControl3 = GameObject.Find("optionsControl3");
		optionsControl4 = GameObject.Find("optionsControl4");
		optionsControl5 = GameObject.Find("optionsControl5");
		optionsControl6 = GameObject.Find("optionsControl6");
		optionsControl7 = GameObject.Find("optionsControl7");
		optionsControl8 = GameObject.Find("optionsControl8");
		optionsControl9 = GameObject.Find("optionsControl9");
		optionsControl10 = GameObject.Find("optionsControl10");
		optionsControl11 = GameObject.Find("optionsControl11");
		optionsControl12 = GameObject.Find("optionsControl12");
		optionsControl13 = GameObject.Find("optionsControl13");
		optionsControl14 = GameObject.Find("optionsControl14");
		optionsControl15 = GameObject.Find("optionsControl15");
		optionsControl16 = GameObject.Find("optionsControl16");
		optionsControl17 = GameObject.Find("optionsControl17");
		optionsControl18 = GameObject.Find("optionsControl18");
		optionsControl19 = GameObject.Find("optionsControl19");
		optionsControl20 = GameObject.Find("optionsControl20");
		optionsControl21 = GameObject.Find("optionsControl21");
		optionsControl22 = GameObject.Find("optionsControl22");

		optionsControlR = GameObject.Find("optionsControlR");
		optionsControlL = GameObject.Find("optionsControlL");

		optionsControl1.GetComponent<UIButton>().text = "Esc";
		optionsControl2.GetComponent<UIButton>().text = "1";
		optionsControl3.GetComponent<UIButton>().text = "2";
		optionsControl4.GetComponent<UIButton>().text = "3";
		optionsControl5.GetComponent<UIButton>().text = "R Click";
		optionsControl6.GetComponent<UIButton>().text = "L Click";
		optionsControl7.GetComponent<UIButton>().text = "Space";
		optionsControl8.GetComponent<UIButton>().text = "Drag";
		optionsControl9.GetComponent<UIButton>().text = "Crl L";
		optionsControl10.GetComponent<UIButton>().text = "Drag";
		optionsControl11.GetComponent<UIButton>().text = "F1";
		optionsControl12.GetComponent<UIButton>().text = "F5";
		optionsControl13.GetComponent<UIButton>().text = "F6";
		optionsControl14.GetComponent<UIButton>().text = "A";
		optionsControl15.GetComponent<UIButton>().text = "S";
		optionsControl16.GetComponent<UIButton>().text = "D";
		optionsControl17.GetComponent<UIButton>().text = "F";
		optionsControl18.GetComponent<UIButton>().text = "G";
		optionsControl19.GetComponent<UIButton>().text = "R Click";
		optionsControl20.GetComponent<UIButton>().text = "L Click";
		optionsControl21.GetComponent<UIButton>().text = "J";
		optionsControl22.GetComponent<UIButton>().text = "Shift";
		OptionsBackHide();
		//////////////GAME//////////////
		CharacterPhotoBord = GameObject.Find("Char");
		Ability1 = GameObject.Find("ab1");
		Ability2 = GameObject.Find("ab2");
		AbilityImage = GameObject.Find("AbilImage");
        dialogue = GameObject.Find("Dialogue").GetComponent<DialogueManager>();

		Ability3 = GameObject.Find("ab3");
		Ability4 = GameObject.Find("ab4");
		Ability5 = GameObject.Find("ab5");

		AbilityBord = GameObject.Find("AbilImage");

		UICharPhoto = GameObject.Find("UICharPhoto");

		UICharacterName = GameObject.Find("UICharacterName");

		UIAbilityArray = GameObject.Find("UIAbilityArray"); 
		UICharBor1 = GameObject.Find("UICharBor1"); 
		UICharBor2 = GameObject.Find("UICharBor2"); 
		UICharBor3 = GameObject.Find("UICharBor3"); 
		UISelector = GameObject.Find("UISelector");
		UIPaulImage = GameObject.Find("UIPaulImage");
		UIChaniImage = GameObject.Find("UIChaniImage");
		UIStilgarImage = GameObject.Find("UIStilgarImage");

		CharFocusedText = GameObject.Find("CharFocusedText");
		CharFocusedImage = GameObject.Find("CharFocusedImage");

		AbilityImageApmliate = GameObject.Find("AbilityImageApmliate");
		abilityLeters = GameObject.Find("abilityLeters");

		players = GameObject.FindGameObjectsWithTag("Player");
        playerManager = GameObject.Find("PlayerManager");
		selectedPlayer = players[playerManager.GetComponent<PlayerManager>().characterSelected];
		lastPlayerSelected = selectedPlayer.name;
	}
    public void Update()
	{
        //para pillar el hitPoint del mouse Pick
        //selectedPlayer.GetComponent<NavAgent>().hitPosition
        players = GameObject.FindGameObjectsWithTag("Player");
        selectedPlayer = players[playerManager.GetComponent<PlayerManager>().characterSelected];
		
		SetAllPositions();
		UpdateMenu();
		UpdateOptions();
		UpdatePlayerPause();
		if (Input.GetKey(KeyCode.I) == KeyState.KEY_DOWN)
        {
			SceneManager.LoadScene("WinScene");
        }

	}
	void UpdatePlayerPause()
    {
		if(isSowing || isOptions)
        {
			for(int a = 0; a < players.Length; a++)
            {
				players[a].GetComponent<Player>().paused = true;

			}
        }
        else
        {
			for (int a = 0; a < players.Length; a++)
			{
				players[a].GetComponent<Player>().paused = false;

			}
		}
    }
	//////////////OPTIONS//////////////
	void OptionsControlShow()
	{
		float y = (InternalCalls.GetRegionGame().y / 2);
		float x = (InternalCalls.GetRegionGame().x / 2);
		float sum = (lastWindowW - x);

		optionsControlText.isActive = true;
		optionsControlText1.isActive = true;
		optionsControlText2.isActive = true;

		optionsControl1.isActive = true;
		optionsControl2.isActive = true;
		optionsControl3.isActive = true;
		optionsControl4.isActive = true;
		optionsControl5.isActive = true;
		optionsControl6.isActive = true;
		optionsControl7.isActive = true;
		optionsControl8.isActive = true;
		optionsControl9.isActive = true;
		optionsControl10.isActive = true;
		optionsControl11.isActive = true;
		optionsControl12.isActive = true;
		optionsControl13.isActive = true;
		optionsControl14.isActive = true;
		optionsControl15.isActive = true;
		optionsControl16.isActive = true;
		optionsControl17.isActive = true;
		optionsControl18.isActive = true;
		optionsControl19.isActive = true;
		optionsControl20.isActive = true;
		optionsControl21.isActive = true;
		optionsControl22.isActive = true;
		optionsControlR.isActive = true;
		optionsControlL.isActive = true;
		pos.Set(-sum + 600, y - 780, 36.1f);
		optionsControlL.GetComponent<Transform2D>().position2D = pos;
		pos.Set(-sum + 800, y - 780, 36.1f);
		optionsControlR.GetComponent<Transform2D>().position2D = pos;

		int a = optionsControlL.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:

				// normal Mode
				break;
			case 2:
				// focused mode
				isFirstOcontrolsL = true;
				break;
			case 3:
                // pressed mode
                if (isFirstOcontrolsL) { 
					if (actualControlOption == 0)
					{
						actualControlOption = 1;

					}
					else
					{
						actualControlOption--;
					}
					//actualControlOption = 0;
					SceneAudio.GetComponent<AudioSource>().SetClipVolume(currVolume);
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				}
				isFirstOcontrolsL = false;
				
				break;
		}

		a = optionsControlR.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				
				// normal Mode
				break;
			case 2:
				// focused mode
				isFirstOcontrolsR = true;
				break;
			case 3:
				// pressed mode
				if (isFirstOcontrolsR)
				{
					if (actualControlOption == 1)
					{
						actualControlOption = 0;

					}
					else
					{
						actualControlOption++;
					}
					SceneAudio.GetComponent<AudioSource>().SetClipVolume(currVolume);
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				}
				//actualControlOption = 1;
				isFirstOcontrolsR = false;
				
				break;
		}
		if (actualControlOption == 0)
        {
            playerManager.GetComponent<InputAction>().SetActionMap(1);

			optionsControl8.GetComponent<UIButton>().text = "Drag";
			optionsControl9.GetComponent<UIButton>().text = "Crl L";
			optionsControl14.GetComponent<UIButton>().text = "A";
			optionsControl15.GetComponent<UIButton>().text = "S";
			optionsControl16.GetComponent<UIButton>().text = "D";
			optionsControl17.GetComponent<UIButton>().text = "F";
			optionsControl18.GetComponent<UIButton>().text = "G";
		}
        else if(actualControlOption == 1)
        {
            playerManager.GetComponent<InputAction>().SetActionMap(0);

            optionsControl9.GetComponent<UIButton>().text = "Q E";
			optionsControl8.GetComponent<UIButton>().text = "WASD";

			optionsControl14.GetComponent<UIButton>().text = "Z";
			optionsControl15.GetComponent<UIButton>().text = "X";
			optionsControl16.GetComponent<UIButton>().text = "C";
			optionsControl17.GetComponent<UIButton>().text = "V";
			optionsControl18.GetComponent<UIButton>().text = "B";
		}
		pos.Set(-sum - 100, y - 230, 36.1f);
		optionsControlText.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 250, y - 230, 36.1f);
		optionsControlText1.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 550, y - 230, 36.1f);
		optionsControlText2.GetComponent<Transform2D>().position2D = pos;


		pos.Set(-sum + 150, y - 220, 36.1f);
		optionsControl1.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 150, y - 280, 36.1f);
		optionsControl2.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 150, y - 340, 36.1f);
		optionsControl3.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 150, y - 410, 36.1f);
		optionsControl4.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 150, y - 480, 36.1f);
		optionsControl5.GetComponent<Transform2D>().position2D = pos;
		optionsControl5.GetComponent<UIButton>().SetTextPosition(-30,-3.2f);

		pos.Set(-sum + 150, y - 540, 36.1f);
		optionsControl6.GetComponent<Transform2D>().position2D = pos;
		optionsControl6.GetComponent<UIButton>().SetTextPosition(-30, -3.2f);

		pos.Set(-sum + 150, y - 585, 36.1f);
		optionsControl7.GetComponent<Transform2D>().position2D = pos;
		optionsControl7.GetComponent<UIButton>().SetTextPosition(-30, -3.2f);

		pos.Set(-sum + 150, y - 640, 36.1f);
		optionsControl8.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 150, y - 710, 36.1f);
		optionsControl9.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 150, y - 780, 36.1f);
		optionsControl10.GetComponent<Transform2D>().position2D = pos;



		pos.Set(-sum + 470, y - 230, 36.1f);
		optionsControl11.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 470, y - 300, 36.1f);
		optionsControl12.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 470, y - 370, 36.1f);
		optionsControl13.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 470, y - 440, 36.1f);
		optionsControl14.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 470, y - 510, 36.1f);
		optionsControl15.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 470, y - 580, 36.1f);
		optionsControl16.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 470, y - 650, 36.1f);
		optionsControl17.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 470, y - 720, 36.1f);
		optionsControl18.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 470, y - 790, 36.1f);
		optionsControl19.GetComponent<Transform2D>().position2D = pos;



		pos.Set(-sum + 750, y - 230, 36.1f);
		optionsControl20.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 750, y - 300, 36.1f);
		optionsControl21.GetComponent<Transform2D>().position2D = pos;

		pos.Set(-sum + 750, y - 370, 36.1f);
		optionsControl22.GetComponent<Transform2D>().position2D = pos;
	}
	void OptionsControlHide()
	{
		optionsControlText.isActive = false;
		optionsControlText1.isActive = false;
		optionsControlText2.isActive = false;

		optionsControl1.isActive = false;
		optionsControl2.isActive = false;
		optionsControl3.isActive = false;
		optionsControl4.isActive = false;
		optionsControl5.isActive = false;
		optionsControl6.isActive = false;
		optionsControl7.isActive = false;
		optionsControl8.isActive = false;
		optionsControl9.isActive = false;
		optionsControl10.isActive = false;
		optionsControl11.isActive = false;		
		optionsControl12.isActive = false;
		optionsControl13.isActive = false;
		optionsControl14.isActive = false;
		optionsControl15.isActive = false;
		optionsControl16.isActive = false;
		optionsControl17.isActive = false;
		optionsControl18.isActive = false;
		optionsControl19.isActive = false;
		optionsControl20.isActive = false;
		optionsControl21.isActive = false;
		optionsControl22.isActive = false;
		optionsControlR.isActive = false;
		optionsControlL.isActive = false;

	}
	void UpdateOptions()
    {
        if (isOptions)
        {
			OptionsControlShow();
			OptionsBackShow();
			SelectedOptionShow();
			OptionsAllButtonsShow();
			
		}
        else
        {
			OptionsControlHide();
			OptionsBackHide();
			OptionsAllButtonsHide();
			SelectedOptionHide();
			
		}
    }
	void SelectedOptionShow()
    {
		if (actualOption != "Screen")
		{
			OptionsScreenHide();
		}
		if (actualOption != "Controls")
		{
			OptionsControlHide();
		}
		if (actualOption != "Sound")
		{
			OptionsSoundHide();
		}


		if (actualOption == "Screen")
		{
			optionsScreenButton.GetComponent<UIButton>().SetButtonState(2);
			OptionsScreenShow();
		}
		else if (actualOption == "Controls")
		{
			optionsControlsButton.GetComponent<UIButton>().SetButtonState(2);
			OptionsControlShow();
		}
		else if (actualOption == "Sound")
		{
			optionsSoundButton.GetComponent<UIButton>().SetButtonState(2);
			OptionsSoundShow();
		}

	}
	void SelectedOptionHide()
	{
		OptionsControlHide();
		OptionsScreenHide();
		OptionsSoundHide();


	}
	void OptionsBackShow()
    {
		float X = (InternalCalls.GetRegionGame().x / 2);
		float Y = (InternalCalls.GetRegionGame().y / 2);
		pos.Set(0.0f, 0.0f, -10.400f);
		optionsBack.GetComponent<Transform2D>().position2D = pos;
		optionsBack.GetComponent<Transform2D>().SetSize(InternalCalls.GetRegionGame());
		optionsBack.isActive = true;
		//////////

		pos.Set(X - 600, 0.0f, -10.400f);
		optionsTransCuad.GetComponent<Transform2D>().position2D = pos;

		bounds.Set(X, (InternalCalls.GetRegionGame().y-300), -10.400f);
		optionsTransCuad.GetComponent<Transform2D>().SetSize(bounds);
		optionsTransCuad.isActive = true;
		//////////

		pos.Set(X - (InternalCalls.GetRegionGame().x / 3), Y - 100, -10.400f);
		optionsTitle.GetComponent<Transform2D>().position2D = pos;
		optionsTitle.isActive = true;


	}
	void OptionsBackHide()
	{
		optionsBack.isActive = false;

		//////////
		optionsTransCuad.isActive = false;

		//////////
		optionsTitle.isActive = false;
	
	}
	void OptionsAllButtonsShow()
    {
		optionsBackButton.isActive = true;
		optionsBackImage.isActive = true;

		optionsScreenButton.isActive = true;
		optionsScreenImage.isActive = true;

		optionsSoundButton.isActive = true;
		optionsSoundImage.isActive = true;

		optionsControlsButton.isActive = true;
		optionsControlsImage.isActive = true;

		////////////////////////////////////////////////
		/// CONTROLS BUTTON
		////////////////////////////////////////////////
		float X = (InternalCalls.GetRegionGame().x / 2);
		float sum = (lastWindowW - X) / 2;
		int a = optionsControlsButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstOControlsB )
				{
					
					isFirstOControlsB = false;
					
				}
				pos.Set(X - 1217 + sum, (InternalCalls.GetRegionGame().y / 2) - 380, -10.4f);
				optionsControlsButton.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(275, 58.5f, 0);
				optionsControlsButton.GetComponent<Transform2D>().SetSize(bounds);

				pos.Set(X - 1217 + sum, (InternalCalls.GetRegionGame().y / 2) - 380, -10.4f);
				optionsControlsImage.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(273.5f, 48, 0);
				optionsControlsImage.GetComponent<Transform2D>().SetSize(bounds);
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstOControlsB)
				{					
					isFirstOControlsB = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}
				pos.Set(X - 1237 + sum, (InternalCalls.GetRegionGame().y / 2) - 380, -10.4f);
				optionsControlsButton.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(275 + 40, 58.5f, 0);
				optionsControlsButton.GetComponent<Transform2D>().SetSize(bounds);

				pos.Set(X - 1237 + sum, (InternalCalls.GetRegionGame().y / 2) - 380, -10.4f);
				optionsControlsImage.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(273.5f + 40, 48, 0);
				optionsControlsImage.GetComponent<Transform2D>().SetSize(bounds);
				break;
			case 3:
				// pressed mode
				actualOption = "Controls";

				SceneAudio.GetComponent<AudioSource>().SetClipVolume(currVolume);
				SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				break;
		}
		////////////////////////////////////////////////
		/// SOUND BUTTON
		////////////////////////////////////////////////

		a = optionsSoundButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstOSoundB )
				{
					
					isFirstOSoundB = false;
					
				}
				pos.Set(X - 1217 + sum, (InternalCalls.GetRegionGame().y / 2) - 280, -10.4f);
				optionsSoundButton.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(275, 58.5f, 0);
				optionsSoundButton.GetComponent<Transform2D>().SetSize(bounds);

				pos.Set(X - 1217 + sum, (InternalCalls.GetRegionGame().y / 2) - 280, -10.4f);
				optionsSoundImage.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(273.5f, 48, 0);
				optionsSoundImage.GetComponent<Transform2D>().SetSize(bounds);
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstOSoundB)
				{					
					isFirstOSoundB = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}
				pos.Set(X - 1237 + sum, (InternalCalls.GetRegionGame().y / 2) - 280, -10.4f);
				optionsSoundButton.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(275 + 40, 58.5f, 0);
				optionsSoundButton.GetComponent<Transform2D>().SetSize(bounds);

				pos.Set(X - 1237 + sum, (InternalCalls.GetRegionGame().y / 2) - 280, -10.4f);
				optionsSoundImage.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(273.5f + 40, 48, 0);
				optionsSoundImage.GetComponent<Transform2D>().SetSize(bounds);
				break;
			case 3:
				// pressed mode
				actualOption = "Sound";

				SceneAudio.GetComponent<AudioSource>().SetClipVolume(currVolume);
				SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				break;
		}

		////////////////////////////////////////////////
		/// SCREEN BUTTON
		////////////////////////////////////////////////

		a = optionsScreenButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstOScreenB)
				{
					
					isFirstOScreenB = false;
					
                }
				pos.Set(X - 1217 + sum, (InternalCalls.GetRegionGame().y / 2) - 180, -10.4f);
				optionsScreenButton.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(275, 58.5f, 0);
				optionsScreenButton.GetComponent<Transform2D>().SetSize(bounds);

				pos.Set(X - 1217 + sum, (InternalCalls.GetRegionGame().y / 2) - 180, -10.4f);
				optionsScreenImage.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(273.5f, 48, 0);
				optionsScreenImage.GetComponent<Transform2D>().SetSize(bounds);
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstOScreenB)
				{				
					isFirstOScreenB = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}
				pos.Set(X - 1237 + sum, (InternalCalls.GetRegionGame().y / 2) - 180, -10.4f);
				optionsScreenButton.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(275 + 40, 58.5f, 0);
				optionsScreenButton.GetComponent<Transform2D>().SetSize(bounds);

				pos.Set(X - 1237 + sum, (InternalCalls.GetRegionGame().y / 2) - 180, -10.4f);
				optionsScreenImage.GetComponent<Transform2D>().position2D = pos;
				bounds.Set(273.5f + 40, 48, 0);
				optionsScreenImage.GetComponent<Transform2D>().SetSize(bounds);
				break;
			case 3:
				// pressed mode
				actualOption = "Screen";

				SceneAudio.GetComponent<AudioSource>().SetClipVolume(currVolume);
				SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				break;
		}


		////////////////////////////////////////////////
		/// BACK BUTTON
		////////////////////////////////////////////////
		a = optionsBackButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstOBackB)
				{

					isFirstOBackB = false;
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 80, -(InternalCalls.GetRegionGame().y / 2) + 100, 36.1f);

					optionsBackButton.GetComponent<Transform2D>().position2D = pos;


					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 80, -(InternalCalls.GetRegionGame().y / 2) + 100, 36.1f);
					optionsBackImage.GetComponent<Transform2D>().position2D = pos;
				}
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstOBackB)
				{
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 120, -(InternalCalls.GetRegionGame().y / 2) + 100, 36.1f);
					optionsBackButton.GetComponent<Transform2D>().position2D = pos;


					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 120, -(InternalCalls.GetRegionGame().y / 2) + 100, 36.1f);
					optionsBackImage.GetComponent<Transform2D>().position2D = pos;

					isFirstOBackB = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}

				break;
			case 3:
				// pressed mode
				isSowing = true;
				isOptions = false;


				isFirstOBackB = true;
				isFirstOScreenB = true;
				isFirstOSoundB = true;
				isFirstOControlsB = true;
				
				//Quitar menu de pausa
				SceneAudio.GetComponent<AudioSource>().SetClipVolume(currVolume);
				SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				break;
		}
	}
	void OptionsAllButtonsHide()
	{
		optionsBackButton.isActive = false;
		optionsBackImage.isActive = false;

		optionsScreenButton.isActive = false;
		optionsScreenImage.isActive = false;

		optionsSoundButton.isActive = false;
		optionsSoundImage.isActive = false;

		optionsControlsButton.isActive = false;
		optionsControlsImage.isActive = false;
	}
	void OptionsScreenShow()
	{
		float y = (InternalCalls.GetRegionGame().y / 2);
		float x = (InternalCalls.GetRegionGame().x / 2);
		float sum = (lastWindowW - x);
		optionsScreenFSCH.isActive = true;
		optionsScreenVSCH.isActive = true;
		optionsScreenSDCH.isActive = true;
		optionsSreenText.isActive = true;
		optionsLanguaje.isActive = true;
		////////////////////////////////////////////////
		/// FULLSCREEN CHECKBOX
		////////////////////////////////////////////////

		pos.Set(x - 650, y - 295, 36.1f);
		optionsScreenFSCH.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(40, 40, 0);
		optionsScreenFSCH.GetComponent<Transform2D>().SetSize(bounds);
		if (optionsScreenFSCH.GetComponent<UICheckbox>().GetIsChecked())
			InternalCalls.SetFullScreen(true);        
		else
			InternalCalls.SetFullScreen(false);		

		////////////////////////////////////////////////
		/// V-SYNC CHECKBOX
		////////////////////////////////////////////////

		pos.Set(x - 650, y - 365, 36.1f);
		optionsScreenVSCH.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(40, 40, 0);
		optionsScreenVSCH.GetComponent<Transform2D>().SetSize(bounds);
		if (optionsScreenVSCH.GetComponent<UICheckbox>().GetIsChecked())
		{
			InternalCalls.SetVSync(true);
        }
        else
        {
			InternalCalls.SetVSync(false);
		}
		////////////////////////////////////////////////
		/// SHADOWS CHECKBOX
		////////////////////////////////////////////////

		pos.Set(x - 650, y - 435, 36.1f);
		optionsScreenSDCH.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(40, 40, 0);
		optionsScreenSDCH.GetComponent<Transform2D>().SetSize(bounds);
		
		if (optionsScreenSDCH.GetComponent<UICheckbox>().GetIsChecked())
		{
			Light.shadowsEnabled = true;
        }
        else
        {
			Light.shadowsEnabled = false;
		}

		pos.Set(-sum + 20, y-230, 36.1f);
		optionsSreenText.GetComponent<Transform2D>().position2D = pos;

		////////////////////////////////////////////////
		/// DROP DOWN
		////////////////////////////////////////////////

		pos.Set(x - 550, y - 500, 36.1f);
		optionsLanguaje.GetComponent<Transform2D>().position2D = pos;
		/*
        if (optionsLanguaje.GetComponent<UIDropDown>().GetDropDownButtonChange())
        {
			int a = optionsLanguaje.GetComponent<UIDropDown>().GetDropDownSelected();
			optionsLanguaje.GetComponent<UIDropDown>().SetDropDownLenguage(a);
		}
		*/

	}
	void OptionsScreenHide()
	{
		optionsScreenFSCH.isActive = false;
		optionsScreenVSCH.isActive = false;
		optionsScreenSDCH.isActive = false;
		optionsSreenText.isActive = false;
		optionsLanguaje.isActive = false;
	}
	void OptionsSoundShow()
	{
		float y = (InternalCalls.GetRegionGame().y / 2);
		float x = (InternalCalls.GetRegionGame().x / 2);
		float sum = (lastWindowW - x);
		optionsSoundText.isActive = true;

        optionsVoicesSound.isActive = true;
        optionsFXcSound.isActive = true;
        optionsMusicSound.isActive = true;
        optionsGeneralSound.isActive = true;


        pos.Set(-sum + 20, y-230 , 36.1f);
		optionsSoundText.GetComponent<Transform2D>().position2D = pos;

		float VoicesSound;
		float FXcSound;
		float MusicSound;

		
		pos.Set(x-650, y - 260, 36.1f);
        optionsVoicesSound.GetComponent<Transform2D>().position2D = pos;
		VoicesSound = 100* optionsVoicesSound.GetComponent<UISlider>().GetSliderActualValue();
		
		SceneAudio.GetComponent<AudioSource>().SetClipVolume(VoicesSound);

		

		pos.Set(x - 650, y - 350, 36.1f);
        optionsFXcSound.GetComponent<Transform2D>().position2D = pos;
		FXcSound = 100 * optionsVoicesSound.GetComponent<UISlider>().GetSliderActualValue();



		pos.Set(x - 650, y - 440, 36.1f);
        optionsMusicSound.GetComponent<Transform2D>().position2D = pos;
		MusicSound = 100 * optionsVoicesSound.GetComponent<UISlider>().GetSliderActualValue();



		pos.Set(x - 650, y - 530, 36.1f);
        optionsGeneralSound.GetComponent<Transform2D>().position2D = pos;
		currVolume = 100 * optionsVoicesSound.GetComponent<UISlider>().GetSliderActualValue();


    }
	void OptionsSoundHide()
	{
		optionsSoundText.isActive = false;
        optionsVoicesSound.isActive = false;
        optionsFXcSound.isActive = false;
        optionsMusicSound.isActive = false;
        optionsGeneralSound.isActive = false;
    }
	//////////////PAUSE//////////////
	
	void UpdateMenu()
    {
		if (isSowing)
		{
			ImageShow();
			RestartButtonShow();
			ResumeButtonShow();
			MainMenuButtonShow();
			OptionsButtonShow();
			if (isSowing == false)
			{
				ImageHide();
				RestartButtonHide();
				ResumeButtonHide();
				MainMenuButtonHide();
				OptionsButtonHide();
            }
		}

		if (Input.GetKey(KeyCode.ESCAPE) == KeyState.KEY_DOWN && !isOptions)
		{
			if (isSowing)
			{
				isSowing = false;
				ImageHide();
				RestartButtonHide();
				ResumeButtonHide();
				MainMenuButtonHide();
				OptionsButtonHide();
                if (dialogue.GetInDialogue()) { dialogue.ContinueDialogue(); }

                SceneAudio.GetComponent<AudioSource>().SetClipVolume(currVolume);

                // Why is it not necessary to put "<Level_2>" and "<Level_3>"?, I don't know
                if (GameObject.Find("LevelManager").GetComponent<Level_1>() != null) 
					GameObject.Find("LevelManager").GetComponent<Level_1>().runGame = true;
			}
            else
			{
                if (dialogue.GetInDialogue()) { dialogue.DisableDialogue(); }
                currVolume = SceneAudio.GetComponent<AudioSource>().GetClipVolume();
				SceneAudio.GetComponent<AudioSource>().SetClipVolume(15.0f);
				isSowing = true;

				if (GameObject.Find("LevelManager").GetComponent<Level_1>() != null)
					GameObject.Find("LevelManager").GetComponent<Level_1>().runGame = false;
			}
		}
	}
	void SetAllPositions()
	{
		//GET POSITION
		if (isSowing||isOptions)
        {
			abilityLeters.isActive = false;
			CharacterPhotoBord.isActive = false;
			AbilityBord.isActive = false;
			UICharPhoto.isActive = false;
			UIAbilityArray.isActive = false;
			UICharBor1.isActive = false;
			UICharBor2.isActive = false;
			UICharBor3.isActive = false;
			UICharacterName.isActive = false;
			UISelector.isActive = false;
			UIPaulImage.isActive = false;
			UIChaniImage.isActive = false;
			UIStilgarImage.isActive = false;
		}
        else
        {
			abilityLeters.isActive = true;
			CharacterPhotoBord.isActive = true;
			AbilityBord.isActive = true;
			UICharPhoto.isActive = true;
			UIAbilityArray.isActive = true;
			UICharBor1.isActive = true;
			UICharBor2.isActive = true;
			UICharBor3.isActive = true;
			UICharacterName.isActive = true;
			UISelector.isActive = true;
			UIPaulImage.isActive = true;
			UIChaniImage.isActive = true;
			UIStilgarImage.isActive = true;
		}

		if(isOptions)
			GameObject.Find("UI Counter").isActive = false;
		else
			GameObject.Find("UI Counter").isActive = true;

		float y = -(InternalCalls.GetRegionGame().y / 2) + 100.0f; 
		float x = -(InternalCalls.GetRegionGame().x / 2);

		//UPDATE ABILITYS BUTTONS
		
		UpdateAbilityButton(x,y);

		//UPDATE THE REST ELEMENTS
		pos.Set(-40, y-50, -10.400f);
		bounds.Set(280, 30, 0);
		abilityLeters.GetComponent<Transform2D>().position2D = pos;
		abilityLeters.GetComponent<Transform2D>().SetSize(bounds);

		pos.Set(x+150, y+30, -10.400f);
		CharacterPhotoBord.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(214, 214f, 0);
		CharacterPhotoBord.GetComponent<Transform2D>().SetSize(bounds);


		pos.Set(0.0f, y-30, -10.400f);
		AbilityBord.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(650, 90f, 0);
		AbilityBord.GetComponent<Transform2D>().SetSize(bounds);

		
		
		
		
		pos.Set(x+150, y+35 , -10.400f);
		UICharPhoto.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(160, 160f, 0);
		UICharPhoto.GetComponent<Transform2D>().SetSize(bounds);

		pos.Set(0, y, -10.400f);
		UIAbilityArray.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(400, 45, 0);
		UIAbilityArray.GetComponent<Transform2D>().SetSize(bounds);

		pos.Set(x+300, y, -10.400f);
		UICharBor1.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(60, 60, 0);
		UICharBor1.GetComponent<Transform2D>().SetSize(bounds);

		pos.Set(x + 380, y, -10.400f);
		UICharBor2.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(60, 60, 0);
		UICharBor2.GetComponent<Transform2D>().SetSize(bounds);

		pos.Set(x + 460, y, -10.400f);
		UICharBor3.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(60, 60, 0);
		UICharBor3.GetComponent<Transform2D>().SetSize(bounds);

		if (selectedPlayer.name != lastPlayerSelected)
		{
			changePlayer(x, y);
			lastPlayerSelected = selectedPlayer.name;
        }
        else
        {
			if (selectedPlayer.name == "Player")
			{				
				pos.Set(x + 300, y + 30, -10.400f);				
			}
			else if (selectedPlayer.name == "Player_2")
			{				
				pos.Set(x + 380, y + 30, -10.400f);				
			}
			else
			{				
				pos.Set(x + 460, y + 30, -10.400f);				
			}
		}

		UISelector.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(20, 20, 0);
		UISelector.GetComponent<Transform2D>().SetSize(bounds);

		pos.Set(x + 120, y - 60, -10.400f);
		UICharacterName.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(214, 214f, 0);
		UICharacterName.GetComponent<Transform2D>().SetSize(bounds);
        
		pos.Set(x+300 , y, -10.400f);
		UIPaulImage.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(60, 60, 0);
		UIPaulImage.GetComponent<Transform2D>().SetSize(bounds);

		pos.Set(x+380, y , -10.400f);
		UIChaniImage.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(60, 60, 0);
		UIChaniImage.GetComponent<Transform2D>().SetSize(bounds);

		pos.Set(x+460, y , -10.400f);
		UIStilgarImage.GetComponent<Transform2D>().position2D = pos;
		bounds.Set(60, 60, 0);
		UIStilgarImage.GetComponent<Transform2D>().SetSize(bounds);
	}
	void changePlayer(float x, float y)
    {
		if (selectedPlayer.name == "Player")
		{
			UICharPhoto.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_paul_portrait.png");
			pos.Set(x + 150, y + 30, -10.400f);
			UICharacterName.GetComponent<UIText>().text = "Paul";
		}
		else if (selectedPlayer.name == "Player_2")
		{
			UICharPhoto.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_chani_portrait.png");
			pos.Set(x + 230, y + 30, -10.400f);
			UICharacterName.GetComponent<UIText>().text = "Chani";
		}
		else if (selectedPlayer.name == "Player_3")
		{
			UICharPhoto.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_stilgar_portrait.png");
			pos.Set(x + 310, y + 30, -10.400f);
			UICharacterName.GetComponent<UIText>().text = "Stilgar";
		}
	}
	void ImageShow()
	{
		Image.isActive = true ;
		pos.Set(0.0f, 0.0f, -10.400f);
		Image.GetComponent<Transform2D>().position2D = pos;
		Image.GetComponent<Transform2D>().SetSize(InternalCalls.GetRegionGame());

	}

	void ImageHide()
	{
		Image.isActive = false;
	}
	void RestartButtonHide()
	{
		restart.isActive = false;
		restartImage.isActive = false;
	}

	void RestartButtonShow()
	{
		restart.isActive = true;
		restartImage.isActive = true;
		int a = restart.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstR)
				{
					isFirstR = false;
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, restart.GetComponent<Transform2D>().position2D.y, 36.1f);

					restart.GetComponent<Transform2D>().position2D = pos;
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, restartImage.GetComponent<Transform2D>().position2D.y, 36.1f);

					restartImage.GetComponent<Transform2D>().position2D = pos;
				}

				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstR)
				{
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					isFirstR = true;
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 200, restart.GetComponent<Transform2D>().position2D.y, 36.1f);

					restart.GetComponent<Transform2D>().position2D = pos;

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 200, restartImage.GetComponent<Transform2D>().position2D.y, 36.1f);

					restartImage.GetComponent<Transform2D>().position2D = pos;
					//poner sonido
				}

				break;
			case 3:
				// pressed mode
				SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				if (SceneManager.currentSceneName == "build")
				{
					SceneManager.LoadScene("build");
				}
				else if (SceneManager.currentSceneName == "build2")
				{
					SceneManager.LoadScene("build2");
				}
				else if (SceneManager.currentSceneName == "build3")
				{
					SceneManager.LoadScene("build3");
				}
				//cambiar de escena
				break;
		}
	}
	void ResumeButtonShow()
	{
		Resume.isActive = true;
		ResumeImage.isActive = true;
		int a = Resume.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
                if (isFirstS)
                {
					isFirstS = false;

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, Resume.GetComponent<Transform2D>().position2D.y, 36.1f);
					Resume.GetComponent<Transform2D>().position2D = pos;

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, ResumeImage.GetComponent<Transform2D>().position2D.y, 36.1f);
					ResumeImage.GetComponent<Transform2D>().position2D = pos;
				}
				

				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstS)
				{
					isFirstS = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 200, Resume.GetComponent<Transform2D>().position2D.y, 36.1f);
					Resume.GetComponent<Transform2D>().position2D = pos;

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 200, ResumeImage.GetComponent<Transform2D>().position2D.y, 36.1f);
					ResumeImage.GetComponent<Transform2D>().position2D = pos;
					//poner sonido
				}

				break;
			case 3:
				// pressed mode
				isSowing = false;
				if (!dialogue.GetEndDialogue()) { dialogue.ContinueDialogue(); }
				//Quitar menu de pausa
				SceneAudio.GetComponent<AudioSource>().SetClipVolume(currVolume);
				SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				if (GameObject.Find("LevelManager").GetComponent<Level_1>() != null)
					GameObject.Find("LevelManager").GetComponent<Level_1>().runGame = true;
				break;
		}
	}
	void ResumeButtonHide()
	{
		Resume.isActive = false;
		ResumeImage.isActive = false;
	}

	void MainMenuButtonShow()
	{
		MainM.isActive=true;
		MainMImage.isActive = true;
		int a = MainM.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
                if (isFirstE)
                {
					isFirstE = false;
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, MainM.GetComponent<Transform2D>().position2D.y, 36.1f);
					MainM.GetComponent<Transform2D>().position2D = pos;

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, MainMImage.GetComponent<Transform2D>().position2D.y, 36.1f);
					MainMImage.GetComponent<Transform2D>().position2D = pos;
				}
				
				
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstE)
				{
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					isFirstE = true;
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 200, MainM.GetComponent<Transform2D>().position2D.y, 36.1f);
					MainM.GetComponent<Transform2D>().position2D = pos;

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 200, MainMImage.GetComponent<Transform2D>().position2D.y, 36.1f);
					MainMImage.GetComponent<Transform2D>().position2D = pos;
					//poner sonido
				}

				break;
			case 3:
				// pressed mode
				SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				SceneManager.LoadScene("MainMenu");
				//cambiar de escena
				break;
		}
	}
	void MainMenuButtonHide()
	{
		MainM.isActive = false;
		MainMImage.isActive = false;
	}


	void OptionsButtonShow()
	{
		Opt.isActive = true;
		OptImage.isActive = true;
		int a = Opt.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
                if (isFirstO)
                {
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, Opt.GetComponent<Transform2D>().position2D.y, 36.1f);
					Opt.GetComponent<Transform2D>().position2D = pos;

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 160, OptImage.GetComponent<Transform2D>().position2D.y, 36.1f);
					OptImage.GetComponent<Transform2D>().position2D = pos;
					isFirstO = false;
				} 
				

				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstO)
				{
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					isFirstO = true;
					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 200, Opt.GetComponent<Transform2D>().position2D.y, 36.1f);
					Opt.GetComponent<Transform2D>().position2D = pos;

					pos.Set(-(InternalCalls.GetRegionGame().x / 2) + 200, OptImage.GetComponent<Transform2D>().position2D.y, 36.1f);
					OptImage.GetComponent<Transform2D>().position2D = pos;
					//poner sonido
				}

				break;
			case 3:
				// pressed mode
				SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
				isOptions = true;
				isSowing = false;
				//cambiar de escena
				break;
		}
		// Options button logic
	}
	void OptionsButtonHide()
	{
		Opt.isActive = false;
		OptImage.isActive = false;
	}
	void UpdateAbilityButton(float x, float y)
	{
		CharFocusedImage.isActive = false;
		CharFocusedText.isActive = false;
		AbilityImageApmliate.isActive = false;
		if (isSowing || isOptions)
		{
			Ability1.isActive = false;
			Ability2.isActive = false;
			Ability3.isActive = false;
			Ability4.isActive = false;
			Ability5.isActive = false;

		}
		else
		{
			Ability1.isActive = true;
			Ability2.isActive = true;
			Ability3.isActive = true;
			Ability4.isActive = true;
			Ability5.isActive = true;
		}
		if (selectedPlayer.name == "Player")//paul
		{
			Ability1.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_paul_crysknife.png");
			Ability2.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_paul_voice.png");
			Ability3.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_paul_throwing_knife.png");
			Ability4.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_paul_throw_stone.png");
			Ability5.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_paul_portrait.png");
		}
		else if (selectedPlayer.name == "Player_2")//chani
		{
			Ability1.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_chani_crysknife.png");
			Ability2.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_chani_camouflage.png");
			Ability3.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_chani_hunter_seeker.png");
			Ability4.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_chani_spice_grenade.png");
			Ability5.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_chani_portrait.png");
		}
		else if (selectedPlayer.name == "Player_3")//stilgar
		{
			Ability1.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_stilgar_sword.png");
			Ability2.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_stilgar_stunner.png");
			Ability3.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_stilgar_trap.png");
			Ability4.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_stilgar_whistle.png");
			Ability5.GetComponent<UIButton>().LoadButtonTexture("Assets/Resources/UI/ui_stilgar_portrait.png");
		}
		bounds.Set(210, 310, 0);
		CharFocusedImage.GetComponent<Transform2D>().SetSize(bounds);

		////////////////////////////////////////////////
		/// ABILITY1 BUTTON
		////////////////////////////////////////////////
		int a = Ability1.GetComponent<UIButton>().GetButtonState();

		pos.Set(-185, y, -10.400f);
		Ability1.GetComponent<Transform2D>().position2D = pos;

		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstA1)
				{

					isFirstA1 = false;

				}
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstA1)
				{
					isFirstA1 = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}
				CharFocusedImage.isActive = true;
				CharFocusedText.isActive = true;
				AbilityImageApmliate.isActive = true;
				pos.Set(-270, y + 200, -10.400f);

				CharFocusedText.GetComponent<Transform2D>().position2D = pos;
				pos.Set(-180, y + 200, -10.400f);
				CharFocusedImage.GetComponent<Transform2D>().position2D = pos;

				pos.Set(-180, y + 300, -10.400f);
				AbilityImageApmliate.GetComponent<Transform2D>().position2D = pos;
				
				if (selectedPlayer.name == "Player")//paul
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_paul_crysknife.png");

					CharFocusedText.GetComponent<UIText>().text = "Kill an enemy\nat a melee\nrange. Drones\ncan�t be killed.";
				}
				else if (selectedPlayer.name == "Player_2")//chani
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_chani_crysknife.png");
					CharFocusedText.GetComponent<UIText>().text = "Kill an enemy\nat a melee\nrange. Drones\ncan�t be\nkilled.";
				}
				else if (selectedPlayer.name == "Player_3")//stilgar
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_stilgar_sword.png");
					CharFocusedText.GetComponent<UIText>().text = "Kill enemies with\na slash in front\nof you.";
				}
				break;
			case 3:
				// pressed mode

				break;
		}

		////////////////////////////////////////////////
		/// ABILITY2 BUTTON
		////////////////////////////////////////////////

		a = Ability2.GetComponent<UIButton>().GetButtonState();
		pos.Set(-85, y, -10.400f);
		Ability2.GetComponent<Transform2D>().position2D = pos;
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstA2)
				{

					isFirstA2 = false;

				}
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstA2)
				{
					isFirstA2 = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}
				CharFocusedImage.isActive = true;
				CharFocusedText.isActive = true;
				AbilityImageApmliate.isActive = true;
				pos.Set(-170, y + 200, -10.400f);
				CharFocusedText.GetComponent<Transform2D>().position2D = pos;
				pos.Set(-85, y + 200, -10.400f);
				CharFocusedImage.GetComponent<Transform2D>().position2D = pos;

				pos.Set(-85, y + 300, -10.400f);
				AbilityImageApmliate.GetComponent<Transform2D>().position2D = pos;
				if (selectedPlayer.name == "Player")//paul
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_paul_voice.png");
					CharFocusedText.GetComponent<UIText>().text = "Mind control\nan enemy.\nDrones can�t\nbe affected.";
				}
				else if (selectedPlayer.name == "Player_2")//chani
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_chani_camouflage.png");
					CharFocusedText.GetComponent<UIText>().text = "Activate it\nto disappear\nfrom enemy\nvision.\nDrones can\ndetect you.";
				}
				else if (selectedPlayer.name == "Player_3")//stilgar
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_stilgar_stunner.png");
					CharFocusedText.GetComponent<UIText>().text = "Fire weapon\nthat can\npierce\nenemie�s\nshields.";
				}

				break;
			case 3:
				// pressed mode

				break;
		}

		////////////////////////////////////////////////
		/// ABILITY3 BUTTON
		////////////////////////////////////////////////

		a = Ability3.GetComponent<UIButton>().GetButtonState();
		pos.Set(0, y, -10.400f);
		Ability3.GetComponent<Transform2D>().position2D = pos;
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstA3)
				{

					isFirstA3 = false;

				}
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstA3)
				{
					isFirstA3 = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}
				CharFocusedImage.isActive = true;
				CharFocusedText.isActive = true;
				AbilityImageApmliate.isActive = true;
				pos.Set(-85, y + 200, -10.400f);
				CharFocusedText.GetComponent<Transform2D>().position2D = pos;
				pos.Set(0, y + 200, -10.400f);
				CharFocusedImage.GetComponent<Transform2D>().position2D = pos;

				pos.Set(0, y + 300, -10.400f);
				AbilityImageApmliate.GetComponent<Transform2D>().position2D = pos;
				if (selectedPlayer.name == "Player")//paul
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_paul_throwing_knife.png");
					CharFocusedText.GetComponent<UIText>().text = "Throw a knife\nto an enemy\nwithin range.\nIt doesn�t\naffect\nshielded\nenemies.";
				}
				else if (selectedPlayer.name == "Player_2")//chani
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_chani_hunter_seeker.png");
					CharFocusedText.GetComponent<UIText>().text = "Send a little\nflying drone\nto kill an\nenemy. Drones\ncan�t be\nkilled.";
				}
				else if (selectedPlayer.name == "Player_3")//stilgar
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_stilgar_trap.png");
					CharFocusedText.GetComponent<UIText>().text = "Set a trap to\nstun enemies\nand destroy\ndrones.";
				}
				break;
			case 3:
				// pressed mode

				break;
		}

		////////////////////////////////////////////////
		/// ABILITY4 BUTTON
		////////////////////////////////////////////////

		a = Ability4.GetComponent<UIButton>().GetButtonState();
		pos.Set(85, y, -10.400f);
		Ability4.GetComponent<Transform2D>().position2D = pos;
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstA4)
				{

					isFirstA4 = false;

				}
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstA4)
				{
					isFirstA4 = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}
				CharFocusedImage.isActive = true;
				CharFocusedText.isActive = true;
				AbilityImageApmliate.isActive = true;
				pos.Set(0, y + 200, -10.400f);
				CharFocusedText.GetComponent<Transform2D>().position2D = pos;
				pos.Set(85, y + 200, -10.400f);
				CharFocusedImage.GetComponent<Transform2D>().position2D = pos;

				pos.Set(85, y + 300, -10.400f);
				AbilityImageApmliate.GetComponent<Transform2D>().position2D = pos;
				if (selectedPlayer.name == "Player")//paul
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_paul_throw_stone.png");
					CharFocusedText.GetComponent<UIText>().text = "Throw a stone\nto make noise\nwithin the\narea.";
				}
				else if (selectedPlayer.name == "Player_2")//chani
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_chani_spice_grenade.png");
					CharFocusedText.GetComponent<UIText>().text = "Throw a grenade\nthat stuns\nenemies in an\narea. It\ndoesn�t affect\nshielded enemies\nnor drones.";
				}
				else if (selectedPlayer.name == "Player_3")//stilgar
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_stilgar_whistle.png");
					CharFocusedText.GetComponent<UIText>().text = "Whistle that\nproduces\nsound around\nthe player to\nattract enemies.";
				}
				break;
			case 3:
				// pressed mode

				break;
		}

		////////////////////////////////////////////////
		/// ABILITY5 BUTTON
		////////////////////////////////////////////////

		a = Ability5.GetComponent<UIButton>().GetButtonState();
		pos.Set(170, y, -10.400f);
		Ability5.GetComponent<Transform2D>().position2D = pos;
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstA5)
				{

					isFirstA5 = false;

				}
				// normal Mode
				break;
			case 2:
				// focused mode
				if (!isFirstA5)
				{
					isFirstA5 = true;
					SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
					//poner sonido
				}
				CharFocusedImage.isActive = true;
				CharFocusedText.isActive = true;
				AbilityImageApmliate.isActive = true;
				pos.Set(80, y + 200, -10.400f);
				CharFocusedText.GetComponent<Transform2D>().position2D = pos;

				pos.Set(170, y + 200, -10.400f);
				CharFocusedImage.GetComponent<Transform2D>().position2D = pos;

				pos.Set(170, y + 300, -10.400f);
				AbilityImageApmliate.GetComponent<Transform2D>().position2D = pos;
				if (selectedPlayer.name == "Player")//paul
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_paul_portrait.png");
					CharFocusedText.GetComponent<UIText>().text = "Coming Soon";
				}
				else if (selectedPlayer.name == "Player_2")//chani
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_chani_portrait.png");
					CharFocusedText.GetComponent<UIText>().text = "Coming Soon";
				}
				else if (selectedPlayer.name == "Player_3")//stilgar
				{
					AbilityImageApmliate.GetComponent<UIImage>().LoadTexture("Assets/Resources/UI/ui_stilgar_portrait.png");
					CharFocusedText.GetComponent<UIText>().text = "Coming Soon";
				}
				break;
			case 3:
				// pressed mode

				break;
		}
	}


}

