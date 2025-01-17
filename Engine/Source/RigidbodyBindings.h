#pragma once

#include "Application.h"
#include "MonoManager.h"

#include "GameObject.h"
#include "TransformComponent.h"
#include "C_RigidBody.h"
#include "ScriptBindings.h"

#include <metadata\object-forward.h>
#include <metadata\object.h>
#include <metadata/class.h>

#include "BulletDynamics/Dynamics/btRigidBody.h"
#include "Math/float3.h"


void ApplyCentralForce(MonoObject* go, MonoObject* force)
{
	float3 f = app->moduleMono->UnboxVector(force);
	
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	btRigidBody* body = rb->GetBody();
	body->activate(true);
	body->applyCentralForce(f);
}

void ApplyCentralImpulse(MonoObject* go, MonoObject* impulse)
{
	float3 f = app->moduleMono->UnboxVector(impulse);
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	btRigidBody* body = rb->GetBody();
	body->activate(true);
	body->applyCentralImpulse(f);
}

void ApplyTorque(MonoObject* go, MonoObject* torque)
{
	float3 f = app->moduleMono->UnboxVector(torque);
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	btRigidBody* body = rb->GetBody();
	body->activate(true);
	body->applyTorque(f);
}

void SetLinearVelocity(MonoObject* go, MonoObject* velocity)
{
	float3 vel = app->moduleMono->UnboxVector(velocity);
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	rb->GetBody()->setLinearVelocity({ vel.x, vel.y, vel.z });
}

void SetIgnoreCollision(MonoObject* go, MonoObject* other, bool value)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	RigidBodyComponent* otherRb = GetComponentMono<RigidBodyComponent*>(other);

	btRigidBody* body = rb->GetBody();
	body->activate(true);
	body->setIgnoreCollisionCheck(otherRb->GetBody(), value);
}

MonoObject* GetLinearVelocity(MonoObject* go)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	return app->moduleMono->Float3ToCS((float3)rb->GetBody()->getLinearVelocity());
}

void ClearForces(MonoObject* go)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	rb->GetBody()->clearForces();
}

MonoObject* GetTotalForce(MonoObject* go)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	float3 f = rb->GetBody()->getTotalForce();
	return app->moduleMono->Float3ToCS(f);
}

void SetAsStatic(MonoObject* go)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	rb->SetAsStatic();
}

void SetAsTrigger(MonoObject* go)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	rb->SetAsTrigger();
}

void SetCollisionType(MonoObject* go, CollisionType type)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	rb->SetCollisionType(type);
}

void SetCollisionSphere(MonoObject* go, float radius, float x, float y, float z)
{
	float3 pos(x, y, z);
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	rb->SetCollisionSphere(radius, pos);
}

void SetHeight(MonoObject* go, float height)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	rb->SetHeight(height);
}

void SetBodyPosition(MonoObject* go, MonoObject* pos)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	float3 bPos = app->moduleMono->UnboxVector(pos);
	rb->GetBody()->getWorldTransform().setOrigin(bPos);
}

void SetBodyRotation(MonoObject* go, MonoObject* pos)
{
	RigidBodyComponent* rb = GetComponentMono<RigidBodyComponent*>(go);
	Quat bRot = app->moduleMono->UnboxQuat(pos);
	rb->GetBody()->getWorldTransform().setRotation(bRot);
}