using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField]
    private Slider _musicSlider;
    [SerializeField]
    private Slider _effectsSlider;

    public static readonly string MUSIC_VOLUME = "musicVolume";
    public static readonly string EFFECTS_VOLUME = "effectsVolume";

    private void Awake()
    {
        _musicSlider.onValueChanged.AddListener(delegate (float value)
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME, value);
            PlayerPrefs.Save();

            SoundController.Instace?.SetMusicVolume(value);
        });

        _effectsSlider.onValueChanged.AddListener(delegate (float value)
        {
            PlayerPrefs.SetFloat(EFFECTS_VOLUME, value);
            PlayerPrefs.Save();

            SoundController.Instace?.SetSFXVolume(value);
        });
    }

    private void OnEnable()
    {
        _musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        _effectsSlider.value = PlayerPrefs.GetFloat(EFFECTS_VOLUME);
    }
}
