using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemySound {TakeDamage, Die};
public class EnemySounds : SerializedMonoBehaviour
{
    public Dictionary<enemySound, AudioSource> sounds;

    public void PlaySound(enemySound sound)
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
