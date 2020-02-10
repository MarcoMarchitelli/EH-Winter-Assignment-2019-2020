using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Localizator {
    static LocalizationCollection collection;
    static LanguageData currentLanguage;

    static Localizator () {
        collection = Resources.Load<LocalizationCollection>( "LocalizationCollection" );
        SetLanguage();
    }

    static void SetLanguage () {
        string customLanguage = PlayerPrefs.GetString("LANGUAGE", null);

        if ( !string.IsNullOrEmpty( customLanguage ) ) {
            return;
        }
        else {
            CultureInfo ci = CultureInfo.InstalledUICulture;

            currentLanguage = null;
            foreach ( var item in collection.languages ) {
                currentLanguage = item.iso == ci.TwoLetterISOLanguageName ? item : collection.fallbackLanguage;
            }
        }
    }

    public static string Localize ( string key ) {
        return currentLanguage.data[key];
    }
}