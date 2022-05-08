#include "Physics3D.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleSceneManager.h"
#include "Scene.h"
#include "C_RigidBody.h"
#include "ScriptComponent.h"
#include "TransformComponent.h"
#include "Mesh.h"

#include "btBulletDynamicsCommon.h"

#include "Profiling.h"

Physics3D::Physics3D(bool start_enabled) : Module(start_enabled)
{
	// Collision configuration contains default setup for memory, collision setup. Advanced
	// users can create their own configuration.
	collisionConfig = new btDefaultCollisionConfiguration();

	// Use the default collision dispatcher
	dispatcher = new btCollisionDispatcher(collisionConfig);

	// You can also try out	btAxis3Sweep or btBroadphaseInterface
	broadPhase = new btDbvtBroadphase();

	// The default constraint solver
	solver = new btSequentialImpulseConstraintSolver;

	debugDraw = new DebugDrawer();
}

Physics3D::~Physics3D()
{
	// Delete dynamics world
	RELEASE(debugDraw);
	RELEASE(solver);
	RELEASE(broadPhase);
	RELEASE(dispatcher);
	RELEASE(collisionConfig);
}

bool Physics3D::Start()
{
	LOG(LogType::L_NORMAL, "Creating 3D Physics simulation");
	LOG(LogType::L_NORMAL, "Creating Physics environment");
	world = new btDiscreteDynamicsWorld(dispatcher, broadPhase, solver, collisionConfig);
	world->setDebugDrawer(debugDraw);
	world->setGravity(GRAVITY);

	return true;
}

void Physics3D::DebugDraw()
{
	world->debugDrawWorld();
}

bool Physics3D::PreUpdate(float dt)
{
	if (app->sceneManager->GetGameState() == GameState::NOT_PLAYING)
	{
		world->stepSimulation(dt, 15);
	}
	else if (app->sceneManager->GetGameState() == GameState::PLAYING)
	{
		world->stepSimulation(app->sceneManager->GetGameDeltaTime(), 15);
		int numManifolds = world->getDispatcher()->getNumManifolds();
		if (numManifolds > 0)
		{
			// Save Reference
			RigidBodyComponent* obAobject = nullptr;
			RigidBodyComponent* obBobject = nullptr;
			ScriptComponent* script = nullptr;
			for (int i = 0; i < numManifolds; i++)
			{
				btPersistentManifold* contactManifold = world->getDispatcher()->getManifoldByIndexInternal(i);
				btCollisionObject* obA = (btCollisionObject*)(contactManifold->getBody0());
				btCollisionObject* obB = (btCollisionObject*)(contactManifold->getBody1());

				// numContacts is important because otherwise we can get false collisions  
				int numContacts = contactManifold->getNumContacts();
				if (numContacts > 0) 
				{
					// Find what objects have collided  
					for (int j = 0; j < bodies.size(); j++)
					{
						if (obA == bodies.at(j)->GetBody())
							obAobject = bodies.at(j);
						if (obB == bodies.at(j)->GetBody())
							obBobject = bodies.at(j);
					}
					// Call Methods for obA
					if (!obAobject->trigger && obAobject->owner->GetComponent<ScriptComponent>())
					{
						script = obAobject->owner->GetComponent<ScriptComponent>();
						// OnEnter
						if (!obAobject->GetOnCollision())
						{
							obAobject->SetOnCollision(true);
							if (obBobject->trigger)
								script->CallOnTriggerEnter(obBobject);
							else
								script->CallOnCollisionEnter(obBobject);
						}	
						// OnState
						else
						{
							if (obBobject->trigger)
								script->CallOnTrigger(obBobject);
							else
								script->CallOnCollision(obBobject);
						}
					}
					// Call Methods for obB
					if (!obBobject->trigger && obBobject->owner->GetComponent<ScriptComponent>())
					{
						script = obBobject->owner->GetComponent<ScriptComponent>();
						// OnEnter
						if (!obBobject->GetOnCollision())
						{
							obBobject->SetOnCollision(true);
							if (obAobject->trigger)
								script->CallOnTriggerEnter(obAobject);
							else
								script->CallOnCollisionEnter(obAobject);
						}
						// OnState
						else
						{
							if (obAobject->trigger)
								script->CallOnTrigger(obAobject);
							else
								script->CallOnCollision(obAobject);
						}
					}
				}				
			}
		}
	}
	
	return true;
}

bool Physics3D::Update(float dt)
{
	RG_PROFILING_FUNCTION("Physics3D Update");

	return true;
}

bool Physics3D::PostUpdate()
{
	return true;
}

bool Physics3D::CleanUp()
{
	// Remove the rigidbodies from the dynamics world and delete them
	for (int i = world->getNumCollisionObjects() - 1; i >= 0; i--)
	{
		btCollisionObject* obj = world->getCollisionObjectArray()[i];
		btRigidBody* body = btRigidBody::upcast(obj);
		if (body && body->getMotionState())
		{
			delete body->getMotionState();
		}
		world->removeCollisionObject(obj);
		RELEASE(obj);
	}

	bodies.clear();
	bodiesNames.clear();

	RELEASE(world);

	return true;
}

bool Physics3D::LoadConstraints()
{
	for (int i = 0; i < GetBodies().size(); i++)
	{
		for (int j = 0; j < GetBodies().at(i)->bodiesUIDs.size(); j++)
		{
			for (int k = 0; k < GetBodies().size(); k++)
			{
				int UID1 = GetBodies().at(k)->owner->GetUUID();
				int UID2 = GetBodies().at(i)->bodiesUIDs.at(j);
				if (UID1 == UID2)
				{
					GetBodies().at(i)->constraintBodies.push_back(GetBodies().at(k));
					GetBodies().at(i)->AddConstraintP2P(GetBodies().at(k));
					break;
				}
			}
		}
	}

	return true;
}

btRigidBody* Physics3D::CollisionShape(const PCube& cube, RigidBodyComponent* component)
{
	btCollisionShape* colShape = new btBoxShape(btVector3(cube.size.x * 0.5f, cube.size.y * 0.5f, cube.size.z * 0.5f));
	btTransform startTransform;
	startTransform.setFromOpenGLMatrix(&cube.transform);
	return AddBody(colShape, startTransform, component);
}

btRigidBody* Physics3D::CollisionShape(const PSphere& sphere, RigidBodyComponent* component)
{
	btCollisionShape* colShape = new btSphereShape(sphere.radius);
	btTransform startTransform;
	startTransform.setFromOpenGLMatrix(&sphere.transform);
	return AddBody(colShape, startTransform, component);
}

btRigidBody* Physics3D::CollisionShape(const PCapsule& capsule, RigidBodyComponent* component)
{
	btCollisionShape* colShape = new btCapsuleShape(capsule.radius, capsule.height);
	btTransform startTransform;
	startTransform.setFromOpenGLMatrix(&capsule.transform);
	return AddBody(colShape, startTransform, component);
}

btRigidBody* Physics3D::CollisionShape(const PCylinder& cylinder, RigidBodyComponent* component, Axis axis)
{
	btCollisionShape* colShape;
	if (axis == Axis::X)
		colShape = new btCylinderShapeX(btVector3(cylinder.height * 0.5f, cylinder.radius, 0.0f));
	if(axis == Axis::Y)
		colShape = new btCylinderShape(btVector3(cylinder.radius, cylinder.height * 0.5f, 0.0f));
	if (axis == Axis::Z)
		colShape = new btCylinderShapeZ(btVector3(cylinder.radius, 0.0f, cylinder.height * 0.5f));

	btTransform startTransform;
	startTransform.setFromOpenGLMatrix(&cylinder.transform);
	return AddBody(colShape, startTransform, component);
}

btRigidBody* Physics3D::CollisionShape(const PPyramid& cone, RigidBodyComponent* component)
{
	btCollisionShape* colShape = new btConeShape(cone.radius, cone.height);
	btTransform startTransform;
	startTransform.setFromOpenGLMatrix(&cone.transform);
	return AddBody(colShape, startTransform, component);
}

btRigidBody* Physics3D::CollisionShape(const PPlane& plane, RigidBodyComponent* component)
{
	btCollisionShape* colShape = new btStaticPlaneShape(btVector3(plane.normal.x, plane.normal.y, plane.normal.z), plane.constant);
	btTransform startTransform;
	startTransform.setFromOpenGLMatrix(&plane.transform);
	return AddBody(colShape, startTransform, component);
}

btRigidBody* Physics3D::CollisionShape(const std::shared_ptr<Mesh> mesh, RigidBodyComponent* component)
{
	btIndexedMesh indexedMesh;
	indexedMesh.m_numTriangles = mesh->GetIndicesSize() / 3;
	indexedMesh.m_triangleIndexBase = (unsigned char*)&mesh->GetIndicesVector()[0];
	indexedMesh.m_triangleIndexStride = 3 * sizeof(unsigned int);
	indexedMesh.m_numVertices = mesh->GetVerticesSize();
	indexedMesh.m_vertexBase = (unsigned char*)&mesh->GetVerticesVector()[0];
	indexedMesh.m_vertexStride = sizeof(Vertex);

	btTriangleIndexVertexArray* mTriangleIndexVertexArray = new btTriangleIndexVertexArray();
	mTriangleIndexVertexArray->addIndexedMesh(indexedMesh, PHY_INTEGER);

	btCollisionShape* colShape = new btBvhTriangleMeshShape(mTriangleIndexVertexArray, true, true);
	btTransform startTransform;
	startTransform.setFromOpenGLMatrix(&component->owner->GetComponent<TransformComponent>()->float4x4ToMat4x4());
	return AddBody(colShape, startTransform, component);
}

btRigidBody* Physics3D::AddBody(btCollisionShape* colShape, btTransform startTransform, RigidBodyComponent* component)
{
	float mass = (component->useGravity && !component->isKinematic) ? component->GetMass() : 0.0f;

	btVector3 localInertia(0, 0, 0);
	if (mass != 0.f)
		colShape->calculateLocalInertia(mass, localInertia);

	btDefaultMotionState* myMotionState = new btDefaultMotionState(startTransform);
	btRigidBody::btRigidBodyConstructionInfo rbInfo(mass, myMotionState, colShape, localInertia);

	btRigidBody* body = new btRigidBody(rbInfo);

	// Set Flags
	if (component->isKinematic)
	{
		body->setCollisionFlags(body->getFlags() | btCollisionObject::CF_KINEMATIC_OBJECT);
		body->setActivationState(DISABLE_DEACTIVATION);
	}		
	if (app->sceneManager->GetGameState() != GameState::PLAYING)
		body->setActivationState(ISLAND_SLEEPING);
	if(component->trigger) body->setCollisionFlags(body->getFlags() | btCollisionObject::CF_NO_CONTACT_RESPONSE);

	// Add body to world
	world->addRigidBody(body);
	bodies.push_back(component);
	bodiesNames.push_back(component->owner->name);

	return body;
}

void Physics3D::DeleteBody(RigidBodyComponent* body, std::string name)
{
	if (body != nullptr)
	{
		world->removeRigidBody(body->GetBody());
		if (body->constraintBodies.empty() == false)
		{
			std::vector<RigidBodyComponent*>::const_iterator k;
			bool clean = false;
			int size = body->GetBody()->getNumConstraintRefs();
			for (size_t i = 0; i < size; i++)
			{
				world->removeConstraint(body->GetBody()->getConstraintRef(0)); // index must be 0
			}
			for (int i = 0; i < bodies.size(); i++)
			{
				if (bodies.at(i)->GetBody() != body->GetBody() && bodies.at(i)->constraintBodies.empty() == false)
				{
					while (clean == false)
					{
						for (k = bodies.at(i)->constraintBodies.begin(); k != bodies.at(i)->constraintBodies.end(); ++k)
						{
							if (*k._Ptr == body)
							{
								bodies.at(i)->constraintBodies.erase(k);
								clean = false;
								break;
							}
							else clean = true;
						}
						if (bodies.at(i)->constraintBodies.empty()) clean = true;
					}
				}
			}
		}
		for (std::vector<RigidBodyComponent*>::iterator i = bodies.begin(); i != bodies.end(); ++i)
		{
			if (*i._Ptr == body)
			{
				bodies.erase(i);
				break;
			}
		}
		for (std::vector<std::string>::iterator i = bodiesNames.begin(); i != bodiesNames.end(); ++i)
		{
			if (*i._Ptr == name)
			{
				bodiesNames.erase(i);
				break;
			}
		}
	}
}

void Physics3D::DesactivateCollision(btRigidBody* body)
{
	world->removeCollisionObject(body);
}
void Physics3D::ActivateCollision(btRigidBody* body)
{
	world->addCollisionObject(body);
}

// =============================================
void Physics3D::AddConstraintP2P(btRigidBody& bodyA, btRigidBody& bodyB, const float3& anchorA, const float3& anchorB)
{
	btTypedConstraint* p2p = new btPoint2PointConstraint(bodyA, bodyB, anchorA, anchorB);
	world->addConstraint(p2p);
	bodyA.addConstraintRef(p2p);
	bodyB.addConstraintRef(p2p);
	p2p->setDbgDrawSize(2.0f);
}
void Physics3D::AddConstraintHinge(btRigidBody& bodyA, btRigidBody& bodyB, const float3& anchorA, const float3& anchorB, const float3& axisA, const float3& axisB, bool disable_collision)
{
	btHingeConstraint* hinge = new btHingeConstraint(bodyA, bodyB, anchorA, anchorB, axisA, axisB);
	world->addConstraint(hinge, disable_collision);
	bodyA.addConstraintRef(hinge);
	bodyB.addConstraintRef(hinge);
	hinge->setDbgDrawSize(2.0f);
}

void Physics3D::SleepAllBodies()
{
	for (int i = 0; i < bodies.size(); i++)
	{
		bodies.at(i)->GetBody()->setActivationState(ISLAND_SLEEPING);
	}
}
void Physics3D::ActiveAllBodies()
{
	for (int i = 0; i < bodies.size(); i++)
	{
		if(bodies.at(i)->isKinematic)
			bodies.at(i)->GetBody()->setActivationState(DISABLE_DEACTIVATION);
		else
			bodies.at(i)->GetBody()->setActivationState(ACTIVE_TAG);
	}
}

// =============================================

//This functions are used directly by Bullet. Do not touch!
void DebugDrawer::drawLine(const btVector3& from, const btVector3& to, const btVector3& color)
{
	line.origin.Set(from.getX(), from.getY(), from.getZ());
	line.destination.Set(to.getX(), to.getY(), to.getZ());
	line.color.Set(color.getX(), color.getY(), color.getZ());
	line.Render();
}

void DebugDrawer::drawContactPoint(const btVector3& PointOnB, const btVector3& normalOnB, btScalar distance, int lifeTime, const btVector3& color)
{
	point.transform.Translate(PointOnB.getX(), PointOnB.getY(), PointOnB.getZ());
	point.color.Set(color.getX(), color.getY(), color.getZ());
	point.Render();
}

void DebugDrawer::reportErrorWarning(const char* warningString)
{
	LOG(LogType::L_WARNING, "Bullet warning: %s", warningString);
}

void DebugDrawer::draw3dText(const btVector3& location, const char* textString)
{
	LOG(LogType::L_NORMAL, "Bullet draw text: %s", textString);
}

void DebugDrawer::setDebugMode(int debugMode)
{
	mode = (DebugDrawModes)debugMode;
}

int	 DebugDrawer::getDebugMode() const
{
	return mode;
}