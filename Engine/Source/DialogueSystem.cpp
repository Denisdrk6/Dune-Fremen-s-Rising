#include "DialogueSystem.h"

#include "Application.h"
#include "FileSystem.h"
#include "Globals.h"
#include "Imgui/imgui.h"
#include "IconsFontAwesome5.h"
#include "ModuleSceneManager.h"

#define MARGIN_IN_TEXT 45

DialogueSystem* DialogueSystem::instance = nullptr;

DialogueSystem* DialogueSystem::GetInstance()
{
	if (!instance) instance = new DialogueSystem();

	return instance;
}

void DialogueSystem::ReleaseInstance()
{
	RELEASE(instance);
}

DialogueSystem::DialogueSystem()
{
	newDialogueFile = false;
	createDialogue = false;
	fileName = "";
	lenguage = lenguageList::SPANISH;
}

DialogueSystem::~DialogueSystem()
{
	for (std::vector<Dialogue>::iterator it = dialogues.begin(); it != dialogues.end(); ++it)
	{
		(*it).dialogue.clear();
	}
	dialogues.clear();

	currDialogue = nullptr;
	currLine = nullptr;
}

void DialogueSystem::OnEditor()
{
	ImGui::Begin("Dialogue Editor", &createDialogue);

	ShowDialogueFiles();

	if (newDialogueFile)
	{
		char text[16];

		strcpy_s(text, fileName.c_str());
		ImGui::InputText("Dialogue Name File", text, IM_ARRAYSIZE(text));
		fileName = text;
	}

	bool addDialogue = false;
	bool deleteDialogue = false;
	std::vector<Dialogue>::iterator auxIt;
	int counterId = 0;
	for (std::vector<Dialogue>::iterator it = dialogues.begin(); it != dialogues.end(); ++it)
	{
		ImGui::PushID(counterId);

		ImGui::Text("Dialogue ID: %d", counterId);

		ImGui::Separator();

		(*it).id = counterId;

		char text[384];
		bool addLine = false;
		bool deleteLine = false;
		std::vector<DialogueLine>::iterator auxIt2;
		int counter = 0;
		for (std::vector<DialogueLine>::iterator it2 = (*it).dialogue.begin(); it2 != (*it).dialogue.end(); ++it2)
		{
			ImGui::PushID(counter);

			strcpy_s(text, (*it2).author.c_str());
			ImGui::InputText("Author", text, IM_ARRAYSIZE(text));
			(*it2).author = text;

			strcpy_s(text, (*it2).line.c_str());
			ImGui::InputText("Text", text, IM_ARRAYSIZE(text));
			(*it2).line = text;

			if (ImGui::Button(ICON_FA_PLUS" Add Line"))
			{
				auxIt2 = it2;
				addLine = true;
			}
			ImGui::SameLine();
			if (ImGui::Button(ICON_FA_MINUS" Delete Line"))
			{
				auxIt2 = it2;
				deleteLine = true;
			}

			ImGui::PopID();

			counter++;
			ImGui::Spacing();
			ImGui::Spacing();
		}

		if (addLine)
		{
			DialogueLine newLine;
			newLine.line = "";
			newLine.author = "";
			(*it).dialogue.insert((++auxIt2), newLine);
		}
		if (deleteLine)
		{
			(*it).dialogue.erase(auxIt2);
		}

		if (ImGui::Button(ICON_FA_PLUS" New Dialogue"))
		{
			auxIt = it;
			addDialogue = true;
		}
		ImGui::SameLine();
		if (ImGui::Button(ICON_FA_TRASH" Delete Dialogue"))
		{
			auxIt = it;
			deleteDialogue = true;
		}

		ImGui::PopID();
		counterId++;
	}

	if (dialogues.size() == 0)
	{
		if (ImGui::Button(ICON_FA_PLUS" New Dialogue"))
		{
			DialogueLine newLine;
			newLine.line = "";
			newLine.author = "";

			Dialogue newDialog;
			newDialog.id = counterId + 1;
			newDialog.dialogue.push_back(newLine);

			dialogues.push_back(newDialog);
		}
	}

	if (addDialogue)
	{
		DialogueLine newLine;
		newLine.line = "";
		newLine.author = "";

		Dialogue newDialog;
		newDialog.id = counterId + 1;
		newDialog.dialogue.push_back(newLine);

		dialogues.insert((++auxIt), newDialog);
	}
	if (deleteDialogue)
	{
		dialogues.erase(auxIt);
	}

	ImGui::Separator();

	if (ImGui::Button(ICON_FA_FILE" Save File"))
	{
		SaveDialogue();
		if (newDialogueFile)
			newDialogueFile = false;
		Reset();
	}

	ImGui::End();
}

void DialogueSystem::ShowDialogueFiles()
{
	std::string guiName = "Dialogues";
	std::vector<std::string> files;
	app->fs->DiscoverFiles(DIALOGUES_FOLDER, files);

	std::string preview = fileName;
	if (preview == "")
		preview = "Select File";

	if (ImGui::BeginCombo(guiName.c_str(), preview.c_str()))
	{
		if (ImGui::Selectable("New Dialogue File"))
		{
			for (std::vector<Dialogue>::iterator it = dialogues.begin(); it != dialogues.end(); ++it)
			{
				(*it).dialogue.clear();
			}
			dialogues.clear();

			fileName = "";
			newDialogueFile = true;
		}

		std::string name;

		for (int i = 0; i < files.size(); ++i)
		{
			name = files[i];
			app->fs->GetFilenameWithoutExtension(name);
			guiName = name;

			if (ImGui::Selectable(guiName.c_str()))
			{
				std::string path = DIALOGUES_FOLDER + files[i];

				JsonParsing particleFile = JsonParsing();

				if (particleFile.ParseFile(path.c_str()) > 0)
				{
					fileName = guiName;
					newDialogueFile = false;
					LoadDialogue(path);
				}
			}
		}

		ImGui::EndCombo();
	}
}

void DialogueSystem::LoadDialogue(std::string path)
{
	JsonParsing dialogFile = JsonParsing();
	LoadDialogueXML();
	if (dialogFile.ParseFile(path.c_str()) > 0)
	{
		for (std::vector<Dialogue>::iterator it = dialogues.begin(); it != dialogues.end(); ++it)
		{
			(*it).dialogue.clear();
		}
		dialogues.clear();

		JSON_Array* jsonArray = dialogFile.GetJsonArray(dialogFile.ValueToObject(dialogFile.GetRootValue()), "Dialogues");

		fileName = dialogFile.GetJsonString("Name");

		size_t size = dialogFile.GetJsonArrayCount(jsonArray);
		for (int i = 0; i < size; ++i)
		{
			JsonParsing dialogue = dialogFile.GetJsonArrayValue(jsonArray, i);

			Dialogue aux;
			aux.id = dialogue.GetJsonNumber("Id");

			JSON_Array* linesArray = dialogue.GetJsonArray(dialogue.ValueToObject(dialogue.GetRootValue()), "Dialogue Lines");
			
			size_t lineSize = dialogue.GetJsonArrayCount(linesArray);
			for (int j = 0; j < lineSize; ++j)
			{
				JsonParsing dialogueLine = dialogue.GetJsonArrayValue(linesArray, j);

				DialogueLine auxLine;

				auxLine.author = dialogueLine.GetJsonString("Author");
				auxLine.line = dialogueLine.GetJsonString("Line");

				aux.dialogue.push_back(auxLine);
			}

			dialogues.push_back(aux);
		}
	}
}

//MHF
void DialogueSystem::LoadDialogueXML()
{
	std::string a;
	lenguage = (lenguageList)app->sceneManager->GetLenguage();
	Clean();
	switch (lenguage)
	{
	case lenguageList::SPANISH:
		a = "dialogos_esp2.xml";
		break;
	case lenguageList::ENGLISH:
		a = "dialogos_ing.xml";
		break;
	default:
		break;
	}
	std::string path = DIALOGUES_FOLDER + a;
	pugi::xml_parse_result result = dialoguesXML.load_file(path.c_str());

	// instead of saving the name of the author we save an id, 
	// then we relate this id to the name saved to this list
	pugi::xml_node n = dialoguesXML.first_child().child("AuthorList");
	int i = 0;
	for (n; n != NULL; n = n.next_sibling("AuthorList"))
	{
		authorList[i] = n.attribute("name").as_string();
		i++;
	}

	//We save the information of each conversation 
	n = dialoguesXML.first_child().child("Dialogue");
	for (n; n != NULL; n = n.next_sibling("Dialogue"))
	{
		DialogueXML* aDialogue = new DialogueXML;
		aDialogue->id = n.attribute("Id").as_int();
		
		LoadLinesXML(n,aDialogue);

		aDialogueXML.push_back(aDialogue);
	}
	//}

}

// Reads each node with the variables associated with each line
void DialogueSystem::LoadLinesXML(pugi::xml_node& node, DialogueXML* dlg)
{
	for (pugi::xml_node m = node.child("node"); m != NULL; m = m.next_sibling("node"))
	{
		DialogueLineXML* node = new DialogueLineXML;
		node->authorId = m.attribute("AuthorId").as_int();
		//node->line.assign(m.attribute("Line").as_string());

		//Put enters in a text, never pass de number:MARGIN_IN_TEXT of chars in line
		node->line.assign(TextWrap(m.attribute("Line").as_string(), MARGIN_IN_TEXT));

		dlg->dialogue.push_back(node);
	}
}

std::string DialogueSystem::TextWrap(std::string text, int margin)
{
	unsigned lineBegin = 0;

	while (lineBegin < text.size())
	{
		const unsigned idealEnd = lineBegin + margin;
		unsigned lineEnd = idealEnd <= text.size() ? idealEnd : text.size() - 1;

		if (lineEnd == text.size() - 1)
			++lineEnd;
		else if (std::isspace(text[lineEnd]))
		{
			text[lineEnd] = '\n';
			++lineEnd;
		}
		else    // backtrack
		{
			unsigned end = lineEnd;
			while (end > lineBegin && !std::isspace(text[end]))
				--end;

			if (end != lineBegin)
			{
				lineEnd = end;
				text[lineEnd++] = '\n';
			}
			else
				text.insert(lineEnd++, 1, '\n');
		}

		lineBegin = lineEnd;
	}
	return text;
}
//--------------------------------

void DialogueSystem::SaveDialogue()
{
	JsonParsing dialogueFile;
	JSON_Array* arrayDialogue = dialogueFile.SetNewJsonArray(dialogueFile.GetRootValue(), "Dialogues");

	dialogueFile.SetNewJsonString(dialogueFile.ValueToObject(dialogueFile.GetRootValue()), "Name", fileName.c_str());

	for (std::vector<Dialogue>::iterator it = dialogues.begin(); it != dialogues.end(); ++it)
	{
		JsonParsing file = JsonParsing();

		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Id", (*it).id);

		JSON_Array* arrayLines = file.SetNewJsonArray(file.GetRootValue(), "Dialogue Lines");

		for (std::vector<DialogueLine>::iterator it2 = (*it).dialogue.begin(); it2 != (*it).dialogue.end(); ++it2)
		{
			JsonParsing file2 = JsonParsing();

			file2.SetNewJsonString(file2.ValueToObject(file2.GetRootValue()), "Author", (*it2).author.c_str());
			file2.SetNewJsonString(file2.ValueToObject(file2.GetRootValue()), "Line", (*it2).line.c_str());

			file.SetValueToArray(arrayLines, file2.GetRootValue());
		}

		dialogueFile.SetValueToArray(arrayDialogue, file.GetRootValue());
	}

	char* buf;
	uint size = dialogueFile.Save(&buf);

	std::string savePath = DIALOGUES_ASSETS_FOLDER;
	savePath += fileName;
	savePath += ".rgdialogue";

	if (app->fs->Save(savePath.c_str(), buf, size) > 0)
		DEBUG_LOG("Dialogue saved succesfully");
	else
		DEBUG_LOG("Dialogue couldn't be saved");

	savePath = DIALOGUES_FOLDER;
	savePath += fileName;
	savePath += ".rgdialogue";
	app->fs->Save(savePath.c_str(), buf, size);

	RELEASE_ARRAY(buf);
}

void DialogueSystem::Reset()
{
	for (std::vector<Dialogue>::iterator it = dialogues.begin(); it != dialogues.end(); ++it)
	{
		(*it).dialogue.clear();
	}
	dialogues.clear();
}

Dialogue* DialogueSystem::GetDialogueById(int id)
{
	for (std::vector<Dialogue>::iterator it = dialogues.begin(); it != dialogues.end(); ++it)
	{
		if ((*it).id == id)
		{
			return it._Ptr;
		}
	}

	return nullptr;
}

void DialogueSystem::StartDialogue()
{
	currLine = &currDialogue->dialogue.front();
}

void DialogueSystem::NextLine()
{
	bool found = false;
	for (std::vector<DialogueLine>::iterator it = currDialogue->dialogue.begin(); it != currDialogue->dialogue.end(); ++it)
	{
		if (found)
		{
			currLine = &(*it);
			break;
		}

		if ((*it).line == currLine->line)
			found = true;
	}
}

void DialogueSystem::ImportToLibrary()
{
	std::vector<std::string> files;
	app->fs->DiscoverFiles(DIALOGUES_ASSETS_FOLDER, files);

	for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
	{
		std::string assetsPath = DIALOGUES_ASSETS_FOLDER;
		assetsPath += (*it);
		std::string libraryPath = DIALOGUES_FOLDER;
		libraryPath += (*it);
		CopyFileA(assetsPath.c_str(), libraryPath.c_str(), false);
	}
}

void DialogueSystem::SetCurrentDialogueIdXML(int id) {
	currentDialogueID = id;
	for (int i = 0; i < aDialogueXML.size(); i++) {
		if (id == aDialogueXML[i]->id) {
			currDialogXML = aDialogueXML[i];
		}
	}
}
DialogueXML* DialogueSystem::GetCurrentDialogueXML() {
	return currDialogXML;
}
void DialogueSystem::StartDialogueXML() {
	currLineXML = currDialogXML->dialogue.front();
	indexLine = 0;
}
