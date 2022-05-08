#include "AudioSourceComponent.h"
#include "Globals.h"
#include "AudioManager.h"

#include "GameObject.h"
#include "TransformComponent.h"

#include "Profiling.h"

AudioSourceComponent::AudioSourceComponent(GameObject* own, TransformComponent* trans) : changePosition(true), volume(50.0f), mute(false), transform(trans), pitch(0.0f), playingID(-1)
{
	owner = own;
	type = ComponentType::AUDIO_SOURCE;
	
	AudioManager::Get()->AddAudioSource(this);
	// Register this audio source
	if (!owner->CheckAudioRegister())
	{
		AkGameObjectID cameraID = owner->GetUUID();
		AudioManager::Get()->RegisterGameObject(cameraID);
		owner->SetAudioRegister(true);
	}

	audioClip.push_back({ "None", false});
}

AudioSourceComponent::~AudioSourceComponent()
{
	AudioManager::Get()->DeleteAudioSource(this);
	AudioManager::Get()->UnregisterGameObject(owner->GetUUID());
}

void AudioSourceComponent::OnEditor()
{
	ImGui::PushID(this);
	if (ImGui::CollapsingHeader(ICON_FA_VOLUME_UP" AudioSource"))
	{
		if (ImGui::TreeNodeEx("Clips"))
		{
			for (int i = 0; i < audioClip.size(); ++i)
			{
				ImGui::PushID((void*)audioClip[i].clipName.c_str());
				if (ImGui::BeginCombo("##AudioClip", audioClip[i].clipName.c_str()))
				{
					if (ImGui::Selectable("None"))
					{
						audioClip[i].clipName = "None";
					}
					std::vector<std::string> events = AudioManager::Get()->GetEventsList();
					for (int j = 0; j < events.size(); j++)
					{
						if (ImGui::Selectable(events[j].c_str()))
						{
							audioClip[i].clipName = events[j];
						}
					}
					ImGui::EndCombo();
				}
				ImGui::SameLine();
				ImGui::Checkbox("On Awake", &audioClip[i].playOnAwake);
				ImGui::PopID();
			}

			if (ImGui::Button(ICON_FA_PLUS))
			{
				audioClip.push_back({"None" , false});
			}
			ImGui::SameLine();
			if (ImGui::Button(ICON_FA_MINUS))
			{
				audioClip.erase(audioClip.end()-1);
			}

			ImGui::TreePop();
		}

		ImGui::Text("Mute");
		ImGui::SameLine();
		if (ImGui::Checkbox("##Mute", &mute))
		{
			if (mute) AK::SoundEngine::SetRTPCValue("Volume", 0, owner->GetUUID());
			else AK::SoundEngine::SetRTPCValue("Volume", volume, owner->GetUUID());
			DEBUG_LOG("THE ID IS %d", owner->GetUUID());
		}

		ImGui::Text("Volume");
		ImGui::SameLine();
		if (ImGui::SliderFloat("##Volume", &volume, 0.0f, 100.0f))
		{
			AK::SoundEngine::SetRTPCValue("Volume", volume, owner->GetUUID());
			mute = false;
		}

		ImGui::Text("Pitch");
		ImGui::SameLine();
		if (ImGui::SliderFloat("##Pitch", &pitch, -2400.0f, 2400.0f))
		{
			AK::SoundEngine::SetRTPCValue("Pitch", pitch, owner->GetUUID());
		}

		if (ImGui::Button("Play"))
		{
			PlayClip(audioClip[0].clipName.c_str());
		}
		ImGui::SameLine();
		if (ImGui::Button("Pause"))
		{
			PauseClip(audioClip[0].clipName.c_str());
		}
		ImGui::SameLine();
		if (ImGui::Button("Resume"))
		{
			ResumeClip(audioClip[0].clipName.c_str());
		}
		ImGui::SameLine();
		if (ImGui::Button("Stop"))
		{
			StopClip(audioClip[0].clipName.c_str());
		}

		ComponentOptions(this);
		ImGui::Separator();
	}
	ImGui::PopID();
}

bool AudioSourceComponent::Update(float dt)
{
	RG_PROFILING_FUNCTION("Audio Source Update");

	if (changePosition)
	{
		float3 position = transform->GetPosition();
		AkSoundPosition audioSourcePos;
		audioSourcePos.SetPosition(position.x, position.y, position.z);
		float3 orientation = transform->GetRotation().ToEulerXYZ().Normalized();
		audioSourcePos.SetOrientation({ 0, 0, -1 }, { 0,1,0 });
		AudioManager::Get()->SetPosition(owner->GetUUID(), audioSourcePos);

		AudioManager::Get()->CheckReverbGameObject(owner->GetUUID());

		changePosition = false;
	}

	return true;
}

bool AudioSourceComponent::OnLoad(JsonParsing& node)
{
	// Register this audio source
	if (!owner->CheckAudioRegister())
	{
		AkGameObjectID cameraID = owner->GetUUID();
		AudioManager::Get()->RegisterGameObject(cameraID);
		owner->SetAudioRegister(true);
	}

	JSON_Array* clips = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "AudioClips");
	int size = node.GetJsonArrayCount(clips);
	if (size > 0)
	{
		audioClip.clear();
		for (int i = 0; i < size; ++i)
		{
			JsonParsing clip = node.GetJsonArrayValue(clips, i);
			audioClip.push_back({ clip.GetJsonString(std::to_string(i).c_str()), false });
		}
	}
	volume = node.GetJsonNumber("Volume");
	pitch = node.GetJsonNumber("Pitch");
	mute = node.GetJsonBool("Mute");

	return true;
}

bool AudioSourceComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
	JsonParsing file = JsonParsing();

	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
	//file.SetNewJsonString(file.ValueToObject(file.GetRootValue()), "Audio Clip", audioClip.c_str());
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Volume", volume);
	file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Pitch", pitch);
	file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Mute", mute);

	JsonParsing clips = JsonParsing();
	std::string label = "0";
	JSON_Array* newArray = file.SetNewJsonArray(file.GetRootValue(), "AudioClips");
	for (int i = 0; i < audioClip.size(); ++i)
	{
		clips = JsonParsing();
		clips.SetNewJsonString(clips.ValueToObject(clips.GetRootValue()), label.c_str(), audioClip.at(i).clipName.c_str());
		label = std::to_string(i + 1);
		file.SetValueToArray(newArray, clips.GetRootValue());
	}

	node.SetValueToArray(array, file.GetRootValue());

	return true;
}

void AudioSourceComponent::PlayClip(std::string clipName)
{
	for (int i = 0; i < audioClip.size(); ++i)
	{
		if (audioClip[i].clipName.c_str() == clipName)
		{
			audioClip[i].playingID = AudioManager::Get()->PostEvent(clipName.c_str(), owner->GetUUID());
		}
	}
}

void AudioSourceComponent::PlayClipEverywhere(std::string clipMap)
{
	AudioManager::Get()->PostEvent(clipMap.c_str(), owner->GetUUID());
}

void AudioSourceComponent::PlayClipsOnAwake()
{
	for (int i = 0; i < audioClip.size(); ++i)
	{
		if (audioClip[i].playOnAwake) AudioManager::Get()->PostEvent(audioClip[i].clipName.c_str(), owner->GetUUID());
	}
}

void AudioSourceComponent::StopClip(std::string audioName)
{
	for (int i = 0; i < audioClip.size(); ++i)
	{
		if (audioClip[i].clipName == audioName)
		{
			AK::SoundEngine::StopPlayingID(audioClip[i].playingID);
		}
	}
}

void AudioSourceComponent::PauseClip(std::string audioName)
{
	for (int i = 0; i < audioClip.size(); ++i)
	{
		if (audioClip[i].clipName == audioName)
		{
			AK::SoundEngine::ExecuteActionOnPlayingID(AK::SoundEngine::AkActionOnEventType_Pause, audioClip[i].playingID);
		}
	}
}

void AudioSourceComponent::PauseClip()
{
	for (int i = 0; i < audioClip.size(); ++i)
	{
		AK::SoundEngine::ExecuteActionOnPlayingID(AK::SoundEngine::AkActionOnEventType_Pause, audioClip[i].playingID);
	}
}

void AudioSourceComponent::ResumeClip(std::string audioName)
{
	for (int i = 0; i < audioClip.size(); ++i)
	{
		if (audioClip[i].clipName == audioName)
		{
			AK::SoundEngine::ExecuteActionOnPlayingID(AK::SoundEngine::AkActionOnEventType_Resume, audioClip[i].playingID);
		}
	}
}

void AudioSourceComponent::ResumeClip()
{
	for (int i = 0; i < audioClip.size(); ++i)
	{
		AK::SoundEngine::ExecuteActionOnPlayingID(AK::SoundEngine::AkActionOnEventType_Resume, audioClip[i].playingID);
	}
}

void AudioSourceComponent::SetClipVolume(float vol)
{
	volume = vol;
	AK::SoundEngine::SetRTPCValue("Volume", vol, owner->GetUUID());
}

float AudioSourceComponent::GetClipVolume()
{
	return volume;
}

void AudioSourceComponent::SetState(const char* group, const char* state)
{
	AK::SoundEngine::SetState(group, state);
}
