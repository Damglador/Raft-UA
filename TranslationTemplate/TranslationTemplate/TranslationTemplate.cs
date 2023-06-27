using RaftModLoader;
﻿using UnityEngine;
using HMLLibrary;
using I2.Loc;
using System.Text;

public class TranslationTemplate : Mod
{
    public void Start()
    {
        Debug.Log(LocalizationManager.Sources[0].Import_CSV(null, Encoding.UTF8.GetString(GetEmbeddedFileBytes("Loc.csv")), eSpreadsheetUpdateMode.Merge, ';'));
        LocalizationManager.LocalizeAll(true);
        Log($"Mod loaded!");
    }
}