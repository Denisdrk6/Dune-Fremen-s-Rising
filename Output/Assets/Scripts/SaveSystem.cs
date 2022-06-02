using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using RagnarEngine;

public static class SaveSystem
{
    public static bool fromContinue = false;
    public static void SaveScene()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = "Library/SavedGame/Scenes/SceneSaved.ragnar";

        // Delete all files before saving, just in case there's some wrong order
        DeleteDirectoryFiles("Library/SavedGame/Scenes");

        FileStream stream = new FileStream(path, FileMode.Create);

        string data = SceneManager.currentSceneName;

        formatter.Serialize(stream, data);

        stream.Close();
    }
    public static void LoadScene()
    {
        string path = "Library/SavedGame/Scenes/SceneSaved.ragnar";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            string data = formatter.Deserialize(stream) as string;

            stream.Close();

            fromContinue = true;

            SceneManager.LoadScene(data);
        }
        else
        {
            //Debug.Log("Save file not found in " + path);
        }
    }
    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = "Library/SavedGame/Players/" + player.gameObject.name + ".ragnar";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static PlayerData LoadPlayer(string playerName)
    {
        string path = "Library/SavedGame/Players/" + playerName + ".ragnar";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;

            stream.Close();

            return data;
        }
        else
        {
            //Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void SaveEnemy(Enemies enemy)
    {
        // Cuidado, si no guarda los enemies, mirar aqui (hay un poltergeist aqui)
        BinaryFormatter formatter = new BinaryFormatter();

        string path = "Library/SavedGame/Enemies/" + enemy.name + ".ragnar";
        FileStream stream = new FileStream(path, FileMode.Create);

        EnemyData data = new EnemyData(enemy);

        formatter.Serialize(stream, data);

        stream.Close();
    }
    public static EnemyData LoadEnemy(string enemyName)
    {
        string finalName = enemyName.Trim();
        string path = "Library/SavedGame/Enemies/" + finalName + ".ragnar";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            EnemyData data = formatter.Deserialize(stream) as EnemyData;

            stream.Close();

            return data;
        }
        else
        {
            //Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteDirectoryFiles(string path)
    {
        System.IO.DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }
    public static bool CheckExistingFile(string path)
    {
        return File.Exists(path);
    }

    public static void SaveLevel(float timer, Vector3 posCam, Quaternion rot, float angle, bool[] ret)
    {
        // Cuidado, si no guarda los enemies, mirar aqui (hay un poltergeist aqui)
        BinaryFormatter formatter = new BinaryFormatter();
        string path = "Library/SavedGame/Scenes/Level.ragnar";
        FileStream stream = new FileStream(path, FileMode.Create);

        LevelData data = new LevelData(timer, posCam, rot, angle, ret);
        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static GameData LoadGameConfig()
    {
        string path = "Library/SavedGame/GameConfig/" + "Config" + ".ragnar";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;

            stream.Close();

            return data;
        }
        else
        {
            //Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void SaveGameConfig(GameData dataGame)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = "Library/SavedGame/GameConfig/" + "Config" + ".ragnar";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, dataGame);

        stream.Close();
    }

    public static LevelData LoadLevel()
    {
        string path = "Library/SavedGame/Scenes/" + "Level" + ".ragnar";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            LevelData data = formatter.Deserialize(stream) as LevelData;

            stream.Close();

            return data;
        }
        else
        {
            //Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void SaveDialogue(DialogueTrigger trigger)
    {
        // Cuidado, si no guarda los enemies, mirar aqui (hay un poltergeist aqui)
        BinaryFormatter formatter = new BinaryFormatter();

        string objectName = trigger.gameObject.name;
        string path = "Library/SavedGame/Dialogues/" + objectName + ".ragnar";
        FileStream stream = new FileStream(path, FileMode.Create);

        DialogueData data = new DialogueData(trigger);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static DialogueData LoadDialogue(string dialogueName)
    {
        string path = "Library/SavedGame/Dialogues/" + dialogueName + ".ragnar";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            DialogueData data = formatter.Deserialize(stream) as DialogueData;

            stream.Close();

            return data;
        }
        else
        {
            //Debug.Log("Save file not found in " + path);
            return null;
        }
    }
}