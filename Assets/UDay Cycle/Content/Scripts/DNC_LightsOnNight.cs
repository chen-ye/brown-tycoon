using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[AddComponentMenu("Day-Night Cycle/Lights")]
public class DNC_LightsOnNight : MonoBehaviour {

    DNC_DayNight DNC;
    public List<Light> Lights = new List<Light>();
    public float Instense = 2.0f;
    private float m_intense = 0.0f;
    void Awake()
    {
        DNC = this.GetComponent<DNC_DayNight>();
        if (Lights.Count <= 0)
        {
            this.enabled = false;
        }

    }

    void Update()
    {
        
        if (!DNC.isDay)
        {
            if (m_intense != Instense)
            {
                m_intense = Mathf.Lerp(m_intense, Instense, Time.deltaTime);
            }
            foreach (Light light in Lights)
            {
                if (light == null)
                    return;

                light.intensity = m_intense;
            }
        }
        else
        {
            if (m_intense > 0.0f)
            {
                m_intense = Mathf.Lerp(m_intense, 0.0f, Time.deltaTime);
                foreach (Light light in Lights)
                {
                    if (light == null)
                        return;
                    light.intensity = m_intense;
                }
            }
        }
    }
}