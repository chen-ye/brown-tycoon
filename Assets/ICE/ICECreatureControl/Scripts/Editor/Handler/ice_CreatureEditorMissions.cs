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
using ICE.Creatures.EditorInfos;
using ICE.Styles;
using ICE.Layouts;




namespace ICE.Creatures.EditorHandler
{
	
	public static class EditorMissions
	{	
		public static void Print( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowMissions )
				return;

			ICEEditorStyle.SplitterByIndent( 0 );
			_control.Display.FoldoutMissions = ICEEditorLayout.Foldout( _control.Display.FoldoutMissions, "Missions", Info.MISSIONS );
			
			if( ! _control.Display.FoldoutMissions ) 
				return;

			EditorGUI.indentLevel++;

			DrawMissionOutpost( _control );
			DrawMissionEscort( _control );
			DrawMissionPatrol( _control );

			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();
		}

		private static void DrawMissionOutpost( ICECreatureControl _control ){
			
			if( _control.Display.ShowMissionsHome == false )
				return;
			
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
			_control.Display.FoldoutMissionOutpost = ICEEditorLayout.Foldout( _control.Display.FoldoutMissionOutpost, "Outpost Mission", Info.MISSION_OUTPOST );
			
			if ( ! _control.Display.FoldoutMissionOutpost) 
				return;
					
			_control.Creature.Missions.Outpost.Enabled = ICEEditorLayout.Toggle("Mission Enabled","", _control.Creature.Missions.Outpost.Enabled, Info.MISSION_ENABLED );

			EditorGUILayout.Separator();
			EditorGUI.BeginDisabledGroup ( _control.Creature.Missions.Outpost.Enabled == false);				
				_control.Creature.Missions.Outpost.Target = EditorSharedTools.DrawTarget( _control, _control.Creature.Missions.Outpost.Target, "Target", Info.MISSION_OUTPOST_TARGET );
				
				EditorGUILayout.Separator();				
				EditorGUILayout.LabelField( "Behaviour", ICEEditorStyle.LabelBold );				
				EditorGUI.indentLevel++;		
					_control.Creature.Missions.Outpost.BehaviourModeTravel = EditorBehaviour.BehaviourSelect( _control, "Travel", "Move behaviour to reach the Outpost", _control.Creature.Missions.Outpost.BehaviourModeTravel , "TRAVEL" ); 
					_control.Creature.Missions.Outpost.BehaviourModeRendezvous = EditorBehaviour.BehaviourSelect( _control, "Rendezvous", "Idle behaviour after reaching the current target move position.", _control.Creature.Missions.Outpost.BehaviourModeRendezvous, "RENDEZVOUS" ); 
					EditorGUI.BeginDisabledGroup( _control.Creature.Missions.Outpost.Target.TargetRandomRange == 0 );
						_control.Creature.Missions.Outpost.BehaviourModeLeisure = EditorBehaviour.BehaviourSelect( _control, "Leisure", "Randomized leisure activities around the Outpost", _control.Creature.Missions.Outpost.BehaviourModeLeisure, "STANDBY" ); 
					EditorGUI.EndDisabledGroup();		
				EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Separator();

		}

		private static void DrawMissionEscort( ICECreatureControl _control  ){
			
			if( _control.Display.ShowMissionsEscort == false )
				return;
			
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
			ICEEditorLayout.BeginHorizontal();
				_control.Display.FoldoutMissionEscort = ICEEditorLayout.Foldout( _control.Display.FoldoutMissionEscort, "Escort Mission" );
				if (GUILayout.Button("SAVE", ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveMissionEscortToFile( _control.Creature.Missions.Escort, _control.gameObject.name );
				if (GUILayout.Button("LOAD", ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Missions.Escort = CreatureIO.LoadMissionEscortFromFile( _control.Creature.Missions.Escort );
			ICEEditorLayout.EndHorizontal( Info.MISSION_ESCORT );
			
			if ( ! _control.Display.FoldoutMissionEscort ) 
				return;
				
			_control.Creature.Missions.Escort.Enabled = ICEEditorLayout.Toggle("Mission Enabled","", _control.Creature.Missions.Escort.Enabled, Info.MISSION_ENABLED );
			EditorGUILayout.Separator();
			EditorGUI.BeginDisabledGroup ( _control.Creature.Missions.Escort.Enabled == false);
			
				_control.Creature.Missions.Escort.Target = EditorSharedTools.DrawTarget( _control, _control.Creature.Missions.Escort.Target, "Target", Info.MISSION_ESCORT_TARGET );
				
				EditorGUILayout.Separator();
				
				ICEEditorLayout.Label( "Behaviour", true, Info.MISSION_ESCORT_BEHAVIOUR );
				
				EditorGUI.indentLevel++;
				
				_control.Creature.Missions.Escort.BehaviourModeFollow = EditorBehaviour.BehaviourSelect( _control, "Follow", "Move behaviour to follow and reach the leader", _control.Creature.Missions.Escort.BehaviourModeFollow, "FOLLOW" );
				_control.Creature.Missions.Escort.BehaviourModeEscort = EditorBehaviour.BehaviourSelect( _control, "Escort", "Move behaviour to escort the leader", _control.Creature.Missions.Escort.BehaviourModeEscort, "ESCORT" );		
				_control.Creature.Missions.Escort.BehaviourModeStandby = EditorBehaviour.BehaviourSelect( _control, "Standby", "Idle behaviour if the leader stops", _control.Creature.Missions.Escort.BehaviourModeStandby, "STANDBY" );
				
				EditorGUI.indentLevel++;
				
				_control.Creature.Missions.Escort.DurationStandby = ICEEditorLayout.Slider( "Duration (until IDLE)", "", _control.Creature.Missions.Escort.DurationStandby, 1, 0, 60 );
				
				EditorGUI.indentLevel--;
				
				_control.Creature.Missions.Escort.BehaviourModeIdle = EditorBehaviour.BehaviourSelect( _control, "Idle", "Idle behaviour if the leader breaks for a longer time-span", _control.Creature.Missions.Escort.BehaviourModeIdle, "IDLE" );
				
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();
			
			EditorGUI.EndDisabledGroup();

		}
		
		private static void DrawMissionPatrol( ICECreatureControl _control )
		{
			if( _control.Display.ShowMissionsPatrol == false )
				return;
			
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
			_control.Display.FoldoutMissionPatrol = ICEEditorLayout.Foldout( _control.Display.FoldoutMissionPatrol, "Patrol Mission", Info.MISSION_PATROL );
			
			if( ! _control.Display.FoldoutMissionPatrol ) 
				return;

			_control.Creature.Missions.Patrol.Enabled = ICEEditorLayout.Toggle("Mission Enabled","", _control.Creature.Missions.Patrol.Enabled, Info.MISSION_ENABLED );
			EditorGUILayout.Separator();
			EditorGUI.BeginDisabledGroup ( _control.Creature.Missions.Patrol.Enabled == false);
			
				ICEEditorLayout.BeginHorizontal();
					_control.Creature.Missions.Patrol.Waypoints.Order = (WaypointOrderType)ICEEditorLayout.EnumPopup("Order Type","", _control.Creature.Missions.Patrol.Waypoints.Order );
					EditorGUI.BeginDisabledGroup( _control.Creature.Missions.Patrol.Waypoints.Order != WaypointOrderType.CYCLE );
						_control.Creature.Missions.Patrol.Waypoints.Ascending = ! ICEEditorLayout.ButtonCheck( "DESC", "descending order", ! _control.Creature.Missions.Patrol.Waypoints.Ascending, ICEEditorStyle.CMDButtonDouble );
					EditorGUI.EndDisabledGroup();	
				ICEEditorLayout.EndHorizontal( Info.MISSION_PATROL_ORDER_TYPE);

				EditorGUILayout.Separator();					
				EditorGUILayout.LabelField( "Behaviour", ICEEditorStyle.LabelBold );
				
				EditorGUI.indentLevel++;					
					_control.Creature.Missions.Patrol.BehaviourModeTravel = EditorBehaviour.BehaviourSelect( _control, "Travel", "Default travel behaviour to reach the first waypoint",  _control.Creature.Missions.Patrol.BehaviourModeTravel, "WP_TRAVEL" );
					_control.Creature.Missions.Patrol.BehaviourModePatrol = EditorBehaviour.BehaviourSelect( _control, "Patrol","Default patrol behaviour to reach the next waypoint", _control.Creature.Missions.Patrol.BehaviourModePatrol, "WP_PATROL" );
					EditorSharedTools.DrawInRangeBehaviour( _control,
                       ref _control.Creature.Missions.Patrol.BehaviourModeLeisure,
                       ref _control.Creature.Missions.Patrol.BehaviourModeRendezvous,
                       ref _control.Creature.Missions.Patrol.DurationOfStay,
                       ref _control.Creature.Missions.Patrol.IsTransitPoint,
                       1 );
				
				/*
					EditorGUI.BeginDisabledGroup( m_creature_control.Creature.Missions.Patrol.IsTransitPoint == true );
						m_creature_control.Creature.Missions.Patrol.BehaviourModeLeisure = EditorBehaviour.BehaviourSelect( m_creature_control, "Leisure", "Leisure activities after reaching this waypoint range", m_creature_control.Creature.Missions.Patrol.BehaviourModeLeisure, "WP_LEISURE" );
						m_creature_control.Creature.Missions.Patrol.BehaviourModeRendezvous = EditorBehaviour.BehaviourSelect( m_creature_control, "Rendezvous", "Action behaviour after reaching this current waypoint target position", m_creature_control.Creature.Missions.Patrol.BehaviourModeRendezvous, "WP_RENDEZVOUS" );
					EditorGUI.EndDisabledGroup();

				m_creature_control.Creature.Missions.Patrol.DurationOfStay = ICEEditorLayout.DurationSlider( "Duration Of Stay", "Duration of stay", m_creature_control.Creature.Missions.Patrol.DurationOfStay, Init.DURATION_OF_STAY_STEP, Init.DURATION_OF_STAY_MIN, Init.DURATION_OF_STAY_MAX,Init.DURATION_OF_STAY_DEFAULT, ref m_creature_control.Creature.Missions.Patrol.IsTransitPoint );
*/
				
				EditorGUI.indentLevel--;
					
				EditorGUILayout.Separator();					
				ICEEditorLayout.Label( "Waypoints", true, Info.MISSION_PATROL_WAYPOINTS );					
				EditorGUI.indentLevel++;					
					ICEEditorLayout.BeginHorizontal();
						_control.Creature.Missions.Patrol.Waypoints.WaypointGroup  = (GameObject)EditorGUILayout.ObjectField( "Add Waypoint Group", _control.Creature.Missions.Patrol.Waypoints.WaypointGroup, typeof(GameObject), true);
						if (GUILayout.Button("REFRESH", ICEEditorStyle.ButtonLarge ))
							_control.Creature.Missions.Patrol.Waypoints.UpdateWaypointGroup();
						ICEEditorLayout.EndHorizontal( Info.MISSION_PATROL_ADD_WAYPOINT_GROUP );					
					ICEEditorLayout.BeginHorizontal();	
						ICEEditorLayout.Label( "Add Single Waypoint", false );
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("ADD WAYPOINT", ICEEditorStyle.ButtonLarge ))
							_control.Creature.Missions.Patrol.Waypoints.Waypoints.Add( new WaypointObject() );
						ICEEditorLayout.EndHorizontal( Info.MISSION_PATROL_ADD_WAYPOINT );									
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();					
				for (int i = 0; i < _control.Creature.Missions.Patrol.Waypoints.Waypoints.Count ; ++i)
					DrawMissionPatrolWaypoint( _control, i );	
								
				EditorGUILayout.Separator();				
			EditorGUI.EndDisabledGroup();

		}

		private static void DrawMissionPatrolWaypoint( ICECreatureControl _control, int _index )
		{
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );
			WaypointObject _waypoint = _control.Creature.Missions.Patrol.Waypoints.Waypoints[ _index ];
			
			if( _waypoint == null )
				return;

			// HEADER BEGIN
			ICEEditorLayout.BeginHorizontal();				
				_waypoint.Enabled = EditorGUILayout.ToggleLeft( "Waypoint #"+ (int)(_index + 1), _waypoint.Enabled, EditorStyles.boldLabel );
				GUILayout.FlexibleSpace();
				
				if( ICEEditorLayout.ButtonUp() )
				{
					WaypointObject _wp = _control.Creature.Missions.Patrol.Waypoints.Waypoints[_index]; 
					_control.Creature.Missions.Patrol.Waypoints.Waypoints.RemoveAt( _index );				
					if( _index - 1 < 0 )
						_control.Creature.Missions.Patrol.Waypoints.Waypoints.Add( _wp );
					else
						_control.Creature.Missions.Patrol.Waypoints.Waypoints.Insert( _index - 1, _wp );				
					return;
				}	
				
				if( ICEEditorLayout.ButtonDown() )
				{
					WaypointObject _wp = _control.Creature.Missions.Patrol.Waypoints.Waypoints[_index]; 
					_control.Creature.Missions.Patrol.Waypoints.Waypoints.RemoveAt( _index );				
					if( _index + 1 > _control.Creature.Missions.Patrol.Waypoints.Waypoints.Count )
						_control.Creature.Missions.Patrol.Waypoints.Waypoints.Insert( 0, _wp );
					else
						_control.Creature.Missions.Patrol.Waypoints.Waypoints.Insert( _index +1, _wp );				
					return;
				}	
				
				if( ICEEditorLayout.ButtonCloseDouble() )
				{
					_control.Creature.Missions.Patrol.Waypoints.Waypoints.RemoveAt( _index );
					--_index;
				}			
			ICEEditorLayout.EndHorizontal( Info.MISSION_PATROL_WAYPOINT );
			// HEADER END

			// CONTENT BEGIN
			EditorGUI.BeginDisabledGroup ( _waypoint.Enabled == false);				
				EditorGUI.indentLevel++;				
					_waypoint = (WaypointObject)EditorSharedTools.DrawTarget( _control, (TargetObject)_waypoint, "Target", Info.MISSION_PATROL_TARGET );
				
					EditorGUILayout.Separator();				
					_waypoint.UseCustomBehaviour = ICEEditorLayout.Toggle( "Custom Behaviour","", _waypoint.UseCustomBehaviour, Info.MISSION_PATROL_CUSTOM_BEHAVIOUR );
				
					EditorGUILayout.Separator();
				
					if( _waypoint.UseCustomBehaviour )
					{
						EditorGUI.indentLevel++;
						
						
						_waypoint.BehaviourModeTravel = EditorBehaviour.BehaviourSelect( _control, "Travel", "Travel behaviour to reach this waypoint and to start this mission", _waypoint.BehaviourModeTravel, "WP_TRAVEL_" + (int)(_index + 1) );
						_waypoint.BehaviourModePatrol = EditorBehaviour.BehaviourSelect( _control, "Patrol", "Patrol behaviour to reach this waypoint", _waypoint.BehaviourModePatrol, "WP_PATROL_" + (int)(_index + 1) );
						
						EditorSharedTools.DrawInRangeBehaviour( _control,
						                                       ref _waypoint.BehaviourModeLeisure,
						                                       ref _waypoint.BehaviourModeRendezvous,
						                                       ref _waypoint.DurationOfStay,
						                                       ref _waypoint.IsTransitPoint,
						                                       _waypoint.TargetRandomRange,
				                                     		  (int)(_index + 1) );
						
						
						
						/*
						EditorGUI.BeginDisabledGroup( _waypoint.IsTransitPoint == true );

										_waypoint.BehaviourModeLeisure = EditorBehaviour.BehaviourSelect( m_creature_control, "Leisure", "Leisure activities after reaching this waypoint range", _waypoint.BehaviourModeLeisure, "WP_LEISURE_" + _index );
										_waypoint.BehaviourModeRendezvous = EditorBehaviour.BehaviourSelect( m_creature_control, "Rendezvous", "Action behaviour after reaching this current target move position", _waypoint.BehaviourModeRendezvous, "WP_RENDEZVOUS_" + _index );
									EditorGUI.EndDisabledGroup();
						
									_waypoint.DurationOfStay = ICEEditorLayout.DurationSlider( "Duration Of Stay", "Duration of stay", _waypoint.DurationOfStay, Init.DURATION_OF_STAY_STEP, Init.DURATION_OF_STAY_MIN, Init.DURATION_OF_STAY_MAX,Init.DURATION_OF_STAY_DEFAULT, ref _waypoint.IsTransitPoint );

							*/
						EditorGUI.indentLevel--;
						
						EditorGUILayout.Separator();
					}
				
				EditorGUI.indentLevel--;
			
			EditorGUI.EndDisabledGroup();
			// CONTENT END
			
		}
		
		/*
		private static void DrawMissionScout( ICECreatureControl _control )
		{
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
			_control.Creature.Missions.Scout.Foldout = ICEEditorLayout.Foldout(_control.Creature.Missions.Scout.Foldout, "Scout Mission", Info.MISSION_SCOUT );
			
			if (_control.Creature.Missions.Scout.Foldout) {

				EditorGUILayout.Separator();
				
				_control.Creature.Missions.Scout.Enabled = EditorGUILayout.ToggleLeft("Scout Mission Enabled", _control.Creature.Missions.Scout.Enabled );
				
				EditorGUILayout.Separator();
				
				
			}
		}*/



	}
}
