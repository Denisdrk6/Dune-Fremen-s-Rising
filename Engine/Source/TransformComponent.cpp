#include "TransformComponent.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleCamera3D.h"
#include "ModuleSceneManager.h"
#include "Scene.h"

#include "C_RigidBody.h"
#include "MeshComponent.h"
#include "ListenerComponent.h"
#include "AudioSourceComponent.h"
#include "AudioReverbZoneComponent.h"
#include "ParticleSystemComponent.h"

#include "CommandsDispatcher.h"
#include "GameObjectCommands.h"

#include "Imgui/imgui_internal.h"
#include "Profiling.h"

TransformComponent::TransformComponent(GameObject* own)
{
	type = ComponentType::TRANSFORM;
	owner = own;
	localMatrix = float4x4::FromTRS(position, rotation, scale);

	if (owner->GetParent() != nullptr)
	{
		TransformComponent* tr = owner->GetParent()->GetComponent<TransformComponent>();
		if (tr != nullptr)
			globalMatrix = localMatrix * tr->GetGlobalTransform();
	}
	else
		globalMatrix = localMatrix;

	active = true;
}

TransformComponent::TransformComponent(TransformComponent* trans)
{
	type = ComponentType::TRANSFORM;

	position = trans->GetPosition();
	rotation = trans->GetRotation();
	scale = trans->GetScale();
	globalMatrix = trans->GetGlobalTransform();
	localMatrix = trans->GetLocalTransform();
	rotationEditor = trans->GetRotEditor();
}

TransformComponent::~TransformComponent()
{
}

bool TransformComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Transform Update");

	if (changeTransform)
	{
		std::stack<GameObject*> stack;
		UpdateTransform();

		//Get each RigidBodies of the GameObject to update their position
		for (int i = 0; i < owner->GetComponents().size(); i++)
			if (owner->GetComponents().at(i)->type == ComponentType::RIGID_BODY)
				static_cast<RigidBodyComponent*>(owner->GetComponents().at(i))->UpdateCollision();

		for (int i = 0; i < owner->GetChilds().size(); ++i)
			stack.push(owner->GetChilds()[i]);

		while (!stack.empty())
		{
			GameObject* go = stack.top();

			UpdateChildTransform(go);
			
			//Get each RigidBodies of the GameObject childs to update their position
			for (int i = 0; i < go->GetComponents().size(); i++)
				if (go->GetComponents().at(i)->type == ComponentType::RIGID_BODY)
					static_cast<RigidBodyComponent*>(go->GetComponents().at(i))->UpdateCollision();

			stack.pop();

			for (int i = 0; i < go->GetChilds().size(); ++i)
				stack.push(go->GetChilds()[i]);
		}

		SetAABB();

		ListenerComponent* listener = owner->GetComponent<ListenerComponent>();
		if (listener != nullptr)
			listener->ChangePosition();

		AudioSourceComponent* audioSource = owner->GetComponent<AudioSourceComponent>();
		if (audioSource != nullptr)
			audioSource->ChangePosition();

		AudioReverbZoneComponent* reverb = owner->GetComponent<AudioReverbZoneComponent>();
		if (reverb != nullptr)
			reverb->ChangePosition();

		changeTransform = false;
	}

	return true;
}

void TransformComponent::OnEditor()
{
	if (ImGui::CollapsingHeader(ICON_FA_ARROWS_ALT" Transform"))
	{
		ImGui::PushItemWidth(90);
		
		ShowTransformationInfo();

		if (ImGui::Button(ICON_FA_UNDO" Reset Transform"))
			ResetTransform();

		ImGui::Separator();
	}
}

void TransformComponent::SetTransform(float3 pos, Quat rot, float3 sca)
{
	position = pos;
	rotation = rot;
	scale = sca;

	globalMatrix = float4x4::FromTRS(position, rotation, scale);	
	DEBUG_LOG("This is %s", owner->GetName());
}

void TransformComponent::SetTransform(float4x4 trMatrix)
{
	globalMatrix = trMatrix;
	globalMatrix.Decompose(position, rotation, scale);
	
	TransformComponent* trans = owner->GetParent()->GetComponent<TransformComponent>();
	if (trans)
	{
		localMatrix = trans->globalMatrix.Inverted() * globalMatrix;
		localMatrix.Decompose(position, rotation, scale);
	}

	changeTransform = true;
}

bool TransformComponent::OnLoad(JsonParsing& node)
{
	active = node.GetJsonBool("Active");
	position = node.GetJson3Number(node, "Position");
	float4 quat = node.GetJson4Number(node, "Quaternion");
	rotation = Quat(quat.x, quat.y, quat.z, quat.w);
	scale = node.GetJson3Number(node, "Scale");
	rotationEditor = node.GetJson3Number(node, "RotationEditor");

	UpdateTransform();
	changeTransform = true;

	return true;
}

bool TransformComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Active", active);
	file.SetNewJson3Number(file, "Position", position);
	file.SetNewJson4Number(file, "Quaternion", rotation);
	file.SetNewJson3Number(file, "RotationEditor", rotationEditor);
	file.SetNewJson3Number(file, "Scale", scale);

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

void TransformComponent::UpdateTransform()
{
	localMatrix = float4x4::FromTRS(position, rotation, scale);

	if (owner->GetParent() && owner->GetParent() != app->sceneManager->GetCurrentScene()->GetRoot())
	{
		TransformComponent* parentTr = owner->GetParent()->GetComponent<TransformComponent>();
		if (parentTr) globalMatrix = parentTr->globalMatrix * localMatrix;
	}
	else
		globalMatrix = localMatrix;

	UpdateBoundingBox();
}

void TransformComponent::UpdateChildTransform(GameObject* go)
{
	TransformComponent* transform = go->GetComponent<TransformComponent>();
	GameObject* parent = go->GetParent();
	TransformComponent* parentTrans = parent->GetComponent<TransformComponent>();

	if (transform)
		transform->globalMatrix = parentTrans->GetGlobalTransform() * transform->localMatrix;
}

void TransformComponent::NewAttachment()
{
	if (owner->GetParent() != app->sceneManager->GetCurrentScene()->GetRoot())
		localMatrix = owner->GetParent()->GetComponent<TransformComponent>()->GetGlobalTransform().Inverted().Mul(globalMatrix);
	
	localMatrix.Decompose(position, rotation, scale);
	changeTransform = true;
	//eulerRotation = rotation.ToEulerXYZ();
}

void TransformComponent::SetAABB()
{
	std::vector<GameObject*> goList = owner->GetChilds();
	owner->ClearAABB();
	OBB childOBB;
	for (int i = 0; i < goList.size(); ++i)
	{
		TransformComponent* tr = goList[i]->GetComponent<TransformComponent>();
		if(tr) tr->SetAABB();
	}

	UpdateBoundingBox();

	app->sceneManager->GetCurrentScene()->RedistributeQuadtree(owner);
}

void TransformComponent::UpdateBoundingBox()
{
	if (owner->GetComponent<MeshComponent>())
	{
		OBB newObb = owner->GetComponent<MeshComponent>()->GetLocalAABB().ToOBB();
		newObb.Transform(globalMatrix);
		owner->SetAABB(newObb);
		//owner->GetComponent<MeshComponent>()->CalculateCM();
	}
	else
	{
		ParticleSystemComponent* partComp = owner->GetComponent<ParticleSystemComponent>();
		if (partComp)
			partComp->UpdateAABB();
	}

}

bool TransformComponent::DrawVec3(std::string& name, float3& vec)
{
	float3 lastVec = vec;
	ImGui::PushID(name.c_str());

	ImGui::Columns(2);
	ImGui::SetColumnWidth(0, 100.0f);
	ImGui::Text(name.c_str());
	ImGui::NextColumn();

	ImGui::PushMultiItemsWidths(3, ImGui::CalcItemWidth());
	ImGui::PushStyleVar(ImGuiStyleVar_ItemSpacing, { 0,0 });

	ImGui::PushStyleColor(ImGuiCol_Button, ImVec4(0.8f, 0.1f, 0.15f, 1.0f));
	ImGui::PushStyleColor(ImGuiCol_ButtonActive, ImVec4(0.8f, 0.1f, 0.15f, 1.0f));
	ImGui::PushStyleColor(ImGuiCol_ButtonHovered, ImVec4(0.8f, 0.1f, 0.15f, 1.0f));
	ImGui::Button("X");
	ImGui::PopStyleColor(3);

	ImGui::SameLine();
	ImGui::DragFloat("##X", &vec.x, 0.1f, 0.0f, 0.0f, "%.2f");
	if (ImGui::IsItemActivated())
		CommandDispatcher::Execute(new MoveGameObjectCommand(owner));
	ImGui::PopItemWidth();
	ImGui::SameLine();

	ImGui::PushStyleColor(ImGuiCol_Button, ImVec4(0.2f, 0.7f, 0.2f, 1.0f));
	ImGui::PushStyleColor(ImGuiCol_ButtonActive, ImVec4(0.2f, 0.7f, 0.2f, 1.0f));
	ImGui::PushStyleColor(ImGuiCol_ButtonHovered, ImVec4(0.2f, 0.7f, 0.2f, 1.0f));
	ImGui::Button("Y");
	ImGui::PopStyleColor(3);

	ImGui::SameLine();
	ImGui::DragFloat("##Y", &vec.y, 0.1f, 0.0f, 0.0f, "%.2f");
	if (ImGui::IsItemActivated())
		CommandDispatcher::Execute(new MoveGameObjectCommand(owner));
	ImGui::PopItemWidth();
	ImGui::SameLine();

	ImGui::PushStyleColor(ImGuiCol_Button, ImVec4(0.1f, 0.25f, 0.8f, 1.0f));
	ImGui::PushStyleColor(ImGuiCol_ButtonActive, ImVec4(0.1f, 0.25f, 0.8f, 1.0f));
	ImGui::PushStyleColor(ImGuiCol_ButtonHovered, ImVec4(0.1f, 0.25f, 0.8f, 1.0f));
	ImGui::Button("Z");
	ImGui::PopStyleColor(3);

	ImGui::SameLine();
	ImGui::DragFloat("##Z", &vec.z, 0.1f, 0.0f, 0.0f, "%.2f");
	if (ImGui::IsItemActivated())
		CommandDispatcher::Execute(new MoveGameObjectCommand(owner));
	ImGui::PopItemWidth();

	ImGui::PopStyleVar();

	ImGui::Columns(1);

	ImGui::PopID();

	if (lastVec.x != vec.x || lastVec.y != vec.y || lastVec.z != vec.z) 
		return true;
	else return false;
}

void TransformComponent::ShowTransformationInfo()
{
	if (DrawVec3(std::string("Position: "), position)) changeTransform = true;

	rotationInEuler.x = RADTODEG * rotationEditor.x;
	rotationInEuler.y = RADTODEG * rotationEditor.y;
	rotationInEuler.z = RADTODEG * rotationEditor.z;
	if (DrawVec3(std::string("Rotation: "), rotationInEuler))
	{
		rotationInEuler.x = DEGTORAD * rotationInEuler.x;
		rotationInEuler.y = DEGTORAD * rotationInEuler.y;
		rotationInEuler.z = DEGTORAD * rotationInEuler.z;

		Quat rotationDelta = Quat::FromEulerXYZ(rotationInEuler.x - rotationEditor.x, rotationInEuler.y - rotationEditor.y, rotationInEuler.z - rotationEditor.z);
		rotation = rotation * rotationDelta;
		rotationEditor = rotationInEuler;
		
		changeTransform = true;
	}

	if (DrawVec3(std::string("Scale: "), scale)) changeTransform = true;
}

void TransformComponent::ResetTransform() 
{
	SetTransform(math::float3::zero, math::Quat::identity, math::float3::one);
	rotationEditor = rotationInEuler = math::float3::zero;
	UpdateTransform();
}

Mat4x4 TransformComponent::float4x4ToMat4x4()
{
	Mat4x4 newTransform;
	int k = 0;
	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
		{
			newTransform[k] = globalMatrix[j][i];
			k++;
		}
	}

	return newTransform;
}

// Object align with camera
void TransformComponent::AlignWithView()
{
	Frustum frus = app->camera->cameraFrustum;
	globalMatrix.SetTranslatePart(frus.Pos());
	float3x3 rot{ frus.WorldRight(), frus.Up(), frus.Front() };
	globalMatrix.SetRotatePart(rot.ToQuat());
	SetTransform(globalMatrix);
}

// Camera align with object
void TransformComponent::AlignViewWithSelected()
{
	float3x3 rot = globalMatrix.RotatePart();
	app->camera->cameraFrustum.SetFrame(position, rot.Col3(2), rot.Col3(1));
	app->camera->CalculateViewMatrix();
}

void TransformComponent::SetGlobalPosition(const float3& pos)
{
	// Posición relativa al padre
	globalMatrix.SetTranslatePart(float4(pos.x, pos.y, pos.z, 1));
	if (owner->GetParent() != app->sceneManager->GetCurrentScene()->GetRoot())
	{

		localMatrix = owner->GetParent()->GetComponent<TransformComponent>()->globalMatrix.Inverted().Mul(globalMatrix);
		localMatrix.Decompose(position, rotation, scale);
	}
	else
	{
		localMatrix = globalMatrix;
		localMatrix.Decompose(position, rotation, scale);
	}
}