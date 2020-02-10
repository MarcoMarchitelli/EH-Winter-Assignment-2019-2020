using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            lang.data = new Dictionary<string, string>();
            languages[i - 3] = lang;
        }

        for ( int i = 1; i < lines.Length; i++ ) {
            string[] cols = lines[i].Split(',');

            for ( int langIndex = 3; langIndex < cols.Length; langIndex++ ) {
                languages[langIndex - 3].data.Add( cols[0], cols[langIndex] );
            }
        }

        AssetDatabase.Refresh();
    }
}