#pragma once

#include "GameObject.h"
#include "AudioSourceComponent.h"
#include "ListenerComponent.h"

#include "ScriptBindings.h"

#include <metadata\object-forward.h>
#include <metadata\object.h>
#include <metadata/class.h>

// AUDIO SOURCE ================================================
void PlayClip(MonoObject* clipName, MonoObject* owner)
{
	// IMPORTANT! Names are flipped but it works, if i try to flip them it won't work
	//TOCHANGE
	char* name = mono_string_to_utf8(mono_object_to_string(owner, 0));
	GetComponentMono<AudioSourceComponent*>(clipName)->PlayClipEverywhere(name);
}

void SetState(MonoObject* go, MonoObject* group, MonoObject* state)
{
	// IMPORTANT! Names are flipped but it works, if i try to flip them it won't work
	char* groupN = mono_string_to_utf8(mono_object_to_string(group, 0));
	char* stateN = mono_string_to_utf8(mono_object_to_string(state, 0));
	GetComponentMono<AudioSourceComponent*>(go)->SetState(groupN, stateN);
}

void StopCurrentClip(MonoObject* go, MonoObject* clipName)
{
	char* audioName = mono_string_to_utf8(mono_object_to_string(clipName, 0));
	std::string nameState = audioName;
	GetComponentMono<AudioSourceComponent*>(go)->StopClip(nameState);
}

void SetClipVolume(MonoObject* go, float vol)
{
	GetComponentMono<AudioSourceComponent*>(go)->SetClipVolume(vol);
}

float GetClipVolume(MonoObject* go)
{
	return GetComponentMono<AudioSourceComponent*>(go)->GetClipVolume();
}

// AUDIO LISTENER ============================================
void TestListener(MonoObject* go)
{
	GetComponentMono<ListenerComponent*>(go);
}
