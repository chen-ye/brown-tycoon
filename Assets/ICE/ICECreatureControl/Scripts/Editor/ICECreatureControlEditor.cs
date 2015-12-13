using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Creatures.EditorHandler;
//using ICE.Creatures.EditorLayouts;
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures.EditorInfos;

namespace ICE.Creatures
{

	[CustomEditor(typeof(ICECreatureControl))]
	public class ICECreatureControlEditor : Editor
	{
		/*
		Color _color_default_background = GUI.backgroundColor;
		Color _color_add = new Color(0.0F, 0.75F, 1.0F, 1);
		Color _color_delete = new Color(1.0F, 0.5F, 0.5F, 1);
		Color _color_global = new Color(0.0F, 0.75F, 1.0F, 1);*/


		//********************************************************************************
		// INITIAL DECLARATION (EDITOR)
		//********************************************************************************
		private static ICECreatureControl m_creature_control;
		//private static ICECreatureRegister m_creature_register;
		private static ICECreatureControlDebug m_creature_debug;

		//********************************************************************************
		// OnEnable
		//********************************************************************************
		public virtual void OnEnable()
		{
			m_creature_control = (ICECreatureControl)target;
			//m_creature_register = FindObjectOfType<ICECreatureRegister>();
			m_creature_debug = m_creature_control.gameObject.GetComponent<ICECreatureControlDebug>();

		}

		//********************************************************************************
		// OnInspectorGUI
		//********************************************************************************
		public override void OnInspectorGUI()
		{
			EditorBehaviour.BehaviourSelectIndex = 0;
			Info.HelpButtonIndex = 0;

			if( m_creature_debug != null )
				m_creature_control.Display.ShowDebug = m_creature_debug.enabled;
			else
				m_creature_control.Display.ShowDebug = false;

			GUI.changed = false;

			EditorGUILayout.Separator();

			Info.HelpEnabled = m_creature_control.Display.ShowHelp;
			Info.DescriptionEnabled = m_creature_control.Display.ShowHelpDescription;

			// COCKPIT
			EditorRegister.Print( m_creature_control.gameObject.name );
			EditorDisplay.Print( m_creature_control.Display );
			EditorInfo.Print( m_creature_control );

			// ESSENTIALS
			EditorEssentials.Print( m_creature_control );

			// STATUS
			EditorStatus.Print( m_creature_control );

			// MISSIONS
			EditorMissions.Print( m_creature_control );

			// INTERACTION
			EditorInteraction.Print( m_creature_control );

			// ENVIRONMENT
			EditorEnvironment.Print( m_creature_control );

			//BEHAVIOURS
			EditorBehaviour.Print( m_creature_control );


			if( m_creature_control.Display.ShowDebug )
			{
				if( m_creature_debug == null )
					m_creature_debug = m_creature_control.gameObject.AddComponent<ICECreatureControlDebug>();
				else if( m_creature_debug.enabled == false )
					m_creature_debug.enabled = true;

			}
			else if( m_creature_debug != null )
			{
				m_creature_debug.enabled = false;
				/*
				DestroyImmediate( m_creature_control.GetComponent<ICECreatureControlDebug>() );
				EditorGUIUtility.ExitGUI();*/

			}

			if (GUI.changed)
				EditorUtility.SetDirty( m_creature_control );

		}
	}
}
