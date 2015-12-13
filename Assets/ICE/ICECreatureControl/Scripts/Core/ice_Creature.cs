using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;


namespace ICE.Creatures.Objects
{

	//--------------------------------------------------
	// ActionObject is the data container for the curent action
	//--------------------------------------------------
	[System.Serializable]
	public class CreatureObject : System.Object
	{
		/*
		public CreatureObject( GameObject gameObject )
		{
			m_Owner = gameObject;
			
			Essentials.Init ( m_Owner );
			
			Genitor.Init ( m_Owner );
			
			Status.Init( m_Owner );
			Behaviour.Init( m_Owner );
			Move.Init( m_Owner );
			Environment.Init( m_Owner );
			Interaction.Init( m_Owner );
			
			CreatureRegister.Register( m_Owner );
		}*/

		private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			Move.Init( m_Owner );
			Essentials.Init ( m_Owner );
			Status.Init( m_Owner );
			Behaviour.Init( m_Owner );

			Missions.Init ( m_Owner );
			Interaction.Init( m_Owner );
			Environment.Init( m_Owner );

			CreatureRegister.Register( m_Owner );
		}

		public void Bye()
		{
			CreatureRegister.Deregister( m_Owner );
		}
/*
		~CreatureObject()
		{
			CreatureRegister.Deregister( m_Owner );
		}*/

		public bool DontDestroyOnLoad = false;
		public bool UseCoroutine = true;

		//--------------------------------------------------
		// Spawner
		//--------------------------------------------------
		[SerializeField]
		private InteractionObject m_Interaction = new InteractionObject();
		public InteractionObject Interaction
		{
			set{ m_Interaction = value; }
			get{ return m_Interaction; }
		}

		//--------------------------------------------------
		// Essence
		//--------------------------------------------------
		
		[SerializeField]
		private EssentialsObject m_Essentials = new EssentialsObject();
		public EssentialsObject Essentials
		{
			set{ m_Essentials = value; }
			get{ return m_Essentials; }
		}

		//--------------------------------------------------
		// Status
		//--------------------------------------------------
		
		[SerializeField]
		private StatusObject m_Status = new StatusObject();
		public StatusObject Status
		{
			set{ m_Status = value; }
			get{ return m_Status; }
		}

		//--------------------------------------------------
		// Behaviour
		//--------------------------------------------------

		[SerializeField]
		private BehaviourObject  m_Behaviour = new BehaviourObject ();
		public BehaviourObject  Behaviour
		{
			set{ m_Behaviour = value; }
			get{ return m_Behaviour; }
		}

		//--------------------------------------------------
		// MISSIONS
		//--------------------------------------------------
		[SerializeField]
		private MissionsObject m_Missions = new MissionsObject();
		public MissionsObject Missions{
			get{ return m_Missions; }
		}


		//--------------------------------------------------
		// Move
		//--------------------------------------------------
		[SerializeField]
		private MoveObject m_Move = new MoveObject();
		public MoveObject Move{
			get{ return m_Move; }
		}

		//--------------------------------------------------
		// ENVIRONMENT
		//--------------------------------------------------
		[SerializeField]
		private EnvironmentObject m_Environment = new EnvironmentObject();
		public EnvironmentObject Environment{
			get{ return m_Environment; }
		}


		private TargetObject m_ActiveTarget = null;				
		public TargetObject ActiveTarget{
			get{ return m_ActiveTarget; }
		}

		private TargetObject m_PreviousTarget = null;				
		public TargetObject PreviousTarget
		{
			get{ return m_PreviousTarget; }
		}

		public bool m_TargetChanged = false;
		public bool TargetChanged{
			get{ return m_TargetChanged; }
		}

		public string ActiveTargetName
		{
			get{
				if( m_ActiveTarget != null && m_ActiveTarget.TargetGameObject != null )
					return m_ActiveTarget.TargetGameObject.name;
				else if( m_ActiveTarget != null  )
					return "INVALID " + m_ActiveTarget.Type.ToString() + " TARGET";
				else
					return "NO TARGET";
			}
		}

		public float ActiveTargetVelocity
		{
			get{
				if( m_ActiveTarget != null && m_ActiveTarget.TargetGameObject != null )
					return m_ActiveTarget.TargetVelocity;
				else
					return 0;
			}
		}

		public string PreviousTargetName
		{
			get{
				if( m_PreviousTarget != null && m_PreviousTarget.TargetGameObject != null )
					return m_PreviousTarget.TargetGameObject.name;
				else if( m_PreviousTarget != null  )
					return "INVALID " + m_PreviousTarget.Type.ToString() + " TARGET";
				else
					return "NONE";
			}
		}
		public bool IsSenseTime( Transform transform )
		{
			return Status.IsSenseTime();
		}


		public bool IsReactionTime( Transform transform )
		{
			if( Status.IsReactionTime() ) 
				return true;
			else if ( ActiveTarget == null )
				return true;
			else if( ActiveTarget.TargetMoveComplete )
				return true;
			else
				return false;			
		}

		public TargetObject FindTargetByName( string _name )
		{
			if( _name == "" )
				return null;

			TargetObject _target = null;

			if( _target == null && Essentials.TargetReady() && Essentials.Target.TargetGameObject.name == _name )
				_target = Essentials.Target;
			if( _target == null && Missions.Outpost.Enabled && Missions.Outpost.TargetReady() && Missions.Outpost.Target.TargetGameObject.name == _name )
				_target = Missions.Outpost.Target;
			if( _target == null && Missions.Escort.Enabled && Missions.Escort.TargetReady() && Missions.Escort.Target.TargetGameObject.name == _name )
				_target = Missions.Escort.Target;
			if( _target == null && Missions.Patrol.Enabled )
				_target = Missions.Patrol.Waypoints.GetWaypointByName( _name );
			/*if( _target == null )
				_target = Interaction.GetInteractorByName( _name );*/

			return _target;
		}

		private List<TargetObject> m_AvailableTargets = new List<TargetObject>();
		public List<TargetObject> AvailableTargets{
			get{return m_AvailableTargets; }
		}

		public void SelectBestTarget()
		{
			TargetObject _favourite_target = null;

			foreach( TargetObject _target in m_AvailableTargets )
			{
				if( _target == null || _target.IsValid == false )
					continue;

				_target.Selectors.ResetRelevanceMultiplier();
				/*
				if( m_ActiveTarget != null && m_ActiveTarget != _target )
					m_ActiveTarget.Selectors.CompareSuccessorTargets( _target, m_Owner.transform.position );
*/
				bool _final_result = false;

				if( _target.TargetInSelectionRange( m_Owner.transform, Move.FieldOfView, Move.VisualRange ) )
					_final_result = true;

				if( _target.Selectors.UseAdvanced )
				{
					foreach( TargetSelectorObject _selector in _target.Selectors.Selectors )
					{
						bool _selector_result = false;

						foreach( TargetSelectorConditionObject _condition in _selector.Conditions )
						{
							if( _condition.Enabled == false )
								continue;

							bool _condition_result = false;

							if( _condition.ExpressionType == TargetSelectorExpressionType.DISTANCE )
							{
								_condition_result = false;//_target.TargetInSelectionRange( m_Owner.transform.position, _condition.Distance );

								//TODO - HANDLE DISTANCE
							}
							else if( _condition.ExpressionType == TargetSelectorExpressionType.BEHAVIOR )
							{
								if( Behaviour.BehaviourModeKey == _condition.BehaviourModeKey )
									_condition_result = true;

								if( _condition.Operator == LogicalOperatorType.NOT )
									_condition_result = ! _condition_result;

							}
							else if( _condition.ExpressionType == TargetSelectorExpressionType.POSITION )
							{
								switch( _condition.PositionType )
								{
									case TargetSelectorPositionType.TARGETMOVEPOSITION:
									_condition_result =_target.TargetMoveComplete;
										break;
									case TargetSelectorPositionType.TARGETMAXRANGE:
									_condition_result =_target.TargetInMaxRange( m_Owner.transform.position );
										break;
								}

								if( _condition.Operator == LogicalOperatorType.NOT )
									_condition_result = ! _condition_result;

							}
							else if( _condition.ExpressionType == TargetSelectorExpressionType.PRECURSOR )
							{
								TargetObject _precursor = null;
								if( _target == m_ActiveTarget )
									_precursor = m_PreviousTarget;
								else
									_precursor = m_ActiveTarget;

								_condition_result = _condition.ComparePrecursorTarget( _precursor );

							}

							if( _condition.ConditionType == ConditionalOperatorType.AND || _condition_result == true )
								_selector_result = _condition_result;			
						}

						if( _selector_result )
						{
							foreach( TargetSelectorStatementObject _statement in _selector.Statements )
							{
								if( _statement.StatementType == TargetSelectorStatementType.PRIORITY )
								{
									_target.Selectors.Priority = _statement.Priority;
								}
								else if( _statement.StatementType == TargetSelectorStatementType.SUCCESSOR )
								{
									if( _statement.SuccessorType == TargetSuccessorType.NAME )
									{
										TargetObject _successor = FindTargetByName( _statement.SuccessorTargetName );
										
										if( _successor != null && _successor.IsValid )
										{
											_favourite_target = _successor;
										}
									}
								}
							}
						}

						if( _selector.ConditionType == ConditionalOperatorType.AND || _selector_result == true )
							_final_result = _selector_result;
					
					}
				}

				if( _final_result )
				{
					bool _handled = false;

					if( _target.Selectors.UseAdvanced && _target.Selectors.Statements.Count > 0 )
					{
						foreach( TargetSelectorStatementObject _statement in _target.Selectors.Statements )
						{
							if( _statement.StatementType == TargetSelectorStatementType.SUCCESSOR )
							{
								if( _statement.SuccessorType == TargetSuccessorType.NAME )
								{
									TargetObject _successor = FindTargetByName( _statement.SuccessorTargetName );

									if( _successor != null && _successor.IsValid )
									{
										_favourite_target = _successor;
										_handled = true;
									}
								}
							}
						}
					}

					if( _handled == false )
					{
						if( _favourite_target == null )
							_favourite_target = _target;
						else if( _target.SelectionPriority > _favourite_target.SelectionPriority )
							_favourite_target = _target;
						else if( _target.SelectionPriority == _favourite_target.SelectionPriority && Random.Range(0,1)==1 )
							_favourite_target = _target;
					}
				}
			}

			SetActiveTarget( _favourite_target );
		}

		/// <summary>
		/// Sets the active target.
		/// </summary>
		/// <param name="_target">_target.</param>
		public void SetActiveTarget( TargetObject _target )
		{
			if( Status.IsDead )
				return;

			if( Move.Deadlocked )
			{
				Move.ResetDeadlock();
				if( Move.DeadlockAction == DeadlockActionType.BEHAVIOUR )
				{
					Behaviour.SetBehaviourModeByKey( Move.DeadlockBehaviour );
					return;
				}
				else if( Move.DeadlockAction == DeadlockActionType.UPDATE && m_ActiveTarget != null )
					m_ActiveTarget.UpdateOffset();
				else
				{
					Status.Kill();
					return;
				}
			}


			if( _target == null || Status.RecreationRequired )
				_target = Essentials.Target;

			if( _target == null || ! _target.IsValid )
			{
				Debug.LogError( m_Owner.name + " have no target!" );
				return;
			}

			if( IsTargetUpdatePermitted( _target ) )
			{
				if( m_ActiveTarget != _target )
				{
					m_PreviousTarget = m_ActiveTarget;

					if( m_PreviousTarget != null )
						m_PreviousTarget.SetActive( false );
			
					m_ActiveTarget = _target;

					if( m_ActiveTarget != null )
						m_ActiveTarget.SetActive( true );
			
					m_TargetChanged = true;
				}

				if( m_ActiveTarget != null )
					Behaviour.SetBehaviourModeByKey( ActiveTarget.BehaviourModeKey );
			}
		}

		private bool IsTargetUpdatePermitted( TargetObject _target )
		{
			if( _target == null )
				return false;
			
			if( m_ActiveTarget == null || Behaviour.BehaviourMode == null || Behaviour.BehaviourMode.Favoured.Enabled == false)
				return true;
			
			bool _permitted = true;
			
			if( ( Behaviour.BehaviourMode.Favoured.Enabled == true ) && (
				( Behaviour.BehaviourMode.Favoured.FavouredMinimumPeriod > 0 && Behaviour.BehaviourTimer < Behaviour.BehaviourMode.Favoured.FavouredMinimumPeriod ) ||
				( Behaviour.BehaviourMode.Favoured.FavouredUntilNextMovePositionReached && ! Move.MovePositionReached() ) ||
				( Behaviour.BehaviourMode.Favoured.FavouredUntilTargetMovePositionReached && ! m_ActiveTarget.TargetMoveComplete ) ||
				( Behaviour.BehaviourMode.Favoured.FavouredUntilNewTargetInRange( _target, Vector3.Distance( _target.TargetGameObject.transform.position, m_Owner.transform.position ) ) ) ||
				( Behaviour.BehaviourMode.HasActiveDetourRule && Behaviour.BehaviourMode.Favoured.FavouredUntilDetourPositionReached && ! Move.DetourComplete ) ) )
				_permitted = false;
			else
				_permitted = true;
			
			//mode check - the new mode could be also forced, so we have to check this here 
			if( _permitted == false )
			{
				BehaviourModeObject _mode = Behaviour.GetBehaviourModeByKey( _target.BehaviourModeKey );
				
				if( _mode != null && _mode.Favoured.Enabled == true )
				{
					if( Behaviour.BehaviourMode.Favoured.FavouredPriority > _mode.Favoured.FavouredPriority )
						_permitted = false;
					else if( Behaviour.BehaviourMode.Favoured.FavouredPriority < _mode.Favoured.FavouredPriority ) 
						_permitted = true;
					else 
						_permitted = (Random.Range(0,1) == 0?false:true);
				}
			}
			
			
			return _permitted;
		}

		//--------------------------------------------------

		public void UpdateMove()
		{
			if( Status.IsDead )
			{
				Move.StopMove();
				return;
			}

			Move.UpdateMove( m_ActiveTarget, Behaviour.GetCurrentBehaviourModeRule() );

			Environment.SurfaceHandler.Update( Move.MoveVelocity );
		}

		//--------------------------------------------------

		public void UpdateBegin()
		{
			m_TargetChanged = false;
			
			Status.UpdateBegin( Move.MoveVelocity );
			Behaviour.UpdateBegin();

			
			if( m_ActiveTarget != null )
				m_ActiveTarget.Update( m_Owner );	
		}

		public void UpdateComplete()
		{


		}

		//--------------------------------------------------
		public void FixedUpdate()
		{
			//TODO: obsolete
			if( Missions.Outpost.TargetReady() )
				Missions.Outpost.Target.FixedUpdate();
			if( Missions.Escort.TargetReady() )
				Missions.Escort.Target.FixedUpdate();
			if( Missions.Patrol.TargetReady() )
				Missions.Patrol.Target.FixedUpdate();
			if( Interaction.TargetReady() )
				Interaction.FixedUpdate();

			if( ActiveTarget != null && ActiveTarget.IsValid )
				ActiveTarget.FixedUpdate();

			if( Behaviour.BehaviourMode != null && Behaviour.BehaviourMode.Rule != null && Behaviour.BehaviourMode.Rule.Influences.Enabled )
			{
				Status.AddDamage( Behaviour.BehaviourMode.Rule.Influences.Damage );
				Status.AddStress( Behaviour.BehaviourMode.Rule.Influences.Stress );
				Status.AddDebility ( Behaviour.BehaviourMode.Rule.Influences.Debility );
				Status.AddHunger( Behaviour.BehaviourMode.Rule.Influences.Hunger );
				Status.AddThirst( Behaviour.BehaviourMode.Rule.Influences.Thirst );
			}

			if( Environment.SurfaceHandler.ActiveSurface != null && Environment.SurfaceHandler.ActiveSurface.Influences.Enabled == true )
			{
				Status.AddDamage( Environment.SurfaceHandler.ActiveSurface.Influences.Damage );
				Status.AddStress( Environment.SurfaceHandler.ActiveSurface.Influences.Stress );
				Status.AddDebility( Environment.SurfaceHandler.ActiveSurface.Influences.Debility );
				Status.AddHunger( Environment.SurfaceHandler.ActiveSurface.Influences.Hunger );
				Status.AddThirst( Environment.SurfaceHandler.ActiveSurface.Influences.Thirst );
			}

			Status.FixedUpdate();

			if( Status.IsDead )
			{
				Behaviour.SetBehaviourModeByKey( Essentials.BehaviourModeDead );
				Status.RespawnRequest();
			}

			if( Status.IsRespawnTime )
				Status.Respawn();

				

		}

		public void HandleCollision( Collision collision )
		{
			if( ! Environment.CollisionHandler.Enabled || Status.IsDead )
				return;

			CollisionDataObject _impact = Environment.CollisionHandler.CheckCollision( collision );

			if( _impact != null )
			{
				Status.AddDamage( _impact.Influences.Damage );
				Status.AddStress( _impact.Influences.Stress );
				Status.AddDebility( _impact.Influences.Debility );
				Status.AddHunger( _impact.Influences.Hunger );
				Status.AddThirst( _impact.Influences.Thirst );

				if( _impact.BehaviourModeKey != "" )
					Behaviour.SetBehaviourModeByKey( _impact.BehaviourModeKey );
			}

		}
	}
}
