#pragma once
#include "ButtonComponent.h"
#include "Component.h"
#include "Color.h"
#include "Text.h"
#include "Transform2DComponent.h"
#include "ModuleUI.h"
#include <map>
#include <string>
#include <vector>
class MyPlane;
class GameObject;
class MaterialComponent;
class Character;
class Shadert;
typedef unsigned int uint;
class DropDownComponent : public Component
{
public:
	DropDownComponent(GameObject* own);
	~DropDownComponent();

	bool Update(float dt) override;
	void Draw(CameraComponent* gameCam = nullptr) override;
	void OnEditor() override;

	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	float2 GetParentPosition();
	State GetState() { return state; };
	void SetState(int newState) { state = (State)newState; };



	inline Text GetDropDownText() { return DropDownText; };
	inline char* GetText() { return text; };
	void SetText(const char* newText)
	{
		DropDownText.setOnlyText(newText);
		strcpy(text, newText);
	}

	std::string GetTextID(int id) {
		return optionsLenguaje[id];
	}

	inline Color GetActualColor() { return generalColor; };

	void SetActualColor(float Red, float Green, float Blue);

	inline void SetNormalMaterial(MaterialComponent* texture) { normalMaterial = texture; };
	inline void SetFocusedMaterial(MaterialComponent* texture) { focusedMaterial = texture; };
	inline void SetPressedMaterial(MaterialComponent* texture) { pressedMaterial = texture; };
	inline void SetDisabledMaterial(MaterialComponent* texture) { disabledMaterial = texture; };
	inline void SetActualMaterial(MaterialComponent* texture) { actual = texture; };

	inline void SetTextPosition(float3 newPoistion) { textPos = newPoistion; }
	inline float3 GetTextPosition() { return textPos; }

	inline MaterialComponent* GetNormalMaterial() { return normalMaterial; };
	inline MaterialComponent* GetFocusedMaterial() { return focusedMaterial; };
	inline MaterialComponent* GetPressedMaterial() { return pressedMaterial; };
	inline MaterialComponent* GetDisabledMaterial() { return disabledMaterial; };
	inline MaterialComponent* GetActualMaterial() { return actual; };

	void setTextColor(float Red, float Green, float Blue);
	float3 GetTextColor();

	inline void SetAlpha(float Alpha) { alpha = Alpha; };
	inline float GetAlpha() { return alpha; };
	MyPlane* planeToDraw;
	void loadFont(const char* path);
	uint VAO = 0, VBO = 0;
	std::map<char, Character> characters;
	Shadert* shader = nullptr;
	std::string GetSelect(); 

	int GetSelectedID() { return selectedRaw; };
	void SetSelectedID(int id) { selectedRaw = id; };
	bool GetChangeState() { return changeSelected; };
	void SetChangeState(bool change) { changeSelected = change; };

private:

	bool changeSelected = false;

	void SetFocusedButtons();
	void UpdateButtons(GameObject* auxiliar);
	ComponentTransform2D* transform;
	std::vector<GameObject*> buttonsArray;
	bool isDeployed = false;
	float alpha = 1.0f;
	Text DropDownText;
	int selectedRaw = 0;
	State state = State::NORMAL;
	float3 textPos = { 0,0,0 };
	MaterialComponent* normalMaterial;
	MaterialComponent* focusedMaterial;
	MaterialComponent* pressedMaterial;
	MaterialComponent* disabledMaterial;
	MaterialComponent* actual;
	std::string fontPath;
	Color textColor = white;
	Color generalColor = white;
	char text[64] = "DropDown";
	// SP=0 / ING=1
	std::vector<std::string> optionsLenguaje = {"Castellano","English"};
	float fontScale = 1;
	std::vector<GameObject*> hitObjs;
	GameObject* focusedGameObject;
};
