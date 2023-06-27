using RaftModLoader;
using UnityEngine;
using HMLLibrary;
using System.Text;
using I2.Loc;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using HarmonyLib;


public class Main : Mod
{
    //======================== Configurate it =========================
    public static string modVersion = "0.0.0";
    public static string modName = "Localization Template";
    public static string AssetBundleName = "thefont.asset";
    public static string newTMP_TTMarksRoughFileName = "TTMarksRough-Bold SDF";
    public static string newTMP_ChineseRocksFileName = "ChineseRocks-Cyrillic SDF";
    public static string newTMP_ChineseRocks_UIFileName = "ChineseRocks-Cyrillic for RML SDF";
    public static string newChineseRocksFileName = "ChineseRocks-Cyrillic";
    //=================================================================

    public static string logPrefix = $"[{modName}] : ";
    AssetBundle asset;
    Harmony harmony;
    public static TMP_FontAsset newTMP_TTMarksRough;
    public static TMP_FontAsset newTMP_ChineseRocks;
    public static TMP_FontAsset newTMP_ChineseRocks_UI;
    public static Font newChineseRocks;
    public static Main Instance { get; set; }
    public IEnumerator Start()
    {
        //============================== Part of font loader ================================
        (harmony = new Harmony(modName)).PatchAll();
        Instance = this;
        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(GetEmbeddedFileBytes(AssetBundleName));
        yield return request;
        asset = request.assetBundle;
        newTMP_TTMarksRough = asset.LoadAsset<TMP_FontAsset>(newTMP_TTMarksRoughFileName);
        newTMP_ChineseRocks = asset.LoadAsset<TMP_FontAsset>(newTMP_ChineseRocksFileName);
        newTMP_ChineseRocks_UI = asset.LoadAsset<TMP_FontAsset>(newTMP_ChineseRocks_UIFileName);
        newChineseRocks = asset.LoadAsset<Font>(newChineseRocksFileName);
        if (newTMP_TTMarksRough == null)
            Debug.LogError($"{logPrefix}{newTMP_TTMarksRough.name} missing");
        else
            Log($"{logPrefix}{newTMP_TTMarksRough.name} loaded");
        if (newTMP_ChineseRocks == null)
            Debug.LogError($"{logPrefix}{newTMP_ChineseRocks.name} missing");
        else
            Log($"{logPrefix}{newTMP_ChineseRocks.name} loaded");
        if (newTMP_ChineseRocks_UI == null)
            Debug.LogError($"{logPrefix}{newTMP_ChineseRocks_UI.name} missing");
        else
            Log($"{logPrefix}{newTMP_ChineseRocks_UI.name} loaded");
        if (newChineseRocks == null)
            Debug.LogError($"{logPrefix}{newChineseRocks.name} missing");
        else
            Log($"{logPrefix}{newChineseRocks.name} loaded");
        foreach (var t in Resources.FindObjectsOfTypeAll<TMP_Text>())
            Patch_TMP_Awake.Postfix(t);
        //======================================================================================
        //==================================== Translation loader ================================
        Debug.Log(LocalizationManager.Sources[0].Import_CSV(null, Encoding.UTF8.GetString(GetEmbeddedFileBytes("Loc.csv")), eSpreadsheetUpdateMode.Merge, ';'));
        LocalizationManager.LocalizeAll(true);
        Log($"Mod loaded! Version {modVersion}");
    }
}
//======================================= Font loader ==================================
public class FontLoader : MonoBehaviour
{
    public static FontLoader Instance { get; set; }
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
            if (UpdateFont(t))
            {
                upd++;
            }
        }
        Debug.Log($"{Main.logPrefix}Updated font for {upd} text components");
    }
    public static bool UpdateFont(Text __instance)
    {
        if (__instance.font.name != "ChineseRocks")
            return false;
        __instance.font = Main.newChineseRocks;
        return true;
    }
}
[HarmonyPatch(typeof(Text))]
[HarmonyPatch("text", MethodType.Setter)]
static class Patch_Text_Setter
{
    public static void Postfix(Text __instance) => FontLoader.UpdateFont(__instance);
}
[HarmonyPatch(typeof(LocalizationManager), "SetLanguageAndCode")]
static class Patch_UpdateLanguage
{
    public static void Postfix()
    {
        FontLoader.Instance.StartUpdateAfterDelayRoutine();
    }
}
//=================================== Notebook TMPFont ===============================================
[HarmonyPatch(typeof(LocalizeTarget_TextMeshPro_Label), "SetFont")]
static class Patch_SetLabelFont
{
    static void Prefix(ref TMP_FontAsset newFont)
    {
        newFont = Main.newTMP_TTMarksRough;
    }
}
//================================== TMPFont loader ==================================================
[HarmonyPatch]
static class Patch_TMP_Awake
{
    [HarmonyPatch(typeof(TextMeshPro), "Awake")]
    public static void Postfix(TMP_Text __instance)
    {
        if (__instance.name.StartsWith("TextMeshPro", StringComparison.OrdinalIgnoreCase) && (__instance.font.name == "ChineseRocks SDF" || __instance.font.name == "ChineseRocks SDF UI"))
        {
            __instance.font = Main.newTMP_ChineseRocks;
        }
        else if (__instance.font.name == "RML_BaseFont" || __instance.font.name == "ChineseRocks SDF" || __instance.font.name == "ChineseRocks SDF UI")
        {
            __instance.font = Main.newTMP_ChineseRocks_UI;
        }
        else if (__instance.font.name == "tt_marks_rough_bold")
        {
            __instance.font = Main.newTMP_TTMarksRough;
        }
    }
    [HarmonyPatch(typeof(TextMeshProUGUI), "Awake")]
    static void Postfix(TextMeshProUGUI __instance) => Postfix(__instance as TMP_Text);
}
[HarmonyPatch(typeof(Block), "OnFinishedPlacement")]
class Patch_Block
{
    static void Postfix(Block __instance)
    {
        if (__instance is Storage_Small) { FontLoader.Instance.StartUpdateAfterDelayRoutine(); }
    }
}