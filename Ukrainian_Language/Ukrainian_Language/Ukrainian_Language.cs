using RaftModLoader;
using UnityEngine;
using HMLLibrary;
using System.Text;
using I2.Loc;
using System.IO;
using System;
using System.Net;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using HarmonyLib;

namespace UkrainianLanguage
{
    public class Main : Mod
    {
        public static string dataFolder = HLib.path_modsFolder + "\\ModData\\Ukrainian Language\\";
        public static string lastLanguageFile = dataFolder + "\\" + "Last Language Used.txt";
        AssetBundle asset;
        Harmony harmony;
        public static TMP_FontAsset TMP_TTMakrsRoughCyrillic;
        public static TMP_FontAsset TMP_ChineseRocksCyrillic;
        public static TMP_FontAsset TMP_ChineseRocksCyrillic_RML;
        public static Font ChineseRocksCyrillic;
        public static Main Instance { get; set; }
        public static string modVersion = "1.1.1";
        public static string modName = "Ukrainian Language";
        public static string logPrefix = $"[{modName}] : ";

        public IEnumerator Start()
        {
            (harmony = new Harmony("com.damglador.UkrainianLanguage")).PatchAll();
            Instance = this;
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("thefont.asset"));
            yield return request;
            asset = request.assetBundle;
            TMP_TTMakrsRoughCyrillic = asset.LoadAsset<TMP_FontAsset>("TTMarksRough-Bold SDF");
            TMP_ChineseRocksCyrillic = asset.LoadAsset<TMP_FontAsset>("ChineseRocks-Cyrillic SDF");
            TMP_ChineseRocksCyrillic_RML = asset.LoadAsset<TMP_FontAsset>("ChineseRocks-Cyrillic for RML SDF");
            ChineseRocksCyrillic = asset.LoadAsset<Font>("ChineseRocks-Cyrillic");
            if (ChineseRocksCyrillic == null)
                Debug.LogError(logPrefix + "Не вдалось завантажити TTMakrsRoughCyrillic TMPFont");
            else
                Log("TTMakrsRoughCyrillic TMPFont успішно завантажено");
            if (TMP_ChineseRocksCyrillic_RML == null)
                Debug.LogError(logPrefix + "Не вдалось завантажити ChineseRocksCyrillic TMPFont for RML");
            else
                Log("ChineseRocksCyrillic TMPFont for RML успішно завантажено");
            if (TMP_ChineseRocksCyrillic == null)
                Debug.LogError(logPrefix + "Не вдалось завантажити ChineseRocksCyrillic TMPFont");
            else
                Log("ChineseRocksCyrillic TMPFont успішно завантажено");
            if (ChineseRocksCyrillic == null)
                Debug.LogError(logPrefix + "Не вдалось завантажити ChineseRocksCyrillic Font");
            else
                Log("ChineseRocksCyrillic Font успішно завантажено");
            if (!Directory.Exists(dataFolder)) 
            {
                Debug.LogWarning(logPrefix + "Data folder cant be found");
                Directory.CreateDirectory(dataFolder);
                Log("Data folder created");
            }
            UpdateTranslationFile();
            RestoreLanguage();
            foreach (var t in Resources.FindObjectsOfTypeAll<TMP_Text>())
                Patch_TMP_Awake.Postfix(t);
            Log($"Mod loaded! Version {modVersion}");
        }

        public void StartUpdateAfterDelayRoutine()
        {
            StartCoroutine(UpdateAfterDelayRoutine());
        }

        //Font loader ==================================================
        public IEnumerator UpdateAfterDelayRoutine()
        {
            yield return new WaitForSeconds(0.1f);
            var upd = 0;
            foreach (var t in Resources.FindObjectsOfTypeAll<Text>())
            {
                if (Main.UpdateFont(t))
                {
                    upd++;
                }
            }
            Log($"Updated font for {upd} text components");
        }
        public static bool UpdateFont(Text __instance)
        {
            if (__instance.font.name != "ChineseRocks")
                return false;
            __instance.font = ChineseRocksCyrillic; 
            return true;
        }
        public void RestoreLanguage()
        {
            if (File.Exists(lastLanguageFile))
            {
                var last = File.ReadAllText(lastLanguageFile, Encoding.UTF8);
                Log("Мова попереднього сеансу: " + last);
                if (last == LocalizationManager.CurrentLanguage)
                {
                    Log($"Попередня мова: {last}. Поточна мова: {LocalizationManager.CurrentLanguage}.\nПоточна мова відповідає попередній");
                    StartUpdateAfterDelayRoutine();
                } else
                {
                    LocalizationManager.CurrentLanguage = last;
                    LocalizationManager.LocalizeAll(true);
                    Log("Мова змінена на: " + LocalizationManager.CurrentLanguage);
                }
            }
            else
                Patch_UpdateLanguage.Postfix();
        }
        public static class Logger
        {
            private static string logFilePath = dataFolder + "\\" + "Log.txt";
            public static void Log(string message)
            {
                string logMessage = $"{DateTime.Now}: {message}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
        }
        [ConsoleCommand(name: "Я_хочу_пітси", docs: "А ти?")]
        public static void Я_хочу_пітси()
        {
            Debug.Log("Обійдешся");
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
                    Debug.Log(logPrefix + $"Файл перекладу успішно завантажено з репозиторію: {repositoryUrl}");
                }
                catch (WebException e)
                {
                    string errorMessage = "Виникла помилка при завантаженні файлу перекладу з репозиторію GitHub\nБудь ласка перевірте з'єднання з інтернетом, щоб оновити чи завантажити файл перекладу, або зверніться до головного кодера.\nКонтакти:\nDiscord: damglador\nSteam: Damglador, та й будь-де впринципі Damglador";
                    if (e.Status == WebExceptionStatus.ConnectFailure || e.Status == WebExceptionStatus.Timeout)
                    {
                        Debug.LogError(logPrefix + errorMessage);
                    }
                    else
                    {
                        Debug.LogError(logPrefix + errorMessage);
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
    [HarmonyPatch(typeof(Text))]
    [HarmonyPatch("text", MethodType.Setter)]
    static class Patch_Text_Setter
    {
        public static void Postfix(Text __instance) => Main.UpdateFont(__instance);
    }

    [HarmonyPatch(typeof(LocalizationManager), "SetLanguageAndCode")]
    static class Patch_UpdateLanguage
    {
        public static void Postfix()
        {
            File.WriteAllText(Main.lastLanguageFile, LocalizationManager.CurrentLanguage, Encoding.UTF8);
            Debug.Log(Main.logPrefix + $"Збережено вибір мови: {LocalizationManager.CurrentLanguage}");
            Main.Instance.StartUpdateAfterDelayRoutine();
        }
    }
    //Notebook TMPFont loader ==================================================
    [HarmonyPatch(typeof(LocalizeTarget_TextMeshPro_Label), "SetFont")]
    static class Patch_SetLabelFont
    {
        static void Prefix(ref TMP_FontAsset newFont)
        {
            if (LocalizationManager.CurrentLanguage == "Українська")
                newFont = Main.TMP_TTMakrsRoughCyrillic;
        }
    }
    //TMPFont loader ==================================================
    [HarmonyPatch]
    static class Patch_TMP_Awake
    {
        [HarmonyPatch(typeof(TextMeshPro), "Awake")]
        public static void Postfix(TMP_Text __instance)
        {
            if (__instance.name.StartsWith("TextMeshPro", StringComparison.OrdinalIgnoreCase) && (__instance.font.name == "ChineseRocks SDF" || __instance.font.name == "ChineseRocks SDF UI"))
            {
                __instance.font = Main.TMP_ChineseRocksCyrillic;
            } 
            else if (__instance.font.name == "RML_BaseFont" || __instance.font.name == "ChineseRocks SDF" || __instance.font.name == "ChineseRocks SDF UI")
            {
                __instance.font = Main.TMP_ChineseRocksCyrillic_RML;
            }
            else if (__instance.font.name == "tt_marks_rough_bold")
            {
                __instance.font = Main.TMP_TTMakrsRoughCyrillic;
            }

        }
        [HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
        static void Postfix(TextMeshProUGUI __instance) => Postfix(__instance as TMP_Text);
    }
    [HarmonyPatch(typeof(Block), "OnFinishedPlacement")]
    class Patch_Block
    {
        static void Postfix (Block __instance)
        {
            if (__instance is Storage_Small) { Main.Instance.StartUpdateAfterDelayRoutine(); }
        }
    }
}