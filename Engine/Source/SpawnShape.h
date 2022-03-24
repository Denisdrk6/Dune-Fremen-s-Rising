#pragma once
#include "JsonParsing.h"

struct Particle;

enum class SPAWN_SHAPE_TYPE : int
{
	SPHERE,
	CONE,
	CIRCUMFERENCE,
};


class SpawnShape
{
public:

	SpawnShape(SPAWN_SHAPE_TYPE type);
	virtual ~SpawnShape(){}

	virtual void Spawn(Particle& particle, bool hasInitialSpeed, float speed, float4x4& gTrans, float* offset){}

	virtual void OnEditor(int emitterIndex){}

	virtual bool OnLoad(JsonParsing& node) {
		return true;
	}
	virtual bool OnSave(JsonParsing& node, JSON_Array* array) {
		return true;
	}

	SPAWN_SHAPE_TYPE GetType()const;

protected:

	SPAWN_SHAPE_TYPE shapeType;
};