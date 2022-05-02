#pragma once
#include <string>
#include <queue>
#include <vector>

class GameObject;
class JsonParsing;
typedef unsigned int uint;

class PrefabManager
{
public:
	static PrefabManager* GetInstance();
	static void ReleaseInstance();
	~PrefabManager();

	void SavePrefab(GameObject* gameObject, int option);
	GameObject* LoadPrefab(const char* path, bool begin = false);
	void UpdatePrefabs(GameObject* prefabPath);

	void AddObjectsFromPrefabs(JsonParsing& prefabFile, GameObject* gameObject, std::vector<uint>& idToDeletePrefab, std::vector<GameObject*>& listGo, std::vector<GameObject*>& listParents);
	void RemoveObjectFromPrefabs(std::vector<uint>& idToDeletePrefab, std::vector<uint>& idToDeleteScene, std::vector<GameObject*>& listGo);
	void FillListGoParents(std::queue<GameObject*>& que, std::vector<GameObject*>& listGo, std::vector<uint>& idToDeleteScene, std::vector<GameObject*>& listParents);

	void ImportToLibrary();

private:
	PrefabManager();
	std::string RenameFile(GameObject* gameObject, std::string& fileName);

	static PrefabManager* instance;
};