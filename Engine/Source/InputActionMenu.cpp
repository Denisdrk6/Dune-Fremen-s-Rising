
#include "Application.h"
#include "ModuleInput.h"
#include "FileSystem.h"
#include "InputActionMenu.h"
#include "IconsFontAwesome5.h"
#include "Globals.h"
#include <string>

#include "SDL.h"

#define IMGUI_DEFINE_MATH_OPERATORS
#include "Imgui/imgui_internal.h"
#include "Imgui/imgui_stdlib.h"
#include "Profiling.h"

InputActionMenu::InputActionMenu() : Menu(false, "Input Action")
{
	actionMaps.push_back(std::shared_ptr<ActionMaps>(new ActionMaps()));
	currentMap = NULL;
	currentAction = NULL;
	currentBinding = NULL;
}

InputActionMenu::~InputActionMenu()
{
	actionMaps.clear();
}

bool InputActionMenu::Start()
{
	bool ret = true;

#ifdef DIST

#else
	ImportToLibrary();
#endif

	return ret;
}

bool InputActionMenu::Update(float dt)
{
	bool ret = true;
	
	ImGui::Begin(ICON_FA_KEYBOARD" Input Actions", &active, ImGuiWindowFlags_MenuBar | ImGuiWindowFlags_NoCollapse);

	if (ImGui::IsItemClicked())
	{
		ImGui::Text("XD");
	}

	ImGui::PushStyleVar(ImGuiStyleVar_ItemSpacing, ImVec2(15.0f, 0.0f));
	if (ImGui::BeginMenuBar())
	{
		ImGui::Text("Asset name: %s", currentInputAssetName.c_str());

		if (ImGui::Button("Save Asset"))
		{
			SaveInputActionFile(currentInputAssetPath.c_str());
		}

		ImGui::EndMenuBar();
	}
	ImGui::PopStyleVar();

	ImGui::Columns(3, nullptr);
	
	ColumnActionMaps();

	ImGui::NextColumn();

	ColumnActions();

	ImGui::NextColumn();

	ColumnProperties();

	ImGui::End();
	return ret;
}

void InputActionMenu::ColumnProperties()
{
	ImGui::BeginChild("Properties");
	ImGui::Text("Properties");
	ImGui::Separator();
	if (actionMaps.at(currentMap).get()->GetActions()->at(currentAction).get()->GetBindings() != nullptr)
	{
		ImGui::Text("Binding: %s", currentBindingItem);
		ImGui::Text("Path: ");
		ImGui::SameLine();
		if (ImGui::BeginCombo("##combo", currentBindingItem))
		{
			for (int n = 0; n < IM_ARRAYSIZE(app->input->keyNameList); n++)
			{
				if ((app->input->keyNameList[n] != NULL) && (app->input->keyNameList[n][0] != '\0'))
				{
					bool is_selected = (currentBindingItem == app->input->keyNameList[n]);
					if (ImGui::Selectable(app->input->keyNameList[n], is_selected))
					{
						currentBindingItem = app->input->keyNameList[n];
						actionMaps.at(currentMap).get()->GetActions()->at(currentAction).get()->SetBinding(currentBinding, n);
					}
					if (is_selected)
						ImGui::SetItemDefaultFocus();
				}
			}
			ImGui::EndCombo();
		}
	}
	
	ImGui::EndChild();
}

void InputActionMenu::ColumnActions()
{
	ImGui::BeginChild("Actions");
	ImGui::Text("Actions");
	ImGui::SameLine();
	std::string text = ICON_FA_PLUS;
	auto posX = (ImGui::GetCursorPosX() + ImGui::GetColumnWidth() - ImGui::CalcTextSize(text.c_str()).x
		- ImGui::GetScrollX() - 2 * ImGui::GetStyle().ItemSpacing.x);
	if (posX > ImGui::GetCursorPosX())
		ImGui::SetCursorPosX(posX);
	if (ImGui::Button(text.c_str()))
	{
		ImGui::OpenPopup("AddAction");
		
	}
	if (ImGui::BeginPopup("AddAction"))
	{
		if (ImGui::Selectable("Add Action"))
		{
			actionMaps[currentMap].get()->GetActions()->push_back(std::shared_ptr<Actions>(new Actions()));
		}
		if (ImGui::Selectable("Add Binding"))
		{
			actionMaps[currentMap].get()->GetActions()->at(currentAction).get()->AddBinding(0);
		}
		ImGui::EndPopup();
	}
	ImGui::Separator();
	for (size_t i = 0; i < actionMaps[currentMap].get()->GetActions()->size(); i++)
	{
		ImGui::PushID(i);
		bool open = ImGui::TreeNodeEx(actionMaps[currentMap].get()->GetActions()->at(i)->GetName().c_str(),
			ImGuiTreeNodeFlags_FramePadding | ImGuiTreeNodeFlags_DefaultOpen | (actionMaps[currentMap].get()->GetActions()->at(currentAction) == actionMaps[currentMap].get()->GetActions()->at(i) ? ImGuiTreeNodeFlags_Selected : 0));

		if (ImGui::IsMouseDoubleClicked(0) && ImGui::IsItemHovered())
		{
			showActionNamePopUp = true;
		}

		if (ImGui::IsItemClicked())
		{
			currentAction = i;
		}

		if (open)
		{
			for (size_t j = 0; j < actionMaps[currentMap].get()->GetActions()->at(i).get()->GetBindings()->size(); j++)
			{
				ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags_Leaf | ImGuiTreeNodeFlags_FramePadding;
				if (actionMaps.at(currentMap).get()->GetActions()->at(currentAction) == actionMaps.at(currentMap).get()->GetActions()->at(i))
				{
					if (actionMaps.at(currentMap).get()->GetActions()->at(currentAction).get()->GetBindings()->at(currentBinding) == actionMaps.at(currentMap).get()->GetActions()->at(i).get()->GetBindings()->at(j))
					{
						flags |= ImGuiTreeNodeFlags_Selected;
					}
				}
				
				ImGui::TreeNodeEx(SDL_GetScancodeName((SDL_Scancode)actionMaps[currentMap].get()->GetActions()->at(i).get()->GetBindings()->at(j)),
					flags);

				if (ImGui::IsItemClicked())
				{
					currentAction = i;
					currentBinding = j;
				}
				ImGui::TreePop();
			}
			ImGui::TreePop();
		}

		ImGui::PopID();
	}
	if (showActionNamePopUp)
	{
		ImGui::OpenPopup("SetName");
		if (ImGui::BeginPopup("SetName"))
		{
			static char name[50] = "\0";
			ImGui::InputText("##Name", name, sizeof(char) * 50);

			if (ImGui::Button("Set"))
			{
				actionMaps[currentMap]->GetActions()->at(currentAction)->SetName(name);
				showActionNamePopUp = false;
			}
			ImGui::EndPopup();
		}
	}
	ImGui::EndChild();
}

void InputActionMenu::ColumnActionMaps()
{
	ImGui::BeginChild("Action Maps");
	ImGui::Text("Action Maps");
	ImGui::SameLine();
	std::string text = ICON_FA_PLUS;
	auto posX = (ImGui::GetCursorPosX() + ImGui::GetColumnWidth() - ImGui::CalcTextSize(text.c_str()).x
		- ImGui::GetScrollX() - 2 * ImGui::GetStyle().ItemSpacing.x);
	if (posX > ImGui::GetCursorPosX())
		ImGui::SetCursorPosX(posX);
	if (ImGui::Button(text.c_str()))
	{
		actionMaps.push_back(std::shared_ptr<ActionMaps>(new ActionMaps()));
	}
	ImGui::Separator();
	for (size_t i = 0; i < actionMaps.size(); i++)
	{
		bool open = ImGui::TreeNodeEx(actionMaps[i]->GetName().c_str(),
			(actionMaps[currentMap] == actionMaps[i] ? ImGuiTreeNodeFlags_Selected : 0) | ImGuiTreeNodeFlags_Leaf | ImGuiTreeNodeFlags_FramePadding);

		if (ImGui::IsMouseDoubleClicked(0) && ImGui::IsItemHovered())
		{
			showActionMapNamePopUp = true;
		}

		if (ImGui::IsItemClicked())
		{
			currentMap = i;
			currentAction = 0;
			currentBinding = 0;
		}

		if (open)
		{
			ImGui::TreePop();
		}
	}
	if (showActionMapNamePopUp)
	{
		ImGui::OpenPopup("SetName");
		if (ImGui::BeginPopup("SetName"))
		{
			static char name[50] = "\0";
			ImGui::InputText("##Name", name, sizeof(char) * 50);

			if (ImGui::Button("Set"))
			{
				actionMaps[currentMap]->SetName(name);
				showActionMapNamePopUp = false;
			}
			ImGui::EndPopup();
		}
	}
	ImGui::EndChild();
}

bool InputActionMenu::CleanUp()
{
	bool ret = true;

	return ret;
}

void InputActionMenu::ResetCurrent()
{
	currentMap = 0;
	currentAction = 0;
	currentBinding = 0;
	currentBindingItem = NULL;
}

bool InputActionMenu::SaveInputActionFile(const char* path)
{
	DEBUG_LOG("Saving Input Action: %s", path);

	std::string fileName = path;
	app->fs->GetFilenameWithoutExtension(fileName);

	JsonParsing file;

	file = file.SetChild(file.GetRootValue(), "Input Action");
	JSON_Array* array = file.SetNewJsonArray(file.GetRootValue(), "Action Maps");
	for (int i = 0; i < actionMaps.size(); i++)
	{
		actionMaps[i]->OnSave(file, array);
	}

	uint size = file.SaveFile(path);

	if (size > 0)
		DEBUG_LOG("Input Action saved succesfully");
	else
		DEBUG_LOG("Input Action couldn't be saved");

	return true;
}

bool InputActionMenu::LoadInputActionFile(const char* path)
{
	actionMaps.clear();

	JsonParsing sceneFile = JsonParsing();

	if (sceneFile.ParseFile(path) > 0)
	{
		JSON_Array* jsonArray = sceneFile.GetJsonArray(sceneFile.ValueToObject(sceneFile.GetRootValue()), "Action Maps");

		size_t size = sceneFile.GetJsonArrayCount(jsonArray);
		for (int i = 0; i < size; ++i)
		{
			JsonParsing go = sceneFile.GetJsonArrayValue(jsonArray, i);
			std::shared_ptr<ActionMaps> aM(new ActionMaps());
			aM->OnLoad(go);
			actionMaps.push_back(aM);
		}
		currentInputAssetName = path;
		app->fs->GetFilenameWithoutExtension(currentInputAssetName);
		currentInputAssetPath = path;
	}
	else
	{
		DEBUG_LOG("Input Asset couldn't be loaded");
	}
	return false;
}

void InputActionMenu::ImportToLibrary()
{
	std::vector<std::string> files;
	app->fs->DiscoverFiles("Assets/Resources/", files);

	for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
	{
		std::string ext = (*it).substr((*it).find_last_of("."), (*it).length());
		if (ext == ".inputaction")
		{
			std::string assetsPath = "Assets/Resources/";
			assetsPath += (*it);
			std::string libraryPath = LIBRARY_FOLDER;
			libraryPath += (*it);
			CopyFileA(assetsPath.c_str(), libraryPath.c_str(), false);
		}
	}
}

Actions::Actions()
{
	name = "Move";
}

Actions::~Actions()
{

}

void Actions::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Name", name.c_str());

	JSON_Array* newArray = file.SetNewJsonArray(file.GetRootValue(), "Bindings");

	for (int i = 0; i < bindings.size(); i++)
	{
		JsonParsing newFile = JsonParsing();
		newFile.SetNewJsonNumber(newFile.ValueToObject(newFile.GetRootValue()), "Path", bindings[i]);
		file.SetValueToArray(newArray, newFile.GetRootValue());
	}

	node.SetValueToArray(array, file.GetRootValue());
}

void Actions::OnLoad(JsonParsing& node)
{
	name = node.GetJsonString("Name");

	JSON_Array* jsonArray = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "Bindings");
	size_t size = node.GetJsonArrayCount(jsonArray);
	for (int i = 0; i < size; ++i)
	{
		JsonParsing c = node.GetJsonArrayValue(jsonArray, i);
		bindings.push_back(c.GetJsonNumber("Path"));
	}
}

ActionMaps::ActionMaps()
{
	name = "Player";
	actions.push_back(std::shared_ptr<Actions>(new Actions()));
}

ActionMaps::~ActionMaps()
{
	actions.clear();
}

void ActionMaps::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Name", name.c_str());

	JSON_Array* newArray = file.SetNewJsonArray(file.GetRootValue(), "Actions");
	for (int i = 0; i < actions.size(); i++)
	{
		actions[i]->OnSave(file, newArray);
	}
	node.SetValueToArray(array, file.GetRootValue());
}

void ActionMaps::OnLoad(JsonParsing& node)
{
	actions.clear();
	name = node.GetJsonString("Name");

	JSON_Array* jsonArray = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "Actions");

	size_t size = node.GetJsonArrayCount(jsonArray);
	for (int i = 0; i < size; ++i)
	{
		JsonParsing c = node.GetJsonArrayValue(jsonArray, i);
		std::shared_ptr<Actions> a(new Actions());
		a->OnLoad(c);
		actions.push_back(a);
	}
}
