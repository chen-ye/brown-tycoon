using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures.EnumTypes;
using System.Xml;
using System.Xml.Serialization;

namespace ICE
{
	namespace Creatures
	{
		namespace Objects
		{
			[System.Serializable]
			public class MissionsObject : System.Object
			{
				//private GameObject m_Owner = null;

				public void Init( GameObject _owner )
				{
					//m_Owner = _owner;
				/*
					m_Outpost.Init( m_Owner );
					m_Escort.Init( m_Owner );
					m_Patrol.Init( m_Owner );*/
				}

				//--------------------------------------------------
				// HOME
				//--------------------------------------------------
				[SerializeField]
				private OutpostObject m_Outpost = new OutpostObject();
				public OutpostObject Outpost{
					get{return m_Outpost; }
				}
				
				//--------------------------------------------------
				// ESCORT
				//--------------------------------------------------
				[SerializeField]
				private EscortObject m_Escort = new EscortObject();
				public EscortObject Escort{
					set{ m_Escort = value; }
					get{ return m_Escort; }
				}
				
				//--------------------------------------------------
				// PATROL
				//--------------------------------------------------
				[SerializeField]
				private PatrolObject m_Patrol = new PatrolObject();
				public PatrolObject Patrol{
					get{ return m_Patrol; }
				}

				//--------------------------------------------------
				// SCOUT
				//--------------------------------------------------
				[SerializeField]
				private ScoutObject m_Scout = new ScoutObject();
				public ScoutObject Scout
				{
					get{ return m_Scout; }
				}

				//--------------------------------------------------
				// FORMATION
				//--------------------------------------------------
				[SerializeField]
				private FormationObject m_Formation = new FormationObject();
				public FormationObject Formation
				{
					get{ return m_Formation; }
				}

				//--------------------------------------------------
				// HORDE
				//--------------------------------------------------
				[SerializeField]
				private HordeObject m_Horde = new HordeObject();
				public HordeObject Horde
				{
					get{ return m_Horde; }
				}

				public TargetObject FindNextTargetInRange( Transform transform, float range )
				{
					Ray ray = new Ray(transform.position, transform.forward);

					RaycastHit[] hits = Physics.SphereCastAll( ray, range );

					GameObject _object = null;
					float _distance = Mathf.Infinity;

					for ( int i = 0; i < hits.Length; i++ ) 
					{
						RaycastHit hit = hits[i];

						if( hit.transform.gameObject.GetComponent<MeshFilter>() )
						{
							GameObject _obj = hit.transform.gameObject;

							if ( _obj != null ) 
							{

								float _dist = Tools.GetHorizontalDistance( transform.position, _obj.transform.position );

								if( _dist < _distance )
								{
									_object = _obj;
									_distance = _dist;

								}
							}
						}
					}

					if( _object != null )
					{
						Outpost.Target.TargetGameObject = _object;
						Outpost.Target.TargetStopDistance = 2;
						Outpost.Target.TargetRandomRange = _distance;
						Outpost.Enabled = true;
						return Outpost.Target;
					}
					else
						return null;


				}
			}



			[System.Serializable]
			public abstract class MissionObject : System.Object
			{
				public MissionObject( TargetType _type )
				{
					m_Target = new TargetObject( _type );
				}

				[SerializeField]
				private TargetObject m_Target = null;
				public virtual TargetObject Target 
				{
					set{ m_Target = value; }
					get{ return m_Target; }
				}

				public bool Enabled = false;
			}


			[System.Serializable]
			public class OutpostObject : MissionObject
			{
				public OutpostObject() : base( TargetType.OUTPOST ) {}

				//--------------------------------------------------
				// Mission Home Settings
				//--------------------------------------------------
				
				public string BehaviourModeTravel = "";
				public string BehaviourModeLeisure = "";
				public string BehaviourModeRendezvous = "";

				public bool TargetReady()
				{
					if( Target.IsValid && Enabled == true )
						return true;
					else
						return false;
				}
				/*
				private CreatureObject m_Creature = null;
				protected CreatureObject GetCreature()
				{
					if( m_Creature == null )
						m_Creature = m_Owner.GetComponent<ICECreatureControl>().Creature;
					
					return m_Creature;
				}*/

				public TargetObject PrepareTarget( GameObject _owner, CreatureObject _creature )
				{
					if( TargetReady() == false || _owner == null || _creature == null || _creature.Behaviour == null )
						return null;

					// if the active target is not a OUTPOST or if the creature outside the max range it have to travel to reach its target
					//if( _creature.ActiveTarget == null || _creature.ActiveTarget.Type != TargetType.OUTPOST || ! Target.TargetInMaxRange( _owner.transform.position ))
					if( ! Target.TargetInMaxRange( _owner.transform.position ) )
						Target.BehaviourModeKey = BehaviourModeTravel;

					// if the creature reached the TargetMovePosition it should do the rendezvous behaviour
					else if( Target.TargetMoveComplete )
						Target.BehaviourModeKey = BehaviourModeRendezvous;

					// in all other case the creature should be standby and do some leisure activities
					else if( Target.TargetRandomRange > 0 )
						Target.BehaviourModeKey = BehaviourModeLeisure;
		
					return Target;
				}
			}

			[System.Serializable]
			[XmlRoot("EscortObject")]
			public class EscortObject : MissionObject
			{
				public EscortObject() : base( TargetType.ESCORT ) {}
	

				//--------------------------------------------------
				// Mission Escort Settings
				//--------------------------------------------------

				public float DelayEscort = 0.0f;
				public float DelayFollow = 0.0f;
				public float DelayStandby = 1.0f;
				public float DelayIdle  = 2.0f;

				public float DurationStandby = 10.0f;

				public string BehaviourModeEscort = "";
				public string BehaviourModeFollow = "";
				public string BehaviourModeStandby = "";
				public string BehaviourModeIdle = "";

				public bool TargetReady()
				{
					if( Target.IsValid && Enabled == true )
						return true;
					else
						return false;
				}

				public TargetObject PrepareTarget( GameObject _owner, CreatureObject _creature )
				{
					if( TargetReady() == false || _owner == null || _creature == null || _creature.Behaviour == null )
						return null;

					// if the creature has reached the target move position and the target doesn't move the creature should be standby or idle
					if ( Target.TargetMoveComplete && Target.TargetVelocity == 0 )
					{
						if( Target.BehaviourModeKey == BehaviourModeStandby && _creature.Behaviour.BehaviourTimer > DurationStandby )
							Target.BehaviourModeKey = BehaviourModeIdle;
							
						else if( Target.BehaviourModeKey != BehaviourModeIdle )
							Target.BehaviourModeKey = BehaviourModeStandby;
					}

					else if( ! Target.TargetInMaxRange( _owner.transform.position ) )
						Target.BehaviourModeKey = BehaviourModeFollow;

					else
						Target.BehaviourModeKey = BehaviourModeEscort;
					/*
					else if ( Target.TargetMoveComplete && Target.TargetVelocity > 0 )
						Target.BehaviourModeKey = BehaviourModeEscort;

					else if( Target.TargetVelocity > 0 || _creature.Behaviour.BehaviourModeKey != BehaviourModeIdle )
						Target.BehaviourModeKey = BehaviourModeFollow;
*/
					return Target;
				}
			}


			[System.Serializable]
			public class PatrolObject : MissionObject
			{
				public PatrolObject() : base( TargetType.WAYPOINT ) {}

				public bool TargetReady()
				{
					if( hasWaypoints == true && Enabled == true )
						return true;
					else
						return false;
				}

				public override TargetObject Target
				{
					get{ return Waypoints.Waypoint as TargetObject; }
				}

				//--------------------------------------------------
				// Mission Patrol Settings
				//--------------------------------------------------
				[SerializeField]
				private WaypointList m_Waypoints = new WaypointList();
				public WaypointList Waypoints
				{
					get{ return m_Waypoints; }
				}

				public bool hasWaypoints{
					get{
						if( Waypoints.Waypoints.Count > 0 )
							return true;
						else
							return false;
					}
				}


				public bool IsTransitPoint = true;
				public float DurationOfStay = 0;
				public float GetDesiredDurationOfStay()
				{
					if( Waypoints.Waypoint != null && Waypoints.Waypoint.UseCustomBehaviour )
						return Waypoints.Waypoint.DurationOfStay;
					else
						return DurationOfStay;
				}

				public bool GetIsTransitPoint()
				{
					if( Waypoints.Waypoint != null && Waypoints.Waypoint.UseCustomBehaviour )
						return Waypoints.Waypoint.IsTransitPoint;
					else
						return IsTransitPoint;
				}

				public string BehaviourModeTravel = "";
				public string BehaviourModePatrol = "";
				public string BehaviourModeLeisure = "";
				public string BehaviourModeRendezvous = "";

				public string GetBehaviourModeTravel()
				{
					if( Waypoints.Waypoint != null && Waypoints.Waypoint.UseCustomBehaviour )
						return Waypoints.Waypoint.BehaviourModeTravel;
					else
						return BehaviourModeTravel;
				}

				public string GetBehaviourModeTravelByIndex( int _index )
				{
					if( _index > 0 && _index < Waypoints.Waypoints.Count && Waypoints.Waypoints[_index] != null && Waypoints.Waypoints[_index].UseCustomBehaviour )
						return Waypoints.Waypoints[_index].BehaviourModeTravel;
					else
						return BehaviourModeTravel;
				}

				public string GetBehaviourModePatrol()
				{
					if( Waypoints.Waypoint != null && Waypoints.Waypoint.UseCustomBehaviour )
						return Waypoints.Waypoint.BehaviourModePatrol;
					else
						return BehaviourModePatrol;
				}
				
				public string GetBehaviourModePatrolByIndex( int _index )
				{
					if( _index > 0 && _index < Waypoints.Waypoints.Count && Waypoints.Waypoints[_index] != null && Waypoints.Waypoints[_index].UseCustomBehaviour )
						return Waypoints.Waypoints[_index].BehaviourModePatrol;
					else
						return BehaviourModePatrol;
				}

				public string GetBehaviourModeLeisure()
				{
					if( Waypoints.Waypoint != null && Waypoints.Waypoint.UseCustomBehaviour )
						return Waypoints.Waypoint.BehaviourModeLeisure;
					else
						return BehaviourModeLeisure;
				}

				public string GetBehaviourModeLeisureByIndex( int _index )
				{
					if( _index > 0 && _index < Waypoints.Waypoints.Count && Waypoints.Waypoints[_index] != null && Waypoints.Waypoints[_index].UseCustomBehaviour )
					   return Waypoints.Waypoints[_index].BehaviourModeLeisure;
					else
						return BehaviourModeLeisure;
				}

				public string GetBehaviourModeRendezvous()
				{
					if( Waypoints.Waypoint != null && Waypoints.Waypoint.UseCustomBehaviour )
						return Waypoints.Waypoint.BehaviourModeRendezvous;
					else
						return BehaviourModeLeisure;
				}
				
				public string GetBehaviourModeRendezvousByIndex( int _index )
				{
					if( _index > 0 && _index < Waypoints.Waypoints.Count && Waypoints.Waypoints[_index] != null && Waypoints.Waypoints[_index].UseCustomBehaviour )
						return Waypoints.Waypoints[_index].BehaviourModeRendezvous;
					else
						return BehaviourModeRendezvous;
				}

				private float m_DurationOfStayTimer = 0;
				private float m_DurationOfStayUpdateTime = 0;


				/// <summary>
				/// Prepares a waypoint target.
				/// </summary>
				/// <returns>a waypoint target as potential target candidate</returns>
				/// <description>The UpdateTarget methods only prepares and/or preselect their targets to provide a potential 
				/// target candidates for the final selection.
				/// </description>
				public TargetObject PrepareTarget( GameObject _owner, CreatureObject _creature )
				{
					if( TargetReady() == false || _owner == null || _creature == null || _creature.Behaviour == null )
						return null;

					// as long as the creature is inside the max range, the duration of stay will measured, otherwise the timer will adjusted to zero.
					if( Target.TargetInMaxRange( _owner.transform.position ) )
					{
						if( m_DurationOfStayUpdateTime > 0 )
							m_DurationOfStayTimer += Time.time - m_DurationOfStayUpdateTime;

						m_DurationOfStayUpdateTime = Time.time;
					}
					else
					{
						m_DurationOfStayUpdateTime = 0;
						m_DurationOfStayTimer = 0;
					}

					// if the active target is not a WAYPOINT we have to find the nearest waypoint and set the travel bahaviour 
					if( _creature.ActiveTarget == null || _creature.ActiveTarget.Type != TargetType.WAYPOINT )
					{
						// btw. GetWaypointByPosition() changes the target, so GetBehaviourModeTravel() returns the behaviour of the new waypoint,
						// which means, that the travel behavour always specifies the approach and not the departure.
						Waypoints.GetWaypointByPosition( _owner.transform.position );
						Target.BehaviourModeKey = GetBehaviourModeTravel();
					}
				
					// our creature have reached the max range of the given target waypoint - the max range is the random range plus target stop distance, 
					// or if the random range is zero just the target stop distance. 
					else if( Target.TargetMoveComplete )
					{
						if( GetIsTransitPoint() || GetDesiredDurationOfStay() == 0 || m_DurationOfStayTimer >= GetDesiredDurationOfStay() )
						{
							// btw. GetNextWaypoint() changes the target, so GetBehaviourModePatrol() returns the behaviour of the new waypoint,
							// which means, that a patrol behavour always specifies the approach and not the departure.
							Waypoints.GetNextWaypoint();
							Target.BehaviourModeKey = GetBehaviourModePatrol();
						}
						else
						{
							Target.BehaviourModeKey = GetBehaviourModeRendezvous();
						}
					}
					else if( ! GetIsTransitPoint() && Target.TargetInMaxRange( _owner.transform.position ) )
					{
						Target.BehaviourModeKey = GetBehaviourModeLeisure();
					}
					else
					{
						// before our creature can start with the patrol behaviour, it has to reach the first waypoint, so we check the 
						// previous target and if this is null or not a waypoint our creature is still travelling to the nearest waypoint
						// which we have found in the first rule.
						if( _creature.PreviousTarget != null && _creature.PreviousTarget.Type != TargetType.WAYPOINT )
							Target.BehaviourModeKey = GetBehaviourModeTravel();
						
						// now it looks that our creature is on the way between two waypoints and so it should use the patrol behaviour  
						else
							Target.BehaviourModeKey = GetBehaviourModePatrol();
					}

/*
					if( _creature.Move.CurrentTarget == Target && _creature.Move.HasDetour && _creature.Move.DetourPositionReached( _owner.transform.position ) )
					{
					}*/
	
					return Target;
				}
			}

			[System.Serializable]
			public class ScoutObject : MissionObject
			{
				public ScoutObject() : base( TargetType.UNDEFINED ) {}
			}

			[System.Serializable]
			public class FormationObject : MissionObject
			{
				public FormationObject() : base( TargetType.UNDEFINED ) {}
		
			}

			[System.Serializable]
			public class HordeObject : MissionObject
			{
				public HordeObject() : base( TargetType.UNDEFINED ) {}
			}




		}
	}
}
