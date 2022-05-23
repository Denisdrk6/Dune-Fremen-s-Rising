#pragma once
#include "GameObject.h"
#include "Quadtree.h"

#include "Resource.h"

#include <vector>

struct SerializedField;

enum class Object3D
{
	CUBE = 0,
	PYRAMIDE,
	SPHERE,
	CYLINDER
};

class CameraComponent;

class Scene : public Resource
{
public:
	Scene(uint uid, std::string& assets, std::string& library);
	~Scene();

	bool Start();
	bool PreUpdate(float dt);
	bool Update(float dt);
	bool PostUpdate();
	bool Draw();
	bool CleanUp();

	void NewScene();

	GameObject* CreateGameObject(GameObject* parent, bool createTransform = true, bool begin = false);
	GameObject* CreateGameObjectChild(const char* name, GameObject* parent);
	GameObject* CreateGameObjectParent(const char* name, GameObject* child);
	std::vector<GameObject*> GetGameObjectsList() const 
	{ 
		std::stack<GameObject*> objects;
		std::vector<GameObject*> sceneObjects;
		objects.push(root);

		while (!objects.empty())
		{
			GameObject* go = objects.top();
			objects.pop();

			for (int i = 0; i < go->GetChilds().size(); ++i)
			{
				objects.push(go->GetChilds()[i]);
				sceneObjects.push_back(go->GetChilds()[i]);
			}
		}

		return sceneObjects;
	}

	inline GameObject* GetRoot() const { return root; }
	GameObject* GetGoByUuid(double uuid) const;

	void SetMainCamera(CameraComponent* camComponent) { mainCamera = camComponent; }

	GameObject* Create3DObject(Object3D type, GameObject* parent);

	void MoveGameObjectUp(GameObject* object);
	void MoveGameObjectDown(GameObject* object);
	void ReparentGameObjects(uint uuid, GameObject* go);
	void ReparentGameObjects(GameObject* parent, GameObject* go);

	void Load() override;
	void UnLoad() override;
	
	bool LoadScene(const char* name, bool fromLibrary = false);
	bool SaveScene(const char* name);
	void SaveTest(JsonParsing& node, JSON_Array* array, int deadCount, std::string playerName, float3 playerPos, float time);

	void DuplicateGO(GameObject* go, GameObject* parent);

	inline void ResetQuadtree() { resetQuadtree = true; }
	
	void RedistributeQuadtree(GameObject* go);
	
	// Please do not remove these two getters, if one gets removed and the other is used, some things won't work
	// ====================================================
	inline bool* GetDrawQuad() { return &drawQuad; }
	inline bool GetDebugDrawQuadtree() { return drawQuad; }
	// ====================================================

	Quadtree& GetQuadtree() { return qTree; }

	inline GameObject* GetPlayer() { return player; };


public:
	CameraComponent* mainCamera;
	GameObject* camera;

private:
	GameObject* root;
	GameObject* player;

	Quadtree qTree;

	bool frameSkip = false;
	bool resetQuadtree;
	bool drawQuad = false;
};