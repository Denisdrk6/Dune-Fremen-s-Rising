#pragma once

#include "ModuleNavMesh.h"
#include "MonoManager.h"

#include "NavAgentComponent.h"

#include "TransformBindings.h"

MonoArray* CalculateAgentPath(MonoObject* go, MonoObject* dest)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	float3 destination = app->moduleMono->UnboxVector(dest);
	std::vector<float3> wayPoints = agent->pathfinding->CalculatePath(agent, destination);

	MonoClass* vecClass = mono_class_from_name(app->moduleMono->image, SCRIPTS_NAMESPACE, "Vector3");
	MonoArray* ret = mono_array_new(app->moduleMono->domain, vecClass, wayPoints.size());

	for (int i = 0; i < wayPoints.size(); ++i)
		mono_array_set(ret, MonoObject*, i, app->moduleMono->Float3ToCS(wayPoints[i]));
	
	return ret;
}

bool ValidDestination(MonoObject* dest)
{
	float3 destination = app->moduleMono->UnboxVector(dest);
	dtStatus status;
	const float m_polyPickExt[3] = { 2,4,2 };
	Pathfinder* path = app->navMesh->GetPathfinding();
	status = path->m_navQuery->findNearestPoly(destination.ptr(), m_polyPickExt, &path->m_filter, 0, nullptr);
	if (dtStatusFailed(status) || (status & DT_STATUS_DETAIL_MASK))
		return false;

	return true;
}

bool MoveAgentPath(MonoObject* go)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	return agent->pathfinding->MovePath(agent);
}

void ClearPath(MonoObject* go)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	agent->owner->GetComponent<RigidBodyComponent>()->GetBody()->setLinearVelocity({ 0,0,0 });
	agent->agentProperties->path.clear();
}

int PathSize(MonoObject* go)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	return agent->agentProperties->path.size();
}

MonoObject* GetHitPosition(MonoObject* go)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	return app->moduleMono->Float3ToCS(agent->pathfinding->hitPosition);
}

void SetHitPosition(MonoObject* go, MonoObject* hitPos)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	float3 hit = app->moduleMono->UnboxVector(hitPos);
	agent->pathfinding->hitPosition = hit;
}

MonoObject* GetRayCastA(MonoObject* go)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	return app->moduleMono->Float3ToCS(agent->pathfinding->rayCast[0]);
}

MonoObject* GetRayCastB(MonoObject* go)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	return app->moduleMono->Float3ToCS(agent->pathfinding->rayCast[1]);
}

bool MoveAgentTo(MonoObject* go, MonoObject* dest)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	float3 destination = app->moduleMono->UnboxVector(dest);
	return agent->pathfinding->MoveTo(agent, destination);
}

void SetAgentPath(MonoObject* go, MonoArray* path)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);

	std::vector<float3> wayPoints;
	int size = mono_array_length(path);
	for (int i = 0; i < size; ++i)
	{
		float3 v = app->moduleMono->UnboxVector(mono_array_get(path, MonoObject*, i));
		wayPoints.push_back(v);
	}

	agent->agentProperties->path.clear();
	agent->agentProperties->path = wayPoints;
}
MonoObject* GetAgentPath(MonoObject* go)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	return  app->moduleMono->Float3ToCS(agent->agentProperties->path[0]);
}

void SetAgentSpeed(MonoObject* go, float speed)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	agent->agentProperties->speed = speed;
}

float GetAgentSpeed(MonoObject* go)
{
	NavAgentComponent* agent = GetComponentMono<NavAgentComponent*>(go);
	return agent->agentProperties->speed;
}