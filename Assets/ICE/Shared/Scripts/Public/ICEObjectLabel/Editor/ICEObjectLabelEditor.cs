using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE.Styles;
using ICE.Layouts;

namespace ICE.Shared
{
	[CustomEditor(typeof(ICEObjectLabel))]
	[CanEditMultipleObjects]
	public class ICEObjectLabelEditor : Editor 
	{
		private static ICEObjectLabel m_object_label;

		float _height = 0;

		public virtual void OnEnable()
		{
			m_object_label = (ICEObjectLabel)target;

			Renderer _renderer = m_object_label.transform.GetComponentInChildren<Renderer>();

			if( _renderer != null )
				_height = _renderer.bounds.size.y;
		}

		public override void OnInspectorGUI()
		{
			GUI.changed = false;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Camera", true);
			EditorGUI.indentLevel++;
				m_object_label.CameraName = ICEEditorLayout.CameraPopup( "Camera", "", m_object_label.CameraName );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Text", true);
			EditorGUI.indentLevel++;
				m_object_label.LabelText = EditorGUILayout.TextArea( m_object_label.LabelText, GUILayout.Height(50) );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Offset", true);
			EditorGUI.indentLevel++;
				m_object_label.LabelVerticalOffset = ICEEditorLayout.DefaultSlider( "Vertical", "Vertical Offset", m_object_label.LabelVerticalOffset, 0.1f, 0,5, _height + 0.1f );
				m_object_label.LabelHorizontalOffset = ICEEditorLayout.DefaultSlider( "Horizontal", "Horizontal Offset", m_object_label.LabelHorizontalOffset, 0.1f, -5,5, 0 );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Font", true);
			EditorGUI.indentLevel++;
				m_object_label.LabelColor = ICEEditorLayout.DefaultColor( "Color", "", m_object_label.LabelColor, Color.white );
				m_object_label.LabelStyle = (FontStyle)ICEEditorLayout.EnumPopup( "Font Style", "", m_object_label.LabelStyle );
				m_object_label.LabelAnchor = (TextAnchor)ICEEditorLayout.EnumPopup( "Anchor", "", m_object_label.LabelAnchor );
				m_object_label.LabelAlignment = (TextAlignment)ICEEditorLayout.EnumPopup( "Alignment", "", m_object_label.LabelAlignment );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Visibility", true);
			EditorGUI.indentLevel++;
				m_object_label.MaxDistance = ICEEditorLayout.DefaultSlider( "Max. Distance", "Maximum distance within which the label is visisble.", m_object_label.MaxDistance, 0.5f, 1, 500, 100 );
				m_object_label.ClampToScreen = ICEEditorLayout.Toggle( "Always On Screen", "Displays the Label always on screen.", m_object_label.ClampToScreen );
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Separator();

			if (GUI.changed)
				EditorUtility.SetDirty( m_object_label );

		}
	}
}
