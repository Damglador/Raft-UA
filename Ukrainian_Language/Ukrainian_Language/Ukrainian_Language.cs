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
using System.Reflection;
using System.Collections;
using TMPro;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;

public class Ukrainian_Language : Mod
{
    public static string dataFolder = HLib.path_modsFolder + "\\ModData\\Ukrainian Language\\";
    public static string lastLanguageFile = dataFolder + "\\" + "Last Language Used.txt";
    AssetBundle asset;
    public IEnumerator Start()
    {
        Log("Ukrainian Language loaded!");
        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("thefont.asset"));
        yield return request;
        asset = request.assetBundle;
        Font CyrillicFont = asset.LoadAsset<Font>("ChineseRocks-Cyrillic");
        if (CyrillicFont != null)
        {
            Log("Шрифт успішно завантажено?");
        }
        else
        {
            Debug.LogError("Не вдалось завантажити шрифт");
        }
        if (!Directory.Exists(dataFolder))
        {
            try
            {
                Directory.CreateDirectory(dataFolder);
                Log("dataFolder успішно створено.");
            }
            catch (Exception)
            {
                Debug.LogError("Виникла помилка при створенні dataFolder");
            }
        }
        if (!File.Exists(lastLanguageFile))
        {
            Debug.LogWarning("Файл пам'яті відсутній.");
            try
            {
                using (StreamWriter sw = new StreamWriter(lastLanguageFile)) { }
            }
            catch (Exception)
            {
                Debug.LogError("Виникла помилка при створенні файлу пам'яті");
            }
        }
        UpdateTranslationFile();
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
        Font CyrillicFont = asset.LoadAsset<Font>("ChineseRocks-Cyrillic");
        Text[] textComponents = FindObjectsOfType<Text>();

        foreach (Text textComponent in textComponents)
        {
            if (textComponent.font != CyrillicFont)
            {
                textComponent.font = CyrillicFont;
                Log("Шрифт компоненту «" + textComponent + "» було замінено на «" + CyrillicFont + "»");
            }
        }
        string content = File.ReadAllText(lastLanguageFile);
        if (content != LocalizationManager.CurrentLanguage)
        {
            File.WriteAllText(lastLanguageFile, LocalizationManager.CurrentLanguage);
            Log("Оновлено файл пам'яті");
        }
    }

    public void OnModUnload()
    {
        asset.Unload(true);
        Log("Ukrainian Language has been unloaded!");
    }
    [ConsoleCommand(name: "UpdateTranslationFile", docs: "Download latest version of translation from GitHub repository")]

    public static void UpdateTranslationFile()
    {
        string repositoryUrl = "https://github.com/Damglador/Raft-UA/raw/l10n_main/uk/Loc.csv";
        string savePath = dataFolder + "\\Loc.csv"; // Замініть на шлях, де ви хочете зберегти файл

        using (WebClient webClient = new WebClient())
        {
            try
            {
                webClient.Encoding = Encoding.UTF8; // Встановлюємо кодування UTF-8
                string fileContent = webClient.DownloadString(repositoryUrl);

                File.WriteAllText(savePath, fileContent, Encoding.UTF8);

                Debug.Log("Файл перекладу успішно завантажено з репозиторію: " + repositoryUrl);
            }
            catch (WebException e)
            {
                string errorMessage = "Виникла помилка при завантаженні файлу перекладу з репозиторію GitHub\nБудь ласка перевірте з'єднання з інтернетом, щоб оновити чи завантажити файл перекладу, або зверніться до головного кодера.\nКонтакти:\nDiscord: damglador\nSteam: Damglador, та й будь-де впринципі Damglador";
                if (e.Status == WebExceptionStatus.ConnectFailure || e.Status == WebExceptionStatus.Timeout)
                {
                    Debug.LogError(errorMessage);
                }
                else
                {
                    Debug.LogError(errorMessage);
                }
            }
        }
        ImportLocalizationFromCSV("Loc.csv");
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