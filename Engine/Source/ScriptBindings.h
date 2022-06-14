#pragma once
#include "Application.h"
#include "ModuleWindow.h"
#include "ModuleRenderer3D.h"
#include "ModuleInput.h"
#include "ModuleSceneManager.h"
#include "ModuleEditor.h"

#include "ResourceManager.h"
#include "PrefabManager.h"
#include "DialogueSystem.h"

#include "ButtonComponent.h"
#include "MaterialComponent.h"
#include "MeshComponent.h"
#include "C_RigidBody.h"
#include "Texture.h"
#include "ParticleSystemComponent.h"
#include "LightComponent.h"
#include "Lights.h"

#include "Scene.h"
#include "TransformBindings.h"
#include "GameView.h"

#include <queue>

#include <metadata\object-forward.h>
#include <metadata\object.h>

#include "BulletDynamics/Dynamics/btRigidBody.h"

// Input bindings ===============================================================================

int GetKey(MonoObject* x)
{
	if (app != nullptr)
		return app->input->GetKey(*(int*)mono_object_unbox(x));

	return 0;
}
int GetMouseClick(MonoObject* x)
{
	if (app != nullptr)
		return app->input->GetMouseButton(*(int*)mono_object_unbox(x));

	return 0;
}
int MouseX()
{
	if (app != nullptr)
		return app->input->GetMouseXMotion();

	return 0;
}
int MouseY()
{
	if (app != nullptr)
		return app->input->GetMouseYMotion();

	return 0;
}
void SetCursorState(int x)
{
	if (app != nullptr)
		app->input->SetCursorState(x);
}

int GetCursorState()
{
	if (app != nullptr)
		return app->input->GetCursorState();
}

void RestoreDefaultCursor()
{
	/*if (app != nullptr)
		app->input->RestoreDefaultCursor();*/
}

void SetEagleCursor(bool ret)
{
	if (app != nullptr)
	{
		if (ret)
		{
			int i = 0;
			for (std::vector<SDL_Cursor*>::iterator it = app->input->GetCursors()->begin(); it != app->input->GetCursors()->end(); ++it)
			{
				if (i == 4)
				{
					app->input->GetCursors()->erase(it);
					app->input->GetCursors()->insert(it, SDL_CreateColorCursor(SDL_LoadBMP("Library/Cursors/ui_eagle.bmp"), 0, 0));
					break;
				}
				i++;
			}
		}
		else
		{
			int i = 0;
			for (std::vector<SDL_Cursor*>::iterator it = app->input->GetCursors()->begin(); it != app->input->GetCursors()->end(); ++it)
			{
				if (i == 4)
				{
					app->input->GetCursors()->erase(it);
					app->input->GetCursors()->insert(it, SDL_CreateColorCursor(SDL_LoadBMP("Library/Cursors/paul_throw_stone.bmp"), 0, 0));
					break;
				}
				i++;
			}
		}	
	}
}

// Input bindings ===============================================================================

// Component bindings ===============================================================================

// Transform ========

MonoObject* GetForward(MonoObject* go)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	MonoClass* vecBind = mono_class_from_name(app->moduleMono->image, "RagnarEngine", "Vector3");
	return app->moduleMono->Float3ToCS(tr->GetForward());
}

MonoObject* GetRight(MonoObject* go)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	MonoClass* vecBind = mono_class_from_name(app->moduleMono->image, "RagnarEngine", "Vector3");
	return app->moduleMono->Float3ToCS(tr->GetRight());
}

MonoObject* GetUp(MonoObject* go)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	MonoClass* vecBind = mono_class_from_name(app->moduleMono->image, "RagnarEngine", "Vector3");
	return app->moduleMono->Float3ToCS(tr->GetUp());
}

MonoObject* GetPosition(MonoObject* go)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	float3 position = tr->GetLocalTransform().TranslatePart();
	return app->moduleMono->Float3ToCS(position);
}

MonoObject* GetGlobalPosition(MonoObject* go)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	float3 position = tr->GetGlobalTransform().TranslatePart();
	return app->moduleMono->Float3ToCS(position);
}

MonoObject* GetRotation(MonoObject* go)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	Quat rotation = Quat::identity;
	float3 p, s;
	tr->GetLocalTransform().Decompose(p, rotation, s);
	return app->moduleMono->QuatToCS(rotation);
}

MonoObject* GetGlobalRotation(MonoObject* go)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	Quat rotation = Quat::identity;
	float3 p, s;
	tr->GetGlobalTransform().Decompose(p, rotation, s);
	return app->moduleMono->QuatToCS(rotation);
}

void SetGlobalRotation(MonoObject* go, MonoObject* newRot)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	tr->SetRotation(app->moduleMono->UnboxQuat(newRot));
}

MonoObject* GetScale(MonoObject* go)
{
	TransformComponent* tr = GetComponentMono<TransformComponent*>(go);
	float3 scale = tr->GetLocalTransform().GetScale();
	return app->moduleMono->Float3ToCS(scale);
}

void SetPosition(MonoObject* go, MonoObject* position)
{
	if (TransformComponent* tr = GetComponentMono<TransformComponent*>(go))
	{
		tr->SetPosition(app->moduleMono->UnboxVector(position));
		tr->UpdateTransform();
	}
}

void SetGlobalPosition(MonoObject* go, MonoObject* position)
{
	if (TransformComponent* tr = GetComponentMono<TransformComponent*>(go))
	{
		tr->SetGlobalPosition(app->moduleMono->UnboxVector(position));
		tr->UpdateTransform();
	}
}

void SetRotation(MonoObject* go, MonoObject* rotation)
{
	if (TransformComponent* tr = GetComponentMono<TransformComponent*>(go))
	{
		// Should update inspector rotation too?
		tr->SetRotation(app->moduleMono->UnboxQuat(rotation));
		tr->UpdateTransform();
	}
}

void SetScale(MonoObject* go, MonoObject* scale)
{
	if (TransformComponent* tr = GetComponentMono<TransformComponent*>(go))
	{
		tr->SetScale(app->moduleMono->UnboxVector(scale));
		tr->UpdateTransform();
	}
}
float GetAngleBetween(MonoObject* vector1, MonoObject* vector2) // In degrees
{
	float3 vec1 = app->moduleMono->UnboxVector(vector1);
	float3 vec2 = app->moduleMono->UnboxVector(vector2);	
	return vec1.AngleBetween(vec2) * RADTODEG;
}
float GetDistanceBetween(MonoObject* vector1, MonoObject* vector2)
{
	float3 vec1 = app->moduleMono->UnboxVector(vector1);
	float3 vec2 = app->moduleMono->UnboxVector(vector2);
	return vec1.Distance(vec2);
}
MonoObject* RotateY(MonoObject* vector, float anglesDegrees)
{
	float3 dirV = app->moduleMono->UnboxVector(vector);
	return app->moduleMono->Float3ToCS(dirV * math::float3x3::RotateY(anglesDegrees * DEGTORAD));
}
// Transform ========


MonoString* GetTexturePath(MonoObject* go)
{
	MaterialComponent* matComp = GetComponentMono<MaterialComponent*>(go);
	std::string p = matComp->GetTexture()->GetAssetsPath();
	return mono_string_new(app->moduleMono->domain, p.c_str());
}

void SetTexturePath(MonoObject* go, MonoString* texturePath)
{
	char* goName = mono_string_to_utf8(mono_object_to_string(go, 0));
	MaterialComponent* matComp = GetComponentMono<MaterialComponent*>(go);
	char* path = mono_string_to_utf8(texturePath);
	std::string p = path;

	std::shared_ptr<Texture> newTexture = std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->LoadResource(p));
	matComp->SetTextureType(TextureType::DIFFUSE);
	matComp->SetTexture(newTexture);
}

float GetDiffuseAlpha(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return gameObject->GetComponent<MaterialComponent>()->GetDiffuseAlpha();
}

void SetDiffuseAlpha(MonoObject* go, float value)
{
	if (value > 1) value = 1;
	else if (value < 0) value = 0;
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	gameObject->GetComponent<MaterialComponent>()->SetDiffuseAlpha(value);
}

MonoBoolean GetEmissiveEnabled(MonoObject* go)
{
	MaterialComponent* mat = GetComponentMono<MaterialComponent*>(go);
	return mat->IsEmissiveEnabled();
}

void SetEmissiveEnabled(MonoObject* go, MonoBoolean value)
{
	MaterialComponent* mat = GetComponentMono<MaterialComponent*>(go);
	mat->SetEmissiveEnabled(value);
}

MonoObject* GetEmissiveColor(MonoObject* go)
{
	MaterialComponent* mat = GetComponentMono<MaterialComponent*>(go);
	return app->moduleMono->Float3ToCS(mat->GetEmissiveColor());
}

void SetEmissiveColor(MonoObject* go, MonoObject* color)
{
	MaterialComponent* mat = GetComponentMono<MaterialComponent*>(go);
	mat->SetEmissiveColor(app->moduleMono->UnboxVector(color));
}

// Light ============================

void SetDirectionalLightShadowsEnabled(MonoBoolean* value)
{
	app->renderer3D->dirLight->generateShadows = value;
}

MonoBoolean GetDirectionalLightShadowsEnabled()
{
	return app->renderer3D->dirLight->generateShadows;
}

float GetLightIntensity(MonoObject* go)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	return lightComp->GetLight()->intensity;
}

void SetLightIntensity(MonoObject* go, float intensity)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	lightComp->GetLight()->intensity = intensity;
}

float GetLightLinear(MonoObject* go)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	assert(lightComp->GetLight()->type == LightType::POINT && "The Light MUST be a Point Light");

	if (PointLight* l = (PointLight*)lightComp->GetLight())
		return l->lin;
}

void SetLightLinear(MonoObject* go, float lin)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	assert(lightComp->GetLight()->type == LightType::POINT && "The Light MUST be a Point Light");

	PointLight* l = (PointLight*)lightComp->GetLight();
	l->lin = lin;
}

float GetLightQuadratic(MonoObject* go)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	assert(lightComp->GetLight()->type == LightType::POINT && "The Light MUST be a Point Light");

	if (PointLight* l = (PointLight*)lightComp->GetLight())
		return l->quadratic;
}

void SetLightQuadratic(MonoObject* go, float quadratic)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	assert(lightComp->GetLight()->type == LightType::POINT && "The Light MUST be a Point Light");

	PointLight* l = (PointLight*)lightComp->GetLight();
	l->quadratic = quadratic;
}

float GetLightConstant(MonoObject* go)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	assert(lightComp->GetLight()->type == LightType::POINT && "The Light MUST be a Point Light");

	if (PointLight* l = (PointLight*)lightComp->GetLight())
		return l->constant;
}

void SetLightConstant(MonoObject* go, float constant)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	assert(lightComp->GetLight()->type == LightType::POINT && "The Light MUST be a Point Light");

	PointLight* l = (PointLight*)lightComp->GetLight();
	l->constant = constant;
}

MonoObject* GetLightAmbient(MonoObject* go)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	return app->moduleMono->Float3ToCS(lightComp->GetLight()->ambient);
}

void SetLightAmbient(MonoObject* go, MonoObject* ambient)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	lightComp->GetLight()->ambient = app->moduleMono->UnboxVector(ambient);
}

MonoObject* GetLightDiffuse(MonoObject* go)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	return app->moduleMono->Float3ToCS(lightComp->GetLight()->diffuse);
}
void SetLightDiffuse(MonoObject* go, MonoObject* ambient)
{
	ComponentLight* lightComp = GetComponentMono<ComponentLight*>(go);
	lightComp->GetLight()->diffuse = app->moduleMono->UnboxVector(ambient);
}

// Light ============================


// GameObject =======================

void GameObjectDrawOutline(MonoObject* gameObject, MonoObject* color)
{
	GameObject* go = app->moduleMono->GameObjectFromCSGO(gameObject);
	float3 col = app->moduleMono->UnboxVector(color);
	std::pair<GameObject*, float3> p(go, col);
	app->renderer3D->gosToDrawOutline.push_back(p);
}

void GameObjectStopDrawOutline(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	std::pair<GameObject*, float3> p(gameObject, float3::one);

	for (int i = 0; i < app->renderer3D->gosToDrawOutline.size(); ++i)
		if (app->renderer3D->gosToDrawOutline[i].first == p.first)
			app->renderer3D->gosToDrawOutline.erase(app->renderer3D->gosToDrawOutline.begin() + i);
}

MonoObject* InstantiateGameObject(MonoObject* name, MonoObject* position, MonoObject* rotation)
{
	GameObject* go = app->sceneManager->GetCurrentScene()->CreateGameObject(nullptr);
	char* goName = mono_string_to_utf8(mono_object_to_string(name, 0));
	go->SetName(goName);
	mono_free(goName);

	float3 p = app->moduleMono->UnboxVector(position);
	TransformComponent* tr = go->GetComponent<TransformComponent>();
	tr->SetPosition(p);

	Quat r = app->moduleMono->UnboxQuat(rotation);
	tr->SetRotation(r);
	return app->moduleMono->GoToCSGO(go);
}

void Instantiate3DObject(MonoObject* name, int primitiveType, MonoObject* position, MonoObject* rotation)
{
	Object3D t = static_cast<Object3D>(primitiveType);
	GameObject* go = app->sceneManager->GetCurrentScene()->Create3DObject(t, nullptr);
	char* goName = mono_string_to_utf8(mono_object_to_string(name, 0));
	go->SetName(goName);
	mono_free(goName);

	float3 p = app->moduleMono->UnboxVector(position);
	TransformComponent* tr = go->GetComponent<TransformComponent>();
	tr->SetPosition(p);

	Quat r = app->moduleMono->UnboxQuat(rotation);
	tr->SetRotation(r);
}

MonoObject* Instantiate3DGameObject(MonoObject* name, int primitiveType, MonoObject* position)
{
	Object3D t = static_cast<Object3D>(primitiveType);
	GameObject* go = app->sceneManager->GetCurrentScene()->Create3DObject(t, nullptr);
	char* goName = mono_string_to_utf8(mono_object_to_string(name, 0));
	go->SetName(goName);
	mono_free(goName);

	float3 p = app->moduleMono->UnboxVector(position);
	TransformComponent* tr = go->GetComponent<TransformComponent>();
	tr->SetPosition(p);
	tr->UpdateTransform();

	return app->moduleMono->GoToCSGO(go);
}

MonoObject* InstancePrefab(MonoObject* name, MonoObject* position, bool begin = false)
{
	char* goName = mono_string_to_utf8(mono_object_to_string(name, 0));
	DEBUG_LOG("Instantiating prefab %s", goName);

	std::string	path;

#ifdef DIST
	path = PREFABS_FOLDER;
#else
	path = PREFABS_ASSETS_FOLDER;
#endif

	path += goName;
	path += ".rgprefab";
	
	mono_free(goName);

	GameObject* go = PrefabManager::GetInstance()->LoadPrefab(path.c_str(), begin);

	float3 pos = app->moduleMono->UnboxVector(position);
	if (TransformComponent* tr = go->GetComponent<TransformComponent>())
	{
		tr->SetGlobalPosition(pos);
		tr->SetAABB();
	}
	if (RigidBodyComponent* rb = go->GetComponent<RigidBodyComponent>())
		rb->GetBody()->getWorldTransform().setOrigin(pos);

	if (ScriptComponent* script = go->GetComponent<ScriptComponent>())
		script->CallOnCreation();

	return app->moduleMono->GoToCSGO(go);
}

MonoObject* Destroy(MonoObject* go)
{
	GameObject* toDelete = app->moduleMono->GameObjectFromCSGO(go);
	toDelete->GetParent()->GetChilds().erase(toDelete->GetParent()->FindChildren(toDelete));
	RELEASE(toDelete);
	return 0;
}

MonoObject* AddComponentMono(MonoObject* go, int componentType)
{
	GameObject* owner = app->moduleMono->GameObjectFromCSGO(go);
	Component* comp = owner->CreateComponent(static_cast<ComponentType>(componentType));

	return app->moduleMono->ComponentToCS(comp);
}

void DeleteComponentMono(MonoObject* go, MonoObject* component)
{
	GameObject* owner = app->moduleMono->GameObjectFromCSGO(go);
	owner->RemoveComponent(GetComponentMono<Component*>(component));
}

MonoObject* FindGameObjectWithName(MonoObject* name)
{
	char* goName = mono_string_to_utf8(mono_object_to_string(name, 0));

	std::queue<GameObject*> q;
	for (auto& go : app->sceneManager->GetCurrentScene()->GetGameObjectsList())
		q.push(go);

	while (!q.empty())
	{
		GameObject* curr = q.front();
		q.pop();

		if (curr->GetName() == std::string(goName))
		{
			mono_free(goName);
			return app->moduleMono->GoToCSGO(curr);
		}
	}

	mono_free(goName);
	return nullptr;
}

MonoArray* FindGameObjectsWithTag(MonoObject* tag)
{
	char* tagName = mono_string_to_utf8(mono_object_to_string(tag, 0));
	
	std::vector<MonoObject*> objects;
	for (auto& go : app->sceneManager->GetCurrentScene()->GetGameObjectsList())
	{
		if (go->tag == tagName)
			objects.push_back(app->moduleMono->GoToCSGO(go));
	}
	
	MonoClass* goClass = mono_class_from_name(app->moduleMono->image, SCRIPTS_NAMESPACE, "GameObject");
	MonoArray* ret = mono_array_new(app->moduleMono->domain, goClass, objects.size());
	
	for (int i = 0; i < objects.size(); ++i)
		mono_array_set(ret, MonoObject*, i, objects[i]);

	mono_free(tagName);
	return ret;
}

MonoArray* GetGameObjectChilds(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);

	std::vector<MonoObject*> objects;
	for (auto& child : gameObject->GetChilds())
		objects.push_back(app->moduleMono->GoToCSGO(child));

	MonoClass* goClass = mono_class_from_name(app->moduleMono->image, SCRIPTS_NAMESPACE, "GameObject");
	MonoArray* ret = mono_array_new(app->moduleMono->domain, goClass, objects.size());

	for (int i = 0; i < objects.size(); ++i)
		mono_array_set(ret, MonoObject*, i, objects[i]);

	return ret;
}

MonoString* GetGameObjectTagMono(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return mono_string_new(app->moduleMono->domain, gameObject->tag.c_str());
}

void SetGameObjectTagMono(MonoObject* go, MonoString* newTag)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	char* tagName = mono_string_to_utf8(newTag);
	gameObject->tag = tagName;
}

MonoString* GetGameObjectName(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return mono_string_new(app->moduleMono->domain, gameObject->name.c_str());
}

void SetGameObjectName(MonoObject* go, MonoString* newName)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	char* name = mono_string_to_utf8(newName);
	gameObject->name = name;
}

MonoBoolean GetGameObjectIsActive(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return gameObject->active;
}

void SetGameObjectIsActive(MonoObject* go, MonoBoolean value)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	gameObject->EnableDisableActive(value);
}

MonoBoolean GetHasBeenUpdate(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return gameObject->hasBeenUpdate;
}

MonoBoolean GetActiveComponent(MonoObject* go)
{
	Component* comp = GetComponentMono<Component*>(go);
	return comp->active;
}
void SetActiveComponent(MonoObject* go, MonoBoolean value)
{
	Component* comp = GetComponentMono<Component*>(go);
	comp->active = value;
}
MonoObject* GetSizeAABB(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return app->moduleMono->Float3ToCS(gameObject->GetAABB().Size());
}
MonoObject* GetMinAABB(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return app->moduleMono->Float3ToCS(gameObject->GetAABB().minPoint);
}
MonoObject* GetMaxAABB(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return app->moduleMono->Float3ToCS(gameObject->GetAABB().maxPoint);
}
void SetSizeAABB(MonoObject* go, MonoObject* min, MonoObject* max)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	float3 minPoint = app->moduleMono->UnboxVector(min);
	float3 maxPoint = app->moduleMono->UnboxVector(max);

	OBB newObb = AABB(minPoint, maxPoint).ToOBB();
	gameObject->SetAABB(newObb);
}

void ReparentToRoot(MonoObject* go)
{
	GameObject* parent = app->moduleMono->GameObjectFromCSGO(go);
	Scene* currentScene = app->sceneManager->GetCurrentScene();

	currentScene->ReparentGameObjects(parent, currentScene->GetRoot());
}

void ChangeMesh(MonoObject* go, MonoString* name)
{
	GameObject* parent = app->moduleMono->GameObjectFromCSGO(go);
	std::string fileName = mono_string_to_utf8(name);

	std::vector<std::string> files;
	app->fs->DiscoverFiles("Library/Meshes/", files);
	for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
	{
		if ((*it).find(".rgmesh") != std::string::npos)
		{
			app->fs->GetFilenameWithoutExtension(*it);
			*it = (*it).substr((*it).find_last_of("_") + 1, (*it).length());
			std::shared_ptr<Resource> res = ResourceManager::GetInstance()->LoadResource(std::stoll(*it));
	
			if (res.get()->GetName().find(fileName) != std::string::npos)
			{
				MeshComponent* mesh = parent->GetComponent<MeshComponent>();
				parent->GetComponent<MeshComponent>()->SetMesh(res);
				return;
			}
		}
	}
}

void AddChild(MonoObject* go, MonoObject* child)
{
	GameObject* parent = app->moduleMono->GameObjectFromCSGO(go);
	GameObject* newChild = app->moduleMono->GameObjectFromCSGO(child);

	parent->AddChild(newChild);
	newChild->SetParent(parent);
	parent->GetParent()->RemoveChild(newChild);
}

void EraseChild(MonoObject* go, MonoObject* child)
{
	GameObject* parent = app->moduleMono->GameObjectFromCSGO(go);
	GameObject* newChild = app->moduleMono->GameObjectFromCSGO(child);

	parent->RemoveChild(newChild);
}

MonoObject* GetItsParent(MonoObject* go)
{
	GameObject* object = app->moduleMono->GameObjectFromCSGO(go);
	return app->moduleMono->GoToCSGO(object->GetParent());
}

bool GetGameObjectIsInteractuable(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return gameObject->isInteractuable;
}

void SetGameObjectIsInteractuable(MonoObject* go, MonoBoolean value)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	gameObject->isInteractuable = value;
}

MonoObject* GetGameObjectInteractuableColor(MonoObject* go)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	return app->moduleMono->Float3ToCS(gameObject->GetComponent<MaterialComponent>()->GetInteractuableColor());
}

void SetGameObjectInteractuableColor(MonoObject* go, MonoObject* color)
{
	GameObject* gameObject = app->moduleMono->GameObjectFromCSGO(go);
	float3 col = app->moduleMono->UnboxVector(color);
	gameObject->GetComponent<MaterialComponent>()->SetInteractuableColor(col);
}

// GameObject =======================

// Particle System ==================
MonoArray* GetEmitters(MonoObject* go)
{
	ParticleSystemComponent* particleSystem = GetComponentMono<ParticleSystemComponent*>(go);
	if (!particleSystem)
		return nullptr;

	MonoClass* emitterClass = mono_class_from_name(app->moduleMono->image, SCRIPTS_NAMESPACE, "Emitter");
	MonoArray* ret = mono_array_new(app->moduleMono->domain, emitterClass, particleSystem->GetEmitters().size());

	for (int i = 0; i < particleSystem->GetEmitters().size(); ++i)
	{
		MonoObject* emitterObject = ParticleEmitterToCS(particleSystem->GetEmitters()[i]);
		mono_array_set(ret, MonoObject*, i, emitterObject);
	}

	return ret;
}

void PlayEmitter(MonoObject* go)
{
	ParticleSystemComponent* particleSystem = GetComponentMono<ParticleSystemComponent*>(go);
	particleSystem->Play();
}

void PauseEmitter(MonoObject* go)
{
	ParticleSystemComponent* particleSystem = GetComponentMono<ParticleSystemComponent*>(go);
	particleSystem->Stop();
}
// Particle System ==================

float GetGameTimeStep()
{
	return app->sceneManager->GetGameDeltaTime();
}

void SetTimeScale(float scale)
{
	app->sceneManager->GetTimer().SetTimeScale(scale);
}

void SetDirectionParticle(MonoObject* go, MonoObject* direction)
{
	ParticleSystemComponent* particleSystem = GetComponentMono<ParticleSystemComponent*>(go);
	std::vector<ParticleEmitter*> emmiters = particleSystem->GetEmitters();

	for (std::vector<ParticleEmitter*>::iterator it = emmiters.begin(); it != emmiters.end(); ++it)
	{
		float3 dir = app->moduleMono->UnboxVector(direction);
		(*it)->SetDirection(dir);
	}
}

// Scene Manager
void NextScene()
{
	app->sceneManager->NextScene();
	app->renderer3D->gosToDrawOutline.clear();
	app->renderer3D->ClearPointLights();
}

void SaveScene(MonoString* string)
{
	char* name = mono_string_to_utf8(string);
	app->sceneManager->SaveScene(name);
}

void LoadScene(MonoString* string)
{
	char* name = mono_string_to_utf8(string);
	app->sceneManager->NextScene(name);
	app->renderer3D->gosToDrawOutline.clear();
	app->renderer3D->ClearPointLights();
}

void SaveTest(int deadCount, MonoString* playerName, MonoObject* playerPos, float time)
{
	char* name = mono_string_to_utf8(playerName);
	float3 pos = app->moduleMono->UnboxVector(playerPos);
	app->sceneManager->SaveTesting(deadCount, name, pos, time);
}

MonoString* GetLastSceneName()
{
	return mono_string_new(app->moduleMono->domain, app->sceneManager->GetLastSceneName().c_str());
}

MonoString* GetCurrentSceneName()
{
	return mono_string_new(app->moduleMono->domain, app->sceneManager->GetCurrentSceneName().c_str());
}

void Exit()
{
	app->sceneManager->Exit();
}

MonoObject* GetRegionGame()
{
	float4 vec4 = app->editor->GetGameView()->GetBounds();
	float3 vec3 = { vec4.z, vec4.w, 0 };
	return app->moduleMono->Float3ToCS(vec3);
}

void RequestDamageFeedback()
{
	app->renderer3D->RequestDamageFeedback();
}

void RequestScreenRectangle(MonoObject* color)
{
	app->renderer3D->RequestScreenRectangle(app->moduleMono->UnboxQuat(color).CastToFloat4());
}

void EndRequestScreenRectangle()
{
	app->renderer3D->EndRequestScreenRectangle();
}

// Dialogue System ======================================
MonoString* GetDialogueLine()
{
	//return mono_string_new(app->moduleMono->domain, DialogueSystem::GetInstance()->GetCurrentLine().c_str());
	//MHF
	return mono_string_new(app->moduleMono->domain, DialogueSystem::GetInstance()->GetCurrentLineXML().c_str());
}

MonoString* GetDialogueLineAuthor()
{
	
	//return mono_string_new(app->moduleMono->domain, DialogueSystem::GetInstance()->GetOwnerOfLine().c_str());
	//MHF
	return mono_string_new(app->moduleMono->domain, DialogueSystem::GetInstance()->GetOwnerOfLineXML().c_str());
}

int GetDialogueLineAuthorId() {
	return DialogueSystem::GetInstance()->GetOwnerIdOfLineXML();
}

bool NextLine()
{
	//DialogueSystem::GetInstance()->NextLine();
	//MHF
	return DialogueSystem::GetInstance()->NextLineXML();
}

void StartDialogueById(int id)
{
	/*
	DialogueSystem* sys = DialogueSystem::GetInstance();
	Dialogue* aux = sys->GetDialogueById(id);
	sys->SetDialogueAsCurrent(aux);
	sys->StartDialogue();
	*/
	//MHF
	DialogueSystem* sys = DialogueSystem::GetInstance();
	sys->SetCurrentDialogueIdXML(id);
	sys->StartDialogueXML();
}

int GetDialogueLenguage() {
	return app->sceneManager->GetLenguage();
}

void LoadDialogueFile(MonoString* name)
{
	//char* fileName = mono_string_to_utf8(name);
	//std::string path = DIALOGUES_FOLDER;
	//path += fileName;
	//path += ".rgdialogue";
	//DialogueSystem::GetInstance()->LoadDialogue(path);
	//MHF
	DialogueSystem::GetInstance()->LoadDialogueXML();
}

//configuration

void SetFullScreen(bool newState) 
{
	app->window->SetFullscreenDesktop(newState);
}
bool GetFullScreen()
{
	return app->window->GetWindowFullscreenDesktop();
}

void SetVSync(bool newState)
{
	app->renderer3D->SetVsync(newState);
}
bool GetVSync()
{
	return app->renderer3D->GetVsync();
}
MonoObject* GetMousePosition()
{
	float2 fMousePos;
	float2 mPos = float2::zero;
	float4 viewport = float4::zero;
#ifndef DIST
	mPos = { ImGui::GetIO().MousePos.x, ImGui::GetIO().MousePos.y };
	viewport = app->editor->GetGameView()->GetBounds();
#else
	mPos = { (float)app->input->GetMouseX() ,(float)app->input->GetMouseY() };
	viewport = { 0,0, (float)*app->window->GetWindowWidth(), (float)*app->window->GetWindowHeight() };
#endif
	fMousePos = { mPos.x - viewport.x , mPos.y - viewport.y };
	float3 ret(fMousePos.x- (viewport.z/2), -fMousePos.y+(viewport.w / 2), 0);
	return app->moduleMono->Float3ToCS(ret);
}