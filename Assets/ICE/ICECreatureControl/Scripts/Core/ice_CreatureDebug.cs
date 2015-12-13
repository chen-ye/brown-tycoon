using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Shared.Objects;

//using UnityEditor;
//using UnityEditor.AnimatedValues;


namespace ICE.Creatures.Objects
{

	/// <summary>
	/// Gizmo object.
	/// </summary>
	[System.Serializable]
	public class GizmoObject : System.Object
	{
		private GameObject m_Owner = null;
		private ICECreatureControl m_creature_control = null;
		//private ICECreatureRegister m_creature_register = null;
		
		public ICECreatureControl CreatureControl{
			get{
				if( m_creature_control == null )
					m_creature_control = m_Owner.GetComponent<ICECreatureControl>();

				return m_creature_control;
			}
		}
		
		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;
			m_creature_control = m_Owner.GetComponent<ICECreatureControl>();


		}

		public bool ShowPath = true;
		private List<Vector3> m_PathPositions = new List<Vector3>();
		public int PathPositionsLimit = 1000;
		public float PathPrecision = 0.5f;

		public bool Enabled = true;
		public bool ShowText = true;
		
		public float Level = 2;

		public Color TargetColor = new Vector4(0, 0.5f, 1, 0.5f);
		public Color ActiveTargetColor = new Vector4(0, 0, 1, 1);
		public Color MoveColor = new Vector4(0, 0.5f, 0.5f, 0.5f);
		public Color MovePreviousPathColor = new Vector4(0, 0.5f, 0.5f, 0.5f);
		public Color MoveCurrentPathColor = new Vector4(0, 0.9f, 0.9f, 1f);
		public Color MoveProjectedPathColor = new Vector4(0, 0.9f, 0.9f, 1f);
		public Color MoveEscapeColor = new Vector4(0,0.75f, 0.75f, 0.5f);
		public Color MoveAvoidColor = new Vector4(0,0.75f, 0.75f, 0.5f);
		public Color MoveDetourColor = new Vector4(0,0.75f, 0.75f, 0.5f);
		public Color MoveOrbitColor = new Vector4(0,0.75f, 0.75f, 0.5f);
		public Color InteractionColor = new Vector4(0.75f, 0.5f, 0.65f, 1);
		//public bool ShowSolidInteractionRange = true;
		//public float SolidInteractionAlpha = 0.025f;

		public bool ShowHome = true;
		public bool ShowOutpost = true;
		public bool ShowEscort = true;
		public bool ShowPatrol = true;
		public bool ShowInteractor = true;
		
		public LabelType Label = LabelType.Blue;

		/// <summary>
		/// Draws the home.
		/// </summary>
		public void DrawHome()
		{
			if( ! CreatureControl.Creature.Essentials.TargetReady() || ShowHome == false )
				return;
		
			TargetObject _target = CreatureControl.Creature.Essentials.Target;
			DrawTargetGizmos( _target );	

			if( ! Application.isPlaying )
			{
				BehaviourModeObject _mode = null;
				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Essentials.BehaviourModeTravel );			
				DrawBehaviourModeGizmos( _target, _mode );
				
				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Essentials.BehaviourModeLeisure );			
				DrawBehaviourModeGizmos( _target, _mode );
				
				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Essentials.BehaviourModeRendezvous );			
				DrawBehaviourModeGizmos( _target, _mode );
			}
		}

		/// <summary>
		/// Draws the outpost.
		/// </summary>
		public void DrawOutpost()
		{
			if( ! CreatureControl.Creature.Missions.Outpost.TargetReady() || ShowOutpost == false  )
				return;

			TargetObject _target = CreatureControl.Creature.Missions.Outpost.Target;
			DrawTargetGizmos( _target );	

			if( ! Application.isPlaying )
			{
				BehaviourModeObject _mode = null;
				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Outpost.BehaviourModeTravel );			
				DrawBehaviourModeGizmos( _target, _mode );
				
				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Outpost.BehaviourModeLeisure );			
				DrawBehaviourModeGizmos( _target, _mode );

				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Outpost.BehaviourModeRendezvous );			
				DrawBehaviourModeGizmos( _target, _mode );
			}
		}


		/// <summary>
		/// Draws the escort.
		/// </summary>
		public void DrawEscort()
		{
			if( ! CreatureControl.Creature.Missions.Escort.TargetReady() || ShowEscort == false  )
				return;

			TargetObject _target = CreatureControl.Creature.Missions.Escort.Target;
			DrawTargetGizmos( _target );	

			if( ! Application.isPlaying )
			{

				BehaviourModeObject _mode = null;
				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Escort.BehaviourModeFollow );			
				DrawBehaviourModeGizmos( _target, _mode );

				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Escort.BehaviourModeEscort );			
				DrawBehaviourModeGizmos( _target, _mode );

				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Escort.BehaviourModeIdle );			
				DrawBehaviourModeGizmos( _target, _mode );

				_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Escort.BehaviourModeStandby );			
				DrawBehaviourModeGizmos( _target, _mode );
			}

		}

		/// <summary>
		/// Draws the patrol.
		/// </summary>
		public void DrawPatrol()
		{
			if( ! CreatureControl.Creature.Missions.Patrol.TargetReady() || ShowPatrol == false  )
				return;
			
			TargetObject _target = null;
			Vector3 _target_move_position = Vector3.zero;
			float _target_stop_distance = 0;
			
			WaypointObject _last_waypoint_target =	CreatureControl.Creature.Missions.Patrol.Waypoints.GetLastValidWaypoint();
			
			Vector3 _last_target_move_position = Vector3.zero;
			float _last_target_stop_distance = 0;
			
			if( _last_waypoint_target != null )
			{
				_last_target_move_position = _last_waypoint_target.TargetMovePosition;
				_last_target_move_position.y = GetLevel();
				_last_target_stop_distance = _last_waypoint_target.TargetStopDistance;
			}

			for(  int i = 0 ; i < CreatureControl.Creature.Missions.Patrol.Waypoints.Waypoints.Count ; i++ )
			{
				_target = (TargetObject)CreatureControl.Creature.Missions.Patrol.Waypoints.Waypoints[i];

				if( _target.IsValid == false )
					continue;

				_target_move_position = _target.TargetMovePosition;
				_target_move_position.y = GetLevel();
				_target_stop_distance = _target.TargetStopDistance;
				
				if( CreatureControl.Creature.Missions.Patrol.Waypoints.Waypoints[i].Enabled )
				{
					DrawTargetGizmos( _target );

					Color _default_color = Gizmos.color;
					Gizmos.color = MoveProjectedPathColor;
					CustomGizmos.OffsetPath( _last_target_move_position, _last_target_stop_distance , _target_move_position, _target_stop_distance );
					Gizmos.color = _default_color;

					_last_target_move_position = _target_move_position;
					_last_target_stop_distance = _target_stop_distance;
				}
				else
				{
					Color _color = TargetColor;
					_color.a = 0.25f;
					DrawTargetGizmos( _target, _color );
				}

				if( ! Application.isPlaying )
				{
					BehaviourModeObject _mode = null;
					_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Patrol.GetBehaviourModeTravelByIndex(i) );			
					DrawBehaviourModeGizmos( _target, _mode );

					_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Patrol.GetBehaviourModePatrolByIndex(i));			
					DrawBehaviourModeGizmos( _target, _mode );

					_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Patrol.GetBehaviourModeLeisureByIndex(i) );			
					DrawBehaviourModeGizmos( _target, _mode );

					_mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( CreatureControl.Creature.Missions.Patrol.GetBehaviourModeRendezvousByIndex(i) );			
					DrawBehaviourModeGizmos( _target, _mode );
				}
			}
		}

		/// <summary>
		/// Draws the interaction.
		/// </summary>
		public void DrawInteraction()
		{
			if( CreatureControl.Creature.Interaction.Interactors.Count == 0 || ShowInteractor == false  )
				return;

			for(  int i = 0 ; i < CreatureControl.Creature.Interaction.Interactors.Count ; i++ )
			{
				InteractorObject _interactor = CreatureControl.Creature.Interaction.Interactors[i];
								
				if( ! _interactor.Enabled )
					continue;

				List<GameObject> _target_game_objects = GetCreaturesByName( _interactor.Name );

				if( _target_game_objects != null && _target_game_objects.Count > 0 )
				{
					foreach( GameObject _target_game_object in _target_game_objects )
					{
						Vector3 _interactor_pos = _target_game_object.transform.position;	

						TargetObject _target = new TargetObject( TargetType.INTERACTOR );

						_target.TargetGameObject = _target_game_object;
						_target.BehaviourModeKey = _interactor.BehaviourModeKey;	
						_target.Selectors.Copy( _interactor.Selectors ); 
						_target.UpdateOffset( _interactor.DefaultOffset );
						_target.TargetStopDistance = _interactor.DefaultStopDistance;
						_target.TargetRandomRange = _interactor.DefaultRandomRange;
						_target.TargetSmoothingMultiplier = 0;//_interactor.DefaultSmoothingMultiplier; // TODO:REWORK TargetSmoothingMultiplier
						_target.TargetIgnoreLevelDifference = _interactor.DefaultIgnoreLevelDifference;

						DrawTargetGizmos( _target );
			
						// DIRECTION
						CustomGizmos.Arrow( _target.TargetPosition, _target.TargetDirection * _target.TargetStopDistance , 0.5f, 20 );
					
						Vector3 _last_move_position = _target.TargetMovePosition;
						float _last_stop_distance = _target.TargetStopDistance;

						_last_move_position.y = GetLevel();

						Color _default_color = Gizmos.color;
						Color _target_color = ( _target.Active?ActiveTargetColor:TargetColor); 

						foreach( InteractorRuleObject _rule in _interactor.Rules )
						{
							if( ! _rule.Enabled )
								continue;

							_target = new TargetObject( TargetType.INTERACTOR );
							
							_target.TargetGameObject = _target_game_object;
							_target.BehaviourModeKey = _rule.BehaviourModeKey;	
							_target.Selectors.Copy( _rule.Selectors ); 

							_target.UpdateOffset( _interactor.DefaultOffset );
							_target.TargetStopDistance = _interactor.DefaultStopDistance;
							_target.TargetRandomRange = _interactor.DefaultRandomRange;
							_target.TargetSmoothingMultiplier = 0;//_interactor.DefaultSmoothingMultiplier; // TODO:REWORK TargetSmoothingMultiplier
							_target.TargetIgnoreLevelDifference = _interactor.DefaultIgnoreLevelDifference;
			
							if( _rule.OverrideTargetMovePosition )
							{
								_target.UpdateOffset( _rule.Offset );
								_target.TargetStopDistance = _rule.StopDistance;
								_target.TargetRandomRange = _rule.RandomRange;
								_target.TargetSmoothingMultiplier = 0;
								_target.TargetIgnoreLevelDifference = _rule.IgnoreLevelDifference;
							}

							DrawTargetGizmos( _target );

							Vector3 _move_position = _target.TargetMovePosition;
							_move_position.y = GetLevel();

							Gizmos.color = _target_color;
							if( _last_move_position != Vector3.zero )
								CustomGizmos.OffsetPath( _last_move_position, _last_stop_distance, _move_position, _target.TargetStopDistance, true );
			
							_last_stop_distance = _target.TargetStopDistance;
							_last_move_position = _move_position;

						}
						
						Gizmos.color = _target_color;
						if( _last_move_position != Vector3.zero )
							CustomGizmos.OffsetPath( _last_move_position, _last_stop_distance, _interactor_pos, 0, false );
				
						Gizmos.color = _default_color;
						//CustomGizmos.OffsetPath( _default_data.OffsetPosition, _default_data.StopDistance, _interactor_pos, 0 );
					}
				}		
			}
		}



		/// <summary>
		/// Gets the level.
		/// </summary>
		/// <returns>The level.</returns>
		private float GetLevel()
		{
			return CreatureControl.transform.position.y + Level;
		}
		

		/// <summary>
		/// Gets creature list by name
		/// </summary>
		/// <returns>The creatures by name.</returns>
		/// <param name="_name">_name.</param>
		/// <summary></summary>
		private List<GameObject> GetCreaturesByName( string _name )
		{
			if( ICECreatureRegister.Register )
				return ICECreatureRegister.Register.GetCreaturesByName( _name );

			return null;
		}

		private void DrawDirectionAngle( Transform _center, float _radius, float _angle, float _max = 0, float _min = 0 )
		{
			Vector3 _center_pos = _center.position;
			float _level = GetLevel();
			
			Vector3 _left_pos = Tools.GetDirectionPosition( _center, - _angle, _radius );
			Vector3 _right_pos = Tools.GetDirectionPosition( _center, _angle, _radius );		
			
			_center_pos.y = _level;
			_left_pos.y = _level;
			_right_pos.y = _level;
			
			Gizmos.DrawLine( _center_pos, _left_pos );
			Gizmos.DrawLine( _center_pos, _right_pos );

			CustomGizmos.Arc( _center, _radius, 1, - _angle, _angle, _level, false ); 

			if( _min > 0 && _min < _radius )
				CustomGizmos.Arc( _center, _min, 1, - _angle, _angle, _level, true );
			if( _max > _radius )
				CustomGizmos.Arc( _center, _max, 1, - _angle, _angle, _level, true ); 
		}

		private void DrawDirectionAngle( Transform _center, float _inner_radius, float _outer_radius, float _outer_radius_max, float _outer_radius_min, float _inner_angle, float _outer_angle )
		{
			Vector3 _center_pos = _center.position;
			float _level = GetLevel();

			Vector3 _left_inner_pos = Tools.GetDirectionPosition( _center, - _inner_angle, _inner_radius );
			Vector3 _right_inner_pos = Tools.GetDirectionPosition( _center, _inner_angle, _inner_radius );				
			Vector3 _left_outer_pos = Tools.GetDirectionPosition( _center, - _outer_angle, _outer_radius );
			Vector3 _right_outer_pos = Tools.GetDirectionPosition( _center, _outer_angle, _outer_radius );
			
			_center_pos.y = _level;
			_left_inner_pos.y = _level;
			_right_inner_pos.y = _level;
			_left_outer_pos.y = _level;
			_right_outer_pos.y = _level;
			
			Gizmos.DrawLine( _center_pos, _left_inner_pos );
			Gizmos.DrawLine( _center_pos, _right_inner_pos );
			
			Gizmos.DrawLine( _left_inner_pos, _left_outer_pos );
			Gizmos.DrawLine( _right_inner_pos, _right_outer_pos );
			
			CustomGizmos.Arc( _center, _inner_radius, 1, - _inner_angle, _inner_angle, _level, false ); 
			CustomGizmos.Arc( _center, _outer_radius, 1, - _outer_angle, _outer_angle, _level, false ); 
			
			CustomGizmos.Arc( _center, _outer_radius_min, 1, - _outer_angle, _outer_angle, _level, true ); 
			CustomGizmos.Arc( _center, _outer_radius_max, 1, - _outer_angle, _outer_angle, _level, true ); 
		}

		private void DrawBehaviourModeRuleGizmos( TargetObject _target, BehaviourModeRuleObject _rule )
		{
			#if UNITY_EDITOR
			if( _target == null || _target.IsValid == false || _rule == null )
				return;

			Vector3 _owner_position = m_Owner.transform.position;
			Vector3 _target_pos = _target.TargetPosition;
			Vector3 _target_move_position = _target.TargetMovePosition;
			float _target_stop_distance = _target.TargetStopDistance;
			float _target_selection_range = _target.Selectors.SelectionRange;
			//float _target_selection_angle = _target.Selectors.SelectionAngle;

			DrawOwnerGizmos();

			if( Application.isPlaying )
			{
				DrawPathGizmos();
				DrawMoveGizmos();
			}

			float _level = GetLevel();

			_owner_position.y = _level;
			_target_pos.y = _level;
			_target_move_position.y = _level;

			if( _rule.Move.Type == MoveType.DEFAULT )
			{
			}
			else if( _rule.Move.Type == MoveType.AVOID )
			{
				MoveContainer _move = _rule.Move;
				//float _radius = _target_selection_range  + 0.25f;// _move.Avoid.MaxAvoidDistance;//( _target_selection_range / 2 ) + 0.25f;
				//float _angle = _move.Avoid.MaxDirectionAngle;

				Gizmos.color = MoveAvoidColor;
				UnityEditor.Handles.color = Gizmos.color;

				//DrawDirectionAngle( m_Owner.transform, _radius, _angle );
				//DrawDirectionAngle( _target.TargetGameObject.transform, _radius, _angle );

				Vector3 _right = Vector3.Cross( _target.TargetGameObject.transform.up, _target.TargetGameObject.transform.forward );
				Vector3 _avoid_pos_right = _target_pos + ( _right * _move.Avoid.AvoidDistance );
				Vector3 _avoid_pos_left = _target_pos + ( _right * -_move.Avoid.AvoidDistance );

				CustomGizmos.OffsetPath( _target_pos, 0, _avoid_pos_right, _move.MoveStopDistance );
				CustomGizmos.DottedCircle( _avoid_pos_right,_move.MoveStopDistance, 5, 2 );
				CustomGizmos.OffsetPath( _target_pos, 0, _avoid_pos_left, _move.MoveStopDistance );
				CustomGizmos.DottedCircle( _avoid_pos_left,_move.MoveStopDistance, 5, 2 );

				if( Application.isPlaying )
				{
					/*
					Gizmos.color = Color.blue;
					Gizmos.DrawLine( _target_pos, CreatureControl.Creature.Move.AvoidMovePosition );
					Gizmos.color = Color.red;
					Gizmos.DrawLine( _target_pos, CreatureControl.Creature.Move.MovePosition );
					*/

				}
			}
			else if( _rule.Move.Type == MoveType.ESCAPE )
			{
				MoveContainer _move = _rule.Move;
				float _inner_radius = ( _target_selection_range / 2 ) + 0.25f;
				float _outer_radius = _inner_radius + _rule.Move.MoveSegmentLength;
				float _outer_radius_max = _inner_radius + _rule.Move.MoveSegmentLengthMax;
				float _outer_radius_min = _inner_radius + _rule.Move.MoveSegmentLengthMin;

				float _inner_angle = 0;
				float _outer_angle = _inner_angle + _move.Escape.RandomEscapeAngle;

				Gizmos.color = MoveEscapeColor;

				Vector3 _left_inner_pos = Tools.GetDirectionPosition( _target.TargetGameObject.transform, - _inner_angle, _inner_radius );
				Vector3 _right_inner_pos = Tools.GetDirectionPosition( _target.TargetGameObject.transform, _inner_angle, _inner_radius );				
				Vector3 _left_outer_pos = Tools.GetDirectionPosition( _target.TargetGameObject.transform, - _outer_angle, _outer_radius );
				Vector3 _right_outer_pos = Tools.GetDirectionPosition( _target.TargetGameObject.transform, _outer_angle, _outer_radius );

				_target_pos.y = _level;
				_left_inner_pos.y = _level;
				_right_inner_pos.y = _level;
				_left_outer_pos.y = _level;
				_right_outer_pos.y = _level;

				Gizmos.DrawLine( _target_pos, _left_inner_pos );
				Gizmos.DrawLine( _target_pos, _right_inner_pos );

				Gizmos.DrawLine( _left_inner_pos, _left_outer_pos );
				Gizmos.DrawLine( _right_inner_pos, _right_outer_pos );

				CustomGizmos.Arc( _target.TargetGameObject.transform, _inner_radius, 1, - _inner_angle, _inner_angle, _level, false ); 
				CustomGizmos.Arc( _target.TargetGameObject.transform, _outer_radius, 1, - _outer_angle, _outer_angle, _level, false ); 

				CustomGizmos.Arc( _target.TargetGameObject.transform, _outer_radius_min, 1, - _outer_angle, _outer_angle, _level, true ); 
				CustomGizmos.Arc( _target.TargetGameObject.transform, _outer_radius_max, 1, - _outer_angle, _outer_angle, _level, true ); 

				// DENGER ZONE BEGIN
				_inner_radius+= 0.25f;
				float _degree = CustomGizmos.GetBestDegrees( _inner_radius, _inner_angle );
				CustomGizmos.ArrowArc( _target.TargetGameObject.transform, _inner_radius,_degree ,- _inner_angle,_inner_angle, _level ); 

				Transform _target_transform = _target.TargetGameObject.transform;
				Vector3 _center = _target_transform.position;
				Vector3 _center_pos = Tools.GetDirectionPosition( _target_transform, 0, _inner_radius + ( _inner_radius / 10 ) );
				Vector3 _left_pos = Tools.GetDirectionPosition( _target_transform, - _inner_angle, _inner_radius );
				Vector3 _right_pos = Tools.GetDirectionPosition( _target_transform, _inner_angle , _inner_radius );
				
				_center.y = _level;
				_center_pos.y = _level;
				_left_pos.y = _level;
				_right_pos.y = _level;
				
				Gizmos.DrawLine( _center, _center_pos );
				Gizmos.DrawLine( _center, _left_pos );
				Gizmos.DrawLine( _center, _right_pos );

			}
			else if( _rule.Move.Type == MoveType.ORBIT )
			{
				MoveContainer _move = _rule.Move;
				float _degrees = CreatureControl.Creature.Move.OrbitDegrees;
				float _radius = _move.Orbit.Radius;

				if( Application.isPlaying )
				{
					_move = CreatureControl.Creature.Move.CurrentMove;
					_radius = CreatureControl.Creature.Move.OrbitRadius;
				}

				Gizmos.color = MoveOrbitColor;
				CustomGizmos.Orbit( m_Owner.transform.position, _target_move_position, _radius, _degrees, _move.Orbit.RadiusShift, _move.Orbit.MinDistance, _move.Orbit.MaxDistance, _level );
			}
			else if( _rule.Move.Type == MoveType.DETOUR )
			{
				Vector3 _detour_pos = _rule.Move.Detour.Position;

				_detour_pos.y = _level;
	
				Gizmos.color = MoveDetourColor;
				CustomGizmos.OffsetPath( _detour_pos, _rule.Move.MoveStopDistance , _target_move_position, _target_stop_distance ); 
				CustomGizmos.Circle( _detour_pos, _rule.Move.MoveStopDistance, 5, false ); // STOP DISTANCE RANGE
				//DrawMoveGizmos( Vector3 _initial_position, Vector3 _move_position, float _stop_distance, float _random_range, Vector3 _target_position, float _target_stop_distance )
			}

			#endif
		}
		
		private Vector3 DrawBehaviourModeGizmos( TargetObject _target, BehaviourModeObject _mode )
		{
			if( _target == null || _target.IsValid == false || _mode == null )
				return Vector3.zero;
			
			foreach( BehaviourModeRuleObject _rule in _mode.Rules )
				DrawBehaviourModeRuleGizmos( _target, _rule );
			
			return Vector3.zero;
		}

		private Vector3 m_PreviousOwnerPosition = Vector3.zero;
		public void DrawPathGizmos()
		{
			// PATH BEGIN
			if( ShowPath == false )
				return;

			Color _default = Gizmos.color;
			Gizmos.color = MovePreviousPathColor;

			Vector3 _owner_pos = m_Owner.transform.position;
			_owner_pos.y = GetLevel();

			if( PathPositionsLimit > 0 && m_PathPositions.Count >= PathPositionsLimit )
				m_PathPositions.RemoveAt(0);
			
			if( PathPrecision == 0 || Vector3.Distance( m_PreviousOwnerPosition, _owner_pos ) >= PathPrecision )
			{
				m_PreviousOwnerPosition = _owner_pos;
				m_PathPositions.Add( _owner_pos );
			}
			
			Vector3 _prior_pos = Vector3.zero;
			foreach( Vector3 _pos in m_PathPositions)
			{
				if( _prior_pos != Vector3.zero  )
					Gizmos.DrawLine( _prior_pos, _pos );
	
				_prior_pos = _pos;
			}

			if( m_creature_control.Creature.Move.NavMeshAgentReady )
			{
				Gizmos.color = MoveCurrentPathColor;
				NavMeshPath _path = m_creature_control.Creature.Move.NavMeshAgentComponent.path;
				_prior_pos = Vector3.zero;
				foreach( Vector3 _pos in _path.corners )
				{
					if( _prior_pos != Vector3.zero  )
						Gizmos.DrawLine( _prior_pos, _pos );
					
					_prior_pos = _pos;
				}
			}

			Gizmos.color = _default;
		}

		public void DrawOwnerGizmos()
		{
			#if UNITY_EDITOR
			Vector3 _owner_pos = m_Owner.transform.position;
			_owner_pos.y = GetLevel();

			CustomGizmos.Triangle( _owner_pos, m_Owner.transform.forward, 2, 35 );

			if( m_creature_control.Creature.Move.FieldOfView > 0 )
			{
				float _angle = m_creature_control.Creature.Move.FieldOfView;
				float _distance = m_creature_control.Creature.Move.VisualRange;
				
				if( _distance == 0 )
					_distance = 1.5f;

				float _degree = CustomGizmos.GetBestDegrees( _distance, _angle );
				CustomGizmos.ArrowArc( m_Owner.transform, _distance, _degree, - _angle, _angle, GetLevel() );

				if( _angle < 180 )
				{
					Vector3 _left_pos = Tools.GetDirectionPosition( m_Owner.transform, - _angle, _distance );
					Vector3 _right_pos = Tools.GetDirectionPosition( m_Owner.transform, _angle, _distance );

					CustomGizmos.OffsetPath( m_Owner.transform.position, 2, _left_pos, 1) ;
					CustomGizmos.OffsetPath( m_Owner.transform.position, 2, _right_pos, 1) ;
				}
			}

			if( m_creature_control.Creature.Move.GroundOrientation == GroundOrientationType.NONE || m_creature_control.Creature.Move.GroundOrientation == GroundOrientationType.QUADRUPED )
			{
				if( m_creature_control.Creature.Move.GroundOrientationPlus )
				{
					UnityEditor.Handles.CubeCap( 0, m_creature_control.Creature.Move.FrontLeftPositionGround, m_Owner.transform.rotation, 0.1f);
					UnityEditor.Handles.CubeCap( 0, m_creature_control.Creature.Move.BackLeftPositionGround, m_Owner.transform.rotation, 0.1f);
					UnityEditor.Handles.CubeCap( 0, m_creature_control.Creature.Move.FrontRightPositionGround, m_Owner.transform.rotation, 0.1f);
					UnityEditor.Handles.CubeCap( 0, m_creature_control.Creature.Move.BackRightPositionGround, m_Owner.transform.rotation, 0.1f);
					
					Gizmos.DrawLine( m_creature_control.Creature.Move.FrontLeftPositionGround, m_creature_control.Creature.Move.FrontLeftPosition );
					Gizmos.DrawLine( m_creature_control.Creature.Move.BackLeftPositionGround, m_creature_control.Creature.Move.BackLeftPosition );
					Gizmos.DrawLine( m_creature_control.Creature.Move.FrontRightPositionGround, m_creature_control.Creature.Move.FrontRightPosition );
					Gizmos.DrawLine( m_creature_control.Creature.Move.BackRightPositionGround, m_creature_control.Creature.Move.BackRightPosition );
					
					UnityEditor.Handles.CubeCap( 0, m_creature_control.Creature.Move.FrontLeftPosition, m_Owner.transform.rotation, 0.1f);
					UnityEditor.Handles.CubeCap( 0, m_creature_control.Creature.Move.BackLeftPosition, m_Owner.transform.rotation, 0.1f);
					UnityEditor.Handles.CubeCap( 0, m_creature_control.Creature.Move.FrontRightPosition, m_Owner.transform.rotation, 0.1f);
					UnityEditor.Handles.CubeCap( 0, m_creature_control.Creature.Move.BackRightPosition, m_Owner.transform.rotation, 0.1f);
				}
			}
			#endif
		}

		public void DrawMoveGizmos()
		{
			#if UNITY_EDITOR
			if( m_Owner == null || m_creature_control.Creature.Move.CurrentTarget == null )
				return;

			TargetObject _target = m_creature_control.Creature.Move.CurrentTarget;

			Vector3 _move_position = m_creature_control.Creature.Move.MovePosition;
			Vector3 _target_move_position = _target.TargetMovePosition;
			float _move_stop_distance = m_creature_control.Creature.Move.CurrentMove.MoveStopDistance;
			float _move_lateral_variance = m_creature_control.Creature.Move.CurrentMove.MoveLateralVariance;

			Vector3 _owner_pos = m_Owner.transform.position;
			_owner_pos.y = GetLevel();
			_move_position.y = GetLevel();
			_target_move_position.y = GetLevel();



			CustomGizmos.OffsetPath( _owner_pos, 2, _move_position, _move_stop_distance ); // PATH FROM CREATURE TO NEXT MOVE POSITION
			CustomGizmos.Circle( _move_position, _move_stop_distance, 5, false ); // STOP DISTANCE RANGE

			if( m_creature_control.Creature.Move.CurrentMove.Type != MoveType.ESCAPE )
				CustomGizmos.OffsetPath( _move_position, _move_stop_distance , _target_move_position , _target.TargetStopDistance );  // PATH NEXT MOVE POSITION TO TARGET MOVE POSITION

			if( _move_lateral_variance > 0 )
			{
				float _max_range = _move_stop_distance + _move_lateral_variance;

				CustomGizmos.ZickZackCircle( _move_position, _move_lateral_variance, "", false ); // RANDOM RANGE
				CustomGizmos.Circle( _move_position, _max_range, 5, true ); // MAX MOVE RANGE
			}

			#endif
		}

		private void DrawTargetGizmos( TargetObject _target, Color _target_color )
		{ 
			Color _selection_color = ( _target.Active?ActiveTargetColor:InteractionColor);
			
			DrawTargetGizmos( _target, _target_color, _selection_color );
		}

		private void DrawTargetGizmos( TargetObject _target )
		{ 
			Color _target_color = ( _target.Active?ActiveTargetColor:TargetColor);
			Color _selection_color = ( _target.Active?ActiveTargetColor:InteractionColor);

			DrawTargetGizmos( _target, _target_color, _selection_color );
		}

		/// <summary>
		/// Draws the target gizmos.
		/// </summary>
		/// <param name="_target">_target.</param>
		private void DrawTargetGizmos( TargetObject _target, Color _target_color, Color _selection_color )
		{ 
			#if UNITY_EDITOR
			if( _target == null || _target.IsValid == false )
				return;

			Vector3 _target_pos = _target.TargetPosition;
			Vector3 _target_direction = _target.TargetDirection;
			Vector3 _target_pos_offset = _target.TargetOffsetPosition;
			Vector3 _target_pos_move = _target.TargetMovePosition;
			float _target_selection_distance = _target.Selectors.SelectionRange;
			float _target_selection_angle = _target.Selectors.SelectionAngle;

			Vector3 _target_pos_top = _target_pos;
			Vector3 _target_pos_bottom = _target_pos;

			float _level = GetLevel();
			_target_pos.y = GetLevel();
			_target_pos_offset.y = GetLevel();
			_target_pos_move.y = GetLevel();
			_target_pos_top.y = GetLevel() + 3;
			_target_pos_bottom.y = 0;

			Color _previous_color = Gizmos.color;
			Gizmos.color = _target_color;
			UnityEditor.Handles.color = _target_color;
		
			// SELECTION RANGE BEGIN
			/*if( ShowSolidInteractionRange && SolidInteractionAlpha > 0 )
			{
				UnityEditor.Handles.color = _selection_color;
				UnityEditor.Handles.DrawWireDisc( _target_pos , Vector3.up, _target_selection_distance );
				
				Color _selection_color_transparent = _selection_color;
				_selection_color_transparent.a = SolidInteractionAlpha;
				UnityEditor.Handles.color = _selection_color_transparent;
				UnityEditor.Handles.DrawSolidDisc( _target_pos , Vector3.up, _target_selection_distance );
			}
			else
			{
				UnityEditor.Handles.color = _selection_color;
				UnityEditor.Handles.DrawWireDisc( _target_pos , Vector3.up, _target_selection_distance );
			}*/


			//if( _target_selection_angle > 0 && _target_selection_angle <= 180 )
			{
				if( _target_selection_angle == 0 )
					_target_selection_angle = 180;


				CustomGizmos.ArrowArc( _target.TargetGameObject.transform, _target_selection_distance, CustomGizmos.GetBestDegrees( _target_selection_distance, _target_selection_angle ), - _target_selection_angle, _target_selection_angle, _level, true );
			
			
				if( _target_selection_angle < 180 )
				{
					Transform _target_transform = _target.TargetGameObject.transform;
					Vector3 _center = _target_transform.position;
					Vector3 _center_pos = Tools.GetDirectionPosition( _target_transform, 0, _target_selection_distance + ( _target_selection_distance / 10 ) );
					Vector3 _left_pos = Tools.GetDirectionPosition( _target_transform, - _target_selection_angle, _target_selection_distance - 0.25f );
					Vector3 _right_pos = Tools.GetDirectionPosition( _target_transform, _target_selection_angle , _target_selection_distance - 0.25f );
					
					_center.y = _level;
					_center_pos.y = _level;
					_left_pos.y = _level;
					_right_pos.y = _level;
					
					//Gizmos.DrawLine( _center, _center_pos );
					Gizmos.DrawLine( _center, _left_pos );
					Gizmos.DrawLine( _center, _right_pos ); /**/
				}
			}
			// SELECTION RANGE END




			// TARGET ICON BEGIN
			Gizmos.DrawLine( _target_pos_bottom, _target_pos_top );
			Gizmos.DrawIcon( _target_pos_top, "ice_waypoint.png");
			// TARGET ICON END


			// TARGET STOP DISTANCE BEGIN
			if( _target.TargetIgnoreLevelDifference )
				CustomGizmos.Circle( _target_pos_move, _target.TargetStopDistance, 5, false ); // IGNORES LEVEL DIFFERNCE SO WE DRAW JUST A CIRCLE
			else
				Gizmos.DrawWireSphere( _target_pos_move, _target.TargetStopDistance );// IF THE LEVEL IS IMPORTANT FOR THE STOP DISTANCE WE DRAW A SPHERE
			// TARGET STOP DISTANCE END

			// MAX AND RANDOM RANGE BEGIN
			CustomGizmos.Circle( _target_pos_offset, _target.TargetRandomRange , 5, false ); // RANDOM RANGE
			CustomGizmos.Circle( _target_pos_offset, _target.TargetRandomRange + _target.TargetStopDistance , 5, true ); // MAX RANGE
			// MAX AND RANDOM RANGE END

			// SPECIAL CASES FOR INTERACTOR WHICH COULD HAVE SEVERAL SELECTION RANGES
			if( _target.Type != TargetType.INTERACTOR )
			{
				if( Vector3.Distance( _target_pos_move, _target_pos_offset ) <= _target.TargetStopDistance )
					CustomGizmos.OffsetPath( _target_pos, 0.0f, _target_pos_offset, _target.TargetStopDistance );
				else
					Gizmos.DrawLine( _target_pos, _target_pos_offset );

				CustomGizmos.OffsetPath( _target_pos_offset, 0.0f, _target_pos_move, _target.TargetStopDistance );

				// DIRECTION
				CustomGizmos.Arrow( _target_pos, _target_direction * _target.TargetStopDistance , 0.5f, 20 );
			}

			BehaviourModeObject _mode = CreatureControl.Creature.Behaviour.GetBehaviourModeByKey( _target.BehaviourModeKey );			
			DrawBehaviourModeGizmos( _target, _mode );

			// PRIVIOUS COLOR
			Gizmos.color = _previous_color;
			UnityEditor.Handles.color = _previous_color;

			#endif
		}
	}

	/// <summary>
	/// Pointer object.
	/// </summary>
	[System.Serializable]
	public class PathObject : System.Object
	{

		public void NavMeshPath( NavMeshAgent _agent )
		{/*
		GameObject _object = new GameObject();
		_object.transform.parent = _agent.gameObject.transform;
		LineRenderer _line = _object.AddComponent<LineRenderer>();
		_line.SetColors(Color.white,Color.white);
		_line.SetWidth(0.1f,0.1f);
		//Get def material
		
		_line.gameObject.renderer.material.color = Color.white;
		_line.gameObject.renderer.material.shader = Shader.Find("Sprites/Default");
		_line.gameObject.AddComponent<LineScript>();
		_line.SetVertexCount(_agent.path.corners.Length+1);
		int i = 0;
		foreach(Vector3 v in p.corners)
		{
			_line.SetPosition(i,v);
			//Debug.Log("position agent"+g.transform.position);
			//Debug.Log("position corner = "+v);
			i++;
		}
		_line.SetPosition(p.corners.Length,_agent.destination);
		_line.gameObject.tag = "ligne";*/
		}
	}
	/// <summary>
	/// Pointer object.
	/// </summary>
	[System.Serializable]
	public class PointerObject : System.Object
	{
		public PointerObject()
		{
			PointerColor = Color.red;
		}
		public PointerObject( Color _color )
		{
			PointerColor = _color;
		}
		
		[SerializeField]
		private bool m_Enabled = false;
		public bool Enabled{
			set{
				if( ! value )
					DestroyPointer();
				
				m_Enabled = value;
			}
			get{ return m_Enabled; }
		}
		
		public Color PointerColor;
		
		[SerializeField]
		private PrimitiveType m_PointerType = PrimitiveType.Sphere;
		public PrimitiveType PointerType{
			set{
				if( m_PointerType != value )
				{
					m_PointerType = value;
					DestroyPointer();
				}
			}
			get{ return m_PointerType; }
		}
		public Vector3 PointerSize = new Vector3( 0.5f ,0.5f , 0.5f );
		
		private GameObject m_Pointer = null;
		public GameObject Pointer{
			get{
				
				if( Enabled )
				{
					if( m_Pointer == null )
						m_Pointer = GameObject.CreatePrimitive( PointerType );
					
					m_Pointer.GetComponent<Collider>().enabled = false;
					m_Pointer.SetActive( Enabled );
					//m_Pointer.transform.position = m_TargetMovePosition;
					m_Pointer.transform.localScale = PointerSize;
					m_Pointer.GetComponent<Renderer>().material.color = PointerColor;
				}
				
				return m_Pointer;
			}
		}
		
		private void DestroyPointer()
		{
			if( m_Pointer != null )
			{
				UnityEngine.Object.Destroy ( m_Pointer );
				m_Pointer = null;
			}
		}

	}


	[System.Serializable]
	public class DebugObject : System.Object
	{
		private GameObject m_Owner = null;
		private ICECreatureControl m_creature_control = null;

		public ICECreatureControl CreatureControl{
			get{
				if( m_creature_control == null )
					m_creature_control = m_Owner.GetComponent<ICECreatureControl>();
				
				return m_creature_control;
			}
		}

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			m_creature_control = m_Owner.GetComponent<ICECreatureControl>();
			Gizmos.Init( m_Owner );
		}

		public bool DebugLogEnabled = false;

		[SerializeField]
		private PointerObject m_MovePointer = new PointerObject( Color.blue );
		public PointerObject MovePointer{
			get{ return m_MovePointer; }
		}
		
		[SerializeField]
		private PointerObject m_TargetPositionPointer = new PointerObject( Color.green );
		public PointerObject TargetPositionPointer{
			get{ return m_TargetPositionPointer; }
		}

		[SerializeField]
		private GizmoObject m_Gizmos = new GizmoObject();
		public GizmoObject Gizmos{
			get{ return m_Gizmos; }
		}

		public void DebugLog()
		{
			if( DebugLogEnabled == false )
				return;

			if( CreatureControl.Creature.TargetChanged )
				Debug.Log( "TARGET INFO : '" + m_Owner.name.ToUpper() + "' CHANGED TARGET '" + CreatureControl.Creature.PreviousTargetName + "' TO '" + CreatureControl.Creature.ActiveTargetName + "'!");

			if( CreatureControl.Creature.Behaviour.BehaviourModeChanged )
				Debug.Log( "BEHAVIOUR INFO : '" + m_Owner.name.ToUpper() + "' CHANGED BEHAVIOURMODE '" + CreatureControl.Creature.Behaviour.LastBehaviourModeKey + "' TO '" + CreatureControl.Creature.Behaviour.BehaviourModeKey + "'!");
			
			if( CreatureControl.Creature.Behaviour.BehaviourModeRulesChanged )
				Debug.Log( "BEHAVIOUR INFO : '" + m_Owner.name.ToUpper() + "' PREPARES " + CreatureControl.Creature.Behaviour.BehaviourMode.Rules.Count +  " RULES FOR '" + CreatureControl.Creature.Behaviour.BehaviourModeKey + "'!");

			if( CreatureControl.Creature.Behaviour.BehaviourModeRuleChanged )
				Debug.Log( "BEHAVIOUR INFO : '" + m_Owner.name.ToUpper() + "' SELECT 'RULE " + (int)(CreatureControl.Creature.Behaviour.BehaviourMode.RuleIndex + 1 )+ "' OF '" + CreatureControl.Creature.Behaviour.BehaviourModeKey + "'!");

			//Debug.Log( "MOVE INFO : TargetMovePosition = " + CreatureControl.Creature.Move.CurrentTarget.TargetMovePosition.ToString() + "' MovePosition = " + CreatureControl.Creature.Move.MovePosition.ToString());

		}
	}
}
