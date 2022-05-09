#pragma once

#include "Component.h"

class TransformComponent;
class ActionMaps;
typedef struct _MonoMethod MonoMethod;

class InputActionComponent : public Component
{
public:

	InputActionComponent(GameObject* own);
	~InputActionComponent();

	void OnEditor() override;
	bool Update(float dt) override;

	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	bool LoadInputAsset(const char* path);
	void LoadAllInputAssets(const char* folder);

	void SetActionMap(int index);

private:

	std::shared_ptr<ActionMaps> currentPreset;
	std::vector<std::shared_ptr<ActionMaps>> currentActionMaps;
	std::string currentAssetName;
	std::string currentAssetPath;
	

	std::vector<std::vector<std::string>> scriptsNameList;
	std::vector<std::vector<int>> currentMethodList;
	std::vector<MonoMethod*> monoMethodList;
	std::vector<std::vector<std::vector<std::string>>> scriptsMethodsList;

	bool assetWindowActive;
	std::vector<std::string> inputAssetsList;
};