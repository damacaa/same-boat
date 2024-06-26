using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Localization
{
    public enum Language
    {
        En = 0,
        Es = 1
    }

    public class LocalizationManager
    {
        public static Action<Language> OnLanguageChanged;
        public static Language CurrentLanguage { get; private set; }

        public static string PREF_SELECTED_LANGUAGE_KEY = "language";

        public static void SetLanguage(Language language)
        {
            CurrentLanguage = language;
            PlayerPrefs.SetInt(PREF_SELECTED_LANGUAGE_KEY, (int)CurrentLanguage);
            OnLanguageChanged?.Invoke(CurrentLanguage);
            //Debug.Log(language.ToString());
        }
        public static void Update()
        {
            OnLanguageChanged?.Invoke(CurrentLanguage);
            //Debug.Log("Update: " + CurrentLanguage.ToString());
        }
    }
}
