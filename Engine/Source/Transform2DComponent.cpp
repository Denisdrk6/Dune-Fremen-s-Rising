#include "Transform2DComponent.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleEditor.h"
#include "ModuleUI.h"
#include "ModuleScene.h"

#include "GameObject.h"
#include "GameView.h"

#include <math.h>


ComponentTransform2D::ComponentTransform2D(/*float3 pos, float3 sca, float3 rot,*/ GameObject* own)
{
	internalPosition = { 0,0,0 };
	position = { 0,0,84.5f };
	scale.x = 30;
	scale.y = 15;
	scale.z = 1;

	//rotationEuler = rot;
	rotationEuler = float3(0, 0, 0);
	buttonWidth = 300;
	buttonHeight = 100;

	rotationQuat = FromEulerToQuat(rotationEuler);
	type = ComponentType::TRANFORM2D;
	float4x4 aux;
	transMatrix = aux.FromTRS(internalPosition, rotationQuat, scale);
	transmat = transMatrix;
	transMatrix = transMatrix.Transposed();
	type = ComponentType::TRANFORM2D;
	
}

ComponentTransform2D::~ComponentTransform2D()
{
	app->userInterface->DeleteUIGameObjects(owner);
}

bool ComponentTransform2D::Update(float dt)
{
	
	float4 viewport = app->editor->GetGameView()->GetBounds() * app->scene->mainCamera->GetZoomRatio();
	float temporalW = (viewport.z * 25) / 847;
	float temporalH = (viewport.w * 15) / 649;
	scale.x = (buttonWidth / 0.6) / temporalW;
	scale.y = (buttonHeight / 0.75) / temporalH;



	internalPosition.x = (position.x * 30.0f) / (viewport.z / 2);

	/*float res = (viewport.w * 1.5) / 649;
	res = 1.5 - (res - 1.5)+0.05;*/
	internalPosition.y = (position.y * 30.0f) / (viewport.w / 2);
	internalPosition.z = position.z;

	/*internalPosition.x = position.x/1;
	internalPosition.y = position.y/8;*/

	rotationQuat = FromEulerToQuat(rotationEuler);

	float4x4 aux;
	transMatrix = aux.FromTRS(internalPosition, rotationQuat, scale);
	transmat = transMatrix;
	transMatrix = transMatrix.Transposed();


	return true;
}

void ComponentTransform2D::OnEditor()
{
	if (showEdit) {
		if (ImGui::CollapsingHeader("2D Transform"))
		{
			ImGui::TextColored(ImVec4(0, 0, 255, 255), "Size");

			if (ImGui::DragFloat("Width", &buttonWidth, 0.5f, 0, 1000000))
			{

			}
			if (ImGui::DragFloat("Height", &buttonHeight, 0.5f, 0, 1000000))
			{

			}


			ImGui::TextColored(ImVec4(0, 0, 255, 255), "Position");
			static bool wasNull = true;


			if (ImGui::DragFloat("Position X", &position.x, 0.5f))
			{

			}
			if (ImGui::DragFloat("Position Y", &position.y, 0.5f))
			{

			}
			if (ImGui::DragFloat("Position Z", &position.z, 0.5f))
			{

			}
		}
	}
}


Quat ComponentTransform2D::FromEulerToQuat(float3 eulerAngles)
{
	eulerAngles.x = math::DegToRad(eulerAngles.x);
	eulerAngles.y = math::DegToRad(eulerAngles.y);
	eulerAngles.z = math::DegToRad(eulerAngles.z);

	Quat q = q.FromEulerXYZ(eulerAngles.x, eulerAngles.y, eulerAngles.z);

	return q;
}

float3 ComponentTransform2D::FromQuatToEuler(Quat quatAngles)
{
	float3 angles;

	angles = quatAngles.ToEulerXYZ();

	angles.x = math::RadToDeg(angles.x);
	angles.y = math::RadToDeg(angles.y);
	angles.z = math::RadToDeg(angles.z);
	float3 toReturn(angles.x,angles.y,angles.z);

	return toReturn;
}
bool ComponentTransform2D::OnLoad(JsonParsing& node)
{
	active = node.GetJsonBool("Active");
	position = node.GetJson3Number(node, "Position");
	scale = node.GetJson3Number(node, "Scale");
	rotationEuler = node.GetJson3Number(node, "rotationEuler");
	buttonWidth = node.GetJsonNumber("buttonWidth");
	buttonHeight = node.GetJsonNumber("buttonHeight");
	
	float4x4 aux;
	transMatrix = aux.FromTRS(position, rotationQuat, scale);
	transmat = transMatrix;
	transMatrix = transMatrix.Transposed();
	return true;
}

bool ComponentTransform2D::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();


	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Active", active);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)18);
	file.SetNewJson3Number(file, "Position", position);
	file.SetNewJson3Number(file, "Scale", scale);
	file.SetNewJson3Number(file, "rotationEuler", rotationEuler);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "buttonWidth", buttonWidth);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "buttonHeight", buttonHeight);

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}
