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
using ICE.Creatures.EditorInfos;
using ICE.Styles;
using ICE.Layouts;

namespace ICE.Creatures
{
	
	[CustomEditor(typeof(ICECreatureControlDebug))]
	[CanEditMultipleObjects]
	public class ICECreatureControlDebugEditor : Editor
	{
		ICECreatureControlDebug m_creature_debug = null;

		public virtual void OnEnable()
		{
			m_creature_debug = (ICECreatureControlDebug)target;
		}

		public override void OnInspectorGUI()
		{
			GUI.changed = false;

			EditorGUILayout.Separator();
			m_creature_debug.CreatureDebug.DebugLogEnabled = ICEEditorLayout.ToggleLeft("Use Debug Log", "", m_creature_debug.CreatureDebug.DebugLogEnabled, true);

			DrawGizmos();
			DrawPointer();

			if (GUI.changed )
				EditorUtility.SetDirty( m_creature_debug );
		}

		void DrawGizmos()
		{
			m_creature_debug.CreatureDebug.Gizmos.Enabled = ICEEditorLayout.ToggleLeft("Use Gizmos", "", m_creature_debug.CreatureDebug.Gizmos.Enabled, true );
			
			if( m_creature_debug.CreatureDebug.Gizmos.Enabled )
			{
								
				EditorGUI.indentLevel++;
				
				
				
					ICEEditorLayout.BeginHorizontal();

					/*
					LabelType label = (LabelType)ICEEditorLayout.EnumPopup ("Labels", m_creature_debug.Debug.Gizmos.Label );
					
					if( label != m_creature_debug.Debug.Gizmos.Label )
					{
						m_creature_debug.Debug.Gizmos.Label = label;
						ICEEditorLayout.AssignLabel( _creature_control.gameObject, (int)m_creature_debug.Debug.Gizmos.Label );
					}*/
					
					ICEEditorLayout.EndHorizontal();
					
					//EditorGUILayout.Separator();
					
					//m_creature_debug.CreatureDebug.Gizmos.ShowText = EditorGUILayout.Toggle( "Show Text", m_creature_debug.CreatureDebug.Gizmos.ShowText );
					m_creature_debug.CreatureDebug.Gizmos.Level = ICEEditorLayout.Slider( "Gizmos Offset", "", m_creature_debug.CreatureDebug.Gizmos.Level, 0.5f, 0,50 );
					
					EditorGUILayout.Separator();
					ICEEditorLayout.Label( "Move Gizmos", true );
					EditorGUI.indentLevel++;					
		
						m_creature_debug.CreatureDebug.Gizmos.ShowPath = ICEEditorLayout.Toggle( "Path", "", m_creature_debug.CreatureDebug.Gizmos.ShowPath );
								
						EditorGUI.BeginDisabledGroup( m_creature_debug.CreatureDebug.Gizmos.ShowPath == false );
						EditorGUI.indentLevel++;
							m_creature_debug.CreatureDebug.Gizmos.PathPositionsLimit = (int)ICEEditorLayout.DefaultSlider( "Max. Path Length", "", m_creature_debug.CreatureDebug.Gizmos.PathPositionsLimit, 1, 10,10000, 100 );
							m_creature_debug.CreatureDebug.Gizmos.PathPrecision = ICEEditorLayout.DefaultSlider( "Path Precision", "", m_creature_debug.CreatureDebug.Gizmos.PathPrecision, 0.25f, 0,5, 0.5f );
							EditorGUILayout.Separator();
							m_creature_debug.CreatureDebug.Gizmos.MoveProjectedPathColor = ICEEditorLayout.DefaultColor ("Projected Path", "", m_creature_debug.CreatureDebug.Gizmos.MoveProjectedPathColor, Init.GIZMOS_COLOR_PATH_PROJECTED );
							m_creature_debug.CreatureDebug.Gizmos.MovePreviousPathColor = ICEEditorLayout.DefaultColor ("Previous Path", "", m_creature_debug.CreatureDebug.Gizmos.MovePreviousPathColor, Init.GIZMOS_COLOR_PATH_PREVIOUS );
							m_creature_debug.CreatureDebug.Gizmos.MoveCurrentPathColor = ICEEditorLayout.DefaultColor ("Current Path", "", m_creature_debug.CreatureDebug.Gizmos.MoveCurrentPathColor, Init.GIZMOS_COLOR_PATH_CURRENT );
						EditorGUI.indentLevel--;
						EditorGUI.EndDisabledGroup();
						

						EditorGUILayout.Separator();
						m_creature_debug.CreatureDebug.Gizmos.MoveColor = ICEEditorLayout.DefaultColor ("Move", "", m_creature_debug.CreatureDebug.Gizmos.MoveColor, Init.GIZMOS_COLOR_MOVE );
						EditorGUI.indentLevel++;
							m_creature_debug.CreatureDebug.Gizmos.MoveDetourColor = ICEEditorLayout.DefaultColor ("Detour", "", m_creature_debug.CreatureDebug.Gizmos.MoveDetourColor, Init.GIZMOS_COLOR_MOVE_DETOUR );
							m_creature_debug.CreatureDebug.Gizmos.MoveOrbitColor = ICEEditorLayout.DefaultColor ("Orbit", "", m_creature_debug.CreatureDebug.Gizmos.MoveOrbitColor, Init.GIZMOS_COLOR_MOVE_ORBIT );
							m_creature_debug.CreatureDebug.Gizmos.MoveEscapeColor = ICEEditorLayout.DefaultColor ("Escape", "", m_creature_debug.CreatureDebug.Gizmos.MoveEscapeColor, Init.GIZMOS_COLOR_MOVE_ESCAPE );
							m_creature_debug.CreatureDebug.Gizmos.MoveAvoidColor = ICEEditorLayout.DefaultColor ("Avoid", "", m_creature_debug.CreatureDebug.Gizmos.MoveAvoidColor, Init.GIZMOS_COLOR_MOVE_AVOID );
						EditorGUI.indentLevel--;
					EditorGUI.indentLevel--;

					EditorGUILayout.Separator();
					ICEEditorLayout.Label( "Target Gizmos", true );
					EditorGUI.indentLevel++;
						m_creature_debug.CreatureDebug.Gizmos.TargetColor = ICEEditorLayout.DefaultColor ("inactive", "", m_creature_debug.CreatureDebug.Gizmos.TargetColor, Init.GIZMOS_COLOR_TARGET );
						m_creature_debug.CreatureDebug.Gizmos.ActiveTargetColor = ICEEditorLayout.DefaultColor("active", "", m_creature_debug.CreatureDebug.Gizmos.ActiveTargetColor, Init.GIZMOS_COLOR_TARGET_ACTIVE );
					EditorGUI.indentLevel--;	

					EditorGUILayout.Separator();

					m_creature_debug.CreatureDebug.Gizmos.ShowHome = ICEEditorLayout.ToggleLeft( "Home Gizmos", "", m_creature_debug.CreatureDebug.Gizmos.ShowHome, true );
					m_creature_debug.CreatureDebug.Gizmos.ShowOutpost = ICEEditorLayout.ToggleLeft( "Mission Outpost Gizmos", "", m_creature_debug.CreatureDebug.Gizmos.ShowOutpost, true );
					m_creature_debug.CreatureDebug.Gizmos.ShowEscort = ICEEditorLayout.ToggleLeft( "Mission Escort Gizmos", "", m_creature_debug.CreatureDebug.Gizmos.ShowEscort, true );
					m_creature_debug.CreatureDebug.Gizmos.ShowPatrol = ICEEditorLayout.ToggleLeft( "Mission Patrol Gizmos", "", m_creature_debug.CreatureDebug.Gizmos.ShowPatrol, true );


					m_creature_debug.CreatureDebug.Gizmos.ShowInteractor = ICEEditorLayout.ToggleLeft( "Interaction Gizmos", "", m_creature_debug.CreatureDebug.Gizmos.ShowInteractor, true );

					EditorGUI.BeginDisabledGroup( m_creature_debug.CreatureDebug.Gizmos.ShowInteractor == false );
					EditorGUI.indentLevel++;	
						m_creature_debug.CreatureDebug.Gizmos.InteractionColor = ICEEditorLayout.DefaultColor("Interaction", "", m_creature_debug.CreatureDebug.Gizmos.InteractionColor, Init.GIZMOS_COLOR_INTERACTION );
						/*EditorGUI.indentLevel++;
						
							m_creature_debug.CreatureDebug.Gizmos.ShowSolidInteractionRange = ICEEditorLayout.Toggle( "Solid Interaction Range", "", m_creature_debug.CreatureDebug.Gizmos.ShowSolidInteractionRange );

							EditorGUI.BeginDisabledGroup( m_creature_debug.CreatureDebug.Gizmos.ShowSolidInteractionRange == false );
							m_creature_debug.CreatureDebug.Gizmos.SolidInteractionAlpha = ICEEditorLayout.DefaultSlider( "Solid Interaction Range Alpha", "", m_creature_debug.CreatureDebug.Gizmos.SolidInteractionAlpha, 0.005f, 0, 1, Init.GIZMOS_COLOR_INTERACTION_ALPHA);
							EditorGUI.EndDisabledGroup();
				
						EditorGUI.indentLevel--;	*/
					EditorGUI.indentLevel--;	
					EditorGUILayout.Separator();
					EditorGUI.EndDisabledGroup();
				/*
								ICEEditorLayout.BeginHorizontal();
								_creature_control.Action.Move.Gizmos.TargetMovePosition = EditorGUILayout.ColorField ("Waypoint", _creature_control.Action.Move.Gizmos.WaypointColor );
								*/

				
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();
			}

		}

		void DrawPointer()
		{
			ICEEditorLayout.Label( "Runtime Pointer", true );
			EditorGUI.indentLevel++;
				m_creature_debug.CreatureDebug.MovePointer.Enabled = ICEEditorLayout.Toggle("Use Path Pointer", "", m_creature_debug.CreatureDebug.MovePointer.Enabled );
				if( m_creature_debug.CreatureDebug.MovePointer.Enabled )
				{
					EditorGUI.indentLevel++;					
						m_creature_debug.CreatureDebug.MovePointer.PointerType = (PrimitiveType)ICEEditorLayout.EnumPopup ("Type","", m_creature_debug.CreatureDebug.MovePointer.PointerType );
						m_creature_debug.CreatureDebug.MovePointer.PointerSize = EditorGUILayout.Vector3Field ("Size", m_creature_debug.CreatureDebug.MovePointer.PointerSize );
						m_creature_debug.CreatureDebug.MovePointer.PointerColor = ICEEditorLayout.DefaultColor ("Color", "", m_creature_debug.CreatureDebug.MovePointer.PointerColor, Init.GIZMOS_COLOR_MOVE );
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}

				m_creature_debug.CreatureDebug.TargetPositionPointer.Enabled = ICEEditorLayout.Toggle("Use Destination Pointer", "", m_creature_debug.CreatureDebug.TargetPositionPointer.Enabled );
				if( m_creature_debug.CreatureDebug.TargetPositionPointer.Enabled )
				{
					EditorGUI.indentLevel++;					
						m_creature_debug.CreatureDebug.TargetPositionPointer.PointerType = (PrimitiveType)ICEEditorLayout.EnumPopup ("Type","", m_creature_debug.CreatureDebug.TargetPositionPointer.PointerType );
						m_creature_debug.CreatureDebug.TargetPositionPointer.PointerSize = EditorGUILayout.Vector3Field ("Size", m_creature_debug.CreatureDebug.TargetPositionPointer.PointerSize );
						m_creature_debug.CreatureDebug.TargetPositionPointer.PointerColor = ICEEditorLayout.DefaultColor ("Color", "", m_creature_debug.CreatureDebug.TargetPositionPointer.PointerColor, Init.GIZMOS_COLOR_TARGET );
					EditorGUI.indentLevel--;					
					EditorGUILayout.Separator();
				}
			EditorGUI.indentLevel--;			
			EditorGUILayout.Separator();
		}
	}
}
