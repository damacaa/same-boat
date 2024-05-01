using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour
{
    #region SINGLETON
    public static SoundController Instace;

    #endregion

    #region VARIABLES
    // Settings
    [Header("Volume")]
    public float sfxVolume = 0.8f;

    // Pointers
    [Header("Audio mixers")]
    [SerializeField]
    AudioMixer _sfxMixer;
    [SerializeField]
    AudioMixer _bgmMixer;

    [Header("Audio sources")]
    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioSource bgmSource;


    [Header("Clips")]
    [SerializeField]
    private AudioClip game_sng;
    [SerializeField]
    private AudioClip menu_sng;
    [SerializeField]
    private AudioClip ui_snd;
    [SerializeField]
    private AudioClip boat_snd;
    [SerializeField]
    private AudioClip win_snd;
    [SerializeField]
    private AudioClip lose_snd;

    private bool stopRequested = false;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        if (!Instace)
            Instace = this;
        else
        {
            Destroy(this);
            return;
        }

        PlaySong(0);

        DontDestroyOnLoad(gameObject);
    }

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
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Plays a sound effect.
    /// NAMES: boat, win, fail, ui
    /// </summary>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    /// 

    public enum Sound
    {
        Win,
        Fail,
        UI,
        Boat
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
            case Sound.Boat:
                sfxSource.PlayOneShot(boat_snd, sfxVolume);
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
        bgmSource.volume = 1;
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
        _bgmMixer.SetFloat("Volume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        //bgmSource.volume = volume;
    }
    /// <summary>
    /// Sets the volume of effects
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float volume)
    {
        _sfxMixer.SetFloat("Volume", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        //sfxSource.volume = volume;
    }

    float _bgmSpeed;
    internal void SetSpeed(float speed)
    {
        if (speed == _bgmSpeed)
            return;

        _bgmSpeed = speed;
        bgmSource.pitch = _bgmSpeed;
    }
    #endregion
}