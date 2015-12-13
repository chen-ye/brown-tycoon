using UnityEngine;
using System.Collections;
public class DNC_Test : MonoBehaviour {

    public DNC_DayNight DNC;
    public Texture BackBg;
    public Texture2D BlackBox;

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, Screen.height - 253, 256, 128), "", Style);
        GUILayout.Label("Day progress speed");
        GUILayout.BeginHorizontal();
        DNC.dayCycleInMinutes= GUILayout.HorizontalSlider(DNC.dayCycleInMinutes, 30.0f, 1.0f);
        GUILayout.Box(DNC.dayCycleInMinutes.ToString("00"),GUILayout.Width(30));
        GUILayout.EndHorizontal();
        if (DNC.isDay)
        {
            GUILayout.Box("Is Day",Style);
        }
        else
        {
            GUILayout.Box("Is Night", Style);
        }
        GUILayout.Label("Day Progress in Hour");
        GUILayout.BeginHorizontal();
        GUILayout.HorizontalSlider(DNC.timeInHours, 0.0f, 24f);
        GUILayout.Box(DNC.timeInHours.ToString("00")+ "h",GUILayout.Width(40));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUI.DrawTexture(new Rect(0, Screen.height - 125, 256, 128), BackBg);

    }

    public void CalledOnDay()
    {
        Debug.Log("IsDay");
    }
    public void CalledOnNight()
    {
        Debug.Log("IsNight");
    }

    private GUIStyle Style
    {
        get
        {
            GUIStyle s = new GUIStyle();
            s.normal.background = BlackBox;
            s.normal.textColor = new Color(1, 1, 1, 0.9f);
            s.alignment = TextAnchor.MiddleCenter;

            return s;
        }
    }
}