#pragma once
#include "Component.h"
#include "Color.h"
#include<vector>

class MyPlane;
class GameObject;
class MaterialComponent;
class ImageComponent : public Component
{
public:
	ImageComponent(GameObject* own);
	~ImageComponent();

	bool Update(float dt) override;
	void Draw(CameraComponent* gameCam = nullptr) override;
	void OnEditor() override;

	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	void SetActualColor(float Red, float Green, float Blue);
	inline Color GetActualColor() { return actualColor; };
	inline std::string GetText() { return text; };

	inline void SetAlpha(float Alpha) { alpha = Alpha; };
	inline float GetAlpha() { return alpha; };

	int LoadTexture(std::string newTexture);
	void UseTexture(int ID);
	MyPlane* planeToDraw;
private:
	MaterialComponent* principal;
	std::vector<MaterialComponent*> materialList;
	float alpha = 1.0f;
	State state = State::NORMAL;
	Color actualColor = white;
	std::string text;
};