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
	
	public static class EditorEssentials
	{	
		public static void Print( ICECreatureControl _creature_control )
		{
			if( ! _creature_control.Display.ShowEssentials )
				return;

			ICEEditorStyle.SplitterByIndent( 0 );
			_creature_control.Display.FoldoutEssentials = ICEEditorLayout.Foldout( _creature_control.Display.FoldoutEssentials , "Essentials", Info.ESSENTIALS );
			
			if( ! _creature_control.Display.FoldoutEssentials ) 
				return;

			HandleEssentialSettings( _creature_control );
			HandleSystemSettings( _creature_control );

			
		
		}

		/// <summary>
		/// Handles the system settings.
		/// </summary>
		/// <param name="_creature_control">_creature_control.</param>
		private static void HandleSystemSettings( ICECreatureControl _control )
		{
			EditorGUILayout.Separator();


			ICEEditorLayout.Label( "Motion and Pathfinding", true, Info.ESSENTIALS_SYSTEM );
			EditorGUI.indentLevel++;

				ICEEditorLayout.BeginHorizontal();
					_control.Creature.Move.GroundCheck = (GroundCheckType)ICEEditorLayout.EnumPopup("Ground Check", "Method to handle ground related checks and movements", _control.Creature.Move.GroundCheck );
					if( _control.Creature.Move.GroundCheck == GroundCheckType.RAYCAST )
					{
						if (GUILayout.Button("Add Layer", ICEEditorStyle.ButtonMiddle ))
							_control.Creature.Move.GroundLayers.Add( "Default" );
						
					}				
				ICEEditorLayout.EndHorizontal( Info.ESSENTIALS_SYSTEM_GROUND_CHECK );
				
				if( _control.Creature.Move.GroundCheck == GroundCheckType.RAYCAST )
				{
					EditorGUI.indentLevel++;
					DrawGroundLayers( _control.Creature.Move.GroundLayers );
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.Separator();

				if( _control.Creature.Move.GroundOrientation == GroundOrientationType.NONE  || _control.Creature.Move.GroundOrientation == GroundOrientationType.BIPED )
				{
					_control.Creature.Move.GroundOrientationPlus = false;
				}
		
				ICEEditorLayout.BeginHorizontal();
					_control.Creature.Move.GroundOrientation = (GroundOrientationType)ICEEditorLayout.EnumPopup("Ground Orientation", "Vertical direction relative to the ground", _control.Creature.Move.GroundOrientation );
					EditorGUI.BeginDisabledGroup( _control.Creature.Move.GroundOrientation == GroundOrientationType.NONE  || _control.Creature.Move.GroundOrientation == GroundOrientationType.BIPED );
						_control.Creature.Move.GroundOrientationPlus = ICEEditorLayout.ButtonCheck( "PLUS", "", _control.Creature.Move.GroundOrientationPlus, ICEEditorStyle.CMDButtonDouble );
					EditorGUI.EndDisabledGroup();
				ICEEditorLayout.EndHorizontal( Info.ESSENTIALS_SYSTEM_GROUND_ORIENTATION );

				EditorGUI.BeginDisabledGroup( _control.Creature.Move.GroundOrientation == GroundOrientationType.NONE );
				EditorGUI.indentLevel++;

					if( _control.Creature.Move.GroundOrientationPlus )
					{
						_control.Creature.Move.Width = ICEEditorLayout.DefaultSlider( "Width", "", _control.Creature.Move.Width, 0.01f, 0, 45, ( _control.GetComponentInChildren<Renderer>().bounds.size.x / _control.transform.lossyScale.x ) );
						EditorGUI.indentLevel++;
							_control.Creature.Move.WidthOffset = ICEEditorLayout.DefaultSlider( "x-Offset", "", _control.Creature.Move.WidthOffset, 0.01f, -10, 10, 0 );
						EditorGUI.indentLevel--;
						_control.Creature.Move.Depth = ICEEditorLayout.DefaultSlider( "Depth", "", _control.Creature.Move.Depth, 0.01f, 0, 45, ( _control.GetComponentInChildren<Renderer>().bounds.size.z / _control.transform.lossyScale.z ) );
						EditorGUI.indentLevel++;
							_control.Creature.Move.DepthOffset = ICEEditorLayout.DefaultSlider( "z-Offset", "", _control.Creature.Move.DepthOffset, 0.01f, -10, 10, 0 );
						EditorGUI.indentLevel--;
					
					}

					_control.Creature.Move.BaseOffset = ICEEditorLayout.DefaultSlider( "Base Offset", "", _control.Creature.Move.BaseOffset, 0.01f, -1, 1,0, Info.ESSENTIALS_SYSTEM_BASE_OFFSET ); 
					_control.Creature.Move.UseLeaningTurn = ICEEditorLayout.Toggle("Use Leaning Turn", "Allows to lean into a turn", _control.Creature.Move.UseLeaningTurn ,  Info.ESSENTIALS_SYSTEM_LEAN_ANGLE );
					
					if( _control.Creature.Move.UseLeaningTurn )
						Info.Warning( Info.ESSENTIALS_SYSTEM_LEAN_ANGLE_WARNING );

					EditorGUI.indentLevel++;
						EditorGUI.BeginDisabledGroup( _control.Creature.Move.UseLeaningTurn == false );
						_control.Creature.Move.LeanAngleMultiplier = ICEEditorLayout.DefaultSlider( "Lean Angle Multiplier", "Lean angle multiplier", _control.Creature.Move.LeanAngleMultiplier, 0.05f, 0, 1, 0.5f );
						_control.Creature.Move.MaxLeanAngle = ICEEditorLayout.DefaultSlider( "Max. Lean Angle", "Maximum lean angle", _control.Creature.Move.MaxLeanAngle, 0.25f, 0, 45, 35 );
						EditorGUI.EndDisabledGroup();
					EditorGUI.indentLevel--;
				EditorGUI.indentLevel--;
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.Separator();

				_control.Creature.Move.UseInternalGravity = ICEEditorLayout.Toggle("Handle Gravity", "Use internal gravity", _control.Creature.Move.UseInternalGravity,  Info.ESSENTIALS_SYSTEM_GRAVITY );
				
				EditorGUI.indentLevel++;
					EditorGUI.BeginDisabledGroup( _control.Creature.Move.UseInternalGravity == false );
						_control.Creature.Move.Gravity = ICEEditorLayout.AutoSlider( "Gravity", "Gravity value (default 9.8)", _control.Creature.Move.Gravity, 0.01f, 0, 100, ref _control.Creature.Move.UseWorldGravity, 9.81f );
					EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();

				_control.Creature.Move.UseDeadlockHandling = ICEEditorLayout.Toggle("Handle Deadlocks", "Use deadlock handling", _control.Creature.Move.UseDeadlockHandling, Info.DEADLOCK );

				EditorGUI.indentLevel++;
				EditorGUI.BeginDisabledGroup( _control.Creature.Move.UseDeadlockHandling == false );
				
					_control.Creature.Move.DeadlockMinMoveDistance = ICEEditorLayout.DefaultSlider( "Test 1 - Move Distance", "Expected distance the creature should have covered until the defined interval", _control.Creature.Move.DeadlockMinMoveDistance, 0.01f, 0, 5, 0.2f, Info.DEADLOCK_MOVE_DISTANCE );
					EditorGUI.indentLevel++;
						_control.Creature.Move.DeadlockMoveInterval = ICEEditorLayout.DefaultSlider( "Test Interval (sec.)", "Interval until the next test", _control.Creature.Move.DeadlockMoveInterval, 0.25f, 0, 30, 2, Info.DEADLOCK_MOVE_INTERVAL );
						_control.Creature.Move.DeadlockMoveMaxCriticalPositions = (int)ICEEditorLayout.DefaultSlider( "Max. Critical Positions", "Tolerates the defined number of critical positions before deadlocked will flagged as true.", _control.Creature.Move.DeadlockMoveMaxCriticalPositions, 1, 0, 100, 10, Info.DEADLOCK_MOVE_CRITICAL_POSITION );
					EditorGUI.indentLevel--;

					_control.Creature.Move.DeadlockLoopRange = ICEEditorLayout.DefaultSlider( "Test 2 - Loop Range", "Expected distance the creature should have covered until the defined interval", _control.Creature.Move.DeadlockLoopRange, 0.01f, 0, 25, _control.Creature.Move.MoveStopDistance, Info.DEADLOCK_LOOP_RANGE );
					EditorGUI.indentLevel++;
						_control.Creature.Move.DeadlockLoopInterval = ICEEditorLayout.DefaultSlider( "Test Interval (sec.)", "Interval until the next test", _control.Creature.Move.DeadlockLoopInterval, 0.25f, 0, 30, 5, Info.DEADLOCK_LOOP_INTERVAL );
						_control.Creature.Move.DeadlockLoopMaxCriticalPositions = (int)ICEEditorLayout.DefaultSlider( "Max. Critical Positions", "Tolerates the defined number of critical positions before deadlocked will flagged as true.", _control.Creature.Move.DeadlockLoopMaxCriticalPositions, 1, 0, 100, 10, Info.DEADLOCK_LOOP_CRITICAL_POSITION );
					EditorGUI.indentLevel--;


					_control.Creature.Move.DeadlockAction = (DeadlockActionType)ICEEditorLayout.EnumPopup( "Deadlock Action", "", _control.Creature.Move.DeadlockAction, Info.DEADLOCK_ACTION );

					if( _control.Creature.Move.DeadlockAction == DeadlockActionType.BEHAVIOUR )
					{
						EditorGUI.indentLevel++;
						_control.Creature.Move.DeadlockBehaviour = EditorBehaviour.BehaviourSelect( _control, "Deadlock Behaviour","", _control.Creature.Move.DeadlockBehaviour, "DEADLOCK", Info.DEADLOCK_ACTION_BEHAVIOUR );
						EditorGUI.indentLevel--;
					}
				EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;
				EditorGUILayout.Separator();

				EditorGUILayout.Separator();
				_control.Creature.Move.FieldOfView = ICEEditorLayout.DefaultSlider( "Field Of View", "Field Of View", _control.Creature.Move.FieldOfView * 2, 0.05f, 0, 360, 60, Info.FOV ) / 2;
				EditorGUI.BeginDisabledGroup( _control.Creature.Move.FieldOfView == 0 );
				EditorGUI.indentLevel++;
					_control.Creature.Move.VisualRange = ICEEditorLayout.DefaultSlider( "Visual Range", "Max. Sighting Distance", _control.Creature.Move.VisualRange, 0.25f, 0, 500, 100, Info.FOV_VISUAL_RANGE );
				EditorGUI.indentLevel--;		
				EditorGUI.EndDisabledGroup();
			
			/*
				Info.Help ( Info.ESSENTIALS_SYSTEM_GRADIENT_ANGLE );
				_creature_control.Creature.Move.MaxGradientAngle = ICEEditorLayout.DefaultSlider("Max. Gradient Angle", "Maximum gradient angle for walkable surfaces", _creature_control.Creature.Move.MaxGradientAngle, 1, 0, 90, 45 );

*/

				EditorGUILayout.Separator();
				EditorGUILayout.LabelField( "Default Move" );
				EditorGUI.indentLevel++;
					EditorSharedTools.DrawMove( ref _control.Creature.Move.DefaultMove.MoveSegmentLength, ref _control.Creature.Move.DefaultMove.MoveStopDistance,  ref _control.Creature.Move.DefaultMove.MoveSegmentVariance,  ref _control.Creature.Move.DefaultMove.MoveLateralVariance, ref _control.Creature.Move.DefaultMove.MoveIgnoreLevelDifference, Info.MOVE_DEFAULT );
				EditorGUI.indentLevel--;
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Additional Components", true, Info.EXTERNAL_COMPONENTS );
			EditorGUI.indentLevel++;

				if( _control.GetComponent<NavMeshAgent>() == null )
					_control.Creature.Move.UseNavMesh = false;				
					_control.Creature.Move.UseNavMesh = ICEEditorLayout.Toggle("Use NavMeshAgent","", _control.Creature.Move.UseNavMesh, Info.EXTERNAL_COMPONENTS_NAVMESHAGENT );
				if( _control.Creature.Move.UseNavMesh && _control.GetComponent<NavMeshAgent>() == null )
					_control.gameObject.AddComponent<NavMeshAgent>();
						
				ICEEditorLayout.BeginHorizontal();
					if( _control.GetComponent<Rigidbody>() == null )
						_control.Creature.Move.UseRigidbody = false;				
						_control.Creature.Move.UseRigidbody = ICEEditorLayout.Toggle("Use Rigidbody","", _control.Creature.Move.UseRigidbody );		
					if( _control.Creature.Move.UseRigidbody && _control.GetComponent<Rigidbody>() == null )
						_control.gameObject.AddComponent<Rigidbody>();

					GUILayout.FlexibleSpace();

					if( _control.GetComponent<Rigidbody>() != null )
					{
						if (GUILayout.Button("FULL", ICEEditorStyle.ButtonMiddle ))
						{
							Rigidbody _rigidbody = _control.GetComponent<Rigidbody>();
							
							_rigidbody.useGravity = true;
							_rigidbody.isKinematic = false;					
							_rigidbody.angularDrag = 0;
							_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
						}

						if (GUILayout.Button("SEMI", ICEEditorStyle.ButtonMiddle ))
						{
							Rigidbody _rigidbody = _control.GetComponent<Rigidbody>();
							
							_rigidbody.useGravity = false;
							_rigidbody.isKinematic = true;					
							_rigidbody.angularDrag = 0.05f;
							_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
						}

						if (GUILayout.Button("OFF", ICEEditorStyle.ButtonMiddle ))
						{
							Rigidbody _rigidbody = _control.GetComponent<Rigidbody>();

							_rigidbody.useGravity = false;
							_rigidbody.isKinematic = true;					
							_rigidbody.angularDrag = 0;
							_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
						}
					}

				ICEEditorLayout.EndHorizontal( Info.EXTERNAL_COMPONENTS_RIGIDBODY );

				EditorGUI.BeginDisabledGroup( false == false);
					if( _control.GetComponent<CharacterController>() == null )
						_control.Creature.Move.UseCharacterController = false;
					_control.Creature.Move.UseCharacterController = EditorGUILayout.Toggle("Use Character Controller", _control.Creature.Move.UseCharacterController );
					if( _control.Creature.Move.UseCharacterController && _control.GetComponent<CharacterController>() == null )
						_control.gameObject.AddComponent<CharacterController>();
				EditorGUI.EndDisabledGroup();

			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField( "Runtime Behaviour", ICEEditorStyle.LabelBold );
			EditorGUI.indentLevel++;
				_control.Creature.UseCoroutine = ICEEditorLayout.Toggle("Use Coroutine","", _control.Creature.UseCoroutine, Info.RUNTIME_COROUTINE );

				_control.Creature.DontDestroyOnLoad = ICEEditorLayout.Toggle("Dont Destroy On Load","", _control.Creature.DontDestroyOnLoad, Info.RUNTIME_DONTDESTROYONLOAD );
				
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();
		}

		/// <summary>
		/// Handles the essential settings.
		/// </summary>
		/// <param name="_creature_control">_creature_control.</param>
		private static void HandleEssentialSettings( ICECreatureControl _control )
		{

			_control.Creature.Essentials.Target = EditorSharedTools.DrawTarget( _control, _control.Creature.Essentials.Target, "Home", Info.ESSENTIALS_HOME );

			EditorGUILayout.Separator();
			ICEEditorLayout.Label( "Behaviours", true, Info.ESSENTIALS_BEHAVIOURS );
			EditorGUI.indentLevel++;
	
				_control.Creature.Essentials.BehaviourModeTravel = EditorBehaviour.BehaviourSelect( _control, "Travel", "Move behaviour if your creature is on a journey", _control.Creature.Essentials.BehaviourModeTravel, "TRAVEL" );
				_control.Creature.Essentials.BehaviourModeRendezvous = EditorBehaviour.BehaviourSelect( _control, "Rendezvous", "Idle behaviour after reaching the current target move position.", _control.Creature.Essentials.BehaviourModeRendezvous, "RENDEZVOUS" );
		
				EditorGUI.BeginDisabledGroup( _control.Creature.Essentials.Target.TargetRandomRange == 0 );
					_control.Creature.Essentials.BehaviourModeLeisure = EditorBehaviour.BehaviourSelect( _control, "Leisure", "Randomized leisure activities around the home area", _control.Creature.Essentials.BehaviourModeLeisure, "LEISURE" );
				EditorGUI.EndDisabledGroup();	

				_control.Creature.Essentials.BehaviourModeDead = EditorBehaviour.BehaviourSelect( _control, "Dead", "Static behaviour if your creature is dead", _control.Creature.Essentials.BehaviourModeDead, "DEAD" );
				_control.Creature.Essentials.BehaviourModeRespawn = EditorBehaviour.BehaviourSelect( _control, "Respawn", "Idle behaviour after respawn", _control.Creature.Essentials.BehaviourModeRespawn, "RESPAWN" );
			EditorGUI.indentLevel--;
			EditorGUILayout.Separator();

		}
		
		/// <summary>
		/// Draws the ground layers.
		/// </summary>
		/// <param name="_layers">_layers.</param>
		private static void DrawGroundLayers( List<string> _layers )
		{
			for( int i = 0 ; i < _layers.Count; i++ )
			{
				ICEEditorLayout.BeginHorizontal();
				_layers[i] = LayerMask.LayerToName( EditorGUILayout.LayerField( "Ground Layer", LayerMask.NameToLayer(_layers[i]) ));
				if (GUILayout.Button("X", ICEEditorStyle.CMDButton ))
				{
					_layers.RemoveAt(i);
					--i;
				}
				ICEEditorLayout.EndHorizontal();
			}
			
		}
	}
}
