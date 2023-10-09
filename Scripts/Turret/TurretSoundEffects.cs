using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSoundEffects : SerializedMonoBehaviour
{
    [SerializeField]
    private Dictionary<turretSounds, AudioSource> sounds;

    public void PlaySound(turretSounds sound)
    {
        GameObject audio = new GameObject(sound.ToString() + " sound");
        Destroy(audio, 3);

        AudioSource activeAudio = audio.AddComponent<AudioSource>();

        activeAudio.clip = sounds[sound].clip;
        activeAudio.loop = sounds[sound].loop;
        activeAudio.volume = sounds[sound].volume;
        activeAudio.pitch = sounds[sound].pitch;

        activeAudio.transform.SetParent(transform);

        activeAudio.Play();
    }
}
