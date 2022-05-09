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
    public void Start()
	{
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


		continueB.GetComponent<UIButton>().SetButtonState(0);
		continueB.GetComponent<UIButton>().SetButtonTextColor(121,121,121);
		continueB.GetComponent<UIButton>().SetButtonGeneralColor(121, 121, 121);


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
		//optionsScreenSDCH.GetComponent<UICheckbox>().SetCheckboxState(Light.shadowsEnabled);
		//optionsScreenFSCH.GetComponent<UICheckbox>().SetCheckboxState(InternalCalls.GetFullScreen());
		//optionsScreenVSCH.GetComponent<UICheckbox>().SetCheckboxState(InternalCalls.GetVSync());
		OptionsBackHide();
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
                    SceneManager.LoadScene("build");
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

            OptionsBackShow();
            SelectedOptionShow();
            OptionsAllButtonsShow();
        }
		else
		{
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

		}
		else if (actualOption == "Sound")
		{
			optionsSoundButton.GetComponent<UIButton>().SetButtonState(2);
			OptionsSoundShow();
		}

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

		pos.Set(x - 550, y - 500, 36.1f);
		optionsLanguaje.GetComponent<Transform2D>().position2D = pos;
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
