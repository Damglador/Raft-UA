using RaftModLoader;
using UnityEngine;
using HMLLibrary;
using System.Text;
using I2.Loc;
using Mono.Cecil.Cil;
using System.IO;
using System;
using System.Net;
using System.Threading.Tasks;
using static MeshCombineStudio.CombinedLODManager;
using Newtonsoft.Json.Linq;

public class UkrainianLanguage : Mod
{
    static bool ExtraSettingsAPI_Loaded = false;
    static bool StartUpPending = true;
    public static string dataFolder = HLib.path_modsFolder + "\\ModData\\Language\\";
    public static string lastLanguage = dataFolder + "\\" + "Last Language Used.txt";

    public void Start()
    {
        Log("Модифікація успішно завантажена!");
        string repositoryUrl = "https://github.com/Damglador/Raft-UA";
        string filePath = "Loc.csv";
        string savePath = dataFolder + "\\Loc.csv"; // Замініть на шлях, де ви хочете зберегти файл

        using (WebClient webClient = new WebClient())
        {
            try
            {
                webClient.Encoding = Encoding.UTF8; // Встановлюємо кодування UTF-8
                string fileContent = webClient.DownloadString($"{repositoryUrl}/raw/main/{filePath}");

                File.WriteAllText(savePath, fileContent, Encoding.UTF8);

                Log("Файл перекладу успішно завантажено.");
            }
            catch (Exception)
            {
                Log("Виникла помилка при завантаженні файлу: {ex.Message}");
            }
        }

        ImportLocalizationFromCSV("Loc.csv");
        //LocalizationManager.CurrentLanguage = "Українська";
        Log("Language = " + LocalizationManager.CurrentLanguage);

        if (File.Exists(lastLanguage))
        {
            Log("Знайдено файл мови попереднього запуску.");
            using (StreamReader reader = new StreamReader(lastLanguage))
            {
                string firstLine = reader.ReadLine();
                Log("Мова попереднього сеансу: " + firstLine);
            }
            string content = File.ReadAllText(lastLanguage);
            LocalizationManager.CurrentLanguage = content;
            Log("Language set to " + LocalizationManager.CurrentLanguage);
        }
        else
        {
            Log("Файл не існує.");
            File.Create(lastLanguage);
            if (File.Exists(lastLanguage))
            {
                Log("Успішно створено потрібний файл");

            }
        }

    }
    public void Update()
    {
        string content = File.ReadAllText(lastLanguage);
        if (content != LocalizationManager.CurrentLanguage)
        {
            File.WriteAllText(lastLanguage, LocalizationManager.CurrentLanguage);
            Log("Оновлено файл пам'яті");
        }
    }

    public void OnModUnload()
    {
        Log("Ukrainian Language has been unloaded!");
    }

    [ConsoleCommand(name: "ImportLocalizationFromCSV", docs: "File must be in ModData folder and seperator must be ';'.\nUsage: ImportLocalizationFromCSV filename")]
    public static void ImportLocalizationFromCSVCommand(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            Debug.LogError("Invalid argument.");
            Debug.LogError("Type 'help ImportLocalizationFromCSV'");
            return;
        }

        if (!ImportLocalizationFromCSV(args[0]))
            Debug.LogError("Type 'help ImportLocalizationFromCSV'");
    }

    public static bool ImportLocalizationFromCSV(string filename)
    {
        if (filename.IsNullOrEmpty())
        {
            Debug.LogError("Invalid filename.");
            return false;
        }

        if (!File.Exists(dataFolder + filename))
        {
            Debug.LogError($"No file with name {filename} in ModData.");
            return false;
        }

        Debug.Log(LocalizationManager.Sources[0].Import_CSV(null, File.ReadAllText(dataFolder + filename, Encoding.UTF8), eSpreadsheetUpdateMode.Merge, ';'));
        LocalizationManager.LocalizeAll(true);
        return true;
    }

    public static HNotification ErrorNotification(string message)
    {
        return FindObjectOfType<HNotify>().AddNotification(HNotify.NotificationType.normal, message, 10, HNotify.ErrorSprite);
    }

    public static HNotification SuccessNotification(string message)
    {
        return FindObjectOfType<HNotify>().AddNotification(HNotify.NotificationType.normal, message, 5, HNotify.CheckSprite);
    }
}