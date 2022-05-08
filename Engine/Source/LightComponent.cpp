#include "LightComponent.h"
#include "Application.h"

#include "ModuleRenderer3D.h"

#include "GameObject.h"
#include "TransformComponent.h"
#include "Lights.h"

#include "Profiling.h"

ComponentLight::ComponentLight()
{
	this->type = ComponentType::LIGHT;
}

ComponentLight::~ComponentLight()
{
	if (this->owner->name == "Directional Light")
		return;

	switch (light->type)
	{
	case LightType::POINT:
		app->renderer3D->RemovePointLight((PointLight*)light);
		break;
	case LightType::SPOT:
		app->renderer3D->RemoveSpotLight((SpotLight*)light);
		break;
	default:
		break;
	}
}

bool ComponentLight::Update(float dt)
{
	RG_PROFILING_FUNCTION("Light Component Update");

	if (light->type == LightType::DIRECTIONAL)
	{			
		if (TransformComponent* tr = owner->GetComponent<TransformComponent>())
		{
			DirectionalLight* l = (DirectionalLight*)light;
			l->dir = tr->GetRotation().CastToFloat4().Float3Part();
			l->dir.Normalize();
			
			Frustum frustum;
			frustum.pos = tr->GetGlobalPosition();
			//AABB shadowsAABB = app->renderer3D->shadowsAABB;
			//frustum.pos = float3((shadowsAABB.MaxX() + shadowsAABB.MinX()) * 0.5f, (shadowsAABB.MaxY() + shadowsAABB.MinY()) * 0.5f, shadowsAABB.MaxZ());

			frustum.front = app->renderer3D->dirLight->dir;
			float3 right = frustum.front.Cross({ 0,1,0 }).Normalized();
			frustum.up = right.Cross(frustum.front).Normalized();
			frustum.type = FrustumType::OrthographicFrustum;

			frustum.orthographicHeight = 512;
			frustum.orthographicWidth = 512;
			frustum.nearPlaneDistance = 0.001;
			frustum.farPlaneDistance = 500000;

			frustum.SetKind(FrustumProjectiveSpace::FrustumSpaceGL, FrustumHandedness::FrustumRightHanded);

			float4x4 lightView = frustum.ViewMatrix();
			float4x4 lightProjection = frustum.projectionMatrix;

			l->lightSpace = lightProjection * lightView;
		}
	}
	else if (light->type == LightType::POINT)
	{
		if (TransformComponent* tr = owner->GetComponent<TransformComponent>())
		{
			PointLight* l = (PointLight*)light;
			float3 pos = tr->GetGlobalPosition();
			if (pos.x != l->position.x || pos.y != l->position.y || pos.z != l->position.z)
			{
				l->position = pos;
				light = l;
			}
		}
	}
	else if (light->type == LightType::SPOT)
	{
		if (TransformComponent* tr = owner->GetComponent<TransformComponent>())
		{
			SpotLight* l = (SpotLight*)light;
			float3 pos = tr->GetGlobalPosition();
			if (pos.x != l->position.x || pos.y != l->position.y || pos.z != l->position.z)
			{
				l->position = pos;
				light = l;
			}
			
			float3 dir = tr->GetRotation().CastToFloat4().Float3Part();
			if (dir.x == 0 && dir.y == 0 && dir.z == 0)
			{
				l->direction = { 0,-1,0 };
			}
			else
			{
				l->direction = tr->GetRotation().CastToFloat4().Float3Part();
				l->direction.Normalize();
			}
			
		}
	}

	return true;
}

void ComponentLight::OnEditor()
{
	switch (light->type)
	{
		case LightType::DIRECTIONAL:
		{
			if (ImGui::CollapsingHeader(ICON_FA_LIGHTBULB" Light"))
			{
				ImGui::Separator();
				ImGui::Dummy({ 10,10 });

				DirectionalLight* l = (DirectionalLight*)light;

				ImGui::DragFloat("Intensity", &l->intensity, 0.1f, 0.0f);
				ImGui::ColorEdit3("Ambient Color", l->ambient.ptr());
				ImGui::ColorEdit3("Diffuse Color", l->diffuse.ptr());
				ImGui::ColorEdit3("Specular Color", l->specular.ptr());

				ImGui::Checkbox("Shadows", &l->generateShadows);

				ImGui::Text("Direction: %f %f %f", l->dir.x, l->dir.y, l->dir.z);

				ComponentOptions(this);
				ImGui::Separator();
			}
			break;
		}		
		case LightType::POINT:
		{
			if (ImGui::CollapsingHeader(ICON_FA_LIGHTBULB" Light"))
			{
				PointLight* l = (PointLight*)light;

				ImGui::ColorEdit3("Ambient Color", l->ambient.ptr());
				ImGui::ColorEdit3("Diffuse Color", l->diffuse.ptr());
				ImGui::ColorEdit3("Specular Color", l->specular.ptr());

				ImGui::DragFloat("Intensity", &l->intensity, 0.1f, 0.0f, 100.0f);

				ImGui::DragFloat("Constant", &l->constant, 0.001f, 0.1f, 100.0f);
				ImGui::DragFloat("Linear", &l->lin, 0.001f);
				ImGui::DragFloat("Quadratic", &l->quadratic, 0.001f, 0.0f, 1.0f);

				ComponentOptions(this);
				ImGui::Separator();
			}
			break;
		}
		case LightType::SPOT:
		{
			if (ImGui::CollapsingHeader(ICON_FA_LIGHTBULB" Light"))
			{
				SpotLight* l = (SpotLight*)light;

				ImGui::ColorEdit3("Ambient Color", l->ambient.ptr());
				ImGui::ColorEdit3("Diffuse Color", l->diffuse.ptr());
				ImGui::ColorEdit3("Specular Color", l->specular.ptr());

				ImGui::DragFloat("Intensity", &l->intensity, 0.01f);
				ImGui::DragFloat("CutOff", &l->cutOff, 0.001f, 1.0f,0.0f);
				ImGui::DragFloat("Outer CutOff", &l->outerCutOff, 0.001f, l->cutOff, 0.0f);

				ComponentOptions(this);
				ImGui::Separator();
			}
			break;
		}
	}
}

void ComponentLight::SetLight(Light* light)
{
	this->light = light;
}

void ComponentLight::SetAsPointLight()
{
	owner->name = "Point Light";
	light = new PointLight();
	app->renderer3D->AddPointLight((PointLight*)light);
}

void ComponentLight::SetAsSpotLight()
{
	owner->name = "Spot Light";
	light = new SpotLight();
	app->renderer3D->AddSpotLight((SpotLight*)light);
}

Light* ComponentLight::GetLight()
{
	switch (light->type)
	{
		case LightType::DIRECTIONAL:
		{
			return (DirectionalLight*)light;
		}

		case LightType::POINT:
		{
			return (PointLight*)light;
		}

		case LightType::SPOT:
		{
			return (SpotLight*)light;
		}
	}
	return nullptr;
}

bool ComponentLight::OnLoad(JsonParsing& node)
{
	active = node.GetJsonBool("Active");

	int lightType = node.GetJsonNumber("Light Type");

	switch ((LightType)lightType)
	{
		case LightType::DIRECTIONAL:
		{
			DirectionalLight* l = new DirectionalLight();

			l->dir = node.GetJson3Number(node, "Direction");
			l->ambient = node.GetJson3Number(node, "Ambient");
			l->diffuse = node.GetJson3Number(node, "Diffuse");
			l->specular = node.GetJson3Number(node, "Specular");
			l->intensity = node.GetJsonNumber("Intensity");
			l->generateShadows = node.GetJsonBool("Shadows");
			l->type = LightType::DIRECTIONAL;

			light = l;

			delete app->renderer3D->dirLight;
			app->renderer3D->dirLight = l;

			break;
		}
		case LightType::POINT:
		{
			PointLight* l = new PointLight();

			l->position = node.GetJson3Number(node, "Position");
			l->ambient = node.GetJson3Number(node, "Ambient");
			l->diffuse = node.GetJson3Number(node, "Diffuse");
			l->specular = node.GetJson3Number(node, "Specular");
			l->intensity = node.GetJsonNumber("Intensity");
			l->constant = node.GetJsonNumber("Constant");
			l->lin = node.GetJsonNumber("Linear");
			l->quadratic = node.GetJsonNumber("Quadratic");
			l->type = LightType::POINT;

			light = l;
			app->renderer3D->AddPointLight(l);

			break;
		}
		case LightType::SPOT:
		{
			SpotLight* l = new SpotLight();

			l->position = node.GetJson3Number(node, "Position");
			l->direction = node.GetJson3Number(node, "Direction");
			l->ambient = node.GetJson3Number(node, "Ambient");
			l->diffuse = node.GetJson3Number(node, "Diffuse");
			l->specular = node.GetJson3Number(node, "Specular");
			l->cutOff = node.GetJsonNumber("CutOff");
			l->outerCutOff = node.GetJsonNumber("Outer CutOff");
			l->type = LightType::SPOT;

			light = l;
			app->renderer3D->AddSpotLight(l);

			break;
		}
	}

	return true;
}

bool ComponentLight::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Active", active);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Light Type", (int)light->type);

	switch (light->type)
	{
		case LightType::DIRECTIONAL:
		{
			DirectionalLight* l = (DirectionalLight*)light;

			//file.SetNewJson3Number(file, "Position", l->position);
			file.SetNewJson3Number(file, "Direction", l->dir);
			file.SetNewJson3Number(file, "Ambient", l->ambient);
			file.SetNewJson3Number(file, "Diffuse", l->diffuse);
			file.SetNewJson3Number(file, "Specular", l->specular);
			file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Intensity", l->intensity);
			file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Shadows", l->generateShadows);

			break;
		}
		case LightType::POINT:
		{
			PointLight* l = (PointLight*)light;
			file.SetNewJson3Number(file, "Position", l->position);
			file.SetNewJson3Number(file, "Ambient", l->ambient);
			file.SetNewJson3Number(file, "Diffuse", l->diffuse);
			file.SetNewJson3Number(file, "Specular", l->specular);
			file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Intensity", l->intensity);
			file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Constant", l->constant);
			file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Linear", l->lin);
			file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Quadratic", l->quadratic);

			break;
		}
		case LightType::SPOT:
		{
			SpotLight* l = (SpotLight*)light;

			file.SetNewJson3Number(file, "Position", l->position);
			file.SetNewJson3Number(file, "Direction", l->direction);
			file.SetNewJson3Number(file, "Ambient", l->ambient);
			file.SetNewJson3Number(file, "Diffuse", l->diffuse);
			file.SetNewJson3Number(file, "Specular", l->specular);
			file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "CutOff", l->cutOff);
			file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Outer CutOff", l->outerCutOff);

			break;
		}
	}

	node.SetValueToArray(array, file.GetRootValue());
	
	return true;
}
