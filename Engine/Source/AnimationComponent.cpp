#include "AnimationComponent.h"
#include "Application.h"

#include "GameObject.h"
#include "MeshComponent.h"

#include "ModuleScene.h"

#include "FileSystem.h"
#include "ResourceManager.h"

#include "Imgui/imgui_stdlib.h"

AnimationComponent::AnimationComponent(GameObject* own) : showAnimMenu(false), deltaTime(0.0f), currAnim(nullptr), playing(false), loopTime(0.0f)
{
	type = ComponentType::ANIMATION;
	owner = own;

	currentTime = 0.0f;

	finalBoneMatrices.reserve(100);

	for (int i = 0; i < 100; i++)
	{
		finalBoneMatrices.push_back(float4x4::identity);
	}

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

				std::vector<std::string> files;
				app->fs->DiscoverFiles("Library/Animations", files);
				for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
				{
					if ((*it).find(".rganim") != std::string::npos)
					{
						app->fs->GetFilenameWithoutExtension(*it);
						*it = (*it).substr((*it).find_last_of("_") + 1, (*it).length());
						uint uid = std::stoll(*it);
						std::shared_ptr<Animation> res = std::static_pointer_cast<Animation>(ResourceManager::GetInstance()->GetResource(uid));
						if (ImGui::Selectable(res->GetName().c_str()))
						{
							res->Load();
							currState->anim = res;
							if (currAnim && currAnim->anim.use_count() - 1 == 1) currAnim->anim->UnLoad();
							currAnim = currState;
						}
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
				animations.push_back({ "None", nullptr, false });
				for (int i = 0; i < animations.size(); ++i)
				{
					if (animations[i].state == aux) currAnim = &animations[i];
				}
			}
			else
			{
				animations.push_back({ "None", nullptr, false });
			}
		}
		ImGui::SameLine();
		if (ImGui::Button(ICON_FA_MINUS))
		{
			animations.erase(animations.end() - 1);
		}

		ImGui::Separator();

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
	ImGui::PopID();

	ImGui::Separator();
}

bool AnimationComponent::Update(float dt)
{
	deltaTime = dt;
	if (currAnim && playing && app->scene->GetGameState() == GameState::PLAYING)
	{
		// Loop time keeps track of the miliseconds that passed since the start of the animation
		// GetDuration gets the duration of the animation in miliseconds
		if (loopTime > currAnim->anim->GetDuration())
		{
			currAnim->hasFinished = true;
			// When it reaches the desired duration, we reset everything
			if (currAnim->loop && animQueue.empty()) loopTime = 0.0f;
			else
			{
				loopTime = 0.0f;
				playing = false;
				if(animQueue.empty())
				{
					Play("Idle");
				}
				else
				{
					currAnim = animQueue.front();
					animQueue.pop();
				}
			}
		}

		// Loop time increases by our delta time
		loopTime += dt;
		currentTime += currAnim->anim->GetTicksPerSecond() * dt;
		currentTime = fmod(currentTime, currAnim->anim->GetTicks());
		CalculateBoneTransform(currAnim->anim->GetHierarchyData(), float4x4::identity);
	}

	return true;
}

void AnimationComponent::CalculateBoneTransform(HierarchyData& data, float4x4 parentTransform)
{
	std::string nodeName = data.name;
	float4x4 nodeTransform = data.transform;

	Bone* bone = currAnim->anim->FindBone(nodeName);

	if (bone)
	{
		bone->Update(currentTime);
		nodeTransform = bone->GetTransform();
	}

	float4x4 globalTransformation = parentTransform * nodeTransform;

	auto boneInfoMap = owner->GetComponent<MeshComponent>()->GetBoneMap();
	if (boneInfoMap.find(nodeName) != boneInfoMap.end())
	{
		int index = boneInfoMap[nodeName].id;
		float4x4 offset = boneInfoMap[nodeName].offset;
		finalBoneMatrices[index] = globalTransformation * offset;
	}

	for (int i = 0; i < data.childrenCount; i++)
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
		animF.SetNewJsonString(animF.ValueToObject(animF.GetRootValue()), "Path Anim Assets", (*it).anim.get()->GetAssetsPath().c_str());
		animF.SetNewJsonBool(animF.ValueToObject(animF.GetRootValue()), "Loop", (*it).loop);
		
		file.SetValueToArray(newArray, animF.GetRootValue());
	}

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

void AnimationComponent::SetAnimation(std::shared_ptr<Resource> a)
{
	//currAnim = std::static_pointer_cast<Animation>(a);
}

void AnimationComponent::Play(std::string state)
{
	for (int i = 0; i < animations.size(); ++i)
	{
		if (animations[i].state == state)
		{
			if (animQueue.empty())
			{
				currAnim = &animations[i];
				playing = true;
			}
			else
				animQueue.push(currAnim);
			break;
		}
	}

}

void AnimationComponent::GetAnimations()
{
	ImVec2 winPos = ImGui::GetWindowPos();
	ImVec2 size = ImGui::GetWindowSize();
	ImVec2 mouse = ImGui::GetIO().MousePos;
	if (!(mouse.x < winPos.x + size.x && mouse.x > winPos.x &&
		mouse.y < winPos.y + size.y && mouse.y > winPos.y))
	{
		if (ImGui::GetIO().MouseClicked[0]) showAnimMenu = false;
	}

	std::vector<std::string> files;
	app->fs->DiscoverFiles("Library/Animations", files);
	for (std::vector<std::string>::iterator it = files.begin(); it != files.end(); ++it)
	{
		if ((*it).find(".rganim") != std::string::npos)
		{
			app->fs->GetFilenameWithoutExtension(*it);
			*it = (*it).substr((*it).find_last_of("_") + 1, (*it).length());
			uint uid = std::stoll(*it);
			std::shared_ptr<Resource> res = ResourceManager::GetInstance()->GetResource(uid);
			if (ImGui::Selectable(res->GetName().c_str()))
			{
				res->Load();
				//if (currAnim.use_count() - 1 == 1) currAnim->UnLoad();
				SetAnimation(res);
			}
		}
	}
}