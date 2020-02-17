using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( menuName = "Localization/Collection" )]
public class LocalizationCollection : ScriptableObject {
    public LanguageData[] languages;

    public LanguageData fallbackLanguage;

    public LanguageData forcedLanguage;
}