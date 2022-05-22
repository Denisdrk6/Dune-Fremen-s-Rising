#pragma once
#include "Component.h"

class Light;
class TransformComponent;

class ComponentLight : public Component
{
public:
	ComponentLight(GameObject* own);
	virtual ~ComponentLight();

	bool Update(float dt) override;
	void OnEditor() override;

	void SetLight(Light* light);
	void SetAsPointLight();
	void SetAsSpotLight();

	Light* GetLight();

	virtual bool OnLoad(JsonParsing& node) override;
	virtual bool OnSave(JsonParsing& node, JSON_Array* array) override;

private:
	Light* light = nullptr;
	TransformComponent* ownerTransform;
};