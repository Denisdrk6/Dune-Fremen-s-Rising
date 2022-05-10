#include "Scene.h"
#include "Application.h"
#include "ModuleRenderer3D.h"
#include "Globals.h"

#include "ModuleEditor.h"
#include "ModuleUI.h"
#include "Physics3D.h"
#include "ModuleSceneManager.h"

#include "FileSystem.h"

#include "ResourceManager.h"
#include "AudioManager.h"

//Scripting
#include "ScriptComponent.h"

#include "TransformComponent.h"
#include "MeshComponent.h"

#include <stack>
#include "Profiling.h"

Scene::Scene(uint uid, std::string& assets, std::string& library) : mainCamera(nullptr), resetQuadtree(true), camera(nullptr), player(nullptr), Resource(uid, ResourceType::SCENE, assets, library)
{
	root = new GameObject();
	root->SetName("Untitled");
	if (assets == "")
	{
		name = "";
	}
	else
	{
		name = assets;
		app->fs->GetFilenameWithoutExtension(name);
	}
}

Scene::~Scene()
{
	UnLoad();
}

bool Scene::Start()
{
	RG_PROFILING_FUNCTION("Starting Scene");

	//camera = CreateGameObject(nullptr);
	//camera->CreateComponent(ComponentType::CAMERA);
	//camera->SetName("Camera");
	//camera->CreateComponent(ComponentType::AUDIO_LISTENER);
	//camera->CreateComponent(ComponentType::AUDIO_SOURCE);
	
	//player = CreateGameObject(nullptr);
	//player->CreateComponent(ComponentType::AUDIO_SOURCE);
	//player->SetName("Player");
	//player->tag = "Player";
	
	//qTree.Create(AABB(float3(-200, -50, -200), float3(200, 50, 200)));

	//AkAuxSendValue aEnvs[1];
	//root->GetChilds()[1]->GetChilds()[1]->CreateComponent(ComponentType::AUDIO_REVERB_ZONE);

	//aEnvs[0].listenerID = camera->GetUUID();
	//aEnvs[0].auxBusID = AK::SoundEngine::GetIDFromString(L"ReverbZone");
	//aEnvs[0].fControlValue = 0.0f;
	//
	//if (AK::SoundEngine::SetGameObjectAuxSendValues(camera->GetUUID(), aEnvs, 1) != AK_Success)
	//{
	//	DEBUG_LOG("Couldnt set aux send values");
	//}

	//LoadScene("Assets/Scenes/build.ragnar");

	return true;
}

bool Scene::PreUpdate(float dt)
{
	static bool refresh = true;

	if (refresh)
	{
		resetQuadtree = true;
		refresh = false;
	}

	if (app->sceneManager->GetGameState() == GameState::PLAYING) app->sceneManager->GetTimer().Start();

	return true;
}

bool Scene::Update(float dt)
{
	RG_PROFILING_FUNCTION("Updating Scene");

	if (mainCamera != nullptr) mainCamera->Update(dt);

	for (int i = 0; i < root->GetChilds().size(); ++i)
		if (root->GetChilds()[i]->active)
			root->GetChilds()[i]->Update(dt);

	if (resetQuadtree)
	{
		qTree.Clear();
		qTree.Create(AABB(float3(-400, -50, -400), float3(400, 50, 400)));
		std::stack<GameObject*> objects;

		for (int i = 0; i < root->GetChilds().size(); ++i)
			objects.push(root->GetChilds()[i]);

		while (!objects.empty())
		{
			GameObject* go = objects.top();
			objects.pop();

			qTree.Insert(go);

			for (int i = 0; i < go->GetChilds().size(); ++i)
				objects.push(go->GetChilds()[i]);
		}

		resetQuadtree = false;
	}

	return true;
}

bool Scene::PostUpdate()
{
	if (app->sceneManager->GetGameState() == GameState::PLAYING) app->sceneManager->GetTimer().FinishUpdate();

	return true;
}

bool Scene::Draw(const AABB* shadowsIntersectionAABB)
{
	RG_PROFILING_FUNCTION("Scene PostUpdate");

	std::stack<GameObject*> stack;

	for (int i = 0; i < root->GetChilds().size(); ++i)
		stack.push(root->GetChilds()[i]);

	while (!stack.empty())
	{
		GameObject* go = stack.top();
		stack.pop();

		if (go->GetActive())
		{
			if (go != app->editor->GetGO() && !go->isUI)
			{
				if (app->renderer3D->genShadows)
				{
					if(shadowsIntersectionAABB->Contains(go->GetAABB()))
						go->Draw(nullptr);
				}
				else
				{
					go->Draw(nullptr);
				}

			}

			for (int i = 0; i < go->GetChilds().size(); ++i)
				stack.push(go->GetChilds()[i]);
		}
	}
	//for (int i = 0; i < root->GetChilds().size(); ++i)
	//{
	//	GameObject* go = root->GetChilds()[i];
	//	if (go->GetActive())
	//		go->Draw();
	//}

	return true;
}

bool Scene::CleanUp()
{
	RELEASE(root);

	return true;
}

void Scene::NewScene()
{
	RELEASE(root);

	root = new GameObject();
	root->SetName("Untitled");

	GameObject* camera = CreateGameObject(nullptr);
	camera->CreateComponent(ComponentType::CAMERA);
	camera->SetName("Camera");

	qTree.Create(AABB(float3(-400, -50, -400), float3(400, 50, 400)));

	app->editor->SetGO(nullptr);
}

GameObject* Scene::CreateGameObject(GameObject* parent, bool createTransform, bool begin)
{
	RG_PROFILING_FUNCTION("Creating Game Object");

	GameObject* object = new GameObject();
	if (createTransform) object->CreateComponent(ComponentType::TRANSFORM);
	if (parent != nullptr)
	{
		object->SetParent(parent);
		parent->AddChild(object);
	}
	else
	{
		object->SetParent(root);
		root->AddChild(object, begin);
	}
	
	return object;
}

GameObject* Scene::CreateGameObjectChild(const char* name, GameObject* parent)
{
	GameObject* object = CreateGameObject(parent);
	object->SetName(name);

	return object;
}

GameObject* Scene::CreateGameObjectParent(const char* name, GameObject* child)
{
	GameObject* object = CreateGameObject(child->GetParent());
	object->SetName(name);

	child->GetParent()->RemoveChildren(child->GetParent()->FindChildren(child));
	object->AddChild(child);

	return object;
}

GameObject* Scene::Create3DObject(Object3D type, GameObject* parent)
{
	GameObject* object = CreateGameObject(parent);
	std::string path;

	switch (type)
	{
	case Object3D::CUBE:
		object->SetName("Cube");
		path = "Settings/EngineResources/__Cube.mesh";
		break;
	case Object3D::PYRAMIDE:
		object->SetName("Pyramide");
		path = "Settings/EngineResources/__Pyramide.mesh";
		break;
	case Object3D::SPHERE:
		object->SetName("Sphere");
		path = "Settings/EngineResources/__Sphere.mesh";
		break;
	case Object3D::CYLINDER:
		object->SetName("Cylinder");
		path = "Settings/EngineResources/__Cylinder.mesh";
		break;
	}

	if (!path.empty())
	{
		MeshComponent* mesh = (MeshComponent*)object->CreateComponent(ComponentType::MESH_RENDERER);
		mesh->SetMesh(ResourceManager::GetInstance()->LoadResource(path));
	}

	return object;
}

void Scene::MoveGameObjectUp(GameObject* object)
{
	if (object == root->GetChilds()[0]) return;

	int size = root->GetChilds().size();
	for (int i = 0; i < size; ++i)
	{
		if (root->GetChilds()[i] == object)
		{
			GameObject* aux = root->GetChilds()[i];

			root->GetChilds()[i] = root->GetChilds()[i - 1];
			root->GetChilds()[i - 1] = aux;
			break;
		}
	}
}

void Scene::MoveGameObjectDown(GameObject* object)
{
	int size = root->GetChilds().size() - 1;
	if (object == root->GetChilds()[size]) return;

	for (int i = size; i >= 0; --i)
	{
		if (root->GetChilds()[i] == object)
		{
			GameObject* aux = root->GetChilds()[i];

			root->GetChilds()[i] = root->GetChilds()[i + 1];
			root->GetChilds()[i + 1] = aux;
			break;
		}
	}
}

void Scene::ReparentGameObjects(uint uuid, GameObject* go)
{
	GameObject* gameObj = GetGoByUuid(uuid);
	GameObject* parentObj = gameObj->GetParent();

	parentObj->RemoveChild(gameObj);
	gameObj->SetParent(go);
	go->AddChild(gameObj);

	if (TransformComponent* trans = gameObj->GetComponent<TransformComponent>())
	{
		trans->NewAttachment();
		trans->SetAABB();
	}	
}

void Scene::ReparentGameObjects(GameObject* parent, GameObject* go)
{
	GameObject* parentObj = parent->GetParent();

	parentObj->RemoveChild(parent);
	parent->SetParent(go);
	go->AddChild(parent);

	if (TransformComponent* trans = parent->GetComponent<TransformComponent>())
	{
		trans->NewAttachment();
		trans->SetAABB();
	}
}

void Scene::Load()
{
	RELEASE(root);
	if (root == nullptr)
	{
		//LoadScene(assetsPath.c_str());
		LoadScene(libraryPath.c_str(), true);
	}
}

void Scene::UnLoad()
{
	qTree.Clear();
	RELEASE(root);
	mainCamera = nullptr;
	camera = nullptr;
	player = nullptr;
}

GameObject* Scene::GetGoByUuid(double uuid) const
{
	std::stack<GameObject*> goStack;
	goStack.push(root);

	while (!goStack.empty())
	{
		GameObject* go = goStack.top();
		goStack.pop();
		if (go->GetUUID() == uuid)	return go;
		else
		{
			for (int i = 0; i < go->GetChilds().size(); ++i)
			{
				goStack.push(go->GetChilds()[i]);
			}
		}
	}

	return nullptr;
}

bool Scene::LoadScene(const char* path, bool fromLibrary)
{
	RG_PROFILING_FUNCTION("Loading Scene");

	DEBUG_LOG("Loading Scene");

	RELEASE(root);
	app->userInterface->UIGameObjects.clear();

	if (std::string(path).find("scenePlay") == -1)
	{
		if (fromLibrary)
		{
			libraryPath = path;
			//name = libraryPath;
		}
		else
		{
			assetsPath = path;
			name = assetsPath;
		}
	}
	

	JsonParsing sceneFile = JsonParsing();


	if(!fromLibrary)
		app->fs->GetFilenameWithoutExtension(name);

	std::string p = fromLibrary ? libraryPath : assetsPath;
	if (sceneFile.ParseFile(p.c_str()) > 0)
	{
		JSON_Array* jsonArray = sceneFile.GetJsonArray(sceneFile.ValueToObject(sceneFile.GetRootValue()), "Game Objects");

		size_t size = sceneFile.GetJsonArrayCount(jsonArray);
		for (int i = 0; i < size; ++i)
		{
			JsonParsing go = sceneFile.GetJsonArrayValue(jsonArray, i);
			if (go.GetJsonNumber("Parent UUID") == 0)
			{
				name = go.GetJsonString("Name");
				root = new GameObject();
				root->OnLoad(go);
			}
			else
			{
				GameObject* parent = GetGoByUuid(go.GetJsonNumber("Parent UUID"));
				GameObject* child = CreateGameObject(parent, false);
				child->OnLoad(go);
				if (child->GetName() == std::string("Player"))
				{
					player = child;
					// Register this camera as the default listener.
					AkGameObjectID playerID = player->GetUUID();
					AudioManager::Get()->SetDefaultListener(&playerID, player->GetComponent<TransformComponent>());
				}
				if (child->GetName() == std::string("Camera"))
				{
					camera = child;
					mainCamera = child->GetComponent<CameraComponent>();
				}
			}
		}
		// TODO: This has been comented to avoid potential crash on release. Will need to be used in the future, so dont delete.
		//for (auto i = app->sceneManager->referenceMap.begin(); i != app->sceneManager->referenceMap.end(); ++i)
		//{
		//	// Get the range of the current key
		//	auto range = app->sceneManager->referenceMap.equal_range(i->first);

		//	// Now render out that whole range
		//	for (auto d = range.first; d != range.second; ++d)
		//	{
		//		d->second->fiValue.goValue = GetGoByUuid(d->first);

		//		if (d->second->fiValue.goValue)
		//		{
		//			if (std::find(d->second->fiValue.goValue->csReferences.begin(), d->second->fiValue.goValue->csReferences.end(), d->second) == d->second->fiValue.goValue->csReferences.end())
		//				d->second->fiValue.goValue->csReferences.push_back(d->second);

		//			d->second->parentSC->SetField(d->second->field, d->second->fiValue.goValue);
		//		}
		//	}
		//}
		app->physics->LoadConstraints();
		DEBUG_LOG("Scene loaded");
	}
	else
	{
		DEBUG_LOG("Scene couldn't be loaded");
	}

	// TODO: Check this because it can be much cleaner
	qTree.Clear();
	qTree.Create(AABB(float3(-400, -50, -400), float3(400, 50, 400)));
	app->editor->SetGO(nullptr);

	return true;
}

bool Scene::SaveScene(const char* name)
{
	DEBUG_LOG("Saving Scene");

	assetsPath = name;
	
	std::string rootName = name;
	app->fs->GetFilenameWithoutExtension(rootName);
	root->SetName(rootName.c_str());

	JsonParsing sceneFile;
	/*sceneFile.SetNewJsonString(sceneFile.ValueToObject(sceneFile.GetRootValue()), "SceneName", name);*/

	sceneFile = sceneFile.SetChild(sceneFile.GetRootValue(), "Scene");
	JSON_Array* array = sceneFile.SetNewJsonArray(sceneFile.GetRootValue(), "Game Objects");
	root->OnSave(sceneFile, array);

	uint size = sceneFile.SaveFile(name);
	
	if (size > 0)
		DEBUG_LOG("Scene saved succesfully");
	else
		DEBUG_LOG("Scene couldn't be saved");

	return true;
}

void Scene::SaveTest(JsonParsing& node, JSON_Array* array, int deadCount, std::string playerName, float3 playerPos, float time)
{
	//Load	
	JsonParsing sceneFile = JsonParsing();
	sceneFile.ParseFile("Testing.json");
	JSON_Array* jsonArray = sceneFile.GetJsonArray(sceneFile.ValueToObject(sceneFile.GetRootValue()), "Scenes");
	int size = sceneFile.GetJsonArrayCount(jsonArray);
	for (int i = 0; i < size; ++i)
	{
		JsonParsing file = JsonParsing();
		JsonParsing go = sceneFile.GetJsonArrayValue(jsonArray, i);
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Round", go.GetJsonNumber("Round"));
		file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Scene Name", go.GetJsonString("Scene Name"));
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Dead Count", go.GetJsonNumber("Dead Count"));
		file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Player Dead", go.GetJsonString("Player Dead"));
		file.SetNewJson3Number(file, "Last Pos", go.GetJson3Number(go, "Last Pos"));
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Time", go.GetJsonNumber("Time"));
		node.SetValueToArray(array, file.GetRootValue());
	}
	// Open
	JsonParsing file = JsonParsing();
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Round", size++);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Scene Name", name.c_str());
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Dead Count", deadCount);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Player Dead", playerName.c_str());
	file.SetNewJson3Number(file, "Last Pos", playerPos);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Time", time);

	//Close
	node.SetValueToArray(array, file.GetRootValue());
}

void Scene::DuplicateGO(GameObject* go, GameObject* parent)
{
	GameObject* gameObject = new GameObject();
	gameObject->SetName(go->GetName());

	gameObject->SetParent(parent);
	parent->AddChild(gameObject);

	for (int i = 0; i < go->GetComponents().size(); ++i)
	{
		gameObject->CopyComponent(go->GetComponents()[i]);
	}

	for (int i = 0; i < go->GetChilds().size(); ++i)
	{
		DuplicateGO(go->GetChilds()[i], gameObject);
	}
	//gameObject->SetAABB(go->GetAABB());
}