//
// Copyright (c) 2009-2010 Mikko Mononen memon@inside.org
//
// This software is provided 'as-is', without any express or implied
// warranty.  In no event will the authors be held liable for any damages
// arising from the use of this software.
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgment in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.
//

#define _USE_MATH_DEFINES
#include <math.h>
#include <stdio.h>
#include <ctype.h>
#include <string.h>
#include <algorithm>

#include "Recast/Recast.h"
#include "DebugUtils/DebugDraw.h"
#include "DebugUtils/RecastDebugDraw.h"
#include "Detour/DetourNavMesh.h"

#include "InputGeom.h"
#include "Application.h"
#include "Mesh.h"
#include "Globals.h"
#include "ResourceManager.h"
#include "ModuleRenderer3D.h"
#include "VertexBuffer.h"

#include "mmgr/mmgr.h"

static bool intersectSegmentTriangle(const float* sp, const float* sq,
									 const float* a, const float* b, const float* c,
									 float &t)
{
	float v, w;
	float ab[3], ac[3], qp[3], ap[3], norm[3], e[3];
	rcVsub(ab, b, a);
	rcVsub(ac, c, a);
	rcVsub(qp, sp, sq);
	
	// Compute triangle normal. Can be precalculated or cached if
	// intersecting multiple segments against the same triangle
	rcVcross(norm, ab, ac);
	
	// Compute denominator d. If d <= 0, segment is parallel to or points
	// away from triangle, so exit early
	float d = rcVdot(qp, norm);
	if (d <= 0.0f) return false;
	
	// Compute intersection t value of pq with plane of triangle. A ray
	// intersects iff 0 <= t. Segment intersects iff 0 <= t <= 1. Delay
	// dividing by d until intersection has been found to pierce triangle
	rcVsub(ap, sp, a);
	t = rcVdot(ap, norm);
	if (t < 0.0f) return false;
	if (t > d) return false; // For segment; exclude this code line for a ray test
	
	// Compute barycentric coordinate components and test if within bounds
	rcVcross(e, qp, ap);
	v = rcVdot(ac, e);
	if (v < 0.0f || v > d) return false;
	w = -rcVdot(ab, e);
	if (w < 0.0f || v + w > d) return false;
	
	// Segment/ray intersects triangle. Perform delayed division
	t /= d;
	
	return true;
}

static char* parseRow(char* buf, char* bufEnd, char* row, int len)
{
	bool start = true;
	bool done = false;
	int n = 0;
	while (!done && buf < bufEnd)
	{
		char c = *buf;
		buf++;
		// multirow
		switch (c)
		{
			case '\n':
				if (start) break;
				done = true;
				break;
			case '\r':
				break;
			case '\t':
			case ' ':
				if (start) break;
			default:
				start = false;
				row[n++] = c;
				if (n >= len-1)
					done = true;
				break;
		}
	}
	row[n] = '\0';
	return buf;
}

InputGeom::InputGeom() :
	m_chunkyMesh(nullptr),
	m_mesh(),
	m_hasBuildSettings(false),
	m_offMeshConCount(0),
	m_volumeCount(0)
{
}

InputGeom::~InputGeom()
{
	if (m_chunkyMesh != nullptr)
	{
		delete m_chunkyMesh;
		m_chunkyMesh = nullptr;
	}

	if (m_mesh != nullptr)
	{
		m_mesh->vertices.clear();
		m_mesh->indices.clear();

		delete m_mesh;
		m_mesh = nullptr;
	}
}


bool InputGeom::loadMesh(Mesh* mesh)
{
	if (m_mesh)
	{
		delete m_chunkyMesh;
		m_chunkyMesh = nullptr;

		m_mesh->vertices.clear();
		m_mesh->indices.clear();
		delete m_mesh;
		m_mesh = nullptr;
	}
	m_offMeshConCount = 0;
	m_volumeCount = 0;

	float* vertices = new float[mesh->GetVerticesSize() * 3];
	const std::vector<Vertex>& meshVertices = mesh->GetVerticesVector();
	for (int i = 0; i < mesh->GetVerticesSize(); i++)
	{
		vertices[i * 3]		= meshVertices[i].position.x;
		vertices[i * 3 + 1] = meshVertices[i].position.y;
		vertices[i * 3 + 2] = meshVertices[i].position.z;

		m_mesh->vertices[i] = mesh->GetVerticesVector()[i].position;
	}

	rcCalcBounds(vertices, mesh->GetVerticesSize(), m_meshBMin, m_meshBMax);

	m_chunkyMesh = new rcChunkyTriMesh();
	if (!m_chunkyMesh)
	{
		LOG(LogType::L_ERROR, "buildTiledNavigation: Out of memory 'm_chunkyMesh'.");
		return false;
	}

	if (!rcCreateChunkyTriMesh(vertices, (int*)&mesh->GetIndicesVector()[0], m_mesh->indices.size() / 3, 256, m_chunkyMesh))
	{
		LOG(LogType::L_ERROR, "buildTiledNavigation: Failed to build chunky mesh.");
		RELEASE_ARRAY(vertices);
		return false;
	}		
	
	RELEASE_ARRAY(vertices);
	return true;
}

bool InputGeom::SetChunkyMesh()
{
	if (m_chunkyMesh)
	{
		delete m_chunkyMesh;
		m_chunkyMesh = nullptr;
	}

	float* vertices = new float[m_mesh->vertices.size() * 3];
	const std::vector<float3>& meshVertices = m_mesh->vertices;
	for (int i = 0; i < m_mesh->vertices.size(); i++)
	{
		vertices[i * 3] = meshVertices[i].x;
		vertices[i * 3 + 1] = meshVertices[i].y;
		vertices[i * 3 + 2] = meshVertices[i].z;
	}

	rcCalcBounds(vertices, m_mesh->vertices.size(), m_meshBMin, m_meshBMax);

	m_chunkyMesh = new rcChunkyTriMesh();
	if (!m_chunkyMesh)
	{
		LOG(LogType::L_ERROR, "buildTiledNavigation: Out of memory 'm_chunkyMesh'.");
		return false;
	}
	if (!rcCreateChunkyTriMesh(vertices, (int*)&m_mesh->indices[0], m_mesh->indices.size() / 3, 256, m_chunkyMesh))
	{
		LOG(LogType::L_ERROR, "buildTiledNavigation: Failed to build chunky mesh.");
		//RELEASE_ARRAY(vertices);
		return false;
	}

	RELEASE_ARRAY(vertices);

	return true;
}

void InputGeom::SetMesh()
{
	m_mesh = new SimpleMesh();
}

void InputGeom::MergeToMesh(std::shared_ptr<Mesh> new_mesh, float4x4 new_mesh_transform)
{
	if (m_mesh == nullptr)
		m_mesh = new SimpleMesh();
	++m_offMeshConCount;
	++m_volumeCount;

	// Vertex Merging =====================================================================
	int total_vertices = m_mesh->vertices.size() + new_mesh->GetVerticesSize();
	std::vector<float3> merged_vertices;

	int indices_offset = m_mesh->vertices.size();

	merged_vertices = m_mesh->vertices;

	std::vector<float3> new_mesh_vertices;
	new_mesh_vertices.reserve(new_mesh->GetVerticesSize());

	for (int i = 0; i < new_mesh->GetVerticesSize(); i++)
		new_mesh_vertices.push_back(new_mesh->GetVerticesVector()[i].position);

	for (size_t i = 0; i < new_mesh->GetVerticesSize(); i++)
	{
		float3 vertex = new_mesh_transform.MulPos(new_mesh_vertices[i]);
		merged_vertices.push_back(vertex);
	}

	new_mesh_vertices.clear();
	m_mesh->vertices.clear();

	m_mesh->vertices = merged_vertices;

	//Indices Merging =====================================================================

	int total_indices = m_mesh->indices.size() + new_mesh->GetIndicesSize();
	std::vector<unsigned int> merged_indices;

	merged_indices = m_mesh->indices;

	for (size_t i = 0; i < new_mesh->GetIndicesSize(); i++)
		merged_indices.push_back(new_mesh->GetIndicesVector()[i] + indices_offset);

	m_mesh->indices.clear();

	m_mesh->indices = merged_indices;
}

void InputGeom::DrawMesh()
{
	if (m_mesh != nullptr)
	{
		for (size_t i = 0; i < m_mesh->vertices.size() / 3; i += 3)
		{
			float3 a = float3(m_mesh->vertices[i]);
			float3 b = float3(m_mesh->vertices[i + 1]);
			float3 c = float3(m_mesh->vertices[i + 2]);

			//app->renderer3D->AddDebugTriangles(a, b, c, float3(0.1, 0.75, 0.5));
		}
	}
}

static bool isectSegAABB(const float* sp, const float* sq,
						 const float* amin, const float* amax,
						 float& tmin, float& tmax)
{
	static const float EPS = 1e-6f;
	
	float d[3];
	d[0] = sq[0] - sp[0];
	d[1] = sq[1] - sp[1];
	d[2] = sq[2] - sp[2];
	tmin = 0.0;
	tmax = 1.0f;
	
	for (int i = 0; i < 3; i++)
	{
		if (fabsf(d[i]) < EPS)
		{
			if (sp[i] < amin[i] || sp[i] > amax[i])
				return false;
		}
		else
		{
			const float ood = 1.0f / d[i];
			float t1 = (amin[i] - sp[i]) * ood;
			float t2 = (amax[i] - sp[i]) * ood;
			if (t1 > t2) { float tmp = t1; t1 = t2; t2 = tmp; }
			if (t1 > tmin) tmin = t1;
			if (t2 < tmax) tmax = t2;
			if (tmin > tmax) return false;
		}
	}
	
	return true;
}


bool InputGeom::raycastMesh(float* src, float* dst, float& tmin)
{
	if (m_chunkyMesh == nullptr)
		return false;
	float dir[3];
	rcVsub(dir, dst, src);

	// Prune hit ray.
	float btmin, btmax;
	if (!isectSegAABB(src, dst, m_meshBMin, m_meshBMax, btmin, btmax))
		return false;

	float p[2], q[2];
	p[0] = src[0] + (dst[0]-src[0])*btmin;
	p[1] = src[2] + (dst[2]-src[2])*btmin;
	q[0] = src[0] + (dst[0]-src[0])*btmax;
	q[1] = src[2] + (dst[2]-src[2])*btmax;
	
	int cid[512];
	const int ncid = rcGetChunksOverlappingSegment(m_chunkyMesh, p, q, cid, 512);
	if (!ncid)
		return false;
	
	tmin = 1.0f;
	bool hit = false;
	float* verts = new float[m_mesh->vertices.size() * 3];
	const std::vector<float3>& meshVertices = m_mesh->vertices;
	for (int i = 0; i < m_mesh->vertices.size(); i++)
	{
		verts[i * 3] = meshVertices[i].x;
		verts[i * 3 + 1] = meshVertices[i].y;
		verts[i * 3 + 2] = meshVertices[i].z;
	}
	
	for (int i = 0; i < ncid; ++i)
	{
		const rcChunkyTriMeshNode& node = m_chunkyMesh->nodes[cid[i]];
		const int* tris = &m_chunkyMesh->tris[node.i*3];
		const int ntris = node.n;

		for (int j = 0; j < ntris*3; j += 3)
		{
			float t = 1;
			if (intersectSegmentTriangle(src, dst,
										 &verts[tris[j]*3],
										 &verts[tris[j+1]*3],
										 &verts[tris[j+2]*3], t))
			{
				if (t < tmin)
					tmin = t;
				hit = true;
			}
		}
	}
	
	verts = nullptr;

	return hit;
}

void InputGeom::addOffMeshConnection(const float* spos, const float* epos, const float rad,
									 unsigned char bidir, unsigned char area, unsigned short flags)
{
	if (m_offMeshConCount >= MAX_OFFMESH_CONNECTIONS) return;
	float* v = &m_offMeshConVerts[m_offMeshConCount*3*2];
	m_offMeshConRads[m_offMeshConCount] = rad;
	m_offMeshConDirs[m_offMeshConCount] = bidir;
	m_offMeshConAreas[m_offMeshConCount] = area;
	m_offMeshConFlags[m_offMeshConCount] = flags;
	m_offMeshConId[m_offMeshConCount] = 1000 + m_offMeshConCount;
	rcVcopy(&v[0], spos);
	rcVcopy(&v[3], epos);
	m_offMeshConCount++;
}

void InputGeom::deleteOffMeshConnection(int i)
{
	m_offMeshConCount--;
	float* src = &m_offMeshConVerts[m_offMeshConCount*3*2];
	float* dst = &m_offMeshConVerts[i*3*2];
	rcVcopy(&dst[0], &src[0]);
	rcVcopy(&dst[3], &src[3]);
	m_offMeshConRads[i] = m_offMeshConRads[m_offMeshConCount];
	m_offMeshConDirs[i] = m_offMeshConDirs[m_offMeshConCount];
	m_offMeshConAreas[i] = m_offMeshConAreas[m_offMeshConCount];
	m_offMeshConFlags[i] = m_offMeshConFlags[m_offMeshConCount];
}

void InputGeom::drawOffMeshConnections(duDebugDraw* dd, bool hilight)
{
	unsigned int conColor = duRGBA(192,0,128,192);
	unsigned int baseColor = duRGBA(0,0,0,64);
	dd->depthMask(false);

	dd->begin(DU_DRAW_LINES, 2.0f);
	for (int i = 0; i < m_offMeshConCount; ++i)
	{
		float* v = &m_offMeshConVerts[i*3*2];

		dd->vertex(v[0],v[1],v[2], baseColor);
		dd->vertex(v[0],v[1]+0.2f,v[2], baseColor);
		
		dd->vertex(v[3],v[4],v[5], baseColor);
		dd->vertex(v[3],v[4]+0.2f,v[5], baseColor);
		
		duAppendCircle(dd, v[0],v[1]+0.1f,v[2], m_offMeshConRads[i], baseColor);
		duAppendCircle(dd, v[3],v[4]+0.1f,v[5], m_offMeshConRads[i], baseColor);

		if (hilight)
		{
			duAppendArc(dd, v[0],v[1],v[2], v[3],v[4],v[5], 0.25f,
						(m_offMeshConDirs[i]&1) ? 0.6f : 0.0f, 0.6f, conColor);
		}
	}	
	dd->end();

	dd->depthMask(true);
}

void InputGeom::addConvexVolume(const float* verts, const int nverts,
								const float minh, const float maxh, unsigned char area)
{
	if (m_volumeCount >= MAX_VOLUMES) return;
	ConvexVolume* vol = &m_volumes[m_volumeCount++];
	memset(vol, 0, sizeof(ConvexVolume));
	memcpy(vol->verts, verts, sizeof(float)*3*nverts);
	vol->hmin = minh;
	vol->hmax = maxh;
	vol->nverts = nverts;
	vol->area = area;
}

void InputGeom::deleteConvexVolume(int i)
{
	m_volumeCount--;
	m_volumes[i] = m_volumes[m_volumeCount];
}

void InputGeom::drawConvexVolumes(struct duDebugDraw* dd, bool /*hilight*/)
{
	dd->depthMask(false);

	dd->begin(DU_DRAW_TRIS);
	
	for (int i = 0; i < m_volumeCount; ++i)
	{
		const ConvexVolume* vol = &m_volumes[i];
		unsigned int col = duIntToCol(vol->area, 32);
		for (int j = 0, k = vol->nverts-1; j < vol->nverts; k = j++)
		{
			const float* va = &vol->verts[k*3];
			const float* vb = &vol->verts[j*3];

			dd->vertex(vol->verts[0],vol->hmax,vol->verts[2], col);
			dd->vertex(vb[0],vol->hmax,vb[2], col);
			dd->vertex(va[0],vol->hmax,va[2], col);
			
			dd->vertex(va[0],vol->hmin,va[2], duDarkenCol(col));
			dd->vertex(va[0],vol->hmax,va[2], col);
			dd->vertex(vb[0],vol->hmax,vb[2], col);

			dd->vertex(va[0],vol->hmin,va[2], duDarkenCol(col));
			dd->vertex(vb[0],vol->hmax,vb[2], col);
			dd->vertex(vb[0],vol->hmin,vb[2], duDarkenCol(col));
		}
	}
	
	dd->end();

	dd->begin(DU_DRAW_LINES, 2.0f);
	for (int i = 0; i < m_volumeCount; ++i)
	{
		const ConvexVolume* vol = &m_volumes[i];
		unsigned int col = duIntToCol(vol->area, 220);
		for (int j = 0, k = vol->nverts-1; j < vol->nverts; k = j++)
		{
			const float* va = &vol->verts[k*3];
			const float* vb = &vol->verts[j*3];
			dd->vertex(va[0],vol->hmin,va[2], duDarkenCol(col));
			dd->vertex(vb[0],vol->hmin,vb[2], duDarkenCol(col));
			dd->vertex(va[0],vol->hmax,va[2], col);
			dd->vertex(vb[0],vol->hmax,vb[2], col);
			dd->vertex(va[0],vol->hmin,va[2], duDarkenCol(col));
			dd->vertex(va[0],vol->hmax,va[2], col);
		}
	}
	dd->end();

	dd->begin(DU_DRAW_POINTS, 3.0f);
	for (int i = 0; i < m_volumeCount; ++i)
	{
		const ConvexVolume* vol = &m_volumes[i];
		unsigned int col = duDarkenCol(duIntToCol(vol->area, 255));
		for (int j = 0; j < vol->nverts; ++j)
		{
			dd->vertex(vol->verts[j*3+0],vol->verts[j*3+1]+0.1f,vol->verts[j*3+2], col);
			dd->vertex(vol->verts[j*3+0],vol->hmin,vol->verts[j*3+2], col);
			dd->vertex(vol->verts[j*3+0],vol->hmax,vol->verts[j*3+2], col);
		}
	}
	dd->end();
	
	dd->depthMask(true);
}
