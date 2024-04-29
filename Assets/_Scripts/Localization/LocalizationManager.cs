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
        public static void SetLanguage(Language language)
        {
            CurrentLanguage = language;
            OnLanguageChanged?.Invoke(CurrentLanguage);
        }
        public static void Update()
        {
            OnLanguageChanged?.Invoke(CurrentLanguage);
        }
    }
}
