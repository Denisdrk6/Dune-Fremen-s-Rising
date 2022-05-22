using System;
using RagnarEngine;

public class mainMenuBackScreen : RagnarComponent
{
	string actualOption = "Screen";
	Vector3 pos;
	Vector3 bounds;
	float currVolume = 0.0f;
	bool isFirstS = true;
	bool isFirstE = true;
	bool isFirstCre = true;
	bool isFirstCont = true;
	bool isFirstOp = true;
	GameObject SceneAudio;
	GameObject continueB;
	GameObject start;
	GameObject options;
	GameObject exit;
	GameObject credits;

	GameObject continueImage;
	GameObject startImage;
	GameObject optionsImage;
	GameObject exitImage;
	GameObject creditsImage;

	GameObject back;

    //////////////OPTIONS//////////////
    bool isOptions = false;
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
	int actualControlOption = 0;

	GameObject[] players;

	public void Start()
	{
		players = new GameObject[3];
		players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var item in players)
        {
			item.GetComponent<Animation>().PlayAnimation("Idle");
        }
		GameObject.Find("stilgar_sword").GetComponent<Animation>().PlayAnimation("Idle");

		pos = new Vector3(0.0f, 0.0f, 0.0f);
        bounds = new Vector3(0.0f, 0.0f, 0.0f);
        SceneAudio = GameObject.Find("AudioMainMenu");
        SceneAudio.GetComponent<AudioSource>().PlayClip("MUSICPLAY");
        SceneAudio.GetComponent<AudioSource>().SetState("MUSIC", "MAINMENU");
        continueB = GameObject.Find("Button Continue");
		start = GameObject.Find("Button Start");
		options = GameObject.Find("Button Options");
		exit = GameObject.Find("Button Exit");
		credits = GameObject.Find("Button Credits");



		continueImage = GameObject.Find("Image Continue");
		startImage = GameObject.Find("Image Start");
		optionsImage = GameObject.Find("Image Options");
		exitImage = GameObject.Find("Image Exit");
		creditsImage = GameObject.Find("Image Credits");



		back = GameObject.Find("Background");


		//continueB.GetComponent<UIButton>().SetButtonState(0);
		if (!SaveSystem.CheckExistingFile("Library/SavedGame/Scenes/SceneSaved.ragnar"))
        {
			continueB.GetComponent<UIButton>().SetVisualDisabled(true);
			continueB.GetComponent<UIButton>().SetButtonTextColor(121, 121, 121);
			continueB.GetComponent<UIButton>().SetButtonGeneralColor(121, 121, 121);
		}
		//Library / SavedGame / Scenes / SceneSaved.ragnar

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
		lastWindowW= (InternalCalls.GetRegionGame().x / 2);

        GameData load = SaveSystem.LoadGameConfig();
        if (load != null)
        {
            LoadOptions(load);
        }

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
    }

    void LoadOptions(GameData load)
    {
        optionsScreenFSCH.GetComponent<UICheckbox>().SetCheckboxState(load.fullScreen);
        InternalCalls.SetFullScreen(load.fullScreen);
        optionsScreenVSCH.GetComponent<UICheckbox>().SetCheckboxState(load.vsync);
        optionsScreenSDCH.GetComponent<UICheckbox>().SetCheckboxState(load.shadowsEnabled);
        Light.shadowsEnabled = load.shadowsEnabled;

        optionsLanguaje.GetComponent<UIDropDown>().SetDropDownLenguage(load.language); 
        optionsLanguaje.GetComponent<UIDropDown>().SetSelected(load.language);

    }

    void SaveOptions()
    {
        GameData ej = new GameData(
            optionsScreenVSCH.GetComponent<UICheckbox>().GetIsChecked(),
            optionsScreenSDCH.GetComponent<UICheckbox>().GetIsChecked(),
            optionsScreenFSCH.GetComponent<UICheckbox>().GetIsChecked(),
            optionsLanguaje.GetComponent<UIDropDown>().GetLenguaje());
        SaveSystem.SaveGameConfig(ej);
    }

    public void Update()
	{
		// Button options
		OptionsButtonAction();
		// Button start
		PlayButtonAction();
		// Button exit
		ExitButtonAction();
		// Button credits
		CreditsButtonAction();
		// Button options
		ContinueButtonAction();		
		// Image back
		

		updateOptions();
		BackgroundImageAction();
	}
	void ContinueButtonAction()
    {
        if (!isOptions)
        {
            continueImage.isActive = true;
			

			continueB.isActive = true;
            int a = continueB.GetComponent<UIButton>().GetButtonState();
            switch (a)
            {
                case 0:
                    // disabled Mode
                    break;
                case 1:
                    if (!isFirstCont)
                    {

						pos.Set(continueB.GetComponent<Transform2D>().position2D.x - 20, continueB.GetComponent<Transform2D>().position2D.y, 36.1f);
                        continueB.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(continueImage.GetComponent<Transform2D>().position2D.x - 20, continueImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        continueImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstCont = true;
                    }
                    // normal Mode
                    break;
                case 2:
                    // focused mode
                    if (isFirstCont)
                    {
                        SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
                        pos.Set(continueB.GetComponent<Transform2D>().position2D.x + 20, continueB.GetComponent<Transform2D>().position2D.y, 36.1f);
                        continueB.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(continueImage.GetComponent<Transform2D>().position2D.x + 20, continueImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        continueImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstCont = false;
                        //poner sonido
                    }
                    break;
                case 3:
                    // pressed mode
                    SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");


					//cambiar de escena
					SaveSystem.LoadScene();
                    break;
            }
        }
        else
        {
            continueImage.isActive = false;
            continueB.isActive = false;
        }
    }
	void OptionsButtonAction()
    {
        if (!isOptions)
        {
            optionsImage.isActive = true;
            options.isActive = true;
            int a = options.GetComponent<UIButton>().GetButtonState();
            switch (a)
            {
                case 0:
                    // disabled Mode
                    break;
                case 1:
                    if (!isFirstOp)
                    {
                        pos.Set(options.GetComponent<Transform2D>().position2D.x - 20, options.GetComponent<Transform2D>().position2D.y, 36.1f);
                        options.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(optionsImage.GetComponent<Transform2D>().position2D.x - 20, optionsImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        optionsImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstOp = true;
                    }
                    // normal Mode
                    break;
                case 2:
                    // focused mode
                    if (isFirstOp)
                    {
                        SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
                        pos.Set(options.GetComponent<Transform2D>().position2D.x + 20, options.GetComponent<Transform2D>().position2D.y, 36.1f);
                        options.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(optionsImage.GetComponent<Transform2D>().position2D.x + 20, optionsImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        optionsImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstOp = false;
                        //poner sonido
                    }
                    break;
                case 3:
                    // pressed mode
                    SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");
                    isOptions = true;

                    //cambiar de escena
                    break;
            }
        }
        else
        {
			
            optionsImage.isActive = false;
            options.isActive = false;
        }
    }
	void CreditsButtonAction()
	{
        if (!isOptions)
        {
            credits.isActive = true;
            creditsImage.isActive = true;
            int a = credits.GetComponent<UIButton>().GetButtonState();
            switch (a)
            {
                case 0:
                    // disabled Mode
                    break;
                case 1:
                    if (!isFirstCre)
                    {
                        pos.Set(credits.GetComponent<Transform2D>().position2D.x - 20, credits.GetComponent<Transform2D>().position2D.y, 36.1f);
                        credits.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(creditsImage.GetComponent<Transform2D>().position2D.x - 20, creditsImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        creditsImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstCre = true;
                    }
                    // normal Mode
                    break;
                case 2:
                    // focused mode
                    if (isFirstCre)
                    {
                        SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
                        pos.Set(credits.GetComponent<Transform2D>().position2D.x + 20, credits.GetComponent<Transform2D>().position2D.y, 36.1f);
                        credits.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(creditsImage.GetComponent<Transform2D>().position2D.x + 20, creditsImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        creditsImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstCre = false;
                        //poner sonido
                    }
                    break;
                case 3:
                    // pressed mode
                    SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");

                    SceneManager.LoadScene("Credits");
                    //cambiar de escena
                    break;
            }
        }
        else
        {
            credits.isActive = false;
            creditsImage.isActive = false;
        }

    }
	void PlayButtonAction()
	{
		if (!isOptions) 
        {
			start.isActive = true;
            startImage.isActive = true;
            int a = start.GetComponent<UIButton>().GetButtonState();
            switch (a)
            {
                case 0:
                    // disabled Mode
                    break;
                case 1:
                    if (!isFirstS)
                    {
                        pos.Set(start.GetComponent<Transform2D>().position2D.x - 20, start.GetComponent<Transform2D>().position2D.y, 36.1f);
                        start.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(startImage.GetComponent<Transform2D>().position2D.x - 20, startImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        startImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstS = true;
                    }
                    // normal Mode
                    break;
                case 2:
                    // focused mode
                    if (isFirstS)
                    {
                        SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
                        pos.Set(start.GetComponent<Transform2D>().position2D.x + 20, start.GetComponent<Transform2D>().position2D.y, 36.1f);
                        start.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(startImage.GetComponent<Transform2D>().position2D.x + 20, startImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        startImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstS = false;
                        //poner sonido
                    }
                    break;
                case 3:
                    // pressed mode
                    SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");

					SaveSystem.fromContinue = false;
                    SceneManager.LoadScene("Cinematic_7");
                    //cambiar de escena
                    break;
            }

        }
        else
        {
			
			start.isActive = false;
            startImage.isActive = false;
        }

    }
	void ExitButtonAction()
	{
        if (!isOptions)
        {
            exit.isActive = true;
            exitImage.isActive = true;
            int a = exit.GetComponent<UIButton>().GetButtonState();
            switch (a)
            {
                case 0:
                    // disabled Mode
                    break;
                case 1:
                    if (!isFirstE)
                    {
                        pos.Set(exit.GetComponent<Transform2D>().position2D.x - 20, exit.GetComponent<Transform2D>().position2D.y, 36.1f);
                        exit.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(exitImage.GetComponent<Transform2D>().position2D.x - 20, exitImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        exitImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstE = true;
                    }
                    // normal Mode
                    break;
                case 2:
                    // focused mode
                    if (isFirstE)
                    {
                        SceneAudio.GetComponent<AudioSource>().PlayClip("UI_HOVER");
                        pos.Set(exit.GetComponent<Transform2D>().position2D.x + 20, exit.GetComponent<Transform2D>().position2D.y, 36.1f);
                        exit.GetComponent<Transform2D>().position2D = pos;


                        pos.Set(exitImage.GetComponent<Transform2D>().position2D.x + 20, exitImage.GetComponent<Transform2D>().position2D.y, 36.1f);
                        exitImage.GetComponent<Transform2D>().position2D = pos;
                        isFirstE = false;
                        //poner sonido
                    }
                    break;
                case 3:
                    // pressed mode
                    SceneAudio.GetComponent<AudioSource>().PlayClip("UI_SELECT");

                    SceneManager.Exit();
                    //salir del Juego
                    break;
            }
        }
        else
        {
			exit.isActive = false;
            exitImage.isActive = false;
        }

    }

	void BackgroundImageAction()
	{
        if (!isOptions)
        {
            back.isActive = true;
            back.GetComponent<Transform2D>().SetSize(InternalCalls.GetRegionGame()); 

            pos = back.GetComponent<Transform2D>().position2D;
            pos.Set(0.0f, 0.0f, -8.5f);
            back.GetComponent<Transform2D>().position2D = pos;
        }
        else
        {
			pos.Set(0.0f, 2000.0f, -8.5f);
			back.GetComponent<Transform2D>().position2D = pos;
			//back.isActive = false;
		}
	}
	//////////OPTIONS
	void updateOptions()
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
			OptionsBackHide();
			OptionsAllButtonsHide();
			SelectedOptionHide();
			OptionsControlHide();
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
				if (isFirstOcontrolsL)
				{
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
			optionsControl8.GetComponent<UIButton>().text = "Drag";
			optionsControl9.GetComponent<UIButton>().text = "Crl L";
			optionsControl14.GetComponent<UIButton>().text = "A";
			optionsControl15.GetComponent<UIButton>().text = "S";
			optionsControl16.GetComponent<UIButton>().text = "D";
			optionsControl17.GetComponent<UIButton>().text = "F";
			optionsControl18.GetComponent<UIButton>().text = "G";
		}
		else if (actualControlOption == 1)
		{
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
		optionsControl5.GetComponent<UIButton>().SetTextPosition(-30, -3.2f);

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
	void SelectedOptionHide()
	{

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

		bounds.Set(X, (InternalCalls.GetRegionGame().y - 300), -10.400f);
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
		float sum=(lastWindowW- X)/2;
		int a = optionsControlsButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 0:
				// disabled Mode
				break;
			case 1:
				if (isFirstOControlsB)
				{

					isFirstOControlsB = false;
					
				}
				pos.Set(X-1217+sum, (InternalCalls.GetRegionGame().y / 2) - 380, -10.4f);
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
				if (isFirstOSoundB)
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
				
				isOptions = false;


				isFirstOBackB = true;
				isFirstOScreenB = true;
				isFirstOSoundB = true;
				isFirstOControlsB = true;

                SaveOptions();

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
		{
		
			InternalCalls.SetFullScreen(true);
		}
		else
		{

			InternalCalls.SetFullScreen(false);
		}

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

		pos.Set(-sum+20, y - 230, 36.1f);
		optionsSreenText.GetComponent<Transform2D>().position2D = pos;

		////////////////////////////////////////////////
		/// DROP DOWN
		////////////////////////////////////////////////

		pos.Set(x - 550, y - 500, 36.1f);
		optionsLanguaje.GetComponent<Transform2D>().position2D = pos;

		
		if (optionsLanguaje.GetComponent<UIDropDown>().GetDropDownButtonChange())
		{
			int a = optionsLanguaje.GetComponent<UIDropDown>().GetDropDownSelected();
            optionsLanguaje.GetComponent<UIDropDown>().SetDropDownLenguage(a);

        }

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


		pos.Set(-sum + 20, y - 230, 36.1f);
		optionsSoundText.GetComponent<Transform2D>().position2D = pos;

		float generalSound;
		float VoicesSound;
		float FXcSound;
		float MusicSound;


		pos.Set(x - 650, y - 260, 36.1f);
		optionsVoicesSound.GetComponent<Transform2D>().position2D = pos;
		VoicesSound = 100 * optionsVoicesSound.GetComponent<UISlider>().GetSliderActualValue();



		pos.Set(x - 650, y - 350, 36.1f);
		optionsFXcSound.GetComponent<Transform2D>().position2D = pos;
		FXcSound = 100 * optionsVoicesSound.GetComponent<UISlider>().GetSliderActualValue();



		pos.Set(x - 650, y - 440, 36.1f);
		optionsMusicSound.GetComponent<Transform2D>().position2D = pos;
		MusicSound = 100 * optionsVoicesSound.GetComponent<UISlider>().GetSliderActualValue();



		pos.Set(x - 650, y - 530, 36.1f);
		optionsGeneralSound.GetComponent<Transform2D>().position2D = pos;
		generalSound = 100 * optionsVoicesSound.GetComponent<UISlider>().GetSliderActualValue();


	}
	void OptionsSoundHide()
	{
		optionsSoundText.isActive = false;
		optionsVoicesSound.isActive = false;
		optionsFXcSound.isActive = false;
		optionsMusicSound.isActive = false;
		optionsGeneralSound.isActive = false;
	}


}
