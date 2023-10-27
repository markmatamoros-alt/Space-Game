using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SFXManager : MonoBehaviour
{
    List<AudioSource> loopMusic;
    List<AudioClip> sfxList;

    void Start()
    {
        //LoadMusicDatabase();
        //LoadServerSFXDatabase();
        //LoadPlayerSFXDatabase();
        //StartCoroutine(PlaySFX("sfx/server/ship_win.wav"));
    }

    public void LoadMusicDatabase()
    {
        loopMusic = new List<AudioSource>();
        StartCoroutine(LoadMusic("sfx/server/ship_soundbed.wav"));
        StartCoroutine(LoadMusic("sfx/server/ship_damage_1_soundbed.wav"));
        StartCoroutine(LoadMusic("sfx/server/ship_damage_2_soundbed.wav"));
        StartCoroutine(LoadMusic("sfx/server/ship_damage_3_soundbed.wav"));
        StartCoroutine(LoadMusic("sfx/server/ship_damage_4_soundbed.wav"));
        StartCoroutine(LoadMusic("sfx/server/blackhole_soundbed.wav"));
        Debug.Log("Loaded Music Database");
    }

    public void LoadServerSFXDatabase()
    {
        sfxList = new List<AudioClip>();
        StartCoroutine(LoadSFX("sfx/server/countdown.wav"));
        StartCoroutine(LoadSFX("sfx/server/game_start.wav"));
        StartCoroutine(LoadSFX("sfx/server/round_begins.wav"));
        StartCoroutine(LoadSFX("sfx/server/blackhole_warning.wav"));
        StartCoroutine(LoadSFX("sfx/server/round_end.wav"));
        StartCoroutine(LoadSFX("sfx/server/ship_win.wav"));
        StartCoroutine(LoadSFX("sfx/server/ship_destroyed.wav"));

    }
    public void LoadPlayerSFXDatabase()
    {
        sfxList = new List<AudioClip>();
        StartCoroutine(LoadSFX("sfx/player/success.wav"));
        StartCoroutine(LoadSFX("sfx/player/fail.wav"));
        StartCoroutine(LoadSFX("sfx/player/success.wav"));
        StartCoroutine(LoadSFX("sfx/player/gameover.wav"));
        StartCoroutine(LoadSFX("sfx/player/player_ready.wav"));

    }

    public IEnumerator LoadSFX(string _name)
    {

        string path = Path.Combine(Application.streamingAssetsPath, _name);

        using (WWW w = new WWW(path))
        {
            //Debug.Log("using w");

            yield return w;
            // Debug.Log("play audio");
            AudioClip clip = w.GetAudioClip(false, false, AudioType.WAV);
            clip.name = _name;
            sfxList.Add(clip);

        }


    }

    public void PlaySFX(string _name)
    {

        if (sfxList != null)
        {
            for (int i = 0; i < sfxList.Count; i++)
            {
                if (sfxList[i].name == _name)
                {
                    AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
                    audioSource.time = 0;
                    audioSource.clip = sfxList[i];
                    audioSource.Play();
                    Destroy(audioSource, audioSource.clip.length);

                }
            }
        }




    }


    public void PlaySFX(string _name, float _pitchMin, float _pitchMax, float _pan)
    {

        if (sfxList != null)
        {
            for (int i = 0; i < sfxList.Count; i++)
            {
                if (sfxList[i].name == _name)
                {
                    AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
                    audioSource.time = 0;
                    audioSource.clip = sfxList[i];
                    audioSource.pitch = Random.Range(_pitchMin, _pitchMax);
                    audioSource.panStereo = _pan;
                    audioSource.Play();
                    Destroy(audioSource, audioSource.clip.length);

                }
            }
        }



    }

    public IEnumerator LoadMusic(string _name)
    {

        string path = Path.Combine(Application.streamingAssetsPath, _name);

        using (WWW w = new WWW(path))
        {
            //Debug.Log("using w");

            yield return w;
            // Debug.Log("play audio");
            AudioClip clip = w.GetAudioClip(false, false, AudioType.WAV);
            clip.name = _name;

            AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.time = 0;
            audioSource.clip = clip;

            loopMusic.Add(audioSource);





        }
    }

    public void PlayMusic(string _name)
    {
        Debug.Log("Trying to play music:" + _name);

        if (loopMusic != null)
        {
            for (int i = 0; i < loopMusic.Count; i++)
            {
                Debug.Log("music:" + i + " name:" + loopMusic[i].clip.name);

                if (loopMusic[i].clip.name == _name)
                {
                    Debug.Log("Playing music:" + _name);

                    loopMusic[i].time = 0;
                    loopMusic[i].Play();

                }
            }
        }
        else
        {
            Debug.Log("Can't play music" + _name + ". Musics not loaded yet");
        }

    }


    public void StopMusic(string _name)
    {

        if (loopMusic != null)
        {
            for (int i = 0; i < loopMusic.Count; i++)
            {
                if (loopMusic[i].clip.name == _name)
                {
                    loopMusic[i].Stop();

                }
            }
        }
        else
        {
            Debug.Log("Can't play music. No musics loaded");
        }

    }

    public void StopAllMusic()
    {


        if (loopMusic != null)
        {
            for (int i = 0; i < loopMusic.Count; i++)
            {

                loopMusic[i].Stop();


            }
        }

    }

    public void StopAllMusicExcept(string _name)
    {


        if (loopMusic != null)
        {
            for (int i = 0; i < loopMusic.Count; i++)
            {
                if (loopMusic[i].clip.name != _name)
                {
                    loopMusic[i].Stop();

                }
            }
        }

    }



}
