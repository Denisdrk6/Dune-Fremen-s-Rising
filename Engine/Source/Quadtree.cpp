#include "Quadtree.h"
#include "Globals.h"

#include "GameObject.h"
#include "VertexBuffer.h"
#include "IndexBuffer.h"

#include "Geometry/LineSegment.h"
#include "Profiling.h"

Quadtree::Quadtree() : root(nullptr)
{
}

Quadtree::~Quadtree()
{
	RELEASE(root);
}

void Quadtree::Create(AABB limits)
{
	RG_PROFILING_FUNCTION("Creating the quadtree");
	root = new QuadtreeNode(limits);
}

void Quadtree::Clear()
{
	RG_PROFILING_FUNCTION("Clearing the quadtree");
	RELEASE(root);
}

void Quadtree::Insert(GameObject* go)
{
	if (root != nullptr)
	{
		if (go->GetAABB().Intersects(root->GetBox()))
		{
			root->Insert(go);
		}
	}
}

void Quadtree::Remove(GameObject* go)
{
	std::stack<QuadtreeNode*> nodes;
	if (root) nodes.push(root);

	while (!nodes.empty())
	{
		QuadtreeNode* node = nodes.top();

		std::vector<GameObject*>::const_iterator it = std::find(node->GetObjects().begin(), node->GetObjects().end(), go);
		if (it != node->GetObjects().end())
			node->Remove(it);

		nodes.pop();

		if (node->IsSlotAvailable() == false)
		{
			for (int i = 0; i < 4; ++i)
				nodes.push(node->GetChild(i));
		}
	}
}

void Quadtree::Intersect(std::vector<GameObject*>& gos, Ray ray)
{
	if (root != nullptr)
	{
		std::stack<QuadtreeNode*> nodes;
		nodes.push(root);

		while (!nodes.empty())
		{
			QuadtreeNode* node = nodes.top();
			nodes.pop();

			if (ray.Intersects(node->GetBox()))
			{
				for (std::vector<GameObject*>::const_iterator it = node->GetObjects().begin(); it != node->GetObjects().end(); ++it)
				{
					if (ray.Intersects((*it)->GetAABB()))
						gos.push_back(*it);
				}

				for (int i = 0; i < 4; ++i)
				{
					if (node->GetChild(i) != nullptr) nodes.push(node->GetChild(i));
				}
			}
		}
	}
}

void Quadtree::Intersect(std::set<GameObject*>& gos, CameraComponent* frustum, float scaleFactor)
{
	if (root != nullptr && frustum != nullptr)
	{
		std::stack<QuadtreeNode*> nodes;

		nodes.push(root);

		while (!nodes.empty())
		{
			QuadtreeNode* node = nodes.top();
			nodes.pop();

			int intersect = 0;
			if (node)
			{
				AABB aabb = node->GetBox();
				aabb.Scale(aabb.CenterPoint(), scaleFactor);
				intersect = frustum->ContainsAaBox(aabb);
			}
			if (intersect == 1 || intersect == 2)
			{
				for (std::vector<GameObject*>::const_iterator it = node->GetObjects().begin(); it != node->GetObjects().end(); ++it)
				{
					AABB aabb = (*it)->GetAABB();
					aabb.Scale(aabb.CenterPoint(), scaleFactor);
					intersect = frustum->ContainsAaBox(aabb);
					if ((*it)->active && (intersect == 1 || intersect == 2))
						gos.insert(*it);
				}

				for (int i = 0; i < 4; ++i)
				{
					if (node->GetChild(i) != nullptr) nodes.push(node->GetChild(i));
				}
			}
		}
	}
}

void Quadtree::CollectGo(std::set<GameObject*>& gos, std::stack<QuadtreeNode*>& nodes)
{
	while (!nodes.empty())
	{
		QuadtreeNode* node = nodes.top();
		for (std::vector<GameObject*>::const_iterator it = node->GetObjects().begin(); it != node->GetObjects().end(); ++it)
			gos.insert(*it);

		nodes.pop();
	}
}

void Quadtree::CollectGoOnlyStatic(std::set<GameObject*>& gos, std::stack<QuadtreeNode*>& nodes)
{
	while (!nodes.empty())
	{
		QuadtreeNode* node = nodes.top();
		for (std::vector<GameObject*>::const_iterator it = node->GetObjects().begin(); it != node->GetObjects().end(); ++it)
			if((*it)->staticObj) gos.insert(*it);

		nodes.pop();
	}
}

void Quadtree::CollectGoByTag(std::set<GameObject*>& gos, std::stack<QuadtreeNode*>& nodes, std::string tag)
{
	while (!nodes.empty())
	{
		QuadtreeNode* node = nodes.top();
		for (std::vector<GameObject*>::const_iterator it = node->GetObjects().begin(); it != node->GetObjects().end(); ++it)
			if ((*it)->tag == tag) gos.insert(*it);

		nodes.pop();
	}
}

void Quadtree::CollectNodes(std::stack<QuadtreeNode*>& nodes, LineSegment ray)
{
	std::stack<QuadtreeNode*> stack;
	stack.push(root);

	while (!stack.empty())
	{
		QuadtreeNode* node = stack.top();
		if (ray.Intersects(node->GetBox()))
			nodes.push(node);

		stack.pop();

		for (int i = 0; i < 4; ++i)
			if (node->GetChild(i) != nullptr) stack.push(node->GetChild(i));
	}
}

void Quadtree::DebugDraw()
{
	if (root != nullptr)
	{
		root->DebugDraw();
	}
}

QuadtreeNode::QuadtreeNode(AABB limits) : box(limits)
{
	childs[0] = nullptr;
	childs[1] = nullptr;
	childs[2] = nullptr;
	childs[3] = nullptr;

	float3 corners[8];
	limits.GetCornerPoints(corners);

	unsigned int indices[24] =
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

	vbo = new VertexBuffer(corners, sizeof(float3) * 8);
	ebo = new IndexBuffer(indices, 24);
}

QuadtreeNode::~QuadtreeNode()
{
	RELEASE(childs[0]);
	RELEASE(childs[1]);
	RELEASE(childs[2]);
	RELEASE(childs[3]);

	gameObjects.clear();

	RELEASE(vbo);
	RELEASE(ebo);
}

void QuadtreeNode::Insert(GameObject* go)
{
	RG_PROFILING_FUNCTION("Quadtree Insert GameObject");

	if (IsSlotAvailable() && (gameObjects.size() < 4 || box.HalfSize().LengthSq() <= 20.0f * 20.0f))
	{
		gameObjects.push_back(go);
	}
	else
	{
		if (IsSlotAvailable()) CreateChilds();

		gameObjects.push_back(go);
		RedistributeChilds();
	}
}

void QuadtreeNode::CreateChilds()
{
	float3 boxSize(box.Size());
	float3 childsSize(boxSize.x * 0.5f, boxSize.y, boxSize.z * 0.5f);

	float3 center(box.CenterPoint());
	float3 childCenter(center);
	AABB childAABB;

	// Upper left
	childCenter.x = center.x - (boxSize.x * 0.25f);
	childCenter.z = center.z + (boxSize.z * 0.25f);
	childAABB.SetFromCenterAndSize(childCenter, childsSize);
	childs[0] = new QuadtreeNode(childAABB);

	// Upper right
	childCenter.x = center.x + (boxSize.x * 0.25f);
	childCenter.z = center.z + (boxSize.z * 0.25f);
	childAABB.SetFromCenterAndSize(childCenter, childsSize);
	childs[1] = new QuadtreeNode(childAABB);

	// Bottom left
	childCenter.x = center.x - (boxSize.x * 0.25f);
	childCenter.z = center.z - (boxSize.z * 0.25f);
	childAABB.SetFromCenterAndSize(childCenter, childsSize);
	childs[2] = new QuadtreeNode(childAABB);

	// Bottom right
	childCenter.x = center.x + (boxSize.x * 0.25f);
	childCenter.z = center.z - (boxSize.z * 0.25f);
	childAABB.SetFromCenterAndSize(childCenter, childsSize);
	childs[3] = new QuadtreeNode(childAABB);
}

void QuadtreeNode::RedistributeChilds()
{
	RG_PROFILING_FUNCTION("Quadtree Redistribute GameObjects");

	for (std::vector<GameObject*>::iterator it = gameObjects.begin(); it != gameObjects.end();)
	{
		GameObject* go = *it;
		AABB goAABB = go->GetAABB().MinimalEnclosingAABB();

		bool intersection[4];

		for (int i = 0; i < 4; ++i)
			intersection[i] = childs[i]->box.Intersects(goAABB);

		if (intersection[0] && intersection[1] && intersection[2] && intersection[3])
			++it;
		else
		{
			it = gameObjects.erase(it);
			for (int i = 0; i < 4; ++i)
				if (intersection[i]) childs[i]->Insert(go);
		}
	}
}

void QuadtreeNode::DebugDraw()
{
	glEnableClientState(GL_VERTEX_ARRAY);

	vbo->Bind();
	glVertexPointer(3, GL_FLOAT, 0, NULL);
	ebo->Bind();

	glDrawElements(GL_LINES, ebo->GetCount(), GL_UNSIGNED_INT, NULL);

	vbo->Unbind();
	ebo->Unbind();

	glDisableClientState(GL_VERTEX_ARRAY);

	if (!IsSlotAvailable())
	{
		for (int i = 0; i < 4; ++i)
			childs[i]->DebugDraw();
	}
}