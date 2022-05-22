#pragma once
#include "Component.h"
#include <vector>
#include <string>
#include <algorithm>

class GameObject;
class RigidBodyComponent;

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

	void CallOnCreation();
	void CallOnDestroy();
	void CallOnTriggerEnter(RigidBodyComponent* other);
	void CallOnTrigger(RigidBodyComponent* other);
	void CallOnTriggerExit(RigidBodyComponent* other);

	void CallOnCollisionEnter(RigidBodyComponent* other);
	void CallOnCollision(RigidBodyComponent* other);

	inline uint32_t GetScriptGO() { return noGCobject; }


private:
//#ifndef STANDALONE
	void OnEditor() override;
	void SelectScript();
	void DisplayField(SerializedField& field, const char* dropType);
//#endif // !STANDALONE

public:
	std::vector<std::string> methods;
	std::vector<SerializedField> fields;

	MonoMethod* startMethod;
	MonoMethod* updateMethod;
	MonoMethod* onTriggerEnterMethod;
	MonoMethod* onTriggerMethod;
	MonoMethod* onTriggerExitMethod;
	MonoMethod* onCollisionEnterMethod;
	MonoMethod* onCollisionMethod;
	MonoMethod* onCreationMethod;
	MonoMethod* onDestroyMethod;

	uint32_t noGCobject;
	std::string name = "";

	bool firstUpdate;

	static ScriptComponent* runningScript;

private:
	bool callStart;
};