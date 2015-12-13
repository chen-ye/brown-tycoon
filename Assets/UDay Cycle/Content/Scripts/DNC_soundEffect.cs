////////////////////////////////DNC_soundEffect///////////////////////////////////
////////////////////////Use this for sound effect of day and night///////////////

using UnityEngine;
using System.Collections;
[AddComponentMenu("Day-Night Cycle/Sounds")]
public class DNC_soundEffect : MonoBehaviour {

    DNC_DayNight DNC;
    public AudioClip DayEffect;
    public AudioSource DaySource;
    public AudioClip NightEffect;
    public AudioSource NightSource;
    private float m_volume;
    private float m_volume2;

    void Awake()
    {
        DNC = this.GetComponent<DNC_DayNight>();
        DaySource.clip = DayEffect;
        NightSource.clip = NightEffect;
        DaySource.Play();
        NightSource.Play();
    }

    void Update()
    {
        if (DNC.isDay)
        {
            if (m_volume2 != 0)
            {
                m_volume2 = Mathf.Lerp(m_volume2, 0.0f, Time.deltaTime);
                NightSource.volume = m_volume2;
            }
            //Day OutFade
            if (m_volume != 1.0f)
            {
                m_volume = Mathf.Lerp(m_volume, 1.0f, Time.deltaTime);
            }
            DaySource.volume = m_volume;
        }
        else
        {
            if (m_volume != 0)
            {
                m_volume = Mathf.Lerp(m_volume, 0.0f, Time.deltaTime);
                DaySource.volume = m_volume;
            }
            //Night Sound OutFade
            if (m_volume2 != 1.0f)
            {
                m_volume2 = Mathf.Lerp(m_volume2, 1.0f, Time.deltaTime);
            }
            
            NightSource.volume = m_volume2;

        }


    }
}
