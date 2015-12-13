using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class TimeOfDayTransition
{
	public string name;
	public bool enabled;

	public float startHour;
	public float durationInHours;

	public Color ambientLight;

	public float skyboxBlendValue;
	public Color skyboxTintColor;

	public Color fogColor;
	public float fogDensity;

	public Color sunColor;
	public float sunIntensity;
	public Color sunTintColor;

	public Color moonTintColor;

	public Color auxColor1, auxColor2;
}

[Serializable]
public class EnvironmentState
{
	public float skyboxBlendValue;
	public Color skyboxTintColor;

	public Color fogColor;
	public float fogDensity;

	public float sunIntensity;
	public Color sunColor;
	public Color sunTintColor;

	public Color ambientLight;

	public Color moonTintColor;

	public Color auxColor1, auxColor2;
}
