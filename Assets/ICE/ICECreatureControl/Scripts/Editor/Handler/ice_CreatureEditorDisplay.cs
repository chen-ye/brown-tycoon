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
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures.EditorInfos;

namespace ICE.Creatures.EditorHandler
{
	public static class EditorDisplay
	{
		public static void Print( DisplayData _display )
		{
			Header( _display );
			Options( _display );
			Footer( _display );
		}

		private static void Header( DisplayData _display )
		{
			ICEEditorLayout.BeginHorizontal();
			
			string _global_info = " (local)";
			
			if( _display.UseGlobalAll )
				_global_info = " (all global)";
			else if( _display.UseGlobal )
				_global_info = " (global)";
			
			_display.DisplayOptions = (DisplayOptionType)ICEEditorLayout.EnumPopup("Display Options" + _global_info,"", _display.DisplayOptions );
			
			string _use_global_all_title = "ALL GLOBAL";
			if( _display.UseGlobalAll )
				_use_global_all_title = "LOCAL";
			
			if( _display.UseGlobalAll )
				GUI.backgroundColor = Color.yellow;
			
			if (GUILayout.Button( _use_global_all_title, ICEEditorStyle.ButtonLarge ))
			{
				if( ! _display.UseGlobalAll )
					_display.SetLocalToGlobal();
				else
					_display.SetGlobalToLocal();

				_display.UseGlobalAll = ! _display.UseGlobalAll;
			}
			
			GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
			
			ICEEditorLayout.EndHorizontal( Info.DISPLAY_OPTIONS );			
			HandleQuickChange( _display);	

			/*
			EditorGUI.indentLevel++;
				if( ! _display.UseGlobalAll )
					_display.UseGlobal = EditorGUILayout.ToggleLeft( "Use Global Display Options", _display.UseGlobal );
			EditorGUI.indentLevel--;
			*/

			_display.ShowMissionsHome = true;
			_display.ShowMissionsEscort = true;
			_display.ShowMissionsPatrol = true;

			_display.ShowBehaviourMove = true;
			_display.ShowBehaviourAudio = true;
			_display.ShowBehaviourInluences = true;
			_display.ShowBehaviourEffect = true;
			_display.ShowBehaviourLink = true;
		}

		private static void Footer( DisplayData _display )
		{
			EditorGUI.indentLevel++;
				_display.ShowHelp = ICEEditorLayout.ToggleLeft("Show Help","Displays all help informations", _display.ShowHelp, false );
				_display.ShowDebug = ICEEditorLayout.ToggleLeft("Show Debug","Displays debug informations and gizmos",  _display.ShowDebug, false );
				_display.ShowInfo = ICEEditorLayout.ToggleLeft("Show Info","Displays creature informations",  _display.ShowInfo, false );
			EditorGUI.indentLevel--;

		}

		private static void Options( DisplayData _display )
		{
			if( _display.DisplayOptions == DisplayOptionType.START )
			{
				_display.ShowEssentials = true;
				_display.ShowStatus = false;
				_display.ShowMissions = false;
				_display.ShowBehaviour = false;
				_display.ShowInteractionSettings = false;
				_display.ShowEnvironmentSettings = false;
			}
			else if( _display.DisplayOptions == DisplayOptionType.BASIC )
			{
				_display.ShowEssentials = true;
				_display.ShowStatus = true;
				_display.ShowMissions = true;
				_display.ShowBehaviour = false;
				_display.ShowInteractionSettings = false;
				_display.ShowEnvironmentSettings = false;
			}
			else if( _display.DisplayOptions == DisplayOptionType.FULL )
			{
				_display.ShowEssentials = true;
				_display.ShowStatus = true;
				_display.ShowMissions = true;
				_display.ShowBehaviour = true;
				_display.ShowInteractionSettings = true;
				_display.ShowEnvironmentSettings = true;
			}
		}

		private static void HandleQuickChange( DisplayData _display )
		{
			if( _display.DisplayOptions == DisplayOptionType.MENU )
			{
				EditorGUILayout.Separator();
				
				ICEEditorLayout.BeginHorizontal();
				
				GUILayout.FlexibleSpace();
				if (GUILayout.Button( "ESSENTIALS", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180) ))
				{
					_display.ShowEssentials = true;
					_display.ShowStatus = false;
					_display.ShowMissions = false;
					_display.ShowBehaviour = false;
					_display.ShowInteractionSettings = false;
					_display.ShowEnvironmentSettings = false;
				}
				
				if (GUILayout.Button( "STATUS", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
				{
					_display.ShowEssentials = false;
					_display.ShowStatus = true;
					_display.ShowMissions = false;
					_display.ShowBehaviour = false;
					_display.ShowInteractionSettings = false;
					_display.ShowEnvironmentSettings = false;
				}

				if (GUILayout.Button( "MISSIONS", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
				{
					_display.ShowEssentials = false;
					_display.ShowStatus = false;
					_display.ShowMissions = true;
					_display.ShowBehaviour = false;
					_display.ShowInteractionSettings = false;
					_display.ShowEnvironmentSettings = false;
				}

				GUILayout.FlexibleSpace();
				ICEEditorLayout.EndHorizontal();
				
				ICEEditorLayout.BeginHorizontal();
				
					GUILayout.FlexibleSpace();

					if (GUILayout.Button( "INTERACTION", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
					{
						_display.ShowEssentials = false;
						_display.ShowStatus = false;
						_display.ShowMissions = false;
						_display.ShowBehaviour = false;
						_display.ShowInteractionSettings = true;
						_display.ShowEnvironmentSettings = false;
					}

					if (GUILayout.Button( "ENVIRONMENT", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
					{
						_display.ShowEssentials = false;
						_display.ShowStatus = false;
						_display.ShowMissions = false;
						_display.ShowBehaviour = false;
						_display.ShowInteractionSettings = false;
						_display.ShowEnvironmentSettings = true;
					}

					if (GUILayout.Button( "BEHAVIOURS", GUILayout.MinWidth(80),  GUILayout.MaxWidth(180)  ))
					{
						_display.ShowEssentials = false;
						_display.ShowStatus = false;
						_display.ShowMissions = false;
						_display.ShowBehaviour = true;
						_display.ShowInteractionSettings = false;
						_display.ShowEnvironmentSettings = false;
					}

					GUILayout.FlexibleSpace();
				ICEEditorLayout.EndHorizontal();
				
				EditorGUILayout.Separator();
				
			}
			
		}
		

	}
}
