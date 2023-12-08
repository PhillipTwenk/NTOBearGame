using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVFXOnEnter : MonoBehaviour
{
    // Start is called before the first frame update
    public ParticleSystem _particleSystem;
    private void OnTriggerEnter(Collider other)
    {
        _particleSystem.Play();
    }
}
