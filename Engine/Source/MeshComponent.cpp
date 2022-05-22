#include "MeshComponent.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleCamera3D.h"
#include "ModuleRenderer3D.h"
#include "ModuleSceneManager.h"

#include "Scene.h"

#include "TransformComponent.h"
#include "MaterialComponent.h"
#include "AnimationComponent.h"

#include "FileSystem.h"
#include "ResourceManager.h"
#include "Mesh.h"

#include "Profiling.h"

MeshComponent::MeshComponent(GameObject* own, TransformComponent* trans) : material(nullptr), transform(trans), faceNormals(false), verticesNormals(false), normalLength(1.0f), colorNormal(150.0f, 0.0f, 255.0f)
{
	type = ComponentType::MESH_RENDERER;
	owner = own;
	mesh = nullptr;
	material = owner->GetComponent<MaterialComponent>();
}

MeshComponent::MeshComponent(MeshComponent* meshComponent, TransformComponent* trans) : material(nullptr), showMeshMenu(false)
{
	transform = trans;
	mesh = meshComponent->GetMesh();

	faceNormals = meshComponent->faceNormals;
	verticesNormals = meshComponent->verticesNormals;
	normalLength = meshComponent->normalLength;
	colorNormal = meshComponent->colorNormal;

	localBoundingBox = meshComponent->localBoundingBox;
}

MeshComponent::~MeshComponent()
{
	if (mesh.use_count() - 1 == 1) mesh->UnLoad();
}

void MeshComponent::Draw(CameraComponent* gameCam)
{
	if (material != nullptr && material->GetActive()) material->Bind(gameCam);
	
	if (mesh != nullptr) mesh->Draw(verticesNormals, faceNormals, colorNormal, normalLength);
	
	if (material != nullptr && material->GetActive()) material->Unbind();
}

void MeshComponent::DrawOutline(CameraComponent* gameCam, const float3& color)
{
	if (!mesh)
		return;
	
	glEnableClientState(GL_VERTEX_ARRAY);
	float scaleFactor = 0;
	float cmMultiplier = 0;
#ifdef DIST
	scaleFactor = 1.0f;
	cmMultiplier = 0.0001f;
#else
	scaleFactor = 1.05f;
	cmMultiplier = 0.05;
#endif

	Quat q = Quat::identity;
	if (owner->GetParent() != app->sceneManager->GetCurrentScene()->GetRoot())
	{
		q = transform->GetRotation() * owner->GetParent()->GetComponent<TransformComponent>()->GetRotation();
	}
	else
	{
		q = transform->GetRotation();
	}
	float4x4 model = float4x4::FromTRS(transform->GetGlobalTransform().Col3(3) - owner->GetOffsetCM() * cmMultiplier, q, transform->GetScale() * scaleFactor);
	float4x4 view = float4x4::identity;
	float4x4 projection = float4x4::identity;
	if (gameCam)
	{
		view = gameCam->matrixViewFrustum;
		projection = gameCam->matrixProjectionFrustum;
	}
	else
	{
		view = app->camera->matrixViewFrustum;
		projection = app->camera->matrixProjectionFrustum;
	}

	std::shared_ptr<Shader> s = material->GetOutlineShader();
	s->Bind();
	s->SetUniformMatrix4f("model", model.Transposed());
	s->SetUniformMatrix4f("view", view.Transposed());
	s->SetUniformMatrix4f("projection", projection.Transposed());
	s->SetUniformVec3f("color", color);
	
	if (AnimationComponent* anim = owner->GetComponent<AnimationComponent>())
	{
		auto transforms = anim->GetFinalBoneMatrices();
		for (int i = 0; i < transforms.size(); ++i)
			s->SetUniformMatrix4f("finalBonesMatrices[" + std::to_string(i) + "]", transforms[i].Transposed());
	}

	mesh->Draw(verticesNormals, faceNormals, colorNormal, normalLength);

	s->Unbind();

	glDisableClientState(GL_VERTEX_ARRAY);
}

void MeshComponent::OnEditor()
{
	ImGui::PushID(this);

	if (ImGui::CollapsingHeader(ICON_FA_CUBES" Mesh Renderer"))
	{
		Checkbox(this, "Active", active);
		ImGui::Text("Select mesh");
		ImGui::SameLine();
		if (ImGui::Button(mesh ? mesh->GetName().c_str() : "No Mesh"))
		{
			showMeshMenu = true;
		}
		ImGui::Text("Number of vertices: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1.0f, 1.0f, 0.0f, 1.0f), "%d", mesh ? mesh->GetVerticesSize() : 0);
		ImGui::Text("Number of indices: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1.0f, 1.0f, 0.0f, 1.0f), "%d", mesh ? mesh->GetIndicesSize() : 0);
		ImGui::Checkbox("Vertices normals", &verticesNormals);
		ImGui::Checkbox("Face normals", &faceNormals);
		ImGui::DragFloat("Normal Length", &normalLength, 0.200f);
		ImGui::DragFloat3("Normal Color", colorNormal.ptr(), 1.0f, 0.0f, 255.0f);
		ImGui::Text("Reference Count: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1.0f, 1.0f, 0.0f, 1.0f), "%d", mesh ? mesh.use_count() : 0);

		ImGui::Checkbox("Show AABB     ", &owner->showAABB);		
		ImGui::SameLine();		
		ImGui::Checkbox("Show OBB", &owner->showOBB);

		ComponentOptions(this);
		ImGui::Separator();
	}

	if (showMeshMenu)
		MenuChangeMesh();

	ImGui::PopID();
}

void MeshComponent::MenuChangeMesh()
{
	ImGui::Begin("Meshes", &showMeshMenu, ImGuiWindowFlags_NoDocking | ImGuiWindowFlags_NoCollapse);
	ImVec2 winPos = ImGui::GetWindowPos();
	ImVec2 size = ImGui::GetWindowSize();
	ImVec2 mouse = ImGui::GetIO().MousePos;
	if (!(mouse.x < winPos.x + size.x && mouse.x > winPos.x &&
		mouse.y < winPos.y + size.y && mouse.y > winPos.y))
	{
		if (ImGui::GetIO().MouseClicked[0]) showMeshMenu = false;
	}

	std::vector<std::shared_ptr<Resource>> res;

	std::vector<std::string> files;
	app->fs->DiscoverFiles("Library/Meshes/", files);
	for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
	{
		if ((*it).find(".rgmesh") != std::string::npos)
		{
			app->fs->GetFilenameWithoutExtension(*it);
			*it = (*it).substr((*it).find_last_of("_") + 1, (*it).length());
			uint uid = std::stoll(*it);
			res.push_back(ResourceManager::GetInstance()->LoadResource(uid));
		}
	}

	std::sort(res.begin(), res.end(), [](std::shared_ptr<Resource> a, std::shared_ptr<Resource> b) {
		return (a.get()->GetName() < b.get()->GetName());
		});

	for (std::vector<std::shared_ptr<Resource>>::iterator it = res.begin(); it != res.end(); ++it)
	{
		if (ImGui::Selectable((*it).get()->GetName().c_str()))
			SetMesh((*it));
	}

	ImGui::End();
}

bool MeshComponent::OnLoad(JsonParsing& node)
{
	mesh = std::static_pointer_cast<Mesh>(ResourceManager::GetInstance()->LoadResource(std::string(node.GetJsonString("Path"))));
	active = node.GetJsonBool("Active");

	if (mesh)
	{
		localBoundingBox.SetNegativeInfinity();
		localBoundingBox.Enclose(mesh->GetPositions().data(), mesh->GetPositions().size());
		owner->SetAABB(localBoundingBox);

		CalculateCM();
	}

	return true;
}

bool MeshComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Path", mesh->GetAssetsPath().c_str());
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Active", active);

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

void MeshComponent::SetMesh(std::shared_ptr<Resource> m)
{
	if (mesh.use_count() - 1 == 1) mesh->UnLoad();
	mesh = std::static_pointer_cast<Mesh>(m);

	if (mesh)
	{
		localBoundingBox.SetNegativeInfinity();
		localBoundingBox.Enclose(mesh->GetPositions().data(), mesh->GetPositions().size());
		owner->SetAABB(localBoundingBox);

		CalculateCM();
	}
}

bool MeshComponent::HasMaterial()
{
	if (material) return true;
	
	return false;
}
const std::map<std::string, BoneInfo>& MeshComponent::GetBoneMap()
{
	return mesh->GetBoneMap();
}

void MeshComponent::CalculateCM()
{
	// Calcule CM offset
	float3 posBody = owner->GetOOB().CenterPoint();
	float3 posObj = transform->GetGlobalTransform().Col3(3);
	owner->SetOffsetCM(posBody - posObj);
	owner->SetOffsetCM(quatRotate(transform->GetRotation().Inverted(), owner->GetOffsetCM()));
	owner->SetOffsetCM(quatRotate(transform->GetRotation(), owner->GetOffsetCM()));
}