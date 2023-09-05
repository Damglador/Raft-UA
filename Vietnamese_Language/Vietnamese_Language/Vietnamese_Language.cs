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

namespace VietnameseLanguage
{
    public class Main : Mod
    {
        public static string dataFolder = HLib.path_modsFolder + "\\ModData\\Vietnamese Language\\";
        public static string lastLanguageFile = dataFolder + "\\Last Language Used.txt";
        public static string translationFile = dataFolder + "\\Loc.csv";
        AssetBundle asset;
        Harmony harmony;
        public static TMP_FontAsset NoteBookNewFont;
        public static TMP_FontAsset SVNEngine_TMP;
        public static TMP_FontAsset SVNEngineUI_TMP;
        public static Font SVNEngine;
        public static TMP_FontAsset newCalibriBold_TMP;
        public static TMP_FontAsset newLiberationSansBold_TMP;
        public static TMP_FontAsset newUbuntuBold_TMP;
        public static Main Instance { get; set; }
        public static string modVersion = "1.0.0";
        public static string modName = "Vietnamese Language";
        public static string logPrefix = $"[{modName}] : ";

        public IEnumerator Start()
        {
            (harmony = new Harmony("com.damglador.VietnameseLanguage")).PatchAll();
            Instance = this;
            //============================== Loading Font ====================================
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes("vietnamesefont.asset"));
            yield return request;
            asset = request.assetBundle;
            NoteBookNewFont = asset.LoadAsset<TMP_FontAsset>("Pangolin-Regular SDF");
            SVNEngine_TMP = asset.LoadAsset<TMP_FontAsset>("SVN-Engine SDF");
            SVNEngineUI_TMP = asset.LoadAsset<TMP_FontAsset>("SVN-Engine SDF");
            SVNEngine = asset.LoadAsset<Font>("SVN-Engine");
            newCalibriBold_TMP = asset.LoadAsset<TMP_FontAsset>("NotoSans-Bold SDF");
            newLiberationSansBold_TMP = asset.LoadAsset<TMP_FontAsset>("NotoSans-Bold SDF");
            newUbuntuBold_TMP = asset.LoadAsset<TMP_FontAsset>("NotoSans-Bold SDF");
            doesThisTMPFontExist(NoteBookNewFont);
            doesThisTMPFontExist(SVNEngine_TMP);
            doesThisTMPFontExist(SVNEngineUI_TMP);
            doesThisTMPFontExist(newCalibriBold_TMP);
            doesThisTMPFontExist(newLiberationSansBold_TMP);
            doesThisTMPFontExist(newUbuntuBold_TMP);
            if (SVNEngine != null)
                Log("ChineseRocksEdited успішно завантажено");
            else
                Debug.LogError(logPrefix + "Не вдалось завантажити ChineseRocksEdited");
            //================================================================================
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
        public void doesThisTMPFontExist (TMP_FontAsset font)
        {
            if (font != null)
                Debug.Log(logPrefix + $"{font.name} loaded");
            else
                Debug.LogError(logPrefix + $"Failed to load {font.name}");
        }
        //======================================= Font loader ==================================================
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
            Log($"Updated font for {upd} text components");
        }
        //=======================================================================================================
        public static bool UpdateFont(Text __instance)
        {
            if (__instance.font.name != "ChineseRocks")
                return false;
            else if (__instance.name == "AmountText")
                return false;
            __instance.font = SVNEngine;
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
        [ConsoleCommand(name: "UpdateTranslationFile", docs: "Download latest version of translation from GitHub repository")]
        public static void UpdateTranslationFile()
        {
            string repositoryUrl = "https://github.com/alvin3915/Vietnamese_Language/raw/main/Loc.csv";
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.Encoding = Encoding.UTF8; // Встановлюємо кодування UTF-8
                    string fileContent = webClient.DownloadString(repositoryUrl);
                    File.WriteAllText(translationFile, fileContent, Encoding.UTF8);
                    LocalizationManager.Sources[0].Import_CSV(null, File.ReadAllText(translationFile, Encoding.UTF8), eSpreadsheetUpdateMode.Merge, ';');
                    LocalizationManager.LocalizeAll(true);
                    Debug.Log(logPrefix + $"Translation file downloaded from repository: {repositoryUrl}");
                }
                catch (WebException e)
                {
                    string errorMessage = "Failed to load translation from repositore. Please contact with mod owner or check your Internet conection.\nContacts:\nDiscord: damglador or userx2204\nSteam: Damglador";
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
            if (LocalizationManager.CurrentLanguage == "Vietnamese")
                newFont = Main.NoteBookNewFont;
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
                __instance.font = Main.SVNEngine_TMP;
            else if (__instance.font.name == "RML_BaseFont" || __instance.font.name == "ChineseRocks SDF" || __instance.font.name == "ChineseRocks SDF UI" || __instance.font.name == "ServerName")
                __instance.font = Main.SVNEngineUI_TMP;
            else if (__instance.font.name == "tt_marks_rough_bold SDF")
                __instance.font = Main.NoteBookNewFont;
            else if (__instance.font.name == "LiberationSans SDF")
                __instance.font = Main.newLiberationSansBold_TMP;
            else if (__instance.font.name == "Ubuntu-Bold SDF")
                __instance.font = Main.newUbuntuBold_TMP;
            else if (__instance.font.name == "calibrib SDF")
                __instance.font = Main.newCalibriBold_TMP;
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