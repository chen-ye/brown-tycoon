// This script will  rotate the suns and moons in the sky according to the current in game time.
// This script will  change the skybox from the day skybox to the night skybox as time progresses in game.

using UnityEngine;

[AddComponentMenu("Day-Night Cycle")]
public class DNC_DayNight : MonoBehaviour
{
	private const float SECOND = 1; // constant for 1 second
	private const float MINUTE = 60 * SECOND; // constant for 60 seconds in a minute
	private const float HOUR = 60 * MINUTE; // constant for 60 minutes in an hour
	private const float DAY = 24 * HOUR; // constant for 24 hours in a day
	
	private EnvironmentState _currentEnvironmentState;
	private TimeOfDayTransition _currentTransition;
	private EnvironmentState _sourceEnvironmentState;

	private Light _sunLight;

	
	// Current time in seconds
	private float _timeInSeconds;
    /// <summary>
    /// if false = isNight
    /// </summary>
    [HideInInspector]
    public bool isDay;
    /// <summary>
    /// SkyCamera see documentation for more info
    /// </summary>
	public Camera playerCamera;
    /// <summary>
    /// player root or main camera
    /// </summary>
	public Transform attachedTo;
    /// <summary>
    /// Sun Model
    /// </summary>
	public Transform sun;
    /// <summary>
    /// Moon Model
    /// </summary>
	public Transform moon;
    /// <summary>
    /// Material with the SkyBlend shader
    /// </summary>
    public Material SkyBlend = null;
    /// <summary>
    /// specialized layer added to Uday
    /// </summary>
    public LayerMask UDayMask;
	// Number of days passed since the game had started
	
	private int daysPassed;

	public TimeOfDayTransition[] timeOfDayTransitions;
	public int InitialStateIndex;

	
	// Day-night cycle period in real time minutes
	public float dayCycleInMinutes = 1;

	
	// Current time in hours
	public float timeInHours; 
//Time and Light
	private void Start()
{
	_sunLight = sun.GetComponentInChildren<Light>();
	if (InitialStateIndex >= 0 && InitialStateIndex < timeOfDayTransitions.Length)
{
	_currentTransition = timeOfDayTransitions[InitialStateIndex];
}
	else
{	
	_currentTransition = timeOfDayTransitions[0];
	}
    if (RenderSettings.skybox == null || RenderSettings.skybox != SkyBlend)
    {
        RenderSettings.skybox = SkyBlend;
    }
        //If playercamera no have skycamera script, them added this.
    if (playerCamera != null)
    {
        if (playerCamera.GetComponent<DNC_SkyCamera>() == null)
        {
            playerCamera.gameObject.AddComponent<DNC_SkyCamera>();
            playerCamera.cullingMask = UDayMask;
        }
    }
    else
    {
        Debug.LogError("you need to add the playerCamera");
    }
	_timeInSeconds = HOUR * timeInHours;
	_sourceEnvironmentState = GetEnvironmentStateFromTransition(_currentTransition);
	ApplyEnvironmentState(_sourceEnvironmentState);
	}

	public int GetSeconds()
	{
	return (int) _timeInSeconds;
	}

	public int GetDaysPassed()
	{	
	return daysPassed;
	}

	public EnvironmentState GetCurrentEnvironmentState()
	{
	return _currentEnvironmentState;
	}
	//Color of Sky
	private EnvironmentState ReadEnvironmentState()
	{
		var env = new EnvironmentState
			{
				ambientLight = RenderSettings.ambientLight,
				moonTintColor = moon.gameObject.GetComponent<Renderer>().material.GetColor("_Color"),
				fogColor = RenderSettings.fogColor,
				fogDensity = RenderSettings.fogDensity,
				sunColor = _sunLight.color,
				sunIntensity = _sunLight.intensity,
				sunTintColor = sun.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor"),
				skyboxBlendValue = RenderSettings.skybox.GetFloat("_Blend"),
				skyboxTintColor = RenderSettings.skybox.GetColor("_Tint"),
			};

	if (_currentEnvironmentState != null)
	{
	env.auxColor1 = _currentEnvironmentState.auxColor1;
	env.auxColor2 = _currentEnvironmentState.auxColor2;
	}
	else
	{
	env.auxColor1 = Color.black;
	env.auxColor2 = Color.black;
	}

	return env;
}

	private void ApplyEnvironmentState(EnvironmentState env)
	{
		RenderSettings.ambientLight = env.ambientLight;

		moon.gameObject.GetComponent<Renderer>().material.SetColor("_Color", env.moonTintColor);

		RenderSettings.fogColor = env.fogColor;
		RenderSettings.fogDensity = env.fogDensity;
		RenderSettings.fog = Mathf.Abs(env.fogDensity) > Mathf.Epsilon;

		RenderSettings.skybox.SetFloat("_Blend", env.skyboxBlendValue);
		RenderSettings.skybox.SetColor("_Tint", env.skyboxTintColor);

		_sunLight.color = env.sunColor;
		_sunLight.intensity = env.sunIntensity;
		sun.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", env.sunTintColor);

		_currentEnvironmentState = env;
	}

	private void Update()
{
		// Update time
		var _realSecondToIngameSecond = 24 * 60 / dayCycleInMinutes;
		_timeInSeconds += Time.deltaTime * _realSecondToIngameSecond;

		if (_timeInSeconds >= DAY)
	{
			_timeInSeconds -= DAY;
			daysPassed++;
	}
		if (_timeInSeconds < 0)
	{
			timeInHours += DAY;
}

		timeInHours = _timeInSeconds / HOUR;

        if (timeInHours >= 6 && timeInHours <= 18)
        {
            isDay = true;
        }
        else
        {
            isDay = false;
        }
		// Update Sun and Moon position
		transform.rotation = Quaternion.Euler(new Vector3(360 / DAY * _timeInSeconds, 0, 0));

		if (playerCamera != null)
		{
			transform.position = attachedTo.position;

			var ambient = _sunLight.color * _sunLight.intensity;
			playerCamera.backgroundColor = ambient;
		}

		// Update environment state
		if (_currentTransition == null)
		{
			_currentTransition = FindActiveTransition(_timeInSeconds);
			if (_currentTransition != null)
			{
				_sourceEnvironmentState = ReadEnvironmentState();
			}
		}

		if (_currentTransition != null)
		{
			ApplyCurrentTransition();
		}
	}

	private static EnvironmentState GetEnvironmentStateFromTransition(TimeOfDayTransition t)
	{
		var env = new EnvironmentState
		          {
		          		ambientLight = t.ambientLight,
		          		moonTintColor = t.moonTintColor,
		          		sunColor = t.sunColor,
		          		sunIntensity = t.sunIntensity,
		          		sunTintColor = t.sunTintColor,
		          		fogColor = t.fogColor,
		          		fogDensity = t.fogDensity,
		          		skyboxBlendValue = t.skyboxBlendValue,
		          		skyboxTintColor = t.skyboxTintColor,
		          		auxColor1 = t.auxColor1,
		          		auxColor2 = t.auxColor2,
		          };
		return env;
	}

	private void ApplyCurrentTransition()
	{
		var s = _sourceEnvironmentState;
		var t = _currentTransition;

		var currentTime = timeInHours;
		if (currentTime - t.startHour < 0)
		{
			currentTime += 24;
		}

		var x = (currentTime - t.startHour) / t.durationInHours;
		if (x > 1)
		{
			x = 1;

			_currentTransition = null;
		}

		
		var env = new EnvironmentState
		          {
		          		ambientLight = Color.Lerp(s.ambientLight, t.ambientLight, x),
		          		moonTintColor = Color.Lerp(s.moonTintColor, t.moonTintColor, x),
		          		sunColor = Color.Lerp(s.sunColor, t.sunColor, x),
		          		sunIntensity = Mathf.Lerp(s.sunIntensity, t.sunIntensity, x),
		          		sunTintColor = Color.Lerp(s.sunTintColor, t.sunTintColor, x),
		          		fogColor = Color.Lerp(s.fogColor, t.fogColor, x),
		          		fogDensity = Mathf.Lerp(s.fogDensity, t.fogDensity, x),
		          		skyboxBlendValue = Mathf.Lerp(s.skyboxBlendValue, t.skyboxBlendValue, x),
		          		skyboxTintColor = Color.Lerp(s.skyboxTintColor, t.skyboxTintColor, x),
		          		auxColor1 = Color.Lerp(s.auxColor1, t.auxColor1, x),
		          		auxColor2 = Color.Lerp(s.auxColor2, t.auxColor2, x),
		          };

		ApplyEnvironmentState(env);
	}

	private TimeOfDayTransition FindActiveTransition(float seconds)
	{
		var hours = seconds / HOUR;
		foreach (var transition in timeOfDayTransitions)
		{
			if (transition.enabled
			    && hours > transition.startHour
			    && (hours - transition.startHour) < transition.durationInHours)
			{
				return transition;
			}
		}

		return null;
	}
}