#pragma once

#include "ScriptComponent.h"

#include "AnimationComponent.h"
#include "Animation.h"

#include <metadata\object-forward.h>
#include <metadata\object.h>
#include <metadata/class.h>

void PlayAnimation(MonoObject* go, MonoObject* animationName)
{
	char* animName = mono_string_to_utf8(mono_object_to_string(animationName, 0));
	std::string nameState = animName;
	GetComponentMono<AnimationComponent*>(go)->Play(nameState);
}

bool HasFinished(MonoObject* go)
{
	return GetComponentMono<AnimationComponent*>(go)->HasFinished();
}

float GetDuration(MonoObject* go)
{
	return GetComponentMono<AnimationComponent*>(go)->currAnim->anim->GetDuration();
}

float GetLoopTime(MonoObject* go)
{
	return GetComponentMono<AnimationComponent*>(go)->loopTime;
}