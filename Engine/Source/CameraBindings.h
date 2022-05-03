#pragma once
#include "ScriptComponent.h"
#include "CameraComponent.h"
#include "ModuleCamera3D.h"
#include "ModuleRenderer3D.h"

#include "Math/float3x3.h"
#include "Geometry/LineSegment.h"
#include "Geometry/Triangle.h"

#include "ScriptBindings.h"
#include <metadata\object-forward.h>
#include <metadata\object.h>
#include <metadata/class.h>

void LookAt(MonoObject* go, MonoObject* transformToLook)
{
	float3 target = app->moduleMono->UnboxVector(transformToLook);

	Frustum* cameraFrustum = GetComponentMono<CameraComponent*>(go)->GetFrustum();
	float3 directionFrustum = target - cameraFrustum->Pos();
	directionFrustum.Normalize();

	float3x3 lookAt = float3x3::LookAt(cameraFrustum->Front(), directionFrustum, cameraFrustum->Up(), float3(0.0f, 1.0f, 0.0f));
	cameraFrustum->SetFront(lookAt.MulDir(cameraFrustum->Front()).Normalized());
	cameraFrustum->SetUp(lookAt.MulDir(cameraFrustum->Up()).Normalized());
}

void ChangeFov(MonoObject* go, float newFov)
{
	CameraComponent* camComp = GetComponentMono<CameraComponent*>(go);
	camComp->SetNewFov(DegToRad(newFov));
	camComp->UpdateFov();
	camComp->CompileBuffers();
}

void ScriptMovement(MonoObject* go, float x, float y, float z)
{
	CameraComponent* cam = GetComponentMono<CameraComponent*>(go);
	cam->ScriptMovement(x, y, z);
}

void ScriptRotation(MonoObject* go, float x, float y, float z)
{
	CameraComponent* cam = GetComponentMono<CameraComponent*>(go);
	cam->ScriptRotation(x, y, z);
}

void ScriptRotationAngle(MonoObject* go, float ang)
{
	CameraComponent* cam = GetComponentMono<CameraComponent*>(go);
	cam->ScriptRotation(ang);
}

float GetAngle(MonoObject* go)
{
	CameraComponent* cam = GetComponentMono<CameraComponent*>(go);
	return cam->GetAngle();
}

void LockControlls(MonoObject* go, bool lock)
{
	CameraComponent* cam = GetComponentMono<CameraComponent*>(go);
	cam->LockControlls(lock);
}

float GetZoom(MonoObject* go)
{
	CameraComponent* cam = GetComponentMono<CameraComponent*>(go);
	return cam->GetZoom();
}

void ScriptZoom(MonoObject* go, float t)
{
	CameraComponent* cam = GetComponentMono<CameraComponent*>(go);
	cam->ScriptZoom(t);
}

MonoObject* HitToTag(MonoObject* initPos, MonoObject* endPos, MonoObject* tag)
{
	float3 pointA = app->moduleMono->UnboxVector(initPos);
	float3 pointB = app->moduleMono->UnboxVector(endPos);
	char* tagName = mono_string_to_utf8(mono_object_to_string(tag, 0));
	LineSegment picking(pointA, pointB);

	std::stack<QuadtreeNode*> nodes;
	std::set<GameObject*> gameObjects;
	app->sceneManager->GetCurrentScene()->GetQuadtree().CollectNodes(nodes, picking);
	app->sceneManager->GetCurrentScene()->GetQuadtree().CollectGoOnlyStatic(gameObjects, nodes);

	std::map<float, GameObject*> triangleMap;
	float3 hit;
	app->camera->ThrowRayCast(gameObjects, picking, triangleMap, hit);

	// Throw Ray from enemy head to player head
	if (!triangleMap.empty() && (*triangleMap.begin()).second->tag == tagName)
		return app->moduleMono->GoToCSGO((*triangleMap.begin()).second);

	triangleMap.clear();
	return nullptr;
}

int PerceptionCone(MonoObject* initPos, MonoObject* _forward, int _angle, int rays, int radius, MonoArray* arr, int size)
{
	float3 pointA = app->moduleMono->UnboxVector(initPos);
	float3 forward = app->moduleMono->UnboxVector(_forward);
	std::vector<GameObject*> gos = app->moduleMono->UnboxArray(arr, size);
	float3 forwardAux = forward;
	float angle = _angle * DEGTORAD;

	forward = forward * float3x3::RotateY((360-(_angle/2)) * DEGTORAD);
	std::vector<float3> vertex;
	vertex.reserve(rays);

	std::stack<QuadtreeNode*> nodes;
	std::set<GameObject*> gameObjects;
	std::map<float, GameObject*> triangleMap;
	float3 arrayPos[] = { forward * radius, forwardAux * float3x3::RotateY(angle / 2) * radius };

	for (size_t i = 0; i < 2; i++)
	{
		LineSegment ray(pointA, pointA + arrayPos[i]);
		app->sceneManager->GetCurrentScene()->GetQuadtree().CollectNodes(nodes, ray);
		app->sceneManager->GetCurrentScene()->GetQuadtree().CollectGoOnlyStatic(gameObjects, nodes);
		std::stack<QuadtreeNode*>().swap(nodes);
	}

	float3 hit = float3::zero;
	for (int i = 0; i < rays; i++)
	{
		LineSegment ray(pointA, pointA + (forward * float3x3::RotateY(angle/rays * i) * radius));
		app->camera->ThrowRayCastOnlyOBB(gameObjects, ray, triangleMap, hit);
		
		vertex.push_back(pointA); // origin
		if (i != 0) vertex.push_back(vertex.at(vertex.size() - 2)); // previous 
		vertex.push_back(hit); // this

		if (i == 1)
		{
			vertex.erase(vertex.begin());
			vertex.erase(vertex.begin());
		}
		triangleMap.clear();
		hit = float3::zero;
	}

	int ret = -1;
	for (size_t i = 0; i < vertex.size() && ret == -1 ; i+=3)
	{
		Triangle t(vertex[i], vertex[i+1], vertex[i+2]);
		for (size_t j = 0; j < gos.size(); j++)
		{
			if (t.Intersects(gos.at(j)->GetOOB()))
			{
				ret = j;
				break;
			}
		}		
	}

	// Inverse triangle
	std::reverse(vertex.begin(), vertex.end());

	// Add to enemyCones list
	int sizeCon = app->renderer3D->enemyCones.size();
	app->renderer3D->enemyCones.resize(sizeCon + vertex.size());
	memcpy(&app->renderer3D->enemyCones[sizeCon], &vertex[0], vertex.size() * sizeof(float3));

	return ret;
}