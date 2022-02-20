#include "Application.h"
#include "Globals.h"
#include "AudioManager.h"
#include "ModuleRenderer3D.h"
#include "ModuleInput.h"
#include "ModuleScene.h"

#include "ConsoleMenu.h"
#include "ConfigurationMenu.h"
#include "MainMenuBar.h"
#include "AboutMenu.h"
#include "InspectorMenu.h"
#include "HierarchyMenu.h"
#include "ContentBrowserMenu.h"
#include "FogWarMenu.h"
#include "Texture.h"
#include "TextureImporter.h"
#include "ResourceManager.h"
#include "ModuleEditor.h"
#include "ModuleCamera3D.h"

#include "Dialogs.h"
#include "IconsFontAwesome5.h"
#include "Style.h"

#include "Profiling.h"

MainMenuBar::MainMenuBar() : Menu(true), saveWindow(false), buttonPlay(nullptr), buttonPause(nullptr), buttonNextFrame(nullptr), buttonStop(nullptr), buttonPauseBlue(nullptr)
{
	showMenu = false;

	menus.reserve(7);
	menus.emplace_back(new ConsoleMenu());
	menus.emplace_back(new ConfigurationMenu());
	menus.emplace_back(new AboutMenu());
	menus.emplace_back(new InspectorMenu());
	menus.emplace_back(new HierarchyMenu());
	menus.emplace_back(new ContentBrowserMenu());
	menus.emplace_back(new FogWarMenu());

	stylesList = { "Deep Dark", "Red & Dark", "Green & Blue", "Classic Dark", "Visual Studio", "Dark Visual", "Gold & Black", "Smooth Dark" };
}

MainMenuBar::~MainMenuBar()
{
}

bool MainMenuBar::Start()
{
	//TextureImporter::ImportTexture(std::string("Assets/Resources/PlayButton.png"));
	//TextureImporter::ImportTexture(std::string("Assets/Resources/PauseButton.png"));
	//TextureImporter::ImportTexture(std::string("Assets/Resources/NextFrame.png"));
	//TextureImporter::ImportTexture2(std::string("Assets/Resources/PauseButtonActive.png"));
	//TextureImporter::ImportTexture2(std::string("Assets/Resources/StopButton.png"));
	
	buttonPlay = new Texture(-5, std::string("Settings/EngineResources/PlayButton.rgtexture"));
	buttonPlay->Load();

	buttonStop = new Texture(-6, std::string("Settings/EngineResources/StopButton.rgtexture"));
	buttonStop->Load();

	buttonPause = new Texture(-7, std::string("Settings/EngineResources/PauseButton.rgtexture"));
	buttonPause->Load();

	buttonPauseBlue = new Texture(-8, std::string("Settings/EngineResources/PauseButtonActive.rgtexture"));
	buttonPauseBlue->Load();

	buttonNextFrame = new Texture(-9, std::string("Settings/EngineResources/NextFrame.rgtexture"));
	buttonNextFrame->Load();

	for (int i = 0; i < menus.size(); ++i)
	{
		menus[i]->Start();
	}

	Style::SetStyle(style);

	return true;
}

bool MainMenuBar::Update(float dt)
{
	if (ImGui::BeginMainMenuBar())
	{
		bool ret = false;

		if (ImGui::BeginMenu(ICON_FA_FILE" File"))
		{
			// Project options (Create, open...)
			if (ImGui::MenuItem(ICON_FA_FILE_UPLOAD" New Project", "Ctrl + N", &ret))
			{
				saveWindow = true;
			}
			if (ImGui::MenuItem(ICON_FA_FOLDER_OPEN" Open Project", "Ctrl + O", &ret))
			{
				std::string filePath = Dialogs::OpenFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
				if (!filePath.empty()) app->scene->LoadScene(filePath.c_str());
			}

			ImGui::Separator();

			if (ImGui::MenuItem(ICON_FA_SAVE" Save", "Ctrl + S", &ret))
			{
				if (app->scene->SceneDirectory().empty())
				{
					std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
					if (!filePath.empty()) app->scene->SaveScene(filePath.c_str());
				}
				else app->scene->SaveScene(app->scene->SceneDirectory().c_str());
			}
			if (ImGui::MenuItem(ICON_FA_SAVE" Save As", "Ctrl + Shift + S", &ret))
			{
				std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
				if (!filePath.empty()) app->scene->SaveScene(filePath.c_str());
			}
			if (ImGui::MenuItem(ICON_FA_WINDOW_CLOSE" Exit", "ESC", &ret))
			{
				return false;
			}
			ImGui::EndMenu();
		}
		if (ImGui::IsItemHovered())
		{
			ImGui::SetTooltip("Opens the file menu");
		}

		if (ImGui::BeginMenu(ICON_FA_EDIT" Edit"))
		{
			ImGui::MenuItem(ICON_FA_UNDO" Undo", "Ctrl + Z", &ret);
			ImGui::MenuItem(ICON_FA_REDO" Redo", "Ctrl + Y", &ret);
			ImGui::EndMenu();
		}
		if (ImGui::IsItemHovered())
		{
			ImGui::SetTooltip("Opens the edit menu");
		}

		if (ImGui::BeginMenu(ICON_FA_WINDOW_RESTORE" Window"))
		{
			ImGui::MenuItem(ICON_FA_WINDOW_MAXIMIZE" Console", NULL, &GetConsole()->active);
			ImGui::MenuItem(ICON_FA_WRENCH" Configuration", NULL, &menus[(int)Menus::CONFIGURATION]->active);
			ImGui::MenuItem(ICON_FA_SITEMAP" Hierarchy", NULL, &menus[(int)Menus::HIERARCHY]->active);
			ImGui::MenuItem(ICON_FA_INFO_CIRCLE" Inspector", NULL, &menus[(int)Menus::INSPECTOR]->active);
			ImGui::MenuItem(ICON_FA_CLOUD" Fog War", NULL, &menus[(int)Menus::FOGWAR]->active);

			ImGui::EndMenu();
		}
		if (ImGui::IsItemHovered())
		{
			ImGui::SetTooltip("Opens the window menu");
		}

		if (ImGui::BeginMenu(ICON_FA_EYE" View"))
		{
			if (ImGui::MenuItem("Depth Test", NULL, app->renderer3D->GetDepthTest()))
			{
				app->renderer3D->SetDepthTest();
			}
			if (ImGui::MenuItem("Cull Face", NULL, app->renderer3D->GetCullFace()))
			{
				app->renderer3D->SetCullFace();
			}
			if (ImGui::MenuItem("Lighting", NULL, app->renderer3D->GetLighting()))
			{
				app->renderer3D->SetLighting();
			}
			if (ImGui::MenuItem("Color Material", NULL, app->renderer3D->GetColorMaterial()))
			{
				app->renderer3D->SetColorMaterial();
			}
			if (ImGui::MenuItem("Texture 2D", NULL, app->renderer3D->GetTexture2D()))
			{
				app->renderer3D->SetTexture2D();
			}
			if (ImGui::MenuItem("Stencil", NULL, app->renderer3D->GetStencil()))
			{
				app->renderer3D->SetStencil();
			}
			if (ImGui::MenuItem("Blending", NULL, app->renderer3D->GetBlending()))
			{
				app->renderer3D->SetBlending();
			}
			if (ImGui::MenuItem("Wire", NULL, app->renderer3D->GetWireMode()))
			{
				app->renderer3D->SetWireMode();
			}
			if (ImGui::MenuItem("Show Raycast", NULL, app->renderer3D->GetRayCast()))
			{
				app->renderer3D->SetWireMode();
			}
			ImGui::EndMenu();
		}
		if (ImGui::IsItemHovered())
		{
			ImGui::SetTooltip("Opens the view menu");
		}

		if (ImGui::BeginMenu(ICON_FA_PLUS" GameObject"))
		{
			if (ImGui::MenuItem(ICON_FA_LAYER_GROUP" Create Empty Object", "Ctrl+Shift+N"))
			{
				if (app->editor->GetGO() != nullptr) app->scene->CreateGameObject(app->editor->GetGO());
				else app->scene->CreateGameObject(nullptr);
			}
			if (ImGui::MenuItem(ICON_FA_OBJECT_UNGROUP" Create Child", "Alt+Shift+N"))
			{
				if (app->editor->GetGO() != nullptr) app->scene->CreateGameObjectChild("GameObjectChild", app->editor->GetGO());
			}
			if (ImGui::MenuItem(ICON_FA_OBJECT_GROUP" Create Parent", "Ctrl+Shift+G"))
			{
				if (app->editor->GetGO() != nullptr) app->scene->CreateGameObjectParent("GameObjectParent", app->editor->GetGO());
			}
			if (ImGui::BeginMenu(ICON_FA_CUBES" Create 3D Object"))
			{
				if (ImGui::MenuItem("Cube"))
				{
					if (app->editor->GetGO() != nullptr) app->scene->Create3DObject(Object3D::CUBE, app->editor->GetGO());
					else app->scene->Create3DObject(Object3D::CUBE, nullptr);
				}
				else if (ImGui::MenuItem("Pyramide"))
				{
					if (app->editor->GetGO() != nullptr) app->scene->Create3DObject(Object3D::PYRAMIDE, app->editor->GetGO());
					else app->scene->Create3DObject(Object3D::PYRAMIDE, nullptr);
				}
				else if (ImGui::MenuItem("Sphere"))
				{
					if (app->editor->GetGO() != nullptr) app->scene->Create3DObject(Object3D::SPHERE, app->editor->GetGO());
					else app->scene->Create3DObject(Object3D::SPHERE, nullptr);
				}
				else if (ImGui::MenuItem("Cylinder"))
				{
					if (app->editor->GetGO() != nullptr) app->scene->Create3DObject(Object3D::CYLINDER, app->editor->GetGO());
					else app->scene->Create3DObject(Object3D::CYLINDER, nullptr);
				}
				ImGui::EndMenu();
			}
			ImGui::Separator();
			if (app->editor->GetGO() == nullptr)
			{
				ImGui::PushStyleColor(ImGuiCol_Text, IM_COL32(128, 128, 128, 255));
				ImGui::PushStyleColor(ImGuiCol_HeaderHovered, IM_COL32(128, 128, 128, 100));
			}
			if (ImGui::MenuItem("Align with view", "Ctrl+Shift+F"))
			{
				if (app->editor->GetGO()) AlignWithView();
			}
			if (ImGui::MenuItem("Align view to selected", "Alt+Shift+F"))
			{
				if (app->editor->GetGO()) AlignViewWithSelected();
			}
			if (app->editor->GetGO() == nullptr)
			{
				ImGui::PopStyleColor();
				ImGui::PopStyleColor();
			}
			ImGui::EndMenu();
		}

		if (ImGui::BeginMenu(ICON_FA_INFO_CIRCLE" Help"))
		{
			ImGui::MenuItem("Demo Menu", NULL, &showMenu);
			ImGui::MenuItem(ICON_FA_USER" About Ragnar Engine", "", &menus[(int)Menus::ABOUT]->active);
			if (ImGui::MenuItem(ICON_FA_ADDRESS_BOOK" Documentation", "F1", &ret))
			{
				app->RequestBrowser("https://github.com/UriKurae/Ragnar-Engine");
			}
			if (ImGui::MenuItem(ICON_FA_BUG" Report a Bug", "", &ret))
			{
				app->RequestBrowser("https://github.com/UriKurae/Ragnar-Engine/issues");
			}
			if (ImGui::MenuItem(ICON_FA_DOWNLOAD" Download latest", "", &ret))
			{
				app->RequestBrowser("https://github.com/UriKurae/Ragnar-Engine/releases");
			}
			ImGui::EndMenu();
		}
		if (ImGui::IsItemHovered())
		{
			ImGui::SetTooltip("Opens the help menu");
		}
		if (ImGui::BeginMenu(ICON_FA_BORDER_STYLE" Set Style"))
		{
			for (int i = 0; i < stylesList.size(); i++)
			{
				if (ImGui::MenuItem(stylesList.at(i).c_str()))
				{
					Style::SetStyle(i);
					style = i;
				}
			}
			ImGui::Separator();

			float auxAlpha = alphaStyle;
			ImGui::Text(ICON_FA_SORT_ALPHA_DOWN_ALT" PopUp Alpha:");
			ImGui::PushItemWidth(100);
			if (ImGui::InputFloat("##Alpha", &alphaStyle, 0.1f))
			{
				if (alphaStyle < auxAlpha)
					Style::SetAlpha(0.9);
				else Style::SetAlpha(1.1);
				alphaStyle = auxAlpha;
			}
			ImGui::PopItemWidth();

			ImGui::EndMenu();
		}
		ImGui::EndMainMenuBar();
	}

	if (saveWindow)
	{
		bool saved = true;
		ImVec2 size = { 200, 100 };
		ImVec2 position = { (float)(app->window->width / 2) - 100, (float)(app->window->height / 2) - 50 };
		ImGui::SetNextWindowPos(position);
		ImGui::Begin("Ask for Save", &saved, ImGuiWindowFlags_NoCollapse | ImGuiWindowFlags_NoDocking | ImGuiWindowFlags_NoTitleBar | ImGuiWindowFlags_NoResize);
		ImGui::TextWrapped("Do you want to save the changes you made in: ");

		std::string dir = app->scene->SceneDirectory();
		if (!dir.empty())
		{
			dir = dir.substr(dir.find("Output\\") + 7, dir.length());
		}
		ImGui::TextWrapped("%s", dir.empty() ? "Untitled" : dir.c_str());
		ImGui::NewLine();
		if (ImGui::Button("Save"))
		{
			if (app->scene->SceneDirectory().empty())
			{
				std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
				if (!filePath.empty()) app->scene->SaveScene(filePath.c_str());
			}
			else
			{
				app->scene->SaveScene(app->scene->SceneDirectory().c_str());
			}
			app->scene->NewScene();
			saveWindow = false;
		}
		ImGui::SameLine();
		if (ImGui::Button("Don't Save"))
		{
			app->scene->NewScene();
			saveWindow = false;
		}
		ImGui::SameLine();
		if (ImGui::Button("Cancel")) saveWindow = false;
		ImGui::End();
	}

	bool ret = true;
	ImGui::Begin(" ", &ret, ImGuiWindowFlags_NoScrollbar | ImGuiWindowFlags_NoDecoration | ImGuiWindowFlags_NoResize | ImGuiWindowFlags_NoTitleBar);
	ImGui::PushStyleColor(ImGuiCol_Button, { 0, 0, 0, 0 });
	ImGui::PushStyleColor(ImGuiCol_Border, { 0, 0, 0, 0 });
	ImGui::PushStyleColor(ImGuiCol_BorderShadow, { 0, 0, 0, 0 });
	ImGui::SameLine(ImGui::GetWindowSize().x * 0.5f - 81);
	
	if (app->scene->GetGameState() == GameState::NOT_PLAYING)
	{
		if (ImGui::ImageButton((ImTextureID)buttonPlay->GetId(), { 27,18 }))
		{
			app->scene->Play();
			AudioManager::Get()->PlayAllAudioSources();
			ImGui::StyleColorsClassic();
		}

		ImGui::SameLine();
		ImGui::ImageButton((ImTextureID)buttonPause->GetId(), { 27,18 });

		ImGui::SameLine();
		ImGui::ImageButton((ImTextureID)buttonNextFrame->GetId(), { 27,18 });

	}
	else if (app->scene->GetGameState() == GameState::PLAYING || app->scene->GetGameState() == GameState::PAUSE)
	{
		if (ImGui::ImageButton((ImTextureID)buttonStop->GetId(), { 27,18 }))
		{
			AudioManager::Get()->StopAllAudioSources();
			app->scene->Stop();
			//StyleTheme();
		}
		ImGui::SameLine();

		if (app->scene->GetGameState() == GameState::PAUSE)
		{
			if (ImGui::ImageButton((ImTextureID)buttonPauseBlue->GetId(), { 27,18 }))
			{
				app->scene->Resume();
				AudioManager::Get()->ResumeAllAudioSources();
			}
		}
		else if (ImGui::ImageButton((ImTextureID)buttonPause->GetId(), { 27,18 }))
		{
			AudioManager::Get()->PauseAllAudioSources();
			app->scene->Pause();
		}

		ImGui::SameLine();
		if (ImGui::ImageButton((ImTextureID)buttonNextFrame->GetId(), { 27,18 })) if (app->scene->GetGameState() == GameState::PAUSE) app->scene->NextFrame();

	}

	if (app->input->GetKey(SDL_SCANCODE_LCTRL) == KeyState::KEY_REPEAT &&
		app->input->GetKey(SDL_SCANCODE_LSHIFT) != KeyState::KEY_REPEAT && 
		app->input->GetKey(SDL_SCANCODE_N) == KeyState::KEY_DOWN)
	{
		saveWindow = true;
	}

	if (app->input->GetKey(SDL_SCANCODE_LCTRL) == KeyState::KEY_REPEAT &&
		app->input->GetKey(SDL_SCANCODE_O) == KeyState::KEY_DOWN)
	{
		std::string filePath = Dialogs::OpenFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
		if (!filePath.empty())
		{
			app->scene->LoadScene(filePath.c_str());
		}
	}

	if (app->input->GetKey(SDL_SCANCODE_LSHIFT) == KeyState::KEY_REPEAT)
	{
		if (app->input->GetKey(SDL_SCANCODE_LCTRL) == KeyState::KEY_REPEAT)
		{
			if (app->input->GetKey(SDL_SCANCODE_S) == KeyState::KEY_DOWN)
			{
				std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
				if (!filePath.empty()) app->scene->SaveScene(filePath.c_str());
			}
			if (app->input->GetKey(SDL_SCANCODE_F) == KeyState::KEY_DOWN)
			{
				AlignWithView();
			}
			if (app->input->GetKey(SDL_SCANCODE_G) == KeyState::KEY_DOWN)
			{
				if (app->editor->GetGO() != nullptr) app->scene->CreateGameObjectParent("GameObjectParent", app->editor->GetGO());
			}
			if (app->input->GetKey(SDL_SCANCODE_N) == KeyState::KEY_DOWN)
			{
				if (app->editor->GetGO() != nullptr) app->scene->CreateGameObject(app->editor->GetGO());
				else app->scene->CreateGameObject(nullptr);
			}
		}
		else if (app->input->GetKey(SDL_SCANCODE_LALT) == KeyState::KEY_REPEAT)
		{
			if (app->input->GetKey(SDL_SCANCODE_F) == KeyState::KEY_DOWN)
			{
				AlignViewWithSelected();
			}
			if (app->input->GetKey(SDL_SCANCODE_N) == KeyState::KEY_DOWN)
			{
				if (app->editor->GetGO() != nullptr) app->scene->CreateGameObjectChild("GameObjectChild", app->editor->GetGO());
			}
		}
	}
	
	else if (app->input->GetKey(SDL_SCANCODE_LCTRL) == KeyState::KEY_REPEAT &&
		app->input->GetKey(SDL_SCANCODE_S) == KeyState::KEY_DOWN)
	{
		if (app->scene->SceneDirectory().empty())
		{
			std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
			if (!filePath.empty()) app->scene->SaveScene(filePath.c_str());
		}
		else app->scene->SaveScene(app->scene->SceneDirectory().c_str());
	}

	else if (app->input->GetKey(SDL_SCANCODE_LCTRL) == KeyState::KEY_REPEAT &&
		app->input->GetKey(SDL_SCANCODE_LSHIFT) == KeyState::KEY_REPEAT)
	{
		
	}

	ImGui::PopStyleColor(3);
	ImGui::End();

	if (showMenu)
	{
		ImGui::ShowDemoWindow(&showMenu);
		ImGui::ShowMetricsWindow(&showMenu);
	}

	for (unsigned int i = 0; i < menus.size(); ++i)
	{
		if (menus[i]->active) menus[i]->Update(dt);
	}

	return true;
}

bool MainMenuBar::CleanUp()
{
	for (unsigned int i = 0; i < menus.size(); ++i)
	{
		RELEASE(menus[i]);
	}

	RELEASE(buttonPlay);
	RELEASE(buttonStop);
	RELEASE(buttonPause);
	RELEASE(buttonPauseBlue);
	RELEASE(buttonNextFrame);

	menus.clear();

	return true;
}

ConsoleMenu* MainMenuBar::GetConsole()
{
	if (!menus.empty())
	{
		ConsoleMenu* test = (ConsoleMenu*)menus[(int)Menus::CONSOLE];

		return test;
	}

	return nullptr;
}

std::string& MainMenuBar::GetCurrentDir()
{
	ContentBrowserMenu* content = (ContentBrowserMenu*)menus[(int)Menus::CONTENT_BROWSER];

	return content->GetCurrentDir();
}

void MainMenuBar::SetStyle(int _style)
{
	style = _style; 
	Style::SetStyle(style);
}

// Object align with camera
void MainMenuBar::AlignWithView()
{
	GameObject* temp = app->editor->GetGO();
	if (temp != nullptr)
	{
		TransformComponent* transform = temp->GetComponent<TransformComponent>();
		float4x4 matrix = transform->GetGlobalTransform();
		Frustum frus = app->camera->cameraFrustum;
		matrix.SetTranslatePart(frus.Pos());
		float3x3 rot{ frus.WorldRight(), frus.Up(), frus.Front() };
		matrix.SetRotatePart(rot.ToQuat());
		transform->SetTransform(matrix);
	}
}
// Camera align with object
void MainMenuBar::AlignViewWithSelected()
{
	GameObject* temp = app->editor->GetGO();
	if (temp != nullptr)
	{
		TransformComponent* transform = temp->GetComponent<TransformComponent>();
		float4x4 matrix = transform->GetGlobalTransform();
		float3x3 rot = matrix.RotatePart();
		app->camera->cameraFrustum.SetFrame(transform->GetGlobalTransform().Col3(3), rot.Col3(2), rot.Col3(1));
	}
}
