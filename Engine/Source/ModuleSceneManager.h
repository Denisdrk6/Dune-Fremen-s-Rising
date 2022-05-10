#pragma once
#include "Module.h"
#include "GameTimer.h"

#include <vector>
#include <memory>
#include <map>

enum class GameState
{
	NOT_PLAYING = 0,
	PLAYING,
	PAUSE
};

typedef unsigned int uint;

struct SerializedField;
class Scene;

class ModuleSceneManager : public Module
{
public:
	ModuleSceneManager(bool startEnabled = true);
	~ModuleSceneManager();

	bool Start() override;
	bool PreUpdate(float dt) override;
	bool Update(float dt) override;
	bool PostUpdate() override;
	bool Draw();
	bool CleanUp() override;

	void ImportPrimitives();

	Scene* GetCurrentScene() const { return currentScene.get(); }

	void LoadBuild();
	void SaveBuild();

	void NewScene();
	void AddScene(std::shared_ptr<Scene> newScene);
	void DeleteScene(std::shared_ptr<Scene> scene);

	void ChangeScene(const char* sceneName);
	inline bool GetChangeScene() { return changeScene; };

	void NextScene();
	void NextScene(const char* name);
	void SaveScene(const char* name);
	void LoadScene(const char* name);

	// Pop Up
	void WarningWindow();
	void BuildWindow();
	void ShowCreateLigthSensibleShaderWindow();
	void ShowCreateNotLigthSensibleShaderWindow();

	GameState GetGameState() { return gameState; }
	GameTimer& GetTimer() { return gameTimer; }

	std::string GetLastSceneName() { return lastSceneName; }
	std::string GetCurrentSceneName();

	inline float GetGameDeltaTime() { return gameTimer.GetDeltaTime(); }
	inline float GetTimeScale() { return gameTimer.GetTimeScale(); }
	inline void Exit() { exit = true; }

	void Play();
	void Stop();
	void Pause();
	void Resume();
	inline void NextFrame() { frameSkip = true; }

	std::vector<std::shared_ptr<Scene>>& GetScenes() { return scenes; }
	void ShortCuts();

	void SaveTesting(int deadCount, std::string playerName, float3 playerPos, float time);

	inline float GetTransitionAlpha() { return transitionAlpha; }
	inline bool IsSceneChanging() { return changeScene; }

public:
	std::multimap<uint, SerializedField*> referenceMap;
	bool newSceneLoaded;

	// bool popUp's
	bool saveScene = false;
	bool showBuildMenu = false;
	bool showCreateLightSensibleShaderWindow = false;
	bool showCreateNotLightSensibleShaderWindow = false;

	void SetLenguage(int lenguageID) { lenguage = lenguageID; }
	int GetLenguage() { return lenguage; }

private:

	int index = 0;
	int lastIndex = 0;
	bool changeScene = false;
	bool pendingToBake = false;
	std::string lastSceneName;

	bool exit = false;
	GameState gameState;
	GameTimer gameTimer;
	bool frameSkip;

	std::shared_ptr<Scene> currentScene;
	std::shared_ptr<Scene> sceneSelected = nullptr;
	std::vector<std::shared_ptr<Scene>> scenes;

	int lenguage = 0;
	bool enteringFade;
	bool exitingFade;
	bool fadeInCompleted;
	float transitionAlpha;
};