#include "ModuleCamera3D.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleInput.h"
#include "ModuleEditor.h"
#include "ModuleNavMesh.h"
#include "ModuleSceneManager.h"
#include "Scene.h"

#include "TransformComponent.h"
#include "MeshComponent.h"
#include "Mesh.h"

#include "Viewport.h"
#include "GameView.h"
#include "Geometry/LineSegment.h"
#include "Geometry/Triangle.h"

#include "Profiling.h"

ModuleCamera3D::ModuleCamera3D(bool startEnabled) : horizontalFov(DegToRad(70.0f)), verticalFov(0.0f), nearPlane(0.5f), farPlane(777.0f), Module(startEnabled), updateViewPort(true)
{
	name = "Camera3D";
	cameraFrustum.SetKind(FrustumProjectiveSpace::FrustumSpaceGL, FrustumHandedness::FrustumRightHanded);
	cameraFrustum.SetViewPlaneDistances(nearPlane, farPlane);
	CalculateVerticalFov(horizontalFov, SCREEN_WIDTH, SCREEN_HEIGHT);
	cameraFrustum.SetPerspective(horizontalFov, verticalFov);
	cameraFrustum.SetFrame(float3(0.0f, 1.5f, 5.0f), float3(0.0f, 0.0f, -1.0f), float3(0.0f, 1.0f, 0.0f));
	
	visualizeFrustum = false;
}

ModuleCamera3D::~ModuleCamera3D()
{}

bool ModuleCamera3D::CleanUp()
{
	DEBUG_LOG("Cleaning camera");
	return true;
}

bool ModuleCamera3D::Start()
{
	DEBUG_LOG("Setting up the camera");
	return true;
}

bool ModuleCamera3D::Update(float dt)
{
	RG_PROFILING_FUNCTION("Module Camera Update");

	if (dt > 0.1)
		return true;

	if (updateViewPort)
	{
		// Auxiliar variables
		float3 newPos = cameraFrustum.Pos();
		float3 newFront = cameraFrustum.Front();
		float3 newUp = cameraFrustum.Up();

		float speed = 15.0f * dt;
		if (app->input->GetKey(SDL_SCANCODE_LSHIFT) == KeyState::KEY_REPEAT) speed *= 2;

		// Inputs for the camera
		if (app->input->GetKey(SDL_SCANCODE_T) == KeyState::KEY_UP)
		{
			newUp = float3::unitY;
			newFront = -float3::unitZ;
		}
		if (app->input->GetMouseButton(SDL_BUTTON_RIGHT) == KeyState::KEY_REPEAT)
		{
			if (app->input->GetKey(SDL_SCANCODE_W) == KeyState::KEY_REPEAT) newPos += cameraFrustum.Front() * speed;
			if (app->input->GetKey(SDL_SCANCODE_S) == KeyState::KEY_REPEAT) newPos -= cameraFrustum.Front() * speed;

			if (app->input->GetKey(SDL_SCANCODE_A) == KeyState::KEY_REPEAT) newPos -= cameraFrustum.WorldRight() * speed;
			if (app->input->GetKey(SDL_SCANCODE_D) == KeyState::KEY_REPEAT) newPos += cameraFrustum.WorldRight() * speed;

			if (app->input->GetKey(SDL_SCANCODE_Q) == KeyState::KEY_REPEAT) newPos -= cameraFrustum.Up() * speed;
			if (app->input->GetKey(SDL_SCANCODE_E) == KeyState::KEY_REPEAT) newPos += cameraFrustum.Up() * speed;

			RotateAround(dt, newFront, newUp);
		}

		OrbitAround(dt, newUp, newFront, newPos);
		Focus(newFront, newUp, newPos);
		MousePicking(newPos, newFront, speed);

		cameraFrustum.SetFrame(newPos, newFront, newUp);
		CalculateViewMatrix();
	}

	return true;
}

void ModuleCamera3D::MousePicking(math::float3& newPos, math::float3& newFront, float speed)
{
	// Get size scene
	float4 size = app->editor->GetViewport()->GetBounds();
	float2 pos(app->input->GetMouseX(), app->input->GetMouseY());

	// Chaeck if mouse is focused on scene tab
	if (app->editor->GetViewport()->GetState())
	{
		if (app->input->GetMouseZ() == 1) newPos += newFront * speed * 10;
		if (app->input->GetMouseZ() == -1) newPos -= newFront * speed * 10;

		if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_UP && !ImGuizmo::IsUsing())
		{
			float2 mousePos = { (float)app->input->GetMouseX(), (float)app->input->GetMouseY() };
			mousePos.x = 2 * ((mousePos.x - size.x) / (size.z)) - 1.0f;
			mousePos.y = -(2 * ((mousePos.y - (size.y + 10.0f)) / (size.w)) - 1.0f);

			// Build RayCast
			LineSegment picking = cameraFrustum.UnProjectLineSegment(mousePos.x, mousePos.y);
			rayCastToDraw = picking.ToLine();

			// Send ray to NavMesh
			if (app->sceneManager->GetGameState() == GameState::PLAYING)
				app->navMesh->CheckNavMeshIntersection(picking, SDL_BUTTON_LEFT);

			//DEBUG_LOG("POSITION X %f, POSITION Y %f", mousePos.x, mousePos.y);
			//DEBUG_LOG("SIZE X %f, SIZE Y %f", size.x, size.y);

			// Fill gameObjects list 
			std::stack<QuadtreeNode*> nodes;
			app->sceneManager->GetCurrentScene()->GetQuadtree().CollectNodes(nodes, picking);
			std::set<GameObject*> gameObjects;
			app->sceneManager->GetCurrentScene()->GetQuadtree().CollectGo(gameObjects, nodes);

			std::map<float, GameObject*> triangleMap;
			float3 hit;
			ThrowRayCast(gameObjects, picking, triangleMap, hit);

			if (!triangleMap.empty()) app->editor->SetGO((*triangleMap.begin()).second);
			else if (triangleMap.empty() && !ImGuizmo::IsUsing())
			{
				app->editor->SetGO(nullptr);
				app->editor->SetSelectedParent(nullptr);
			}
		}
	}
}

void ModuleCamera3D::ThrowRayCast(std::set<GameObject*>& gameObjects, math::LineSegment& picking, std::map<float, GameObject*>& triangleMap, float3& hitPoint)
{
	LineSegment prevLine = picking;
	for (std::set<GameObject*>::iterator it = gameObjects.begin(); it != gameObjects.end(); ++it)
	{
		TransformComponent* transform = (*it)->GetComponent<TransformComponent>();
		if ((*it)->GetAABB().IsFinite() && transform)
		{
			picking = prevLine;
			if (picking.Intersects((*it)->GetOOB()))
			{
				MeshComponent* meshComponent = (*it)->GetComponent<MeshComponent>();
				if (meshComponent)
				{
					int size = 0;
					if (meshComponent->GetMesh())
						size = meshComponent->GetMesh()->GetIndicesSize();

					if (size > 0)
					{
						float distance = 0.0f;
						math::vec hitP = { 0.0f, 0.0f, 0.0f };
						const std::vector<Vertex>& meshVertices = meshComponent->GetMesh()->GetVerticesVector();
						const std::vector<unsigned int>& meshIndices = meshComponent->GetMesh()->GetIndicesVector();

						picking.Transform(transform->GetGlobalTransform().Inverted());
						for (int i = 0; i < size; i += 3)
						{
							// TODO: Is this ok?
							const math::Triangle tri(meshVertices[meshIndices[i]].position, meshVertices[meshIndices[i + 1]].position, meshVertices[meshIndices[i + 2]].position);
							if (picking.Intersects(tri, &distance, &hitP))
							{
								triangleMap[distance] = (*it);
								if ((*triangleMap.begin()).second == (*it))
									hitPoint = prevLine.a + prevLine.Dir() * distance * prevLine.Length();
								break;
							}
						}
					}
				}
			}
		}
	}
	if (hitPoint.Equals(float3::zero))
		hitPoint = prevLine.b;
}

void ModuleCamera3D::ThrowRayCastOnlyOBB(std::set<GameObject*>& gameObjects, math::LineSegment& picking, std::map<float, GameObject*>& aabbMap, float3& hitPoint)
{
	LineSegment prevLine = picking;
	float dNear, dFar = 0;
	for (std::set<GameObject*>::iterator it = gameObjects.begin(); it != gameObjects.end(); ++it)
	{
		TransformComponent* transform = (*it)->GetComponent<TransformComponent>();
		if ((*it)->GetAABB().IsFinite() && transform)
		{
			picking = prevLine;
			if (picking.Intersects((*it)->GetOOB(), dNear, dFar))
			{
				aabbMap[dFar * prevLine.Length()] = (*it);
				if ((*aabbMap.begin()).second == (*it))
					hitPoint = prevLine.a + prevLine.Dir() * dFar * prevLine.Length();
				break;
			}		
		}
	}

	if (hitPoint.Equals(float3::zero))
		hitPoint = prevLine.b;
}

void ModuleCamera3D::RotateAround(float dt, math::float3& newFront, math::float3& newUp)
{
	float dX = -app->input->GetMouseXMotion();
	float dY = -app->input->GetMouseYMotion();
	if (dY != 0)
	{
		Quat rotateVertical;
		rotateVertical = rotateVertical.RotateAxisAngle(cameraFrustum.WorldRight().Normalized(), dY * dt * 2);
		newFront = rotateVertical * newFront;
		newUp = rotateVertical * newUp;
		newFront.Normalize();
		newUp.Normalize();
		float3::Orthonormalize(newFront, newUp);
	}
	if (dX != 0)
	{
		Quat rotateHorizontal;
		rotateHorizontal = rotateHorizontal.RotateY(dX * dt);
		newFront = rotateHorizontal * newFront;
		newUp = rotateHorizontal * newUp;
		newFront.Normalize();
		newUp.Normalize();
		float3::Orthonormalize(newFront, newUp);
	}
}

void ModuleCamera3D::OrbitAround(float& dt, math::float3& newUp, math::float3& newFront, math::float3& newPos)
{
	if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_REPEAT &&
		app->input->GetKey(SDL_SCANCODE_LALT) == KeyState::KEY_REPEAT)
	{
		GameObject* target = app->editor->GetGO();
		float dX = -app->input->GetMouseXMotion();
		float dY = -app->input->GetMouseYMotion();

		if (target != nullptr)
		{
			float3 targetPos;
			Quat targetRot;
			float3 targetSize;
			target->GetComponent<TransformComponent>()->GetGlobalTransform().Decompose(targetPos, targetRot, targetSize);
			float3 distanceTarget = cameraFrustum.Pos() - targetPos;

			Quat rotateOrbitY;
			rotateOrbitY = rotateOrbitY.RotateY(dX * dt);
			rotateOrbitY.Normalize();
			//distanceTarget = rotateOrbitY * distanceTarget;

			Quat rotateOrbitX;
			rotateOrbitX = rotateOrbitX.RotateAxisAngle(cameraFrustum.WorldRight().Normalized(), dY * dt);
			rotateOrbitX.Normalize();
			newUp = rotateOrbitX * newUp;
			newUp.Normalize();
			newUp = rotateOrbitY * newUp;
			newUp.Normalize();
			distanceTarget = rotateOrbitX * rotateOrbitY * distanceTarget;
			newFront = distanceTarget.Normalized().Neg();
			newPos = distanceTarget + targetPos;
			float3::Orthonormalize(newFront, newUp);
		}
	}
}

void ModuleCamera3D::Focus(math::float3& newFront, math::float3& newUp, math::float3& newPos)
{
	if (app->input->GetKey(SDL_SCANCODE_F) == KeyState::KEY_UP && app->input->GetKey(SDL_SCANCODE_LSHIFT) == KeyState::KEY_IDLE)
	{
		GameObject* objSelected = app->editor->GetGO();
		if (objSelected != nullptr)
		{
			if (MeshComponent* mesh = objSelected->GetComponent<MeshComponent>())
			{
				float3 meshCenter = objSelected->GetOffsetCM();
				newFront = (meshCenter - cameraFrustum.Pos()).Normalized();
				newUp = newFront.Cross(float3(0.0f, 1.0f, 0.0f).Cross(newFront).Normalized());
				const float meshRadius = mesh->GetLocalAABB().HalfDiagonal().Length();
				const float currentDistance = meshCenter.Distance(cameraFrustum.Pos());
				newPos = meshCenter + ((cameraFrustum.Pos() - meshCenter).Normalized() * meshRadius * 2);
			}
			else
			{
				float3 pivot = objSelected->GetComponent<TransformComponent>()->GetGlobalTransform().Col3(3);
				newFront = (pivot - cameraFrustum.Pos()).Normalized();
				newUp = newFront.Cross(float3(0.0f, 1.0f, 0.0f).Cross(newFront).Normalized());
			}
		}
	}
}

void ModuleCamera3D::LookAt(float3& target)
{
	float3 directionFrustum = target - cameraFrustum.Pos();
	directionFrustum.Normalize();

	float3x3 lookAt = float3x3::LookAt(cameraFrustum.Front(), directionFrustum, cameraFrustum.Up(), float3(0.0f, 1.0f, 0.0f));
	cameraFrustum.SetFront(lookAt.MulDir(cameraFrustum.Front()).Normalized());
	cameraFrustum.SetUp(lookAt.MulDir(cameraFrustum.Up()).Normalized());
}

void ModuleCamera3D::CalculateVerticalFov(float horizontalFovRadians, float width, float height)
{
	verticalFov = 2 * Atan((Tan(horizontalFovRadians / 2)) * (height / width));
	cameraFrustum.SetVerticalFovAndAspectRatio(verticalFov, (width / height));
	currentScreenHeight = height;
	currentScreenWidth = width;
}

void ModuleCamera3D::CalculateViewMatrix()
{
	matrixProjectionFrustum = cameraFrustum.ComputeProjectionMatrix();
	matrixViewFrustum = cameraFrustum.ComputeViewMatrix();
}

void ModuleCamera3D::UpdateFovAndScreen(float width, float height)
{
	if (currentScreenHeight != height || currentScreenWidth != width)
	{
		verticalFov = 2 * Atan((Tan(horizontalFov / 2)) * (height / width));
		cameraFrustum.SetVerticalFovAndAspectRatio(verticalFov, (width / height));
		currentScreenHeight = height;
		currentScreenWidth = width;
	}
}

void ModuleCamera3D::UpdateFov()
{
	verticalFov = 2 * Atan((Tan(horizontalFov / 2)) * (currentScreenHeight / currentScreenWidth));
	cameraFrustum.SetVerticalFovAndAspectRatio(verticalFov, (currentScreenWidth / currentScreenHeight));
}

void ModuleCamera3D::SetPlanes()
{
	cameraFrustum.SetViewPlaneDistances(nearPlane, farPlane);
}

int ModuleCamera3D::ContainsAaBox(const AABB& boundingBox)
{
	
	if (boundingBox.IsFinite())
	{
		if (cameraFrustum.Contains(boundingBox))
		{
			return 1;

		}
		else if (cameraFrustum.Intersects(boundingBox))
		{
			return 2;
		}
		return 0;
	}
	return -1;
}

bool ModuleCamera3D::LoadConfig(JsonParsing& node)
{
	//float3 pos;
	//float3 front;
	//float3 up;

	//pos.x = node.GetJsonNumber("PosX");
	//pos.y = node.GetJsonNumber("PosY");
	//pos.z = node.GetJsonNumber("PosZ");

	//front.x = node.GetJsonNumber("FrontX");
	//front.y = node.GetJsonNumber("FrontY");
	//front.z = node.GetJsonNumber("FrontZ");

	//up.x = node.GetJsonNumber("UpX");
	//up.y = node.GetJsonNumber("UpY");
	//up.z = node.GetJsonNumber("UpZ");

	//cameraFrustum.SetFrame(pos, front, up);
	//matrixViewFrustum = cameraFrustum.ViewMatrix();
	//matrixProjectionFrustum = cameraFrustum.ProjectionMatrix();

	return true;
}

bool ModuleCamera3D::SaveConfig(JsonParsing& node)
{
	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "PosX", cameraFrustum.Pos().x);
	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "PosY", cameraFrustum.Pos().y);
	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "PosZ", cameraFrustum.Pos().z);

	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "FrontX", cameraFrustum.Front().x);
	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "FrontY", cameraFrustum.Front().y);
	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "FrontZ", cameraFrustum.Front().z);

	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "UpX", cameraFrustum.Up().x);
	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "UpY", cameraFrustum.Up().y);
	//node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "UpZ", cameraFrustum.Up().z);

	return true;
}