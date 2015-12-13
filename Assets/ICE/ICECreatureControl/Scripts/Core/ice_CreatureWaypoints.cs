using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures.EnumTypes;

namespace ICE
{
	namespace Creatures
	{
		namespace Objects
		{
			[System.Serializable]
			public static class WaypointsRegister : System.Object
			{
				[SerializeField]
				private static List<WaypointList> m_WaypointLists;
				public static List<WaypointList> WaypointLists
				{
					get{ return m_WaypointLists; }
				}
			}

			[System.Serializable]
			public class WaypointObject : TargetObject
			{
				public WaypointObject() : base( TargetType.WAYPOINT ) {}
				public WaypointObject( GameObject _object ) : base( TargetType.WAYPOINT ) 
				{
					Waypoint = _object;
				}

				public bool Enabled = true;

				[SerializeField]
				private string m_Name = "";

				public GameObject Waypoint{
					set{ TargetGameObject = value; }
					get{ return TargetGameObject; }
				}

				public string Name
				{
					get{
						if( m_Name == "" && Waypoint != null )
							m_Name = Waypoint.name;

						return m_Name;
					}
					set{ m_Name = value; }
				}

				public bool UseCustomBehaviour = false;

				public float DurationOfStay  = 0.0f;
				public bool IsTransitPoint = true;

				public string BehaviourModeTravel = "";
				public string BehaviourModePatrol = "";
				public string BehaviourModeLeisure = "";
				public string BehaviourModeRendezvous = "";
				
			}
			
			[System.Serializable]
			public class WaypointList : System.Object
			{
				public bool Foldout = true;

				[SerializeField]
				private GameObject m_WaypointGroup = null;
				public GameObject WaypointGroup
				{
					set{ 
						if( m_WaypointGroup != value )
						{
							m_WaypointGroup = value;

							UpdateWaypointGroup();
						}
					
					}
					get{ return m_WaypointGroup; }
				}

				public void UpdateWaypointGroup()
				{
					if( m_WaypointGroup == null || m_WaypointGroup.transform.childCount == 0 )
						return;
					
					Reset();
					
					for( int i = 0 ; i < m_WaypointGroup.transform.childCount ; i++ )
					{
						GameObject _object = m_WaypointGroup.transform.GetChild( i ).gameObject;
						
						if( _object )
						{
							Waypoints.Add( new WaypointObject( _object ) );
						}
					}
				}

				private WaypointObject m_LastWaypoint = null;
				public WaypointObject LastWaypoint{
					get{ return m_LastWaypoint; }
				}

				private WaypointObject m_Waypoint = null;
				private int m_WaypointIndex = 0;

				public string Identifier = "";
				public WaypointOrderType Order;
				public bool Ascending = true;

				[SerializeField]
				private List<WaypointObject> m_Waypoints = new List<WaypointObject>();
				public List<WaypointObject> Waypoints
				{
					get{ return m_Waypoints; }
				}

				public WaypointObject Waypoint
				{
					get{ 
						if( m_Waypoint == null )
							return GetNextWaypoint();
						else
							return m_Waypoint;
					}
				}

				public List<WaypointObject> GetValidWaypoints()
				{
					List<WaypointObject> _waypoints = new List<WaypointObject>();
					
					foreach( WaypointObject _waypoint in m_Waypoints )
					{
						if( _waypoint.TargetGameObject != null && _waypoint.Enabled ) 
							_waypoints.Add( _waypoint );
					}
					
					return _waypoints; 
				}

				public WaypointObject GetLastValidWaypoint()
				{
					WaypointObject _last_waypoint = null;

					foreach( WaypointObject _waypoint in m_Waypoints )
					{
						if( _waypoint.TargetGameObject != null && _waypoint.Enabled ) 
							_last_waypoint = _waypoint;
					}

					return _last_waypoint; 
				}

				public WaypointObject GetWaypointByName( string _name )
				{
					List<WaypointObject> _waypoints = GetValidWaypoints();

					foreach( WaypointObject _waypoint in _waypoints )
					{
						if( _waypoint.IsValid && _waypoint.TargetGameObject.name == _name )
							return _waypoint;
					}

					return null;
				}

				public WaypointObject GetWaypointByPosition( Vector3 _position )
				{
					WaypointObject new_waypoint = null;
					int new_waypoint_index = m_WaypointIndex;
					float distance = Mathf.Infinity;

					List<WaypointObject> _waypoints = GetValidWaypoints();

					for( int i = 0 ; i < _waypoints.Count ; i++ )
					{
						float tmp_distance = Vector3.Distance( _position, _waypoints[ i ].TargetOffsetPosition );
						if( tmp_distance < distance )
						{
							new_waypoint_index = i;
							new_waypoint = _waypoints[ new_waypoint_index ];
							distance = tmp_distance;
						}
					}

					if( new_waypoint != null )
					{
						m_LastWaypoint = m_Waypoint;
						m_Waypoint = new_waypoint;
						m_WaypointIndex = new_waypoint_index;
					}

					return m_Waypoint;
				}

				public void Reset()
				{
					m_Waypoints.Clear();
				}

				public WaypointObject GetNextWaypoint()
				{
					List<WaypointObject> _waypoints = GetValidWaypoints();

					if( _waypoints.Count == 0 )
						return null;

					WaypointObject new_waypoint = null;
					int new_waypoint_index = m_WaypointIndex;

					if( _waypoints.Count == 1 )
					{
						new_waypoint_index = 0;
						new_waypoint = _waypoints[ new_waypoint_index ];
					}
					else if( Order == WaypointOrderType.RANDOM )
					{
						new_waypoint_index = Random.Range(0,_waypoints.Count);
						new_waypoint = _waypoints[ new_waypoint_index ];
					}
					else 
					{
						if( Order == WaypointOrderType.PINGPONG )
						{
							if( Ascending && new_waypoint_index + 1 >= _waypoints.Count )
								Ascending = false;
							else if( ! Ascending && new_waypoint_index - 1 < 0 )
								Ascending = true;
						}
					
						if( Ascending )
						{
							new_waypoint_index++;

							if( new_waypoint_index >= _waypoints.Count )
								new_waypoint_index = 0;
						}
						else
						{
							new_waypoint_index--;
							
							if( new_waypoint_index < 0 )
								new_waypoint_index = _waypoints.Count - 1;
						}

						new_waypoint = _waypoints[ new_waypoint_index ];
					}


					if( new_waypoint != null )
					{
						m_LastWaypoint = m_Waypoint;
						m_Waypoint = new_waypoint;
						m_WaypointIndex = new_waypoint_index;
					}

					return m_Waypoint;
				
				}
			}


		}
	}
}
