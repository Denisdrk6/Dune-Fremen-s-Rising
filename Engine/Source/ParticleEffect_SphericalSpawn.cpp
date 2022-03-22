#include "ParticleEffect_SphericalSpawn.h"

ParticleEffect_SphericalSpawn::ParticleEffect_SphericalSpawn() : ParticleEffect_SpawnShape(SPAWN_SHAPE_TYPE::SPHERE),
	radius(1.0f), 
	angle(0.0f),
	useDirection(false)
{
}

ParticleEffect_SphericalSpawn::~ParticleEffect_SphericalSpawn()
{
}

void ParticleEffect_SphericalSpawn::Spawn(Particle& particle, bool hasInitialSpeed, float speed, float4x4& gTrans, float* offset)
{
	//LCG random;

	////Get a random spawn point inside of a quad defined by a point and a radius
	//float u = random.Float(-radius, radius);
	//float x1 = random.Float(-radius, radius);
	//float x2 = random.Float(-radius, radius);
	//float x3 = random.Float(-radius, radius);

	//float mag = sqrt(x1 * x1 + x2 * x2 + x3 * x3);
	//x1 /= mag; x2 /= mag; x3 /= mag;
	//float c = cbrt(u);
	//float3 localPos;
	//localPos.x = offset[0] + x1 * c;
	//localPos.y = offset[1] + x2 * c;
	//localPos.z = offset[2] + x3 * c;
	//particle.position = gTrans.TransformPos(localPos);

	//if (hasInitialSpeed)
	//{
	//	float3 localSpeed = (localPos - float3(offset[0], offset[1], offset[2])).Normalized() * speed;
	//	particle.velocity = gTrans.TransformDir(localSpeed).Normalized() * speed;

	//	if (useDirection)
	//	{
	//		float3 direction = (localPos - float3(offset[0], offset[1], offset[2]));
	//		direction = gTrans.TransformDir(direction).Normalized();

	//		float4x4 cameraView = app->camera

	//		//float4x4 cameraView = EngineExternal->moduleCamera->editorCamera.ViewMatrixOpenGL().Transposed();
	//		//float4x4 cameraView = EngineExternal->moduleRenderer3D->GetGameRenderTarget()->ViewMatrixOpenGL().Transposed();

	//		direction = cameraView.TransformDir(direction);

	//		float2 directionViewProj = float2(direction.x, direction.y).Normalized();
	//		float2 xAxis = float2(1, 0);
	//		float finalAngle = xAxis.AngleBetween(directionViewProj);
	//		if (directionViewProj.y < 0)
	//			finalAngle = 360 * DEGTORAD - finalAngle;
	//		finalAngle += angle * DEGTORAD;

	//		particle.rotation = finalAngle;
	//	}

	//}
}

void ParticleEffect_SphericalSpawn::OnEditor(int emitterIndex)
{
}

bool ParticleEffect_SphericalSpawn::OnLoad(JsonParsing& node)
{
	return false;
}

bool ParticleEffect_SphericalSpawn::OnSave(JsonParsing& node, JSON_Array* array)
{
	return false;
}
