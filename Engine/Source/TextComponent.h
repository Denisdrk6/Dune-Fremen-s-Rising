#pragma once

#include "Component.h"
#include "Color.h"
#include "Text.h"
#include "ModuleUI.h"
#include <map>
class SDL_Rect;
class SDL_Texture;
class MyPlane;
class GameObject;
class Character;
class Shadert;
class ComponentTransform2D;
class MaterialComponent;
typedef unsigned int uint;

class TextComponent : public Component
{
public:

	TextComponent(GameObject* own);
	~TextComponent();

	bool Update(float dt) override;
	void Draw(CameraComponent* gameCam = nullptr) override;
	void OnEditor() override;
	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	float2 GetParentPosition();

	std::string GetText() { return textToShow.textt; };
	void SetText(char* newText)
	{		
		textToShow.setOnlyText(newText);
		//strcpy(text, newText);
	}
	void setTextColor(float Red, float Green, float Blue);
	float3 GetTextColor();
	Text textToShow;
	
	uint VAO = 0, VBO = 0;
	void loadFont(std::string path);
	std::map<char, Character> characters;
	Shadert* shader = nullptr;
private:
	std::string fontPath;
	void loadtext(std::string path);
	void savetext();
	bool fadeUI = false;
	Color textColor = white;
	MyPlane* planeToDraw;
	//char text[358] = "Text";
	float fontScale = 0.5;
	std::string fileText = "-";
	
	// Components to get at start
	ComponentTransform2D* ownerTransform2DComponent;
	MaterialComponent* ownerMaterialComponent;
};
