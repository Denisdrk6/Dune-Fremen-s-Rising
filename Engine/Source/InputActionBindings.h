#pragma once

#include "ScriptComponent.h"

#include "InputActionComponent.h"

#include <metadata\object-forward.h>
#include <metadata\object.h>
#include <metadata/class.h>

void SetActionMap(MonoObject* go, int index)
{
	GetComponentMono<InputActionComponent*>(go)->SetActionMap(index);
}