#include "BillboardParticleComponent.h"

#include "ModuleSceneManager.h"
#include "Scene.h"

#include "glew/include/GL/glew.h"
#include "SDL/include/SDL_opengl.h"
#include <gl/GLU.h>
#include <gl/GL.h>
#include "Math/float3x3.h"

#include "Profiling.h"

BillboardParticleComponent::BillboardParticleComponent(GameObject* own, TransformComponent* trans) :
	particleAlignment(Particle_Alignment::CAMERA_ALIGNED),
	isHorizontalAligned(true)
{
	type = ComponentType::BILLBOARD;
	owner = own;
	transform = trans;
}

BillboardParticleComponent::~BillboardParticleComponent()
{
}

bool BillboardParticleComponent::Update(float dt)
{
	return true;
}

void BillboardParticleComponent::OnEditor()
{
	if (ImGui::CollapsingHeader("Billboard"))
	{
		ImGui::PushItemWidth(90);
		ImGui::Separator();

		std::string tempAlignment = " ";

		switch (particleAlignment) {
		case Particle_Alignment::CAMERA_ALIGNED: 
			tempAlignment = "Camera Aligned"; 
			break;
		case Particle_Alignment::WORLD_ALIGNED: 
			tempAlignment = "World Aligned"; 
			break;
		case Particle_Alignment::AXIS_ALIGNED: 
			tempAlignment = "Axis Aligned"; 
			break;
		}

		ImGui::Text("Current Alignment: %s", tempAlignment.c_str());

		if (ImGui::BeginMenu("Change particle's alignment")) {

			if (ImGui::MenuItem("Camera Aligned"))
				SetAlignment(Particle_Alignment::CAMERA_ALIGNED);
			if (ImGui::MenuItem("World Aligned"))
				SetAlignment(Particle_Alignment::WORLD_ALIGNED);
			if (ImGui::MenuItem("Axis Aligned"))
				SetAlignment(Particle_Alignment::AXIS_ALIGNED);

			ImGui::EndMenu();
		}

		if (particleAlignment == Particle_Alignment::AXIS_ALIGNED) {
			std::string tempName = (isHorizontalAligned ? "Horizontal" : "Vertical");
			ImGui::Text("Current Aligned Axis: %s", tempName.c_str());

			if (ImGui::BeginMenu("Change aligned axis")) {
				if (ImGui::MenuItem("Horizontal")) 
					isHorizontalAligned = true;
				if (ImGui::MenuItem("Vertical")) 
					isHorizontalAligned = false;

				ImGui::EndMenu();
			}
		}

		ImGui::Spacing();
		ImGui::Spacing();
		ComponentOptions(this);
		ImGui::Separator();
	}
}

void BillboardParticleComponent::Align()
{
	switch (particleAlignment)
	{
	case Particle_Alignment::CAMERA_ALIGNED:
		CameraAlign();
		break;
	case Particle_Alignment::WORLD_ALIGNED:
		WorldAlign();
		break;
	case Particle_Alignment::AXIS_ALIGNED:
		AxisAlign();
		break;
	default:
		break;
	}
}

void BillboardParticleComponent::SetAlignment(Particle_Alignment alignment)
{
	particleAlignment = alignment;
}

Quat BillboardParticleComponent::GetAlignment()
{
	switch (particleAlignment)
	{
	case Particle_Alignment::CAMERA_ALIGNED:
		return CameraAlign();
		break;
	case Particle_Alignment::WORLD_ALIGNED:
		return WorldAlign();
		break;
	case Particle_Alignment::AXIS_ALIGNED:
		return AxisAlign();
		break;
	default:
		break;
	}

	return Quat::identity;
}

Quat BillboardParticleComponent::CameraAlign()
{
	//Frustum* camFrustum = app->scene->mainCamera->GetFrustum();
	//float3 billboardForward = (camFrustum->Pos() - transform->GetGlobalTransform().TranslatePart()).Normalized();

	//float3 up = camFrustum->Up();
	//float3 right = up.Cross(billboardForward);
	////up = billboardForward.Cross(right);

	//float3x3 mat = float3x3::identity;
	//mat.Set(right.x, right.y, right.z, up.x, up.y, up.z, billboardForward.x, billboardForward.y, billboardForward.z);

	//Quat ret = mat.Inverted().ToQuat();
	//return ret;

	Frustum* camFrust = app->sceneManager->GetCurrentScene()->mainCamera->GetFrustum();

	if (app->sceneManager->GetGameState() == GameState::NOT_PLAYING)
		camFrust = &app->camera->cameraFrustum;

	float3 billboardForward = (camFrust->pos - transform->GetGlobalTransform().TranslatePart()).Normalized();

	float3 up = camFrust->up;
	float3 right = up.Cross(billboardForward);
	up = billboardForward.Cross(right);

	float3x3 mat = float3x3::identity;
	mat.Set(right.x, right.y, right.z, up.x, up.y, up.z, billboardForward.x, billboardForward.y, billboardForward.z);

	Quat ret = mat.Transposed().ToQuat();
	return ret;
}

Quat BillboardParticleComponent::WorldAlign()
{
	// TODO: particles always face camera, but they can be softly rotated
	return Quat::identity;
}

Quat BillboardParticleComponent::AxisAlign()
{
	return Quat::identity;
}