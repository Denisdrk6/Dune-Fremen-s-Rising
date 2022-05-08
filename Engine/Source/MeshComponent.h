#pragma once
#include "Component.h"
#include "Geometry/AABB.h"
#include "Bone.h"
#include <map>

class TransformComponent;
class MaterialComponent;
class Mesh;
class Resource;

class MeshComponent : public Component
{
public:
	MeshComponent(GameObject* own, TransformComponent* trans);
	MeshComponent(MeshComponent* meshComponent, TransformComponent* trans);
	~MeshComponent();

	void Draw(CameraComponent* gameCam = nullptr) override;
	void DrawOutline(CameraComponent* gameCam, const float3& color) override;
	void OnEditor() override;

	void MenuChangeMesh();

	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	void SetMesh(std::shared_ptr<Resource> m);

	void CalculateCM();
	bool HasMaterial();

	inline void SetTransform(TransformComponent* trans) { transform = trans; }
	inline void SetMaterial(MaterialComponent* mat) { material = mat; }

	inline AABB GetLocalAABB() { return localBoundingBox; }
	const std::shared_ptr<Mesh> GetMesh() const { return mesh; }
	const std::map<std::string, BoneInfo>& GetBoneMap();

private:
	TransformComponent* transform;
	MaterialComponent* material;

	float normalLength;
	float3 colorNormal;
	
	bool faceNormals;
	bool verticesNormals;

	std::shared_ptr<Mesh> mesh;

	AABB localBoundingBox;

	bool showMeshMenu = false;
};