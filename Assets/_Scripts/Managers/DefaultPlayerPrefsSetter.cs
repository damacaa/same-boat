using System.Collections;
using System.Collections.Generic;
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

            PlayerPrefs.Save();
        }
    }
}
