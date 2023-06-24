using RaftModLoader;
using UnityEngine;
using HMLLibrary;
using System.Text;
using I2.Loc;
using System.IO;
using System;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using static SO_TradingPost_Buyable;

namespace UkrainianLanguage
{
    public class Main : Mod
    {
        public static string dataFolder = HLib.path_modsFolder + "\\ModData\\Ukrainian Language\\";
        public static string lastLanguageFile = dataFolder + "\\" + "Last Language Used.txt";
        AssetBundle asset;
        Harmony harmony;
        public static TMP_FontAsset TMPCyrillicFont;
        public static Font CyrillicFont;
        public static Main Instance { get; set; }

        public IEnumerator Start()
        {
            Instance = this;
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("thefont.asset"));
            yield return request;
            asset = request.assetBundle;
            TMPCyrillicFont = asset.LoadAsset<TMP_FontAsset>("ChineseRocks-Cyrillic SDF");
            CyrillicFont = asset.LoadAsset<Font>("ChineseRocks-Cyrillic");
            if (CyrillicFont == null)
            {
                Debug.LogError("Не вдалось завантажити шрифт");
            }
            else
            {
                Log("Шрифт успішно завантажено");
            }
            UpdateTranslationFile();
            RestoreLanguage();
            foreach (var t in Resources.FindObjectsOfTypeAll<TMP_Text>())
                Patch_TMP_Awake.Postfix(t);
            (harmony = new Harmony("com.damglador.UkrainianLanguage")).PatchAll();
            Log("loaded!");
        }

        public void StartUpdateAfterDelayRoutine()
        {
            StartCoroutine(UpdateAfterDelayRoutine());
        }

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

            Debug.Log($"Updated {upd} text components");
        }

        public static bool UpdateFont(Text __instance)
        {
            if (__instance.name.StartsWith("ConsoleLinePrefab"))
                return false;
            __instance.font = Main.CyrillicFont;
            return true;
        }

        public void RestoreLanguage()
        {
            if (File.Exists(lastLanguageFile))
            {
                var last = File.ReadAllText(lastLanguageFile, Encoding.UTF8);
                Log("Мова попереднього сеансу: " + last);
                LocalizationManager.CurrentLanguage = last;
                LocalizationManager.LocalizeAll(true);
                Log("Мова змінена на: " + LocalizationManager.CurrentLanguage);
            }
            else
                Patch_UpdateLanguage.Postfix();
        }
        public void OnModUnload()
        {
            if (asset)
                asset.Unload(true);
            harmony?.UnpatchAll(harmony.Id);
            Log("unloaded!");
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
            Debug.Log("Збережено вибір мови: " + LocalizationManager.CurrentLanguage);
            Main.Instance.StartUpdateAfterDelayRoutine();
        }
    }
    [HarmonyPatch(typeof(LocalizeTarget_TextMeshPro_Label), "SetFont")]
    static class Patch_SetLabelFont
    {
        static void Prefix(ref TMP_FontAsset newFont)
        {
            if (LocalizationManager.CurrentLanguage == "Українська")
                newFont = Main.TMPCyrillicFont;
        }
    }
    [HarmonyPatch]
    static class Patch_TMP_Awake
    {
        [HarmonyPatch(typeof(TextMeshPro), "Awake")]
        public static void Postfix(TMP_Text __instance)
        {
            if (!__instance.name.StartsWith("ConsoleLinePrefab"))
            {
                __instance.font = Main.TMPCyrillicFont;
            }
        }
        [HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
        static void Postfix(TextMeshProUGUI __instance) => Postfix(__instance as TMP_Text);
    }
}