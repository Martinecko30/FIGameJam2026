using System;
using Core;
using UnityEngine;

namespace Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        public Sound[] musicSounds, sfxSounds;
        public float musicVolume, sfxVolume;

        public static AudioManager instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            foreach (Sound s in musicSounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }

            foreach (Sound s in sfxSounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        private void Start()
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1);
            instance.PlayMusic("Game");
        }

        // ----------------------------- Music Functions -------------------------------------------

        public float PlayMusic(string name)
        {
            Sound sound = Array.Find(musicSounds, sound => sound.name == name);
            if (sound == null)
            {
                Debug.LogWarning("Sound " + name + " wasn't found!");
                return -1;
            }

            sound.volume = PlayerPrefs.GetFloat("musicVolume");
            sound.source.Play();
            return sound.clip.length;
        }

        public float PauseMusic(string name)
        {
            Sound sound = Array.Find(musicSounds, sound => sound.name == name);
            if (sound == null)
            {
                Debug.LogWarning("Sound " + name + " wasn't found!");
                return -1;
            }

            sound.source.Pause();
            return sound.clip.length;
        }

        public float StopMusic(string name)
        {
            Sound sound = Array.Find(musicSounds, sound => sound.name == name);
            if (sound == null)
            {
                Debug.LogWarning("Sound " + name + " wasn't found!");
                return -1;
            }

            sound.source.Stop();
            return sound.clip.length;
        }

        public void StopAllMusic()
        {
            foreach (var sound in musicSounds)
            {
                sound.source.Stop();
            }
        }

        public void ChangeMusicSettings(float volume = -1.0f, float pitch = -1.0f)
        {
            if (volume != -1.0f)
                musicVolume = volume;
            //if (pitch != -1.0f)
            //  musicVolume = pitch;

            foreach (Sound s in musicSounds)
            {
                if (volume != -1.0f) s.source.volume = volume;
                if (pitch != -1.0f) s.source.pitch = pitch;
            }
        }

        // ----------------------------- SFX functions -------------------------------------------

        public float PlaySFX(string name)
        {
            Sound sound = Array.Find(sfxSounds, sound => sound.name == name);
            if (sound == null)
            {
                Debug.LogWarning("Sound " + name + " wasn't found!");
                return -1;
            }

            sound.source.Play();
            return sound.clip.length;
        }

        public float PauseSFX(string name)
        {
            Sound sound = Array.Find(sfxSounds, sound => sound.name == name);
            if (sound == null)
            {
                Debug.LogWarning("Sound " + name + " wasn't found!");
                return -1;
            }

            sound.source.Pause();
            return sound.clip.length;
        }

        public float StopSFX(string name)
        {
            Sound sound = Array.Find(sfxSounds, sound => sound.name == name);
            if (sound == null)
            {
                Debug.LogWarning("Sound " + name + " wasn't found!");
                return -1;
            }

            sound.source.Pause();
            return sound.clip.length;
        }

        public void StopAllSFX()
        {
            foreach (var sound in sfxSounds)
            {
                sound.source.Stop();
            }
        }

        public void ChangeSFXSettings(float volume = -1.0f, float pitch = -1.0f)
        {
            if (volume != -1.0f) sfxVolume = volume;
            foreach (Sound s in sfxSounds)
            {
                if (volume != -1.0f) s.source.volume = volume;
                if (pitch != -1.0f) s.source.pitch = pitch;
            }
        }

    }
}