using UnityEngine;
using System.Collections;

namespace ICE.Shared
{
	public class ICEObjectLabel : MonoBehaviour {
	
	public string CameraName = "";
	public float LabelVerticalOffset = 1;
	public float LabelHorizontalOffset = 0;
	public Color LabelColor = Color.red;
	public string LabelText = "";
	public bool ClampToScreen = false;
	public float MaxDistance = 100;
	public FontStyle LabelStyle = FontStyle.Normal;
	public TextAlignment LabelAlignment = TextAlignment.Center;
	public TextAnchor LabelAnchor = TextAnchor.MiddleCenter;
	
	private Camera m_LabelCamera = null;
	private GameObject m_Label = null;
	private float clampBorderSize = 0.05f;
	private GUIText m_LabelText = null;
	
	
	void Awake()
	{
		if( m_Label == null )
		{
			m_Label = new GameObject();
			m_Label.transform.parent = transform;
			m_Label.name = "Label";
		}
		
		m_LabelText = m_Label.AddComponent<GUIText>();
		m_LabelText.alignment = LabelAlignment;
		m_LabelText.anchor = LabelAnchor;
		m_LabelText.color = LabelColor;
		m_LabelText.fontStyle = LabelStyle;
		
		foreach( Camera _cam in Camera.allCameras )
		{
			if( _cam.name == CameraName )
			{
				m_LabelCamera = _cam; 
				break;
			}
			
		}
		
		if( LabelText == "" )
			LabelText = transform.name;

		m_LabelText.enabled = false;
		
	}
	
	void Update () {
		

		m_LabelText.text = LabelText;
		
		if( m_LabelCamera != null && m_LabelCamera.isActiveAndEnabled && Vector3.Distance( transform.position, m_LabelCamera.transform.position ) < MaxDistance )
		{
				m_LabelText.enabled = true;
			float _distance = Vector3.Distance( transform.position, m_LabelCamera.transform.position );
			if( _distance < 1 )
				_distance = 1;
			
			Color _c = m_LabelText.color;
			_c.a = (100 - ( 100/MaxDistance*_distance ))/100;
			m_LabelText.color = _c;
			
			if( ClampToScreen )
			{
				Vector3 relativePosition = m_LabelCamera.transform.InverseTransformPoint( transform.position + new Vector3( LabelHorizontalOffset, LabelVerticalOffset, 0 ));
				relativePosition.z =  Mathf.Max( relativePosition.z, 1.0f );
				
				m_Label.transform.position = m_LabelCamera.WorldToViewportPoint( m_LabelCamera.transform.TransformPoint( relativePosition ) );
				m_Label.transform.position = new Vector3( Mathf.Clamp(m_Label.transform.position.x, clampBorderSize, 1.0f - clampBorderSize ),
				                                         Mathf.Clamp(m_Label.transform.position.y, clampBorderSize, 1.0f - clampBorderSize ),
				                                         m_Label.transform.position.z);
				
			}
			else
			{
				float _angle = Vector3.Angle( Vector3.forward, m_LabelCamera.transform.InverseTransformPoint( transform.position ) );
				
				
				if( _angle < 90 )
					m_Label.transform.position = m_LabelCamera.WorldToViewportPoint( transform.position + new Vector3( LabelHorizontalOffset, LabelVerticalOffset, 0 ) );
				else
					m_LabelText.enabled = false;
			}
		}
		else
		{
				m_LabelText.enabled = false;
		}
		
	}
}
}