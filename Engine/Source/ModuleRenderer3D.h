#pragma once
#include "Module.h"

#include "Shapes.h"
#include "SDL_video.h"
#include "Geometry/AABB.h"

#include <vector>
#include <set>
#include <utility>

#define MAX_LIGHTS 8

class VertexArray;
class VertexBuffer;
class IndexBuffer;
class Framebuffer;
class TextureBuffer;
class Material;
class Shader;
class Texture;
class GameObject;
class CameraComponent;

class PointLight;
class SpotLight;
class DirectionalLight;

class ModuleRenderer3D : public Module
{
public:
	ModuleRenderer3D(bool startEnabled = true);
	~ModuleRenderer3D();

	bool Init(JsonParsing& node) override;
	bool Start() override;
	bool PreUpdate(float dt) override;
	bool PostUpdate();
	bool CleanUp();

	bool LoadConfig(JsonParsing& node) override;
	bool SaveConfig(JsonParsing& node) override;

	void OnResize(int width, int height);

	void SetDepthTest();
	void SetCullFace();
	void SetLighting();
	void SetColorMaterial();
	void SetTexture2D();
	void SetStencil();
	void SetBlending();
	void SetWireMode();
	void SetVsync(bool newValue);

	inline bool* GetDepthTest() { return &depthTest; }
	inline bool* GetCullFace() { return &cullFace; }
	inline bool* GetLighting() { return &lighting; }
	inline bool* GetColorMaterial() { return &colorMaterial; }
	inline bool* GetTexture2D() { return &texture2D; }
	inline bool* GetStencil() { return &stencil; }
	inline bool* GetBlending() { return &blending; }
	inline bool* GetWireMode() { return &wireMode; }
	inline bool* GetVsync() { return &vsync; }
	inline bool* GetRayCast() { return &rayCast; }
	inline bool* GetNavMesh() { return &navMesh; }
	inline bool* GetDrawGrid() { return &drawGrid; }

	void DrawCubeDirectMode();

	Material* GetDefaultMaterial();
	unsigned int GetDefaultShader();

	void AddMaterial(Material* material);
	inline const std::vector<Shader*>& GetShaders() { return shaders; }

	void AddPointLight(PointLight* pl);
	inline std::vector<PointLight*>& GetPointLights() { return pointLights; }

	void AddSpotLight(SpotLight* sl);
	inline const std::vector<SpotLight*>& GetSpotLights() { return spotLights; }

	void ClearPointLights();
	void ClearSpotLights();

	void RemovePointLight(PointLight* light);
	void RemoveSpotLight(SpotLight* light);

	void RequestDamageFeedback();

private:
	void PushCamera(const float4x4& proj, const float4x4& view);
	void DebugDraw(GameObject* objSelected);
	void GenerateShadows(const std::set<GameObject*>& objects, CameraComponent* gameCam, AABB& shadowsAABB);

	void DrawDamageFeedback();

public:
	PPlane grid;
	unsigned int shadowsDepthTexture;
	std::vector<std::pair<GameObject*, float3>> gosToDrawOutline;

	//Light lights[MAX_LIGHTS];
	SDL_GLContext context;
	Mat4x4 projectionMatrix;

	Framebuffer* fbo;
	Framebuffer* mainCameraFbo;

	bool depthTest;
	bool cullFace;
	bool lighting;
	bool colorMaterial;
	bool texture2D;
	bool stencil;
	bool blending;
	bool wireMode;
	bool vsync;
	bool rayCast;
	bool navMesh;
	bool drawGrid;

	GameObject* goDirLight;
	DirectionalLight* dirLight;

	std::vector<PointLight*> pointLights;
	std::vector<SpotLight*> spotLights;

	std::vector<float3> enemyCones;
	
	bool genShadows;

private:
	Material* defaultMaterial;
	unsigned int defaultShader;

	std::vector<Shader*> shaders;
	std::vector<Material*> materials;

	VertexArray* distVao;
	VertexBuffer* distVbo;
	IndexBuffer* distIbo;
	std::shared_ptr<Shader> postProcessingShader;
	
	std::shared_ptr<Shader> textureShader;
	std::shared_ptr<Texture> damageTexture;
	std::shared_ptr<Texture> whiteTexture;

	VertexBuffer* vbo;
	std::shared_ptr<Shader> coneShader;

	unsigned int shadowsFbo;
	
	bool dmgFeedbackRequested;
	
};