
#include "Application.h"
#include "Globals.h"

#include "ModuleInput.h"
#include "ScriptComponent.h"
#include "GameObject.h"
#include "FileSystem.h"
#include "ModuleSceneManager.h"
#include "InputActionComponent.h"
#include "InputActionMenu.h"
#include "MonoManager.h"

#include "IconsFontAwesome5.h"
#include "Profiling.h"
#include <metadata\debug-helpers.h>

InputActionComponent::InputActionComponent(GameObject* own)
{
	owner = own;
	type = ComponentType::INPUT_ACTION;
	assetWindowActive = false;
	currentPreset = nullptr;
}

InputActionComponent::~InputActionComponent()
{
}

void InputActionComponent::OnEditor()
{
	ImGui::PushID(this);
	if (ImGui::CollapsingHeader(ICON_FA_GAMEPAD" Player Input"))
	{
		ImGui::Text("Actions:");
		ImGui::SameLine();
		std::string text = currentAssetName + (std::string)ICON_FA_EDIT;
		auto posX = (ImGui::GetCursorPosX() + ImGui::GetColumnWidth() - ImGui::CalcTextSize(text.c_str()).x
			- ImGui::GetScrollX() - 2 * ImGui::GetStyle().ItemSpacing.x);
		if (posX > ImGui::GetCursorPosX())
			ImGui::SetCursorPosX(posX);
		ImGui::Text(currentAssetName.c_str());
		ImGui::SameLine();
		if (ImGui::Button(ICON_FA_EDIT))
		{
			assetWindowActive = true;
			inputAssetsList.clear();
			LoadAllInputAssets("Library");
		}

		if (currentActionMaps.size() > 0)
		{
			if (ImGui::TreeNodeEx("Events", ImGuiTreeNodeFlags_DefaultOpen))
			{
				for (size_t i = 0; i < currentActionMaps.size(); i++)
				{
					ImGui::PushID(i);
					if (ImGui::TreeNodeEx(currentActionMaps[i]->GetName().c_str(), ImGuiTreeNodeFlags_DefaultOpen))
					{
						for (size_t j = 0; j < currentActionMaps[i]->GetActions()->size(); j++)
						{
							ImGui::PushID(j);
							if (ImGui::TreeNodeEx(currentActionMaps[i]->GetActions()->at(j)->GetName().c_str(), ImGuiTreeNodeFlags_DefaultOpen))
							{
								if (ImGui::BeginCombo("Script", scriptsNameList[i][j].c_str()))
								{
									const char* scriptName;
									for (int k = 0; k < app->moduleMono->userScripts.size(); k++)
									{
										scriptName = mono_class_get_name(app->moduleMono->userScripts[k]);
										if (ImGui::Selectable(scriptName))
										{
											scriptsNameList[i][j] = scriptName;
											scriptsMethodsList[i][j].clear();

											app->moduleMono->DebugAllMethodsShortName(USER_SCRIPTS_NAMESPACE, scriptName, scriptsMethodsList[i][j]);
										}
									}
									ImGui::EndCombo();
								}
								const char* methodName = nullptr;
								if (scriptsMethodsList[i][j].size() > 0)
								{
									methodName = scriptsMethodsList[i][j][currentMethodList[i][j]].c_str();
								}
								else
								{
									methodName = "";
								}
								if (ImGui::BeginCombo("Method", methodName))
								{
									const char* methodName;
									for (int actualMethod = 0; actualMethod < scriptsMethodsList[i][j].size(); actualMethod++)
									{
										methodName = scriptsMethodsList[i][j][actualMethod].c_str();
										if (ImGui::Selectable(methodName))
										{
											currentMethodList[i][j] = actualMethod;

											MonoClass* klass = mono_class_from_name(app->moduleMono->image, USER_SCRIPTS_NAMESPACE, scriptsNameList[i][j].c_str());
											std::string methodN = ":" + std::string(methodName);
											MonoMethodDesc* mdesc = mono_method_desc_new(methodN.c_str(), false);
											monoMethodList[j] = mono_method_desc_search_in_class(mdesc, klass);
											mono_method_desc_free(mdesc);
										}
									}
									ImGui::EndCombo();
								}
								ImGui::TreePop();
							}
							ImGui::PopID();
						}
						ImGui::TreePop();
					}
					ImGui::PopID();
				}
				ImGui::TreePop();
			}
		}
	}
	ImGui::PopID();

	if (assetWindowActive)
	{
		ImGui::Begin("Input Action Assets", &assetWindowActive, ImGuiWindowFlags_NoDocking | ImGuiWindowFlags_NoCollapse);
		ImVec2 winPos = ImGui::GetWindowPos();
		ImVec2 size = ImGui::GetWindowSize();
		ImVec2 mouse = ImGui::GetIO().MousePos;
		if (!(mouse.x < winPos.x + size.x && mouse.x > winPos.x &&
			mouse.y < winPos.y + size.y && mouse.y > winPos.y))
		{
			if (ImGui::GetIO().MouseClicked[0]) assetWindowActive = false;
		}

		if (ImGui::BeginTable("table3", 3))
		{
			for (int item = 0; item < inputAssetsList.size(); item++)
			{
				ImGui::TableNextColumn();
				std::string assetName = inputAssetsList[item];
				app->fs->GetFilenameWithoutExtension(assetName);
				if (ImGui::Selectable(assetName.c_str()))
				{
					assetWindowActive = false;
					LoadInputAsset(inputAssetsList[item].c_str());
					currentPreset = currentActionMaps[0];
				}
			}
			ImGui::EndTable();
		}

		ImGui::End();
	}
}

bool InputActionComponent::Update(float dt)
{
	bool ret = true;

	//Check if bindings have been pressed
	if (app->sceneManager->GetGameState() == GameState::PLAYING)
	{
		for (std::vector<std::shared_ptr<Actions>>::iterator action = currentPreset->GetActions()->begin(); action < currentPreset->GetActions()->end(); action++)
		{
			int j = action - currentPreset->GetActions()->begin();
			for (std::vector<int>::iterator bind = (*action)->GetBindings()->begin(); bind < (*action)->GetBindings()->end(); bind++)
			{
				if (app->input->GetKey(*bind) == KeyState::KEY_DOWN && monoMethodList[j] != nullptr)
				{
					MonoObject* exec = nullptr;
					uint32_t objOwner = dynamic_cast<ScriptComponent*>(owner->GetComponent(ComponentType::SCRIPT))->GetScriptGO();
					mono_runtime_invoke(monoMethodList[j], mono_gchandle_get_target(objOwner), NULL, &exec);
				}
			}
		}
	}

	return ret;
}

bool InputActionComponent::OnLoad(JsonParsing& node)
{
	LoadInputAsset(node.GetJsonString("Path"));

	currentAssetName = node.GetJsonString("Path");

	JSON_Array* actionMapsList = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "Action Maps List");
	JsonParsing mainFile = node.GetJsonArrayValue(actionMapsList, 0);
	for (size_t i = 0; i < currentActionMaps.size(); i++)
	{
		JSON_Array* iArr = mainFile.GetJsonArray(mainFile.ValueToObject(mainFile.GetRootValue()), std::to_string(i).c_str());
		JsonParsing aM = mainFile.GetJsonArrayValue(iArr, 0);
		for (size_t j = 0; j < currentActionMaps[i]->GetActions()->size(); j++)
		{
			JSON_Array* jArr = aM.GetJsonArray(aM.ValueToObject(aM.GetRootValue()), std::to_string(j).c_str());
			JsonParsing a = aM.GetJsonArrayValue(jArr, 0);
			scriptsNameList[i][j] = a.GetJsonString("Script");
			currentMethodList[i][j] = a.GetJsonNumber("Method");
			if (scriptsNameList[i][j] != "")
			{
				app->moduleMono->DebugAllMethodsShortName(USER_SCRIPTS_NAMESPACE, a.GetJsonString("Script"), scriptsMethodsList[i][j]);
				std::string methodName = scriptsMethodsList[i][j][currentMethodList[i][j]];
				MonoClass* klass = mono_class_from_name(app->moduleMono->image, USER_SCRIPTS_NAMESPACE, a.GetJsonString("Script"));
				std::string methodN = ":" + methodName;
				MonoMethodDesc* mdesc = mono_method_desc_new(methodN.c_str(), false);
				monoMethodList[j] = mono_method_desc_search_in_class(mdesc, klass);
				mono_method_desc_free(mdesc);
			}
		}
	}
	currentPreset = currentActionMaps[0];

	return true;
}

bool InputActionComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Path", currentAssetPath.c_str());

	JSON_Array* newArray = file.SetNewJsonArray(file.GetRootValue(), "Action Maps List");
	JsonParsing actionMapFile = JsonParsing();
	for (size_t i = 0; i < currentActionMaps.size(); ++i)
	{
		JSON_Array* actionMaps = actionMapFile.SetNewJsonArray(actionMapFile.GetRootValue(), std::to_string(i).c_str());
		JsonParsing actionsFile = JsonParsing();
		for (size_t j = 0; j < currentActionMaps[i]->GetActions()->size(); ++j)
		{
			JSON_Array* actions = actionsFile.SetNewJsonArray(actionsFile.GetRootValue(), std::to_string(j).c_str());
			JsonParsing actionsInfoFile = JsonParsing();
			actionsInfoFile.SetNewJsonString(actionsInfoFile.ValueToObject(actionsInfoFile.GetRootValue()), "Script", scriptsNameList[i][j].c_str());
			actionsInfoFile.SetNewJsonNumber(actionsInfoFile.ValueToObject(actionsInfoFile.GetRootValue()), "Method", currentMethodList[i][j]);
			actionsFile.SetValueToArray(actions, actionsInfoFile.GetRootValue());
		}
		actionMapFile.SetValueToArray(actionMaps, actionsFile.GetRootValue());
	}
	file.SetValueToArray(newArray, actionMapFile.GetRootValue());

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

bool InputActionComponent::LoadInputAsset(const char* path)
{
	currentActionMaps.clear();
	scriptsNameList.clear();
	scriptsMethodsList.clear();
	currentMethodList.clear();
	monoMethodList.clear();

	JsonParsing sceneFile = JsonParsing();

	if (sceneFile.ParseFile(path) > 0)
	{
		JSON_Array* jsonArray = sceneFile.GetJsonArray(sceneFile.ValueToObject(sceneFile.GetRootValue()), "Action Maps");

		size_t size = sceneFile.GetJsonArrayCount(jsonArray);
		scriptsNameList.resize(size);
		scriptsMethodsList.resize(size);
		currentMethodList.resize(size);
		for (int i = 0; i < size; ++i)
		{
			JsonParsing go = sceneFile.GetJsonArrayValue(jsonArray, i);
			std::shared_ptr<ActionMaps> aM(new ActionMaps());
			aM->OnLoad(go);
			currentActionMaps.push_back(aM);
			scriptsNameList[i].resize(aM->GetActions()->size());
			scriptsMethodsList[i].resize(aM->GetActions()->size());
			currentMethodList[i].resize(aM->GetActions()->size());
			monoMethodList.resize(aM->GetActions()->size());
		}
		currentAssetPath = path;
		currentAssetName = path;
		app->fs->GetFilenameWithExtension(currentAssetName);
	}
	else
	{
		DEBUG_LOG("Input Asset couldn't be loaded");
	}
	return false;
}

void InputActionComponent::LoadAllInputAssets(const char* folder)
{
	std::vector<std::string> files;
	std::vector<std::string> dirs;
	std::string path = folder + std::string("/");
	app->fs->DiscoverFilesAndDirs(path.c_str(), files, dirs);
	for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
	{
		if ((*it).find(".inputaction") != std::string::npos)
		{
			inputAssetsList.push_back(*it);
		}
	}
	for (std::vector<std::string>::iterator it = dirs.begin(); it != dirs.end(); ++it)
	{
		LoadAllInputAssets((*it).c_str());
	}
}

void InputActionComponent::SetActionMap(int index)
{
	if (currentActionMaps[index] != nullptr)
	{
		currentPreset = currentActionMaps[index];
	}
}