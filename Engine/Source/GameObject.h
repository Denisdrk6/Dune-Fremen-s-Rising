#pragma once

#include <string>
#include <vector>

#include "Geometry/OBB.h"
#include "Geometry/AABB.h"
#include "CameraComponent.h"

typedef unsigned int uint;
typedef JSON_Array;

struct SerializedField;
class JsonParsing;
class Component;

class GameObject
{
public:
	GameObject();
	~GameObject();

	bool Update(float dt);
	void Draw(CameraComponent* gameCam);
	void DrawOutline();
	void DrawEditor();
	void DebugColliders(float3* points, float3 color = float3::one);

	Component* CreateComponent(ComponentType type, const char* name = nullptr);
	void AddComponent(Component* component);
	void RemoveComponent(Component* component);
	void MoveComponent(Component* component, int position);
	void CopyComponent(Component* component);
	inline const std::vector<Component*> GetComponents() const { return components; }
	Component* GetComponent(ComponentType type);
	
	void AddChild(GameObject* object, bool begin = false);
	void RemoveChild(GameObject* object);
	inline void RemoveChildren(std::vector<GameObject*>::const_iterator i) { children.erase(i); };

	inline std::vector<GameObject*>::const_iterator FindChildren(GameObject* child) { return std::find(children.begin(), children.end(), child); };

	inline void SetParent(GameObject* object) { parent = object; }
	inline void SetName(const char* n) { name = n; }
	inline void SetAudioRegister(bool check) { audioRegistered = check; }
	inline void UnPrefab() { prefabID = 0; }
	inline void SetPrefabID(uint id) { prefabID = id; }

	inline uint const GetUUID() const { return uuid; }
	inline uint const GetPrefabID() const { return prefabID; }
	inline const char* GetName() const { return name.c_str(); }
	inline GameObject* GetParent() const { return parent; }
	inline const bool& GetActive() const { return active; }
	inline std::vector<GameObject*>& GetChilds() { return children; }
	inline bool CheckAudioRegister() { return audioRegistered; }

	void SetAABB(AABB newAABB, bool needToClean = false);
	void SetAABB(OBB newOBB);
	void SetNewAABB();
	inline AABB GetAABB() { return globalAabb; }
	inline OBB GetOOB() { return globalObb; }
	inline void ClearAABB() { globalAabb.SetNegativeInfinity(); }
	void EditAABB(float3 offset, float3 size);

	void MoveChildrenUp(GameObject *child);
	void MoveChildrenDown(GameObject *child);

	void OnLoad(JsonParsing& node);
	void OnSave(JsonParsing& node, JSON_Array* array);

	void OnSavePrefab(JsonParsing& node, JSON_Array* array, int option);
	void UpdateFromPrefab(JsonParsing& node, bool isParent);

	inline float3 GetOffsetCM() { return offsetCM; };
	inline void SetOffsetCM(float3 offset) { offsetCM = offset; };

	void EnableDisableActive(bool ret);
	void EnableDisableStatic(bool ret);

	template<typename T>
	T* GetComponent();
	
public:
	std::string name;
	bool active;
	bool staticObj;
	std::string tag;
	std::string layer;
	std::string prefabPath;

	bool isUI = false;

	std::vector<SerializedField*> csReferences;
	std::vector<Component*> components;

	bool showAABB = false;
	bool showOBB = false;
private:

	GameObject* parent;
	std::vector<GameObject*> children;

	bool newComponent;

	AABB globalAabb;
	OBB globalObb;

	uint uuid;
	uint prefabID;
	float3 offsetCM = float3::zero;

	//MouseMoveCommand mouseMoveCommand;

	// Boolean to check if any component from audio (AudioSource, listener) has already been registered. 
	bool audioRegistered;
};

template<typename T>
inline T* GameObject::GetComponent()
{
	T* component = nullptr;

	for (std::vector<Component*>::iterator i = components.begin(); i < components.end(); ++i)
	{
		component = dynamic_cast<T*>(*i);
		if (component != nullptr)
			return component;
	}

	return component;
}