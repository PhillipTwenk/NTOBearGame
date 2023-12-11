using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSoundEffect : MonoBehaviour
{
    private AudioSource Source;
    public AudioClip HitHammer;
    public AudioClip ThornHit;
    public AudioClip NewMessage;
    public AudioClip Walking;
    public AudioClip Running;
    void Start()
    {
        Source = GetComponent<AudioSource>();
    }
    public void HitHammerPlay() {
        Source.PlayOneShot(HitHammer);
    }
    public void ThornPlay() {
        if(gameObject.CompareTag("ThornSound"))
        {
            Source.PlayOneShot(ThornHit);
        }
    }
    public void WalkingPlay() {
        Source.PlayOneShot(Walking);
    }
    public void RunningPlay() {
        Source.PlayOneShot(Running);
    }
}
