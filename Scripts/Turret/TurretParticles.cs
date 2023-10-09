using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretParticles : SerializedMonoBehaviour
{
    [SerializeField]
    private Dictionary<TurretParticle, ParticleSystem> particles;

    public void Play(TurretParticle toPlay)
    {
        particles[toPlay].Play();
    }
}
