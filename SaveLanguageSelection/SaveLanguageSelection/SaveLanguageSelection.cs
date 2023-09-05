using RaftModLoader;
﻿using UnityEngine;
using HMLLibrary;
using HarmonyLib;
using System.IO;
using I2.Loc;
using System.Text;

public class SaveLanguageSelection : Mod
{
    public static string dataFolder = HLib.path_modsFolder + "\\ModData\\SaveLanguageSelection\\";
    public static string lastLanguageFile = dataFolder + "\\LastUsedLanguage";
    public static SaveLanguageSelection Instance { get; set; }
    public void Start()
    {
        Instance = this;
        Debug.Log("Mod SaveLanguageSelection has been loaded!");
        RestoreLanguage();
    }
    public void RestoreLanguage()
    {
        if (File.Exists(lastLanguageFile))
        {
            var last = File.ReadAllText(lastLanguageFile, Encoding.UTF8);
            Log("Language used before: " + last);
            if (last == LocalizationManager.CurrentLanguage)
            {
                Log($"Language used before: {last}. Current language: {LocalizationManager.CurrentLanguage}.\nLanguage wasnt changed");
            }
            else
            {
                LocalizationManager.CurrentLanguage = last;
                LocalizationManager.LocalizeAll(true);
                Log("Language changed to: " + LocalizationManager.CurrentLanguage);
            }
        }
        else
            Patch_UpdateLanguage.Postfix();
    }
    [HarmonyPatch(typeof(LocalizationManager), "SetLanguageAndCode")]
    static class Patch_UpdateLanguage
    {
        public static void Postfix()
        {
            File.WriteAllText(SaveLanguageSelection.lastLanguageFile, LocalizationManager.CurrentLanguage, Encoding.UTF8);
            Debug.Log("[SaveLanguageSelection] : " + $"Language selection saved: {LocalizationManager.CurrentLanguage}");
        }
    }
}