using UnityEngine;

[AddComponentMenu("Day-Night Cycle/SkyCamera")]
[RequireComponent(typeof(Camera))]
public class DNC_SkyCamera : MonoBehaviour
{
	private bool _isFogEnabled;
	private Color _ambientLight;

//Render settings controller
	public void OnPreRender()
	{
		_isFogEnabled = RenderSettings.fog;
		_ambientLight = RenderSettings.ambientLight;

		RenderSettings.fog = false;
		RenderSettings.ambientLight = new Color(1f, 1f, 1f);
	}

	public void OnPostRender()
	{
		RenderSettings.fog = _isFogEnabled;
		RenderSettings.ambientLight = _ambientLight;
	}
}
