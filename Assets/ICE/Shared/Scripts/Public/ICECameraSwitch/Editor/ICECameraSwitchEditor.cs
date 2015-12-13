using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using ICE;
using ICE.Layouts;
using ICE.Styles;

namespace ICE.Shared
{
	[CustomEditor(typeof(ICECameraSwitch))]
	public class ICECameraSwitchEditor : Editor {

		public ICECameraSwitch m_camera_switch;

		public virtual void OnEnable()
		{
			m_camera_switch = (ICECameraSwitch)target;
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.Separator();

			string[] options = {"Camera 1","Camera 2","Camera 3","Camera 4","Camera 5","Camera 6","Camera 7","Camera 8","Camera 9","Camera 10"};
			m_camera_switch.DefaultCamera = EditorGUILayout.Popup( "Default Camera", m_camera_switch.DefaultCamera, options );

			EditorGUILayout.Separator();

			int _default_index = m_camera_switch.DefaultCamera;
			int _index = 0;



			EditorGUILayout.LabelField( "Camera 1" + (_default_index == _index++?" (default)":"") );
				EditorGUI.indentLevel++;
					m_camera_switch.Camera1 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera1, typeof(GameObject), true);
					m_camera_switch.CameraKey1 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey1 );
				EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

			EditorGUILayout.LabelField( "Camera 2" + (_default_index == _index++?" (default)":"") );
				EditorGUI.indentLevel++;
					m_camera_switch.Camera2 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera2, typeof(GameObject), true);
					m_camera_switch.CameraKey2 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey2 );
				EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

			EditorGUILayout.LabelField( "Camera 3" + (_default_index == _index++?" (default)":"") );
				EditorGUI.indentLevel++;
					m_camera_switch.Camera3 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera3, typeof(GameObject), true);
					m_camera_switch.CameraKey3 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey3 );
				EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

			EditorGUILayout.LabelField( "Camera 4" + (_default_index == _index++?" (default)":"") );
				EditorGUI.indentLevel++;
					m_camera_switch.Camera4 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera4, typeof(GameObject), true);
					m_camera_switch.CameraKey4 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey4 );
				EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

			EditorGUILayout.LabelField( "Camera 5" + (_default_index == _index++?" (default)":"") );			
				EditorGUI.indentLevel++;			
					m_camera_switch.Camera5 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera5, typeof(GameObject), true);
					m_camera_switch.CameraKey5 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey5 );
				EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

			EditorGUILayout.LabelField( "Camera 6" + (_default_index == _index++?" (default)":"") );
				EditorGUI.indentLevel++;			
					m_camera_switch.Camera6 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera6, typeof(GameObject), true);
					m_camera_switch.CameraKey6 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey6 );
				EditorGUI.indentLevel--;			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField( "Camera 7" + (_default_index == _index++?" (default)":"") );
				EditorGUI.indentLevel++;
					m_camera_switch.Camera7 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera7, typeof(GameObject), true);
					m_camera_switch.CameraKey7 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey7 );
				EditorGUI.indentLevel--;			
			EditorGUILayout.Separator();
						
			EditorGUILayout.LabelField( "Camera 8" + (_default_index == _index++?" (default)":"") );
				EditorGUI.indentLevel++;
					m_camera_switch.Camera8 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera8, typeof(GameObject), true);
					m_camera_switch.CameraKey8 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey8 );
				EditorGUI.indentLevel--;			
			EditorGUILayout.Separator();
						
			EditorGUILayout.LabelField( "Camera 9" + (_default_index == _index++?" (default)":"") );		
				EditorGUI.indentLevel++;			
					m_camera_switch.Camera9 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera9, typeof(GameObject), true);
					m_camera_switch.CameraKey9 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey9 );
				EditorGUI.indentLevel--;			
			EditorGUILayout.Separator();

			EditorGUILayout.LabelField( "Camera 10" + (_default_index == _index++?" (default)":"") );
				EditorGUI.indentLevel++;			
					m_camera_switch.Camera0 = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.Camera0, typeof(GameObject), true);
					m_camera_switch.CameraKey0 = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.CameraKey0 );
				EditorGUI.indentLevel--;			
			EditorGUILayout.Separator();

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField( "Map Camera" );
			EditorGUI.indentLevel++;
				m_camera_switch.MapCamera = (GameObject)EditorGUILayout.ObjectField("Camera Object", m_camera_switch.MapCamera, typeof(GameObject), true);
				m_camera_switch.MapCameraKey = (KeyCode) ICEEditorLayout.EnumPopup( "Key","",  m_camera_switch.MapCameraKey );
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
		}
	}
}
