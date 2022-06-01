#include "C_RigidBody.h"
#include "Application.h"
#include "Globals.h"

#include "Physics3D.h"
#include "ModuleSceneManager.h"
#include "Scene.h"

#include "TransformComponent.h"
#include "MeshComponent.h"

#include "ImGui/imgui_stdlib.h"
#include "btBulletDynamicsCommon.h"
#include "Math/float3x3.h"

#include "Profiling.h"

RigidBodyComponent::RigidBodyComponent(GameObject* obj, CollisionType type, float mass, bool isKinematic) : Component(), collisionType(type), mass(mass), isKinematic(isKinematic)
{
	owner = obj;
	this->type = ComponentType::RIGID_BODY;
	ownerTransform = owner->GetComponent<TransformComponent>();

	SetCollisionType(type);
}

RigidBodyComponent::~RigidBodyComponent()
{
	if (body != nullptr)
	{
		app->physics->DeleteBody(this, owner->name);
		constraintBodies.clear();
	}	
}

//Function to make colliders of the same GameObject ignore one another
void RigidBodyComponent::IgnoreCollision()
{
	RigidBodyComponent* comp = nullptr;
	Component* component = nullptr;
	for (int i = 0; i < owner->GetComponents().size(); i++)
	{
		component = owner->GetComponents().at(i);
		if (component->type == ComponentType::RIGID_BODY &&	component != this)
		{
			comp = static_cast<RigidBodyComponent*>(component);

			body->setIgnoreCollisionCheck(comp->body, true);
			comp->body->setIgnoreCollisionCheck(body, true);
		}
	}
}

//Function that calculates the size of the mesh and creates a collision of the same dimensions
void RigidBodyComponent::SetBoundingBox()
{
	float3 pos, radius, size;

	if (owner->GetComponent<MeshComponent>() == nullptr)
	{
		pos = ownerTransform->GetGlobalTransform().Col3(3);
		radius = { 1,1,1 };
		size = { 1,1,1 };
	}
	else
	{
		OBB obb = owner->GetOOB();
		pos = obb.CenterPoint();
		radius = obb.r;
		size = obb.Size();
	}		
	
	switch (collisionType)
	{
	case CollisionType::BOX:
		box.FromRS(ownerTransform->GetRotation(), {1,1,1});
		box.SetPos(pos);
		box.size = size;
		break;
	case CollisionType::SPHERE:
		sphere.FromRS(ownerTransform->GetRotation(), { 1,1,1 });
		sphere.SetPos(pos);
		sphere.radius = radius.MaxElement();
		break;
	case CollisionType::CAPSULE:
		capsule.FromRS(ownerTransform->GetRotation(), { 1,1,1 });
		capsule.SetPos(pos);
		capsule.radius = radius.MaxElementXZ();
		capsule.height = size.y;
		break;
	case CollisionType::CYLINDER:
		cylinder.FromRS(ownerTransform->GetRotation(), { 1,1,1 });
		cylinder.SetPos(pos);
		cylinder.radius = radius.MinElement();
		cylinder.height = size.z;
		break;
	case CollisionType::CONE:
		cone.FromRS(ownerTransform->GetRotation(), { 1,1,1 });
		cone.SetPos(pos);
		cone.radius = radius.MaxElementXZ();
		cone.height = size.y;
		break;
	case CollisionType::STATIC_PLANE:
		plane.FromRS(ownerTransform->GetRotation(), { 1,1,1 });
		plane.SetPos(pos);
		break;
	default:
		break;
	}
}

//When the engine state is "playing" the GameObject follows the RigidBody
bool RigidBodyComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Rigidbody Update");

	if (app->sceneManager->GetGameState() == GameState::PLAYING)
	{
		if (trigger)
		{
			btTransform t;
			t.setBasis(float3x3::FromQuat(ownerTransform->GetRotation()));
			t.setOrigin(ownerTransform->GetGlobalTransform().Col3(3) + owner->GetOffsetCM());

			body->setWorldTransform(t);
		}
		else if (collisionType != CollisionType::MESH && (body->getActivationState() == ACTIVE_TAG || body->getActivationState() == WANTS_DEACTIVATION || body->getActivationState() == DISABLE_DEACTIVATION))
		{
			float4x4 CM2 = float4x4::FromTRS(body->getCenterOfMassPosition() - owner->GetOffsetCM(), body->getWorldTransform().getRotation(), ownerTransform->GetScale());
			ownerTransform->SetTransform(CM2);
		}
	}

	return true;
}

//Function to update the position of the collider in the center of the GameObject
void RigidBodyComponent::UpdateCollision()
{
	if (app->sceneManager->GetGameState() != GameState::PLAYING)
	{
		btTransform t;
		t.setBasis(float3x3::FromQuat(ownerTransform->GetRotation()));
		if (collisionType == CollisionType::MESH)
			t.setOrigin(ownerTransform->GetGlobalTransform().Col3(3));
		else t.setOrigin(ownerTransform->GetGlobalTransform().Col3(3) + owner->GetOffsetCM());
		
		body->setWorldTransform(t);
	}
}

//Function ImGui to draw/edit parameters
void RigidBodyComponent::OnEditor()
{
	// Important condition: body != nullptr 
	// Because OnStop the body don't exist but OnPlay can modify her parameters
	if (ImGui::CollapsingHeader(ICON_FA_ATOM" Rigid Body"))
	{
		if (ImGui::Checkbox("Active    ", &active))
		{
			if (active) app->physics->ActivateCollision(body);
			else app->physics->DesactivateCollision(body);
		}
		ImGui::SameLine();
		static const char* collisions[] = { "Box", "Sphere", "Capsule", "Cylinder", "Cone", "Plane", "Mesh"};

		ImGui::PushItemWidth(85);
		int currentCollision = (int)collisionType;
		if (ImGui::Combo("Collision Type", &currentCollision, collisions, 7))
		{
			collisionType = (CollisionType)currentCollision;
			SetCollisionType(collisionType);
		}
		ImGui::PopItemWidth();

		ImVec2 winSize = ImGui::GetWindowSize();
		winSize.x -= 130;

		ImGui::Text("Mass:        "); ImGui::SameLine();
		ImGui::PushItemWidth(winSize.x);
		if (ImGui::DragFloat("##Mass", &mass, 0.1f, 0.0f, INFINITE))
		{
			if (body->isStaticObject() && mass != 0.0f)
			{
				useGravity = true;
				CreateBody();
			}
			if (mass != 0.f)
			{
				btVector3 inertia;
				body->getCollisionShape()->calculateLocalInertia(mass, inertia);
				body->setMassProps(mass, inertia);
			}
		}
		ImGui::PopItemWidth();

		ImGui::Text("Friction:    "); ImGui::SameLine();
		ImGui::PushItemWidth(winSize.x);
		if (ImGui::DragFloat("##Friction", &friction, 0.05f, 0.f, 1.f) && body)
			body->setFriction(friction);
		ImGui::PopItemWidth();

		ImGui::Text("Restitution: "); ImGui::SameLine();
		ImGui::PushItemWidth(winSize.x);
		if (ImGui::DragFloat("##Restitution", &restitution, 0.1f, 0.f, 1.f) && body)
			body->setRestitution(restitution);
		ImGui::PopItemWidth();

		if (collisionType != CollisionType::MESH && ImGui::Checkbox("Use Gravity", &useGravity))
		{
			if (!useGravity && !isKinematic)
				SetAsStatic();
			else
			{
				if (mass == 0) mass = 1.0f;
				CreateBody();
			}
		}
		if (ImGui::Checkbox("Is Kinematic", &isKinematic))
		{
			if (isKinematic)
			{
				body->setCollisionFlags(body->getFlags() | btCollisionObject::CF_KINEMATIC_OBJECT);
				if (app->sceneManager->GetGameState() == GameState::PLAYING)
					body->setActivationState(DISABLE_DEACTIVATION);
			}
			else CreateBody();
		}
		if (ImGui::Checkbox("Is Trigger", &trigger))
		{
			if (trigger)
				SetAsTrigger();
			else
				CreateBody();
		}
		if (ImGui::Checkbox("Force Activation", &fActivation))
		{
			if (fActivation)
				body->setActivationState(DISABLE_DEACTIVATION);
			else
				body->setActivationState(WANTS_DEACTIVATION);
		}
		ImGui::Text("OnCollision: %s", onCollision ? "true" : "false");
		Combos();

		ComponentOptions(this);
		ImGui::Separator();
	}
}

//pt.2
void RigidBodyComponent::Combos()
{
	ImGuiStyle& style = ImGui::GetStyle();
	ImVec4 colors = style.Colors[ImGuiCol_Border];

	ImGui::PushStyleColor(ImGuiCol_Header, IM_COL32(0, 0, 0, 0));
	ImGui::PushStyleColor(ImGuiCol_Border, IM_COL32(0, 0, 0, 0));
	if (ImGui::CollapsingHeader("Collision mesh"))
	{
		ImGui::PushStyleColor(ImGuiCol_Border, colors);
		EditCollisionMesh();
		ImGui::PopStyleColor();
		ImGui::Separator();
	}
	if (ImGui::CollapsingHeader("Constraints"))
	{
		ImGui::PushStyleColor(ImGuiCol_Border, colors);

		ImGui::Text("Freeze Position:");
		if (ImGui::DragFloat3("##FreezePosition", &movementConstraint.x, 0.05f, 0.f, 1.f) && body)
			body->setLinearFactor(movementConstraint);

		ImGui::Text("Freeze Rotation:");
		if (ImGui::DragFloat3("##FreezeRotation", &rotationConstraint.x, 0.05f, 0.f, 1.f) && body)
			body->setAngularFactor(rotationConstraint);

		ImGui::NewLine();
		ImGui::Text("Add P2P:"); ImGui::SameLine();
		ImGui::PushItemWidth(150);
		if (ImGui::BeginCombo("##AddConstraint", "Select body"))
		{
			for (int i = 0; i < app->physics->GetBodiesNames().size(); i++)
			{
				if (owner->GetName() != app->physics->GetBodiesNames().at(i))
				{
					if (ImGui::Selectable(app->physics->GetBodiesNames().at(i).c_str()))
					{
						constraintBodies.push_back(app->physics->GetBodies().at(i));
						app->physics->GetBodies().at(i)->constraintBodies.push_back(this);
						AddConstraintP2P(app->physics->GetBodies().at(i));
					}
				}				
			}
			ImGui::EndCombo();
		}
		ImGui::PopItemWidth();

		ImGui::Text("List of constraint P2P:");
		if (constraintBodies.size() == 0) ImGui::TextColored(ImVec4(1.00f, 1.00f, 1.00f, 0.50f), "-List is empty");
		for (int i = 0; i < constraintBodies.size(); i++)
		{
			ImGui::PushID(i);
			ImGui::InputText("##Name", &constraintBodies.at(i)->owner->name, ImGuiInputTextFlags_::ImGuiInputTextFlags_ReadOnly);
			ImGui::PopID();
		}

		ImGui::PopStyleColor();
		ImGui::Separator();
	}
	if (ImGui::CollapsingHeader("Damping"))
	{
		ImGui::PushStyleColor(ImGuiCol_Border, colors);

		ImGui::Text("Linear Damping:");
		if (ImGui::DragFloat("##LinearDamping", &linearDamping, 0.05f, 0.f, 1.f) && body)
			body->setDamping(linearDamping, angularDamping);

		ImGui::Text("Angular Damping:");
		if (ImGui::DragFloat("##AngularDamping", &angularDamping, 0.05f, 0.f, 1.f) && body)
			body->setDamping(linearDamping, angularDamping);

		ImGui::PopStyleColor();
	}
	ImGui::PopStyleColor();
	ImGui::PopStyleColor();
}

void RigidBodyComponent::AddConstraintP2P(RigidBodyComponent* const& val)
{	
	btVector3 center;
	float r1, r2;
	body->getCollisionShape()->getBoundingSphere(center, r1);
	val->GetBody()->getCollisionShape()->getBoundingSphere(center, r2);
	app->physics->AddConstraintP2P(*body, *val->GetBody(), float3(r1, r1, r1), float3(r2, r2, r2));
}

void RigidBodyComponent::SetCollisionType(CollisionType type)
{
	if (type == CollisionType::MESH)
	{
		useGravity = false;
		isKinematic = false;
		mass = 0.0f;
	}
		
	collisionType = type;
	CreateBody();	
}

void RigidBodyComponent::ResetLocalValues()
{
	LOG(LogType::L_NORMAL, "%f, %f, %f",box.size[0], box.size[1], box.size[2]);
	box.size = { 1,1,1 };
	capsule.radius = 1;
	capsule.height = 2;
	cylinder.radius = 1;
	cylinder.height = 2;
	cone.radius = 1;
	cone.height = 2;
}

//Function ImGui to draw/edit parameters of the shape
void RigidBodyComponent::EditCollisionMesh()
{
	switch (collisionType)
	{
	case CollisionType::BOX:
		ImGui::Text("Box Size: ");
		if (ImGui::DragFloat3("##Box", &box.size.x, 0.1f, 0.1f, INFINITE)) editMesh = true;
		break;
	case CollisionType::SPHERE:
		ImGui::Text("Radius: "); ImGui::SameLine();
		if (ImGui::DragFloat("##Radius", &sphere.radius, 0.1f, 0.1f, INFINITE)) editMesh = true;
		break;
	case CollisionType::CAPSULE:
		ImGui::Text("Radius: "); ImGui::SameLine();
		if (ImGui::DragFloat("##Radius", &capsule.radius, 0.1f, 0.1f, INFINITE)) editMesh = true;
		ImGui::Text("Height: "); ImGui::SameLine();
		if (ImGui::DragFloat("##Height", &capsule.height, 0.1f, 0.1f, INFINITE)) editMesh = true;
		break;
	case CollisionType::CYLINDER:
		ImGui::Text("Radius: "); ImGui::SameLine();
		if (ImGui::DragFloat("##Radius", &cylinder.radius, 0.1f, 0.1f, INFINITE)) editMesh = true;
		ImGui::Text("Height: "); ImGui::SameLine();
		if (ImGui::DragFloat("##Height", &cylinder.height, 0.1f, 0.1f, INFINITE)) editMesh = true;

		if (ImGui::RadioButton("X", (Axis)cylinderAxis == Axis::X)) {
			cylinderAxis = (int)Axis::X;
			CreateBody();
		}
		ImGui::SameLine();
		if (ImGui::RadioButton("Y", (Axis)cylinderAxis == Axis::Y)) {
			cylinderAxis = (int)Axis::Y;
			CreateBody();
		}
		ImGui::SameLine();
		if (ImGui::RadioButton("Z", (Axis)cylinderAxis == Axis::Z)) {
			cylinderAxis = (int)Axis::Z;
			CreateBody();		
		}

		break;
	case CollisionType::CONE:
		ImGui::Text("Radius: "); ImGui::SameLine();
		if (ImGui::DragFloat("##Radius", &cone.radius, 0.1f, 0.1f, INFINITE)) editMesh = true;
		ImGui::Text("Height: "); ImGui::SameLine();
		if (ImGui::DragFloat("##Height", &cone.height, 0.1f, 0.1f, INFINITE)) editMesh = true;
		break;
	case CollisionType::STATIC_PLANE:
		ImGui::Text("Normal: ");
		if (ImGui::DragFloat3("##Normal", &plane.normal.x, 0.1f, 0.0f, 1))
		{
			if (plane.normal.x == 0.0f && plane.normal.y == 0.0f && plane.normal.z == 0.0f) plane.normal.y = 0.1f;
			editMesh = true;
		}
		ImGui::Text("Constant: "); ImGui::SameLine();
		if (ImGui::DragFloat("##Constant", &plane.constant, 0.1f)) editMesh = true;
		break;
	case CollisionType::MESH:
		ImGui::Text("Don't can modified collision mesh");
		break;
	default:
		break;
	}

	if (editMesh)
	{
		UpdateCollisionMesh();
	}
}

//Function to update dimensions of the RigidBody when parameters have been changed
void RigidBodyComponent::UpdateCollisionMesh()
{
	switch (body->getCollisionShape()->getShapeType())
	{
	case BOX_SHAPE_PROXYTYPE:
		body->getCollisionShape()->setLocalScaling(box.size);
		break;
	case SPHERE_SHAPE_PROXYTYPE:
		static_cast<btSphereShape*>(body->getCollisionShape())->setUnscaledRadius(sphere.radius);
		break;
	case CAPSULE_SHAPE_PROXYTYPE:
		body->getCollisionShape()->setLocalScaling(btVector3(capsule.radius, capsule.height * 0.5f, capsule.radius));
		break;
	case CYLINDER_SHAPE_PROXYTYPE:
		if ((Axis)cylinderAxis == Axis::X)
			body->getCollisionShape()->setLocalScaling(btVector3(cylinder.height * 0.5f, cylinder.radius, 0.0f));
		if ((Axis)cylinderAxis == Axis::Y)
			body->getCollisionShape()->setLocalScaling(btVector3(cylinder.radius, cylinder.height * 0.5f, 0.0f));
		if ((Axis)cylinderAxis == Axis::Z)
			body->getCollisionShape()->setLocalScaling(btVector3(cylinder.radius, 0.0f, cylinder.height * 0.5f));
		break;
	case CONE_SHAPE_PROXYTYPE:
		body->getCollisionShape()->setLocalScaling(btVector3(cone.radius, cone.height * 0.5f, cone.radius));
		break;
	case STATIC_PLANE_PROXYTYPE:
		CreateBody(false);
		break;
	default:
		break;
	}
	editMesh = false;
}

float4x4 RigidBodyComponent::btScalarTofloat4x4(btScalar* transform)
{
	float4x4 newTransform;
	int k = 0;
	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			newTransform[j][i] = transform[k];
			k++;
		}
	}

	return newTransform;
}

void RigidBodyComponent::CreateBody(bool changeShape)
{
	if (body != nullptr)
		app->physics->DeleteBody(this, owner->name);

	if (changeShape) SetBoundingBox();

	switch (collisionType)
	{
	case CollisionType::BOX:
		body = app->physics->CollisionShape(box, this);
		break;
	case CollisionType::SPHERE:
		body = app->physics->CollisionShape(sphere, this);
		break;
	case CollisionType::CAPSULE:
		body = app->physics->CollisionShape(capsule, this);
		break;
	case CollisionType::CYLINDER:
		body = app->physics->CollisionShape(cylinder, this, (Axis)cylinderAxis);
		break;
	case CollisionType::CONE:
		body = app->physics->CollisionShape(cone, this);
		break;
	case CollisionType::STATIC_PLANE:
		body = app->physics->CollisionShape(plane, this);
		break;
	case CollisionType::MESH:
		body = app->physics->CollisionShape(owner->GetComponent<MeshComponent>()->GetMesh(), this);
		break;
	default:
		break;
	}

	SetPhysicsProperties();
	ResetLocalValues();
	IgnoreCollision();
}

void RigidBodyComponent::SetPhysicsProperties()
{
	if (body != nullptr)
	{
		body->setFriction(friction);
		body->setRestitution(restitution);
		body->setLinearFactor(movementConstraint);
		body->setAngularFactor(rotationConstraint);
		body->setDamping(linearDamping, angularDamping);
	}
}

void RigidBodyComponent::SetMass(float mass)
{
	btVector3 localInertia(0, 0, 0);
	if (mass != 0.f)
		body->getCollisionShape()->calculateLocalInertia(mass, localInertia);
	body->setMassProps(mass, localInertia);
	this->mass = mass;
}

void RigidBodyComponent::SetAsStatic()
{
	useGravity = false;
	isKinematic = false;
	mass = 0.0f;
	CreateBody();
}

void RigidBodyComponent::SetAsTrigger()
{
	body->setCollisionFlags(body->getFlags() | btCollisionObject::CF_NO_CONTACT_RESPONSE);
	body->setActivationState(DISABLE_DEACTIVATION);
	fActivation = true;
	trigger = true;
}

bool RigidBodyComponent::OnLoad(JsonParsing& node)
{
	active = node.GetJsonBool("Active");

	//Collision Type
	collisionType = (CollisionType)(int)node.GetJsonNumber("CollisionType");
	//Physic properties
	useGravity = node.GetJsonBool("Gravity");
	isKinematic = node.GetJsonBool("Kinematic");
	trigger = node.GetJsonBool("Trigger");
	fActivation = node.GetJsonBool("fActivation");
	mass = node.GetJsonNumber("Mass");
	friction = node.GetJsonNumber("Friction");
	restitution = node.GetJsonNumber("Restitution");

	//Damping
	linearDamping = node.GetJsonNumber("LinearDamping");
	angularDamping = node.GetJsonNumber("AngularDamping");

	//Constrains
	movementConstraint = node.GetJson3Number(node, "MovementConstraint");
	rotationConstraint = node.GetJson3Number(node, "RotationConstraint");

	CreateBody();
	switch (collisionType)
	{
	case CollisionType::BOX:
		box.size = node.GetJson3Number(node, "Size");
		break;
	case CollisionType::SPHERE:
		sphere.radius = node.GetJsonNumber("Radius");
		sphere.sectors = node.GetJsonNumber("Sectors");
		sphere.stacks = node.GetJsonNumber("Stacks");
		break;
	case CollisionType::CAPSULE:
		capsule.radius = node.GetJsonNumber("Radius");
		capsule.height = node.GetJsonNumber("Height");
		break;
	case CollisionType::CYLINDER:
		cylinder.radius = node.GetJsonNumber("Radius");
		cylinder.height = node.GetJsonNumber("Height");
		cylinder.sectorCount = node.GetJsonNumber("Sectors");
		break;
	case CollisionType::CONE:
		cone.radius = node.GetJsonNumber("Radius");
		cone.height = node.GetJsonNumber("Height");
		break;
	case CollisionType::STATIC_PLANE:
		plane.normal = node.GetJson3Number(node, "Normal");
		plane.constant = node.GetJsonNumber("Constant");
		break;
	default:
		break;
	}
	UpdateCollisionMesh();
	SetPhysicsProperties();

	if (fActivation) body->setActivationState(DISABLE_DEACTIVATION);

	uint size = node.GetJsonNumber("SizeConstraint");
	if (size > 0)
	{
		JSON_Array* array = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "ConstraintBodies");

		for (int i = 0; i < size; i++)
		{
			bodiesUIDs.push_back(json_array_get_number(array, i));
		}
	}

	return true;
}

bool RigidBodyComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);

	//Collision dimensions
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "CollisionType", (int)collisionType);
	switch (collisionType)
	{
	case CollisionType::BOX:
		file.SetNewJson3Number(file, "Size", box.size);
		break;
	case CollisionType::SPHERE:
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Radius", sphere.radius);
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Sectors", sphere.sectors);
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Stacks", sphere.stacks);
		break;
	case CollisionType::CAPSULE:
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Radius", capsule.radius);
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Height", capsule.height);
		break;
	case CollisionType::CYLINDER:
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Radius", cylinder.radius);
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Height", cylinder.height);
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Sectors", cylinder.sectorCount);
		break;
	case CollisionType::CONE:
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Radius", cone.radius);
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Height", cone.height);
		break;
	case CollisionType::STATIC_PLANE:
		file.SetNewJson3Number(file, "Normal", plane.normal);
		file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Constant", plane.constant);
		break;
	default:
		break;
	}

	//Collision physics
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Gravity", useGravity);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Kinematic", isKinematic);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Trigger", trigger);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "fActivation", fActivation);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Mass", mass);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Friction", friction);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Restitution", restitution);

	//Constrains
	file.SetNewJson3Number(file, "MovementConstraint", movementConstraint);
	file.SetNewJson3Number(file, "RotationConstraint", rotationConstraint);

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "SizeConstraint", constraintBodies.size());
	if (constraintBodies.size() > 0)
	{
		JSON_Array* arrayConstrains = file.SetNewJsonArray(file.GetRootValue(), "ConstraintBodies");
		for (int i = 0; i < constraintBodies.size(); i++)
			json_array_append_number(arrayConstrains, constraintBodies.at(i)->owner->GetUUID());
	}

	//Damping
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "LinearDamping", linearDamping);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "AngularDamping", angularDamping);


	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Active", active);

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

// Use specificly in Scripting
void RigidBodyComponent::SetCollisionSphere(float sphereRadius, float3 pos)
{
	collisionType = CollisionType::SPHERE;
	sphere.radius = sphereRadius;
	sphere.SetPos(pos);
	useGravity = false;
	isKinematic = false;
	mass = 0.0f;
	CreateBody(false);
}

void RigidBodyComponent::SetHeight(float height)
{
	if (height != 1) 
	{
		float3 offset(0, owner->GetAABB().HalfSize().y * (1-height), 0);
		PCapsule capAux = capsule;
		OBB obb = owner->GetOOB();
		float3 pos = body->getCenterOfMassPosition();

		capsule.transform.Rotate(body->getWorldTransform().getRotation().getAngle() * RADTODEG, Vec3(0,1,0));
		capsule.SetPos(pos.x, pos.y - offset.y, pos.z);
		capsule.radius *= obb.r.MaxElementXZ();
		capsule.height *= obb.Size().y * 0.5 * height - 0.1f;

		CreateBody(false);
		owner->SetOffsetCM(owner->GetOffsetCM() - offset);
		capsule = capAux;
	}
	else
	{
		float radius = capsule.radius;
		float height = capsule.height;

		CreateBody();
		body->getCollisionShape()->setLocalScaling(btVector3(radius, height * 0.5f, radius));
		capsule.radius = radius;
		capsule.height = height;

		owner->GetComponent<MeshComponent>()->CalculateCM();
	}
	body->setActivationState(DISABLE_DEACTIVATION);
	fActivation = true;
}