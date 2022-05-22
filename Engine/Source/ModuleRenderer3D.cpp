#include "ModuleRenderer3D.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleWindow.h"
#include "ModuleCamera3D.h"
#include "ModuleEditor.h"
#include "ModuleSceneManager.h"
#include "ModuleUI.h"
#include "Physics3D.h"

#include "LightComponent.h"
#include "CameraComponent.h"
#include "TransformComponent.h"
#include "NavAgentComponent.h"

#include "ResourceManager.h"
#include "Scene.h"
#include "Lights.h"
#include "Framebuffer.h"
#include "VertexArray.h"
#include "VertexBuffer.h"
#include "IndexBuffer.h"
#include "Shader.h"
#include "Texture.h"
#include "NavMeshBuilder.h"
#include "GameView.h"
#include "Viewport.h"

#include "GL/glew.h"
#include "Imgui/imgui_impl_sdl.h"
#include "Imgui/imgui_impl_opengl3.h"
#include "Imgui/ImguiStyle.h"
#include "IL/ilut.h"

#include "Profiling.h"

ModuleRenderer3D::ModuleRenderer3D(bool startEnabled) : Module(startEnabled), mainCameraFbo(nullptr), rayCast(true)
{
	name = "Renderer";
	context = NULL;
	fbo = nullptr;

	depthTest = true;
	cullFace = true;
	colorMaterial = true;
	lighting = true;
	texture2D = true;
	stencil = true;
	blending = false;
	wireMode = false;
	vsync = false;
	rayCast = false;
	navMesh = false;
	drawGrid = false;
}

// Destructor
ModuleRenderer3D::~ModuleRenderer3D()
{
}

// Called before render is available
bool ModuleRenderer3D::Init(JsonParsing& node)
{
	DEBUG_LOG("Creating 3D Renderer context");
	bool ret = true;
	
	//Create context
	context = SDL_GL_CreateContext(app->window->window);
	if(context == NULL)
	{
		DEBUG_LOG("OpenGL context could not be created! SDL_Error: %s\n", SDL_GetError());
		ret = false;
	}
	
	if(ret == true)
	{
		GLenum err = glewInit();
		if (GLEW_OK != err)
		{
			/* Problem: glewInit failed, something is seriously wrong. */
			DEBUG_LOG("Error: %s\n", glewGetErrorString(err));
		}

		DEBUG_LOG("Status: Using GLEW %s\n", glewGetString(GLEW_VERSION));

		//Use Vsync
		vsync = node.GetJsonBool("vsync");
		if(SDL_GL_SetSwapInterval(vsync) < 0)
			DEBUG_LOG("Warning: Unable to set VSync! SDL Error: %s\n", SDL_GetError());

#ifndef DIST
		IMGUI_CHECKVERSION();
		ImGui::CreateContext();
		ImGuiIO& io = ImGui::GetIO(); 
		io.IniFilename = "Settings/imgui.ini";
		(void)io;
		io.ConfigFlags |= ImGuiConfigFlags_DockingEnable;

		//ImGui::StyleColorsDark();
		ImGui::DarkTheme();

		ImGui_ImplSDL2_InitForOpenGL(app->window->window, context);
		ImGui_ImplOpenGL3_Init();
#endif
		
		DEBUG_LOG("Vendor: %s", glGetString(GL_VENDOR));
		DEBUG_LOG("Renderer: %s", glGetString(GL_RENDERER));
		DEBUG_LOG("OpenGL version supported %s", glGetString(GL_VERSION));
		DEBUG_LOG("GLSL: %s\n", glGetString(GL_SHADING_LANGUAGE_VERSION));
		
		glHint(GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST);
		glClearDepth(1.0f);
		
		//Initialize clear color
		glClearColor(0.f, 0.f, 0.f, 1.f);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
		
		depthTest = node.GetJsonBool("depth test");
		cullFace = node.GetJsonBool("cull face");
		lighting = node.GetJsonBool("lighting");
		colorMaterial = node.GetJsonBool("color material");
		texture2D = node.GetJsonBool("texture 2D");
		stencil = node.GetJsonBool("stencil");
		blending = node.GetJsonBool("blending");
		wireMode = node.GetJsonBool("wire mode");
		drawGrid = node.GetJsonBool("draw grid");
		
		if (depthTest) SetDepthTest();
		if (cullFace) SetCullFace();
		if (lighting) SetLighting();
		if (colorMaterial) SetColorMaterial();
		if (texture2D) SetTexture2D();
		if (stencil) SetStencil();
		if (blending) SetBlending();
		if (wireMode) SetWireMode();		
	}
	
	int w = *app->window->GetWindowWidth();
	int h = *app->window->GetWindowHeight();
	OnResize(w, h);
	
	fbo = new Framebuffer(w, h, 1);
	fbo->Unbind();
	mainCameraFbo = new Framebuffer(w, h, 0);
	mainCameraFbo->Unbind();	

//#ifdef DIST
	distVao = new VertexArray();

	float vertices[] =
	{
		-1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
		 1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
		 1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
		-1.0f,  1.0f, 0.0f, 0.0f, 1.0f
	};
	
	unsigned int indices[] = { 0,1,2,2,3,0 };

	distVbo = new VertexBuffer();
	distVbo->SetData(vertices, sizeof(vertices));
	distVbo->SetLayout({
		{ShaderDataType::VEC3F, "position"},
		{ShaderDataType::VEC2F, "texCoords"},
	});
	distVao->AddVertexBuffer(*distVbo);

	distIbo = new IndexBuffer(indices, 6);
	distVao->SetIndexBuffer(*distIbo);

//#else
	grid.SetPos(0, 0, 0);
	grid.constant = 0;
	grid.axis = true;
//#endif

	dirLight = new DirectionalLight();
	goDirLight = app->sceneManager->GetCurrentScene()->CreateGameObject(0);
	goDirLight->SetName("Directional Light");

	TransformComponent* tr = goDirLight->GetComponent<TransformComponent>();
	tr->SetPosition({ 50,50,50 });
	ComponentLight* lightComp = (ComponentLight*)goDirLight->CreateComponent(ComponentType::LIGHT);
	lightComp->SetLight(dirLight);

	vbo = new VertexBuffer();
	
	genShadows = true;


	glGenFramebuffers(1, &shadowsFbo);
	glBindFramebuffer(GL_FRAMEBUFFER, shadowsFbo);
	glGenTextures(1, &shadowsDepthTexture);
	glBindTexture(GL_TEXTURE_2D, shadowsDepthTexture);
	glTexImage2D(GL_TEXTURE_2D, 0, GL_DEPTH_COMPONENT, 4096, 4096, 0, GL_DEPTH_COMPONENT, GL_UNSIGNED_BYTE, NULL);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glFramebufferTexture2D(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_TEXTURE_2D, shadowsDepthTexture, 0);

	dmgFeedbackRequested = false;
	
	return ret;
}

bool ModuleRenderer3D::Start()
{
	postProcessingShader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/Shaders/postProcessing.shader")));
	coneShader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/Shaders/basic.shader")));
	textureShader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/Shaders/texture.shader")));
	damageTexture = std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/damage.png")));
	whiteTexture = std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/white.png")));

	return true;
}

bool ModuleRenderer3D::PreUpdate(float dt)
{
#ifndef DIST

	//glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
	//// Editor Camera FBO
	//fbo->Bind();
	//PushCamera(app->camera->matrixProjectionFrustum, app->camera->matrixViewFrustum);
	//
	//glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
	//glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
	//
	//glMatrixMode(GL_PROJECTION);
	//glLoadMatrixf(app->camera->matrixProjectionFrustum.Transposed().ptr());
	//glMatrixMode(GL_MODELVIEW);
	//glLoadMatrixf(app->camera->matrixViewFrustum.Transposed().ptr());

#endif

	return true;
}

// PostUpdate present buffer to screen
bool ModuleRenderer3D::PostUpdate()
{
	RG_PROFILING_FUNCTION("Rendering");

	std::set<GameObject*> objects;

	// TODO: wtf quadtree man.
	app->sceneManager->GetCurrentScene()->GetQuadtree().Intersect(objects, app->sceneManager->GetCurrentScene()->mainCamera);
	
	AABB shadowsAABB;
#ifndef DIST

	PushCamera(app->camera->matrixProjectionFrustum, app->camera->matrixViewFrustum);

	if(drawGrid) grid.Render();

	if (rayCast)
	{
		math::LineSegment line = app->camera->rayCastToDraw.ToLineSegment(50.0f);
		glLineWidth(2.5f);
		glBegin(GL_LINES);
		glColor3f(1.0f, 0.64f, 0.0f);
		glVertex3f(line.a.x, line.a.y, line.a.z);
		glVertex3f(line.b.x, line.b.y, line.b.z);
		glColor3f(1.0f, 1.0f, 1.0f);

		glEnd();
		glLineWidth(1.0f);
	}
	PushCamera(float4x4::identity, float4x4::identity);


	// Shadow Pass ===================================
	
	if (dirLight->generateShadows)
	{
		GenerateShadows(objects, nullptr, shadowsAABB);
	}

	// Scene Pass ====================================
	fbo->Bind();	

	glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);

	GLenum drawBuffers[] = { GL_COLOR_ATTACHMENT0, GL_COLOR_ATTACHMENT1 };
	glDrawBuffers(2, drawBuffers);

	GameObject* objSelected = app->editor->GetGO();

	glEnable(GL_BLEND);
	if (app->camera->visualizeFrustum)
	{
		for (std::set<GameObject*>::iterator it = objects.begin(); it != objects.end(); ++it)
		{
			if ((*it) != objSelected)
				(*it)->Draw(nullptr);
		}
	}
	else app->sceneManager->GetCurrentScene()->Draw();
	// Scene Pass ====================================
	glDisable(GL_BLEND);

	for (auto& p : gosToDrawOutline)
	{
		glStencilFunc(GL_NOTEQUAL, 1, 0xFF);
		glStencilMask(0x00);
		glDisable(GL_DEPTH_TEST);
		p.first->DrawOutline(nullptr, p.second);

		glStencilMask(0xFF);
		glStencilFunc(GL_ALWAYS, 0, 0xFF);
		if (depthTest) glEnable(GL_DEPTH_TEST);
		//glColor3f(1.0f, 1.0f, 1.0f);
		p.first->Draw(nullptr);
	}

	DebugDraw(objSelected);
	
	//glClear(GL_DEPTH_BUFFER_BIT);
	fbo->Unbind();

#endif

	if (dirLight->generateShadows)
	{
		GenerateShadows(objects, app->sceneManager->GetCurrentScene()->mainCamera, shadowsAABB);
	}


	// Scene Pass ====================================
	mainCameraFbo->Bind();
	glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
#ifdef DIST
	GLenum drawBuffers[] = { GL_COLOR_ATTACHMENT0, GL_COLOR_ATTACHMENT1 };
#endif
	glDrawBuffers(2, drawBuffers);

	std::vector<GameObject*> gos;
	gos.resize(gosToDrawOutline.size());
	for (int i = 0; i < gosToDrawOutline.size(); ++i)
		gos.push_back(gosToDrawOutline[i].first);

	CameraComponent* cam = app->sceneManager->GetCurrentScene()->mainCamera;
	for (std::set<GameObject*>::iterator it = objects.begin(); it != objects.end(); ++it)
	{
		if (std::find(gos.begin(), gos.end(), *it) == gos.end())
			(*it)->Draw(cam);
	}

	for (auto& p : gosToDrawOutline)
	{
		glStencilFunc(GL_NOTEQUAL, 1, 0xFF);
		glStencilMask(0x00);
		glDisable(GL_DEPTH_TEST);
		p.first->DrawOutline(cam, p.second);

		glStencilMask(0xFF);
		glStencilFunc(GL_ALWAYS, 0, 0xFF);
		if (depthTest) glEnable(GL_DEPTH_TEST);
		p.first->Draw(cam);
	}


	vbo->SetData(enemyCones.data(), sizeof(float3) * enemyCones.size());
	vbo->SetLayout({ {ShaderDataType::VEC3F, "position"} });
	
	float rcolor = 0;
	float gcolor = 1;
	float bcolor = 0;

	glEnable(GL_BLEND);
	coneShader->Bind();
	coneShader->SetUniformMatrix4f("projection", cam->matrixProjectionFrustum.Transposed());
	coneShader->SetUniformMatrix4f("view", cam->matrixViewFrustum.Transposed());
	
	coneShader->SetUniform1f("rcolor", rcolor);
	coneShader->SetUniform1f("gcolor", gcolor);
	coneShader->SetUniform1f("bcolor", bcolor);
	
	vbo->Bind();
	glVertexPointer(3, GL_FLOAT, 0, NULL);
	glEnableClientState(GL_VERTEX_ARRAY);
	glDrawArrays(GL_TRIANGLES, 0, enemyCones.size());
	vbo->Unbind();

	coneShader->Unbind();
    enemyCones.clear();
	glDisable(GL_BLEND);

#ifndef DIST 
	app->userInterface->Draw();
#endif

	mainCameraFbo->Unbind();

#ifdef DIST
	
	// Inside each function there is a comprobation so it does not get resized each frame
	float2 size = { (float)*app->window->GetWindowWidth(), (float)*app->window->GetWindowHeight() };
	mainCameraFbo->ResizeFramebuffer(size.x, size.y);
	// OnResize gets called when an SDL event of window resize is triggered
	//OnResize(size.x, size.y);
	app->sceneManager->GetCurrentScene()->mainCamera->UpdateFovAndScreen(size.x, size.y);

	glDisable(GL_DEPTH_TEST);
	postProcessingShader->Bind();
	
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, mainCameraFbo->GetColorId());
	GLuint textLoc1 = glGetUniformLocation(postProcessingShader->GetId(), "colorTexture");
	glUniform1i(textLoc1, 0);

	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, mainCameraFbo->GetNormalId());
	GLuint textLoc2 = glGetUniformLocation(postProcessingShader->GetId(), "normalTexture");
	glUniform1i(textLoc2, 1);

	glActiveTexture(GL_TEXTURE2);
	glBindTexture(GL_TEXTURE_2D, mainCameraFbo->GetDepthId());
	GLuint textLoc3 = glGetUniformLocation(postProcessingShader->GetId(), "depthTexture");
	glUniform1i(textLoc3, 2);

	distVao->Bind();
	distIbo->Bind();

	glDrawElements(GL_TRIANGLES, distIbo->GetCount(), GL_UNSIGNED_INT, 0);
	
	postProcessingShader->Unbind();

	if (dmgFeedbackRequested)
	{
		DrawDamageFeedback();
	}

	distIbo->Unbind();
	distVao->Unbind();

#else
	app->editor->Draw(fbo, mainCameraFbo);
#endif

#ifdef DIST
	
	glEnable(GL_BLEND);
	app->userInterface->Draw();
	
	if (app->sceneManager->IsSceneChanging())
	{
		textureShader->Bind();
		textureShader->SetUniform1f("alpha", app->sceneManager->GetTransitionAlpha());
		textureShader->SetUniformVec3f("color", { 0,0,0 });

		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, whiteTexture->GetId());
		GLuint textLoc1 = glGetUniformLocation(whiteTexture->GetId(), "tex");
		glUniform1i(textLoc1, 0);

		distVao->Bind();
		distIbo->Bind();

		glDrawElements(GL_TRIANGLES, distIbo->GetCount(), GL_UNSIGNED_INT, 0);
		
		distVao->Unbind();
		distIbo->Unbind();
	}

	glDisable(GL_BLEND);
	glEnable(GL_DEPTH_TEST);
	glBindTexture(GL_TEXTURE_2D, 0);

#endif

	SDL_GL_SwapWindow(app->window->window);
	
	//glDisable(GL_BLEND);

	return true;
}

// Called before quitting
bool ModuleRenderer3D::CleanUp()
{
	DEBUG_LOG("Destroying 3D Renderer");

	RELEASE(fbo);
	RELEASE(mainCameraFbo);
	RELEASE(dirLight);
	//RELEASE(defaultShader);

	for(auto& pl : pointLights)
	{
		delete pl;
		pl = nullptr;
	}
	pointLights.clear();

	for (auto& pl : spotLights)
	{
		delete pl;
		pl = nullptr;
	}
	spotLights.clear();

#ifndef DIST
	ImGui_ImplOpenGL3_Shutdown();
	ImGui_ImplSDL2_Shutdown();
	ImGui::DestroyContext();
#else
	RELEASE(distVao);
	RELEASE(distVbo);
#endif
	SDL_GL_DeleteContext(context);

	return true;
}

bool ModuleRenderer3D::LoadConfig(JsonParsing& node)
{
	vsync = node.GetJsonBool("vsync");
	depthTest = node.GetJsonBool("depth test");
	cullFace = node.GetJsonBool("cull face");
	lighting = node.GetJsonBool("lighting");
	colorMaterial = node.GetJsonBool("color material");
	texture2D = node.GetJsonBool("texture 2D");
	stencil = node.GetJsonBool("stencil");
	blending = node.GetJsonBool("blending");
	wireMode = node.GetJsonBool("wire mode");
	navMesh = node.GetJsonBool("navmesh");
	drawGrid = node.GetJsonBool("draw grid");

	SetVsync(vsync);
	SetDepthTest();
	SetCullFace();
	SetLighting();
	SetColorMaterial();
	SetTexture2D();
	SetStencil();
	SetBlending();
	SetWireMode();

	return true;
}

bool ModuleRenderer3D::SaveConfig(JsonParsing& node)
{
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "vsync", vsync);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "depth test", depthTest);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "cull face", cullFace);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "lighting", lighting);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "color material", colorMaterial);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "texture 2D", texture2D);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "stencil", stencil);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "blending", blending);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "wire mode", wireMode);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "navmesh", navMesh);
	node.SetNewJsonBool(node.ValueToObject(node.GetRootValue()), "draw grid", drawGrid);

	return true;
}

void ModuleRenderer3D::OnResize(int width, int height)
{
	glViewport(0, 0, width, height);

	glMatrixMode(GL_PROJECTION);
	glLoadIdentity();
	projectionMatrix = Perspective(60.0f, (float)width / (float)height, 0.125f, 512.0f);
	glLoadMatrixf(&projectionMatrix);

	glMatrixMode(GL_MODELVIEW);
	glLoadIdentity();
}

void ModuleRenderer3D::SetDepthTest()
{
	if (depthTest) glEnable(GL_DEPTH_TEST);
	else glDisable(GL_DEPTH_TEST);
}

void ModuleRenderer3D::SetCullFace()
{
	if (cullFace) glEnable(GL_CULL_FACE);
	else glDisable(GL_CULL_FACE);
}

void ModuleRenderer3D::SetLighting()
{
	if (lighting) glEnable(GL_LIGHTING);
	else glDisable(GL_LIGHTING);
}

void ModuleRenderer3D::SetColorMaterial()
{
	if (colorMaterial) glEnable(GL_COLOR_MATERIAL);
	else glDisable(GL_COLOR_MATERIAL);
}

void ModuleRenderer3D::SetTexture2D()
{
	if (texture2D) glEnable(GL_TEXTURE_2D);
	else glDisable(GL_TEXTURE_2D);
}

void ModuleRenderer3D::SetStencil()
{
	if (stencil)
	{
		glEnable(GL_STENCIL_TEST);
		glStencilOp(GL_KEEP, GL_KEEP, GL_REPLACE);
	}
	else glDisable(GL_STENCIL_TEST);
}

void ModuleRenderer3D::SetBlending()
{
	if (blending) glEnable(GL_BLEND);
	else glDisable(GL_BLEND);
}

void ModuleRenderer3D::SetWireMode()
{
	if (wireMode) glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
	else glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
}

void ModuleRenderer3D::SetVsync(bool newValue)
{
	SDL_GL_SetSwapInterval(newValue);
}

void ModuleRenderer3D::DrawCubeDirectMode()
{
	GLfloat v0[3] = { 1.0f, 1.0f, 0.0f };
	GLfloat v1[3] = { 0.0f, 1.0f, 0.0f };
	GLfloat v2[3] = { 0.0f, 0.0f, 0.0f };
	GLfloat v3[3] = { 1.0f, 0.0f, 0.0f };
	GLfloat v4[3] = { 1.0f, 0.0f,-1.0f };
	GLfloat v5[3] = { 1.0f, 1.0f,-1.0f };
	GLfloat v6[3] = { 0.0f, 1.0f,-1.0f };
	GLfloat v7[3] = { 0.0f, 0.0f,-1.0f };

	glBegin(GL_TRIANGLES);

	// Front
	glVertex3fv(v0);
	glVertex3fv(v1);
	glVertex3fv(v2);

	glVertex3fv(v2);
	glVertex3fv(v3);
	glVertex3fv(v0);

	// Right
	glVertex3fv(v0);
	glVertex3fv(v3);
	glVertex3fv(v4);

	glVertex3fv(v4);
	glVertex3fv(v5);
	glVertex3fv(v0);

	// Top
	glVertex3fv(v0);
	glVertex3fv(v5);
	glVertex3fv(v6);

	glVertex3fv(v6);
	glVertex3fv(v1);
	glVertex3fv(v0);

	// Back
	glVertex3fv(v7);
	glVertex3fv(v6);
	glVertex3fv(v5);

	glVertex3fv(v5);
	glVertex3fv(v4);
	glVertex3fv(v7);

	// Left
	glVertex3fv(v7);
	glVertex3fv(v2);
	glVertex3fv(v1);

	glVertex3fv(v1);
	glVertex3fv(v6);
	glVertex3fv(v7);

	// Bottom
	glVertex3fv(v7);
	glVertex3fv(v4);
	glVertex3fv(v3);

	glVertex3fv(v3);
	glVertex3fv(v2);
	glVertex3fv(v7);

	glEnd();
}

Material* ModuleRenderer3D::GetDefaultMaterial()
{
	return defaultMaterial;
}

uint ModuleRenderer3D::GetDefaultShader()
{
	return defaultShader;
}

void ModuleRenderer3D::AddMaterial(Material* material)
{
	materials.emplace_back(material);
}

void ModuleRenderer3D::AddPointLight(PointLight* pl)
{
	pointLights.push_back(pl);
}

void ModuleRenderer3D::AddSpotLight(SpotLight* sl)
{
	spotLights.push_back(sl);
}

void ModuleRenderer3D::ClearPointLights()
{
	pointLights.clear();
}

void ModuleRenderer3D::ClearSpotLights()
{
	spotLights.clear();
}

void ModuleRenderer3D::RemovePointLight(PointLight* light)
{
	for (std::vector<PointLight*>::iterator it = pointLights.begin(); it != pointLights.end(); ++it)
	{
		if ((*it) == light)
		{
			light->intensity = 0;
			(*it)->toDelete = false;
			//delete (*it);
			//*it = nullptr;
			//delete* it;
			//delete light;
			//light = 0;
			//*it = 0;
			//pointLights.erase(it);
			break;
		}
	}
}

void ModuleRenderer3D::RemoveSpotLight(SpotLight* light)
{
	for (std::vector<SpotLight*>::iterator it = spotLights.begin(); it != spotLights.end(); ++it)
	{
		if ((*it) == light)
		{
			light->intensity = 0;
			(*it)->toDelete = false;
			//delete (*it);
			//*it = nullptr;
			//delete* it;
			//delete light;
			//light = 0;
			//*it = 0;
			//pointLights.erase(it);
			break;
		}
	}
}

void ModuleRenderer3D::RequestDamageFeedback()
{
	damageTextureAlpha = 0.6f;
	dmgFeedbackRequested = true;
}

void ModuleRenderer3D::PushCamera(const float4x4& proj, const float4x4& view)
{
	//glPushMatrix();
	glMatrixMode(GL_PROJECTION);
	glLoadMatrixf(proj.Transposed().ptr());

	//glPushMatrix();
	glMatrixMode(GL_MODELVIEW);
	glLoadMatrixf(view.Transposed().ptr());
}

void ModuleRenderer3D::DebugDraw(GameObject* objSelected)
{
	PushCamera(app->camera->matrixProjectionFrustum, app->camera->matrixViewFrustum);

	if (app->sceneManager->GetCurrentScene()->GetDebugDrawQuadtree())
		app->sceneManager->GetCurrentScene()->GetQuadtree().DebugDraw();

	if (navMesh && app->navMesh->GetNavMeshBuilder() != nullptr)
	{
		app->navMesh->GetNavMeshBuilder()->DebugDraw();

		if (objSelected)
		{
			NavAgentComponent* navMesh = navMesh = objSelected->GetComponent<NavAgentComponent>();
			if (navMesh) app->navMesh->GetPathfinding()->RenderPath(navMesh);
		}
	}

	if (app->physics->GetDebugMode())
		app->physics->DebugDraw();

	PushCamera(float4x4::identity, float4x4::identity);

	if (stencil && objSelected && objSelected->GetActive())
	{
		//glColor3f(0.25f, 0.87f, 0.81f);
		glStencilFunc(GL_NOTEQUAL, 1, 0xFF);
		glStencilMask(0x00);
		glDisable(GL_DEPTH_TEST);
		objSelected->DrawOutline(nullptr, { 0.25, 0.87, 0.81, });

		glStencilMask(0xFF);
		glStencilFunc(GL_ALWAYS, 0, 0xFF);
		if (depthTest) glEnable(GL_DEPTH_TEST);
		objSelected->Draw(nullptr);
	}
}

void ModuleRenderer3D::GenerateShadows(const std::set<GameObject*>& objects, CameraComponent* gameCam, AABB& shadAABB)
{
	glBindFramebuffer(GL_FRAMEBUFFER, shadowsFbo);
	glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT);
	glDrawBuffers(0, nullptr);

	GameObject* objSelected = app->editor->GetGO();

	glCullFace(GL_FRONT);
	genShadows = true;

	AABB shadowsAABB = shadAABB;
	shadowsAABB.SetNegativeInfinity();

	if (!gameCam)
	{
		for (std::set<GameObject*>::iterator it = objects.begin(); it != objects.end(); ++it)
		{
			OBB obb = (*it)->GetOOB();
			obb.Scale(obb.CenterPoint(), 2);
			shadowsAABB.Enclose(obb);
		}
		AABB camAABB = app->camera->cameraFrustum.MinimalEnclosingAABB();
		camAABB.Scale(camAABB.CenterPoint(), 2);
		AABB intersectionAABB = shadowsAABB.Intersection(camAABB);

		if (app->camera->visualizeFrustum)
		{
			for (std::set<GameObject*>::iterator it = objects.begin(); it != objects.end(); ++it)
			{
				if ((*it) != objSelected && intersectionAABB.Contains((*it)->GetAABB()))
					(*it)->Draw(nullptr);
			}
		}
		else app->sceneManager->GetCurrentScene()->Draw(&intersectionAABB);
	}
	else
	{		
		for (std::set<GameObject*>::iterator it = objects.begin(); it != objects.end(); ++it)
		{
			OBB obb = (*it)->GetOOB();
			obb.Scale(obb.CenterPoint(), 2);
			shadowsAABB.Enclose(obb);
		}
		
		AABB camAABB = gameCam->GetFrustum()->MinimalEnclosingAABB();
		camAABB.Scale(camAABB.CenterPoint(), 2.f);
		AABB intersectionAABB = shadowsAABB.Intersection(camAABB);

		for (std::set<GameObject*>::iterator it = objects.begin(); it != objects.end(); ++it)
		{
			if (intersectionAABB.Contains((*it)->GetAABB()))
				(*it)->Draw(gameCam);
		}
	}

	glBindFramebuffer(GL_FRAMEBUFFER, 0);
	glCullFace(GL_BACK);
	genShadows = false;
}

void ModuleRenderer3D::DrawDamageFeedback()
{
	damageTextureAlpha -= 0.5 * app->sceneManager->GetGameDeltaTime();
	if (damageTextureAlpha < 0)
	{
		damageTextureAlpha = 0.6;
		dmgFeedbackRequested = false;
		return;
	}

	textureShader->Bind();
	glActiveTexture(GL_TEXTURE0);
	glBindTexture(GL_TEXTURE_2D, damageTexture->GetId());
	GLuint textLoc1 = glGetUniformLocation(textureShader->GetId(), "tex");
	glUniform1i(textLoc1, 0);

	glEnable(GL_BLEND);
	textureShader->SetUniform1f("alpha", damageTextureAlpha);
	textureShader->SetUniformVec3f("color", { 1,1,1 });
	glDrawElements(GL_TRIANGLES, distIbo->GetCount(), GL_UNSIGNED_INT, 0);
	glDisable(GL_BLEND);

	glBindTexture(GL_TEXTURE_2D, 0);
	damageTexture->Unbind();
}
