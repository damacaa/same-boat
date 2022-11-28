using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    #region VARIABLES
    // Settings
    public float timePerStep = 0.62f;
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
    private AudioClip[] chicken_steps_snd;
    [SerializeField]
    private AudioClip[] fox_snd;
    [SerializeField]
    private AudioClip[] fox_steps_snd;
    [SerializeField]
    private AudioClip[] corn_snd;
    [SerializeField]
    private AudioClip[] corn_steps_snd;
    [SerializeField]
    private AudioClip[] boat_snd;
    [SerializeField]
    private AudioClip boat_steps_snd;
    [SerializeField]
    private AudioClip[] man_snd;
    [SerializeField]
    private AudioClip ui_snd;
    [SerializeField]
    private AudioClip win_snd;
    [SerializeField]
    private AudioClip lose_snd;    
    // Auxiliar
    private bool[] stepsPlaying = new bool[] { false, false, false, false };
    private float[] stepsTimers = new float[] { 0, 0, 0, 0 };
    // private AudioClip[][] steps_snd;
    private string[] stepsNames = new string[] { "corn", "chicken", "fox", "boat" };
    // Test
    //private float testTimer = 0;
    //private int testCounter = 0;
    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        sfxSource = GetComponents<AudioSource>()[0];
        bgmSource = GetComponents<AudioSource>()[1];
        /*
        steps_snd = new AudioClip[4][];
        steps_snd[0] = corn_steps_snd;
        steps_snd[1] = chicken_steps_snd;
        steps_snd[2] = fox_steps_snd;
        steps_snd[3] = new AudioClip[1] {boat_steps_snd};
        */
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>
    /// Plays a sound effect.
    /// NAMES: corn, chicken, fox, boat, win, fail, ui
    /// </summary>
    /// <param name="name"></param>
    /// <param name="effect"></param>
    public void PlaySound(string name)
    {
        if (name.Equals("corn"))
        {
            sfxSource.PlayOneShot(corn_snd[Random.Range(0, corn_snd.Length)], sfxVolume - 0.1f);
        }
        else if (name.Equals("chicken"))
        {
            sfxSource.PlayOneShot(chicken_snd[Random.Range(0, chicken_snd.Length)], sfxVolume);
        }
        else if (name.Equals("fox"))
        {
            sfxSource.PlayOneShot(fox_snd[Random.Range(0, fox_snd.Length)], sfxVolume);
        }
        else if (name.Equals("man"))
        {
            sfxSource.PlayOneShot(man_snd[Random.Range(0, man_snd.Length)], sfxVolume);
        }
        else if (name.Equals("boat"))
        {
            sfxSource.PlayOneShot(boat_snd[Random.Range(0, boat_snd.Length)], sfxVolume);
        }
        else if (name.Equals("win"))
        {
            sfxSource.PlayOneShot(win_snd, sfxVolume);
        }
        else if (name.Equals("fail"))
        {
            sfxSource.PlayOneShot(lose_snd, sfxVolume);
        }
        else if (name.Equals("ui"))
        {
            sfxSource.PlayOneShot(ui_snd, sfxVolume);
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
    /// Sets the volume
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        sfxSource.volume = volume;
        bgmSource.volume = volume;
    }
    #endregion

    #region IN DEVELOPMENT
    /// <summary>
    /// Starts playing steps sound effect
    /// NAMES: corn, chicken, fox and boat
    /// </summary>
    /// <param name="name"></param>
    private void PlaySteps(string name)
    {
        if (name.Equals("corn"))
        {
            stepsPlaying[0] = true;
            stepsTimers[0] = 0;
        }
        else if (name.Equals("chicken"))
        {
            stepsPlaying[1] = true;
            stepsTimers[1] = 0;
        }
        else if (name.Equals("fox"))
        {
            stepsPlaying[2] = true;
            stepsTimers[2] = 0;
        }
        else if (name.Equals("boat"))
        {
            stepsPlaying[3] = true;
            stepsTimers[3] = 0;
        }
    }

    private void PlaySteps(string name, int index)
    {
        if (name.Equals("corn"))
        {
            sfxSource.PlayOneShot(corn_steps_snd[index]);
        }
        else if (name.Equals("chicken"))
        {
            sfxSource.PlayOneShot(chicken_steps_snd[index]);
        }
        else if (name.Equals("fox"))
        {
            sfxSource.PlayOneShot(fox_steps_snd[index]);
        }
        else if (name.Equals("boat"))
        {
            sfxSource.PlayOneShot(boat_steps_snd);
        }
    }

    /// <summary>
    /// Stops playing steps sound effects
    /// NAMES: corn, chicken, fox and boat
    /// </summary>
    /// <param name="name"></param>
    private void StopSteps(string name)
    {
        if (name.Equals("corn"))
        {
            stepsPlaying[0] = false;
        }
        else if (name.Equals("chicken"))
        {
            stepsPlaying[1] = false;
        }
        else if (name.Equals("fox"))
        {
            stepsPlaying[2] = false;
        }
        else if (name.Equals("boat"))
        {
            stepsPlaying[3] = false;
        }
    }

    private void Update()
    {
        /*
        for (int i = 0; i < stepsPlaying.Length; i++)
        {
            if (stepsPlaying[i])
            {
                stepsTimers[i] += Time.deltaTime;
            }
            if (stepsTimers[i] > timePerStep)
            {
                PlaySteps(stepsNames[i], Random.Range(0, steps_snd[i].Length));
            }
        }        
        */
    }
    #endregion
}
