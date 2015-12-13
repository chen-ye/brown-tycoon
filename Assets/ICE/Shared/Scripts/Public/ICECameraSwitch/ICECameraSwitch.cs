using UnityEngine;
using System.Collections;

namespace ICE.Shared
{
	public class ICECameraSwitch : MonoBehaviour {

		//Here is a private reference only this class can access
		private static ICECameraSwitch m_Instance;
		
		//This is the public reference that other classes will use
		public static ICECameraSwitch Instance
		{
			get
			{
				//If m_Instance hasn't been set yet, we grab it from the scene!
				//This will only happen the first time this reference is used.
				if(m_Instance == null)
					m_Instance = GameObject.FindObjectOfType<ICECameraSwitch>();

				//Btw. If you have no CameraSwitch in your scene, the Instance still 
				//null. In this case you could check m_Instance again and create a 
				//new GameObject with the ICECameraSwitch script, so calling Instance 
				//would never failed, but this would be a magic feature, which returns 
				//a valid but unusable object, because there are no cameras assigned, so 
				//it's better to get one error early, than 10 erros durring the gameplay :)

							
				return m_Instance;
			}
		}

		public GameObject MapCamera;
		public KeyCode MapCameraKey = KeyCode.Alpha1;

		public int DefaultCamera = 0;

		public GameObject Camera1;
		public KeyCode CameraKey1 = KeyCode.Alpha1;

		public GameObject Camera2;
		public KeyCode CameraKey2 = KeyCode.Alpha2;

		public GameObject Camera3;
		public KeyCode CameraKey3 = KeyCode.Alpha3;

		public GameObject Camera4;
		public KeyCode CameraKey4 = KeyCode.Alpha4;

		public GameObject Camera5;
		public KeyCode CameraKey5 = KeyCode.Alpha5;

		public GameObject Camera6;
		public KeyCode CameraKey6 = KeyCode.Alpha6;

		public GameObject Camera7;
		public KeyCode CameraKey7 = KeyCode.Alpha7;

		public GameObject Camera8;
		public KeyCode CameraKey8 = KeyCode.Alpha8;

		public GameObject Camera9;
		public KeyCode CameraKey9 = KeyCode.Alpha9;

		public GameObject Camera0;
		public KeyCode CameraKey0 = KeyCode.Alpha0;

		private GameObject[] m_CameraObjects = new GameObject[10];
		
		void Start () {

			m_CameraObjects[0] = Camera1;
			m_CameraObjects[1] = Camera2;
			m_CameraObjects[2] = Camera3;
			m_CameraObjects[3] = Camera4;
			m_CameraObjects[4] = Camera5;
			m_CameraObjects[5] = Camera6;
			m_CameraObjects[6] = Camera7;
			m_CameraObjects[7] = Camera8;
			m_CameraObjects[8] = Camera9;
			m_CameraObjects[9] = Camera0;

			SwitchCamera( DefaultCamera );
		}

		void Update () {

			if (Input.GetKey( CameraKey1 ))
				SwitchCamera(0);
			else if (Input.GetKey( CameraKey2 ))
				SwitchCamera(1);
			else if (Input.GetKey( CameraKey3 ))
				SwitchCamera(2);
			else if (Input.GetKey( CameraKey4 ))
				SwitchCamera(3);
			else if (Input.GetKey( CameraKey5 ))
				SwitchCamera(4);
			else if (Input.GetKey( CameraKey6 ))
				SwitchCamera(5);
			else if (Input.GetKey( CameraKey7 ))
				SwitchCamera(6);
			else if (Input.GetKey( CameraKey8 ))
				SwitchCamera(7);
			else if (Input.GetKey( CameraKey9 ))
				SwitchCamera(8);
			else if (Input.GetKey( CameraKey0 ))
				SwitchCamera(9);
			else if (Input.GetKeyDown( MapCameraKey ))
				ToggleMap();
		}

		public void ToggleMap(){

			if( MapCamera == null || MapCamera.GetComponent<Camera>() == null )
				return;

			MapCamera.SetActive( true );

			Camera cam = (Camera)MapCamera.GetComponent<Camera>();
			
			if( cam == null )
				cam = (Camera)MapCamera.GetComponentInChildren<Camera>();

			if( cam.enabled == false )
			{
				MapCamera.SetActive( true );
				cam.enabled = true;
			}
			else
			{
				cam.enabled = false;
				MapCamera.SetActive( false );
			}
		}

		public void SwitchCamera( int _index ){

			for( int i = 0 ; i < m_CameraObjects.Length; i++) 
			{
				GameObject cam_object = m_CameraObjects[i];

				if( cam_object != null )
				{
					cam_object.SetActive( true );
		
					Camera cam = (Camera)cam_object.GetComponent<Camera>();

					if( cam == null )
						cam = (Camera)cam_object.GetComponentInChildren<Camera>();

					if( cam != null )
					{
						if( i == _index )
						{
							cam_object.SetActive( true );
							cam.enabled = true;
						}
						else
						{
							cam.enabled = false;
							cam_object.SetActive( false );
						}
					}
					else
					{
						Debug.LogWarning( "Sorry! ICECameraSwitch.SwitchCamera can't find Camera on GameObject '" +  cam_object.name + "'" );
					}
				}
			}
		}
	}
}
