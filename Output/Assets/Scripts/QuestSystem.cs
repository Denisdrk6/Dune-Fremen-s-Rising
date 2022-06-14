using System;
using System.Collections.Generic;
using RagnarEngine;

public enum QuestState
{
	ACTIVE = 0,
	INACTIVE,
	COMPLETED,
	UNKNOWN,
};

public enum QuestType
{
	MAIN = 0,
	SECONDARY,
	UNKNOWN,
};

public class Quest
{
	// Constructors and destructor
	public Quest(uint _id, string _name, string _description, QuestState _state, QuestType _type)
    {
		id = _id;
		name = _name;
		description = _description;
		state = _state;
		type = _type;
    }
	// Variables
	uint id;
	string name;
	string description;
	QuestState state;
	QuestType type;
	// Public Methods
	public void ChangeQuestState(QuestState newState) { state = newState; }
	public uint GetQuestId() { return id; }
	public string GetQuestName() { return name; }
	public string GetQuestDescription() { return description; }
	public QuestState GetQuestState() { return state; }
	public QuestType GetQuestType() { return type; }
};
public class QuestSystem : RagnarComponent
{
	public List<Quest> questList;
	public List<Quest> completedQuestList;

	public List<Quest> mainQuestList;
	public List<Quest> secondaryQuestList;

	public GameObject mainQuestNames;
	public GameObject secondaryQuestNames;

	public GameObject completedQuestNames;
	public GameObject questDescription;
	public GameObject questId;
	public GameObject questType;
	public GameObject questState;

	public GameObject mainButton;
	public GameObject secondaryButton;
	public GameObject completedButton;

	public GameObject questBc;
	public GameObject questBord;
	GameObject MissButton;
	Transform2D questBcT;
	Transform2D questBordT;
	GameObject GameState;
	public bool showJournal;

	public bool showMain;
	public bool showSecondary;
	public bool showCompleted;
	public Vector3 position;

	private string mainQuests;
	private string secondaryQuests;
	private string completedQuests;
	private int level = 0;

	//Check Quests Vars General
	public bool levelFinished = false;
	public bool damageRecieved = false;
	public bool hasKilledEnemies = false;
	//Check Quests Vars Lvl 1
	public int enemiesControlled = 0;
	public int enemiesThrowingKnife = 0;
	public int enemiesDistractedStone = 0;
	//Check Quests Vars Lvl 2
	public bool doorsLevel = false;
	public bool midLevel = false;
	public bool camouflageActive = false;
	public int enemiesCamouflage = 0;
	public int enemiesHunterSeeker = 0;
	public int enemiesGrenade = 0;
	//Check Quests Vars Lvl 3
	public bool firstPhaseCompleted = false;
	public bool lastPhaseCompleted = false;
	public bool killWithPaul = false;
	public bool killWithChani = false;
	public bool killWithStilgar = false;
	public bool triggerQuest = true;
	public bool triggerStunner = true;
	public bool completeStunner = false;
	public int enemiesTrap = 0;
	public int enemiesWhistle = 0;
	private bool changePage = false;

	// Easing
	float actualDt = 0;
	bool isPlayng = false;
	bool isSceneWin=false;

	int mult = 1;
	int textRepos = 0;
	int spawnPos = 1;
	double EasingFunction(float x)
    {
		float c1 = 1.70158f;
		float c2 = c1 * 1.525f;
		if(x < 0.5)
        {
			return (Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2;

        }
        else
        {
			return (Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
		}
				
	}
	// Methods

	public void CreateQuest(uint _id, string _name, string _description, QuestState _state, QuestType _type)
    {
		Quest newQuest;
		newQuest = new Quest(_id, _name, _description, _state, _type);
		AddQuest(newQuest);
	}

	public void AddQuest(Quest questToAdd)
    {
		questList.Add(questToAdd);

		if (questToAdd.GetQuestType() == QuestType.MAIN)
			mainQuestList.Add(questToAdd);
		else if (questToAdd.GetQuestType() == QuestType.SECONDARY)
			secondaryQuestList.Add(questToAdd);
	}
	public void CompleteQuest(Quest questToComplete)
    {
		QuestType auxType = questToComplete.GetQuestType();
		completedQuestList.Add(questToComplete);
		questToComplete.ChangeQuestState(QuestState.COMPLETED);

		if (auxType == QuestType.MAIN)
			mainQuestList.Remove(questToComplete);
		else if (auxType == QuestType.SECONDARY)
			secondaryQuestList.Remove(questToComplete);
	}
	public void RemoveQuest(Quest questToDelete)
    {
		questList.Remove(questToDelete);
	}
	public void ChangeQuestState(Quest questToChange, QuestState newState)
    {
		questToChange.ChangeQuestState(newState);
    }

	public Quest GetQuestByID(int id)
    {
		Quest questToReturn = null;
		for(int i = 0; i < questList.Count - 1; ++i)
        {
			if (questList[i].GetQuestId() == id)
				return questList[i];
        }
		return questToReturn;
    }

	public Quest GetQuestByName(string name)
	{
		Quest questToReturn = null;
		for (int i = 0; i < questList.Count - 1; ++i)
		{
			if (questList[i].GetQuestName() == name)
				return questList[i];
		}
		return questToReturn;
	}

	public void Start()
	{
		// Initialize Lists
		questList = new List<Quest>();
		completedQuestList = new List<Quest>();
		mainQuestList = new List<Quest>();
		secondaryQuestList = new List<Quest>();

		if (SceneManager.currentSceneName == "build")
			level = 1;
		else if (SceneManager.currentSceneName == "build2")
			level = 2;
		else if (SceneManager.currentSceneName == "build3")
			level = 3;

		showJournal = false;
		showMain = true;
		showSecondary = false;
		showCompleted = false;

		completedButton = GameObject.Find("Boton Completadas");
		mainButton = GameObject.Find("Boton Principales");
		secondaryButton = GameObject.Find("Boton Secundarias");

		completedButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
		mainButton.GetComponent<UIButton>().SetButtonTextColor(255, 255, 255);
		secondaryButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);

		if (level == 0)
        {
			isSceneWin = true;
			if (SceneManager.lastSceneName == "build")
				level = 1;
			else if (SceneManager.lastSceneName == "build2")
				level = 2;
			else if (SceneManager.lastSceneName == "build3")
				level = 3;

			showMain = false;
			showSecondary = false;
			showCompleted = true;

			completedButton.GetComponent<UIButton>().SetButtonTextColor(255, 255, 255);
			mainButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
		}

		mainQuestNames = GameObject.Find("Titulo Principales");
		secondaryQuestNames = GameObject.Find("Titulo Secundarias");
		completedQuestNames = GameObject.Find("Titulo Completadas");
		questDescription = GameObject.Find("Descripcion");
		questId = GameObject.Find("Id");
		questState = GameObject.Find("Estado");
		questType = GameObject.Find("Tipo");
		
		questBc = GameObject.Find("questBc");
		questBord = GameObject.Find("questBord");
		MissButton = GameObject.Find("MissButton");
		GameState = GameObject.Find("Background");
		questBordT = questBord.GetComponent<Transform2D>();
		questBcT = questBc.GetComponent<Transform2D>();
		
		position = new Vector3(0.0f, 0.0f, 0.0f);
		mainButton.isActive = false;
		secondaryButton.isActive = false;
		completedButton.isActive = false;
		mainQuestNames.isActive = false;
		secondaryQuestNames.isActive = false;
		completedQuestNames.isActive = false;
		questDescription.isActive = false;
		questId.isActive = false;
		questState.isActive = false;
		questType.isActive = false;
		questBc.isActive = false;
		questBord.isActive = false;

		switch (level)
        {
			case 1:
				//Main Quests
				CreateQuest(0, "Ábrete paso a través del cañón", "Encuéntrate con Chani al final del cañón", QuestState.ACTIVE, QuestType.MAIN);
				//Secondaries Quests
				CreateQuest(1, "Intocable", "Completa el nivel sin recibir daño", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(2, "Pacifista", "Completa el nivel sin matar a nadie", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(3, "Azotador de mentes", "Controla mentalmente a tres enemigos", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(4, "Pilla esta!", "Mata 5 enemigos utilizando el cuchillo arrojadizo", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(5, "Habrá sido el viento", "Distrae a 5 enemigos con piedras", QuestState.ACTIVE, QuestType.SECONDARY);
				//Auxiliar quest (Solve a Bug)
				CreateQuest(6, "", "", QuestState.ACTIVE, QuestType.SECONDARY);
				break;
			case 2:
				//Main Quests
				CreateQuest(7, "Visita no grata", "Infiltrate en Arrakeen", QuestState.ACTIVE, QuestType.MAIN);
				CreateQuest(8, "La Reunión", "Encuentra a Stilgar y Lady Jessica", QuestState.ACTIVE, QuestType.MAIN);
				CreateQuest(9, "Cambio de planes", "Rescata a Stilgar de los soldados Harkonnen", QuestState.ACTIVE, QuestType.MAIN);
				//Secondaries Quests
				CreateQuest(10, "Intocable", "Completa el nivel sin recibir daño", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(11, "Pacifista", "Completa el nivel sin matar a nadie", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(12, "Desde las sombras", "Acaba con 3 enemigos mientras estás camuflado", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(13, "Detrás de ti", "Usa el Hunter-seeker contra 3 enemigos", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(14, "Toque especial", "Aturde a 4 enemigos con granadas", QuestState.ACTIVE, QuestType.SECONDARY);
				//Auxiliar quest (Solve a Bug)
				CreateQuest(15, "", "", QuestState.ACTIVE, QuestType.SECONDARY);
				break;
			case 3:
				//Main Quests
				CreateQuest(16, "De nuevo en casa", "Rescata a Lady Jessica de los Harkonnen", QuestState.ACTIVE, QuestType.MAIN);
				CreateQuest(17, "Yo soy la venganza", "Acaba con Rabban", QuestState.ACTIVE, QuestType.MAIN);
				//Secondaries Quests
				CreateQuest(18, "Intocable", "Completa el nivel sin recibir daño", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(19, "Trabajo en equipo", "Mata 1 enemigo con cada personaje", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(20, "Mano fácil", "Gasta toda la munición de la Stunner", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(21, "Atrapado", "Consigue que 3 enemigos caigan en tus trampas", QuestState.ACTIVE, QuestType.SECONDARY);
				CreateQuest(22, "Sirena", "Atrae 3 enemigos con silbidos", QuestState.ACTIVE, QuestType.SECONDARY);
				//Auxiliar quest (Solve a Bug)
				CreateQuest(23, "", "", QuestState.ACTIVE, QuestType.SECONDARY);
				break;
        }
        if (isSceneWin)
        {
			showJournal = true;
			isPlayng = true;
		}
		if (SceneManager.currentSceneName== "WinScene")
        {
			LoadMissions();
        }
		
	}
	public void Update()
	{
		if(changePage && !isSceneWin)
        {
			changePage = false;
			GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetPlayerSelected().PlayerPause();
        }
		// Logic from quests
		switch (level)
        {
			case 1:
				if (levelFinished)
                {
					CompleteQuest(GetQuestByID(0));
					if (!damageRecieved) CompleteQuest(GetQuestByID(1));
					if (!hasKilledEnemies) CompleteQuest(GetQuestByID(2));
					levelFinished = false;
				}
				if (enemiesControlled == 3)
				{
					CompleteQuest(GetQuestByID(3));
					enemiesControlled = -100;
				}
				if (enemiesThrowingKnife == 5)
				{
					CompleteQuest(GetQuestByID(4));
					enemiesThrowingKnife = -100;
				}
				if (enemiesDistractedStone == 5)
				{
					CompleteQuest(GetQuestByID(5));
					enemiesDistractedStone = -100;
				}
				break;
			case 2:
				if (levelFinished)
                {
					CompleteQuest(GetQuestByID(7));
					CompleteQuest(GetQuestByID(9));
					if (!damageRecieved) CompleteQuest(GetQuestByID(10));
					if (!hasKilledEnemies) CompleteQuest(GetQuestByID(11));
					levelFinished = false;
				}
				if (doorsLevel)
				{
					CompleteQuest(GetQuestByID(7));
					doorsLevel = false;
				}
				if (midLevel)
                {
					CompleteQuest(GetQuestByID(8));
					midLevel = false;
                }
				if (enemiesCamouflage == 3)
                {
					CompleteQuest(GetQuestByID(12));
					enemiesCamouflage = -100;
				}
				if (enemiesHunterSeeker == 3)
				{
					CompleteQuest(GetQuestByID(13));
					enemiesHunterSeeker = -100;
				}
				if (enemiesGrenade == 4)
				{
					CompleteQuest(GetQuestByID(14));
					enemiesGrenade = -100;
				}
				break;
			case 3:
				if (firstPhaseCompleted)
                {
					CompleteQuest(GetQuestByID(16));
					firstPhaseCompleted = false;
				}
				if (lastPhaseCompleted)
				{
					CompleteQuest(GetQuestByID(17));
					if (!damageRecieved) CompleteQuest(GetQuestByID(18));

					lastPhaseCompleted = false;
				}
				if (triggerQuest && (killWithChani && killWithPaul && killWithStilgar))
                {
					CompleteQuest(GetQuestByID(19));
					triggerQuest = false;
				}
				if (triggerStunner && completeStunner)
                {
					CompleteQuest(GetQuestByID(20));
					completeStunner = false;
					triggerStunner = false;
				}
				if (enemiesTrap == 3)
                {
					CompleteQuest(GetQuestByID(21));
					enemiesTrap = -100;
				}
				if (enemiesWhistle == 3)
                {
					CompleteQuest(GetQuestByID(22));
					enemiesWhistle = -100;
				}
				break;
		}

		// Drawing
		float xCorner = (InternalCalls.GetRegionGame().x / 2);
		float yCorner = (InternalCalls.GetRegionGame().y / 2);
		int a = 0;
		if (isSceneWin)
		{
			mult = -1;
			textRepos = 2750;
			xCorner *= -1;
			xCorner -= (InternalCalls.GetRegionGame().x / 20);
			yCorner -= (InternalCalls.GetRegionGame().y / 4);
			spawnPos = -1;
		}
		if (!isSceneWin)
        {
			position.Set(xCorner - 70, yCorner - 70, 1000000.0f);
			MissButton.GetComponent<Transform2D>().position2D = position;
			a = MissButton.GetComponent<UIButton>().GetButtonState();
			
			switch (a)
			{
				case 3:
					// pressed mode
					if (!isPlayng)
					{
						showJournal = !showJournal;
						isPlayng = true;
					}


					break;
			}
		}
		if (!isSceneWin && Input.GetKey(KeyCode.J) == KeyState.KEY_DOWN)
		{
			if (!isPlayng)
			{
				showJournal = !showJournal;
				isPlayng = true;
			}
		}

		if (showJournal)
		{
			if (isPlayng)
			{
				if (actualDt >= 1)
				{
					isPlayng = false;
					actualDt = 0;
				}
				else
				{
					actualDt += Time.deltaTime;

					float actualvalue = (float)EasingFunction(actualDt);


					position.Set((xCorner - 500 * (mult)) * actualvalue + ((1 - actualvalue) * (xCorner + 400 * spawnPos)), yCorner - 280, 0);

					questBcT.position2D = position;
					questBordT.position2D = position;

					position.Set((xCorner - 743 * (mult)) * actualvalue + ((1 - actualvalue) * (xCorner + 400 * spawnPos)), yCorner - 70.5f, 1000000.0f);
					mainButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner - 500 * (mult)) * actualvalue + ((1 - actualvalue) * (xCorner + 400 * spawnPos)), yCorner - 70.5f, 1000000.0f);
					secondaryButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner - 258 * (mult)) * actualvalue + ((1 - actualvalue) * (xCorner + 400 * spawnPos)), yCorner - 70.5f, 1000000.0f);
					completedButton.GetComponent<Transform2D>().position2D = position;

					position.Set(((xCorner - 800 + (textRepos)) * (mult)) * actualvalue + ((1 - actualvalue) * (xCorner + 400 * spawnPos)), yCorner - 150, 1000000.0f);
					mainQuestNames.GetComponent<Transform2D>().position2D = position;

					position.Set(((xCorner - 800 + (textRepos)) * (mult)) * actualvalue + ((1 - actualvalue) * (xCorner + 400 * spawnPos)), yCorner - 150, 1000000.0f);
					secondaryQuestNames.GetComponent<Transform2D>().position2D = position;

					completedQuestNames.GetComponent<Transform2D>().position2D = position;
				}

			}

			mainButton.isActive = true;
			secondaryButton.isActive = true;
			completedButton.isActive = true;
			questBc.isActive = true;
			questBord.isActive = true;

			if (showMain)
			{
				mainQuestNames.isActive = true;
				secondaryQuestNames.isActive = false;
				completedQuestNames.isActive = false;
				questDescription.isActive = false;
				questId.isActive = false;
				questState.isActive = false;
				questType.isActive = false;
			}
			if (showSecondary)
			{
				mainQuestNames.isActive = false;
				secondaryQuestNames.isActive = true;
				completedQuestNames.isActive = false;
				questDescription.isActive = false;
				questId.isActive = false;
				questState.isActive = false;
				questType.isActive = false;
			}
			if (showCompleted)
			{
				mainQuestNames.isActive = false;
				secondaryQuestNames.isActive = false;
				completedQuestNames.isActive = true;
				questDescription.isActive = false;
				questId.isActive = false;
				questState.isActive = false;
				questType.isActive = false;
			}
		}
		else
		{
			if (isPlayng)
			{
				if (actualDt >= 1)
				{
					isPlayng = false;
					actualDt = 0;
					mainButton.isActive = false;
					secondaryButton.isActive = false;
					completedButton.isActive = false;
					mainQuestNames.isActive = false;
					secondaryQuestNames.isActive = false;
					completedQuestNames.isActive = false;
					questDescription.isActive = false;
					questId.isActive = false;
					questState.isActive = false;
					questType.isActive = false;
					questBc.isActive = false;
					questBord.isActive = false;
				}
				else
				{
					actualDt += Time.deltaTime;

					float actualvalue = (float)EasingFunction(actualDt);


					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 500)), yCorner - 280, 0);

					questBcT.position2D = position;
					questBordT.position2D = position;

					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 743)), yCorner - 70.5f, 1000000.0f);
					mainButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 500)), yCorner - 70.5f, 1000000.0f);
					secondaryButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 258)), yCorner - 70.5f, 1000000.0f);
					completedButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 800 - (textRepos))), yCorner - 150, 1000000.0f);
					mainQuestNames.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 800 - (textRepos))), yCorner - 150, 1000000.0f);
					secondaryQuestNames.GetComponent<Transform2D>().position2D = position;

					completedQuestNames.GetComponent<Transform2D>().position2D = position;
				}
			}
		}

		if (!isSceneWin && (GameState.GetComponent<pauseMenuButton>().isSowing || GameState.GetComponent<pauseMenuButton>().isOptions))
		{
			mainButton.isActive = false;
			secondaryButton.isActive = false;
			completedButton.isActive = false;
			mainQuestNames.isActive = false;
			secondaryQuestNames.isActive = false;
			completedQuestNames.isActive = false;
			questDescription.isActive = false;
			questId.isActive = false;
			questState.isActive = false;
			questType.isActive = false;
			questBc.isActive = false;
			questBord.isActive = false;
		}

		mainQuests = "";
		secondaryQuests = "";
		completedQuests = "";

		if (mainQuestList.Count > 0)
		{
			for (int i = 0; i < mainQuestList.Count; ++i)
			{
				mainQuests += (i+1).ToString();
				mainQuests += ". ";
				mainQuests += mainQuestList[i].GetQuestName().ToString() + "\n    ";
				mainQuests += mainQuestList[i].GetQuestDescription().ToString() + "\n\n";
			}
		}
		else
		{
			mainQuests = "No main quests available";
		}

		if (secondaryQuestList.Count > 0)
		{
			for (int i = 0; i < secondaryQuestList.Count-1; ++i)
			{

				secondaryQuests += (i + 1).ToString();
				secondaryQuests += ". ";
				secondaryQuests += secondaryQuestList[i].GetQuestName().ToString();
				if(secondaryQuestList[i].GetQuestId()==3)
                {
					secondaryQuests += "    (" + enemiesControlled.ToString()+ "/3)"+ "\n    ";
				}
				else if(secondaryQuestList[i].GetQuestId() == 4) 
				{
					secondaryQuests +="    ("+ enemiesThrowingKnife.ToString() + "/5)" + "\n    ";
				}else if(secondaryQuestList[i].GetQuestId() == 5)
				{
					secondaryQuests += "    (" + enemiesDistractedStone.ToString() + "/5)" + "\n    " ;
                }
				else if (secondaryQuestList[i].GetQuestId() == 12)
				{
					secondaryQuests += "    (" + enemiesCamouflage.ToString() + "/3)" + "\n    ";
				}
				else if (secondaryQuestList[i].GetQuestId() == 13)
				{
					secondaryQuests += "    (" + enemiesHunterSeeker.ToString() + "/3)" + "\n    ";
				}
				else if (secondaryQuestList[i].GetQuestId() == 14)
				{
					secondaryQuests += "    (" + enemiesGrenade.ToString() + "/4)" + "\n    ";
				}
				else if (secondaryQuestList[i].GetQuestId() == 21)
				{
					secondaryQuests += "    (" + enemiesTrap.ToString() + "/3)" + "\n    ";
				}
				else if (secondaryQuestList[i].GetQuestId() == 22)
				{
					secondaryQuests += "    (" + enemiesWhistle.ToString() + "/3)" + "\n    ";
				}
				else
                {
					secondaryQuests += "\n    ";
				}
				secondaryQuests += secondaryQuestList[i].GetQuestDescription().ToString() + "\n\n";
			}
		}
		else
		{
			secondaryQuests = "No secondary quests available";
		}

		if (completedQuestList.Count > 0)
        {
			for (int i = 0; i < completedQuestList.Count; ++i)
			{
				completedQuests += completedQuestList[i].GetQuestName().ToString() + "\n\n";				
			}
		}
		else
        {
			completedQuests = "No completed quests available";
		}
		position.Set(242, 68.5f, 0);
		mainButton.GetComponent<Transform2D>().SetSize(position);
		UIButton mainButtonUI = mainButton.GetComponent<UIButton>();
		mainButtonUI.SetTextPosition(-45, -5);
		mainButtonUI.text = "Main";

		position.Set(242, 68.5f, 0);
		secondaryButton.GetComponent<Transform2D>().SetSize(position);
		UIButton secondaryButtonUI = secondaryButton.GetComponent<UIButton>();
		secondaryButtonUI.SetTextPosition(-45, -5);
		secondaryButtonUI.text = "Secondary";

		position.Set(242, 68.5f, 0);
		completedButton.GetComponent<Transform2D>().SetSize(position);
		UIButton completedButtonUI = completedButton.GetComponent<UIButton>();
		completedButtonUI.SetTextPosition(-55, -5);
		completedButtonUI.text = "Completed";
		
		mainQuestNames.GetComponent<UIText>().text = mainQuests;
		if (mainQuests == "No main quests available")
			mainQuestNames.GetComponent<UIText>().SetTextTextColor(255, 0, 0);
		else
			mainQuestNames.GetComponent<UIText>().SetTextTextColor(255, 255, 255);

		secondaryQuestNames.GetComponent<UIText>().text = secondaryQuests;
		if (secondaryQuests == "No secondary quests available")
			secondaryQuestNames.GetComponent<UIText>().SetTextTextColor(255, 0, 0);
		else
			secondaryQuestNames.GetComponent<UIText>().SetTextTextColor(255, 255, 255);

		completedQuestNames.GetComponent<UIText>().text = completedQuests;
		
		if (completedQuests == "No completed quests available")
			completedQuestNames.GetComponent<UIText>().SetTextTextColor(255, 0, 0);
		else
			completedQuestNames.GetComponent<UIText>().SetTextTextColor(255, 255, 255);
		a = mainButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 3:
				// pressed mode
				completedButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
				secondaryButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
				mainButton.GetComponent<UIButton>().SetButtonTextColor(255, 255, 255);
				showMain = true;
				showSecondary = false;
				showCompleted = false;
				changePage = true;
				break;
		}

		a = secondaryButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 3:
				// pressed mode
				completedButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
				mainButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
				secondaryButton.GetComponent<UIButton>().SetButtonTextColor(255, 255, 255);
				showMain = false;
				showSecondary = true;
				showCompleted = false;
				changePage = true;
				break;
		}

		a = completedButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 3:
				// pressed mode
				mainButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
				secondaryButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
				completedButton.GetComponent<UIButton>().SetButtonTextColor(255, 255, 255);
				showMain = false;
				showSecondary = false;
				showCompleted = true;
				changePage = true;
				break;
		}

	}
	public void SaveMissions()
    {
		SaveSystem.DeleteDirectoryFiles("Library/SavedGame/Quest");
		for (int i = 0; i < completedQuestList.Count; i++)
		{
			SaveSystem.SaveQuest(completedQuestList[i]);
        }
    }
	public void LoadMissions()
	{		
		for (int i = 0; i < questList.Count; i++)
		{	
			QuestData data = SaveSystem.LoadQuest(questList[i].GetQuestId().ToString());
			if (data != null&&data.state==QuestState.COMPLETED)
            {
				CompleteQuest(questList[i]);
            }
		}
	}
}