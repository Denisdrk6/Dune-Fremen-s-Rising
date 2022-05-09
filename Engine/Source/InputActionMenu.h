#pragma once

#include "Menu.h"

#include <string>
#include <vector>

class Actions
{
public:
	Actions();
	~Actions();

	void OnSave(JsonParsing& node, JSON_Array* array);
	void OnLoad(JsonParsing& node);

	inline std::string GetName() { return name; }
	inline void SetName(const char* newName) { name = newName; }
	inline void AddBinding(int bind) { bindings.push_back(bind); }
	inline void SetBinding(int vecPos, int bind) { bindings[vecPos] = bind; }
	inline std::vector<int>* GetBindings() { return &bindings; }

private:
	std::string name;
	std::vector<int> bindings;
};

class ActionMaps
{
public:
	ActionMaps();
	~ActionMaps();

	void OnSave(JsonParsing& node, JSON_Array* array);
	void OnLoad(JsonParsing& node);

	inline std::string GetName() { return name; }
	inline void SetName(const char* newName) { name = newName; }
	inline std::vector<std::shared_ptr<Actions>>* GetActions() { return &actions; }

private:
	std::string name;
	std::vector<std::shared_ptr<Actions>> actions;
};

class InputActionMenu : public Menu
{
public:
	InputActionMenu(); 
	~InputActionMenu();

	bool Start() override;
	bool Update(float dt) override;

	void ColumnProperties();
	void ColumnActions();
	void ColumnActionMaps();

	bool CleanUp() override;
	void ResetCurrent();

	bool SaveInputActionFile(const char* path);
	bool LoadInputActionFile(const char* path);

	void ImportToLibrary();

private:

	std::vector<std::shared_ptr<ActionMaps>> actionMaps;

	int currentMap;
	int currentAction;
	int currentBinding;
	const char* currentBindingItem = NULL;

	bool showActionMapNamePopUp = false;
	bool showActionNamePopUp = false;

	std::string currentInputAssetName;
	std::string currentInputAssetPath;
};