using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
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
    private AudioClip[] chicken_snd;
    [SerializeField]
    private AudioClip[] fox_snd;
    [SerializeField]
    private AudioClip[] corn_snd;
    [SerializeField]
    private AudioClip[] boat_snd;
    [SerializeField]
    private AudioClip[] man_snd;
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
    public void PlaySound(string name)
    {
        switch (name)
        {
            case "corn":
                sfxSource.PlayOneShot(corn_snd[Random.Range(0, corn_snd.Length)], sfxVolume - 0.1f);
                break;
            case "chicken":
                sfxSource.PlayOneShot(chicken_snd[Random.Range(0, chicken_snd.Length)], sfxVolume);
                break;
            case "fox":
                sfxSource.PlayOneShot(fox_snd[Random.Range(0, fox_snd.Length)], sfxVolume);
                break;
            case "man":
                sfxSource.PlayOneShot(man_snd[Random.Range(0, man_snd.Length)], sfxVolume);
                break;
            case "boat":
                sfxSource.PlayOneShot(boat_snd[Random.Range(0, boat_snd.Length)], sfxVolume);
                break;
            case "win":
                StopSong();
                sfxSource.PlayOneShot(win_snd, sfxVolume);
                break;
            case "fail":
                StopSong();
                sfxSource.PlayOneShot(lose_snd, sfxVolume);
                break;
            case "ui":
                sfxSource.PlayOneShot(ui_snd, sfxVolume);
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
    /// Sets the volume
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        sfxSource.volume = volume;
        bgmSource.volume = volume;
    }
    #endregion
}
