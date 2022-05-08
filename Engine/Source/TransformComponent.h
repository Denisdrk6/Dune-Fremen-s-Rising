#pragma once
#include "Component.h"
#include "Math/float4x4.h"
#include "Math/float3x3.h"
#include "glmath.h"

enum class Operation
{
	TRANSLATE,
	ROTATE,
	SCALE
};

enum class Mode
{
	LOCAL,
	WORLD
};

class TransformComponent : public Component
{
public:
	TransformComponent(GameObject* owner);
	TransformComponent(TransformComponent* trans);
	~TransformComponent();

	bool Update(float dt) override;
	void OnEditor() override;

	void SetTransform(float3 pos, Quat rot, float3 sca);
	void SetTransform(float4x4 trMatrix);

	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	void UpdateTransform();
	void UpdateChildTransform(GameObject* go);
	void NewAttachment();
	void SetAABB();
	void UpdateBoundingBox();

	void AlignWithView();
	void AlignViewWithSelected();

	inline float4x4 GetLocalTransform() const { return localMatrix; }
	inline float4x4 GetGlobalTransform() const { return globalMatrix; }
	inline float3 GetPosition() const { return position; }
	inline float3 GetGlobalPosition() const { return globalMatrix.Col3(3); }
	inline Quat GetRotation() const { return rotation; }
	inline float3 GetScale() const { return scale; }
	inline float3 GetRotEditor() const { return rotationEditor; }

	inline void SetPosition(const float3& newPosition) { position = newPosition; }
	void SetGlobalPosition(const float3 pos);
	inline void SetRotation(const Quat& newRotation) { rotation = newRotation; }
	inline void SetScale(const float3& newScale) { scale = newScale; }

	bool DrawVec3(std::string& name, float3& vec);
	void ShowTransformationInfo();
	void ResetTransform();

	inline float3 GetForward() { return globalMatrix.RotatePart().Col(2).Normalized(); };
	inline float3 GetRight() { return globalMatrix.RotatePart().Col(0).Normalized(); };
	inline float3 GetUp() { return globalMatrix.RotatePart().Col(1).Normalized(); };

	// UNDO
	inline void ForceUpdateTransform() { changeTransform = true; }
	inline void UpdateEditorRotation() { rotationEditor = rotation.ToEulerXYZ(); };
	// UNDO
	Mat4x4 float4x4ToMat4x4();

private:
	float3 position = float3::zero;
	Quat rotation = Quat::identity;
	float3 rotationInEuler;
	float3 rotationEditor = float3::zero;
	float3 scale = float3::one;

	float4x4 globalMatrix;
	float4x4 localMatrix;

	bool changeTransform = false;
};