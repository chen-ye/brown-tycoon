using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;

using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;

namespace ICE.Creatures
{
	[CustomEditor(typeof(ICECreatureLabel))]
	public class ICECreatureLabelEditor : Editor 
	{
		private static ICECreatureLabel m_creature_label;
		//private static ICECreatureControl m_creature_control;
		//private static ICECreatureRegister m_creature_register;

		float _height = 0;

		public virtual void OnEnable()
		{
			m_creature_label = (ICECreatureLabel)target;
			//m_creature_register = ICECreatureRegister.Register;
			//m_creature_control = m_creature_label.gameObject.GetComponent<ICECreatureControl>();

			Renderer _renderer = m_creature_label.transform.GetComponentInChildren<Renderer>();

			if( _renderer != null )
			{
				_height = _renderer.bounds.size.y;
			}
		}

		public override void OnInspectorGUI()
		{
			GUI.changed = false;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Camera", true);
			EditorGUI.indentLevel++;
			//m_creature_label.CameraName = EditorGUILayout.TextField( "", m_creature_label.CameraName );

			m_creature_label.CameraName = ICEEditorLayout.CameraPopup( "Camera", "", m_creature_label.CameraName );
			EditorGUI.indentLevel--;

	

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Text", true);
			EditorGUI.indentLevel++;
			m_creature_label.LabelText = EditorGUILayout.TextArea( m_creature_label.LabelText, GUILayout.Height(50) );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Offset", true);
			EditorGUI.indentLevel++;
			m_creature_label.LabelVerticalOffset = ICEEditorLayout.DefaultSlider( "Vertical", "Vertical Offset", m_creature_label.LabelVerticalOffset, 0.1f, 0,5, _height + 0.1f );
			m_creature_label.LabelHorizontalOffset = ICEEditorLayout.DefaultSlider( "Horizontal", "Horizontal Offset", m_creature_label.LabelHorizontalOffset, 0.1f, -5,5, 0 );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Font", true);
			EditorGUI.indentLevel++;
			m_creature_label.LabelColor = ICEEditorLayout.DefaultColor( "Color", "", m_creature_label.LabelColor, Color.white );
			m_creature_label.LabelStyle = (FontStyle)ICEEditorLayout.EnumPopup( "Font Style","", m_creature_label.LabelStyle );
			m_creature_label.LabelAnchor = (TextAnchor)ICEEditorLayout.EnumPopup( "Anchor","", m_creature_label.LabelAnchor );
			m_creature_label.LabelAlignment = (TextAlignment)ICEEditorLayout.EnumPopup( "Alignment","", m_creature_label.LabelAlignment );
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Visibility", true);
			EditorGUI.indentLevel++;
			m_creature_label.MaxDistance = ICEEditorLayout.DefaultSlider( "Max. Distance", "Maximum distance within which the label is visisble.", m_creature_label.MaxDistance, 0.5f, 1, 500, 100 );
			m_creature_label.ClampToScreen = ICEEditorLayout.Toggle( "Always On Screen", "Displays the Label always on screen.", m_creature_label.ClampToScreen );
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Separator();
			if (GUI.changed)
				EditorUtility.SetDirty( m_creature_label );

		}
	}
}
