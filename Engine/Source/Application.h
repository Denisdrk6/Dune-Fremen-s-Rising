#pragma once
#include "EngineTimer.h"
#include <list>
#include "JsonParsing.h"


class Module;
class ModuleWindow;
class ModuleInput;
class ModuleSceneManager;
class ModuleRenderer3D;
class ModuleCamera3D;
class ModuleEditor;
class FileSystem;
class MonoManager;
class Physics3D;
class ModuleNavMesh;
class ModuleUI;
class CameraComponent;

#define CONFIG_FILENAME	"config.json"
#define APPLICATION_NAME "Dune: Fremen's Rising"
#define ORGANIZATION_NAME "Yggdrasil Studios"

class Application
{
public:
	Application();
	~Application();

	bool Init();
	bool Update();
	bool CleanUp();

	bool StringCmp(const char* str1, const char* str2);

	void RequestBrowser(const char* path);
	void LogConsole(const char* string);

	void SetFPSLimit(const int fps);
	inline int GetFPSLimit() const { return engineTimer.GetFps(); }
	inline float GetEngineDeltaTime() const { return engineTimer.GetDeltaTime(); }

	inline void SaveConfigRequest() { saveRequested = true; }
	inline void LoadConfigRequest() { loadRequested = true; }

	inline void ResizeRequest() { resizeRequested = true; }

	inline const char* GetAppName() const { return APPLICATION_NAME; }
	inline const char* GetOrganizationName() const { return ORGANIZATION_NAME; }

private:
	void AddModule(Module* mod);
	void PrepareUpdate();
	void FinishUpdate();

	void SaveConfig();
	void LoadConfig();

public:
	ModuleWindow* window = nullptr;
	Physics3D* physics = nullptr;
	ModuleInput* input = nullptr;
	ModuleRenderer3D* renderer3D = nullptr;
	ModuleCamera3D* camera = nullptr;
	ModuleEditor* editor = nullptr;
	ModuleSceneManager* sceneManager = nullptr;
	FileSystem* fs = nullptr;
	MonoManager* moduleMono = nullptr;
	ModuleUI* userInterface = nullptr;
	ModuleNavMesh* navMesh = nullptr;

	JsonParsing jsonFile;
	CameraComponent* gameCam;

private:
	std::list<Module*> listModules;

	EngineTimer engineTimer;

	bool saveRequested;
	bool loadRequested;
	bool resizeRequested;
};

extern Application* app;