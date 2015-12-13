using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class InteractorRuleObject
	{
		public InteractorRuleObject(){}
		public InteractorRuleObject( string _key )
		{
			Enabled = true;
			BehaviourModeKey = _key;
		}

		public bool Enabled;

		public string BehaviourModeKey;	
		public bool OverrideTargetMovePosition;
		public Vector3 Offset;
		public float StopDistance;
		public float RandomRange;
		public bool IgnoreLevelDifference;
		public bool UpdateOffsetOnActivateTarget;
		public bool UpdateOffsetOnMovePositionReached;

		public float SmoothingMultiplier;

		[SerializeField]
		private TargetSelectorsObject m_Selectors = new TargetSelectorsObject( TargetType.INTERACTOR );
		public TargetSelectorsObject Selectors{
			set{ m_Selectors = value;}
			get{ return m_Selectors;}
		}

		public bool UseDefaultSelectionPriority = true;
		public bool BlockRuleUpdateUntilMovePositionReached = false;


		public Vector3 GetOffsetPosition( Transform _transform )
		{
			if( _transform == null )
				return Vector3.zero;

			Vector3 _local_offset = Offset;
			
			_local_offset.x = _local_offset.x/_transform.lossyScale.x;
			_local_offset.y = _local_offset.y/_transform.lossyScale.y;
			_local_offset.z = _local_offset.z/_transform.lossyScale.z;
			
			return _transform.TransformPoint( _local_offset );
		}
	}

	//--------------------------------------------------
	// InteractorObject
	//--------------------------------------------------
	[System.Serializable]
	public class InteractorObject
	{
		public InteractorObject()
		{
			m_Target = new TargetObject( TargetType.INTERACTOR );
		}

		[SerializeField]
		private TargetSelectorsObject m_Selectors = new TargetSelectorsObject( TargetType.INTERACTOR );
		public TargetSelectorsObject Selectors{
			set{ m_Selectors = value;}
			get{ return m_Selectors;}
		}

		public string BehaviourModeKey = "";

		[SerializeField]
		private List<InteractorRuleObject> m_Rules = new List<InteractorRuleObject>();
		public List<InteractorRuleObject> Rules
		{
			set{ m_Rules = value; }
			get{ return m_Rules; }
		}
	
		private InteractorRuleObject m_PreviousRule = null;
		public InteractorRuleObject PreviousRule{
			get{ return m_PreviousRule; }
		}

		private InteractorRuleObject m_ActiveRule = null;
		public InteractorRuleObject ActiveRule{
			get{ return m_ActiveRule; }
		}

		public float GetStartDistance()
		{
			if( Selectors.SelectionRange == 0 )
				return Mathf.Infinity;
			else
				return Selectors.SelectionRange;
		}
		/*
		public InteractorRuleObject GetRuleByDistanceAndSituation( float _distance )
		{
		}*/

		public InteractorRuleObject GetRuleByIndexOffset( int _offset )
		{
			int _index = m_Rules.IndexOf( m_ActiveRule );
			int _new_index = _index + _offset;

			if( _new_index < 0 )
				return null;
			else if( _new_index >= m_Rules.Count )
				return null;
			else
				return m_Rules[_new_index];
		}

		/// <summary>
		/// Returns the nearest rule by distance.
		/// </summary>
		/// <returns>The rule by distance.</returns>
		/// <param name="_distance">_distance.</param>
		public InteractorRuleObject GetRuleByDistance( float _distance )
		{
			InteractorRuleObject _selected = null;

			foreach( InteractorRuleObject _rule in m_Rules )
			{
				if( _distance <= _rule.Selectors.SelectionRange && _rule.Enabled )
					_selected = _rule;
			}

			return _selected;
		}

		public InteractorRuleObject GetBestInteractorRule( Vector3 _position )
		{
			InteractorRuleObject _new_rule = null;

			// if the active rule have advanced settings 
			if( m_ActiveRule != null && m_ActiveRule.OverrideTargetMovePosition )
			{
				if( m_ActiveRule.BlockRuleUpdateUntilMovePositionReached )
				{
					// if the target move position was reached, the creture needs a new rule
					if( Target.TargetMovePositionReached( _position ) )
					{
						// if the active rule have a zero range, we select the next higher rule
						if( m_ActiveRule.Selectors.SelectionRange == 0 )
							_new_rule = GetRuleByIndexOffset( 1 );

						// if the active rule have a zero range, we select the next higher rule
						else
							_new_rule = GetRuleByDistance( Target.TargetDistanceTo( _position ) );
					}
				}
				else
					_new_rule = GetRuleByDistance( Target.TargetDistanceTo( _position ) );
			}

			// handle default - get rule by distance 
			else
			{
				_new_rule = GetRuleByDistance( Target.TargetDistanceTo( _position ) );
			}

			if( m_ActiveRule != _new_rule )
			{
				m_PreviousRule = m_ActiveRule;
				m_ActiveRule = _new_rule;
			}

			return m_ActiveRule;
		}

		[SerializeField]
		private TargetObject m_Target = null;
		public virtual TargetObject Target 
		{
			set{ m_Target = value; }
			get{ return m_Target; }
		}

		public Vector3 DefaultOffset;
		public float DefaultStopDistance;
		public float DefaultRandomRange;
		public bool UpdateOffsetOnActivateTarget = true;
		public bool UpdateOffsetOnMovePositionReached = false;
		public float DefaultSmoothingMultiplier;
		public bool DefaultForceMovePositionReached = false;
		public bool DefaultIgnoreLevelDifference = true;

		public bool Foldout = true;
		public string Name = "";
		
		public bool Enabled = false;

		public bool EnabledHome = false;
		public bool EnabledEscort = false;
		public bool EnabledPatrol = false;
		public bool EnabledScout = false;

	}

	[System.Serializable]
	public class InteractionObject
	{
		private GameObject m_Owner = null;
		private ICECreatureRegister m_CreatureRegister = null;

		[SerializeField]
		private List<InteractorObject> m_Interactors = new List<InteractorObject>();
		public List<InteractorObject> Interactors{
			set{ m_Interactors = value; }
			get{ return m_Interactors; }
		}
		/*
		public TargetObject GetInteractorByName( string _name )
		{
			if( _name == "" )
				return null;

			foreach( InteractorObject _interactor in m_Interactors )
			{
				if( _interactor.Target != null 
			}
		}*/

		private InteractorObject m_Interactor = null;
		public InteractorObject Interactor 
		{
			get{ return m_Interactor; }
		}

		public void Reset()
		{
			m_Interactors.Clear();
			m_Interactor = null;
		}
	

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			if( m_CreatureRegister == null )
				m_CreatureRegister = GameObject.FindObjectOfType<ICECreatureRegister>();
		}

		public List<InteractorObject> GetValidInteractors()
		{
			List<InteractorObject> _interactors = new List<InteractorObject> ();

			foreach( InteractorObject _interactor in Interactors)
			{
				if( _interactor.Enabled )
					_interactors.Add( _interactor );
			}

			return _interactors;
		}
	
		public bool TargetReady()
		{
			return TargetReady( null );
		}

		public bool TargetReady( GameObject _target )
		{
			if( m_Interactor != null && 
			   		m_Interactor.Target != null &&
			   			m_Interactor.Target.TargetGameObject != null && 
			  				 m_Interactor.Enabled == true ) 
			   				//m_TargetAttacker.TargetMovePositionDistanceTo( m_Owner.transform.position ) <= AttackerInteractorData.Sense.Distance )
				return true;
			else
				return false;
		}

		public int GetPriority( GameObject _creature, int _priority )
		{
			if( _priority > 0 || _creature.GetComponent<ICECreatureControl>() == null )
				return _priority;
			else
				return _creature.GetComponent<ICECreatureControl>().Creature.Status.Aggressivity;
		}

		public bool CheckExternalTarget( GameObject _target )
		{
			if( _target == null )
				return false;

			return false;
		}

		public void Sense()
		{

			InteractorObject _best_interactor = null;
			GameObject _best_creature = null;
			int _best_priority = 0;

			float _best_distance = Mathf.Infinity;

			for (int i = 0; i < Interactors.Count; ++i)
			{
				InteractorObject _interactor = Interactors[i];

				if( ! _interactor.Enabled )
					continue;

				GameObject _creature = CreatureRegister.FindNearestCreature( _interactor.Name, m_Owner.transform.position, _interactor.GetStartDistance() );

				if( _creature != null )
				{
					float _creature_distance = Tools.GetHorizontalDistance( m_Owner.transform.position, _creature.transform.position );

					if( _creature_distance <= _best_distance )
					{
						int _priority = GetPriority( _creature, _interactor.Selectors.Priority );

						if( _priority >= _best_priority )
						{
							_best_priority = _priority;
							_best_creature = _creature;
							_best_distance = _creature_distance;
							_best_interactor = _interactor;
						}					
					}
				}
			}

			
			if( _best_creature != null && _best_interactor != null )
			{
				if( m_Interactor == null || m_Interactor.Target.TargetGameObject != _best_creature )
				{
					m_Interactor = _best_interactor;

					m_Interactor.Target = new TargetObject( TargetType.INTERACTOR );
					m_Interactor.Target.TargetGameObject = _best_creature;


					PrepareTarget();
				}
			}
			else
			{
				m_Interactor = null;
			}
		}


		public TargetObject PrepareTarget()
		{
			if( ! TargetReady() )
				return null;

			InteractorRuleObject _rule = m_Interactor.GetBestInteractorRule( m_Owner.transform.position );

			if( _rule == null )
			{
				m_Interactor.Target.BehaviourModeKey = m_Interactor.BehaviourModeKey;	
				m_Interactor.Target.Selectors.Copy( m_Interactor.Selectors ); // incl. default priority
			}
			else
			{
				m_Interactor.Target.BehaviourModeKey = _rule.BehaviourModeKey;
				m_Interactor.Target.Selectors.Copy( _rule.Selectors );
				m_Interactor.Target.Selectors.Priority = ( _rule.UseDefaultSelectionPriority?m_Interactor.Selectors.Priority:_rule.Selectors.Priority );
			}

			if( _rule == null || _rule.OverrideTargetMovePosition == false )
			{
				m_Interactor.Target.UpdateOffset( m_Interactor.DefaultOffset );
				m_Interactor.Target.TargetStopDistance = m_Interactor.DefaultStopDistance;
				m_Interactor.Target.TargetRandomRange = m_Interactor.DefaultRandomRange;
				m_Interactor.Target.TargetSmoothingMultiplier = m_Interactor.DefaultSmoothingMultiplier;
				m_Interactor.Target.UseUpdateOffsetOnActivateTarget= m_Interactor.UpdateOffsetOnActivateTarget;
				m_Interactor.Target.UseUpdateOffsetOnMovePositionReached = m_Interactor.UpdateOffsetOnMovePositionReached;
			}
			else
			{
				m_Interactor.Target.UpdateOffset( _rule.Offset );
				m_Interactor.Target.TargetStopDistance = _rule.StopDistance;
				m_Interactor.Target.TargetRandomRange = _rule.RandomRange;
				m_Interactor.Target.TargetSmoothingMultiplier = _rule.SmoothingMultiplier;
				m_Interactor.Target.UseUpdateOffsetOnActivateTarget= _rule.UpdateOffsetOnActivateTarget;
				m_Interactor.Target.UseUpdateOffsetOnMovePositionReached = _rule.UpdateOffsetOnMovePositionReached;				
			}

			return m_Interactor.Target;


		}

		public void FixedUpdate()
		{
			if( TargetReady() )
				m_Interactor.Target.FixedUpdate();
		}

	}

}

