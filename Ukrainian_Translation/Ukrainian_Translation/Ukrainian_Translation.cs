using RaftModLoader;
using UnityEngine;
using HMLLibrary;
using System.Text;
using I2.Loc;
using Mono.Cecil.Cil;
using System.IO;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Octokit;
using static MeshCombineStudio.CombinedLODManager;
using Newtonsoft.Json.Linq;
using System.Net;
using Google.Apis.Auth.OAuth2;
using System.Net.Http.Headers;
using System.Net.Http;

public class UkrainianLanguage : Mod
{
    static bool ExtraSettingsAPI_Loaded = false;
    static bool StartUpPending = true;
    public static string dataFolder = HLib.path_modsFolder + "\\ModData\\Language\\";
    public static string lastLanguage = dataFolder + "\\" + "Last Language Used.txt";

    public void Start()
    {
        Log("Модифікація успішно завантажена!");
        string owner = "github_username"; // Замініть на власне ім'я користувача GitHub
        string repo = "repository_name"; // Замініть на назву репозиторію
        string filePath = "path/to/file.txt"; // Замініть на шлях до файлу у репозиторії
        string savePath = "C:\\path\\to\\save\\file.txt"; // Замініть на шлях, де ви хочете зберегти файл

        string accessToken = "your_access_token"; // Замініть на свій особистий токен доступу GitHub

        var github = new GitHubClient(new ProductHeaderValue("DownloadFileExample"));
        github.Credentials = new Credentials(accessToken);

        try
        {
            var fileContent = await github.Repository.Content.GetAllContentsByRef(owner, repo, filePath);

            if (fileContent.Count > 0)
            {
                using (var httpClient = new HttpClient())
                {
                    var fileUrl = fileContent[0].DownloadUrl;
                    var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                    File.WriteAllBytes(savePath, fileBytes);

                    Console.WriteLine("Файл успішно завантажено.");
                }
            }
            else
            {
                Console.WriteLine("Файл не знайдено в репозиторії.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Виникла помилка при завантаженні файлу: {ex.Message}");
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