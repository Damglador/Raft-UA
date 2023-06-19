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

public class Ukrainian_Language : Mod
{
    public static string dataFolder = HLib.path_modsFolder + "\\ModData\\Ukrainian Language\\";
    public static string lastLanguageFile = dataFolder + "\\" + "Last Language Used.txt";

    public void Start()
    {
        Log("Модифікація успішно завантажена!");
        if (!Directory.Exists(dataFolder))
        {
            try
            {
                Directory.CreateDirectory(dataFolder);
                Log("dataFolder успішно створено.");
            }
            catch (Exception)
            {
                Debug.LogError("Виникла помилка при створенні dataFolder: {ex.Message}");
            }
        }
        if (!File.Exists(lastLanguageFile))
        {
            try
            {
                Log("Файл пам'яті відсутній.");
                File.Create(lastLanguageFile);
            }
            catch (Exception)
            {
                Debug.LogError("Виникла помилка при створенні файлу пам'яті: {ex.Message}");
            }
        }
        string repositoryUrl = "https://github.com/Damglador/Raft-UA/raw/l10n_main/uk/Loc.csv";
        string savePath = dataFolder + "\\Loc.csv"; // Замініть на шлях, де ви хочете зберегти файл

        using (WebClient webClient = new WebClient())
        {
            try
            {
                webClient.Encoding = Encoding.UTF8; // Встановлюємо кодування UTF-8
                string fileContent = webClient.DownloadString(repositoryUrl);

                File.WriteAllText(savePath, fileContent, Encoding.UTF8);

                Log("Файл перекладу успішно завантажено з репозиторію: " + repositoryUrl);
            }
            catch (Exception)
            {
                Log("Виникла помилка при завантаженні файлу: {ex.Message}\nБудь-ласка перевірте з'єднання з інтернетом, щоб оновити чи завантажити файл перекладу, або зверніться до головного кодера. Контакти:\nDiscord: damglador\nSteam: Damglador, та й будь де впринципі Damglador");
            }
        }

        ImportLocalizationFromCSV("Loc.csv");
        RestoreLanguage();

    }
    public void RestoreLanguage()
    {
        string lastLanguage;
        using (StreamReader reader = new StreamReader(lastLanguageFile))
        {
            lastLanguage = reader.ReadLine();
            Log("Мова попереднього сеансу: " + lastLanguage);
        }
        LocalizationManager.CurrentLanguage = lastLanguage;
        Log("Мова змінена на: " + LocalizationManager.CurrentLanguage);
    }
    public void Update()
    {
        string content = File.ReadAllText(lastLanguageFile);
        if (content != LocalizationManager.CurrentLanguage)
        {
            File.WriteAllText(lastLanguageFile, LocalizationManager.CurrentLanguage);
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
            Debug.LogError("No file with name {filename} in ModData.");
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