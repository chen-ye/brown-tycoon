using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{
	[System.Serializable]
	public struct DetourContainer
	{
		public Vector3 Position;
	}

	[System.Serializable]
	public struct EscapeContainer
	{
		public float EscapeDistance;
		public float RandomEscapeAngle;
	}

	[System.Serializable]
	public struct AvoidContainer
	{
		public float AvoidDistance;
	}

	[System.Serializable]
	public struct OrbitContainer
	{
		public float Radius;
		public float RadiusShift;
		public float MaxDistance;
		public float MinDistance;
	}

	[System.Serializable]
	public struct VelocityContainer
	{
		public VelocityType Type;
		public Vector3 Velocity;
		public bool UseNegativeVelocity;
		public float VelocityMinVariance;
		public float VelocityMaxVariance;
		public bool UseTargetVelocity;
		public float AngularVelocity;
		public bool AngularVelocityAuto;
		public float Inertia;

		public bool UseAutoDrift;
		public float DriftMultiplier;

		public float VelocityMultiplier;
		public float VelocityMultiplierUpdateTimer;
		public float VelocityMultiplierUpdateInterval;
		public float UpdateVelocityMultiplier()
		{
			VelocityMultiplier = Random.Range( VelocityMinVariance, VelocityMaxVariance );
			
			return VelocityMultiplier;
		}
		
		public float GetVelocityMultiplier()
		{
			//VelocityMultiplier = Random.Range( VelocityMinVariance, VelocityMaxVariance );
			
			return VelocityMultiplier;
		}

		public Vector3 GetVelocity( ICECreatureControl _control )
		{
			float _target_velocity = _control.Creature.Move.CurrentTarget.TargetVelocity;
			Vector3 _velocity = _control.Creature.Move.MoveVelocity;
			float _speed = 0;

			// FORWARD BEGIN
			if( UseTargetVelocity && _target_velocity > Velocity.z )
				_speed = _target_velocity + ( _target_velocity * GetVelocityMultiplier() ); 
			else if( ! UseTargetVelocity )
				_speed = Velocity.z + ( Velocity.z * GetVelocityMultiplier() ); 

			if( Type == VelocityType.ADVANCED && Inertia > 0 && _velocity.z != _speed )
			{
				_velocity.z += ( _speed - _velocity.z ) * Inertia;
			}
			else
				_velocity.z = _speed;
			
			_velocity.z *= _control.Creature.Status.SpeedMultiplier;
			// FORWARD END

			// DRIFT BEGIN

			// DRIFT END

			return _velocity;
		}

		public float GetTurnSpeed( ICECreatureControl _control )
		{
			float _speed = 0;

			if( AngularVelocityAuto && Velocity.z > 0 )
				_speed = ( ( 25 / Velocity.z / 4  ) );
			else
				_speed = AngularVelocity;

			return _speed;
		}
	}

	[System.Serializable]
	public struct MoveContainer
	{
		public bool Enabled;

		public MoveType Type;

		public DetourContainer Detour;
		public OrbitContainer Orbit;
		public EscapeContainer Escape;
		public AvoidContainer Avoid;
		public VelocityContainer Velocity;


		public ViewingDirectionType ViewingDirection;
		public Vector3 ViewingDirectionPosition;

		// DEFAULT
		public float MoveStopDistance;
		public float MoveSegmentLength;
		public float MoveSegmentVariance;	
		public float MoveLateralVariance;		
		public bool MoveIgnoreLevelDifference;

		public float GetMoveLateralVariance()
		{
			float _lateral_variance = MoveLateralVariance;
			float _segment_legth = MoveSegmentLength;

			if(  _segment_legth > 0 && _lateral_variance > 0 )
				_lateral_variance = _segment_legth * Random.Range( - _lateral_variance, _lateral_variance );

			return _lateral_variance;
		}

		public float GetMoveSegmentLength()
		{
			float _directional_variance = MoveSegmentVariance;
			float _segment_legth = MoveSegmentLength;
			
			if( _segment_legth > 0 && _directional_variance > 0 )
				_segment_legth += _segment_legth * Random.Range( - _directional_variance, _directional_variance );

			return _segment_legth;
		}

		public float MoveSegmentLengthMax{
			get{ 
				float _directional_variance = MoveSegmentVariance;
				float _segment_legth = MoveSegmentLength;

				if( _directional_variance > 0 )
					_segment_legth += _segment_legth * _directional_variance;
				return _segment_legth;
			}
		}

		public float MoveSegmentLengthMin{
			get{ 
				float _directional_variance = MoveSegmentVariance;
				float _segment_legth = MoveSegmentLength;
				
				if( _directional_variance > 0 )
					_segment_legth += _segment_legth *  - _directional_variance;
				return _segment_legth;
			}
		}

		public float GetMaxMoveSegmentLength()
		{
			float _directional_variance = MoveSegmentVariance;
			float _segment_legth = MoveSegmentLength;
			
			if( _segment_legth > 0 && _directional_variance > 0 )
				_segment_legth *= Random.Range( - _directional_variance, _directional_variance );
			
			return _segment_legth;
		}

		public MoveCompleteType Link;
		public string NextBehaviourModeKey;

		public MoveContainer( MoveType _type )
		{
			this.Enabled = false;
			this.Type = _type;

			this.ViewingDirection = ViewingDirectionType.DEFAULT;
			this.ViewingDirectionPosition = Vector3.zero;

			this.Detour = new DetourContainer();
			this.Orbit = new OrbitContainer();
			this.Escape = new EscapeContainer();
			this.Avoid = new AvoidContainer();
			this.Velocity = new VelocityContainer();

			this.MoveStopDistance = 3;
			this.MoveSegmentLength = 10;
			this.MoveSegmentVariance = 0;
			this.MoveLateralVariance = 0;
			this.MoveIgnoreLevelDifference = true;

			this.Link = MoveCompleteType.DEFAULT;
			this.NextBehaviourModeKey = "";

		}


	}


		/// <summary>
	/// Move object. Handles all creature motions.
	/// </summary>
	[System.Serializable]
	public class MoveObject : System.Object
	{
		private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			m_NavMeshAgent = m_Owner.GetComponent<NavMeshAgent>();

						
		}



		private ICECreatureControl m_Controller = null;
		private ICECreatureControl Controller
		{
			get{
				if( m_Controller == null )
					m_Controller = m_Owner.GetComponent<ICECreatureControl>();

				return m_Controller;
			}
		}


		// Event Handler
		public delegate void OnTargetMovePositionReachedEvent( GameObject _sender, TargetObject _target );
		public event OnTargetMovePositionReachedEvent OnTargetMovePositionReached;

		public delegate void OnMoveCompleteEvent( GameObject _sender, TargetObject _target );
		public event OnMoveCompleteEvent OnMoveComplete;

		public delegate void OnMoveUpdatePositionEvent( GameObject _sender, Vector3 _origin_position, ref Vector3 _new_position );
		public event OnMoveUpdatePositionEvent OnUpdateMovePosition;

		public MoveContainer DefaultMove = new MoveContainer( MoveType.DEFAULT );
		public MoveContainer CurrentMove;

		//private TargetObject m_PreviousTarget = null;
		private TargetObject m_CurrentTarget = null;
		public TargetObject CurrentTarget{
			get{ return m_CurrentTarget; }
		}

		//private BehaviourModeRuleObject m_PreviousBehaviourModeRule = null;
		private BehaviourModeRuleObject m_CurrentBehaviourModeRule = null;
		public BehaviourModeRuleObject CurrentBehaviourModeRule{
			get{ return m_CurrentBehaviourModeRule; }
		}

		private Vector3 m_LastMovePosition = Vector3.zero;
		public Vector3 LastMovePosition{
			get{ return m_LastMovePosition; }
		}

		private Vector3 m_MovePosition = Vector3.zero;
		public Vector3 MovePosition{
			get{ 
				if( m_MovePosition == Vector3.zero && m_Owner != null )
				{
					if( m_Owner.GetComponent<ICECreatureControl>() != null && m_Owner.GetComponent<ICECreatureControl>().Creature.ActiveTarget != null )
						m_MovePosition = m_Owner.GetComponent<ICECreatureControl>().Creature.ActiveTarget.TargetMovePosition;
					else
						m_MovePosition = m_Owner.transform.position;
				}

				return m_MovePosition; 
			
			}
		}

		private Vector3 m_MoveVelocity = Vector3.zero;
		public Vector3 MoveVelocity{
			get{ return m_MoveVelocity; }
		}

		private float m_MoveAngularVelocity = 0.0f;
		public float MoveAngularVelocity{
			get{ return m_MoveAngularVelocity; }
		}

		public float MoveAvoidDistance = 5;
		public float MoveStopDistance = 2;


		private float m_MoveMaxDistance = 10.0f; 
		public float MoveMaxDistance {
			get{ return m_MoveMaxDistance; }
		}

		public bool UseDeadlockHandling = false;



		public float BaseOffset = 0;
		public float MaxGradientAngle = 45;
		public float MinAvoidAngle = 0.35f;

		public bool UseInternalGravity = true;
		public bool UseWorldGravity = true;
		[SerializeField]
		private float m_Gravity = 9.8f;
		public float Gravity{
			set{
				if( ! UseWorldGravity )
					m_Gravity = value;
			}
			get{
				if( UseWorldGravity )
					return Physics.gravity.y * -1;
				else				
					return m_Gravity;
			}
		}



		[SerializeField]
		private bool m_UseRigidbody = false;
		public bool UseRigidbody{
			set{ m_UseRigidbody = value; }
			get{ return m_UseRigidbody; }
		}

		[SerializeField]
		private bool m_UseCharacterController = false;
		public bool UseCharacterController{
			set{ m_UseCharacterController = value; }
			get{ return m_UseCharacterController; }
		}

		private Rigidbody m_Rigidbody = null;
		public Rigidbody RigidbodyComponent{
			get{
				if( m_Rigidbody == null )
					m_Rigidbody = m_Owner.GetComponent<Rigidbody>();
				
				return m_Rigidbody;
			}
		}

		[SerializeField]
		private bool m_UseNavMesh = false;
		public bool UseNavMesh{
			set{ m_UseNavMesh = value; }
			get{ return m_UseNavMesh; }
		}

		public bool NavMeshForced = false;
		private NavMeshAgent m_NavMeshAgent = null;
		public NavMeshAgent NavMeshAgentComponent
		{
			get{

				if( m_NavMeshAgent == null )
					m_NavMeshAgent = m_Owner.GetComponent<NavMeshAgent>();

				return m_NavMeshAgent;
			}
		}

		public int m_NavMeshPathPendingCounter;

		public bool NavMeshAgentReady
		{
			get{
				if( m_UseNavMesh == true && NavMeshAgentComponent != null )
					return true;
				else
					return false;
			}

		}

		public bool NavMeshAgentHasPath
		{
			get{
				if( NavMeshAgentReady == true && m_NavMeshAgent.hasPath == true )
					return true;
				else
					return false;
			}
			
		}

		public bool NavMeshAgentIsOnNavMesh
		{
			get{
				if( NavMeshAgentReady == true && m_NavMeshAgent.isOnNavMesh == true )
					return true;
				else
					return false;
			}
			
		}

		private CharacterController m_CharacterController = null;
		private CharacterController CharacterControllerComponent
		{
			get{
				
				if( m_CharacterController == null )
					m_CharacterController = m_Owner.GetComponent<CharacterController>();
				
				return m_CharacterController;
			}
		}

		public bool CharacterControllerReady
		{
			get{
				if( m_UseCharacterController == true && CharacterControllerComponent != null )
					return true;
				else
					return false;
			}
		}



		public GroundOrientationType GroundOrientation = GroundOrientationType.NONE;
		public bool GroundOrientationPlus = true; 
		public GroundCheckType GroundCheck = GroundCheckType.RAYCAST;
		public bool UseLeaningTurn = false;
		public float MaxLeanAngle = 30;
		public float LeanAngleMultiplier = 0.5f;

		public float FieldOfView = 0;
		public float VisualRange = 0;

		public int GroundLayer = 0;

		[SerializeField]
		private List<string> m_GroundLayers = new List<string>();
		public List<string> GroundLayers{
			get{ return m_GroundLayers; }
		}

		private bool m_HasOrbit = false;
		public bool HasOrbit{
			get{return m_HasOrbit;}
		}

		private bool m_OrbitComplete = false;
		public bool OrbitComplete{
			get{return m_OrbitComplete;}
		}

		private bool m_HasDetour = false;
		public bool HasDetour{
			get{return m_HasDetour;}
		}

		private bool m_DetourComplete = false;
		public bool DetourComplete{
			get{return m_DetourComplete;}
		}

		bool m_HasEscapePosition = false;
		public bool HasEscape{
			get{return m_HasEscapePosition;}
		}

		bool m_HasAvoidPosition = false;
		public bool HasAvoid{
			get{return m_HasAvoidPosition;}
		}

		/// <summary>
		/// Stops the move.
		/// </summary>
		public void StopMove()
		{
			if( NavMeshAgentReady && m_NavMeshAgent.hasPath )
				m_NavMeshAgent.ResetPath();
		}


		/// <summary>
		/// Updates the move.
		/// </summary>
		/// <param name="behaviour">Behaviour.</param>
		/// <param name="target">Target.</param>
		public void UpdateMove( TargetObject _target, BehaviourModeRuleObject _rule )
		{
			if( m_Owner == null || _rule == null || _target == null || _target.IsValid == false )
			{
				//m_PreviousTarget = m_CurrentTarget;
				m_CurrentTarget = null;

				//m_PreviousBehaviourModeRule = m_CurrentBehaviourModeRule;
				m_CurrentBehaviourModeRule = null;

				return;
			}

			if( m_CurrentTarget != _target )
			{
				//m_PreviousTarget = m_CurrentTarget;
				m_CurrentTarget = _target;
			}

			if( m_CurrentBehaviourModeRule != _rule )
			{
				//m_PreviousBehaviourModeRule = m_CurrentBehaviourModeRule;
				m_CurrentBehaviourModeRule = _rule;

				m_CurrentBehaviourModeRule.Move.Velocity.UpdateVelocityMultiplier();
				m_MovePosition = Vector3.zero;
			}

			CurrentMove = m_CurrentBehaviourModeRule.Move;

			if( m_CurrentBehaviourModeRule.Move.Type == MoveType.DEFAULT )
			{
				CurrentMove.MoveSegmentLength = DefaultMove.MoveSegmentLength;
				CurrentMove.MoveStopDistance = DefaultMove.MoveStopDistance;
				CurrentMove.MoveSegmentVariance = DefaultMove.MoveSegmentVariance;
				CurrentMove.MoveLateralVariance = DefaultMove.MoveLateralVariance;
				CurrentMove.MoveIgnoreLevelDifference = DefaultMove.MoveIgnoreLevelDifference;
			}
				
			// VELOCITY BEGIN
			if( m_CurrentBehaviourModeRule.MoveRequired )
			{
				m_MoveVelocity = CurrentMove.Velocity.GetVelocity( Controller );
				m_MoveAngularVelocity = CurrentMove.Velocity.GetTurnSpeed( Controller );
			}
			else
			{
				m_MoveVelocity = Vector3.zero;
				m_MoveAngularVelocity = 0;
			}





			// VELOCITY END

			if( CurrentMove.Type == MoveType.DETOUR )
				m_HasDetour = true;
			else
			{
				m_HasDetour = false;
				m_DetourComplete = false;
			}

			if( CurrentMove.Type == MoveType.ORBIT )
				m_HasOrbit = true;
			else
			{
				m_HasOrbit = false;
				m_OrbitComplete = false;
			}

			if( CurrentMove.Type == MoveType.ESCAPE )
			{
			}
			else
			{
				m_HasEscapePosition = false;
			}

			// HANDLE TARGET MOVE POSITION
			if( m_CurrentTarget.TargetMovePositionReached( m_Owner.transform.position ) )
			{
				if( OnTargetMovePositionReached != null )
					OnTargetMovePositionReached( m_Owner, m_CurrentTarget );
			}

			m_MovePosition = GetMovePosition();

			if( NavMeshAgentReady )
			{
				m_NavMeshAgent.speed = m_MoveVelocity.z;
				m_NavMeshAgent.angularSpeed = m_MoveAngularVelocity * 100;
				m_NavMeshAgent.stoppingDistance = CurrentMove.MoveStopDistance;

				if( m_MovePosition != m_LastMovePosition || ! m_NavMeshAgent.hasPath )
				{
					m_NavMeshAgent.SetDestination( m_MovePosition );
					m_LastMovePosition = m_MovePosition;
				}

				if( m_MoveVelocity == Vector3.zero )
				{
					m_NavMeshAgent.Stop(); 

					if( m_NavMeshAgent.hasPath )
						m_NavMeshAgent.ResetPath();
				}
				else if( m_NavMeshAgent.hasPath ) 
					m_NavMeshAgent.Resume();

				if( CurrentMove.ViewingDirection != ViewingDirectionType.DEFAULT )
				{
					// ROTATION BEGIN
					Vector3 _heading = m_Owner.transform.forward;
					
					if( CurrentMove.ViewingDirection == ViewingDirectionType.CENTER )
						_heading = m_CurrentTarget.TargetPosition - m_Owner.transform.position;
					else if( CurrentMove.ViewingDirection == ViewingDirectionType.OFFSET )
						_heading = m_CurrentTarget.TargetOffsetPosition - m_Owner.transform.position;
					else if( CurrentMove.ViewingDirection == ViewingDirectionType.MOVE )
						_heading = m_CurrentTarget.TargetMovePosition - m_Owner.transform.position;
					else if( CurrentMove.ViewingDirection == ViewingDirectionType.POSITION )
						_heading = CurrentMove.ViewingDirectionPosition - m_Owner.transform.position;

					Quaternion _rotation = Quaternion.LookRotation ( _heading );
					
					_rotation = HandleGroundOrientation( _rotation );

					m_Owner.transform.rotation = Quaternion.Slerp ( m_Owner.transform.rotation, _rotation, m_MoveAngularVelocity * Time.deltaTime );
					// ROTATION END
				}
				else
				{
					m_Owner.transform.rotation = Quaternion.Slerp ( m_Owner.transform.rotation, HandleGroundOrientation( m_Owner.transform.rotation ), m_MoveAngularVelocity * Time.deltaTime );
				}
			}
			/*
			else if( CharacterControllerReady )
			{
				Vector3 moveDirection = Vector3.zero;

				if ( CharacterControllerComponent.isGrounded ) 
				{
					m_MovePosition.y = GetGroundLevel( m_MovePosition );

					Vector3 _heading = MovePosition - m_Owner.transform.position;
					Quaternion _rotation = Quaternion.LookRotation ( _heading );
					_rotation = HandleGroundOrientation( _rotation );
					
					m_Owner.transform.rotation = Quaternion.Slerp ( m_Owner.transform.rotation, _rotation, m_MoveAngularVelocity * Time.deltaTime );

					moveDirection = m_Owner.transform.TransformDirection( Vector3.forward ) * CurrentMove.Velocity.z * Time.deltaTime;
				}
				moveDirection.y -= Gravity * Time.deltaTime;

				CollisionFlags flags = CharacterControllerComponent.Move( moveDirection );
				m_IsGrounded = (flags & CollisionFlags.CollidedBelow) != 0;
			}*/
			else if( HandleGroundLevel() )
			{
				// MOVE BEGIN
				Vector3 _new_position = m_Owner.transform.position;
				_new_position += m_Owner.transform.TransformDirection( Vector3.forward )* m_MoveVelocity.z * Time.deltaTime;
				_new_position += m_Owner.transform.TransformDirection( Vector3.right )* m_MoveVelocity.x * Time.deltaTime;
				_new_position += m_Owner.transform.TransformDirection( Vector3.up )* m_MoveVelocity.y * Time.deltaTime;

				if( OnUpdateMovePosition != null )
					OnUpdateMovePosition( m_Owner, m_Owner.transform.position, ref _new_position );

				m_Owner.transform.position = _new_position;
				// MOVE END

				// ROTATION BEGIN
				m_LastMovePosition = m_MovePosition;

				Vector3 _move_pos = m_MovePosition;
				Vector3 _creature_pos = m_Owner.transform.position;

				_move_pos.y = 0;
				_creature_pos.y = 0;

				Vector3 _heading = _move_pos - _creature_pos;

				if( CurrentMove.ViewingDirection == ViewingDirectionType.CENTER )
					_heading = m_CurrentTarget.TargetPosition - m_Owner.transform.position;
				else if( CurrentMove.ViewingDirection == ViewingDirectionType.OFFSET )
					_heading = m_CurrentTarget.TargetOffsetPosition - m_Owner.transform.position;
				else if( CurrentMove.ViewingDirection == ViewingDirectionType.MOVE )
					_heading = m_CurrentTarget.TargetMovePosition - m_Owner.transform.position;
				else if( CurrentMove.ViewingDirection == ViewingDirectionType.POSITION )
					_heading = CurrentMove.ViewingDirectionPosition - m_Owner.transform.position;
				
				Quaternion _rotation = Quaternion.LookRotation ( _heading );
				
				_rotation = HandleGroundOrientation( _rotation );
				
				m_Owner.transform.rotation = Quaternion.Slerp ( m_Owner.transform.rotation, _rotation, m_MoveAngularVelocity * Time.deltaTime );
				// ROTATION END
			}

			CheckDeadlock();

			if( OnMoveComplete != null )
				OnMoveComplete( m_Owner, m_CurrentTarget );
		}

		//********************************************************************************
		// DEADLOCK HANDLING
		//********************************************************************************

		private bool m_Deadlocked = false;
		public bool Deadlocked{
			get{ return m_Deadlocked;}
		}

		private List<Vector3> m_DeadlocksCriticalPositions = new List<Vector3>();
		public int DeadlocksCriticalPositions{
			get{ return m_DeadlocksCriticalPositions.Count;}
		}

		private List<Vector3> m_DeadlocksCriticalLoops = new List<Vector3>();
		public int DeadlocksCriticalLoops{
			get{ return m_DeadlocksCriticalLoops.Count;}
		}

		private float m_DeadlockMoveTimer = 0;
		public float DeadlockMoveTimer{
			get{ return m_DeadlockMoveTimer;}
		}

		private float m_DeadlockLoopTimer = 0;
		public float DeadlockLoopTimer{
			get{ return m_DeadlockLoopTimer;}
		}

		
		private int m_DeadlocksCount = 0;
		public int DeadlocksCount{
			get{ return m_DeadlocksCount;}
		}

		private int m_DeadlockLoopsCount = 0;
		public int DeadlockLoopsCount{
			get{ return m_DeadlockLoopsCount;}
		}

		private float m_DeadlocksDistance= 0;
		public float DeadlocksDistance{
			get{ return m_DeadlocksDistance;}
		}

		public DeadlockActionType DeadlockAction = DeadlockActionType.DIE;
		public string DeadlockBehaviour = "";

		public float DeadlockMinMoveDistance = 0.25f;
		public float DeadlockMoveInterval = 2;
		public int DeadlockMoveMaxCriticalPositions = 10;

		public float DeadlockLoopRange = 2f;
		public float DeadlockLoopInterval = 5;
		public int DeadlockLoopMaxCriticalPositions = 10;



		private Vector3 m_DeadlockPosition = Vector3.zero;

		public void ResetDeadlock()
		{
			m_DeadlockMoveTimer = 0;
			m_DeadlockLoopTimer = 0;
			m_Deadlocked = false;
			m_DeadlockPosition = m_Owner.transform.position;
		}

		private bool CheckDeadlock()
		{
			if( UseDeadlockHandling == false )
				return false;

			if( m_MoveVelocity.z == 0 )
			{
				m_DeadlockMoveTimer = 0;
				m_DeadlockLoopTimer = 0;
				return false;
			}

			m_DeadlockMoveTimer += Time.deltaTime;
			m_DeadlockLoopTimer += Time.deltaTime;

			if( m_DeadlockPosition == Vector3.zero )
				m_DeadlockPosition = m_Owner.transform.position;

			if( m_DeadlockMoveTimer >= DeadlockMoveInterval )
			{
				m_DeadlocksDistance = Vector3.Distance( m_Owner.transform.position, m_DeadlockPosition );

				// CHECK DEADLOCK
				if( m_DeadlocksDistance <= DeadlockMinMoveDistance )
				{
					if( m_Deadlocked == false )
						m_DeadlocksCount++;

					m_DeadlocksCriticalPositions.Add( m_Owner.transform.position );

					if( m_DeadlocksCriticalPositions.Count > DeadlockMoveMaxCriticalPositions )
						m_Deadlocked = true;
				}
				else if( m_DeadlocksCriticalPositions.Count > 0 )
					m_DeadlocksCriticalPositions.RemoveAt(0);
				else 
				{
					m_DeadlockPosition = m_Owner.transform.position;
					m_DeadlockMoveTimer = 0;
				}

			
			}

			// CHECK INFINITY LOOP
			if( m_DeadlockLoopTimer >= DeadlockLoopInterval )
			{
				if( m_DeadlocksDistance <= DeadlockLoopRange )
				{
					if( m_Deadlocked == false )
						m_DeadlockLoopsCount++;
					
					m_DeadlocksCriticalLoops.Add( m_Owner.transform.position );
					
					if( m_DeadlocksCriticalLoops.Count > DeadlockLoopMaxCriticalPositions )
						m_Deadlocked = true;
				}
				else if( m_DeadlocksCriticalLoops.Count > 0 )
					m_DeadlocksCriticalLoops.RemoveAt(0);
				else
					m_DeadlockLoopTimer = 0;

			}

			if( m_DeadlockMoveTimer == 0 && m_DeadlocksCriticalPositions.Count == 0 && m_DeadlockLoopTimer == 0 && m_DeadlocksCriticalLoops.Count == 0 )
				m_Deadlocked = false;
				
			return m_Deadlocked;
		
		}



		//********************************************************************************
		// MOVE POSITIONS
		//********************************************************************************

		//********************************************************************************
		// GROUND HANDLING
		//********************************************************************************

		private bool m_IsGrounded = true;
		public bool IsGrounded{
			get{ return m_IsGrounded;}
		}
		
		private float m_GroundeLevel = 0;
		private float m_FallSpeed = 9.8f;
		private float m_Altitude = 0;
		private float m_RaycastOffset = 0;



		/// <summary>
		/// Handles the ground level.
		/// </summary>
		/// <returns><c>true</c>, if ground level was handled, <c>false</c> otherwise.</returns>
		private bool HandleGroundLevel()
		{
			if( GroundCheck == GroundCheckType.NONE || ! UseInternalGravity )
				return true;
			
			Vector3 _position = m_Owner.transform.position;
			
			if( GroundCheck == GroundCheckType.RAYCAST )
			{
				LayerMask layerMask = GetLayerMask();
				RaycastHit _hit;
				Vector3 _pos = _position + ( m_Owner.transform.TransformDirection( Vector3.back ) * m_RaycastOffset );
				_pos.y = 1000;
				if (Physics.Raycast( _pos, Vector3.down, out _hit, Mathf.Infinity, layerMask.value ) )
				{
					if( m_Owner != _hit.collider.gameObject )
						m_GroundeLevel = ( m_GroundeLevel + _hit.point.y ) /2;
					else
						m_RaycastOffset += 0.1f;
					
					//Debug.DrawRay( _pos,Vector3.down * 10000,Color.cyan); 
				}
			}
			else 
				m_GroundeLevel = Terrain.activeTerrain.SampleHeight( _position );
			
			_position.y = m_GroundeLevel;
			
			m_Altitude = m_Owner.transform.position.y + BaseOffset - _position.y;
			
			if( m_Altitude < 0.5f )
			{
				_position.y += BaseOffset * (-1);
				m_Owner.transform.position = _position;
				m_IsGrounded = true;
				
				m_FallSpeed = 0;
				
			}
			else
			{
				m_FallSpeed += Gravity * Time.deltaTime; // not correct but looks good  -9.81 m/s^2 * time
				
				m_Owner.transform.position += m_Owner.transform.TransformDirection( Vector3.down ) * m_FallSpeed * Time.deltaTime;
				m_IsGrounded = false;
			}
			
			return m_IsGrounded;
		}
		
		/// <summary>
		/// Gets the ground level.
		/// </summary>
		/// <returns>The ground level.</returns>
		/// <param name="position">Position.</param>
		public float GetGroundLevel( Vector3 position )
		{
			if( GroundCheck == GroundCheckType.NONE )
				return position.y;
			
			Vector3 pos = position;
			pos.y = 1000;
			
			if( GroundCheck == GroundCheckType.RAYCAST )
			{
				LayerMask _mask = GetLayerMask();
				RaycastHit hit;
				if (Physics.Raycast( pos, Vector3.down, out hit, Mathf.Infinity, _mask.value ) )
					position.y = hit.point.y;
			}
			else 
				position.y = Terrain.activeTerrain.SampleHeight( position );
			
			return position.y;
		}

		private LayerMask GetLayerMask()
		{
			LayerMask _mask = 0;
			
			if( GroundLayers.Count > 0 )
			{
				foreach( string _layer in GroundLayers )
					_mask |= (1 << LayerMask.NameToLayer( _layer ));
			}
			else
				_mask = Physics.DefaultRaycastLayers;

			return _mask;
		}

		public float Width = 0;
		public float Depth = 0;
		public float Height = 0;

		public float WidthOffset = 0;
		public float DepthOffset = 0;

		private Vector3 m_FrontLeftPosition = Vector3.zero;
		public Vector3 FrontLeftPosition{
			get{return m_FrontLeftPosition;}
		}

		private Vector3 m_FrontRightPosition = Vector3.zero;
		public Vector3 FrontRightPosition{
			get{return m_FrontRightPosition;}
		}

		private Vector3 m_BackLeftPosition = Vector3.zero;
		public Vector3 BackLeftPosition{
			get{return m_BackLeftPosition;}
		}

		private Vector3 m_BackRightPosition = Vector3.zero;
		public Vector3 BackRightPosition{
			get{return m_BackRightPosition;}
		}

		private Vector3 m_FrontLeftPositionGround = Vector3.zero;
		public Vector3 FrontLeftPositionGround{
			get{return m_FrontLeftPositionGround;}
		}
		
		private Vector3 m_FrontRightPositionGround = Vector3.zero;
		public Vector3 FrontRightPositionGround{
			get{return m_FrontRightPositionGround;}
		}
		
		private Vector3 m_BackLeftPositionGround = Vector3.zero;
		public Vector3 BackLeftPositionGround{
			get{return m_BackLeftPositionGround;}
		}
		
		private Vector3 m_BackRightPositionGround = Vector3.zero;
		public Vector3 BackRightPositionGround{
			get{return m_BackRightPositionGround;}
		}

		/// <summary>
		/// Handles the ground orientation.
		/// </summary>
		public Quaternion HandleGroundOrientation( Quaternion _rotation )
		{
			if( GroundOrientation == GroundOrientationType.NONE )
				return _rotation;

			float _roll_angle = 0;

			if( UseLeaningTurn )
			{
				Vector3 _heading = m_MovePosition - m_Owner.transform.position;
				float _angle_direction = Tools.AngleDirection( m_Owner.transform.forward, m_Owner.transform.up, _heading ) * (-10);

				_roll_angle = m_MoveVelocity.z * _angle_direction/180 * 100;

				_roll_angle = _roll_angle * LeanAngleMultiplier * 2;

				// limits
				if( _roll_angle > MaxLeanAngle )
					_roll_angle = MaxLeanAngle;

				if( _roll_angle < - MaxLeanAngle )
					_roll_angle = - MaxLeanAngle;

				// prepare euler angle 
				if( _roll_angle < 0 )
					_roll_angle = 360 + _roll_angle;

				//Debug.Log ( _roll_angle );
			}

			if( GroundOrientation == GroundOrientationType.BIPED )
			{
				_rotation = Quaternion.Euler( 0, _rotation.eulerAngles.y, _roll_angle );
			}
			else
			{

				Vector3 ray = Vector3.down;//m_Owner.transform.TransformDirection(Vector3.down); //Vector3.down * 100;//
				Vector3 pos = m_Owner.transform.position;
				Vector3 _normal = Vector3.zero;

				if( Width == 0 )
					Width = ( m_Owner.GetComponentInChildren<Renderer>().bounds.size.x / m_Owner.transform.lossyScale.x );
				
				if( Depth == 0 )
					Depth = ( m_Owner.GetComponentInChildren<Renderer>().bounds.size.z / m_Owner.transform.lossyScale.z );
				
				if( Height == 0 )
					Height = ( m_Owner.GetComponentInChildren<Renderer>().bounds.size.y / m_Owner.transform.lossyScale.y );
				
				
				m_FrontLeftPosition = m_Owner.transform.TransformPoint(new Vector3( - (Width/2)+WidthOffset, 0, (Depth/2)+DepthOffset ));
				m_FrontRightPosition = m_Owner.transform.TransformPoint(new Vector3( (Width/2)+WidthOffset, 0, (Depth/2)+DepthOffset ));
				m_BackLeftPosition = m_Owner.transform.TransformPoint(new Vector3( - (Width/2)+WidthOffset, 0, - (Depth/2)+DepthOffset ));
				m_BackRightPosition = m_Owner.transform.TransformPoint(new Vector3( (Width/2)+WidthOffset, 0, - (Depth/2)+DepthOffset ));
			
				if( GroundCheck == GroundCheckType.RAYCAST )
				{
					if( GroundOrientationPlus == false )
					{
						LayerMask _mask = GetLayerMask();

						RaycastHit hit;
						if( Physics.Raycast( pos, ray ,out hit, Mathf.Infinity, _mask ) )
						{
							if( hit.collider.gameObject.GetComponent<Terrain>() ||  hit.collider.gameObject.GetComponent<MeshFilter>() )
								_normal = hit.normal;
						}
					}
					else
					{


						RaycastHit _hit_back_left;
						RaycastHit _hit_back_right;
						RaycastHit _hit_front_left;
						RaycastHit _hit_front_right;

						LayerMask _mask = GetLayerMask();						
						if( Physics.Raycast( m_FrontLeftPosition + Vector3.up, ray, out _hit_front_left, Mathf.Infinity, _mask ) )
							m_FrontLeftPositionGround = _hit_front_left.point;
						if( Physics.Raycast( m_FrontRightPosition + Vector3.up, ray, out _hit_front_right, Mathf.Infinity, _mask ) )
							m_FrontRightPositionGround = _hit_front_right.point;
						if( Physics.Raycast( m_BackLeftPosition + Vector3.up, ray, out _hit_back_left, Mathf.Infinity, _mask ) )
							m_BackLeftPositionGround = _hit_back_left.point;
						if( Physics.Raycast( m_BackRightPosition + Vector3.up, ray, out _hit_back_right, Mathf.Infinity, _mask ) )
							m_BackRightPositionGround = _hit_back_right.point;
						
						_normal = (Vector3.Cross( m_BackRightPositionGround - Vector3.up, m_BackLeftPositionGround - Vector3.up) +
						           Vector3.Cross( m_BackLeftPositionGround - Vector3.up, m_FrontLeftPositionGround - Vector3.up) +
						           Vector3.Cross( m_FrontLeftPositionGround - Vector3.up, m_FrontRightPositionGround - Vector3.up) +
						           Vector3.Cross( m_FrontRightPositionGround - Vector3.up, m_BackRightPositionGround - Vector3.up)
						           ).normalized;
					}
				}
				else
				{
					if( GroundOrientationPlus == false )
					{
						pos.x =  pos.x - Terrain.activeTerrain.transform.position.x;
						pos.z =  pos.z - Terrain.activeTerrain.transform.position.z;
			
						TerrainData _terrain_data = Terrain.activeTerrain.terrainData;
						_normal = _terrain_data.GetInterpolatedNormal( pos.x/_terrain_data.size.x, pos.z/_terrain_data.size.z );
					}
					else
					{
						m_BackRightPositionGround = m_BackRightPosition;
						m_BackRightPositionGround.y = Terrain.activeTerrain.SampleHeight( m_BackRightPositionGround );

						m_BackLeftPositionGround = m_BackLeftPosition;
						m_BackLeftPositionGround.y = Terrain.activeTerrain.SampleHeight( m_BackLeftPositionGround );

						m_FrontLeftPositionGround = m_FrontLeftPosition;
						m_FrontLeftPositionGround.y = Terrain.activeTerrain.SampleHeight( m_FrontLeftPositionGround );

						m_FrontRightPositionGround = m_FrontRightPosition;
						m_FrontRightPositionGround.y = Terrain.activeTerrain.SampleHeight( m_FrontRightPositionGround );


						_normal = (Vector3.Cross( m_BackRightPositionGround - Vector3.up, m_BackLeftPositionGround - Vector3.up) +
						           Vector3.Cross( m_BackLeftPositionGround - Vector3.up, m_FrontLeftPositionGround - Vector3.up) +
						           Vector3.Cross( m_FrontLeftPositionGround - Vector3.up, m_FrontRightPositionGround - Vector3.up) +
						           Vector3.Cross( m_FrontRightPositionGround - Vector3.up, m_BackRightPositionGround - Vector3.up)
						           ).normalized;
					}
				}	

				//var pitch_angle = Vector3.Angle(Vector3.right, _normal)-90;
				//var sss_angle = Vector3.Angle(Vector3.forward, _normal)-90;

				_rotation = Quaternion.FromToRotation( Vector3.up , _normal ) * _rotation;
	
				if( GroundOrientation == GroundOrientationType.QUADRUPED )
					_rotation = Quaternion.Euler( _rotation.eulerAngles.x, _rotation.eulerAngles.y, _roll_angle );

			}

			return _rotation;
		}



		//********************************************************************************
		// MOVE POSITION HANDLING
		//********************************************************************************

		private Vector3 GetMovePosition()
		{
			Vector3 _position = m_MovePosition;

			if( MovePositionUpdateRequired() )
			{
				switch( CurrentMove.Type )
				{
					case MoveType.AVOID: // AVOID MOVE
						_position = GetAvoidPosition();	
						break;
					case MoveType.ESCAPE: // ESCAPE MOVE
						_position = GetEscapePosition();	
						break;
					case MoveType.ORBIT: // ORBIT MOVE
						_position = GetOrbitPosition();
						break;
					case MoveType.DETOUR: // DETOUR MOVE
						_position = GetDetourPosition();
						break;
					case MoveType.RANDOM: // RANDOM MOVE
						_position = GetRandomPosition();
						break;
					default: // DEFAULT AND CUSTOM MOVE
						_position = m_CurrentTarget.TargetMovePosition;
						break;
				}

				_position = ModulateMovePosition( m_Owner.transform.position, _position );
				
			}

			return _position;
		}

		/// <summary>
		/// Gets the modulated move position.
		/// </summary>
		/// <returns>The modulated move position.</returns>
		/// <param name="_owner_pos">_owner_pos.</param>
		/// <param name="_target_pos">_target_pos.</param>
		private Vector3 ModulateMovePosition ( Vector3 _owner_position, Vector3 _desired_move_position ) {

			Vector3 _position = _desired_move_position;

			if( CurrentMove.MoveSegmentLength > 0 )
			{
				float _target_distance = Vector3.Distance(_owner_position, _desired_move_position);
				float _segment_legth = CurrentMove.GetMoveSegmentLength();

				if( _target_distance > 0 && _segment_legth < _target_distance )
				{
					float _f = _segment_legth/_target_distance;

					if( _f == 0 )
						_f = 0.1f;
				
					_position = Vector3.Lerp( _owner_position, _desired_move_position, _f);

					if( CurrentMove.MoveLateralVariance > 0 )
					{
						Vector3 _forward = _position - _owner_position;
						Vector3 _right = Vector3.Cross( Vector3.up, _forward ).normalized;
						_position = _position + ( _right * CurrentMove.GetMoveLateralVariance() );
					}
				}
			}

			_position.y = GetGroundLevel( _position );

			return _position;

		}

		
		//--------------------------------------------------

		private bool MovePositionUpdateRequired()
		{
			if( CurrentMove.MoveStopDistance == 0 || MovePositionReached() || TargetMovePositionReached() || m_MovePosition == Vector3.zero || m_Deadlocked )
				return true;
			else
				return false;
		}

		//--------------------------------------------------

		public bool MovePositionReached()
		{
			if( MovePositionDistance() <=  CurrentMove.MoveStopDistance )
				return true;
			else
				return false;
		}

		//--------------------------------------------------
		
		public bool TargetMovePositionReached()
		{
			// HANDLE TARGET MOVE POSITION
			if( m_CurrentTarget != null && m_CurrentTarget.IsValid && m_CurrentTarget.TargetMovePositionReached( m_Owner.transform.position ) )
				return true;
			else
				return false;
		}
		
		//--------------------------------------------------

		private float MovePositionDistance()
		{
			if( m_Owner == null )
				return 0;
			
			Vector3 pos_1 = m_MovePosition;
			Vector3 pos_2 = m_Owner.transform.position;
			
			if( CurrentMove.MoveIgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}
			
			return Vector3.Distance(pos_1, pos_2);
		}

		//--------------------------------------------------

		private Vector3 GetDetourPosition()
		{
			if( DetourPositionReached() )
				m_DetourComplete = true;
			
			if( m_DetourComplete )
				return m_CurrentTarget.TargetMovePosition;
			else
				return CurrentMove.Detour.Position;
		}

		//--------------------------------------------------
		public bool DetourPositionReached()
		{
			if( CurrentMove.MoveStopDistance == 0 || DetourPositionDistance() <=  CurrentMove.MoveStopDistance  )
				return true;
			else
				return false;
		}
		
		//--------------------------------------------------
		public float DetourPositionDistance()
		{
			if( m_Owner == null || CurrentMove.Detour.Position == Vector3.zero )
				return 0;
			
			Vector3 pos_1 = CurrentMove.Detour.Position;
			Vector3 pos_2 = m_Owner.transform.position;
			
			if( CurrentMove.MoveIgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}
			
			return Vector3.Distance(pos_1, pos_2);
		}

		//--------------------------------------------------


		/// <summary>
		/// Gets a randomized position.
		/// </summary>
		/// <returns>The random position.</returns>
		private Vector3 GetRandomPosition()
		{
			return Tools.GetRandomPosition( m_Owner.transform.position, CurrentMove.GetMoveSegmentLength() ); 
		}

		//private float m_CurrentAvoidAngle = 0;
		private float m_CreatureRelatedDirectionAngle = 0;
		private Vector3 m_AvoidMovePosition = Vector3.zero;
		public Vector3 AvoidMovePosition {
			get{ return m_AvoidMovePosition;}
		}


		private Vector3 GetAvoidPosition()
		{
			Transform _creature = m_Owner.transform;
			Transform _target = m_CurrentTarget.TargetGameObject.transform;

			m_TargetRelatedDirectionAngle = Tools.GetDirectionAngle( _target, _creature.position );
			m_CreatureRelatedDirectionAngle = Tools.GetDirectionAngle( _creature , _target.position );

			m_HasAvoidPosition = true;
			
			float _avoid_range = CurrentMove.Avoid.AvoidDistance;
			if( ( m_CreatureRelatedDirectionAngle >= 0 && m_TargetRelatedDirectionAngle >= 0 ) || 
			   ( m_CreatureRelatedDirectionAngle >= 0 && m_TargetRelatedDirectionAngle <= 0 ) )// AVOID LEFT 
				_avoid_range *= 1;
			else if( ( m_CreatureRelatedDirectionAngle <= 0 && m_TargetRelatedDirectionAngle <= 0 ) || 
			        ( m_CreatureRelatedDirectionAngle <= 0 && m_TargetRelatedDirectionAngle >= 0 ) ) // AVOID RIGHT
				_avoid_range *= -1;
			
			Vector3 _right = Vector3.Cross( _target.up, _target.forward );
			m_AvoidMovePosition = _target.position + ( _right * _avoid_range );

			//Debug.DrawLine(_target.position, m_AvoidMovePosition, Color.green);

			return m_AvoidMovePosition;

		}

		private float m_TargetRelatedDirectionAngle = 0;
		private Vector3 m_EscapeMovePosition = Vector3.zero;
		public Vector3 EscapeMovePosition {
			get{ return m_EscapeMovePosition;}
		}
		
		private float m_EscapeAngle = 0;
		public float EscapeAngle {
			get{ return m_EscapeAngle;}
		}



		/// <summary>
		/// Gets the escape position.
		/// </summary>
		/// <returns>The escape position.</returns>
		private Vector3 GetEscapePosition()
		{
			Transform _creature = m_Owner.transform;
			Transform _target = m_CurrentTarget.TargetGameObject.transform;

			m_HasEscapePosition = true;

			m_TargetRelatedDirectionAngle = Tools.GetDirectionAngle( _target, _creature.position );
			
			Vector3 _heading = _creature.position - _target.position;
			m_EscapeAngle = Tools.GetOffsetAngle( _heading );			
			m_EscapeAngle += Random.Range( - CurrentMove.Escape.RandomEscapeAngle, CurrentMove.Escape.RandomEscapeAngle );
			m_EscapeMovePosition  = Tools.GetAnglePosition( _target.position, m_EscapeAngle, m_CurrentTarget.Selectors.SelectionRange + CurrentMove.Escape.EscapeDistance );
			
			//Debug.DrawLine(_creature.position, m_EscapeMovePosition, Color.green);

			return m_EscapeMovePosition;
		}

		//--------------------------------------------------

		private float m_OrbitRadius;
		public float OrbitRadius{
			get{ return m_OrbitRadius;}
		}

		private float m_OrbitAngle;
		public float OrbitAngle{
			get{ return m_OrbitAngle;}
		}

		private float m_OrbitDegrees = 10;
		public float OrbitDegrees{
			get{ return m_OrbitDegrees;}
		}

		/// <summary>
		/// Gets the orbit position.
		/// </summary>
		/// <returns>The orbit position.</returns>
		private Vector3 GetOrbitPosition() 
		{ 
			Vector3 _center = m_CurrentTarget.TargetMovePositionRaw;
			float _radius = CurrentMove.Orbit.Radius;
			float _rate = CurrentMove.Orbit.RadiusShift;
			float _min = CurrentMove.Orbit.MinDistance;
			float _max = CurrentMove.Orbit.MaxDistance;

			if( m_OrbitRadius == 0 )
			{
				m_OrbitRadius = _radius;
				m_OrbitAngle = Tools.GetOffsetAngle( m_Owner.transform.position - _center );
				m_OrbitDegrees = m_OrbitDegrees * (Random.Range(0,1) == 1?1:-1);
			}

			if( ( _rate > 0 && m_OrbitRadius < _max ) || ( _rate < 0 && m_OrbitRadius > _min ) )
			{
				m_OrbitRadius += _rate * Time.deltaTime;
				m_OrbitComplete = false;

				if( m_OrbitRadius < _min )
					m_OrbitRadius = _min;
				else if( _max > 0 && m_OrbitRadius > _max )
					m_OrbitRadius = _max;

			}
			else
				m_OrbitComplete = true;
		
			m_OrbitAngle += m_OrbitDegrees;
			
			if( m_OrbitAngle > 360 )
				m_OrbitAngle = m_OrbitAngle - 360;
			
			float _a = m_OrbitAngle * Mathf.PI / 180f;

			Vector3 new_position = _center + new Vector3(Mathf.Sin(_a) * m_OrbitRadius, 0, Mathf.Cos(_a) * m_OrbitRadius );
			//new_position.y = GetGroundLevel( new_position );

			return new_position;

		}
	}
}

