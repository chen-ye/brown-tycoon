using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class DNC_Listeners : MonoBehaviour, IEventSystemHandler
{
    /// <summary>
    /// events called when is day
    /// </summary>
    [Serializable]
    public class OnDay : UnityEvent { }
    // Event delegates 
    [SerializeField]
    private OnDay m_OnDayEvent = new OnDay();
    /// <summary>
    /// event calleds when is nitgh
    /// </summary>
    [Serializable]
    public class OnNight : UnityEvent { }
    // Event delegates 
    [SerializeField]
    private OnNight m_OnNightEvent = new OnNight();

    //privates
    private bool m_isDay;
    private DNC_DayNight m_DNC;

    void Awake()
    {
        m_DNC = GetComponent<DNC_DayNight>();
        m_isDay = m_DNC.isDay;
    }

    void Update()
    {
        if (m_DNC == null)
            return;

        if (m_isDay != m_DNC.isDay)
        {
            m_isDay = m_DNC.isDay;
            if (m_isDay == true)
            {
                Day();
            }
            else
            {
                Night();
            }
        }
    }

    private void Day()
    {
        if (!this.enabled)
            return;

        m_OnDayEvent.Invoke();
    }

    private void Night()
    {
        if (!this.enabled)
            return;

        m_OnNightEvent.Invoke();
    }
}