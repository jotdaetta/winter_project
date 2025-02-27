using System;
using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public Sound[] Sounds;
    public AudioSource[] _tracks;

    public string PlayingMusic;

    void Awake()
    {
        Instance = this;
    }

    public void PlayToDist(string musicId, Vector3 originPos, Vector3 targetPos, float distance, float pitch = 0)
    {
        if (Vector2.Distance(originPos, new Vector2(targetPos.x, targetPos.y)) <= distance)
        {
            Play(musicId, pitch);
        }
    }

    public void Play(string musicId, float pitch = 0)
    {
        Sound sound = Array.Find(Sounds, v => v.id == musicId);
        if (sound == null) return;

        StartCoroutine(OnPlay(sound, pitch));
    }

    public void Stop(int track)
    {
        _tracks[track - 1].Stop();
        PlayingMusic = null;
    }

    public void Pause(int track)
    {
        _tracks[track - 1].Pause();
    }

    public void UnPause(int track)
    {
        if (_tracks[track - 1]) _tracks[track - 1].UnPause();
    }

    public IEnumerator OnPlay(Sound sound, float pitch)
    {
        if (sound.track == 4)
        {
            if (PlayingMusic != null && PlayingMusic == sound.id) yield break;
            else PlayingMusic = sound.id;
        }
        AudioSource _audio = _tracks[sound.track - 1];

        if (sound.audioIn)
        {
            _audio.clip = sound.audioIn;
            _audio.loop = false;
            _audio.volume = sound.volume;
            _audio.pitch = sound.pitch;
            _audio.time = sound.startTime;

            if (pitch != 0)
            {
                _audio.pitch = pitch;
            }
            _audio.Play();

            while (true)
            {

                yield return new WaitForSecondsRealtime(0.01f);
                if (!_audio.isPlaying)
                {
                    _audio.clip = sound.audio;
                    _audio.Play();
                    _audio.loop = sound.loop;

                    break;
                }
            }
        }
        else
        {
            _audio.clip = sound.audio;
            _audio.loop = sound.loop;
            _audio.volume = sound.volume;
            _audio.pitch = sound.pitch;
            _audio.time = sound.startTime;

            if (pitch != 0)
            {
                _audio.pitch = pitch;
            }

            _audio.Play();
        }
    }
}
