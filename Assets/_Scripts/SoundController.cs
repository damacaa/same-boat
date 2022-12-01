using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    #region SINGLETON
    public static SoundController Instace;
    private void Awake()
    {
        if (!Instace)
            Instace = this;
        else
            Destroy(this);
    }

    #endregion

    #region VARIABLES
    // Settings
    public float sfxVolume = 0.8f;
    // Pointers
    private AudioSource sfxSource;
    private AudioSource bgmSource;
    // Clips
    [SerializeField]
    private AudioClip game_sng;
    [SerializeField]
    private AudioClip menu_sng;
    [SerializeField]
    private AudioClip ui_snd;
    [SerializeField]
    private AudioClip win_snd;
    [SerializeField]
    private AudioClip lose_snd;

    private bool stopRequested = false;
    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        sfxSource = GetComponents<AudioSource>()[0];
        bgmSource = GetComponents<AudioSource>()[1];

        PlaySong(0);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private void Update()
    {
        if (stopRequested)
        {
            bgmSource.volume = bgmSource.volume - 1 * Time.deltaTime;
            if (bgmSource.volume <= 0)
            {
                stopRequested = false;
            }
        }
    }

    #region PUBLIC METHODS
    /// <summary>
    /// Plays a sound effect.
    /// NAMES: corn, chicken, fox, boat, win, fail, ui
    /// </summary>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// 

    public enum Sound
    {
        Win,
        Fail,
        UI
    }

    public void PlaySound(Sound sound)
    {
        switch (sound)
        {
            case Sound.Win:
                StopSong();
                sfxSource.PlayOneShot(win_snd, sfxVolume);
                break;
            case Sound.Fail:
                StopSong();
                sfxSource.PlayOneShot(lose_snd, sfxVolume);
                break;
            case Sound.UI:
                sfxSource.PlayOneShot(ui_snd, sfxVolume);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Plays a song.
    /// 0 = menu theme; 1 = game theme
    /// </summary>
    /// <param name="index"></param>
    public void PlaySong(int index)
    {
        switch (index)
        {
            case 0:
                bgmSource.clip = menu_sng;
                break;
            case 1:
                bgmSource.clip = game_sng;
                break;
        }
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// Stops the song
    /// </summary>
    public void StopSong()
    {
        stopRequested = true;
    }

    /// <summary>
    /// Sets the volume of music
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float volume)
    {
        bgmSource.volume = volume;
    }
    /// <summary>
    /// Sets the volume of effects
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
    #endregion
}