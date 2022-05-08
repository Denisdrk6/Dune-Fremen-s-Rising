#include "ModuleEditor.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleWindow.h"
#include "ModuleInput.h"
#include "ModuleSceneManager.h"
#include "Scene.h"

#include "ConsoleMenu.h"
#include "InspectorMenu.h"

#include "Viewport.h"
#include "GameView.h"

#include "Imgui/imgui_impl_opengl3.h"
#include "Imgui/imgui_impl_sdl.h"

#include "Profiling.h"

ModuleEditor::ModuleEditor() : selected(nullptr), selectedParent(nullptr), Module(), resource(nullptr)
{
	name = "Editor";

	viewport = new Viewport();
	gameView = new GameView();
}

ModuleEditor::~ModuleEditor()
{
}

bool ModuleEditor::Start()
{
	mainMenuBar.Start();

	ImGuiIO& io = ImGui::GetIO();
	io.Fonts->AddFontDefault();

	// merge in icons from Font Awesome
	static const ImWchar icons_ranges[] = { ICON_MIN_FA, ICON_MAX_FA, 0 };
	ImFontConfig icons_config; icons_config.MergeMode = true; icons_config.PixelSnapH = true;
	io.Fonts->AddFontFromFileTTF(FONT_ICON_FILE_NAME_FAS, 12.0f, &icons_config, icons_ranges);
	// use FONT_ICON_FILE_NAME_FAR if you want regular instead of solid

	return true;
}

bool ModuleEditor::Update(float dt)
{
	RG_PROFILING_FUNCTION("Updating Module Editor");
	bool ret = true;

	ImGui_ImplOpenGL3_NewFrame();
	ImGui_ImplSDL2_NewFrame();
	ImGui::NewFrame();
	ImGuizmo::BeginFrame();
	
	if (app->input->GetMouseButton(SDL_BUTTON_RIGHT) == KeyState::KEY_IDLE && selected)
	{
		if (app->input->GetKey(SDL_SCANCODE_W) == KeyState::KEY_UP) viewport->SetSnap(ImGuizmo::OPERATION::TRANSLATE);
		else if (app->input->GetKey(SDL_SCANCODE_E) == KeyState::KEY_UP) viewport->SetSnap(ImGuizmo::OPERATION::ROTATE);
		else if (app->input->GetKey(SDL_SCANCODE_R) == KeyState::KEY_UP) viewport->SetSnap(ImGuizmo::OPERATION::SCALE);
	}


	ImGui::DockSpaceOverViewport();
	ret = mainMenuBar.Update(dt);

	if (app->input->GetKey(SDL_SCANCODE_LALT) == KeyState::KEY_REPEAT &&
		app->input->GetKey(SDL_SCANCODE_RETURN) == KeyState::KEY_DOWN)
	{
		app->window->SetFullscreen();
	}
	
	if (app->input->GetKey(SDL_SCANCODE_DELETE) == KeyState::KEY_UP)
	{
		if (selected && selected->GetComponent<CameraComponent>() == nullptr)
		{
			selected->GetParent()->GetChilds().erase(selected->GetParent()->FindChildren(selected));
			app->sceneManager->GetCurrentScene()->ResetQuadtree();
			RELEASE(selected);
		}
	}

	if (app->input->GetKey(SDL_SCANCODE_LCTRL) == KeyState::KEY_REPEAT &&
		app->input->GetKey(SDL_SCANCODE_D) == KeyState::KEY_DOWN)
	{
		if (selected) app->sceneManager->GetCurrentScene()->DuplicateGO(selected, selected->GetParent());
	}

	return ret;
}

bool ModuleEditor::Draw(Framebuffer* editorBuffer, Framebuffer* gameBuffer)
{
	RG_PROFILING_FUNCTION("Drawing Module Editor");
	
	gameView->Draw(gameBuffer);
	viewport->Draw(editorBuffer, gameBuffer);
	ImGui::EndFrame();
	ImGui::Render();
	ImGui_ImplOpenGL3_RenderDrawData(ImGui::GetDrawData());

	return true;
}

bool ModuleEditor::CleanUp()
{
#ifndef DIST
	mainMenuBar.CleanUp();
#endif

	RELEASE(viewport);
	RELEASE(gameView);

	return true;
}

void ModuleEditor::SetResource(Resource* res)
{
	selected = nullptr;
	resource = res;
}

void ModuleEditor::SetGO(GameObject* obj)
{
	resource = nullptr;
	selected = obj;
}

bool ModuleEditor::LoadConfig(JsonParsing& node)
{
	std::vector<std::string> tags;
	std::vector<std::string> layers;

	// Load Tags
	JSON_Array *jsonArray = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "tags");
	size_t size = node.GetJsonArrayCount(jsonArray);

	for (int i = 0; i < size; ++i)
	{
		JsonParsing tag = node.GetJsonArrayValue(jsonArray, i);
		tags.push_back(tag.GetJsonString(std::to_string(i).c_str()));
	}
	dynamic_cast<InspectorMenu*>(mainMenuBar.GetMenus().at(mainMenuBar.GetMenus().size()-1))->SetTags(tags);
	
	// Load Layers
	jsonArray = node.GetJsonArray(node.ValueToObject(node.GetRootValue()), "layers");
	size = node.GetJsonArrayCount(jsonArray);

	for (int i = 0; i < size; ++i)
	{
		JsonParsing lay = node.GetJsonArrayValue(jsonArray, i);
		layers.push_back(lay.GetJsonString(std::to_string(i).c_str()));
	}
	dynamic_cast<InspectorMenu*>(mainMenuBar.GetMenus().at(mainMenuBar.GetMenus().size() - 1))->SetLayers(layers);

	// Load style
	mainMenuBar.SetStyle((int)node.GetJsonNumber("style"));

	return true;
}

bool ModuleEditor::SaveConfig(JsonParsing& node)
{
	JsonParsing fileTag = JsonParsing();

	// Save Tags
	JSON_Array* jsonArray = node.SetNewJsonArray(node.GetRootValue(), "tags");
	std::string label = "0";
	std::vector<std::string> tags = GetTags();
	for (int i = 0; i < tags.size(); i++)
	{
		fileTag = JsonParsing();
		fileTag.SetNewJsonString(fileTag.ValueToObject(fileTag.GetRootValue()), label.c_str(), tags.at(i).c_str());
		label = std::to_string(i + 1);
		node.SetValueToArray(jsonArray, fileTag.GetRootValue());
	}

	// Save layers
	JsonParsing fileLay = JsonParsing();
	jsonArray = node.SetNewJsonArray(node.GetRootValue(), "layers");
	label = "0";
	std::vector<std::string> layers = GetLayers();
	for (int i = 0; i < layers.size(); i++)
	{
		fileLay = JsonParsing();
		fileLay.SetNewJsonString(fileLay.ValueToObject(fileLay.GetRootValue()), label.c_str(), layers.at(i).c_str());
		label = std::to_string(i + 1);
		node.SetValueToArray(jsonArray, fileLay.GetRootValue());
	}
	// Save Style
	node.SetNewJsonNumber(node.ValueToObject(node.GetRootValue()), "style", mainMenuBar.GetStyle());

	return true;
}

void ModuleEditor::LogConsole(const char* string)
{
	if (mainMenuBar.GetConsole()) mainMenuBar.GetConsole()->AddLog(string);
}

std::vector<std::string> ModuleEditor::GetTags()
{
	return dynamic_cast<InspectorMenu*>(mainMenuBar.GetMenus().at(mainMenuBar.GetMenus().size() - 1))->GetTags();
}

std::vector<std::string> ModuleEditor::GetLayers()
{
	return dynamic_cast<InspectorMenu*>(mainMenuBar.GetMenus().at(mainMenuBar.GetMenus().size() - 1))->GetLayers();
}
