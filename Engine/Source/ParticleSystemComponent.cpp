#include "ParticleSystemComponent.h"
#include "BillboardParticleComponent.h"

#include "ModuleSceneManager.h"
#include "Scene.h"

#include "GameObject.h"
#include "FileSystem.h"
#include "Globals.h"
#include "Geometry/Sphere.h"
#include "MeshComponent.h"

#include "Profiling.h"

ParticleSystemComponent::ParticleSystemComponent(GameObject* own, TransformComponent* trans, uint numParticles)
{
	type = ComponentType::PARTICLE_SYSTEM;
	transform = trans;
	active = true; // Component is active
    isActive = false; // Simulation is active
    saveConfig = false;
    loadConfig = false;

    if (own->GetComponent<MeshComponent>())
    {
        Sphere s(trans->GetPosition(), 5);
        own->SetAABB(AABB(s));
    }
    sizeAABB = { 10,10,10 };
    offsetAABB = { 0,0,0 };

    if (own->GetComponent<BillboardParticleComponent>() == nullptr)
        own->CreateComponent(ComponentType::BILLBOARD);
}

ParticleSystemComponent::~ParticleSystemComponent()
{
	for (int i = 0; i < emitters.size(); i++)
	{
		delete emitters[i];
	}
	emitters.clear();
}

void ParticleSystemComponent::SetEmitter(ParticleEmitter* emitter)
{
	emitters.push_back(emitter);
}

bool ParticleSystemComponent::Update(float dt)
{
    RG_PROFILING_FUNCTION("Particle System Update");

    offsetAABB = transform->GetGlobalPosition();
    transform->ForceUpdateTransform();

    if (isActive || app->sceneManager->GetGameState() != GameState::NOT_PLAYING)
    {
        if (((float)timer.GetTime()) / 1000.0f < maxDuration || looping == true)
        {
            for (int i = 0; i < emitters.size(); i++)
            {
                emitters[i]->Emit(dt);
                emitters[i]->Update(dt);
            }
        }
    }

    for (std::vector<ParticleEmitter*>::iterator it = emitters.begin(); it != emitters.end(); ++it)
    {
        if ((*it)->toDelete)
        {
            RELEASE((*it));
            emitters.erase(it);
            break;
        }
    }

    if (isActive)
    {
        if ((float)timer.GetTime() / 1000.0f >= maxDuration && looping != true)
        {
            isActive = false;
            for (std::vector<ParticleEmitter*>::iterator it = emitters.begin(); it != emitters.end(); ++it)
            {
                (*it)->RestartEmitter();
            }
            timer.Stop();
        }
    }

    return true;
}

void ParticleSystemComponent::Draw(CameraComponent* gameCam)
{
	if (isActive || app->sceneManager->GetGameState() != GameState::NOT_PLAYING)
		for (auto& e : emitters)
			e->Render(gameCam);
}

void ParticleSystemComponent::OnEditor()
{
    if (ImGui::CollapsingHeader("Particle System"))
    {
        if (ImGui::Button(ICON_FA_SAVE" Save Configuration"))
            saveConfig = !saveConfig;

        if (ImGui::Button("Load Configuration"))
            loadConfig = !loadConfig;

        if (saveConfig)
            SaveConfiguration();

        if (loadConfig)
            LoadConfiguration();

        if (ImGui::Checkbox("Active", &active));

        ImGui::PushItemWidth(90);

        std::string playButtonName = isActive ? "Pause" : "Play";
        if (ImGui::Button(playButtonName.c_str()))
        {
            isActive = !isActive;

            if (isActive)
            {
                Play();
            }
                
            else if (!isActive)
                timer.Stop();
        }

        ImGui::SameLine();
        ImGui::Text("Played for: %.2f", timer.ReadTime() * 0.001f);

        ImGui::Checkbox("Looping", &looping);
        ImGui::SliderFloat("Duration", &maxDuration, 0.0f, 10.0f);

        ImGui::Spacing();
        ImGui::Spacing();

        ImGui::Text("Box Offset: ");
        ImGui::PushItemWidth(300);
        if (ImGui::DragFloat3("##Offset", &offsetAABB.x, 0.1f))
            owner->EditAABB(offsetAABB, sizeAABB);
        ImGui::PopItemWidth();

        ImGui::Text("Box Size: ");
        ImGui::PushItemWidth(300);
        if (ImGui::DragFloat3("##Box", &sizeAABB.x, 0.1f, 0.1f, INFINITE))
            owner->EditAABB(offsetAABB, sizeAABB);
        ImGui::PopItemWidth();

        ImGui::Checkbox("Show AABB     ", &owner->showAABB);
        ImGui::SameLine();
        ImGui::Checkbox("Show OBB", &owner->showOBB);

        ImGui::Spacing();
        ImGui::Spacing();

        if (ImGui::Button(ICON_FA_PLUS" Create Emitter")) {
            emitters.push_back(new ParticleEmitter(owner));
        }

        //ImGui::Spacing();
        std::string guiName = "";
        std::string suffixLabel = "";
        for (int i = 0; i < emitters.size(); ++i)
        {
            suffixLabel = "##";
            suffixLabel += i;
            ImGui::PushID(suffixLabel.c_str());
            ImGui::Separator();
            //ImGui::Spacing();

            emitters[i]->OnEditor(i);
            ImGui::PopID();
        }

        ImGui::Spacing();
        ImGui::Spacing();
        ImGui::Separator();
        ComponentOptions(this);
        ImGui::Separator();
    }
}

void ParticleSystemComponent::UpdateAABB()
{
    owner->EditAABB(offsetAABB, sizeAABB);
}

void ParticleSystemComponent::Play()
{
    timer.Start();
    isActive = true;
    for (int i = 0; i < emitters.size(); ++i)
    {
        emitters[i]->isActive = true;
        // TODO: emitters[i]->RestartEmitter();
    }
}

void ParticleSystemComponent::Stop()
{
    timer.Stop();
    isActive = false;
    for (int i = 0; i < emitters.size(); ++i)
    {
        emitters[i]->isActive = false;
        // TODO: emitters[i]->RestartEmitter();
    }
}

void ParticleSystemComponent::SaveConfiguration()
{
    ImGui::Separator();

    const char extension[13] = ".rgparticles";
    static char name[25] = "\0";

    ImGui::Text("Name: "); ImGui::SameLine();

    std::string id("##");
    id += extension;

    ImGui::InputText(id.c_str(), name, sizeof(char) * 25);
    if (ImGui::Button("Create"))
    {
        std::string nameFile = name;
        std::string path = PARTICLES_FOLDER;
        path += name;

        if (path.find('.') == path.npos)
            path += extension;

        //TODO: Check if the extension is correct, to avoid a .cs.glsl file
        if (path.find(extension) != path.npos)
        {
            bool nameDone = false;
            int count = 0;
            while (!nameDone)
            {
                if (app->fs->Exists(path.c_str()) != 0)
                {
                    count++;
                    nameFile = nameFile.substr(0, nameFile.find_last_of("(")) + "(" + std::to_string(count) + ")";
                    path = PARTICLES_FOLDER;
                    path += nameFile + extension;
                }
                else
                {
                    nameDone = true;
                }
            }

            JsonParsing particuleFile = JsonParsing();
            particuleFile = particuleFile.SetChild(particuleFile.GetRootValue(), "Particule");

            SaveConfig(particuleFile);

            char* buf;
            uint size = particuleFile.Save(&buf);

            if (app->fs->Save(path.c_str(), buf, size) > 0)
                DEBUG_LOG("Particle Configuration saved succesfully");
            else
                DEBUG_LOG("Particle Configuration couldn't be saved");

            RELEASE_ARRAY(buf);
            name[0] = '\0';
        }

        saveConfig = false;
        ImGui::CloseCurrentPopup();
    }
    ImGui::SameLine();
    if (ImGui::Button("Cancel"))
    {
        saveConfig = false;
        ImGui::CloseCurrentPopup();
    }

    ImGui::Separator();
}

void ParticleSystemComponent::LoadConfiguration()
{
    ImGui::Separator();

    std::string guiName = "Configuration";
    std::vector<std::string> files;
    app->fs->DiscoverFiles(PARTICLES_FOLDER, files);

    if (ImGui::BeginCombo(guiName.c_str(), "Select Configuration"))
    {
        std::string name;

        for (int i = 0; i < files.size(); ++i)
        {
            name = files[i];
            app->fs->GetFilenameWithoutExtension(name);
            guiName = name;

            if (ImGui::Selectable(guiName.c_str()))
            {
                std::string path = PARTICLES_FOLDER + files[i];

                JsonParsing particleFile = JsonParsing();

                if (particleFile.ParseFile(path.c_str()) > 0)
                {
                    LoadConfig(particleFile);
                }

                loadConfig = false;
            }
        }
        ImGui::EndCombo();
    }

    ImGui::Separator();
}

bool ParticleSystemComponent::OnLoad(JsonParsing& node)
{
    sizeAABB = node.GetJson3Number(node, "Size");
    offsetAABB = node.GetJson3Number(node, "Offset");
    owner->EditAABB(offsetAABB, sizeAABB);
    JSON_Array* emittersArray = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "Emitters");
    size_t size = node.GetJsonArrayCount(emittersArray);
    maxDuration = node.GetJsonNumber("MaxDuration");
    looping = node.GetJsonBool("Looping");

    for (int i = 0; i < size; ++i)
    {
        JsonParsing file = node.GetJsonArrayValue(emittersArray, i);
        ParticleEmitter* emitter = new ParticleEmitter(owner);
        emitter->OnLoad(file);
        emitters.push_back(emitter);
    }

    //for (auto& emitter : emitters)
    //    emitter->OnLoad(node);
	return true;
}

bool ParticleSystemComponent::OnSave(JsonParsing& node, JSON_Array* array)
{
    JsonParsing file = JsonParsing();

    file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "Type", (int)type);
    file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Active", active);
    file.SetNewJson3Number(file, "Size", sizeAABB);
    file.SetNewJson3Number(file, "Offset", offsetAABB);
    file.SetNewJsonNumber(file.ValueToObject(file.GetRootValue()), "MaxDuration", maxDuration);
    file.SetNewJsonBool(file.ValueToObject(file.GetRootValue()), "Looping", looping);

    JSON_Array* emittersArray = file.SetNewJsonArray(file.GetRootValue(), "Emitters");

    for (auto& emitter : emitters)
        emitter->OnSave(node, emittersArray);

    node.SetValueToArray(array, file.GetRootValue());

	return true;
}

void ParticleSystemComponent::SaveConfig(JsonParsing& node)
{
    node.SetNewJson3Number(node, "Size", sizeAABB);
    node.SetNewJson3Number(node, "Offset", offsetAABB);
    JSON_Array* emittersArray = node.SetNewJsonArray(node.GetRootValue(), "Emitters");

    for (auto& emitter : emitters)
        emitter->OnSave(node, emittersArray);
}

void ParticleSystemComponent::LoadConfig(JsonParsing& node)
{
    sizeAABB = node.GetJson3Number(node, "Size");
    offsetAABB = node.GetJson3Number(node, "Offset");
    owner->EditAABB(offsetAABB, sizeAABB);
    JSON_Array* emittersArray = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "Emitters");
    size_t size = node.GetJsonArrayCount(emittersArray);

    for (int i = 0; i < emitters.size(); ++i)
    {
        RELEASE(emitters[i]);
    }
    emitters.clear();

    for (int i = 0; i < size; ++i)
    {
        JsonParsing file = node.GetJsonArrayValue(emittersArray, i);
        ParticleEmitter* emitter = new ParticleEmitter(owner);
        emitter->OnLoad(file);
        emitters.push_back(emitter);
    }
}
