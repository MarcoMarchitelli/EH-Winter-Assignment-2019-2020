using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Localizator {
    static LocalizationCollection collection;
    static LanguageData currentLanguage;
    public static CultureInfo currentCultureInfo;

    static Dictionary<string,string> currentLanguageData;

    static Localizator () {
        collection = Resources.Load<LocalizationCollection>( "LanguagesCollection" );
        SetLanguage();
    }

    static void SetLanguage () {
#if UNITY_EDITOR
        if ( collection.forcedLanguage != null ) {
            currentLanguage = collection.forcedLanguage;
            SetCultureInfo( currentLanguage );
            SetLanguageDictionary( currentLanguage );
            return;
        }
#endif

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

            SetCultureInfo( currentLanguage );
            SetLanguageDictionary( currentLanguage );
        }
    }

    static void SetCultureInfo ( LanguageData langData ) {
        currentCultureInfo = new CultureInfo( langData.iso );
    }

    static void SetLanguageDictionary ( LanguageData langData ) {
        currentLanguageData = new Dictionary<string, string>();
        for ( int i = 0; i < langData.keys.Length; i++ ) {
            currentLanguageData.Add( langData.keys[i], langData.data[i] );
        }
    }

    public static string Localize ( this string key ) {
#if UNITY_EDITOR
        if ( currentLanguageData.ContainsKey( key ) == false ) {
            Debug.LogErrorFormat( "The key {0} wasn't found in language {1}", key, currentLanguage.name );
            return string.Format( "UNDEF_{0}", key );
        }
#endif

        return currentLanguageData[key];
    }

    public static string Localize ( this string key, params object[] values ) {
#if UNITY_EDITOR
        try {
            return string.Format( currentCultureInfo, key.Localize(), values );
        }
        catch ( System.Exception e ) {
            Debug.LogErrorFormat( "Error parsing format for key {0} in language {1}", key, currentLanguage.name );
            Debug.LogException( e );
            return string.Format( "ERROR_{0}", key );
        }
#else
        return string.Format( currentCultureInfo, key.Localize(), values );
#endif
    }
}