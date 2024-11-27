using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource SFX;

    public AudioClip clickClip;
    public AudioClip itemChangeClip;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClickSound()
    {
        SFX.PlayOneShot(clickClip);
    }

    public void ItemChange()
    {
        SFX.PlayOneShot(itemChangeClip);
    }
}
