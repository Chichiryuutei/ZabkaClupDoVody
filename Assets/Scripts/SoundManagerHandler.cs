using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerHandler : MonoBehaviour
{
    public static AudioClip DeathSound, JumpSoundOne, JumpSoundTwo, JumpSoundThree, TongueSoundOne, TongueSoundTwo, PickupSound, NoToungueSound, MusicBackground, MusicMenu;

    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        DeathSound = Resources.Load<AudioClip>("DeathSound");

        JumpSoundOne = Resources.Load<AudioClip>("jump");
        JumpSoundTwo = Resources.Load<AudioClip>("jump2");
        JumpSoundThree = Resources.Load<AudioClip>("jump3");

        TongueSoundOne = Resources.Load<AudioClip>("tongue");
        TongueSoundTwo = Resources.Load<AudioClip>("tongue2");

        PickupSound = Resources.Load<AudioClip>("PickUpSound");

        NoToungueSound = Resources.Load<AudioClip>("EmptyHook");

        MusicBackground = Resources.Load<AudioClip>("GameMusic");
        MusicMenu = Resources.Load<AudioClip>("MenuMusic");

        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound (string clip)
    {
        
        switch (clip)
        {
            case "DeathSound":
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(DeathSound);
                break;
            case "JumpSoundOne":
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(JumpSoundOne);
                break;
            case "JumpSoundTwo":
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(JumpSoundTwo);
                break;
            case "JumpSoundThree":
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(JumpSoundThree);
                break;
            case "TongueSoundOne":
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(TongueSoundOne);
                break;
            case "TongueSoundTwo":
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(TongueSoundTwo);
                break;
            case "PickupSound":
                audioSrc.volume = 0.1f;
                audioSrc.PlayOneShot(PickupSound);
                break;
            case "NoToungueSound":
                audioSrc.volume = 0.3f;
                audioSrc.PlayOneShot(NoToungueSound);
                break;
            case "MusicBackground":
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(MusicBackground);
                break;
            case "MusicMenu":
                audioSrc.volume = 1f;
                audioSrc.PlayOneShot(MusicMenu);
                break;
        }
    }
}
