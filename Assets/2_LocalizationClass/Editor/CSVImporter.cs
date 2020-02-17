using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class CSVImporter : Editor {
    [MenuItem( "Localization/Import" )]
    public static void Import () {
        string path = string.Format("{0}/2_LocalizationClass/Localization_Example.csv", Application.dataPath);
        string[] lines = System.IO.File.ReadAllLines(path);

        string[] firstColumn = lines[0].Split(',');
        LanguageData[] languages = new LanguageData[firstColumn.Length - 3];

        for ( int i = 3; i < firstColumn.Length; i++ ) {
            string langCode = firstColumn[i];

            string langPath = string.Format( "Assets/2_LocalizationClass/LanguagesData/{0}.asset", langCode);

            LanguageData lang = AssetDatabase.LoadAssetAtPath<LanguageData>(langPath);

            if ( lang == null ) {
                lang = CreateInstance<LanguageData>();
                AssetDatabase.CreateAsset( lang, langPath );
            }

            lang.languageCode = langCode;
            lang.keys = new string[lines.Length - 1];
            lang.data = new string[lines.Length - 1];
            languages[i - 3] = lang;
        }

        for ( int i = 1; i < lines.Length; i++ ) {
            string[] cols = lines[i].Split(',');

            for ( int langIndex = 3; langIndex < cols.Length; langIndex++ ) {
                try {
                    languages[langIndex - 3].keys[i - 1] = cols[0];
                    languages[langIndex - 3].data[i - 1] = cols[langIndex];
                    EditorUtility.SetDirty( languages[langIndex - 3] );
                }
                catch {
                    Debug.LogErrorFormat( "Error row {0} column {1}", i, langIndex );
                }
            }
        }

        AssetDatabase.Refresh();

        CheckConsistency( languages );
        BuildCharacterCollection( languages );
    }

    static void CheckConsistency ( LanguageData[] languages ) {
        var eng = languages.Where(a => a.iso == "en").FirstOrDefault();

        if ( eng == null )
            Debug.LogErrorFormat( "English language not found!" );

        int numberOfBraces_Open_eng,numberOfBraces_Close_eng;

        for ( int i = 0; i < eng.keys.Length; i++ ) {
            string eng_value = eng.data[i];
            numberOfBraces_Open_eng = eng_value.Count( a => a == '{' );
            numberOfBraces_Close_eng = eng_value.Count( a => a == '}' );

            if ( numberOfBraces_Close_eng != numberOfBraces_Open_eng ) {
                Debug.LogErrorFormat( "Opened and closed braces are not matching." );
            }
        }

        for ( int i = 0; i < eng.keys.Length; i++ ) {
            foreach ( var item in languages ) {
                string key = eng.keys[i];
                string lang_value = item.data[i];

                int numberOfBraces_Open_lang = lang_value.Count( a => a == '{' );
                int numberOfBraces_Close_lang = lang_value.Count( a => a == '}' );

                if ( numberOfBraces_Open_lang != numberOfBraces_Close_lang ) {
                    Debug.LogErrorFormat( "Opened and closed braces are not matching in language {0}.", item.name );
                }
            }
        }
    }

    static void BuildCharacterCollection ( LanguageData[] languages ) {
        var chin = languages.Where(a => a.iso == "zh").FirstOrDefault();

        if ( chin == null )
            return;

        List<char> chars = new List<char>();
        foreach ( var item in chin.data ) {
            foreach ( var c in item ) {
                if ( c == 10 || c == 13 )
                    continue;
                if ( chars.Contains( c ) == false )
                    chars.Add( c );
            }
        }

        string alls = string.Join("", chars.Select(a => a.ToString()).ToArray());

        System.IO.File.WriteAllText( Application.dataPath + "/2_LocalizationClass/china_usage.txt", alls );
    }
}