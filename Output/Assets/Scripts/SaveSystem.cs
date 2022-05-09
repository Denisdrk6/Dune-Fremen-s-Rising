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

    public static void SaveTimer(float timer)
    {
        // Cuidado, si no guarda los enemies, mirar aqui (hay un poltergeist aqui)
        BinaryFormatter formatter = new BinaryFormatter();

        string path = "Library/SavedGame/Scenes/" + "Timer" + ".ragnar";
        FileStream stream = new FileStream(path, FileMode.Create);

        TimerData data = new TimerData(timer);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static TimerData LoadTimer()
    {
        string path = "Library/SavedGame/Scenes/" + "Timer" + ".ragnar";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            TimerData data = formatter.Deserialize(stream) as TimerData;

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