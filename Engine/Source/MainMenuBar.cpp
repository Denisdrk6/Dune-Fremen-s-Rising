#include "MainMenuBar.h"
#include "Application.h"
#include "Globals.h"

#include "ModuleRenderer3D.h"
#include "ModuleSceneManager.h"
#include "Scene.h"
#include "ModuleEditor.h"
#include "Physics3D.h"

#include "ConsoleMenu.h"
#include "ConfigurationMenu.h"
#include "AboutMenu.h"
#include "InspectorMenu.h"
#include "HierarchyMenu.h"
#include "ContentBrowserMenu.h"
#include "TextEditorMenu.h"
#include "FogWarMenu.h"
#include "InputActionMenu.h"
#include "NavigatorMenu.h"

#include "TransformComponent.h"
#include "LightComponent.h"
#include "ButtonComponent.h"
#include "ImageComponent.h"
#include "CheckBoxComponent.h"
#include "SliderComponent.h"
#include "TextComponent.h"
#include "Transform2DComponent.h"

#include "CommandsDispatcher.h"
#include "ResourceManager.h"
#include "AudioManager.h"
#include "FileSystem.h"
#include "DialogueSystem.h"

#include "Lights.h"
#include "Texture.h"
#include "Dialogs.h"
#include "Viewport.h"
#include "Style.h"

#include "Math/float3x3.h"

#include "Profiling.h"

MainMenuBar::MainMenuBar() : Menu(true, "MainMenu")
{
	showMenu = false;

	menus.reserve(10);
	menus.emplace_back(new ConsoleMenu());
	menus.emplace_back(new ConfigurationMenu());
	menus.emplace_back(new AboutMenu());
	menus.emplace_back(new HierarchyMenu());
	menus.emplace_back(new ContentBrowserMenu());
	menus.emplace_back(new TextEditorMenu());
	menus.emplace_back(new FogWarMenu());
	menus.emplace_back(new InputActionMenu());
	menus.emplace_back(new NavigatorMenu());
	menus.emplace_back(new InspectorMenu()); // Inspector must be the LAST!!!

	stylesList = { "Deep Dark", "Red & Dark", "Green & Blue", "Classic Dark", "Visual Studio", "Dark Visual", "Gold & Black", "Smooth Dark" };
	iconList = { ICON_FA_WINDOW_MAXIMIZE, ICON_FA_WRENCH, ICON_FA_SITEMAP, ICON_FA_SITEMAP, ICON_FA_SITEMAP, ICON_FA_CODE, ICON_FA_CLOUD, ICON_FA_KEYBOARD, ICON_FA_WALKING, ICON_FA_INFO_CIRCLE };
}

MainMenuBar::~MainMenuBar()
{
}

bool MainMenuBar::Start()
{
	for (int i = 0; i < menus.size(); ++i)
	{
		menus[i]->Start();
	}
	Style::SetStyle(style);

	return true;
}

bool MainMenuBar::Update(float dt)
{
	RG_PROFILING_FUNCTION("Main Menu Bar Update");

	if (ImGui::BeginMainMenuBar())
	{
		if (!FileMenu()) return false;		
		EditMenu();
		WindowMenu();
		ViewMenu();	
		GameObjectMenu();
		HelpMenu();
		SetStyleMenu();		
		
		ImGui::EndMainMenuBar();
	}

	PlayBar();

	if (showMenu)
	{
		ImGui::ShowDemoWindow(&showMenu);
		ImGui::ShowMetricsWindow(&showMenu);
	}

	for (unsigned int i = 0; i < menus.size(); ++i)
	{
		if (menus[i]->active) menus[i]->Update(dt);
	}

	if (DialogueSystem::GetInstance()->createDialogue)
		DialogueSystem::GetInstance()->OnEditor();

	return true;
}

bool MainMenuBar::FileMenu()
{
	if (ImGui::BeginMenu(ICON_FA_FILE" File"))
	{
		// Project options (Create, open...)
		if (ImGui::MenuItem(ICON_FA_FILE_UPLOAD" New Project", "Ctrl + N"))
			app->sceneManager->saveScene = true;

		if (ImGui::MenuItem(ICON_FA_FOLDER_OPEN" Open Project", "Ctrl + O"))
		{
			std::string filePath = Dialogs::OpenFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
			if (!filePath.empty()) app->sceneManager->GetCurrentScene()->LoadScene(filePath.c_str());
		}

		ImGui::Separator();

		if (ImGui::MenuItem(ICON_FA_SAVE" Save", "Ctrl + S"))
		{
			if (app->sceneManager->GetCurrentScene()->GetAssetsPath().empty())
			{
				std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
				if (!filePath.empty()) app->sceneManager->GetCurrentScene()->SaveScene(filePath.c_str());
			}
			else app->sceneManager->GetCurrentScene()->SaveScene(app->sceneManager->GetCurrentScene()->GetAssetsPath().c_str());
		}
		if (ImGui::MenuItem(ICON_FA_SAVE" Save As", "Ctrl + Shift + S"))
		{
			std::string filePath = Dialogs::SaveFile("Ragnar Scene (*.ragnar)\0*.ragnar\0");
			if (!filePath.empty()) app->sceneManager->GetCurrentScene()->SaveScene(filePath.c_str());
		}

		ImGui::Separator();

		if (ImGui::MenuItem("Build"))
			app->sceneManager->showBuildMenu = true;

		if (ImGui::MenuItem(ICON_FA_WINDOW_CLOSE" Exit", "ESC"))
			return false;

		ImGui::EndMenu();
	}
	if (ImGui::IsItemHovered())
	{
		ImGui::SetTooltip("Opens the file menu");
	}

	return true;
}

void MainMenuBar::EditMenu()
{
	if (ImGui::BeginMenu(ICON_FA_EDIT" Edit"))
	{
		if(ImGui::MenuItem(ICON_FA_UNDO" Undo", "Ctrl + Z"))
			CommandDispatcher::Undo();
		if(ImGui::MenuItem(ICON_FA_REDO" Redo", "Ctrl + Y"))
			CommandDispatcher::Redo();

		ImGui::Separator();
		app->editor->GetViewport()->SnapOptions();
		ImGui::EndMenu();
	}
	if (ImGui::IsItemHovered())
	{
		ImGui::SetTooltip("Opens the edit menu");
	}
}

void MainMenuBar::WindowMenu()
{
	if (ImGui::BeginMenu(ICON_FA_WINDOW_RESTORE" Window"))
	{
		std::string iconName;
		for (size_t i = 0; i < menus.size(); i++)
		{
			iconName = iconList.at(i) + " " + menus.at(i)->name;
			ImGui::MenuItem(iconName.c_str(), NULL, &menus[i]->active);
		}

		ImGui::EndMenu();
	}
	if (ImGui::IsItemHovered())
	{
		ImGui::SetTooltip("Opens the window menu");
	}
}

void MainMenuBar::ViewMenu()
{
	if (ImGui::BeginMenu(ICON_FA_EYE" View"))
	{
		if (ImGui::MenuItem("Depth Test", NULL, app->renderer3D->GetDepthTest()))
			app->renderer3D->SetDepthTest();
		if (ImGui::MenuItem("Cull Face", NULL, app->renderer3D->GetCullFace()))
			app->renderer3D->SetCullFace();
		if (ImGui::MenuItem("Lighting", NULL, app->renderer3D->GetLighting()))
			app->renderer3D->SetLighting();
		if (ImGui::MenuItem("Color Material", NULL, app->renderer3D->GetColorMaterial()))
			app->renderer3D->SetColorMaterial();
		if (ImGui::MenuItem("Texture 2D", NULL, app->renderer3D->GetTexture2D()))
			app->renderer3D->SetTexture2D();
		if (ImGui::MenuItem("Stencil", NULL, app->renderer3D->GetStencil()))
			app->renderer3D->SetStencil();
		if (ImGui::MenuItem("Blending", NULL, app->renderer3D->GetBlending()))
			app->renderer3D->SetBlending();
		if (ImGui::MenuItem("Wire", NULL, app->renderer3D->GetWireMode()))
			app->renderer3D->SetWireMode();
		if (ImGui::MenuItem("Show Raycast", NULL, app->renderer3D->GetRayCast()))
			app->renderer3D->SetWireMode();
		static bool showColliders = true;
		if (ImGui::MenuItem("Show Colliders", NULL, &showColliders))
			app->physics->SetDebugDrawing(showColliders);

		ImGui::MenuItem("Show NavMesh", NULL, app->renderer3D->GetNavMesh());
		ImGui::MenuItem("Show Grid", NULL, app->renderer3D->GetDrawGrid());
		ImGui::MenuItem("Show Quad Tree", NULL, app->sceneManager->GetCurrentScene()->GetDrawQuad());
		ImGui::EndMenu();
	}

	if (ImGui::IsItemHovered())
	{
		ImGui::SetTooltip("Opens the view menu");
	}
}

void MainMenuBar::GameObjectMenu()
{
	if (ImGui::BeginMenu(ICON_FA_PLUS" GameObject"))
	{
		CreateGameObjectMenu();
		ImGui::Separator();

		// Align with options
		GameObject* selected = app->editor->GetGO();
		TransformComponent* transSelected = nullptr;
		if (selected)
			transSelected = selected->GetComponent<TransformComponent>();

		if (selected == nullptr)
		{
			ImGui::PushStyleColor(ImGuiCol_Text, IM_COL32(128, 128, 128, 255));
			ImGui::PushStyleColor(ImGuiCol_HeaderHovered, IM_COL32(128, 128, 128, 100));
		}


		if (ImGui::MenuItem("Align with view", "Ctrl+Shift+F"))
			if (selected) transSelected->AlignWithView();

		if (ImGui::MenuItem("Align view to selected", "Alt+Shift+F"))
			if (selected) transSelected->AlignViewWithSelected();

		if (selected == nullptr)
		{
			ImGui::PopStyleColor();
			ImGui::PopStyleColor();
		}
		ImGui::EndMenu();
	}
}

void MainMenuBar::CreateGameObjectMenu()
{
	// Empty Objects
	if (ImGui::MenuItem(ICON_FA_LAYER_GROUP" Create Empty Object", "Ctrl+Shift+N"))
	{
		if (app->editor->GetGO() != nullptr) app->sceneManager->GetCurrentScene()->CreateGameObject(app->editor->GetGO());
		else app->sceneManager->GetCurrentScene()->CreateGameObject(nullptr);
	}

	if (ImGui::MenuItem(ICON_FA_OBJECT_UNGROUP" Create Child", "Alt+Shift+N"))
		if (app->editor->GetGO() != nullptr) app->sceneManager->GetCurrentScene()->CreateGameObjectChild("GameObjectChild", app->editor->GetGO());

	if (ImGui::MenuItem(ICON_FA_OBJECT_GROUP" Create Parent", "Ctrl+Shift+G"))
		if (app->editor->GetGO() != nullptr) app->sceneManager->GetCurrentScene()->CreateGameObjectParent("GameObjectParent", app->editor->GetGO());

	// 3D Object
	if (ImGui::BeginMenu(ICON_FA_CUBES" Create 3D Object"))
	{
		static std::vector<std::string> primitives = { "Cube", "Pyramide", "Sphere", "Cylinder" };
		GameObject* selected = app->editor->GetGO();
		for (int i = 0; i < primitives.size(); i++)
		{
			if (ImGui::MenuItem(primitives.at(i).c_str()))
				app->sceneManager->GetCurrentScene()->Create3DObject((Object3D)i, selected);
		}

		ImGui::EndMenu();
	}
	// UI Object
	if (ImGui::BeginMenu(ICON_FA_CUBES" Create UI element"))
	{
		static std::vector<std::string> uiComponents = { "UI Button", "UI Image", "UI Check Box", "UI Slider", "UI Text","UI InputBox","UI DropDown"};
		for (int i = 0; i < uiComponents.size(); i++)
		{
			if (ImGui::MenuItem(uiComponents.at(i).c_str()))
			{
				GameObject* object = app->sceneManager->GetCurrentScene()->CreateGameObject(nullptr, false);
				object->CreateComponent((ComponentType)(i + (int)ComponentType::UI_BUTTON));
				object->SetName(uiComponents.at(i).c_str());
			}
		}

		ImGui::EndMenu();
	}
	// Lights
	if (ImGui::BeginMenu(ICON_FA_LIGHTBULB " Lights"))
	{
		if (ImGui::MenuItem("Point Light"))
		{
			GameObject* go = app->sceneManager->GetCurrentScene()->CreateGameObject(nullptr);
			ComponentLight* l = (ComponentLight*)go->CreateComponent(ComponentType::LIGHT);
			l->SetAsPointLight();
		}
		else if (ImGui::MenuItem("Spot Light"))
		{
			GameObject* go = app->sceneManager->GetCurrentScene()->CreateGameObject(nullptr);
			ComponentLight* l = (ComponentLight*)go->CreateComponent(ComponentType::LIGHT);
			l->SetAsSpotLight();
		}
		ImGui::EndMenu();
	}
	// Shader
	if (ImGui::BeginMenu(ICON_FA_CIRCLE " Shader"))
	{
		static int count = 0;
		if (ImGui::MenuItem("Light-sensible"))
			app->sceneManager->showCreateLightSensibleShaderWindow = true;

		else if (ImGui::MenuItem("Not light-sensible"))
			app->sceneManager->showCreateNotLightSensibleShaderWindow = true;

		ImGui::EndMenu();
	}
	//Input Action
	if (ImGui::MenuItem(ICON_FA_KEYBOARD" Create Input Action"))
	{
		InputActionMenu* iAMenu = static_cast<InputActionMenu*>(menus[(int)Menus::INPUT_ACTION]);
		iAMenu->SaveInputActionFile("Assets/Resources/InputAction.inputaction");
	}

	// Dialogue
	if (ImGui::BeginMenu(ICON_FA_FILE " Dialogue"))
	{
		if (ImGui::MenuItem("Dialogue"))
			DialogueSystem::GetInstance()->createDialogue = true;

		ImGui::EndMenu();
	}
}

void MainMenuBar::HelpMenu()
{
	if (ImGui::BeginMenu(ICON_FA_INFO_CIRCLE" Help"))
	{
		ImGui::MenuItem("Demo Menu", NULL, &showMenu);
		ImGui::MenuItem(ICON_FA_USER" About Ragnar Engine", "", &menus[(int)Menus::ABOUT]->active);

		if (ImGui::MenuItem(ICON_FA_ADDRESS_BOOK" Documentation", "F1"))
			app->RequestBrowser("https://github.com/UriKurae/Ragnar-Engine");

		if (ImGui::MenuItem(ICON_FA_BUG" Report a Bug"))
			app->RequestBrowser("https://github.com/UriKurae/Ragnar-Engine/issues");
	
		if (ImGui::MenuItem(ICON_FA_DOWNLOAD" Download latest"))
			app->RequestBrowser("https://github.com/UriKurae/Ragnar-Engine/releases");
		
		ImGui::EndMenu();
	}
	if (ImGui::IsItemHovered())
	{
		ImGui::SetTooltip("Opens the help menu");
	}
}

void MainMenuBar::SetStyleMenu()
{
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
}

void MainMenuBar::PlayBar()
{
	bool ret = true;
	ImGui::Begin(" ", &ret, ImGuiWindowFlags_NoScrollbar | ImGuiWindowFlags_NoDecoration | ImGuiWindowFlags_NoResize | ImGuiWindowFlags_NoTitleBar);
	ImGui::PushStyleColor(ImGuiCol_Button, { 0, 0, 0, 255 });
	ImGui::PushStyleColor(ImGuiCol_Border, { 0, 0, 0, 255 });
	ImGui::SameLine(ImGui::GetWindowSize().x * 0.5f - 81);
	GameState state = app->sceneManager->GetGameState();

	if (state == GameState::NOT_PLAYING)
	{
		if (ImGui::Button(ICON_FA_PLAY))
		{
			app->sceneManager->Play();
			AudioManager::Get()->PlayAllAudioSources();
			app->physics->ActiveAllBodies();
		}

		ImGui::SameLine();
		ImGui::Button(ICON_FA_PAUSE);
		ImGui::SameLine();
		ImGui::Button(ICON_FA_STEP_FORWARD);
	}
	else
	{
		if (ImGui::Button(ICON_FA_STOP))
		{
			app->sceneManager->Stop();
			app->physics->SleepAllBodies();
		}
		ImGui::SameLine();

		if (ImGui::Button(ICON_FA_PAUSE))
		{
			if (state == GameState::PAUSE)
			{
				app->sceneManager->Resume();
				AudioManager::Get()->ResumeAllAudioSources();
				app->physics->ActiveAllBodies();
			}
			else
			{
				AudioManager::Get()->PauseAllAudioSources();
				app->sceneManager->Pause();
				app->physics->SleepAllBodies();
			}
		}
		ImGui::SameLine();

		if (ImGui::Button(ICON_FA_STEP_FORWARD) && state == GameState::PAUSE)
			app->sceneManager->NextFrame(); // Not implemented yet
	}
	ImGui::PopStyleColor(2);
	ImGui::End();
}

bool MainMenuBar::CleanUp()
{
	for (unsigned int i = 0; i < menus.size(); ++i)
	{
		RELEASE(menus[i]);
	}
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