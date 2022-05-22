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
	// Lists
	public List<Quest> questList;
	public List<Quest> completedQuestList;
	public List<Quest> activeQuestList;

	public GameObject activeQuestNames;
	public GameObject completedQuestNames;
	public GameObject questDescription;
	public GameObject questId;
	public GameObject questType;
	public GameObject questState;

	public GameObject activeButton;
	public GameObject completedButton;

	public GameObject questBc;
	public GameObject questBord;

	Transform2D questBcT;
	Transform2D questBordT;

	public bool showJournal;
	public bool showActive;
	public bool showCompleted;
	public Vector3 position;

	private int captainsDefeated;

	private string activeQuests;
	private string completedQuests;


	// Easing
	float actualDt = 0;
	bool isPlayng = false;
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
		activeQuestList.Add(questToAdd);
	}
	public void CompleteQuest(Quest questToComplete)
    {
		completedQuestList.Add(questToComplete);
		questToComplete.ChangeQuestState(QuestState.COMPLETED);
		activeQuestList.Remove(questToComplete);
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
		activeQuestList = new List<Quest>();

		activeQuestNames = GameObject.Find("Titulo Activas");
		completedQuestNames = GameObject.Find("Titulo Completadas");
		questDescription = GameObject.Find("Descripcion");
		questId = GameObject.Find("Id");
		questState = GameObject.Find("Estado");
		questType = GameObject.Find("Tipo");

		activeButton = GameObject.Find("Boton Activas");
		completedButton = GameObject.Find("Boton Completadas");
		completedButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
		activeButton.GetComponent<UIButton>().SetButtonTextColor(255, 255, 255);
		questBc = GameObject.Find("questBc");
		questBord = GameObject.Find("questBord");
		
		questBordT= questBord.GetComponent<Transform2D>();
		questBcT = questBc.GetComponent<Transform2D>();

		showJournal = false;
		showActive = true;
		showCompleted = false;
		position = new Vector3(0.0f, 0.0f, 0.0f);

		activeButton.isActive = false;
		completedButton.isActive = false;
		activeQuestNames.isActive = false;
		completedQuestNames.isActive = false;
		questDescription.isActive = false;
		questId.isActive = false;
		questState.isActive = false;
		questType.isActive = false;
		questBc.isActive = false;
		questBord.isActive = false;

		CreateQuest(0, "Elimina a los 3 capitanes", "Elimina a los 3 capitanes", QuestState.ACTIVE, QuestType.MAIN);
		CreateQuest(1, "Sal de la cantera/cañon", "Sal de la cantera/cañon", QuestState.ACTIVE, QuestType.MAIN);
		CreateQuest(2, "Consigue la llave para abrir el muro", "Consigue la llave para abrir el muro", QuestState.ACTIVE, QuestType.MAIN);
		CreateQuest(3, "Reunete con Jessica y Stilgard en la plaza", "Reunete con Jessica y Stilgard en la plaza", QuestState.ACTIVE, QuestType.MAIN);
		CreateQuest(4, "Llega al palacio", "Llega al palacio", QuestState.ACTIVE, QuestType.MAIN);
		CreateQuest(5, "Busca como entrar", "Busca como entrar", QuestState.ACTIVE, QuestType.MAIN);
		CreateQuest(6, "Derrota a Rabban", "Derrota a Rabban", QuestState.ACTIVE, QuestType.MAIN);

		captainsDefeated = 0;
	}
	public void Update()
	{
		float xCorner = (InternalCalls.GetRegionGame().x / 2);
		float yCorner = (InternalCalls.GetRegionGame().y / 2);

		if (Input.GetKey(KeyCode.J) == KeyState.KEY_DOWN){
			showJournal = !showJournal;
			isPlayng = true;
		}
		if (showJournal)
        {
            if (isPlayng) {
				if (actualDt >= 1)
				{
					isPlayng = false;
					actualDt = 0;
				}
				else
				{
					actualDt += Time.deltaTime;

					float actualvalue = (float)EasingFunction(actualDt);


					position.Set((xCorner - 400) * actualvalue + ((1 - actualvalue) * (xCorner + 400)), yCorner - 280, 0);

					questBcT.position2D = position;
					questBordT.position2D = position;

					position.Set((xCorner - 579.5f) * actualvalue + ((1 - actualvalue) * (xCorner + 400)), yCorner - 70.5f, 1000000.0f);
					activeButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner - 220.5f) * actualvalue + ((1 - actualvalue) * (xCorner + 400)), yCorner - 70.5f, 1000000.0f);
					completedButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner - 700) * actualvalue + ((1 - actualvalue) * (xCorner + 400)), yCorner - 150, 1000000.0f);
					activeQuestNames.GetComponent<Transform2D>().position2D = position;

					completedQuestNames.GetComponent<Transform2D>().position2D = position;
				}

			}

			activeButton.isActive = true;
			completedButton.isActive = true;
			questBc.isActive = true;
			questBord.isActive = true;
			if (showActive)
            {
				activeQuestNames.isActive = true;
				completedQuestNames.isActive = false;
				questDescription.isActive = false;
				questId.isActive = false;
				questState.isActive = false;
				questType.isActive = false;
			}
			if (showCompleted)
            {
				activeQuestNames.isActive = false;
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

					activeButton.isActive = false;
					completedButton.isActive = false;
					activeQuestNames.isActive = false;
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


					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 400)), yCorner - 280, 0);

					questBcT.position2D = position;
					questBordT.position2D = position;

					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 579.5f)), yCorner - 70.5f, 1000000.0f);
					activeButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 220.5f)), yCorner - 70.5f, 1000000.0f);
					completedButton.GetComponent<Transform2D>().position2D = position;

					position.Set((xCorner + 400) * actualvalue + ((1 - actualvalue) * (xCorner - 700)), yCorner - 150, 1000000.0f);
					activeQuestNames.GetComponent<Transform2D>().position2D = position;

					completedQuestNames.GetComponent<Transform2D>().position2D = position;
				}

			}
			
		}

		activeQuests = "";
		completedQuests = "";

		if (activeQuestList.Count > 0)
		{
			for (int i = 0; i < activeQuestList.Count; ++i)
			{
				activeQuests += activeQuestList[i].GetQuestName().ToString() + "\n\n";
			}
		}
        else
        {
			activeQuests = "No active quests available";
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

		

		

		position.Set(365.5f, 69.5f, 0);
		activeButton.GetComponent<Transform2D>().SetSize(position);
		UIButton activeButtonUI = activeButton.GetComponent<UIButton>();
		activeButtonUI.SetTextPosition(-45,-5);
		activeButtonUI.text = "Active Quests";
		

		position.Set(365.5f, 69.5f, 0);
		completedButton.GetComponent<Transform2D>().SetSize(position);
		UIButton completedButtonUI = completedButton.GetComponent<UIButton>();
		completedButtonUI.SetTextPosition(-55, -5);
		completedButtonUI.text = "Completed Quests";
		

		activeQuestNames.GetComponent<UIText>().text = activeQuests;
		
		if (activeQuests == "No active quests available")
			activeQuestNames.GetComponent<UIText>().SetTextTextColor(255, 0, 0);
		else
			activeQuestNames.GetComponent<UIText>().SetTextTextColor(255, 255, 255);

		completedQuestNames.GetComponent<UIText>().text = completedQuests;
		
		if (completedQuests == "No completed quests available")
			completedQuestNames.GetComponent<UIText>().SetTextTextColor(255, 0, 0);
		else
			completedQuestNames.GetComponent<UIText>().SetTextTextColor(255, 255, 255);

		int a = activeButton.GetComponent<UIButton>().GetButtonState();
		switch (a)
		{
			case 3:
				// pressed mode
				completedButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
				activeButton.GetComponent<UIButton>().SetButtonTextColor(255, 255, 255);
				showActive = true;
				showCompleted = false;
				
				break;
		}

		int b = completedButton.GetComponent<UIButton>().GetButtonState();
		switch (b)
		{

			case 3:
				// pressed mode
				activeButton.GetComponent<UIButton>().SetButtonTextColor(81, 81, 81);
				completedButton.GetComponent<UIButton>().SetButtonTextColor(255, 255, 255);
				showActive = false;
				showCompleted = true;
				
				break;
		}

		if (Input.GetKey(KeyCode.M) == KeyState.KEY_DOWN && captainsDefeated < 3) ++captainsDefeated;
		if (captainsDefeated == 3 && GetQuestByName("Elimina a los 3 capitanes").GetQuestState() == QuestState.ACTIVE) CompleteQuest(GetQuestByName("Elimina a los 3 capitanes"));
	}
}