using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioSource[] m_AudioSources;
    private RoBoot m_RoBoot;

    public void PlayAudio(int num)
    {
        m_AudioSources[num].Play();
    }

    public void StopAudio(int num)
    {
        m_AudioSources[num].Stop();
    }
}
