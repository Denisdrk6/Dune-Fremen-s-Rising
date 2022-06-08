#pragma once
#include "Component.h"
#include "Shapes.h"

class GameObject;
class btRigidBody;
class TransformComponent;
enum class Axis;

enum class CollisionType
{
	BOX = 0,
	SPHERE = 1,
	CAPSULE = 2,
	CYLINDER = 3,
	CONE = 4,
	STATIC_PLANE = 5,
	MESH = 6
};

class RigidBodyComponent : public Component
{
public:
	RigidBodyComponent(GameObject* obj, CollisionType type = CollisionType::BOX, float mass = 1.0f,  bool isKinematic = false);
	void IgnoreCollision();
	virtual ~RigidBodyComponent();

	void SetBoundingBox();
	bool Update(float dt) override;
	void UpdateCollision();

	void OnEditor() override;
	void Combos();

	void AddConstraintP2P(RigidBodyComponent* const& val);

	void ResetLocalValues();
	void EditCollisionMesh();
	void UpdateCollisionMesh();

	void CreateBody(bool changeShape = true);
	void SetPhysicsProperties();
	inline float GetMass() { return mass; };
	void SetMass(float mass);

	void SetAsStatic();
	void SetAsTrigger();
	float4x4 btScalarTofloat4x4(btScalar* transform);

	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;
	
	inline btRigidBody* GetBody() { return body; };
	inline CollisionType GetCollisionType() { return collisionType; };
	void SetCollisionType(CollisionType type);

	inline void SetOnCollision(bool ret) { onCollision = ret; };
	inline bool GetOnCollision() { return onCollision; };

	inline void SetCollisionTarget(RigidBodyComponent* obj) { collisionTarget = obj; };
	inline RigidBodyComponent* GetCollisionTarget() { return collisionTarget; };
	
	// Used specifictly on scripting. (Future deleting)
	void SetCollisionSphere(float sphereRadius, float3 pos);
	void SetHeight(float height);

public:
	bool useGravity = true;
	bool isKinematic = false;
	bool trigger = false;
	bool fActivation = false;

	std::vector<RigidBodyComponent*> constraintBodies;
	std::vector<int> bodiesUIDs;
	std::vector<RigidBodyComponent*> triggerList;

private:
	btRigidBody* body = nullptr;
	CollisionType collisionType = CollisionType::BOX;

	float mass = 1.0f;
	float friction = 1.0f;
	float restitution = 0.2f;

	// Block the movement and rotation
	float3 movementConstraint = float3::one;
	float3 rotationConstraint = float3::one;

	// Having linear damping at zero means objects will keep moving until friction slows them down.
	float linearDamping = 0.0f;
	float angularDamping = 0.0f;

	// Figures
	PCube box;
	PSphere sphere;
	PCapsule capsule;
	PCylinder cylinder;
	PPyramid cone;
	PPlane plane;

	TransformComponent* ownerTransform;

	int cylinderAxis = 3;

	bool editMesh = false;
	bool onCollision = false;

	RigidBodyComponent* collisionTarget = nullptr;
};
