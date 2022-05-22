#pragma once
#include "GameObject.h"
#include "Particle.h"
#include "ParticleEffect.h"
#include "TransformComponent.h"
#include "Algorithm/Random/LCG.h"

#include <array>

#define INSTANCE_DATA_LENGHT 20

class Shader;
class VertexArray;
class VertexBuffer;
class IndexBuffer;
class TransformComponents;

struct ParticleVertex
{
	float3 position;
	float2 texCoords;
	float4 color;
};

class ParticleEmitter {

public:

	ParticleEmitter(GameObject* owner);
	~ParticleEmitter();

	virtual void Emit(float dt);
	void Render(CameraComponent* gameCam);
	virtual void UpdateParticle(float dt);
	virtual void Update(float dt);

	void OnEditor(int emitterIndex);
	void SetParticlesPerSecond(float particlesPerSec);

	void CreateParticleEffect(ParticleEffectType type);
	bool isEffectActive(ParticleEffectType type);
	std::string GetNameFromEffect(ParticleEffectType type);
	ParticleEffect* GetParticleEffect(ParticleEffectType type);

	virtual bool OnLoad(JsonParsing& node);
	virtual bool OnSave(JsonParsing& node, JSON_Array* array);

	void RestartEmitter();

	void SetDirection(float3 newDirection);

protected:
	void AddInstancedAttribute(unsigned int vao, unsigned int vbo, int attributeIndex, int dataSize, int instancedDataLength, int offset);
	void ShowTextureMenu();

	void StartBatch();
	void NextBatch();
	void DrawParticle(const float3& pos, float rotation, const float3& size, const float4& color, Quat newRotation);

	void SetUpBuffers();
public:

	int maxParticles;
	float particlesPerSecond;
	bool isActive, toDelete;

	std::vector<ParticleEffect*> effects;

	GameObject* own;

	float minLifeTime;
	float maxLifeTime;

	float timer;
	float currTimer;
	float loopTimer;
	float loopTimerOnEditor;

	int tilesX;
	int tilesY;

	//float2 texCoords[4] = {};
private:

	struct EmitterData
	{
		unsigned int maxQuads;
		unsigned int maxVertices;
		unsigned int maxIndices;
		//unsigned int maxTextureSlots = 32;

		VertexArray* vertexArray;
		VertexBuffer* vertexBuffer;
		IndexBuffer* indexBuffer;
		//std::array<std::shared_ptr<Texture>, 32> texture; // 32 == textureSlots
		//unsigned int textureSlotIndex = 1;

		std::shared_ptr<Shader> shader;

		unsigned int indexCount = 0;
		ParticleVertex* vertexBufferBase = nullptr;
		ParticleVertex* vertexBufferPtr = nullptr;
		
		float4 vertexPositions[4];

		unsigned int drawCalls;
	};
	EmitterData data;

	std::vector<Particle> particlePool;
	unsigned int poolIndex;

	std::string emitterName;
	char charsOfName[50];
	bool showTexMenu;
	LCG random;

	ParticleReference particleReference;
	std::shared_ptr<Texture> texture;

	ParticleEffect* effect;

	bool isEmitterCubical;
	float spreadDistanceX;
	float spreadDistanceY;
	float spreadDistanceZ;

	TransformComponent* transform;

	float iterTileX = 0;
	float iterTileY = 0;

	// Owner components
	TransformComponent* ownerTransformComponent;
};
