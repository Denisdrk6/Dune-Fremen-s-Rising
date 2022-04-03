#include "ModuleSceneManager.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleNavMesh.h"
#include "ModuleRenderer3D.h"
#include "ModuleWindow.h"
#include "ModuleInput.h"
#include "ModuleEditor.h"
#include "ResourceManager.h"

#include "TransformComponent.h"

#include "FileSystem.h"
#include "MeshImporter.h"
#include "Primitives.h"
#include "Scene.h"
#include "Bone.h"
#include "Dialogs.h"
#include "ShaderHelpers.h"

#include "imgui/imgui_stdlib.h"
#include <fstream>
#include "Profiling.h"

ModuleSceneManager::ModuleSceneManager(bool startEnabled) : gameState(GameState::NOT_PLAYING), Module(startEnabled)
{
	uint uid = ResourceManager::GetInstance()->CreateResource(ResourceType::SCENE, std::string(""), std::string(""));
	currentScene = std::static_pointer_cast<Scene>(ResourceManager::GetInstance()->GetResource(uid));

	name = "SceneManager";
}

ModuleSceneManager::~ModuleSceneManager()
{
	currentScene = nullptr;
}

bool ModuleSceneManager::Start()
{
	ResourceManager::GetInstance()->ImportResourcesFromLibrary();

	ResourceManager::GetInstance()->ImportAllResources();

	ImportPrimitives();

	ResourceManager::GetInstance()->DeleteResource(currentScene->GetUID());
	currentScene = nullptr;

	currentScene = std::static_pointer_cast<Scene>(ResourceManager::GetInstance()->GetResource(std::string("Assets/Scenes/build.ragnar")));
	currentScene->Load();

	currentScene->Start();

	LoadBuild();

	referenceMap.clear();
	
	
#ifdef DIST
	Play();
#endif

	return true;
}

bool ModuleSceneManager::PreUpdate(float dt)
{
	currentScene->PreUpdate(gameTimer.GetDeltaTime());
	ShortCuts();

	if (gameState == GameState::PLAYING) gameTimer.Start();

	return true;
}

bool ModuleSceneManager::Update(float dt)
{
	if (changeScene)
	{
		currentScene->UnLoad();
		currentScene = scenes[index];
		currentScene->Load();
		newSceneLoaded = true;
		changeScene = false;
		app->navMesh->BakeNavMesh();
	}

	currentScene->Update(gameTimer.GetDeltaTime());
	
	return !exit;
}

bool ModuleSceneManager::PostUpdate()
{
	if (saveScene) WarningWindow();
	if (showBuildMenu) BuildWindow();
	if (showCreateLightSensibleShaderWindow)
		ShowCreateLigthSensibleShaderWindow();
	if (showCreateNotLightSensibleShaderWindow)
		ShowCreateNotLigthSensibleShaderWindow();

	if (gameState == GameState::PLAYING) gameTimer.FinishUpdate();

	currentScene->PostUpdate();

	return true;
}

bool ModuleSceneManager::Draw()
{
	currentScene->Draw();

	return true;
}

bool ModuleSceneManager::CleanUp()
{
	for (int i = 0; i < scenes.size(); ++i)
	{
		scenes[i]->CleanUp();
	}

	return true;
}

void ModuleSceneManager::ImportPrimitives()
{
	std::vector<Vertex> vertices;
	std::vector<unsigned int> indices;
	std::map<std::string, BoneInfo> bonesUid;
	//std::vector<float3> normals;
	//std::vector<float2> texCoords;

	RCube::CreateCube(vertices, indices);
	std::string library;
	ResourceManager::GetInstance()->CreateResource(ResourceType::MESH, std::string("Settings/EngineResources/__Cube.mesh"), library);
	MeshImporter::SaveMesh(library, vertices, indices, bonesUid);

	vertices.clear();
	indices.clear();
	//normals.clear();
	//texCoords.clear();
	library.clear();

	RPyramide::CreatePyramide(vertices, indices);
	ResourceManager::GetInstance()->CreateResource(ResourceType::MESH, std::string("Settings/EngineResources/__Pyramide.mesh"), library);
	MeshImporter::SaveMesh(library, vertices, indices, bonesUid);

	vertices.clear();
	indices.clear();
	//normals.clear();
	//texCoords.clear();
	library.clear();

	RSphere::CreateSphere(vertices, indices);
	ResourceManager::GetInstance()->CreateResource(ResourceType::MESH, std::string("Settings/EngineResources/__Sphere.mesh"), library);
	MeshImporter::SaveMesh(library, vertices, indices, bonesUid);

	vertices.clear();
	indices.clear();
	//normals.clear();
	//texCoords.clear();
	library.clear();

	RCylinder::CreateCylinder(vertices, indices);
	ResourceManager::GetInstance()->CreateResource(ResourceType::MESH, std::string("Settings/EngineResources/__Cylinder.mesh"), library);
	MeshImporter::SaveMesh(library, vertices, indices, bonesUid);

	vertices.clear();
	indices.clear();
}

void ModuleSceneManager::LoadBuild()
{
	char* buffer = nullptr;

	if (app->fs->Load("Settings/EngineResources/build.json", &buffer) > 0)
	{
		scenes.clear();
		JsonParsing json = JsonParsing(buffer);

		int size = json.GetJsonNumber("ScenesSize");

		for (int i = 0; i < size; ++i)
		{
			std::string name = "Scene" + std::to_string(i);
			std::string sceneName = json.GetJsonString(name.c_str());

			std::vector<std::shared_ptr<Scene>> scenesVec = ResourceManager::GetInstance()->GetScenes();

			for (int j = 0; j < scenesVec.size(); ++j)
			{
				if (scenesVec[j]->GetName() == sceneName)
				{
					AddScene(scenesVec[j]);
					break;
				}
			}
		}
	}

	RELEASE_ARRAY(buffer);
}

void ModuleSceneManager::SaveBuild()
{
	JsonParsing json;

	json.SetNewJsonNumber(json.ValueToObject(json.GetRootValue()), "ScenesSize", scenes.size());

	for (int i = 0; i < scenes.size(); ++i)
	{
		std::string name = "Scene" + std::to_string(i);
		json.SetNewJsonString(json.ValueToObject(json.GetRootValue()), name.c_str(), scenes[i]->GetName().c_str());
	}

	char* buffer = nullptr;

	int size = json.Save(&buffer);

	app->fs->Save("Settings/EngineResources/build.json", buffer, size);
}

void ModuleSceneManager::NewScene()
{
	currentScene->NewScene();
}

void ModuleSceneManager::AddScene(std::shared_ptr<Scene> newScene)
{
	scenes.push_back(newScene);
}

void ModuleSceneManager::DeleteScene(std::shared_ptr<Scene> scene)
{
	for (int i = 0; i < scenes.size(); ++i)
	{
		if (scenes[i] == scene)
		{
			scenes.erase(scenes.begin() + i);
			break;
		}
	}
}

void ModuleSceneManager::ChangeScene(const char* sceneName)
{
	if (currentScene->GetAssetsPath() == "")
	{
		ResourceManager::GetInstance()->DeleteResource(currentScene->GetUID());
	}
	currentScene = std::static_pointer_cast<Scene>(ResourceManager::GetInstance()->LoadResource(std::string(sceneName)));
}

void ModuleSceneManager::NextScene()
{
	if (index == scenes.size() - 1) index = 0;
	else ++index;
	changeScene = true;
}

void ModuleSceneManager::NextScene(const char* name)
{
	for (int i = 0; i < scenes.size(); ++i)
	{
		if (scenes[i]->GetName() == name)
		{
			index = i;
			changeScene = true;
			break;
		}
	}
}

void ModuleSceneManager::Play()
{
	DEBUG_LOG("Saving Scene");

	JsonParsing sceneFile;

	sceneFile = sceneFile.SetChild(sceneFile.GetRootValue(), "Scene");
	JSON_Array* array = sceneFile.SetNewJsonArray(sceneFile.GetRootValue(), "Game Objects");
	currentScene->GetRoot()->OnSave(sceneFile, array);

	char* buf;
	uint size = sceneFile.Save(&buf);

	if (app->fs->Save(SCENES_LIBRARY_FOLDER "scenePlay.ragnar", buf, size) > 0)
		DEBUG_LOG("Scene saved succesfully");
	else
		DEBUG_LOG("Scene couldn't be saved");

	RELEASE_ARRAY(buf);

	gameState = GameState::PLAYING;
	gameTimer.ResetTimer();

	if (currentScene != scenes[index])
	{
		currentScene->UnLoad();
		currentScene = scenes[index];
		currentScene->Load();
		lastIndex = index;
		newSceneLoaded = true;
	}
}

void ModuleSceneManager::Stop()
{
	app->renderer3D->ClearPointLights();
	app->renderer3D->ClearSpotLights();

	currentScene->UnLoad();
	currentScene = scenes[lastIndex];
	currentScene->LoadScene("Library/Scenes/scenePlay.ragnar");
	app->fs->RemoveFile("Library/Scenes/scenePlay.ragnar");
	currentScene->GetQuadtree().Clear();
	currentScene->GetQuadtree().Create(AABB(float3(-200, -50, -200), float3(200, 50, 200)));
	gameState = GameState::NOT_PLAYING;

	gameTimer.SetTimeScale(1.0f);
	index = 0;
}

void ModuleSceneManager::Pause()
{
	gameTimer.SetDesiredDeltaTime(0.0f);
	gameState = GameState::PAUSE;
}

void ModuleSceneManager::Resume()
{
	gameTimer.SetDesiredDeltaTime(0.016f);
	gameState = GameState::PLAYING;
}

void ModuleSceneManager::ShortCuts()
{
	if (app->input->GetKey(SDL_SCANCODE_LCTRL) == KeyState::KEY_REPEAT)
	{
		if (app->input->GetKey(SDL_SCANCODE_LSHIFT) == KeyState::KEY_REPEAT)
		{
			if (app->input->GetKey(SDL_SCANCODE_S) == KeyState::KEY_DOWN)
			{
				std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
				if (!filePath.empty()) currentScene.get()->SaveScene(filePath.c_str());
			}
			else if (app->input->GetKey(SDL_SCANCODE_F) == KeyState::KEY_DOWN)
			{
				if (app->editor->GetGO()) app->editor->GetGO()->GetComponent<TransformComponent>()->AlignWithView();
			}
			else if (app->input->GetKey(SDL_SCANCODE_G) == KeyState::KEY_DOWN)
			{
				if (app->editor->GetGO()) currentScene.get()->CreateGameObjectParent("GameObjectParent", app->editor->GetGO());
			}
			else if (app->input->GetKey(SDL_SCANCODE_N) == KeyState::KEY_DOWN)
			{
				if (app->editor->GetGO()) currentScene.get()->CreateGameObject(app->editor->GetGO());
				else currentScene.get()->CreateGameObject(nullptr);
			}
		}
		else if (app->input->GetKey(SDL_SCANCODE_N) == KeyState::KEY_DOWN)
		{
			saveScene = true;
		}
		else if (app->input->GetKey(SDL_SCANCODE_O) == KeyState::KEY_DOWN)
		{
			std::string filePath = Dialogs::OpenFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
			if (!filePath.empty())
			{
				ChangeScene(filePath.c_str());
			}
		}
		else if (app->input->GetKey(SDL_SCANCODE_S) == KeyState::KEY_DOWN)
		{
			if (currentScene.get()->GetAssetsPath().empty())
			{
				std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
				if (!filePath.empty()) currentScene.get()->SaveScene(filePath.c_str());
			}
			else currentScene.get()->SaveScene(currentScene.get()->GetAssetsPath().c_str());
		}
	}
	else if (app->input->GetKey(SDL_SCANCODE_LALT) == KeyState::KEY_REPEAT)
	{
		if (app->input->GetKey(SDL_SCANCODE_LSHIFT) == KeyState::KEY_REPEAT)
		{
			if (app->input->GetKey(SDL_SCANCODE_F) == KeyState::KEY_DOWN)
			{
				if(app->editor->GetGO()) app->editor->GetGO()->GetComponent<TransformComponent>()->AlignViewWithSelected();
			}
			else if (app->input->GetKey(SDL_SCANCODE_N) == KeyState::KEY_DOWN)
			{
				if (app->editor->GetGO()) currentScene.get()->CreateGameObjectChild("GameObjectChild", app->editor->GetGO());
			}
		}		
	}
}

void ModuleSceneManager::WarningWindow()
{
	ImGui::OpenPopup("Warning");
	if (ImGui::BeginPopupModal("Warning"))
	{
		float offset = ImGui::GetWindowContentRegionMax().x / 2 - ImGui::CalcTextSize("New Scene").x / 2;
		ImGui::SetCursorPosX(offset);
		ImGui::Text(ICON_FA_EXCLAMATION_TRIANGLE); ImGui::SameLine();
		ImGui::Text("New Scene");

		ImGui::TextWrapped("Do you want to save the changes you made in: ");
		std::string dir = currentScene.get()->GetAssetsPath();
		if (!dir.empty())
		{
			dir = dir.substr(dir.find("Output\\") + 7, dir.length());
		}
		ImGui::TextWrapped("%s", dir.empty() ? "Untitled" : dir.c_str());
		ImGui::NewLine();

		offset = ImGui::GetWindowContentRegionMax().x * 0.5 - ImGui::CalcTextSize("SAVE Don't Save Cancel").x * 0.5;
		ImGui::SetCursorPosX(offset);

		if (ImGui::Button("Save"))
		{
			if (currentScene.get()->GetAssetsPath().empty())
			{
				std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
				if (!filePath.empty()) currentScene.get()->SaveScene(filePath.c_str());
			}
			else
			{
				currentScene.get()->SaveScene(currentScene.get()->GetAssetsPath().c_str());
			}
			currentScene.get()->NewScene();
			saveScene = false;
		}
		ImGui::SameLine();

		if (ImGui::Button("Don't Save"))
		{
			currentScene.get()->NewScene();
			saveScene = false;
		}
		ImGui::SameLine();

		if (ImGui::Button("Cancel")) saveScene = false;
	}
	ImGui::EndPopup();
}

void ModuleSceneManager::BuildWindow()
{
	static bool addScene = false;
	ImGui::Begin("Build", &showBuildMenu);
	ImGui::BeginChild("Scenes");

	for (int i = 0; i < scenes.size(); ++i)
	{
		int flags = ImGuiTreeNodeFlags_Leaf;
		flags |= sceneSelected == scenes[i] ? ImGuiTreeNodeFlags_Selected : 0;
		ImGui::TreeNodeEx(scenes[i]->GetName().c_str(), flags);
		if (ImGui::IsItemClicked())
		{
			sceneSelected = scenes[i];
		}
		ImGui::TreePop();
	}

	if (ImGui::Button("+"))
	{
		addScene = true;
	}
	ImGui::SameLine();
	if (ImGui::Button("-"))
	{
		DeleteScene(sceneSelected);
	}

	if (addScene)
	{
		ImGui::Begin("Add Scene");

		std::vector<std::shared_ptr<Scene>> scenes = ResourceManager::GetInstance()->GetScenes();
		static std::shared_ptr<Scene> scene = nullptr;

		for (int i = 0; i < scenes.size(); ++i)
		{
			int flags = ImGuiTreeNodeFlags_Leaf;
			flags |= scene == scenes[i] ? ImGuiTreeNodeFlags_Selected : 0;
			ImGui::TreeNodeEx(scenes[i]->GetName().c_str(), flags);
			if (ImGui::IsItemClicked())
			{
				scene = scenes[i];
			}
			ImGui::TreePop();
		}

		if (ImGui::Button("Add") && scene != nullptr)
		{
			AddScene(scene);
			SaveBuild();
			scene = nullptr;
			addScene = false;
		}

		ImGui::End();
	}

	ImGui::EndChild();

	ImGui::End();
}

void ModuleSceneManager::ShowCreateLigthSensibleShaderWindow()
{
	ImGui::Begin("Create Shader", &showCreateLightSensibleShaderWindow);
	ImGui::Dummy({ 10,2 });

	ImGui::Text("Assets/Resources/Shaders/");
	ImGui::SameLine();

	static std::string name;
	ImGui::PushItemWidth(ImGui::GetWindowContentRegionMax().x - ImGui::CalcTextSize("Assets/Resources/Shaders/").x - 15);
	ImGui::InputText("Shader Name", &name);
	ImGui::PopItemWidth();

	if (ImGui::Button("Create", { 50,25 }))
	{
		if (!name.empty())
		{
			std::string path = "Assets/Resources/Shaders/" + name + ".shader";

			std::ofstream file;
			file.open(path);
			file << ShaderSources::lightSensibleShaderSource;
			file.close();

			ResourceManager::GetInstance()->CreateResource(ResourceType::SHADER, path, std::string());
			showCreateLightSensibleShaderWindow = false;
			name.clear();
		}
	}
	ImGui::End();
}

void ModuleSceneManager::ShowCreateNotLigthSensibleShaderWindow()
{
	ImGui::Begin("Create Shader", &showCreateNotLightSensibleShaderWindow);
	ImGui::Dummy({ 10,2 });

	ImGui::Text("Assets/Resources/Shaders/");
	ImGui::SameLine();

	static std::string name;
	ImGui::PushItemWidth(ImGui::GetWindowContentRegionMax().x - ImGui::CalcTextSize("Assets/Resources/Shaders/").x - 15);
	ImGui::InputText("Shader Name", &name);
	ImGui::PopItemWidth();

	if (ImGui::Button("Create", { 50,25 }))
	{
		if (!name.empty())
		{
			std::string path = "Assets/Resources/Shaders/" + name + ".shader";

			std::ofstream file;
			file.open(path);
			file << ShaderSources::notLightSensibleShaderSource;
			file.close();

			ResourceManager::GetInstance()->CreateResource(ResourceType::SHADER, path, std::string());

			showCreateNotLightSensibleShaderWindow = false;
			name.clear();
		}
	}
	ImGui::End();
}