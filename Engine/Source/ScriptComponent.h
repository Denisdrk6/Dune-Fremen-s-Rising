#pragma once
#include "Component.h"
#include <vector>
#include <string>

class GameObject;
typedef struct _MonoClassField MonoClassField;
typedef struct _MonoMethod MonoMethod;
struct SerializedField;

class ScriptComponent : public Component
{
public:
	ScriptComponent(GameObject* owner, const char* scriptName);
	virtual ~ScriptComponent();

	bool Update(float dt) override;

	bool OnLoad(JsonParsing& nObj) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	void SetField(MonoClassField* field, GameObject* value);
	void DropField(SerializedField& fieldName, const char* dropType);

	void LoadScriptData(const char*);

private:
//#ifndef STANDALONE
	void OnEditor() override;
	void SelectScript();
	void DisplayField(SerializedField& field, const char* dropType);
//#endif // !STANDALONE

public:
	std::vector<std::string> methods;
	std::vector<SerializedField> fields;

	MonoMethod* updateMethod;
	MonoMethod* startMethod;
	uint32_t noGCobject;
	std::string name = "";

	static ScriptComponent* runningScript;
};