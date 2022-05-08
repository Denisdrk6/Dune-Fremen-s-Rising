#pragma once
#include "Component.h"

class TransformComponent;
typedef unsigned int uint;

struct AudioClip
{
	std::string clipName;
	bool playOnAwake;
	uint playingID;
};

class AudioSourceComponent : public Component
{
public:
	AudioSourceComponent(GameObject* own, TransformComponent* trans);
	~AudioSourceComponent();

	void OnEditor() override;
	bool Update(float dt) override;

	bool OnLoad(JsonParsing& node) override;
	bool OnSave(JsonParsing& node, JSON_Array* array) override;

	void PlayClip(std::string clipMap);
	void PlayClipsOnAwake();
	void StopClip(std::string audioName);
	void PauseClip(std::string audioName);
	void PauseClip();
	void ResumeClip(std::string audioName);
	void ResumeClip();
	void SetClipVolume(float vol);
	float GetClipVolume();
	void SetState(const char* group, const char* state);

	inline void ChangePosition() { changePosition = true; }
private:
	bool changePosition;
	TransformComponent* transform;

	// Audio settings
	std::vector<AudioClip> audioClip;
	unsigned int playingID;
	bool mute;
	float volume;
	float pitch;
};