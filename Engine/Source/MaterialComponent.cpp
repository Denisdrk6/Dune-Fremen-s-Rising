#include "MaterialComponent.h"
#include "Application.h"

#include "ModuleRenderer3D.h"
#include "ModuleCamera3D.h"
#include "ModuleEditor.h"
#include "ModuleSceneManager.h"
#include "Scene.h"
#include "GameView.h"

#include "GameObject.h"
#include "TransformComponent.h"
#include "MeshComponent.h"
#include "AnimationComponent.h"

#include "FileSystem.h"
#include "ResourceManager.h"
#include "Texture.h"
#include "Lights.h"
#include "Framebuffer.h"
#include "Globals.h"

#include "GL/glew.h"
#include <fstream>
#include "Profiling.h"

#define MAX_TIME_TO_REFRESH_SHADER 10

void PingPongFloat(float& value, float speed, float max, float min, bool ascendingFirst)
{
	static int dir = ascendingFirst ? 1 : -1;
	value += speed * dir;

	if (value > max)
	{
		value = max;
		dir = -1;
	}
	else if (value < min)
	{
		value = min;
		dir = 1;
	}
}

MaterialComponent::MaterialComponent(GameObject* own, bool defaultMat) : defaultMat(defaultMat)
{
	type = ComponentType::MATERIAL;
	owner = own;
	active = true;

	shader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/Shaders/default.shader")));
	shader->SetUniforms(GetShaderUniforms());
	diff = std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/white.png")));
	normalMap = std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/white.png")));
	emissive = std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/white.png")));

	outlineShader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/Shaders/outlineStencil.shader")));
	shadowShader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/Shaders/shadows.shader")));

	ambientColor = { 0.4,0.4,0.4 };
	diffuseColor = ambientColor;
	specularColor = { 0.5,0.5,0.5 };
	shininess = 5.0f;
	emissiveColor = float3::zero;
	emissiveEnabled = false;
	refreshShaderTimer = 0.0f;
}

MaterialComponent::MaterialComponent(MaterialComponent* mat)
{
	checker = mat->checker;
	diff = mat->diff;
	normalMap = mat->normalMap;

	shader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(std::string("Assets/Resources/Shaders/default.shader")));

	ambientColor = { 0.4,0.4,0.4 };
	diffuseColor = ambientColor;
	specularColor = { 0.5,0.5,0.5 };
	shininess = 5.0f;
}

MaterialComponent::~MaterialComponent()
{
	if (diff.use_count() - 1 == 1) diff->UnLoad();

	MeshComponent* mesh = owner->GetComponent<MeshComponent>();
	if (mesh)
	{
		mesh->SetMaterial(nullptr);
	}

	//RELEASE(shader);
}

void MaterialComponent::OnEditor()
{
	ImGui::PushID(this);

	if (ImGui::CollapsingHeader(ICON_FA_LAYER_GROUP" Material"))
	{
		Checkbox(this, "Active", active);
		ImGui::Checkbox("Cast Shadows", &owner->castShadows);
		ImGui::Checkbox("Is Interactuable", &owner->isInteractuable);
		if (owner->isInteractuable)
			ImGui::ColorEdit3("Interactuable Color", interColor.ptr());

		ImGui::DragFloat("Opacity", &opacity, 0.01, 0, 1);

		DisplayTexturesInfo();

		ImGui::Separator();

		ImGui::Text("Select shader: ");
		ImGui::SameLine();
		if (ImGui::Button(shader->GetName().c_str()))
		{
			showShaderMenu = true;
		}

		ImGui::Text("Reference Count: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d (Warning: There's already one instance of it on the resources map)", shader.use_count());

		ImGui::BulletText("Last time modified: %s", shader->GetLastModifiedDate());

		if (ImGui::Button("Edit Shader", { 100,25 }))	EditorShader();

		ImGui::Separator();

		ShowUniforms();

		if (showShaderEditor)
			ShaderEditor();

		if (showTexMenu)
			MenuTextureList();

		if (showShaderMenu)
			MenuShaderList();

		ComponentOptions(this);
		ImGui::Separator();
	}
	
	ImGui::PopID();
}

void MaterialComponent::MenuTextureList()
{
	ImGui::Begin("Textures", &showTexMenu);
	ImVec2 winPos = ImGui::GetWindowPos();
	ImVec2 size = ImGui::GetWindowSize();
	ImVec2 mouse = ImGui::GetIO().MousePos;
	if (!(mouse.x < winPos.x + size.x && mouse.x > winPos.x &&
		mouse.y < winPos.y + size.y && mouse.y > winPos.y))
	{
		if (ImGui::GetIO().MouseClicked[0]) showTexMenu = false;
	}

	std::vector<std::shared_ptr<Resource>> res;

	std::vector<std::string> files;
	app->fs->DiscoverFiles("Library/Textures/", files);
	for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
	{
		if ((*it).find(".rgtexture") != std::string::npos)
		{
			app->fs->GetFilenameWithoutExtension(*it);
			*it = (*it).substr((*it).find_last_of("_") + 1, (*it).length());
			uint uid = std::stoll(*it);
			res.push_back(ResourceManager::GetInstance()->GetResource(uid));
		}
	}

	std::sort(res.begin(), res.end(), [](std::shared_ptr<Resource> a, std::shared_ptr<Resource> b) {
		return (a.get()->GetName() < b.get()->GetName());
		});

	for (std::vector<std::shared_ptr<Resource>>::iterator it = res.begin(); it != res.end(); ++it)
	{
		if (ImGui::Selectable((*it).get()->GetName().c_str()))
		{
			(*it)->Load();

			if (textureTypeToChange == TextureType::DIFFUSE)
				if (diff.use_count() - 1 == 1) diff->UnLoad();
			else if(textureTypeToChange == TextureType::NORMAL)
				if (normalMap.use_count() - 1 == 1) normalMap->UnLoad();
			else if (textureTypeToChange == TextureType::EMISSIVE)
					if (normalMap.use_count() - 1 == 1) emissive->UnLoad();

			SetTexture((*it));
		}
	}

	ImGui::End();
}
void MaterialComponent::MenuShaderList()
{
	ImGui::Begin("Shaders", &showShaderMenu);
	ImVec2 winPos = ImGui::GetWindowPos();
	ImVec2 size = ImGui::GetWindowSize();
	ImVec2 mouse = ImGui::GetIO().MousePos;
	if (!(mouse.x < winPos.x + size.x && mouse.x > winPos.x &&
		mouse.y < winPos.y + size.y && mouse.y > winPos.y))
	{
		if (ImGui::GetIO().MouseClicked[0]) showShaderMenu = false;
	}

	std::vector<std::string> files;
	app->fs->DiscoverFiles("Assets/Resources/Shaders", files);

	std::sort(files.begin(), files.end(), [](std::string a, std::string b) {
		return (a < b);
		});

	for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
	{
		if ((*it).find(".shader") != std::string::npos)
		{
			if (ImGui::Selectable((*it).c_str()))
			{
				uint uid = ResourceManager::GetInstance()->CreateResource(ResourceType::SHADER, std::string("Assets/Resources/Shaders/" + *it), std::string());
				shader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(uid));
				shader->SetUniforms(GetShaderUniforms());
			}
		}
	}

	ImGui::End();
}
void MaterialComponent::ShaderEditor()
{
	ImGui::Begin("Shader Editor", &showShaderEditor, ImGuiWindowFlags_HorizontalScrollbar | ImGuiWindowFlags_MenuBar | ImGuiWindowFlags_NoCollapse);
	//Update
	auto cpos = editor.GetCursorPosition();
	if (ImGui::BeginMenuBar())
	{
		if (ImGui::Button("Save"))
		{
			std::string s = editor.GetText();

			app->fs->RemoveFile(fileToEdit.c_str());
			app->fs->Save(fileToEdit.c_str(), s.c_str(), editor.GetText().size());

			shader->Refresh();

		}
		ImGui::EndMenuBar();
	}

	ImGui::Text("%6d/%-6d %6d lines  | %s | %s | %s | %s", cpos.mLine + 1, cpos.mColumn + 1, editor.GetTotalLines(),
		editor.IsOverwrite() ? "Ovr" : "Ins",
		editor.CanUndo() ? "*" : " ",
		editor.GetLanguageDefinition().mName.c_str(), fileToEdit.c_str());

	editor.Render("TextEditor");
	ImGui::End();
}

std::vector<Uniform> MaterialComponent::GetShaderUniforms()
{
	GLint activeUniforms;
	glGetProgramiv(shader->GetId(), GL_ACTIVE_UNIFORMS, &activeUniforms);
	std::vector<Uniform> uniforms;

	for (uint i = 0; i < activeUniforms; i++)
	{
		Uniform uniform;
		GLint length;
		GLint size;
		GLchar name[32];
		glGetActiveUniform(shader->GetId(), i, sizeof(name), &length, &size, &uniform.GLtype, name);
		uniform.name = name;
		if (uniform.name != "inColor" && uniform.name != "time" && uniform.name != "modelMatrix" && uniform.name != "viewMatrix" && uniform.name != "projectionMatrix" && uniform.name != "finalBonesMatrices[0]")
		{
			uint uinformLoc = glGetUniformLocation(shader->GetId(), uniform.name.c_str());

			switch (uniform.GLtype)
			{
			case GL_INT:
				uniform.uniformType = UniformType::INT;
				glGetUniformiv(shader->GetId(), uinformLoc, &uniform.integer);
				break;
			case GL_FLOAT:
				uniform.uniformType = UniformType::FLOAT;
				glGetUniformfv(shader->GetId(), uinformLoc, &uniform.floatNumber);
				break;
			case GL_BOOL:
				uniform.uniformType = UniformType::BOOL;
				break;
			case GL_INT_VEC2:
				uniform.uniformType = UniformType::INT_VEC2;
				glGetUniformiv(shader->GetId(), uinformLoc, (GLint*)&uniform.vec2);
				break;
			case GL_INT_VEC3:
				uniform.uniformType = UniformType::INT_VEC3;
				glGetUniformiv(shader->GetId(), uinformLoc, (GLint*)&uniform.vec3);
				break;
			case GL_INT_VEC4:
				uniform.uniformType = UniformType::INT_VEC4;
				glGetUniformiv(shader->GetId(), uinformLoc, (GLint*)&uniform.vec4);
				break;
			case GL_FLOAT_VEC2:
				uniform.uniformType = UniformType::FLOAT_VEC2;
				glGetUniformfv(shader->GetId(), uinformLoc, (GLfloat*)&uniform.vec2);
				break;
			case GL_FLOAT_VEC3:
				uniform.uniformType = UniformType::FLOAT_VEC3;
				glGetUniformfv(shader->GetId(), uinformLoc, (GLfloat*)&uniform.vec3);
				break;
			case GL_FLOAT_VEC4:
				uniform.uniformType = UniformType::FLOAT_VEC4;
				glGetUniformfv(shader->GetId(), uinformLoc, (GLfloat*)&uniform.vec4);
				break;

			//case GL_FLOAT_MAT4:
			//	uniform.uniformType = UniformType::MATRIX4;
			//	glGetnUniformfv(shader->GetId(), uinformLoc, sizeof(uniform.matrix4), &uniform.matrix4.v[0][0]);
			//	break;

			default: uniform.uniformType = UniformType::NONE; 
				break;
			}

			if (uniform.uniformType != UniformType::NONE) 
				uniforms.push_back(uniform);
		}
	}
	return uniforms;
}

void MaterialComponent::ShowUniforms()
{
	for (uint i = 0; i < shader->GetUniforms().size(); i++)
	{
		switch (shader->GetUniforms()[i].uniformType)
		{
		case  UniformType::INT:	
			ImGui::DragInt(shader->GetUniforms()[i].name.c_str(), &(int)shader->GetUniforms()[i].integer, 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		case  UniformType::FLOAT: 
			ImGui::DragFloat(shader->GetUniforms()[i].name.c_str(), &(float)shader->GetUniforms()[i].floatNumber, 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		case  UniformType::INT_VEC2: 
			ImGui::DragInt2(shader->GetUniforms()[i].name.c_str(), (int*)&shader->GetUniforms()[i].vec2, 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		case  UniformType::INT_VEC3: 
			ImGui::DragInt3(shader->GetUniforms()[i].name.c_str(), (int*)&shader->GetUniforms()[i].vec3, 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		case  UniformType::INT_VEC4: 
			ImGui::DragInt4(shader->GetUniforms()[i].name.c_str(), (int*)&shader->GetUniforms()[i].vec4, 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		case  UniformType::FLOAT_VEC2: 
			ImGui::DragFloat2(shader->GetUniforms()[i].name.c_str(), (float*)&shader->GetUniforms()[i].vec2, 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		case  UniformType::FLOAT_VEC3: 
			ImGui::DragFloat3(shader->GetUniforms()[i].name.c_str(), (float*)&shader->GetUniforms()[i].vec3, 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		case  UniformType::FLOAT_VEC4: 
			ImGui::DragFloat4(shader->GetUniforms()[i].name.c_str(), (float*)&shader->GetUniforms()[i].vec4, 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		case UniformType::MATRIX4: 
			ImGui::DragFloat4(shader->GetUniforms()[i].name.c_str(), shader->GetUniforms()[i].matrix4.ToEulerXYZ().ptr(), 0.02f, 0.0f, 0.0f, "%.2f", ImGuiSliderFlags_None); 
			break;
		}	
	}
}

bool MaterialComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Material Update");

	if (refreshShaderTimer <= MAX_TIME_TO_REFRESH_SHADER)
		refreshShaderTimer += dt;
	else
	{
		refreshShaderTimer = 0.0f;
		shader->Refresh();
	}
	return true;
}

bool MaterialComponent::OnLoad(JsonParsing& node)
{
	diff = std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->LoadResource(std::string(node.GetJsonString("Diffuse Path"))));
	normalMap = std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->LoadResource(std::string(node.GetJsonString("Normal Map Path"))));
	active = node.GetJsonBool("Active");
	shader = std::static_pointer_cast<Shader>(ResourceManager::GetInstance()->LoadResource(std::string(node.GetJsonString("Shader Assets Path"))));
	owner->castShadows = node.GetJsonBool("Cast Shadows");
	interColor = node.GetJson3Number(node, "Interactuable Color");
	opacity = node.GetJsonNumber("Opacity");
	emissiveEnabled = node.GetJsonBool("Emissive Enabled");
	emissiveColor = node.GetJson3Number(node, "Emissive Color");

	return true;
}

bool MaterialComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Diffuse Path", diff->GetAssetsPath().c_str());
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Normal Map Path", normalMap->GetAssetsPath().c_str());
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Active", active);
	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Shader Assets Path", shader->GetAssetsPath().c_str());
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Cast Shadows", owner->castShadows);
	file.SetNewJson3Number(file, "Interactuable Color", interColor);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Opacity", opacity);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Emissive Enabled", emissiveEnabled);
	file.SetNewJson3Number(file, "Emissive Color", emissiveColor);

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

void MaterialComponent::Bind(CameraComponent* gameCam)
{
	// Crash when creating a primitive
	if (!this)
		return;

	float4x4 model = owner->GetComponent<TransformComponent>()->GetGlobalTransform();
	
	if (owner->castShadows && app->renderer3D->genShadows)
	{
		glViewport(0, 0, 4096, 4096);
		shadowShader->Bind();
		shadowShader->SetUniformMatrix4f("model", model.Transposed());
		shadowShader->SetUniformMatrix4f("lightSpaceMatrix", app->renderer3D->dirLight->lightSpace.Transposed());

		if (AnimationComponent* anim = owner->GetComponent<AnimationComponent>())
		{
			auto transforms = anim->GetFinalBoneMatrices();
			for (int i = 0; i < transforms.size(); ++i)
				shadowShader->SetUniformMatrix4f("finalBonesMatrices[" + std::to_string(i) + "]", transforms[i].Transposed());
		}

		return;
	}

	if (diff)
		diff->Bind();

	shader->Bind();

	shader->SetUniformMatrix4f("model", model.Transposed());
	shader->SetUniformMatrix4f("lightSpaceMatrix", app->renderer3D->dirLight->lightSpace.Transposed());

	float4x4 view = float4x4::identity;
	float4x4 proj = float4x4::identity;
	float3 camPos = float3::zero;
	Framebuffer* fbo = nullptr;
	float intColSpeed = 0.f;
	if (gameCam)
	{
		view = gameCam->matrixViewFrustum;
		proj = gameCam->matrixProjectionFrustum;
		fbo = app->renderer3D->mainCameraFbo;
		camPos = gameCam->GetFrustum()->pos;
		intColSpeed = app->sceneManager->GetGameDeltaTime();
	}
	else
	{
		view = app->camera->matrixViewFrustum;
		proj = app->camera->matrixProjectionFrustum;
		fbo = app->renderer3D->fbo;
		camPos = app->camera->cameraFrustum.pos;
		intColSpeed = app->GetEngineDeltaTime();
	}

	shader->SetUniformMatrix4f("view", view.Transposed());
	shader->SetUniformMatrix4f("projection", proj.Transposed());
	shader->SetUniformVec3f("camPos", camPos);
	//float4x4 normalMat = view;
	//normalMat.Inverse();
	//shader->SetUniformMatrix3f("normalMatrix", normalMat.Float3x3Part().Transposed());

	if (std::string(owner->GetName()).find("Player") != std::string::npos)
		shader->SetUniform1f("normalsThickness", 0);
	else if (std::string(owner->GetName()).find("Enemy") != std::string::npos)
		shader->SetUniform1f("normalsThickness", 0);
	else
		shader->SetUniform1f("normalsThickness", 1);

	shader->SetUniform1i("isInteractuable", owner->isInteractuable);
	if (owner->isInteractuable)
	{
		shader->SetUniformVec3f("interCol", interColor);
		static float intensity = 0.1;
		PingPongFloat(intensity, 0.05 * intColSpeed, 1, 0, true);

		shader->SetUniform1f("interColIntensity", intensity);
	}

	glActiveTexture(GL_TEXTURE1);
	glBindTexture(GL_TEXTURE_2D, app->renderer3D->shadowsDepthTexture);
	GLuint texLoc = glGetUniformLocation(shader->GetId(), "depthTexture");
	glUniform1i(texLoc, 1);
	
	if (normalMap != std::static_pointer_cast<Texture>(ResourceManager::GetInstance()->GetResource("Assets/Resources/white.png")))
	{
		shader->SetUniform1i("hasNormalMap", 1);
		glActiveTexture(GL_TEXTURE2);
		glBindTexture(GL_TEXTURE_2D, normalMap->GetId());
		texLoc = glGetUniformLocation(shader->GetId(), "normalMap");
		glUniform1i(texLoc, 2);
	}
	else
		shader->SetUniform1i("hasNormalMap", 0);	

	
	glActiveTexture(GL_TEXTURE3);
	glBindTexture(GL_TEXTURE_2D, emissive->GetId());
	texLoc = glGetUniformLocation(shader->GetId(), "emissiveTexture");
	glUniform1i(texLoc, 3);
	shader->SetUniform1i("emissiveEnabled", emissiveEnabled);
	shader->SetUniformVec3f("material.emissiveColor", emissiveColor);

	if (AnimationComponent* anim = owner->GetComponent<AnimationComponent>())
	{
		auto transforms = anim->GetFinalBoneMatrices();
		for (int i = 0; i < transforms.size(); ++i)
			shader->SetUniformMatrix4f("finalBonesMatrices[" + std::to_string(i) + "]", transforms[i].Transposed());
	}

	ShaderSetUniforms();
}

void MaterialComponent::ShaderSetUniforms()
{
	shader->SetUniformVec3f("material.ambient", ambientColor);
	shader->SetUniformVec3f("material.diffuse", diffuseColor);
	shader->SetUniformVec3f("material.specular", specularColor);
	shader->SetUniform1f("material.shininess", shininess);
	shader->SetUniform1f("material.gammaCorrection", 0);
	shader->SetUniform1f("opacity", opacity);

	if (app->renderer3D->goDirLight)
	{
		shader->SetUniformVec3f("dirLight.direction", app->renderer3D->dirLight->dir);
		shader->SetUniformVec3f("dirLight.ambient", app->renderer3D->dirLight->ambient);
		shader->SetUniformVec3f("dirLight.diffuse", app->renderer3D->dirLight->diffuse);
		shader->SetUniformVec3f("dirLight.specular", app->renderer3D->dirLight->specular);
		shader->SetUniform1f("dirLight.intensity", app->renderer3D->dirLight->intensity);
		shader->SetUniform1i("dirLight.genShadows", app->renderer3D->dirLight->generateShadows);
	}
	else
	{
		shader->SetUniformVec3f("dirLight.ambient", float3::zero);
		shader->SetUniformVec3f("dirLight.diffuse", float3::zero);
		shader->SetUniformVec3f("dirLight.specular", float3::zero);
	}

	std::vector<PointLight*>& pls = app->renderer3D->GetPointLights();
	for (int i = 0; i < pls.size(); ++i)
	{
		//float intensity = 0;
		//if (pls[i])
		//	intensity = pls[i]->intensity;

		std::string name = "pointLights[" + std::to_string(i) + "]";
		shader->SetUniformVec3f(name + ".position", pls[i]->position);

		shader->SetUniform1f(name + ".intensity", pls[i]->intensity);
		shader->SetUniform1f(name + ".constant", pls[i]->constant);
		shader->SetUniform1f(name + ".lin", pls[i]->lin);
		shader->SetUniform1f(name + ".quadratic", pls[i]->quadratic);

		float3 props = { pls[i]->constant, pls[i]->lin, pls[i]->quadratic };
		shader->SetUniformVec3f(name + ".properties", props);

		shader->SetUniformVec3f(name + ".ambient", pls[i]->ambient);
		shader->SetUniformVec3f(name + ".diffuse", pls[i]->diffuse);
		shader->SetUniformVec3f(name + ".specular", pls[i]->specular);

		if (pls[i]->toDelete)
		{
			delete pls[i];
			pls[i] = nullptr;
			pls.erase(pls.begin() + i);
		}
	}

	std::vector<SpotLight*> sls = app->renderer3D->GetSpotLights();
	for (int i = 0; i < sls.size(); ++i)
	{
		std::string name = "spotLights[" + std::to_string(i) + "]";
		shader->SetUniformVec3f(name + ".position", sls[i]->position);
		shader->SetUniformVec3f(name + ".direction", sls[i]->direction);

		shader->SetUniform1f(name + ".intensity", sls[i]->intensity);
		shader->SetUniform1f(name + ".cutOff", (sls[i]->cutOff));
		shader->SetUniform1f(name + ".outerCutOff", (sls[i]->outerCutOff));

		shader->SetUniformVec3f(name + ".ambient", sls[i]->ambient);
		shader->SetUniformVec3f(name + ".diffuse", sls[i]->diffuse);
		shader->SetUniformVec3f(name + ".specular", sls[i]->specular);
	}
}

void MaterialComponent::Unbind()
{
	// Crash when creating a primitive
	if (!this) return;
	if (diff) diff->Unbind();

	app->renderer3D->genShadows ? shadowShader->Unbind() : shader->Unbind();
}

void MaterialComponent::SetTexture(std::shared_ptr<Resource> tex)
{
	if (textureTypeToChange == TextureType::DIFFUSE)
		diff = std::static_pointer_cast<Texture>(tex);
	else if (textureTypeToChange == TextureType::NORMAL)
		normalMap = std::static_pointer_cast<Texture>(tex);
	else if(textureTypeToChange == TextureType::EMISSIVE)
		emissive = std::static_pointer_cast<Texture>(tex);
	else
		diff = std::static_pointer_cast<Texture>(tex);
}

void MaterialComponent::EditorShader()
{
	TextEditor::LanguageDefinition lang = TextEditor::LanguageDefinition::GLSL();

	fileToEdit = shader->GetAssetsPath();
	editor.SetShowWhitespaces(false);

	std::ifstream text(fileToEdit.c_str());
	if (text.good())
	{
		std::string str((std::istreambuf_iterator<char>(text)), std::istreambuf_iterator<char>());
		editor.SetText(str);
	}

	showShaderEditor = true;

	shadertoRecompily = shader;
}

void MaterialComponent::DisplayTexturesInfo()
{
	ImGui::PushID(diff->GetUID());
	if (diff != nullptr)
	{
		ImGui::Text("Select Diffuse texture: ");
		ImGui::SameLine();
		if (ImGui::Button(diff ? diff->GetName().c_str() : ""))
		{
			showTexMenu = true;
			textureTypeToChange = TextureType::DIFFUSE;

		}

		ImGui::Image((ImTextureID)diff->GetId(), ImVec2(128, 128));
		ImGui::Indent();
		if (ImGui::CollapsingHeader("Info##Diff"))
		{
			ImGui::Text("Path: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%s", diff->GetAssetsPath().c_str());
			ImGui::Text("Width: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", diff->GetWidth());
			ImGui::Text("Height: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", diff->GetHeight());
			ImGui::Text("Reference Count: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d (Warning: There's already one instance of it on the resources map)", diff.use_count());
		}
		ImGui::Unindent();
	}
	else
	{
		ImGui::Text("Select Diffuse texture: ");
		ImGui::SameLine();
		if (ImGui::Button("No Texture"))
		{
			showTexMenu = true;
			textureTypeToChange = TextureType::DIFFUSE;
		}
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "There's no texture");
		ImGui::Text("Width: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", 0);
		ImGui::Text("Height: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", 0);
	}
	ImGui::PopID();

	ImGui::Separator();
	
	ImGui::PushID(normalMap->GetUID());
	if (normalMap != nullptr)
	{
		ImGui::Text("Select Normal texture: ");
		ImGui::SameLine();
		if (ImGui::Button(((normalMap ? normalMap->GetName() : "") + "##Foo").c_str()))
		{
			showTexMenu = true;
			textureTypeToChange = TextureType::NORMAL;
		}

		ImGui::Image((ImTextureID)normalMap->GetId(), ImVec2(128, 128));

		ImGui::Indent();
		if (ImGui::CollapsingHeader("Info##Normal"))
		{
			ImGui::Text("Path: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%s", normalMap->GetAssetsPath().c_str());
			ImGui::Text("Width: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", normalMap->GetWidth());
			ImGui::Text("Height: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", normalMap->GetHeight());
			ImGui::Text("Reference Count: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d (Warning: There's already one instance of it on the resources map)", normalMap.use_count());
		}
		ImGui::Unindent();
	}
	else
	{
		ImGui::Text("Select Normal texture: ");
		ImGui::SameLine();
		if (ImGui::Button("No Texture"))
		{
			showTexMenu = true;
			textureTypeToChange = TextureType::NORMAL;
		}
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "There's no texture");
		ImGui::Text("Width: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", 0);
		ImGui::Text("Height: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", 0);
	}
	
	ImGui::PopID();

	ImGui::Separator();

	ImGui::Checkbox("Emissive", &emissiveEnabled);

	ImGui::PushID(emissive->GetUID());
	if (emissiveEnabled)
	{
		if (emissive != nullptr)
		{
			ImGui::Text("Select Emissive texture: ");
			ImGui::SameLine();
			if (ImGui::Button(((emissive ? emissive->GetName() : "") + "##Foo2").c_str()))
			{
				showTexMenu = true;
				textureTypeToChange = TextureType::EMISSIVE;
			}

			ImGui::Image((ImTextureID)emissive->GetId(), ImVec2(128, 128));
			ImGui::ColorEdit3("Emissive Color", emissiveColor.ptr());

			ImGui::Indent();
			if (ImGui::CollapsingHeader("Info##Emissive"))
			{
				ImGui::Text("Path: ");
				ImGui::SameLine();
				ImGui::TextColored(ImVec4(1, 1, 0, 1), "%s", emissive->GetAssetsPath().c_str());
				ImGui::Text("Width: ");
				ImGui::SameLine();
				ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", emissive->GetWidth());
				ImGui::Text("Height: ");
				ImGui::SameLine();
				ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", emissive->GetHeight());
				ImGui::Text("Reference Count: ");
				ImGui::SameLine();
				ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d (Warning: There's already one instance of it on the resources map)", emissive.use_count());
			}
			ImGui::Unindent();

		}
		else
		{
			ImGui::Text("Select Emissive texture: ");
			ImGui::SameLine();
			if (ImGui::Button("No Texture"))
			{
				showTexMenu = true;
				textureTypeToChange = TextureType::EMISSIVE;
			}
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "There's no texture");
			ImGui::Text("Width: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", 0);
			ImGui::Text("Height: ");
			ImGui::SameLine();
			ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d", 0);
		}
	}
	ImGui::PopID();
}
