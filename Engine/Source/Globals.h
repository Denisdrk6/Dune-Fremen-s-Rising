#pragma once
#include <windows.h>

#define DEBUG_LOG(format, ...) Log(__FILE__, __LINE__, format, __VA_ARGS__)

void Log(const char file[], int line, const char* format, ...);

#define CAP(n) ((n <= 0.0f) ? n=0.0f : (n >= 1.0f) ? n=1.0f : n=n)

#define DEGTORAD 0.0174532925199432957f
#define RADTODEG 57.295779513082320876f
#define HAVE_M_PI
#define M_PI 3.1416

// Deletes a buffer
#define RELEASE( x )\
    {\
       if( x != nullptr )\
       {\
         delete x;\
	     x = nullptr;\
       }\
    }

// Folders defines
#define ASSETS_FOLDER "Assets/"
#define RESOURCES_FOLDER "Assets/Resources/"
#define SETTINGS_FOLDER "Settings/"
#define LIBRARY_FOLDER "Library/"
#define MODELS_FOLDER "Library/Models/"
#define MESHES_FOLDER "Library/Meshes/"
#define TEXTURES_FOLDER "Library/Textures/"
#define MATERIALS_FOLDER "Library/Materials/"
#define SCRIPTS_FOLDER "Library/ScriptsAssembly/"
#define SCRIPTS_ASSETS_FOLDER "Assets/Scripts/"
#define SHADERS_FOLDER "Library/Shaders/"
#define ANIMATIONS_FOLDER "Library/Animations/"
#define BONES_FOLDER "Library/Bones/"
#define SCENES_FOLDER "Assets/Scenes/"
#define PREFABS_ASSETS_FOLDER "Assets/Prefabs/"
#define PREFABS_FOLDER "Library/Prefabs/"
#define SCENES_LIBRARY_FOLDER "Library/Scenes/"
#define PARTICLES_FOLDER "Assets/ParticlesTemplates/"
#define DIALOGUES_ASSETS_FOLDER "Assets/Dialogues/"
#define DIALOGUES_FOLDER "Library/Dialogues/"
#define AUDIO_FOLDER "Library/Wwise/"
#define FONTS_FOLDER "Library/Fonts/"
#define PLAYER_SAVE_FOLDER "Library/SavedGame/Players"
#define QUEST_SAVE_FOLDER "Library/SavedGame/Quest"
#define SCENES_SAVE_FOLDER "Library/SavedGame/Scenes"
#define ENEMIES_SAVE_FOLDER "Library/SavedGame/Enemies"
#define DIALOGUES_SAVE_FOLDER "Library/SavedGame/Dialogues"
#define CONFIG_SAVE_FOLDER "Library/SavedGame/GameConfig"
#define CURSOR_FOLDER "Library/Cursors"

// Deletes an array of buffers
#define RELEASE_ARRAY( x )\
	{\
       if( x != nullptr )\
       {\
           delete[] x;\
	       x = nullptr;\
		 }\
	 }

typedef unsigned int uint;

static void CMDCompileCS()
{
#pragma region ShellExecute
	SHELLEXECUTEINFO execInfo = { 0 };
	execInfo.cbSize = sizeof(SHELLEXECUTEINFO);
	execInfo.fMask = SEE_MASK_NOCLOSEPROCESS;
	execInfo.hwnd = NULL;
	execInfo.lpVerb = NULL;
	execInfo.lpFile = "cmd";
	execInfo.lpParameters = "/C cd mono-runtime/MSBuild & msbuild ../../Assembly-CSharp.sln /p:Configuration=Release"; //Should include msbuild to the editor folder to make sure this will work? /p:Configuration=Release
	execInfo.lpDirectory = NULL;
	execInfo.nShow = SW_SHOW; /*SW_SHOW  SW_HIDE*/
	execInfo.hInstApp = NULL;

	ShellExecuteEx(&execInfo);

	if (execInfo.hProcess != NULL) {
		WaitForSingleObject(execInfo.hProcess, INFINITE);
		CloseHandle(execInfo.hProcess);
	}

#pragma endregion
}

// Configuration -----------
#define SCREEN_WIDTH 1024
#define SCREEN_HEIGHT 768
#define SCREEN_SIZE 1
#define WIN_FULLSCREEN false
#define WIN_RESIZABLE true
#define WIN_BORDERLESS false
#define WIN_FULLSCREEN_DESKTOP false
#define VSYNC true
#define TITLE "Dune: Fremen's Rising"