#include "CameraComponent.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleSceneManager.h"
#include "Scene.h"
#include "ModuleInput.h"
#include "ModuleWindow.h"
#include "ModuleEditor.h"
#include "ModuleNavMesh.h"
#include "ModuleCamera3D.h"

#include "GameView.h"
#include "TransformComponent.h"

#include "IndexBuffer.h"
#include "VertexBuffer.h"
#include "Math/float3x3.h"
#include "Geometry/LineSegment.h"
#include "Imgui/ImGuizmo.h"

#include <time.h>
#include "Profiling.h"


CameraComponent::CameraComponent(GameObject* own, TransformComponent* trans) : horizontalFov(DegToRad(90.0f)), verticalFov(0.0f), nearPlane(-100.0f), farPlane(100.0f), transform(trans), currentRotation(0, 0, 0, 1), currentScreenHeight(SCREEN_HEIGHT), currentScreenWidth(SCREEN_WIDTH), vbo(nullptr), ebo(nullptr)
{
	type = ComponentType::CAMERA;
	owner = own;
	camera.SetKind(FrustumProjectiveSpace::FrustumSpaceGL, FrustumHandedness::FrustumRightHanded);
	camera.SetViewPlaneDistances(nearPlane, farPlane);
	CalculateVerticalFov(horizontalFov, currentScreenWidth, currentScreenHeight);
	camera.SetPerspective(horizontalFov, verticalFov);
	camera.SetFrame(float3(0.0f, 0.0f, 0.0f), float3(0.0f, 0.0f, 1.0f), float3(0.0f, 1.0f, 0.0f));
	//camera.SetPos(math::vec(0, 0, 0));

	controllerTrans = owner->GetParent()->GetComponent<TransformComponent>();
	transform->SetPosition(float3(0.0f, 55.0f, -50.0f));

	float verticalAngle = 40; // TODO: TEST VARIOUS CAMERA ANGLES
	transform->SetRotation(Quat::RotateX(DEGTORAD * verticalAngle));

	srand(time(NULL));
	CompileBuffers();
}

CameraComponent::~CameraComponent()
{
	RELEASE(vbo);
	RELEASE(ebo);
}

void CameraComponent::OnEditor()
{
	ImGui::PushID(this);

	if (ImGui::CollapsingHeader(ICON_FA_CAMERA" Camera"))
	{
		ImGui::Checkbox("Lock", &lock);
		ImGui::Text("Field of view");
		ImGui::SameLine();
		static float horizontalFovEditor = RadToDeg(horizontalFov);
		if (ImGui::DragFloat("", &horizontalFovEditor, 1.0f, 1.0f, 179.0f))
		{
			horizontalFov = DegToRad(horizontalFovEditor);
			UpdateFov();
			CompileBuffers();
		}

		ImGui::DragFloat("ZoomMin", &zoomMin, 0.1f);
		ImGui::DragFloat("ZoomMax", &zoomMax, 0.1f);
		ImGui::DragFloat("ZoomSpeed", &zoomSpeed, 0.1f);

		ImGui::Text("Clipping planes");

		ImGui::Text("Near");
		ImGui::SameLine();
		ImGui::PushID("NearPlane");
		if (ImGui::DragFloat("", &nearPlane, 0.5f, 0.1f)) SetPlanes();
		ImGui::PopID();

		ImGui::PushID("farPlane");
		ImGui::Text("Far");
		ImGui::SameLine();
		if (ImGui::DragFloat("", &farPlane, 0.5f, 0.1f)) SetPlanes();
		ImGui::PopID();

		ImGui::Text("horizontal angle: %f", horizontalAngle);

		ImGui::Text("- - - - MOVEMENT - - - -");
		OnEditorMovement();

		ImGui::Text("- - - - SHAKE - - - -");
		OnEditorShake();

		ComponentOptions(this);
		ImGui::Separator();
	}
	ImGui::PopID();
}

void CameraComponent::OnEditorMovement()
{

	/*if (ImGui::Button("Switch camera movement"))
	{
		if (freeMovement)
		{
			freeMovement = false;
			followTarget = true;
		}
		else
		{
			freeMovement = true;
			followTarget = false;
		}
	}*/

	if (freeMovement)
	{
		ImGui::Text("Movement: free");
		ImGui::DragFloat("movementSpeed", &movementSpeed, 0.002f, 0.01f, 1.0f);
	}

	/*else if (followTarget)
	{
		ImGui::Text("Movement: target-locked");

		ImGui::Text("Target:");
		ImGui::SameLine();
		ImGui::Button(target != nullptr ? target->GetName() : "none");
		if (ImGui::BeginDragDropTarget())
		{
			const ImGuiPayload* go = ImGui::AcceptDragDropPayload("HierarchyItem");
			if (go)
			{
				uint uuid = *(const uint*)(go->Data);
				target = app->sceneManager->GetCurrentScene()->GetGoByUuid(uuid);
			}
			ImGui::EndDragDropTarget();
		}
	}*/

	ImGui::DragFloat("rotationSpeed", &rotationSpeed, 0.001f, 0.0f);
	ImGui::DragFloat("Radius", &radius, 0.1f, 0.0f);
}

void CameraComponent::OnEditorShake()
{
	ImGui::DragFloat("shakeStrength", &shakeStrength, 0.1f, 0.0f);
	ImGui::DragFloat("shakeDuration", &shakeDuration, 0.1f, 0.0f);

	if (ImGui::Button("<") && smooth > 0) smooth--;
	ImGui::SameLine();
	ImGui::Button("Smoothness Type");
	ImGui::SameLine();
	if (ImGui::Button(">") && smooth < 3) smooth++;

	if (smooth == 0) ImGui::Text("Smoothnes:: none");
	if (smooth == 1) ImGui::Text("Smoothnes:: in-out");
	if (smooth == 2) ImGui::Text("Smoothnes:: in");
	if (smooth == 3) ImGui::Text("Smoothnes:: out");
	if (ImGui::Button("Shake"))
		RequestShake(shakeStrength, shakeDuration);

	ImGui::DragFloat4("##Limits", limits.ptr());
}

bool CameraComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Camera Component Update");

	float4 size = float4::zero;
#ifndef DIST
	size = app->editor->GetGameView()->GetBounds();
#else
	size = { 0,0, (float)*app->window->GetWindowWidth(), (float)*app->window->GetWindowHeight() };
#endif

	camera.SetFrame(transform->GetGlobalTransform().TranslatePart(), transform->GetGlobalTransform().Col3(2), transform->GetGlobalTransform().Col3(1));

	if (app->camera->updateGameView)
	{
		//TODO: Make the click work properly

		//	DEBUG_LOG("SIZE X %f, SIZE Y Y %f", size.x, size.y);
		float2 pos(app->input->GetMouseX(), app->input->GetMouseY());
		if (app->editor->GetGameView()->GetState() && pos.x > size.x && pos.x < size.x + size.z && pos.y > size.y && pos.y < size.y + size.w)
		{
			if (app->input->GetMouseButton(SDL_BUTTON_LEFT) == KeyState::KEY_UP && !ImGuizmo::IsUsing())
			{
				float2 mousePos = { (float)app->input->GetMouseX(), (float)app->input->GetMouseY() };

				mousePos.x = 2 * ((mousePos.x - size.x) / (size.z)) - 1.0f;
				mousePos.y = -(2 * ((mousePos.y - (size.y + 10.0f)) / (size.w)) - 1.0f);

				LineSegment picking = camera.UnProjectLineSegment(mousePos.x, mousePos.y);
				
				if (app->sceneManager->GetGameState() == GameState::PLAYING)
					app->navMesh->CheckNavMeshIntersection(picking, SDL_BUTTON_LEFT);
			}
		}
	}

	// This is bad!
	/*if (targetUID != 0)
	{
		target = app->sceneManager->GetCurrentScene()->GetGoByUuid(targetUID);
		targetUID = 0;
	}*/

	if (target && app->input->GetKey(SDL_SCANCODE_F) == KeyState::KEY_DOWN)
	{
		fixingToTarget ^= true;
	}
	if (target && fixingToTarget)
	{
		float3 pos_ = target->GetComponent<TransformComponent>()->GetPosition() - camera.Pos();
		camera.Translate(pos_ * dt);
		if (pos_.IsZero(0.001)) fixingToTarget = false;
	}

	if (shake)
		Shake(dt);

	if (!CompareRotations(currentRotation, transform->GetRotation()))
	{
		currentRotation = transform->GetRotation();

		float3 newUp = float3::unitY;
		float3 newFront = float3::unitZ;

		newUp = transform->GetRotation() * newUp;

		newUp.Normalize();
		newFront = transform->GetRotation() * newFront;
		newFront.Normalize();

		float3::Orthonormalize(newUp, newFront);

		camera.SetUp(newUp);
		camera.SetFront(newFront);
	}

	// -------------MOVEMENT---------------
	if (app->input->GetMouseButton(SDL_BUTTON_RIGHT) != KeyState::KEY_REPEAT && !lock)
	{
		UpdateMovement();
		UpdateRotation();
		Zoom();
	}


	matrixProjectionFrustum = camera.ComputeProjectionMatrix();
	matrixViewFrustum = camera.ComputeViewMatrix();

	return true;
}

void CameraComponent::Zoom()
{
	if (app->input->GetMouseZ() > 0 && zoom < zoomMax)
	{
		zoom += zoomSpeed;
		transform->SetGlobalPosition(transform->GetGlobalPosition() + (controllerTrans->GetGlobalPosition() - transform->GetGlobalPosition()).Normalized() * zoomSpeed);
	}
	else if (app->input->GetMouseZ() < 0 && zoom > zoomMin)
	{
		zoom -= zoomSpeed;
		transform->SetGlobalPosition(transform->GetGlobalPosition() - (controllerTrans->GetGlobalPosition() - transform->GetGlobalPosition()).Normalized() * zoomSpeed);
	}
}

void CameraComponent::UpdateMovement()
{
	if (freeMovement)
	{
		float3 pos = controllerTrans->GetPosition();
		bool moved = false;
		float horizontalDrag = 0;
		float verticalDrag = 0;
		if (app->input->GetKey(SDL_SCANCODE_W) == KeyState::KEY_REPEAT)
		{
			verticalDrag += movementSpeed;
			moved = true;
		}

		if (app->input->GetKey(SDL_SCANCODE_S) == KeyState::KEY_REPEAT)
		{
			verticalDrag -= movementSpeed;
			moved = true;
		}

		if (app->input->GetKey(SDL_SCANCODE_A) == KeyState::KEY_REPEAT)
		{
			horizontalDrag += movementSpeed;
			moved = true;
		}

		if (app->input->GetKey(SDL_SCANCODE_D) == KeyState::KEY_REPEAT)
		{
			horizontalDrag -= movementSpeed;
			moved = true;
		}

		if (moved)
		{
			if (horizontalDrag != 0)
			{
				pos.x += horizontalDrag * sin(DEGTORAD * (horizontalAngle + 90));
				pos.z += horizontalDrag * cos(DEGTORAD * (horizontalAngle + 90));
			}
			if (verticalDrag != 0)
			{
				pos.x -= -verticalDrag * sin(DEGTORAD * horizontalAngle);
				pos.z -= -verticalDrag * cos(DEGTORAD * horizontalAngle);
			}

			if (pos.x < limits.x) pos.x = limits.x;
			else if (pos.x > limits.z) pos.x = limits.z; // limit.z = bound.x + bound.w
			if (pos.z < limits.y) pos.z = limits.y;
			else if (pos.z > limits.w) pos.z = limits.w; // limit.w = bound.y + bound.h

			controllerTrans->SetPosition(float3(pos.x, pos.y, pos.z));
			controllerTrans->ForceUpdateTransform();
		}
	}
}

void CameraComponent::UpdateRotation()
{
	if (app->input->GetKey(SDL_SCANCODE_Q) == KeyState::KEY_REPEAT)
	{
		horizontalAngle -= rotationSpeed;
		if (horizontalAngle < 0) horizontalAngle += 360;
		controllerTrans->SetRotation(Quat::RotateY(DEGTORAD * horizontalAngle));
		//controllerTrans->SetRotation(Quat::FromEulerXYZ(controllerTrans->GetRotation().ToEulerXYZ().x, DEGTORAD * horizontalAngle, controllerTrans->GetRotation().ToEulerXYZ().z));
		controllerTrans->UpdateEditorRotation();
		controllerTrans->ForceUpdateTransform();
	}
	else if (app->input->GetKey(SDL_SCANCODE_E) == KeyState::KEY_REPEAT)
	{
		horizontalAngle += rotationSpeed;
		if (horizontalAngle > 360) horizontalAngle -= 360;
		controllerTrans->SetRotation(Quat::RotateY(DEGTORAD * horizontalAngle));
		//controllerTrans->SetRotation(Quat::FromEulerXYZ(controllerTrans->GetRotation().ToEulerXYZ().x,DEGTORAD * horizontalAngle, controllerTrans->GetRotation().ToEulerXYZ().z));
		controllerTrans->UpdateEditorRotation();
		controllerTrans->ForceUpdateTransform();
	}
}

void CameraComponent::Draw(CameraComponent* gameCam)
{
	glPushMatrix();
	glMultMatrixf(transform->GetGlobalTransform().Transposed().ptr());
	glEnableClientState(GL_VERTEX_ARRAY);

	vbo->Bind();
	glVertexPointer(3, GL_FLOAT, 0, NULL);
	ebo->Bind();

	glLineWidth(2.0f);
	glColor3f(0.0f, 0.0f, 1.0f);
	glDrawElements(GL_LINES, ebo->GetCount(), GL_UNSIGNED_INT, NULL);
	glColor3f(1.0f, 1.0f, 1.0f);
	glLineWidth(1.0f);

	vbo->Unbind();
	ebo->Unbind();

	glDisableClientState(GL_VERTEX_ARRAY);
	glPopMatrix();
}

void CameraComponent::SetPlanes()
{
	camera.SetViewPlaneDistances(nearPlane, farPlane);
	//CompileBuffers();
}

void CameraComponent::CalculateVerticalFov(float horizontalFovRadians, float width, float height)
{
	verticalFov = 2 * Atan((Tan(horizontalFovRadians / 2)) * (height / width));
	camera.SetVerticalFovAndAspectRatio(verticalFov, (width / height));
	currentScreenHeight = height;
	currentScreenWidth = width;
}

void CameraComponent::UpdateFovAndScreen(float width, float height)
{
	verticalFov = 2 * Atan((Tan(horizontalFov / 2)) * (height / width));
	camera.SetVerticalFovAndAspectRatio(verticalFov, (width / height));
	currentScreenHeight = height;
	currentScreenWidth = width;
}

void CameraComponent::UpdateFov()
{
	verticalFov = 2 * Atan((Tan(horizontalFov / 2)) * (currentScreenHeight / currentScreenWidth));
	camera.SetVerticalFovAndAspectRatio(verticalFov, (currentScreenWidth / currentScreenHeight));
}

void CameraComponent::SetNewFov(float horizontalFovRadians)
{
	horizontalFov = horizontalFovRadians;
}

void CameraComponent::CompileBuffers()
{
	// Configure buffers
	float3 corners[8];
	camera.GetCornerPoints(corners);
	static unsigned int indices[24] =
	{
		0,1,
		1,3,
		3,2,
		2,0,

		1,5,
		4,6,
		7,3,

		6,7,
		6,2,

		7,5,
		4,5,

		4,0
	};

	if (vbo)
	{
		vbo->Unbind();
		RELEASE(vbo);
	}
	ebo = new IndexBuffer(indices, 24);
	vbo = new VertexBuffer(corners, sizeof(float3) * 8);
	ebo->Unbind();
	vbo->Unbind();
}

bool CameraComponent::CompareRotations(Quat& quat1, Quat& quat2)
{
	if (quat1.x == quat2.x && quat1.y == quat2.y && quat1.z == quat2.z && quat1.w == quat2.w)
		return true;
	return false;
}

bool CameraComponent::OnLoad(JsonParsing& node)
{
	nearPlane = node.GetJsonNumber("Near Plane");
	farPlane = node.GetJsonNumber("Far Plane");
	verticalFov = node.GetJsonNumber("Vertical Fov");
	horizontalFov = node.GetJsonNumber("Horizontal Fov");
	camera.SetPos(node.GetJson3Number(node, "Camera Pos"));
	zoom = node.GetJsonNumber("Zoom");
	zoomMin = node.GetJsonNumber("Zoom Min");
	zoomMax = node.GetJsonNumber("Zoom Max");
	zoomSpeed = node.GetJsonNumber("Zoom Speed");
	lock = node.GetJsonBool("Lock");

	// MOVEMENT
	freeMovement = node.GetJsonBool("Free Movement");
	if (!freeMovement) followTarget = true;
	defTarget = owner->GetParent();
	//defTarget->GetComponent<TransformComponent>()->SetPosition(node.GetJson3Number(node, "Default Target Pos"));
	movementSpeed = node.GetJsonNumber("Movement Speed");
	//targetUID = node.GetJsonNumber("Target Pos");
	// ANGLES
	rotationSpeed = node.GetJsonNumber("Rotation Speed");
	radius = node.GetJsonNumber("Radius");
	horizontalAngle = node.GetJsonNumber("Horizontal Angle");
	// SHAKE
	shakeStrength = node.GetJsonNumber("Shake Strength");
	shakeDuration = node.GetJsonNumber("Shake Duration");
	smooth = node.GetJsonNumber("Shake Smooth");

	// Limits
	limits = node.GetJson4Number(node, "Limits");

	SetPlanes();

	return true;
}

bool CameraComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Near Plane", nearPlane);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Far Plane", farPlane);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Vertical Fov", verticalFov);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Horizontal Fov", horizontalFov);
	file.SetNewJson3Number(file, "Camera Pos", camera.Pos());
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Zoom", zoom);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Zoom Min", zoomMin);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Zoom Max", zoomMax);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Zoom Speed", zoomSpeed);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Lock", lock);

	// MOVEMENT
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Free Movement", freeMovement);
	file.SetNewJson3Number(file, "Default Target Pos", defTarget->GetComponent<TransformComponent>()->GetPosition());
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Movement Speed", movementSpeed);
	if (target) file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Target Pos", target->GetUUID());

	// ANGLES
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Rotation Speed", rotationSpeed);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Radius", radius);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Horizontal Angle", horizontalAngle);

	// SHAKE
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Shake Strength", shakeStrength);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Shake Duration", shakeDuration);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Shake Smooth", smooth);

	// Limits
	file.SetNewJson4Number(file, "Limits", limits);

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

int CameraComponent::ContainsAaBox(const AABB& boundingBox)
{
	if (boundingBox.IsFinite())
	{
		if (camera.Contains(boundingBox))
		{
			return 1;
		}
		else if (camera.Intersects(boundingBox))
		{
			return 2;
		}
		return 0;
	}
	return -1;
}

float4x4 CameraComponent::ViewMatrixOpenGL()
{
	float4x4 mat = camera.ViewMatrix();
	return mat.Transposed();
}

//float4x4 CameraComponent::ViewMatrixOpenGL()
//{
//	math::float4x4 mat = camera.ViewMatrix();
//	return mat.Transposed();
//}

void CameraComponent::RequestShake(float strength, float duration)
{
	shakeStrength = strength;
	shakeDuration = duration;
	if (smooth != 0) currentStrength = 0;
	else currentStrength = strength;
	shake = true;
	originalPos = transform->GetGlobalPosition();
}

void CameraComponent::Shake(float dt)
{
	// Exponential
	if (smooth != 0)
	{
		if ((elapsedTime < shakeDuration / 2 && smooth == 1) || (smooth == 2 && elapsedTime < shakeDuration))
		{
			if (currentStrength < shakeStrength)
			{
				currentStrength = shakeStrength * elapsedTime * elapsedTime;
			}
			else currentStrength = shakeStrength;
		}
		else if ((currentStrength > 0 && elapsedTime < shakeDuration && smooth == 1) || (elapsedTime < shakeDuration && smooth == 3))
		{
			currentStrength = shakeStrength * (shakeDuration - elapsedTime) * (shakeDuration - elapsedTime);
		}
		else currentStrength = 0;
	}

	if (elapsedTime < shakeDuration)
	{
		float x = -currentStrength + static_cast <float> (rand()) / (static_cast <float> (RAND_MAX / (currentStrength - -currentStrength)));
		float y = -currentStrength + static_cast <float> (rand()) / (static_cast <float> (RAND_MAX / (currentStrength - -currentStrength)));
		float z = -currentStrength + static_cast <float> (rand()) / (static_cast <float> (RAND_MAX / (currentStrength - -currentStrength)));
		float3 lastPos = transform->GetGlobalPosition();
		lastPos.x += x;
		lastPos.y += y;
		lastPos.z += z;
		camera.SetPos(lastPos);
		elapsedTime += dt;
	}
	else
	{
		shake = false;
		elapsedTime = 0.0f;
		camera.SetPos(originalPos);
	}
}

// Scripting
void CameraComponent::ScriptMovement(float x, float y, float z)
{
	controllerTrans->SetPosition(float3(x, y, z));
	controllerTrans->ForceUpdateTransform();
}