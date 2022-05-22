#include "Animation.h"
#include "AnimationComponent.h"
#include "Application.h"
#include "ModuleSceneManager.h"
#include "Scene.h"

#include "MeshComponent.h"

#include "FileSystem.h"
#include "ResourceManager.h"

#include "Math/TransformOps.h"
#include "Imgui/imgui_stdlib.h"

#include "Profiling.h"

AnimationComponent::AnimationComponent(GameObject* own) : showAnimMenu(false), deltaTime(0.0f), currAnim(nullptr), playing(false), loopTime(0.0f), interpolating(false), lastAnim(nullptr), lastCurrentTime(0.0f), interpolatingVel(1.0f)
{
	type = ComponentType::ANIMATION;
	owner = own;

	currentTime = 0.0f;

	finalBoneMatrices.reserve(100);

	for (int i = 0; i < 100; i++)
	{
		finalBoneMatrices.push_back(float4x4::identity);
	}

	boneInfoMap = owner->GetComponent<MeshComponent>()->GetBoneMap();

	animations.push_back({ "None", nullptr, false });

	active = true;
}

AnimationComponent::AnimationComponent(AnimationComponent* animation) : showAnimMenu(false)
{
	currAnim = animation->currAnim;
}

AnimationComponent::~AnimationComponent()
{
	for (int i = 0; i < animations.size(); ++i)
	{
		std::shared_ptr<Animation> anim = animations[i].anim;
		if (anim.use_count() - 1 == 1)
		{
			anim->UnLoad();
		}
	}
}

void AnimationComponent::OnEditor()
{
	ImGui::PushID(this);
	if (ImGui::CollapsingHeader(ICON_FA_PEOPLE_ARROWS" Animation Component"))
	{
		for (int i = 0; i < animations.size(); ++i)
		{
			AnimState* currState = &animations[i];
			ImGui::PushID((void*)currState->state.c_str());

			int width = ImGui::GetWindowContentRegionMax().x;
			ImGui::PushItemWidth(width / 3);
			ImGui::InputText("##State", &currState->state, 64);
			ImGui::PopItemWidth();

			unsigned int test = currState->state.size();
			ImGui::SameLine();

			ImGui::PushItemWidth(2 * width / 3 - 15.0f);
			if (ImGui::BeginCombo("##Animations", currState->anim ? currState->anim->GetName().c_str() : "None"))
			{
				if (ImGui::Selectable("None"))
				{
					currState->state = "None";
					currState->anim = nullptr;
					currState->loop = false;
				}

				std::vector<std::shared_ptr<Animation>> res;

				std::vector<std::string> files;
				app->fs->DiscoverFiles("Library/Animations", files);
				for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
				{
					if ((*it).find(".rganim") != std::string::npos)
					{
						ImGui::PushID((*it).c_str());
						app->fs->GetFilenameWithoutExtension(*it);
						*it = (*it).substr((*it).find_last_of("_") + 1, (*it).length());
						uint uid = std::stoll(*it);
						res.push_back(std::static_pointer_cast<Animation>(ResourceManager::GetInstance()->GetResource(uid)));
						ImGui::PopID();
					}
				}

				std::sort(res.begin(), res.end(), [](std::shared_ptr<Resource> a, std::shared_ptr<Resource> b) {
					return (a.get()->GetName() < b.get()->GetName());
					});

				for (std::vector<std::shared_ptr<Animation>>::iterator it = res.begin(); it != res.end(); ++it)
				{
					if (ImGui::Selectable((*it).get()->GetName().c_str()))
					{
						(*it)->Load();
						currState->anim = (*it);
						if (currAnim && currAnim->anim.use_count() - 1 == 1) currAnim->anim->UnLoad();
						currAnim = currState;
					}
				}

				ImGui::EndCombo();
			}
			ImGui::PopItemWidth();

			ImGui::Checkbox("Loop", &currState->loop);
			ImGui::SameLine();
			if (ImGui::Button(ICON_FA_INFO) && currAnim)
			{
				currAnim = currState;
			}
			ImGui::PopID();
		}

		if (ImGui::Button(ICON_FA_PLUS))
		{
			if (currAnim != nullptr)
			{
				std::string aux = currAnim->state;
				animations.push_back({ "None", nullptr, false, false });
				for (int i = 0; i < animations.size(); ++i)
				{
					if (animations[i].state == aux) currAnim = &animations[i];
				}
			}
			else
			{
				animations.push_back({ "None", nullptr, false, false });
			}
		}
		ImGui::SameLine();
		if (ImGui::Button(ICON_FA_MINUS))
		{
			animations.erase(animations.end() - 1);
		}

		ImGui::DragFloat("Interp. Velocity", &interpolatingVel, 0.5f, 1.0f);
		ImGui::Separator();

		AnimationInfo();
	}
	ImGui::PopID();
}

void AnimationComponent::AnimationInfo()
{
	if (currAnim && currAnim->anim)
	{
		ImGui::Text("Path: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%s", currAnim->anim->GetAssetsPath().c_str());
		ImGui::Text("Ticks: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%.0f", currAnim->anim->GetTicks());
		ImGui::Text("Ticks Per Second: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%.0f", currAnim->anim->GetTicksPerSecond());
		ImGui::Text("Duration: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%.0f s", currAnim->anim->GetDuration());
		ImGui::Text("Bones Attached: ");
		ImGui::SameLine();
		ImGui::TextColored(ImVec4(1, 1, 0, 1), "%d bones", currAnim->anim->bones.size());
	}
	else
	{
		ImGui::Text("There's is no current animation");
	}
}

bool AnimationComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Animation Update");

	deltaTime = dt;
	if (currAnim && playing && app->sceneManager->GetGameState() == GameState::PLAYING)
	{
		// Loop time keeps track of the miliseconds that passed since the start of the animation
		// GetDuration gets the duration of the animation in miliseconds
		if (loopTime > currAnim->anim->GetDuration())
		{
			currAnim->hasFinished = true;
			// When it reaches the desired duration, we reset everything
			if (currAnim->loop && animQueue.empty())
			{
				loopTime = 0.0f;
			}
			else
			{
				loopTime = 0.0f;
				playing = false;
				lastAnim = currAnim;

				if (!animQueue.empty())
				{
					currAnim = animQueue.front();
					animQueue.pop();
				}

				if (lastAnim != currAnim) interpolating = true;
			}
		}

		if (!interpolating)
		{
			// Loop time increases by our delta time
			loopTime += dt;
			currentTime += currAnim->anim->GetTicksPerSecond() * dt;
			if (!currAnim->loop && currentTime >= currAnim->anim->GetTicks())
			{
				currentTime = currAnim->anim->GetTicks() - currAnim->anim->GetTicksPerSecond() * dt;
			}
			currentTime = fmod(currentTime, currAnim->anim->GetTicks());
		}
		else
		{
			loopTime += dt;
			// 60 is the frames per second that we want
			currentTime += 24.0f * dt;
			if (!currAnim->loop && currentTime >= currAnim->anim->GetTicks())
			{
				currentTime = currAnim->anim->GetTicks() - currAnim->anim->GetTicksPerSecond() * dt;
			}
			currentTime = fmod(currentTime, currAnim->anim->GetTicks());
		}
		
		CalculateBoneTransform(currAnim->anim->GetHierarchyData(), float4x4::identity);
	}

	return true;
}

void AnimationComponent::CalculateBoneTransform(HierarchyData& data, float4x4 parentTransform)
{
	std::string nodeName = data.name;

	float4x4 globalTransformation = float4x4::identity;

	Bone* bone = currAnim->anim->FindBone(nodeName);
	Bone* lastBone = lastAnim->anim->FindBone(nodeName);

	if (bone)
	{
		if (!interpolating)
		{
			bone->Update(currentTime);
		}
		else if (lastBone && lastAnim != currAnim)
		{
			bone->UpdateInterpolation(*lastBone, currentTime, lastCurrentTime, interpolating, interpolatingVel);
			if (!interpolating)
			{
				loopTime = 0.0f;
				currentTime = 0.0f;
			}
		}

		globalTransformation = parentTransform * bone->GetTransform();
	}

	if (boneInfoMap.find(nodeName) != boneInfoMap.end())
	{
		const BoneInfo& boneInfo = boneInfoMap.at(nodeName);
		int index = boneInfo.id;
		const float4x4& offset = boneInfo.offset;
		finalBoneMatrices[index] = globalTransformation * offset;
	}

	for (int i = 0; i < data.childrenCount; ++i)
		CalculateBoneTransform(data.children[i], globalTransformation);
}

bool AnimationComponent::OnLoad(JsonParsing& node)
{
	active = node.GetJsonBool("Active");

	JSON_Array* jsonArray = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "Animations");

	animations.clear();

	std::string currAnimState = node.GetJsonString("Current Anim");
	size_t size = node.GetJsonArrayCount(jsonArray);
	for (int i = 0; i < size; ++i)
	{
		AnimState anim;

		JsonParsing c = node.GetJsonArrayValue(jsonArray, i);
		anim.state = c.GetJsonString("State");
		anim.loop = c.GetJsonBool("Loop");
		anim.anim = std::static_pointer_cast<Animation>(ResourceManager::GetInstance()->LoadResource(std::string(c.GetJsonString("Path Anim Assets"))));

		animations.push_back(anim);
	}

	for (std::vector<AnimState>::iterator it = animations.begin(); it != animations.end(); ++it)
	{
		if (currAnimState == (*it).state)
		{
			currAnim = &(*it);
			break;
		}
	}

	return true;
}

bool AnimationComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Active", active);

	file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Current Anim", currAnim->state.c_str());

	JSON_Array* newArray = file.SetNewJsonArray(file.GetRootValue(), "Animations");
	for (std::vector<AnimState>::iterator it = animations.begin(); it != animations.end(); ++it)
	{
		JsonParsing animF = JsonParsing();

		animF.SetNewJsonString(animF.ValueToObject(animF.GetRootValue()), "State", (*it).state.c_str());
		animF.SetNewJsonString(animF.ValueToObject(animF.GetRootValue()), "Path Anim Assets",(*it).anim != nullptr ? (*it).anim.get()->GetAssetsPath().c_str() : "");
		animF.SetNewJsonBool(animF.ValueToObject(animF.GetRootValue()), "Loop", (*it).loop);
		
		file.SetValueToArray(newArray, animF.GetRootValue());
	}

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

void AnimationComponent::Play(std::string state)
{
	if (app->sceneManager->GetTimeScale() == 0.0f)
	{
		return;
	}
	for (int i = 0; i < animations.size(); ++i)
	{
		if (animations[i].state == state)
		{
			if (animQueue.empty())
			{
				if (!interpolating) lastAnim = currAnim;

				currAnim = &animations[i];
				lastCurrentTime = currentTime;
				
				currAnim->hasFinished = false;
				currentTime = 0.0f;
				loopTime = 0.0f;

				if (lastAnim != currAnim)
				{
					interpolating = true;
				}
				playing = true;
			}
			else
				animQueue.push(currAnim);
			break;
		}
	}
}