#pragma once

#include "ScriptComponent.h"

#include "ButtonComponent.h"
#include "SliderComponent.h"
#include "CheckBoxComponent.h"
#include "Transform2DComponent.h"
#include "TextComponent.h"
#include "ImageComponent.h"
#include "DropDownComponent.h"

#include <metadata\object-forward.h>
#include <metadata\object.h>
#include <metadata/class.h>

MonoObject* Get2DPosition(MonoObject* go)
{
	ComponentTransform2D* tr = GetComponentMono<ComponentTransform2D*>(go);
	//float3 position = ;
	MonoObject* vec = app->moduleMono->Float3ToCS(tr->GetPosition());
	char* name = mono_string_to_utf8(mono_object_to_string(vec, 0));

	return vec;
}

void Set2DPosition(MonoObject* go, MonoObject* position)
{
	if (ComponentTransform2D* tr = GetComponentMono<ComponentTransform2D*>(go))
	{		
		tr->SetPosition(app->moduleMono->UnboxVector(position));
	}
}
 // Button ==============================
int LoadButtonTexture(MonoObject* go, MonoString* text)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	std::string y = mono_string_to_utf8(text);
	
	tr->GetNormalMaterial()->SetTexture(ResourceManager::GetInstance()->LoadResource(y));
	tr->GetFocusedMaterial()->SetTexture(ResourceManager::GetInstance()->LoadResource(y));
	tr->GetPressedMaterial()->SetTexture(ResourceManager::GetInstance()->LoadResource(y));
	tr->GetActualMaterial()->SetTexture(ResourceManager::GetInstance()->LoadResource(y));
	return 0;
}
float GetButtonAlpha(MonoObject* go)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	return tr->GetAlpha();
}
void SetButtonAlpha(MonoObject* go,float newAlpha)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	tr->SetAlpha(newAlpha);
}

void SetButtonTextColor(MonoObject* go, float Red, float Green, float Blue)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);

	tr->setTextColor(Red, Green, Blue);

}
const Vec3 GetButtonTextColor(MonoObject* go)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	return Vec3(tr->GetTextColor().x, tr->GetTextColor().y, tr->GetTextColor().z);
}


void SetButtonGeneralColor(MonoObject* go, float Red, float Green, float Blue)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);

	tr->SetActualColor(Red, Green, Blue);

}
const Vec3 GetButtonGeneralColor(MonoObject* go)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	return Vec3(tr->GetActualColor().r, tr->GetActualColor().g, tr->GetActualColor().b);
}


int GetButtonState(MonoObject* go)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	//float3 position = ;
	

	return (int)tr->GetState();
}
void SetButtonState(MonoObject* go,int newState)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	//float3 position = ;
	tr->SetState(newState);

	
}
void SetButtonText(MonoObject* go, MonoString* text)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	tr->GetButtonText().textt.clear();
	char* aux = mono_string_to_utf8(text);
	tr->SetText(aux);
	
}
const char* GetButtonText(MonoObject* go)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	return tr->GetButtonText().textt.c_str();
}

void SetTextPosition(MonoObject* go, float posx,float posy)
{
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	float3 i = { posx ,posy,0 };
	tr->SetTextPosition(i);



}
float3 GetTextPosition(MonoObject* go)
{
	const char* name = mono_class_get_name(mono_object_get_class(go));
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	//float3 position = ;


	return tr->GetTextPosition();
}

void SetVisualDisabled(MonoObject* go,bool newDisabled)
{
	const char* name = mono_class_get_name(mono_object_get_class(go));
	ButtonComponent* tr = GetComponentMono<ButtonComponent*>(go);
	tr->SetVisualDisabled(newDisabled);
	//float3 position = ;



}
// Checkbox =========================================
const bool GetIsChecked(MonoObject* go)
{
	CheckboxComponent* tr = GetComponentMono<CheckboxComponent*>(go);
	//float3 position = ;

	return tr->GetChecked();
}
int GetCheckboxState(MonoObject* go)
{
	CheckboxComponent* tr = GetComponentMono<CheckboxComponent*>(go);
	//float3 position = ;


	return (int)tr->GetState();
}
void SetCheckboxState(MonoObject* go, bool newState) 
{
	CheckboxComponent* tr = GetComponentMono<CheckboxComponent*>(go);
	tr->setChecker(newState);
}
// Slider =========================================
float GetSliderActualValue(MonoObject* go)
{
	SliderComponent* tr = GetComponentMono<SliderComponent*>(go);
	//float3 position = ;


	return tr->GetBarProgress();
}

// Text ==================================
void SetText(MonoObject* go, MonoString* text)
{
	TextComponent* tr = GetComponentMono<TextComponent*>(go);
	char* aux = mono_string_to_utf8(text);
	tr->SetText(aux);
}
MonoString* GetText(MonoObject* go)
{
	TextComponent* tr = GetComponentMono<TextComponent*>(go);
	return mono_string_new(app->moduleMono->domain, tr->GetText().c_str());
}
void SetTextTextColor(MonoObject* go, float Red,float Green,float Blue)
{
	TextComponent* tr = GetComponentMono<TextComponent*>(go);
	
	tr->setTextColor(Red,Green,Blue);

}
const Vec3 GetTextTextColor(MonoObject* go)
{
	TextComponent* tr = GetComponentMono<TextComponent*>(go);	
	return Vec3(tr->GetTextColor().x, tr->GetTextColor().y, tr->GetTextColor().z);
}

// Transform 2D ========================================
Vec3 GetSize(MonoObject* go)
{
	ComponentTransform2D* tr = GetComponentMono<ComponentTransform2D*>(go);	
	Vec3 vec2(tr->GetButtonWidth(), tr->GetButtonHeight(),0);
	return vec2;
}

void SetSize(MonoObject* go, MonoObject* size)
{
	if (ComponentTransform2D* tr = GetComponentMono<ComponentTransform2D*>(go))
	{
		tr->SetButtonWidth(app->moduleMono->UnboxVector2D(size).x);
		tr->SetButtonHeight(app->moduleMono->UnboxVector2D(size).y);		
	}
}
// Image ========================================

int LoadTexture(MonoObject* go, MonoString* text)
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	std::string y=mono_string_to_utf8(text);
	
	tr->principal->SetTexture(ResourceManager::GetInstance()->LoadResource(y));
	return 0;
}
void UseTexture(MonoObject* go, int ID)
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	
	tr->UseTexture(ID);
}

void SetImageGeneralColor(MonoObject* go, float Red, float Green, float Blue)
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);

	tr->SetActualColor(Red, Green, Blue);

}
const Vec3 GetImageGeneralColor(MonoObject* go)
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	return Vec3(tr->GetActualColor().r, tr->GetActualColor().g, tr->GetActualColor().b);
}

float GetImageAlpha(MonoObject* go)
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	return tr->GetAlpha();
}
void SetImageAlpha(MonoObject* go, float newAlpha)
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	tr->SetAlpha(newAlpha);
}

// DropDown ========================================
void SetDropDownLenguage(MonoObject* go, int selected) {
	app->sceneManager->SetLenguage(selected);
	DropDownComponent* tr = GetComponentMono<DropDownComponent*>(go);
	tr->SetChangeState(false);
}

void SetDropDownLenguageInPause(MonoObject* go, int selected) {
	app->sceneManager->SetLenguage(selected);
	DropDownComponent* tr = GetComponentMono<DropDownComponent*>(go);
	tr->SetChangeState(false);
	DialogueSystem* sys = DialogueSystem::GetInstance();
	sys->LoadDialogueXML();
	sys->UpdateDialogue();
}

int GetDropDownSelected(MonoObject* go)
{
	DropDownComponent* tr = GetComponentMono<DropDownComponent*>(go);
	return (int)tr->GetSelectedID();
}

const char* GetSelected(MonoObject* go)
{
	DropDownComponent* tr = GetComponentMono<DropDownComponent*>(go);
	return tr->GetSelect().c_str();
}

bool GetDropDownButtonChange(MonoObject* go)
{
	DropDownComponent* tr = GetComponentMono<DropDownComponent*>(go);
	return tr->GetChangeState();
}
// Animation ========================================

void StartAnimation(MonoObject* go, int animation) 
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	tr->animations[animation]->StartAnim();
}

void StopAnimation(MonoObject* go, int animation) 
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	tr->animations[animation]->StopAnim();
}

void ChangeAnimationVelocity(MonoObject* go, int animation,float velocity)
{
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	tr->animations[animation]->ChageVelocity(velocity);
}
void SetStaticImage(MonoObject* go, int animation, int image) {
	ImageComponent* tr = GetComponentMono<ImageComponent*>(go);
	tr->animations[animation]->SetStaticimage(image);
}