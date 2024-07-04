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
        public static string lastLanguageFile = dataFolder + "\\Last Language Used.txt";
        public static string translationFile = dataFolder + "\\Loc.csv";
        AssetBundle asset;
        Harmony harmony;
        public static TMP_FontAsset TTMakrsRoughCyrillic_TMP;
        public static TMP_FontAsset ChineseRocksCyrillic_TMP;
        public static TMP_FontAsset ChineseRocksCyrillicUI_TMP;
        public static Font ChineseRocksCyrillic;
        public static TMP_FontAsset CalibriBold_TMP;
        public static TMP_FontAsset LiberationSansBoldCyrillic_TMP;
        public static TMP_FontAsset UbuntuBoldCyrillic_TMP;
        public static Main Instance { get; set; }
        public static string modVersion = "1.1.3";
        public static string modName = "Ukrainian Language";
        public static string logPrefix = $"[{modName}] : ";

        public IEnumerator Start()
        {
            (harmony = new Harmony("com.damglador.UkrainianLanguage")).PatchAll();
            Instance = this;
            //============================== Loading Font ====================================
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("thefont.asset"));
            yield return request;
            asset = request.assetBundle;
            TTMakrsRoughCyrillic_TMP = asset.LoadAsset<TMP_FontAsset>("TTMarksRough-Bold SDF");
            ChineseRocksCyrillic_TMP = asset.LoadAsset<TMP_FontAsset>("ChineseRocks-Cyrillic SDF");
            ChineseRocksCyrillicUI_TMP = asset.LoadAsset<TMP_FontAsset>("ChineseRocks-Cyrillic for RML SDF");
            ChineseRocksCyrillic = asset.LoadAsset<Font>("ChineseRocks-Cyrillic");
            CalibriBold_TMP = asset.LoadAsset<TMP_FontAsset>("Calibri-Bold SDF");
            LiberationSansBoldCyrillic_TMP = asset.LoadAsset<TMP_FontAsset>("LiberationSans-Bold Cyrillic SDF");
            UbuntuBoldCyrillic_TMP = asset.LoadAsset<TMP_FontAsset>("Ubuntu-Bold Cyrillic SDF");
            doesThisTMPFontExist(TTMakrsRoughCyrillic_TMP);
            doesThisTMPFontExist(ChineseRocksCyrillic_TMP);
            doesThisTMPFontExist(ChineseRocksCyrillicUI_TMP);
            doesThisTMPFontExist(CalibriBold_TMP);
            doesThisTMPFontExist(LiberationSansBoldCyrillic_TMP);
            doesThisTMPFontExist(UbuntuBoldCyrillic_TMP);
            if (ChineseRocksCyrillic != null)
                Log("ChineseRocksCyrillic успішно завантажено");
            else
                Debug.LogError(logPrefix + "Не вдалось завантажити ChineseRocksCyrillic");
            //================================================================================
            if (!Directory.Exists(dataFolder)) 
            {
                Debug.LogWarning(logPrefix + "Data folder cant be found");
                Directory.CreateDirectory(dataFolder);
                Log("Data folder created");
            }
            Debug.Log(LocalizationManager.Sources[0].Import_CSV(null, Encoding.UTF8.GetString(GetEmbeddedFileBytes("Loc.csv")), eSpreadsheetUpdateMode.Merge, ';'));
            LocalizationManager.LocalizeAll(true);
            RestoreLanguage();
            foreach (var t in Resources.FindObjectsOfTypeAll<TMP_Text>())
                Patch_TMP_Awake.Postfix(t);
            Log($"Mod loaded! Version {modVersion}");
        }
        public void doesThisTMPFontExist (TMP_FontAsset font)
        {
            if (font != null)
                Debug.Log(logPrefix + $"{font.name} успішно завантажено");
            else
                Debug.LogError(logPrefix + $"Не вдалось завантажити {font.name}");
        }
        public void StartUpdateAfterDelayRoutine()
        {
            StartCoroutine(UpdateAfterDelayRoutine());
        }
        //======================================= Font loader ==================================================
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
        //=======================================================================================================
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
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.Encoding = Encoding.UTF8; // Встановлюємо кодування UTF-8
                    string fileContent = webClient.DownloadString(repositoryUrl);
                    File.WriteAllText(translationFile, fileContent, Encoding.UTF8);
                    LocalizationManager.Sources[0].Import_CSV(null, File.ReadAllText(translationFile, Encoding.UTF8), eSpreadsheetUpdateMode.Merge, ';');
                    LocalizationManager.LocalizeAll(true);
                    Debug.Log(logPrefix + $"Файл перекладу успішно завантажено з репозиторію: {repositoryUrl}");
                    foreach (var t in Resources.FindObjectsOfTypeAll<TMP_Text>())
                        Patch_TMP_Awake.Postfix(t);
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
        }
    }
    //======================================= Font loader ==============================================================
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
    //======================================= Notebook TMPFont loader ==================================================
    [HarmonyPatch(typeof(LocalizeTarget_TextMeshPro_Label), "SetFont")]
    static class Patch_SetLabelFont
    {
        static void Prefix(ref TMP_FontAsset newFont)
        {
            if (LocalizationManager.CurrentLanguage == "Українська")
                newFont = Main.TTMakrsRoughCyrillic_TMP;
        }
    }
    //=========================================== TMPFont loader ========================================================
    [HarmonyPatch]
    static class Patch_TMP_Awake
    {
        [HarmonyPatch(typeof(TextMeshPro), "Awake")]
        public static void Postfix(TMP_Text __instance)
        {
            if (__instance.name.StartsWith("TextMeshPro", StringComparison.OrdinalIgnoreCase) && (__instance.font.name == "ChineseRocks SDF" || __instance.font.name == "ChineseRocks SDF UI"))
                __instance.font = Main.ChineseRocksCyrillic_TMP;
            else if (__instance.font.name == "RML_BaseFont" || __instance.font.name == "ChineseRocks SDF" || __instance.font.name == "ChineseRocks SDF UI" || __instance.font.name == "ServerName")
                __instance.font = Main.ChineseRocksCyrillicUI_TMP;
            else if (__instance.font.name == "tt_marks_rough_bold SDF")
                __instance.font = Main.TTMakrsRoughCyrillic_TMP;
            else if (__instance.font.name == "LiberationSans SDF")
                __instance.font = Main.LiberationSansBoldCyrillic_TMP;
            else if (__instance.font.name == "Ubuntu-Bold SDF")
                __instance.font = Main.UbuntuBoldCyrillic_TMP;
            /*string lastFont = __instance.font.name;
            __instance.font = Main.CalibriBold_TMP;
            if (lastFont != Main.CalibriBold_TMP.name && lastFont != "ChineseRocks SDF" && lastFont != "tt_marks_rough_bold")
                Main.Logger.Log($"Changed font for {__instance.name}    old font: {lastFont}" );*/
        }
        [HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
        static void Postfix(TextMeshProUGUI __instance) => Postfix(__instance as TMP_Text);
    }
    [HarmonyPatch(typeof(Block), "OnFinishedPlacement")]
    class Patch_Block
    {
        static void Postfix (Block __instance)
        {
            if (__instance is Storage_Small) 
                Main.Instance.StartUpdateAfterDelayRoutine();
        }
    }
}