#pragma once
#include "Imgui/imgui.h"
#include "JsonParsing.h"
#include "IconsFontAwesome5.h"

class GameObject;
class CameraComponent;

enum class ComponentType
{
	NONE =				-1,
	TRANSFORM =			0,
	MESH_RENDERER =		1,
	MATERIAL =			2,
	CAMERA =			3,
	SCRIPT =			4,
	AUDIO_SOURCE =		5,
	AUDIO_LISTENER =	6,
	AUDIO_REVERB_ZONE = 7,
	LIGHT =				8,
	ANIMATION =			9,
	BONE =				10,
	RIGID_BODY =		11,
	UI_BUTTON =			12,
	UI_IMAGE =			13,
	UI_CHECKBOX =		14,
	UI_SLIDER =			15,
	UI_TEXT =			16,
	UI_INPUTBOX =		17,
	UI_DROPDOWN =       18,
	TRANFORM2D =		19,
	NAVAGENT =			20,
	PARTICLE_SYSTEM =	21,
	BILLBOARD =			22,
	INPUT_ACTION =		23,
};

enum class State
{
	DISABLED,
	NORMAL,
	FOCUSED,
	PRESSED,
	SELECTED
};

class Component
{
public:
	Component() : active(true), type(ComponentType::NONE), owner(nullptr), collapsed(false) {}
	virtual ~Component() {}

	virtual void Enable() {}
	virtual bool Update(float dt) { return true; }
	virtual void Disable() {}
	virtual void Draw(CameraComponent* gameCam = nullptr) {}
	virtual void DrawOutline() {}

	virtual void OnEditor() {}

	inline void SetOwner(GameObject* own) { owner = own; }

	void Checkbox(Component* component, const char* name, bool& act)
	{
		ImGui::PushID((void*)component);
		ImGui::Checkbox(name, &act);
		ImGui::PopID();
	}

	void ComponentOptions(Component* component);

	inline const ComponentType& GetType() const { return type; }
	inline const bool& GetActive() const { return active; }

	virtual bool OnLoad(JsonParsing& node) { return true; }
	virtual bool OnSave(JsonParsing& node, JSON_Array* array) { return true; }

public:

	ComponentType type;
	GameObject* owner;
	bool active;
	bool collapsed = false;
};