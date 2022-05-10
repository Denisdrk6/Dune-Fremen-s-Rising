#include "MonoManager.h"
#include "Globals.h"
#include "Application.h"

#include "FileSystem.h"
#include "GameObject.h"
#include "Component.h"
#include "TransformComponent.h"

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/mono-config.h>
#include <mono/metadata/debug-helpers.h>

#include "ScriptBindings.h"
#include "AudioBindings.h"
#include "InputActionBindings.h"
#include "RigidbodyBindings.h"
#include "AnimationBindings.h"
#include "CameraBindings.h"
#include "UIBindings.h"
#include "NavAgentBindings.h"

#include <fstream>
#include <PugiXML/pugixml.hpp>

MonoManager::MonoManager(bool start_enabled) : Module(start_enabled), domain(nullptr), domainThread(nullptr), assembly(nullptr), image(nullptr)
, jitDomain(nullptr)
{

}

MonoManager::~MonoManager()
{}

bool MonoManager::Init(JsonParsing& node)
{
	LOG(LogType::L_NORMAL, "Setting up the camera");
	bool ret = true;

//#ifndef STANDALONE
	CMDCompileCS();
//#endif // !STANDALONE

	mono_set_dirs("mono-runtime/lib", "mono-runtime/etc");
	mono_config_parse(NULL);
	jitDomain = mono_jit_init("myapp");

	// Input =====================
	mono_add_internal_call("RagnarEngine.Input::GetKey", GetKey);
	mono_add_internal_call("RagnarEngine.Input::GetMouseClick", GetMouseClick);
	mono_add_internal_call("RagnarEngine.Input::GetMouseX", MouseX);
	mono_add_internal_call("RagnarEngine.Input::GetMouseY", MouseY);
	// Input =====================

	// Transform =================
	mono_add_internal_call("RagnarEngine.Transform::get_localPosition", GetPosition);
	mono_add_internal_call("RagnarEngine.Transform::get_globalPosition", GetGlobalPosition);
	mono_add_internal_call("RagnarEngine.Transform::get_localRotation", GetRotation);
	mono_add_internal_call("RagnarEngine.Transform::get_globalRotation", GetGlobalRotation);
	mono_add_internal_call("RagnarEngine.Transform::set_globalRotation", SetGlobalRotation);
	mono_add_internal_call("RagnarEngine.Transform::get_scale", GetScale);
	mono_add_internal_call("RagnarEngine.Transform::get_up", GetUp);
	mono_add_internal_call("RagnarEngine.Transform::get_right", GetRight);
	mono_add_internal_call("RagnarEngine.Transform::get_forward", GetForward);
	mono_add_internal_call("RagnarEngine.Transform::GetAngleBetween", GetAngleBetween);
	mono_add_internal_call("RagnarEngine.Transform::GetDistanceBetween", GetDistanceBetween);
	mono_add_internal_call("RagnarEngine.Transform::RotateY", RotateY);
	
	mono_add_internal_call("RagnarEngine.Transform::set_localPosition", SetPosition);
	mono_add_internal_call("RagnarEngine.Transform::set_globalPosition", SetGlobalPosition);
	mono_add_internal_call("RagnarEngine.Transform::set_localRotation", SetRotation);
	mono_add_internal_call("RagnarEngine.Transform::set_scale", SetScale);
	// Transform =================

	// Material Comp =============
	//mono_add_internal_call("RagnarEngine.Material::get_texture", GetTexturePath);
	mono_add_internal_call("RagnarEngine.Material::SetTexturePath", SetTexturePath);
	mono_add_internal_call("RagnarEngine.Material::get_diffuseAlpha", GetDiffuseAlpha);
	mono_add_internal_call("RagnarEngine.Material::set_diffuseAlpha", SetDiffuseAlpha);
	mono_add_internal_call("RagnarEngine.Material::get_emissiveEnabled", GetEmissiveEnabled);
	mono_add_internal_call("RagnarEngine.Material::set_emissiveEnabled", SetEmissiveEnabled);
	mono_add_internal_call("RagnarEngine.Material::get_emissiveColor", GetEmissiveColor);
	mono_add_internal_call("RagnarEngine.Material::set_emissiveColor", SetEmissiveColor);

	// Material Comp =============
	
	// Internall Calls =============
	mono_add_internal_call("RagnarEngine.RagnarComponent::get_gameObject", GetGameObjectMonoObject);

	mono_add_internal_call("RagnarEngine.InternalCalls::CreateGameObject", InstantiateGameObject);
	mono_add_internal_call("RagnarEngine.InternalCalls::Create3DObject", Instantiate3DObject);     
	mono_add_internal_call("RagnarEngine.InternalCalls::Create3DObject", Instantiate3DGameObject);
	mono_add_internal_call("RagnarEngine.InternalCalls::InstancePrefab", InstancePrefab);
	mono_add_internal_call("RagnarEngine.InternalCalls::Destroy", Destroy);
	mono_add_internal_call("RagnarEngine.InternalCalls::GetRegionGame", GetRegionGame);
	mono_add_internal_call("RagnarEngine.InternalCalls::RequestDamageFeedback", RequestDamageFeedback);

	mono_add_internal_call("RagnarEngine.GameObject::TryGetComponent", TryGetComponentMono);
	mono_add_internal_call("RagnarEngine.GameObject::TryGetComponents", TryGetComponentsMono);
	mono_add_internal_call("RagnarEngine.GameObject::AddComponent", AddComponentMono);
	mono_add_internal_call("RagnarEngine.GameObject::DeleteComponent", DeleteComponentMono);

	mono_add_internal_call("RagnarEngine.RayCast::HitToTag", HitToTag);
	mono_add_internal_call("RagnarEngine.RayCast::PerceptionCone", PerceptionCone);
	mono_add_internal_call("RagnarEngine.RayCast::ReturnHitpoint", ReturnHitpoint);
	// Internal Calls =============

	// Utility ===================
	mono_add_internal_call("RagnarEngine.Time::get_deltaTime", GetGameTimeStep);
	mono_add_internal_call("RagnarEngine.Time::set_timeScale", SetTimeScale);
	mono_add_internal_call("RagnarEngine.Debug::Log", LogMono);
	mono_add_internal_call("RagnarEngine.GameObject::Find", FindGameObjectWithName);
	mono_add_internal_call("RagnarEngine.GameObject::FindGameObjectsWithTag", FindGameObjectsWithTag);
	mono_add_internal_call("RagnarEngine.GameObject::get_tag", GetGameObjectTagMono);
	mono_add_internal_call("RagnarEngine.GameObject::set_tag", SetGameObjectTagMono);
	mono_add_internal_call("RagnarEngine.GameObject::get_name", GetGameObjectName);
	mono_add_internal_call("RagnarEngine.GameObject::set_name", SetGameObjectName);
	mono_add_internal_call("RagnarEngine.GameObject::get_childs", GetGameObjectChilds);
	mono_add_internal_call("RagnarEngine.GameObject::get_isActive", GetGameObjectIsActive);
	mono_add_internal_call("RagnarEngine.GameObject::set_isActive", SetGameObjectIsActive);
	mono_add_internal_call("RagnarEngine.GameObject::GetActiveComponent", GetActiveComponent);
	mono_add_internal_call("RagnarEngine.GameObject::SetActiveComponent", SetActiveComponent);
	mono_add_internal_call("RagnarEngine.GameObject::GetSizeAABB", GetSizeAABB);
	mono_add_internal_call("RagnarEngine.GameObject::GetMinAABB", GetMinAABB);
	mono_add_internal_call("RagnarEngine.GameObject::GetMaxAABB", GetMaxAABB);
	mono_add_internal_call("RagnarEngine.GameObject::SetSizeAABB", SetSizeAABB);
	mono_add_internal_call("RagnarEngine.GameObject::ReparentToRoot", ReparentToRoot);
	mono_add_internal_call("RagnarEngine.GameObject::ChangeMesh", ChangeMesh);
	mono_add_internal_call("RagnarEngine.GameObject::AddChild", AddChild);
	mono_add_internal_call("RagnarEngine.GameObject::EraseChild", EraseChild);
	mono_add_internal_call("RagnarEngine.GameObject::GetParent", GetItsParent);
	mono_add_internal_call("RagnarEngine.GameObject::SubmitOutlineDrawing", GameObjectDrawOutline);
	mono_add_internal_call("RagnarEngine.GameObject::get_isInteractuable", GetGameObjectIsInteractuable);
	mono_add_internal_call("RagnarEngine.GameObject::set_isInteractuable", SetGameObjectIsInteractuable);
	mono_add_internal_call("RagnarEngine.GameObject::get_interactuableColor", GetGameObjectInteractuableColor);
	mono_add_internal_call("RagnarEngine.GameObject::set_interactuableColor", SetGameObjectInteractuableColor);

	// Utility ===================


	// Audio Source ==============
	mono_add_internal_call("RagnarEngine.AudioSource::PlayClip", PlayClip);
	mono_add_internal_call("RagnarEngine.AudioSource::StopCurrentClip", StopCurrentClip);
	mono_add_internal_call("RagnarEngine.AudioSource::SetClipVolume", SetClipVolume);
	mono_add_internal_call("RagnarEngine.AudioSource::SetState", SetState);
	mono_add_internal_call("RagnarEngine.AudioSource::GetClipVolume", GetClipVolume);
	mono_add_internal_call("RagnarEngine.AudioListener::TestListener", TestListener);
	// Audio Source ==============

	// Rigidbody =================
	mono_add_internal_call("RagnarEngine.Rigidbody::ApplyCentralForce", ApplyCentralForce);
	mono_add_internal_call("RagnarEngine.Rigidbody::ApplyCentralImpulse", ApplyCentralImpulse);
	mono_add_internal_call("RagnarEngine.Rigidbody::ApplyTorque", ApplyTorque);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetIgnoreCollision", SetIgnoreCollision);
	mono_add_internal_call("RagnarEngine.Rigidbody::set_linearVelocity", SetLinearVelocity);
	mono_add_internal_call("RagnarEngine.Rigidbody::get_linearVelocity", GetLinearVelocity);
	mono_add_internal_call("RagnarEngine.Rigidbody::get_totalForce", GetTotalForce);
	mono_add_internal_call("RagnarEngine.Rigidbody::ClearForces", ClearForces);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetAsStatic", SetAsStatic);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetAsTrigger", SetAsTrigger);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetCollisionType", SetCollisionType);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetCollisionSphere", SetCollisionSphere);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetHeight", SetHeight);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetBodyPosition", SetBodyPosition);
	mono_add_internal_call("RagnarEngine.Rigidbody::GetBodyPosition", GetBodyPosition);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetBodyRotation", SetBodyRotation);
	mono_add_internal_call("RagnarEngine.Rigidbody::GetBodyRotation", GetBodyRotation);
	mono_add_internal_call("RagnarEngine.Rigidbody::ApplyVelocity", ApplyVelocity);
	mono_add_internal_call("RagnarEngine.Rigidbody::SetRadiusSphere", SetRadiusSphere);
	// Rigidbody =================

	// Animation =================
	mono_add_internal_call("RagnarEngine.Animation::PlayAnimation", PlayAnimation);
	mono_add_internal_call("RagnarEngine.Animation::HasFinished", HasFinished);
	mono_add_internal_call("RagnarEngine.Animation::GetDuration", GetDuration);
	mono_add_internal_call("RagnarEngine.Animation::GetLoopTime", GetLoopTime);
	// Animation =================

	// Light =====================
	mono_add_internal_call("RagnarEngine.Light::get_shadowsEnabled", GetDirectionalLightShadowsEnabled);
	mono_add_internal_call("RagnarEngine.Light::set_shadowsEnabled", SetDirectionalLightShadowsEnabled);
	mono_add_internal_call("RagnarEngine.Light::get_intensity", GetLightIntensity);
	mono_add_internal_call("RagnarEngine.Light::set_intensity", SetLightIntensity);
	mono_add_internal_call("RagnarEngine.Light::get_linear", GetLightLinear);
	mono_add_internal_call("RagnarEngine.Light::set_linear", SetLightLinear);
	mono_add_internal_call("RagnarEngine.Light::get_quadratic", GetLightQuadratic);
	mono_add_internal_call("RagnarEngine.Light::set_quadratic", SetLightQuadratic);
	mono_add_internal_call("RagnarEngine.Light::get_constant", GetLightConstant);
	mono_add_internal_call("RagnarEngine.Light::set_constant", SetLightConstant);
	mono_add_internal_call("RagnarEngine.Light::get_ambient", GetLightAmbient);
	mono_add_internal_call("RagnarEngine.Light::set_ambient", SetLightAmbient);
	mono_add_internal_call("RagnarEngine.Light::get_diffuse", GetLightDiffuse);
	mono_add_internal_call("RagnarEngine.Light::set_diffuse", SetLightDiffuse);
	// Light =====================

	// NavAgent ==================
	mono_add_internal_call("RagnarEngine.NavAgent::CalculatePath", CalculateAgentPath);
	mono_add_internal_call("RagnarEngine.NavAgent::ValidDestination", ValidDestination);
	mono_add_internal_call("RagnarEngine.NavAgent::get_hitPosition", GetHitPosition);
	mono_add_internal_call("RagnarEngine.NavAgent::set_hitPosition", SetHitPosition);
	mono_add_internal_call("RagnarEngine.NavAgent::get_rayCastA", GetRayCastA);
	mono_add_internal_call("RagnarEngine.NavAgent::get_rayCastB", GetRayCastB);
	mono_add_internal_call("RagnarEngine.NavAgent::ClearPath", ClearPath);
	mono_add_internal_call("RagnarEngine.NavAgent::PathSize", PathSize);
	mono_add_internal_call("RagnarEngine.NavAgent::MovePath", MoveAgentPath);
	mono_add_internal_call("RagnarEngine.NavAgent::MoveTo", MoveAgentTo);
	mono_add_internal_call("RagnarEngine.NavAgent::set_path", SetAgentPath);
	mono_add_internal_call("RagnarEngine.NavAgent::set_speed", SetAgentSpeed);
	mono_add_internal_call("RagnarEngine.NavAgent::get_speed", GetAgentSpeed);
	// NavAgent ==================

	// Particle System ==========
	mono_add_internal_call("RagnarEngine.ParticleSystem::get_emitters", GetEmitters);
	mono_add_internal_call("RagnarEngine.ParticleSystem::Play", PlayEmitter);
	mono_add_internal_call("RagnarEngine.ParticleSystem::Pause", PauseEmitter);
	mono_add_internal_call("RagnarEngine.ParticleSystem::SetDirectionParticle", SetDirectionParticle);
	// Particle System ==========

	// Camera ====================
	mono_add_internal_call("RagnarEngine.Camera::LookAt", LookAt);
	mono_add_internal_call("RagnarEngine.Camera::ChangeFov", ChangeFov);
	mono_add_internal_call("RagnarEngine.Camera::ScriptMovement", ScriptMovement);
	mono_add_internal_call("RagnarEngine.Camera::ScriptRotation", ScriptRotation);
	mono_add_internal_call("RagnarEngine.Camera::ScriptRotationAngle", ScriptRotationAngle);
	mono_add_internal_call("RagnarEngine.Camera::GetAngle", GetAngle);
	mono_add_internal_call("RagnarEngine.Camera::LockControlls", LockControlls);
	mono_add_internal_call("RagnarEngine.Camera::GetZoom", GetZoom);
	mono_add_internal_call("RagnarEngine.Camera::ScriptZoom", ScriptZoom);
	mono_add_internal_call("RagnarEngine.Camera::SwitchPresets", SwitchPresets);
	// Camera ====================

	// Scene Manager =============
	mono_add_internal_call("RagnarEngine.SceneManager::NextScene", NextScene);
	mono_add_internal_call("RagnarEngine.SceneManager::LoadScene", LoadScene);
	mono_add_internal_call("RagnarEngine.SceneManager::SaveScene", SaveScene);
	mono_add_internal_call("RagnarEngine.SceneManager::SaveTest", SaveTest);
	mono_add_internal_call("RagnarEngine.SceneManager::get_lastSceneName", GetLastSceneName);
	mono_add_internal_call("RagnarEngine.SceneManager::get_currentSceneName", GetCurrentSceneName);
	mono_add_internal_call("RagnarEngine.SceneManager::Exit", Exit);
	// Scene Manager =============

	// UI =======================
	mono_add_internal_call("RagnarEngine.Transform2D::get_position2D", Get2DPosition);
	mono_add_internal_call("RagnarEngine.Transform2D::set_position2D", Set2DPosition);

	mono_add_internal_call("RagnarEngine.UIButton::GetButtonState", GetButtonState);
	mono_add_internal_call("RagnarEngine.UIButton::SetButtonState", SetButtonState);
	mono_add_internal_call("RagnarEngine.UIButton::set_text", SetButtonText);
	mono_add_internal_call("RagnarEngine.UIButton::get_text", GetButtonText);

	mono_add_internal_call("RagnarEngine.UIImage::LoadTexture", LoadTexture);
	mono_add_internal_call("RagnarEngine.UIImage::UseTexture", UseTexture);

	mono_add_internal_call("RagnarEngine.UIImage::StartAnimation", StartAnimation);
	mono_add_internal_call("RagnarEngine.UIImage::StopAnimation", StopAnimation);
	mono_add_internal_call("RagnarEngine.UIImage::StopAnimation", ChangeAnimationVelocity);
	mono_add_internal_call("RagnarEngine.UIImage::SetStaticImage", SetStaticImage);

	mono_add_internal_call("RagnarEngine.UIImage::SetImageGeneralColor", SetImageGeneralColor);
	mono_add_internal_call("RagnarEngine.UIImage::GetImageGeneralColor", GetImageGeneralColor);

	mono_add_internal_call("RagnarEngine.UIImage::SetImageAlpha", SetImageAlpha);
	mono_add_internal_call("RagnarEngine.UIImage::GetImageAlpha", GetImageAlpha);

	mono_add_internal_call("RagnarEngine.UIButton::SetButtonAlpha", SetButtonAlpha);
	mono_add_internal_call("RagnarEngine.UIButton::GetButtonAlpha", GetButtonAlpha);

	mono_add_internal_call("RagnarEngine.UIButton::LoadButtonTexture", LoadButtonTexture);

	mono_add_internal_call("RagnarEngine.UIButton::SetTextPosition", SetTextPosition);
	mono_add_internal_call("RagnarEngine.UIButton::GetTextPosition", GetTextPosition);

	mono_add_internal_call("RagnarEngine.UIButton::SetButtonTextColor", SetButtonTextColor);
	mono_add_internal_call("RagnarEngine.UIButton::GetButtonTextColor", GetButtonTextColor);

	mono_add_internal_call("RagnarEngine.UIButton::SetButtonGeneralColor", SetButtonGeneralColor);
	mono_add_internal_call("RagnarEngine.UIButton::GetButtonGeneralColor", GetButtonGeneralColor);
	mono_add_internal_call("RagnarEngine.UIButton::SetVisualDisabled", SetVisualDisabled);

	mono_add_internal_call("RagnarEngine.UICheckbox::GetIsChecked", GetIsChecked);
	mono_add_internal_call("RagnarEngine.UICheckbox::GetCheckboxState", GetCheckboxState);
	mono_add_internal_call("RagnarEngine.UICheckbox::SetCheckboxState", SetCheckboxState);

	mono_add_internal_call("RagnarEngine.UISlider::GetSliderActualValue", GetSliderActualValue);

	mono_add_internal_call("RagnarEngine.Transform2D::GetSize", GetSize);
	mono_add_internal_call("RagnarEngine.Transform2D::SetSize", SetSize);

	mono_add_internal_call("RagnarEngine.UIText::set_text", SetText);
	mono_add_internal_call("RagnarEngine.UIText::get_text", GetText);

	mono_add_internal_call("RagnarEngine.UIText::SetTextTextColor", SetTextTextColor);
	mono_add_internal_call("RagnarEngine.UIText::GetTextTextColor", GetTextTextColor);

	mono_add_internal_call("RagnarEngine.UIDropDown::SetDropDownLenguage", SetDropDownLenguage);
	mono_add_internal_call("RagnarEngine.UIDropDown::SetDropDownLenguageInPause", SetDropDownLenguageInPause);
	mono_add_internal_call("RagnarEngine.UIDropDown::GetDropDownSelected", GetDropDownSelected);
	mono_add_internal_call("RagnarEngine.UIDropDown::GetSelected", GetSelected);
	mono_add_internal_call("RagnarEngine.UIDropDown::SetSelected", SetSelected);
	mono_add_internal_call("RagnarEngine.UIDropDown::GetLenguaje", GetLenguaje);
	mono_add_internal_call("RagnarEngine.UIDropDown::SetLenguaje", SetLenguaje);
	mono_add_internal_call("RagnarEngine.UIDropDown::GetDropDownButtonChange", GetDropDownButtonChange);
	// UI =======================

	// Dialogue System =======================
	mono_add_internal_call("RagnarEngine.Dialogue::GetDialogueLine", GetDialogueLine);
	mono_add_internal_call("RagnarEngine.Dialogue::GetDialogueLineAuthor", GetDialogueLineAuthor);
	mono_add_internal_call("RagnarEngine.Dialogue::GetDialogueLineAuthorId", GetDialogueLineAuthorId);
	mono_add_internal_call("RagnarEngine.Dialogue::NextLine", NextLine);
	mono_add_internal_call("RagnarEngine.Dialogue::StartDialogueById", StartDialogueById);
	mono_add_internal_call("RagnarEngine.Dialogue::LoadDialogueFile", LoadDialogueFile);
	// Dialogue System =======================


	// Configuration ===================
	mono_add_internal_call("RagnarEngine.InternalCalls::SetFullScreen", SetFullScreen);
	mono_add_internal_call("RagnarEngine.InternalCalls::GetFullScreen", GetFullScreen);
	mono_add_internal_call("RagnarEngine.InternalCalls::GetVSync", GetVSync);
	mono_add_internal_call("RagnarEngine.InternalCalls::SetVSync", SetVSync);
	mono_add_internal_call("RagnarEngine.InternalCalls::GetMousePosition", GetMousePosition);
	// Configuration ===================
	
	
	// Input Action ===================
	mono_add_internal_call("RagnarEngine.InputAction::SetActionMap", SetActionMap);
	// Input Action ===================


	InitMono();

	return ret;
}

// -----------------------------------------------------------------
bool MonoManager::CleanUp()
{
	LOG(LogType::L_NORMAL, "Cleaning mono domain");

	//mono_domain_unload(domain);
	mono_jit_cleanup(jitDomain); //Mono cleanup

	return true;
}

//#ifndef STANDALONE
//void MonoManager::OnGUI()
//{
//	if (ImGui::CollapsingHeader("Mono Settings", ImGuiTreeNodeFlags_DefaultOpen))
//	{
//		ImGui::Text("Compile tool: "); ImGui::SameLine(); ImGui::TextColored(ImVec4(1.f, 1.f, 0.f, 1.f), "MSBuild portable version");
//
//	}
//}
//#endif // !STANDALONE

void MonoManager::ReCompileCS()
{
	if (app->sceneManager->GetGameState() == GameState::PLAYING)
		return;

	app->sceneManager->GetCurrentScene()->SaveScene("Assets/Scenes/scenePlay.ragnar");

	//TODO: Clean scene and all render data
	//app->scene->CleanScene();
	//app->renderer3D->ClearAllRenderData();

	mono_domain_unload(domain);
	mono_thread_cleanup();

	while (mono_domain_is_unloading(domain) == true)
	{

	}

	CMDCompileCS();
	InitMono();

	app->sceneManager->GetCurrentScene()->LoadScene("Assets/Scenes/scenePlay.ragnar");
	app->fs->RemoveFile("Assets/Scenes/scenePlay.ragnar");

}

//ASK: Is this the worst idea ever? TOO SLOW
float3 MonoManager::UnboxVector(MonoObject* _obj)
{
	float3 ret;

	MonoClass* klass = mono_object_get_class(_obj);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "x"), &ret.x);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "y"), &ret.y);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "z"), &ret.z);
	return ret;
}
std::vector<GameObject*> MonoManager::UnboxArray(MonoArray* arr, int size)
{
	std::vector<GameObject*> obj;

	for (int i = 0; i < size; ++i)
		obj.push_back(GetGameObject(mono_array_get(arr, MonoObject*, i)));

	return obj;
}
//ASK: Is this the worst idea ever? TOO SLOW
Quat MonoManager::UnboxQuat(MonoObject* _obj)
{
	Quat ret;

	MonoClass* klass = mono_object_get_class(_obj);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "x"), &ret.x);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "y"), &ret.y);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "z"), &ret.z);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "w"), &ret.w);
	return ret;
}

void MonoManager::DebugAllFields(const char* className, std::vector<SerializedField>& _data, MonoObject* obj, ScriptComponent* script, const char* nameSpace)
{
	void* iter = NULL;
	MonoClassField* field;
	MonoClass* klass = mono_class_from_name(mono_assembly_get_image(app->moduleMono->assembly), nameSpace, className);
	while (field = mono_class_get_fields(klass, &iter))
	{
		if (mono_field_get_flags(field) != 1) //TODO: Hardcoded private = 1, public = 6, static = 22, wtf does this mean?
		{
			SerializedField pushField = SerializedField(field, obj, script);
			//uint32_t test = mono_field_get_flags(field);

			_data.push_back(pushField);
			//LOG(LogType::L_NORMAL, mono_field_full_name(method2));
		}
	}
}

void MonoManager::DebugAllMethods(const char* nsName, const char* className, std::vector<std::string>& _data)
{
	void* iter = NULL;
	MonoMethod* method2;
	MonoClass* klass = mono_class_from_name(mono_assembly_get_image(app->moduleMono->assembly), nsName, className);
	while (method2 = mono_class_get_methods(klass, &iter))
	{
		_data.push_back(mono_method_full_name(method2, 1));
		//LOG(LogType::L_NORMAL, mono_method_full_name(method2, 1));
	}
}

void MonoManager::DebugAllMethodsShortName(const char* nsName, const char* className, std::vector<std::string>& _data)
{
	void* iter = NULL;
	MonoMethod* method2;
	MonoClass* klass = mono_class_from_name(mono_assembly_get_image(app->moduleMono->assembly), nsName, className);
	while (method2 = mono_class_get_methods(klass, &iter))
	{
		_data.push_back(mono_method_get_name(method2));
	}
}

MonoObject* MonoManager::GoToCSGO(GameObject* inGo) const
{
	MonoClass* goClass = mono_class_from_name(image, SCRIPTS_NAMESPACE, "GameObject");
	uintptr_t goPtr = reinterpret_cast<uintptr_t>(inGo);

	void* args[3];
	args[0] = &inGo->name;
	args[1] = &goPtr;

	uintptr_t transPTR = reinterpret_cast<uintptr_t>(inGo->GetComponent<TransformComponent>());
	args[2] = &transPTR;

	//uintptr_t audioSourcePTR = reinterpret_cast<uintptr_t>(inGo->GetComponent<AudioSourceComponent>());
	//args[3] = &audioSourcePTR;
	//
	//uintptr_t audioListenerPTR = reinterpret_cast<uintptr_t>(inGo->GetComponent<ListenerComponent>());
	//args[4] = &audioListenerPTR;
	//
	//uintptr_t rbPTR = reinterpret_cast<uintptr_t>(inGo->GetComponent<RigidBodyComponent>());
	//args[5] = &rbPTR;
	//
	//uintptr_t animPTR = reinterpret_cast<uintptr_t>(inGo->GetComponent<AnimationComponent>());
	//args[6] = &animPTR;
	//
	//uintptr_t camPTR = reinterpret_cast<uintptr_t>(inGo->GetComponent<CameraComponent>());
	//args[7] = &camPTR;

	//MonoMethodDesc* constructorDesc = mono_method_desc_new("RagnarEngine.GameObject:.ctor(string,uintptr,uintptr,uintptr,uintptr,uintptr,uintptr)", true);
	MonoMethodDesc* constructorDesc = mono_method_desc_new("RagnarEngine.GameObject:.ctor(string,uintptr,uintptr)", true);
	MonoMethod* method = mono_method_desc_search_in_class(constructorDesc, goClass);
	MonoObject* goObj = mono_object_new(domain, goClass);
	mono_runtime_invoke(method, goObj, args, NULL);

	mono_method_desc_free(constructorDesc);

	return goObj;
}

MonoObject* MonoManager::ComponentToCS(Component* comp) const
{
	std::string compName = std::string();

	MonoClass* compClass = mono_class_from_name(image, SCRIPTS_NAMESPACE, "RagnarComponent");

	MonoMethodDesc* constructorDesc = mono_method_desc_new("RagnarEngine.RagnarComponent:.ctor(uintptr)", true);
	
	MonoMethod* contstructorMethod = mono_method_desc_search_in_class(constructorDesc, compClass);
	MonoObject* compObj = mono_object_new(domain, compClass);
	//uint32_t fieldToken = mono_gchandle_new(goObj, false);

	//MonoClass* monoClass = mono_object_get_class(goObj);
	//MonoClassField* field = mono_class_get_field(monoClass, fieldToken);

	//uintptr_t compPTR = reinterpret_cast<uintptr_t>(comp);
	//mono_field_set_value(goObj, field, comp);
	
	void* args[1];
	uintptr_t compPTR = reinterpret_cast<uintptr_t>(comp);
	args[0] = &compPTR;
	mono_runtime_invoke(contstructorMethod, compObj, args, NULL);
	
	mono_method_desc_free(constructorDesc);

	return compObj;
}


MonoObject* MonoManager::Float3ToCS(float3& inVec) const
{
	MonoClass* vecClass = mono_class_from_name(image, SCRIPTS_NAMESPACE, "Vector3");

	MonoObject* vecObject = mono_object_new(domain, vecClass);
	const char* name = mono_class_get_name(mono_object_get_class(vecObject));

	void* args[3];
	args[0] = &inVec.x;
	args[1] = &inVec.y;
	args[2] = &inVec.z;

	MonoMethodDesc* constDesc = mono_method_desc_new("RagnarEngine.Vector3:.ctor(single,single,single)", true);
	MonoMethod* method = mono_method_desc_search_in_class(constDesc, vecClass);

	mono_runtime_invoke(method, vecObject, args, NULL);

	mono_method_desc_free(constDesc);
	return vecObject;
}

void MonoManager::LoadFieldData(SerializedField& _field, MonoObject* _object)
{
	switch (_field.type)
	{
	case MonoTypeEnum::MONO_TYPE_BOOLEAN:
		mono_field_get_value(_object, _field.field, &_field.fiValue.bValue);
		break;

	case MonoTypeEnum::MONO_TYPE_I4:
		mono_field_get_value(_object, _field.field, &_field.fiValue.iValue);
		break;

	case MonoTypeEnum::MONO_TYPE_CLASS:
		_field.fiValue.goValue = nullptr;
		break;

	case MonoTypeEnum::MONO_TYPE_GENERICINST:
	{
		break;
	}

	case MonoTypeEnum::MONO_TYPE_SZARRAY:
	{
		_field.fiValue.arrValue = nullptr;

		MonoArray* arr;
		mono_field_get_value(_object, _field.field, &_field.fiValue.arrValue);
		break;
	}

	case MonoTypeEnum::MONO_TYPE_R4:
		mono_field_get_value(_object, _field.field, &_field.fiValue.fValue);
		break;

	case MonoTypeEnum::MONO_TYPE_STRING:
		mono_field_get_value(_object, _field.field, &_field.fiValue.strValue);
		break;

	default:
		mono_field_get_value(_object, _field.field, &_field.fiValue.iValue);
		break;
	}
}

MonoObject* MonoManager::QuatToCS(Quat& inVec) const
{
	MonoClass* quadClass = mono_class_from_name(image, SCRIPTS_NAMESPACE, "Quaternion");
	MonoObject* quatObject = mono_object_new(domain, quadClass);

	void* args[4];
	args[0] = &inVec.x;
	args[1] = &inVec.y;
	args[2] = &inVec.z;
	args[3] = &inVec.w;

	MonoMethodDesc* constDesc = mono_method_desc_new("RagnarEngine.Quaternion:.ctor(single,single,single,single)", true);
	MonoMethod* method = mono_method_desc_search_in_class(constDesc, quadClass);

	mono_runtime_invoke(method, quatObject, args, NULL);

	mono_method_desc_free(constDesc);
	return quatObject;
}

GameObject* MonoManager::GameObjectFromCSGO(MonoObject* goObj)
{
	uintptr_t ptr = 0;
	MonoClass* goClass = mono_class_from_name(image, SCRIPTS_NAMESPACE, "GameObject");

	mono_field_get_value(goObj, mono_class_get_field_from_name(goClass, "pointer"), &ptr);

	return reinterpret_cast<GameObject*>(ptr);
}

GameObject* MonoManager::GameObjectFromCSComponent(MonoObject* goComponent)
{
	uintptr_t ptr = 0;
	MonoClass* goClass = mono_class_from_name(image, SCRIPTS_NAMESPACE, "RagnarComponent");

	mono_field_get_value(goComponent, mono_class_get_field_from_name(goClass, "pointer"), &ptr);

	return reinterpret_cast<Component*>(ptr)->owner;
}



////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////


SerializedField::SerializedField() : field(nullptr), parentSC(nullptr)
{
	fiValue.iValue = 0;
}

SerializedField::SerializedField(MonoClassField* _field, MonoObject* _object, ScriptComponent* parent) : field(_field)
{
	type = static_cast<MonoTypeEnum>(mono_type_get_type(mono_field_get_type(field)));
	fiValue.iValue = 0;
	parentSC = parent;

	displayName += "##";
	displayName += mono_field_get_name(_field);

	MonoManager::LoadFieldData(*this, _object);
}

void MonoManager::CreateAssetsScript(const char* localPath)
{
	std::string unnormalizedPath("Assets/Scripts/");
	unnormalizedPath += localPath;
	app->fs->UnNormalizePath(unnormalizedPath);

	std::ofstream outfile(unnormalizedPath.c_str());

	std::string className("Assets/Scripts/");
	className += localPath;
	className = className.substr(className.find_last_of("/") + 1);
	className = className.substr(0, className.find_last_of("."));

	outfile << "using System;" << std::endl << "using RagnarEngine;" << std::endl << std::endl << "public class " << className.c_str() << " : RagnarComponent" << std::endl << "{" << std::endl <<
		"	public void Start()" << std::endl << "	{" << std::endl << std::endl << "	}" << std::endl << 
		"	public void Update()" << std::endl << "	{" << std::endl << std::endl << "	}" << std::endl << std::endl << "}";

	outfile.close();

	AddScriptToSLN(unnormalizedPath.c_str());
	ReCompileCS();
}

void MonoManager::AddScriptToSLN(const char* scriptLocalPath)
{
	pugi::xml_document doc;
	pugi::xml_parse_result result = doc.load_file("Assembly-CSharp.csproj");

	if (result.status == pugi::xml_parse_status::status_file_not_found)
		assert(false, "XML File not loaded");

	std::string path;
	path += scriptLocalPath;
	std::string name = path.substr(path.find_last_of("\\"));

	pugi::xml_node whereToAdd = doc.child("Project");
	for (pugi::xml_node panel = whereToAdd.first_child(); panel != nullptr; panel = panel.next_sibling())
	{
		if (strcmp(panel.name(), "ItemGroup") == 0 && strcmp(panel.first_child().name(), "Compile") == 0)
		{
			panel = panel.append_child();
			panel.set_name("Compile");
			pugi::xml_attribute att = panel.append_attribute("Include");
			att.set_value(path.c_str());

			break;
		}
	}

	doc.save_file("Assembly-CSharp.csproj");
}

void MonoManager::RemoveScriptFromSLN(const char* scriptLocalPath)
{
	pugi::xml_document doc;
	pugi::xml_parse_result result = doc.load_file("Assembly-CSharp.csproj");

	if (result.status == pugi::xml_parse_status::status_file_not_found)
		assert(false, "XML File not loaded");

	std::string path;

	pugi::xml_node whereToRemove = doc.child("Project");
	for (pugi::xml_node panel = whereToRemove.first_child(); panel != nullptr; panel = panel.next_sibling())
	{
		if (strcmp(panel.name(), "ItemGroup") == 0 && strcmp(panel.first_child().name(), "Compile") == 0)
		{
			for (pugi::xml_node toRemove = panel.first_child(); toRemove != nullptr; toRemove = toRemove.next_sibling())
			{
				path = toRemove.attribute("Include").as_string();
				app->fs->NormalizePath(path);

				if (strcmp(path.c_str(), scriptLocalPath) == 0)
				{
					panel.remove_child(toRemove);
					break;
				}
			}
		}
	}

	doc.save_file("Assembly-CSharp.csproj");
}


void MonoManager::InitMono()
{
	domain = mono_domain_create_appdomain("D1", NULL);
	mono_domain_set(domain, 0);
	domainThread = mono_thread_attach(domain);

	MonoImageOpenStatus sts;

	if (app->fs->Exists("Library/ScriptsAssembly/Assembly-CSharp.dll") == false)
		assert(false && "You need to install the 'Visual Studio .NET desktop development pack' to run the engine");

	assembly = mono_assembly_open("Library/ScriptsAssembly/Assembly-CSharp.dll", &sts);
	if (!assembly)
		LOG(LogType::L_ERROR, "ERROR");

	image = mono_assembly_get_image(assembly);

	const MonoTableInfo* table_info = mono_image_get_table_info(image, MONO_TABLE_TYPEDEF);
	int rows = mono_table_info_get_rows(table_info);

	MonoClass* _class = nullptr;

	userScripts.clear();
	for (int i = 0; i < rows; i++)
	{
		uint32_t cols[MONO_TYPEDEF_SIZE];
		mono_metadata_decode_row(table_info, i, cols, MONO_TYPEDEF_SIZE);
		const char* name = mono_metadata_string_heap(image, cols[MONO_TYPEDEF_NAME]);

		if (name[0] != '<')
		{
			const char* name_space = mono_metadata_string_heap(image, cols[MONO_TYPEDEF_NAMESPACE]);
			_class = mono_class_from_name(image, name_space, name);

			if (_class != nullptr && strcmp(mono_class_get_namespace(_class), SCRIPTS_NAMESPACE) != 0 && !mono_class_is_enum(_class))
			{
				userScripts.push_back(_class);
				LOG(LogType::L_WARNING, "%s", mono_class_get_name(_class));
			}
		}
	}
}

void MonoManager::UpdateListScripts()
{
	const MonoTableInfo* table_info = mono_image_get_table_info(image, MONO_TABLE_TYPEDEF);
	int rows = mono_table_info_get_rows(table_info);

	MonoClass* _class = nullptr;

	userScripts.clear();
	for (int i = 0; i < rows; i++)
	{
		uint32_t cols[MONO_TYPEDEF_SIZE];
		mono_metadata_decode_row(table_info, i, cols, MONO_TYPEDEF_SIZE);
		const char* name = mono_metadata_string_heap(image, cols[MONO_TYPEDEF_NAME]);

		if (name[0] != '<')
		{
			const char* name_space = mono_metadata_string_heap(image, cols[MONO_TYPEDEF_NAMESPACE]);
			_class = mono_class_from_name(image, name_space, name);

			if (_class != nullptr && strcmp(mono_class_get_namespace(_class), SCRIPTS_NAMESPACE) != 0 && !mono_class_is_enum(_class))
			{
				userScripts.push_back(_class);
				LOG(LogType::L_WARNING, "%s", mono_class_get_name(_class));
			}
		}
	}
}

float2 MonoManager::UnboxVector2D(MonoObject* _obj)
{
	float2 ret;

	MonoClass* klass = mono_object_get_class(_obj);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "x"), &ret.x);
	mono_field_get_value(_obj, mono_class_get_field_from_name(klass, "y"), &ret.y);
	return ret;
}
