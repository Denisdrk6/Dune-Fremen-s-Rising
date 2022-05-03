#pragma once
#include "Component.h"
#include "Geometry/Frustum.h"

class TransformComponent;
class IndexBuffer;
class VertexBuffer;

class CameraComponent : public Component
{
public:
	CameraComponent(GameObject* own, TransformComponent* trans);
	~CameraComponent();

	bool Update(float dt) override;
	void Zoom();
	void UpdateRotation();
	void UpdateMovement();

	void Draw(CameraComponent* gameCam = nullptr) override;

	void OnEditor() override;
	void OnEditorMovement();
	void OnEditorShake();

	void SetPlanes();
	void CalculateVerticalFov(float horizontalFovRadians, float width, float height);
	void UpdateFovAndScreen(float width, float height);
	void UpdateFov();

	void SetNewFov(float horizontalFovRadians);

	void CompileBuffers();
	bool CompareRotations(Quat& quat1, Quat& quat2);
	void RequestShake(float strength, float duration, int smothness);

	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	int CameraComponent::ContainsAaBox(const AABB& boundingBox);
	inline Frustum* GetFrustum() { return &camera; };

	inline float GetCurrentScreenWidth() { return currentScreenWidth; }
	inline float GetCurrentScreenHeight() { return currentScreenHeight; }

	float4x4 ViewMatrixOpenGL();

	float4x4 matrixViewFrustum;
	float4x4 matrixProjectionFrustum;

	// Scrpting
	void ScriptMovement(float x, float y, float z);
	void ScriptRotation(float x, float y, float z);
	void ScriptRotation(float ang);
	float GetAngle();
	void LockControlls(bool lock) { lockControlls = lock; };
	float GetZoom();
	void ScriptZoom(float zoom);

private:
	void Shake(float dt);

	Frustum camera;
	TransformComponent* transform;
	Quat currentRotation;
	float3 originalPos;

private:

	float nearPlane;
	float farPlane;
	float verticalFov = 45;
	float horizontalFov = 45;

	float currentScreenWidth;
	float currentScreenHeight;

	IndexBuffer* ebo;
	VertexBuffer* vbo;

	//----------------------------------------------
	float zoomSpeed = 1.0f;
	float zoom = 10.0f;
	float zoomMin = 0.0f;
	float zoomMax = 30.0f;

	bool fixingToTarget = false;
	bool freeMovement = true;
	bool followTarget = false;

	GameObject* defTarget = nullptr; // FreeMovement Target
	GameObject* target = nullptr;
	TransformComponent* controllerTrans = nullptr;

	float movementSpeed = 0.05f;
	float rotationSpeed = 0.5f;
	float radius = 20.0f;
	float horizontalAngle = 0.0f;

	//int targetUID = 0;

	float shakeStrength = 0.6f;
	float shakeDuration = 1.0f;
	bool shake = false;
	int easeType = 0;
	float elapsedTime = 0.0f;
	float currentStrength = 0.0f;

	// Limits
	float4 limits;

	//Lock
	bool lockControlls = true;
};