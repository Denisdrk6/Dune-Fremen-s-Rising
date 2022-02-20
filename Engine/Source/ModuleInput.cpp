#include "Application.h"
#include "Globals.h"
#include "ModuleInput.h"
#include "FileSystem.h"
#include "ModuleEditor.h"

#include "Imgui/imgui.h"
#include "Imgui/imgui_impl_sdl.h"

#include "Profiling.h"

#define MAX_KEYS 300

ModuleInput::ModuleInput(bool startEnabled) : Module(startEnabled), pad(nullptr), joy(nullptr)
{
	name = "Input";

	keyboard = new KeyState[MAX_KEYS];

	memset(keyboard, (int)KeyState::KEY_IDLE, sizeof(KeyState) * MAX_KEYS);
	repeated = false;
}

// Destructor
ModuleInput::~ModuleInput()
{
	RELEASE_ARRAY(keyboard);
}

// Called before render is available
bool ModuleInput::Init(JsonParsing& node)
{
	DEBUG_LOG("Init SDL input event system");

	bool ret = true;
	SDL_Init(SDL_INIT_GAMECONTROLLER);

	if(SDL_InitSubSystem(SDL_INIT_EVENTS) < 0)
	{
		DEBUG_LOG("SDL_EVENTS could not initialize! SDL_Error: %s\n", SDL_GetError());
		return false;
	}

	SDL_EventState(SDL_DROPFILE, SDL_ENABLE);

	return ret;
}

// Called every draw update
bool ModuleInput::PreUpdate(float dt)
{
	SDL_PumpEvents();

	const Uint8* keys = SDL_GetKeyboardState(NULL);
	
	for(int i = 0; i < MAX_KEYS; ++i)
	{
		std::string string;
		if(keys[i] == 1)
		{
			if (keyboard[i] == KeyState::KEY_IDLE)
			{
				keyboard[i] = KeyState::KEY_DOWN;
				string = "Keybr :" + std::to_string(i) + " - DOWN";
				strings.push_back(string);
			}
			else
			{
				keyboard[i] = KeyState::KEY_REPEAT;
				if (!repeated)
				{
					repeated = true;
					string = "Keybr :" + std::to_string(i) + " - REPEAT";
					strings.push_back(string);
				}
			}
		}
		else
		{
			if (keyboard[i] == KeyState::KEY_REPEAT || keyboard[i] == KeyState::KEY_DOWN)
			{
				repeated = false;
				keyboard[i] = KeyState::KEY_UP;
				string = "Keybr :" + std::to_string(i) + " - UP";
				strings.push_back(string);
			}
			else
				keyboard[i] = KeyState::KEY_IDLE;
		}
	}

	Uint32 buttons = SDL_GetMouseState(&mouseX, &mouseY);

	mouseX /= SCREEN_SIZE;
	mouseY /= SCREEN_SIZE;
	mouseZ = 0;

	for(int i = 0; i < 5; ++i)
	{
		std::string string;
		if(buttons & SDL_BUTTON(i))
		{
			if (mouseButtons[i] == KeyState::KEY_IDLE)
			{
				mouseButtons[i] = KeyState::KEY_DOWN;
				string = "Mouse :" + std::to_string(i) + " - DOWN";
				strings.push_back(string);
			}
			else
			{
				mouseButtons[i] = KeyState::KEY_REPEAT;
				if (!repeated)
				{
					repeated = true;
					string = "Mouse :" + std::to_string(i) + " - REPEAT";
					strings.push_back(string);
				}
			}
		}
		else
		{
			if (mouseButtons[i] == KeyState::KEY_REPEAT || mouseButtons[i] == KeyState::KEY_DOWN)
			{
				repeated = false;
				mouseButtons[i] = KeyState::KEY_UP;
				string = "Mouse :" + std::to_string(i) + " - UP";
				strings.push_back(string);
			}
			else
			{
				mouseButtons[i] = KeyState::KEY_IDLE;
			}
		}
	}

	mouseXMotion = mouseYMotion = 0;

	//Update gamepad controller
	ControllerUpdate();

	if (IsJoystickAvailable(0))
	{
		if (app->input->GetButton(0, Button::DPAD_Left))
		{
			DEBUG_LOG("DPAD_Left pressed");
		}
	}

	bool quit = false;
	SDL_Event e;

	while(SDL_PollEvent(&e))
	{
		ImGui_ImplSDL2_ProcessEvent(&e);
		switch(e.type)
		{
			case SDL_MOUSEWHEEL:
			mouseZ = e.wheel.y;
			break;

			case SDL_MOUSEMOTION:
			mouseX = e.motion.x / SCREEN_SIZE;
			mouseY = e.motion.y / SCREEN_SIZE;

			mouseXMotion = e.motion.xrel / SCREEN_SIZE;
			mouseYMotion = e.motion.yrel / SCREEN_SIZE;
			break;

			case SDL_DROPFILE:
			{
				std::string filePath = e.drop.file;
				app->fs->ImportFromOutside(filePath, app->editor->GetCurrentDir());
				SDL_free(&filePath);
			}	
			break;

			case SDL_CONTROLLERDEVICEADDED:
				AddController(e.cdevice.which);
				break;

			case SDL_CONTROLLERDEVICEREMOVED:
				RemoveController(e.jdevice.which);
				break;

			case SDL_QUIT:
			quit = true;
			break;

			case SDL_WINDOWEVENT:
			{
				if(e.window.event == SDL_WINDOWEVENT_RESIZED)
					app->ResizeRequest();
			}
		}
	}

	if(quit == true || keyboard[SDL_SCANCODE_ESCAPE] == KeyState::KEY_UP)
		return false;

	return true;
}

// Called before quitting
bool ModuleInput::CleanUp()
{
	DEBUG_LOG("Quitting SDL input event subsystem");
	if (pad != NULL)
	{
		SDL_GameControllerClose(pad);
		pad = nullptr;
	}
	SDL_QuitSubSystem(SDL_INIT_EVENTS);
	return true;
}

bool ModuleInput::IsJoystickAvailable(int joystickId)
{
	return pad ? true : false;
}

bool ModuleInput::ControllerUpdate()
{
	bool ret = true;

	if (pad)
	{
		lastButtons = buttons;
		lastAxis = axes;

		for (unsigned int i = 0; i < static_cast<int>(Button::Count); i++)
		{
			buttons[i] = SDL_GameControllerGetButton(pad, static_cast<SDL_GameControllerButton>(i));
		}
		
		for (unsigned int i = 0; i < static_cast<int>(Axis::Count); i++)
		{
			// SDL ranges axes from -32768 to 32767
			axes[i] = Clamp(SDL_GameControllerGetAxis(pad, static_cast<SDL_GameControllerAxis>(i)) / 32767.f, -1.f, 1.f);
		}
	}

	return ret;
}

bool ModuleInput::LoadConfig(JsonParsing& node)
{
	return true;
}

bool ModuleInput::SaveConfig(JsonParsing& node)
{
	return true;
}

void ModuleInput::AddController(int id)
{
	if (SDL_IsGameController(id))
	{
		pad = SDL_GameControllerOpen(id);

		if (pad)
		{
			DEBUG_LOG("Game controller device with index %d added.", id);
			joy = SDL_GameControllerGetJoystick(pad);
			joyID = SDL_JoystickInstanceID(joy);

			std::fill(buttons.begin(), buttons.end(), false);
			std::fill(lastButtons.begin(), lastButtons.end(), false);
			std::fill(axes.begin(), axes.end(), 0.f);
			std::fill(lastAxis.begin(), lastAxis.end(), 0.f);
		}
		else
		{
			DEBUG_LOG("Error opening game controller with Device Index %d", id);
		}
	}
}

void ModuleInput::RemoveController(int id)
{
	DEBUG_LOG("Game controller device with index %d removed.", id);
	SDL_GameControllerClose(SDL_GameControllerFromInstanceID(id));
	pad = nullptr;
}

bool ModuleInput::GetButton(int joystickId, Button button)
{
	if (SDL_GameControllerFromInstanceID(joystickId))
	{
		return buttons[static_cast<int>(button)];
	}
	DEBUG_LOG("Joystick with id %d is not available!", joystickId);
	return false;
}

bool ModuleInput::GetButtonDown(int joystickId, Button button)
{
	if (SDL_GameControllerFromInstanceID(joystickId))
	{
		return buttons[static_cast<int>(button)] && !lastButtons[static_cast<int>(button)];
	}
	DEBUG_LOG("Joystick with id %d is not available!", joystickId);
	return false;
}

bool ModuleInput::GetButtonUp(int joystickId, Button button)
{
	if (SDL_GameControllerFromInstanceID(joystickId))
	{
		return !buttons[static_cast<int>(button)] && lastButtons[static_cast<int>(button)];
	}
	DEBUG_LOG("Joystick with id %d is not available!", joystickId);
	return false;
}

bool ModuleInput::GetAxis(int joystickId, Axis axis)
{
	if (SDL_GameControllerFromInstanceID(joystickId))
	{
		float val = axes[static_cast<int>(axis)];
		return abs(val) > deadzone ? val : 0.0f;
	}
	DEBUG_LOG("Joystick with id %d is not available!", joystickId);
	return 0.0f;
}
