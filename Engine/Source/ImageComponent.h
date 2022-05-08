#pragma once
#include "Component.h"
#include "Color.h"
#include<vector>

class MyPlane;
class GameObject;
class MaterialComponent;
class UIAnimation {
public:
	std::vector<MaterialComponent*> images;
	bool isPlayng = false;
	bool loop = false;
	float currentDt = 0;
	float timeBetwen = 0;
	void Update(float dt);
	MaterialComponent* Draw();
	bool isStatic = false;
	int animatonState = -1;
	void StartAnim();
	void StopAnim();
	void ChageVelocity(float velocity);
	void SetStaticimage(int image);
};
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
	MaterialComponent* principal;
	std::vector<UIAnimation*> animations;
private:
	
	std::vector<MaterialComponent*> materialList;
	
	float alpha = 1.0f;
	State state = State::NORMAL;
	Color actualColor = white;
	std::string text;
};