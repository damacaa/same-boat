using Localization;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DefaultPlayerPrefsSetter : MonoBehaviour
{
    private const string PREFS_INITIALIZED = "initialized";


    // Start is called before the first frame update
    private void Awake()
    {
        int init = PlayerPrefs.GetInt(PREFS_INITIALIZED);
        if (init == 0)
        {
            PlayerPrefs.SetInt(PREFS_INITIALIZED, 1);

            PlayerPrefs.SetFloat(UIOptions.MUSIC_VOLUME, 1f);
            PlayerPrefs.SetFloat(UIOptions.EFFECTS_VOLUME, 1f);

            PlayerPrefs.SetInt(ProgressManager.COMPLETED_LEVELS, 0);

            if (IsDeviceLanguageSpanish())
                PlayerPrefs.SetInt(LocalizationManager.PREF_SELECTED_LANGUAGE_KEY, (int)Language.Es);
            else
                PlayerPrefs.SetInt(LocalizationManager.PREF_SELECTED_LANGUAGE_KEY, (int)Language.En);

            PlayerPrefs.Save();
        }
    }

    bool IsDeviceLanguageSpanish()
    {
        // Get the current culture information
        CultureInfo currentCulture = CultureInfo.CurrentCulture;

        // Check if the current language is Spanish
        // You could also check the two-letter language code for a more general check
        // For example, "es" is the two-letter code for Spanish
        return currentCulture.TwoLetterISOLanguageName == "es";
    }
}
