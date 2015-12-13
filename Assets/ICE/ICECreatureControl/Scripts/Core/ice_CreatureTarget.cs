using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class TargetSelectorStatementObject : System.Object
	{
		public TargetSelectorStatementObject(){}
		public TargetSelectorStatementObject( TargetSelectorStatementObject _statement )
		{
			StatementType = _statement.StatementType;
			
			SuccessorType = _statement.SuccessorType;
			SuccessorTargetTag = _statement.SuccessorTargetTag;
			SuccessorTargetName = _statement.SuccessorTargetName;
			SuccessorTargetType = _statement.SuccessorTargetType;

			PriorityMultiplier = _statement.PriorityMultiplier;
		}

		public int Priority = 0;
		public float PriorityMultiplier = 0;

		public TargetSelectorStatementType StatementType = TargetSelectorStatementType.NONE;

		public TargetSuccessorType SuccessorType = TargetSuccessorType.TYPE;
		public string SuccessorTargetTag = "";
		public string SuccessorTargetName = "";
		public TargetType SuccessorTargetType = TargetType.UNDEFINED;
	}


	[System.Serializable]
	public class TargetSelectorConditionObject : System.Object
	{
		public TargetSelectorConditionObject(){}
		public TargetSelectorConditionObject( ConditionalOperatorType _condition_type ){
			ConditionType = _condition_type;
		}

		public TargetSelectorConditionObject( ConditionalOperatorType _condition_type, TargetSelectorExpressionType _selector_type ){

			ConditionType = _condition_type;
			ExpressionType = _selector_type;
		}

		public TargetSelectorConditionObject( TargetSelectorConditionObject _selector )
		{
			if( _selector == null )
				return;

			Enabled = _selector.Enabled;
			ExpressionType = _selector.ExpressionType;
			ConditionType = _selector.ConditionType;
			Operator = _selector.Operator;
			
			Distance = _selector.Distance;
			BehaviourModeKey = _selector.BehaviourModeKey;
			PositionType = _selector.PositionType;
			PositionVector = _selector.PositionVector;

			PrecursorType = _selector.PrecursorType;
			PrecursorTargetTag = _selector.PrecursorTargetTag;
			PrecursorTargetName = _selector.PrecursorTargetName;
			PrecursorTargetType = _selector.PrecursorTargetType;

		}

		public bool Enabled = true;
		public TargetSelectorExpressionType ExpressionType = TargetSelectorExpressionType.NONE;
		public ConditionalOperatorType ConditionType = ConditionalOperatorType.AND;	
		public LogicalOperatorType Operator = LogicalOperatorType.EQUAL;

		public float Distance = 0;
		public string BehaviourModeKey = "";
		public TargetSelectorPositionType PositionType = TargetSelectorPositionType.TARGETMOVEPOSITION;
		public Vector3 PositionVector = Vector3.zero;

		public TargetPrecursorType PrecursorType = TargetPrecursorType.TYPE;
		public string PrecursorTargetTag = "";
		public string PrecursorTargetName = "";
		public TargetType PrecursorTargetType = TargetType.UNDEFINED;

		public string ConditionToString()
		{
			string _condition = "";
			switch( ConditionType )
			{
				case ConditionalOperatorType.OR:
					_condition = "OR";
					break;
				default:
					_condition = "AND";
					break;
			}

			return _condition;
		}

		public string OperatorToString()
		{
			string _operator = "";

			if( ExpressionType == TargetSelectorExpressionType.DISTANCE )
			{
				switch( Operator )
				{
					case LogicalOperatorType.EQUAL:
						_operator = "==";
						break;
					case LogicalOperatorType.NOT:
						_operator = "!=";
						break;
					case LogicalOperatorType.LESS:
						_operator = "<";
						break;
					case LogicalOperatorType.LESS_OR_EQUAL:
						_operator = "<=";
						break;				
					case LogicalOperatorType.GREATER:
						_operator = ">";
						break;
					case LogicalOperatorType.GREATER_OR_EQUAL:
						_operator = ">=";
						break;
				}
			}
			else
			{
				switch( Operator )
				{
					case LogicalOperatorType.NOT:
						_operator = "IS NOT";
						break;
					default:
						_operator = "IS";
						break;
				}
			}

			return _operator;
		}

		public bool ComparePrecursorTarget( TargetObject _precursor )
		{
			if( _precursor == null || _precursor.TargetGameObject == null )
				return false;

			bool _result = false;

			// using EQUAL to get true if one of the conditions are correct 
			if( PrecursorType == TargetPrecursorType.TYPE && PrecursorTargetType == _precursor.Type )
				_result = true;
			else if( PrecursorType == TargetPrecursorType.NAME && PrecursorTargetName == _precursor.TargetGameObject.name )
				_result = true;
			else if( PrecursorType == TargetPrecursorType.TAG && _precursor.TargetGameObject.CompareTag( PrecursorTargetTag ) )
				_result = true;

			// if the desired operator is NOT we have to invert the result of the EQUAL check
			if( Operator == LogicalOperatorType.NOT )
				_result = ! _result;
		
			return _result;
		}
		/*
		public bool CompareSuccessorTarget( TargetObject _successor )
		{
			if( _successor == null || _successor.TargetGameObject == null )
				return false;
			
			if( SuccessorType == TargetSuccessorType.TYPE && SuccessorTargetType == _successor.Type )
				return true;
			else if( SuccessorType == TargetSuccessorType.NAME && SuccessorTargetName == _successor.TargetGameObject.name )
				return true;
			else if( SuccessorType == TargetSuccessorType.TAG && _successor.TargetGameObject.CompareTag( SuccessorTargetTag ) )
				return true;
			else
				return false;
		}*/
	}

	[System.Serializable]
	public class TargetSelectorObject : System.Object
	{
		public TargetSelectorObject(){}
		public TargetSelectorObject( ConditionalOperatorType _condition_type )
		{
			m_Conditions.Add( new TargetSelectorConditionObject( _condition_type, TargetSelectorExpressionType.NONE ) );
		}

		public TargetSelectorObject( TargetSelectorObject _selector )
		{
			if( _selector == null )
				return;

			m_Conditions.Clear();
			foreach( TargetSelectorConditionObject _condition in _selector.Conditions )
				m_Conditions.Add( new TargetSelectorConditionObject( _condition ) );

			m_Statements.Clear();
			foreach( TargetSelectorStatementObject _statement in _selector.Statements )
				m_Statements.Add( new TargetSelectorStatementObject( _statement ) );
		}

		public ConditionalOperatorType ConditionType{
			get{
				if( m_Conditions.Count > 0 )
					return m_Conditions[0].ConditionType;
				else
					return ConditionalOperatorType.AND;
			}
		}
		
		[SerializeField]
		private List<TargetSelectorConditionObject> m_Conditions = new List<TargetSelectorConditionObject>();
		public List<TargetSelectorConditionObject> Conditions{
			set{ m_Conditions = value;}
			get{return m_Conditions; }
		}

		[SerializeField]
		private List<TargetSelectorStatementObject> m_Statements = new List<TargetSelectorStatementObject>();
		public List<TargetSelectorStatementObject> Statements{
			set{ m_Statements = value;}
			get{return m_Statements; }
		}
	}

	[System.Serializable]
	public class TargetSelectorsObject : System.Object
	{
		public TargetSelectorsObject(){}
		public TargetSelectorsObject( TargetType _type ){
			this.m_TargetType = _type;
		}

		private TargetType m_TargetType = TargetType.UNDEFINED; 
		public TargetType TargetType{
			get{return m_TargetType;}
		}

		public bool UseSelectionCriteriaForHome;

		public int Priority = 0;
		public int DefaultPriority = 0;

		public float SelectionRange = 0;
		public bool UseFieldOfView = false;
		public float SelectionAngle = 0;
		public bool CanUseDefaultPriority = false;
		public bool UseDefaultPriority = false;
		public bool UseAdvanced = false;

		private float m_DynamicPriority = 0;
		public float DynamicPriority{
			get{ return UpdateRelevance( 0 ); }
		}

		private float m_RelevanceMultiplier = 0;
		public float RelevanceMultiplier{
			get{ 
				if( m_RelevanceMultiplier > 1 )
					m_RelevanceMultiplier = 1;				
				else if( m_RelevanceMultiplier < -1 )
					m_RelevanceMultiplier = -1;

				return m_RelevanceMultiplier; 			
			}
		}

		public void ResetRelevanceMultiplier(){
			m_RelevanceMultiplier = 0;
		}

		public void SetRelevanceMultiplier( float _relevance_multiplier ){
			m_RelevanceMultiplier = _relevance_multiplier;
		}


		[SerializeField]
		private List<TargetSelectorObject> m_SelectorGroups = new List<TargetSelectorObject>();
		public List<TargetSelectorObject> Selectors{
			set{ m_SelectorGroups = value;}
			get{return m_SelectorGroups; }
		}

		[SerializeField]
		private List<TargetSelectorStatementObject> m_Statements = new List<TargetSelectorStatementObject>();
		public List<TargetSelectorStatementObject> Statements{
			set{ m_Statements = value;}
			get{return m_Statements; }
		}

		public float UpdateRelevance( float _relevance_multiplier )
		{
			m_RelevanceMultiplier += _relevance_multiplier;

			m_DynamicPriority = Priority + ( Priority * RelevanceMultiplier );

			if( m_DynamicPriority > 100 )
				m_DynamicPriority = 100;
			
			if( m_DynamicPriority < 0 )
				m_DynamicPriority = 0;

			return m_DynamicPriority;
		}

		public float CompareSuccessorTargets( TargetObject _target, Vector3 _position )
		{
			/*
			if( _target == null || _target.TargetGameObject == null || UseAdvanced == false )
				return 0;

			float _relevance_multiplier = 0;

			foreach( TargetSelectorObject _group in m_SelectorGroups )
			{
				foreach( TargetSelectorConditionObject _selector in _group.Conditions )
				{
					if( _selector.Enabled == true && _selector.ExpressionType == TargetSelectorExpressionType.SUCCESSOR )
					{
						bool _valid = false;

						if( ( _selector.SuccessorType == TargetSuccessorType.NAME && _target.TargetGameObject.name == _selector.SuccessorTargetName ) ||
							( _selector.SuccessorType == TargetSuccessorType.TAG && _target.TargetGameObject.CompareTag( _selector.SuccessorTargetName ) ) ||
							( _selector.SuccessorType == TargetSuccessorType.TYPE && _target.Type == _selector.SuccessorTargetType ) )
						{
							if(  _selector.UseRange )
								_valid = _target.TargetInSelectionRange( _position, _selector.Distance );
							else
								_valid = true;
						}

						if( _valid == _selector.Included )
							_valid = true;
						else
							_valid = false;

						if( _valid )
						{
							if( _selector.UseMultiplier )
								_relevance_multiplier += _selector.RelevanceMultiplier;
							else
								_relevance_multiplier += 1;
						}
					}
				}
			}
			_target.Selectors.UpdateRelevance( _relevance_multiplier );

			return _relevance_multiplier;*/
			return 0;

		}
		/*
		public float SelectionRange{
			set{
				if( UseComplex )
					AddSelectionRange( value );
				else
					m_SelectionRange = value;
			}
			get{
				if( UseComplex )
					return GetFirstRangeSelectorValue();
				else
					return m_SelectionRange;
			}
		}
		
		public void AddSelectionRange( float _distance )
		{
			if( _distance == 0 )
				return;

			TargetSelectorObject _range_selector = GetFirstRangeSelector();
			
			if( _range_selector == null )
			{
				_range_selector = new TargetSelectorObject( TargetSelectorType.RANGE );
				m_Selectors.Add( _range_selector );
			}
			
			_range_selector.Range = _distance;
		}

		private TargetSelectorObject GetMaxRangeSelector()
		{
			TargetSelectorObject _range_selector = null;
			foreach( TargetSelectorObject _selector in m_Selectors )
			{
				if( _selector.Type == TargetSelectorType.RANGE && ( _range_selector == null ||_selector.Range >= _range_selector.Range ) )
					_range_selector = _selector;
			}
			
			return _range_selector;
		}

		private TargetSelectorObject GetFirstRangeSelector()
		{
			TargetSelectorObject _range_selector = null;

			for( int i = 0; i < m_Selectors.Count ; i++ )
			{
				if( m_Selectors[i] != null && m_Selectors[i].Type == TargetSelectorType.RANGE )
					return  m_Selectors[i];
			}

			return null;
		}

		private float GetFirstRangeSelectorValue()
		{
			TargetSelectorObject _selector = GetFirstRangeSelector();

			if( _selector != null )
				return _selector.Range;
			else
				return 0;
		}*/




		public int GetPriority( TargetType _type )
		{
			if( _type == TargetType.HOME && UseSelectionCriteriaForHome == false )
				return GetDefaultPriorityByType( _type );
			else if( UseAdvanced )
				return (int)DynamicPriority;
			else
				return Priority;
		}

		public int GetDefaultPriorityByType( TargetType _type )
		{
			int _priority = 0;
			if( _type == TargetType.HOME )
				_priority = 0;
			else if( _type == TargetType.INTERACTOR )
				_priority = 60;
			else if( _type == TargetType.PATROL )
				_priority = 50;
			else if( _type == TargetType.WAYPOINT )
				_priority = 50;
			else if( _type == TargetType.OUTPOST )
				_priority = 50;
			else if( _type == TargetType.ESCORT )
				_priority = 55;
			
			return _priority;
		}
		
		public float GetDefaultRangeByType( TargetType _type )
		{
			float _range = 0;
			if( _type == TargetType.HOME )
				_range = 0;	
			else if( _type == TargetType.INTERACTOR )
				_range = 20;
			else if( _type == TargetType.PATROL )
				_range = 0;
			else if( _type == TargetType.WAYPOINT )
				_range = 0;
			else if( _type == TargetType.OUTPOST )
				_range = 0;
			else if( _type == TargetType.ESCORT )
				_range = 0;
			
			return _range;
		}

		public float GetDefaultAngleByType( TargetType _type )
		{
			float _angle = 0;
			if( _type == TargetType.HOME )
				_angle = 0;	
			else if( _type == TargetType.INTERACTOR )
				_angle = 50;
			else if( _type == TargetType.PATROL )
				_angle = 0;
			else if( _type == TargetType.WAYPOINT )
				_angle = 0;
			else if( _type == TargetType.OUTPOST )
				_angle = 0;
			else if( _type == TargetType.ESCORT )
				_angle = 0;
			
			return _angle;
		}

		public void Copy( TargetSelectorsObject _selectors )
		{
			if( _selectors == null )
				return;

			UseSelectionCriteriaForHome = _selectors.UseSelectionCriteriaForHome;
			CanUseDefaultPriority = _selectors.CanUseDefaultPriority;

			Priority = _selectors.Priority;
			UseFieldOfView = _selectors.UseFieldOfView;
			SelectionRange = _selectors.SelectionRange;
			SelectionAngle = _selectors.SelectionAngle;
			DefaultPriority = _selectors.DefaultPriority;
			UseDefaultPriority = _selectors.UseDefaultPriority;
			UseAdvanced = _selectors.UseAdvanced;
			SetRelevanceMultiplier( _selectors.RelevanceMultiplier );

			Selectors.Clear();
			foreach( TargetSelectorObject _group in _selectors.Selectors )
				Selectors.Add( new TargetSelectorObject( _group ) );
		}

	}

	//--------------------------------------------------
	// TargetObject is the data container for all potential targets (home, escort, defender, attacker)
	//--------------------------------------------------
	[System.Serializable]
	public class TargetObject : System.Object
	{

		public TargetObject(){}
		public TargetObject( TargetObject _target ){
			Copy( _target );
		}

		public void Copy( TargetObject _target )
		{
			if( _target == null )
				return;

			BehaviourModeKey = _target.BehaviourModeKey;
			TargetIgnoreLevelDifference = _target.TargetIgnoreLevelDifference;
			IsPrefab = _target.IsPrefab;
			TargetOffset = _target.TargetOffset;
			TargetSmoothingMultiplier = _target.TargetSmoothingMultiplier;
			
			TargetGameObject = _target.TargetGameObject;
			TargetRandomRange = _target.TargetRandomRange;
			TargetStopDistance = _target.TargetStopDistance;
			
			UseUpdateOffsetOnActivateTarget = _target.UseUpdateOffsetOnActivateTarget;
			UseUpdateOffsetOnMovePositionReached = _target.UseUpdateOffsetOnMovePositionReached;
			
			Selectors.Copy( _target.Selectors );
		}

		public TargetObject( TargetType _type )
		{
			m_Type = _type;
		}

		private TargetType m_Type = TargetType.UNDEFINED;
		public TargetType Type{
			get{ return m_Type; }
		}

		[SerializeField]
		private GameObject m_TargetGameObject = null;
		[XmlIgnore]
		public virtual GameObject TargetGameObject{
			set{ m_TargetGameObject = value; }
			get{ 
				/*
				if( Application.isPlaying )
				{
					GameObject _object = null;
					if( m_TargetGameObject != null && m_TargetGameObject.activeInHierarchy == false && IsPrefab == true )
	
					if( _object != null && m_TargetGameObject.activeInHierarchy == true )
						m_TargetGameObject = _object;
				}*/

				return m_TargetGameObject; 
			
			}
		}

		private ICECreatureControl m_Controller = null;
		public ICECreatureControl Controller()
		{
			if( m_TargetGameObject == null )
				return null;

			if( m_Controller == null )
				m_Controller = m_TargetGameObject.GetComponent<ICECreatureControl>();

			return m_Controller;
		}

		public bool TargetIsDead
		{
			get{
				if( Controller() != null )
					return Controller().Creature.Status.IsDead;
				else
					return false;
			}
		}

		public bool IsPrefab = false;

		public MoveType TargetMoveType
		{
			get{
				MoveType _move = MoveType.DEFAULT;

				switch( m_Type )
				{
					case TargetType.INTERACTOR:
						_move = MoveType.DEFAULT;
						break;
					case TargetType.OUTPOST:
						_move = MoveType.RANDOM;
						break;
					case TargetType.ESCORT:
						_move = MoveType.DEFAULT;
						break;
					case TargetType.PATROL:
						_move = MoveType.DEFAULT;
						break;
					case TargetType.WAYPOINT:
						_move = MoveType.DEFAULT;
						break;
					default:
						_move = MoveType.DEFAULT;
						break;
					
				}
				return _move;
			}

		}

		[SerializeField]
		private TargetSelectorsObject m_Selectors = new TargetSelectorsObject();
		public TargetSelectorsObject Selectors{
			set{ m_Selectors = value;}
			get{ return m_Selectors;}
		}

		public int SelectionPriority{
			get{ return Selectors.GetPriority( Type ); }
		}

		public bool TargetInFieldOfView( Transform _transform, float _fov_angle, float _fov_distance )
		{
			if( IsValid == false || _transform == null )
				return false;

			// FOV check isn't required or creatures fov settings OFF or adjusted to a full-circle with an infinity distance
			if( Selectors.UseFieldOfView == false || _fov_angle == 0 || ( _fov_angle == 180 && _fov_distance == 0 ) )
				return true;

			float _target_distance = TargetDistanceTo( _transform.position );

			//distance test - if the target is too far, we don't need further checks ... 
			if( _fov_distance == 0 || _fov_distance >= _target_distance )
			{
				float _angle = Tools.GetDirectionAngle( _transform , m_TargetGameObject.transform.position );

				// FOV test - the target must be inside the given range
				if( Mathf.Abs( _angle ) <= _fov_angle )
					return true;
				else
					return false;
			}
			else
				return false;
		}

		public bool TargetInSelectionRange( Transform _transform, float _fov_angle, float _fov_distance )
		{
			if( TargetInFieldOfView( _transform, _fov_angle, _fov_distance ) && TargetInSelectionRange( _transform ) )
				return true;
			else
				return false;
		}

		public bool TargetInSelectionRange( Transform _transform )
		{
			if( IsValid == false || _transform == null )
				return false;

			// if there are no custom criteria for home, the home target is always available ...
			if( Type == TargetType.HOME && Selectors.UseSelectionCriteriaForHome == false )
				return true;

			// unlimited - the target is always available
			if( Selectors.SelectionRange == 0 || Selectors.SelectionRange == 180 )
				return true;


			float _distance = TargetDistanceTo( _transform.position );

			// additional to the range we have also to check the correct field of view angle
			if( Selectors.SelectionAngle > 0 && Selectors.SelectionAngle < 180 )
			{
				float _angle = Tools.GetDirectionAngle( m_TargetGameObject.transform, _transform.position );

				if( _distance <= Selectors.SelectionRange && Mathf.Abs( _angle ) <= Selectors.SelectionAngle )
					return true;
				else
					return false;
			}

			// here we will check the distance only
			else if( _distance <= Selectors.SelectionRange )
				return true;

			else
				return false;
		}

		//public bool UseOffsetAngle = false;



		private float m_OffsetAngle = 0;
		public float OffsetAngle{
			get{ return m_OffsetAngle;}
		}

		private float m_OffsetDistance = 0;
		public float OffsetDistance{
			get{ return m_OffsetDistance;}
		}

		public Vector3 TargetOffset = Vector3.zero;
		public float TargetRandomRange = 0;
		public float TargetStopDistance = 2;
		public bool TargetIgnoreLevelDifference = true;
		public float TargetSmoothingMultiplier = 0;

		public void UpdateRandomRange( float _random_range )
		{

			if( TargetRandomRange != _random_range || _random_range == 0 )
			{
				TargetRandomRange = _random_range;
				UpdateOffset();
			}
			else
			{
				TargetRandomRange = _random_range;
			}
		}

		public float TargetMaxRange{
			get{ return TargetRandomRange + TargetStopDistance; }
		}

		public string BehaviourModeKey = "";

		private float m_LastTargetVelocity = 0.0f;
		private float m_TargetVelocity = 0.0f;
		private Vector3 m_LastTargetPosition = Vector3.zero;

		private bool m_Active = false;
		public bool Active{
			get{ return m_Active; }
		}
		public void SetActive( bool _value )
		{
			if( m_Active != _value && m_Active == false && ( UseUpdateOffsetOnActivateTarget == true || m_FixedOffset == Vector3.zero ) ) 
				UpdateOffset();

			m_Active = _value;
		}

		public bool IsValid{
			get{
				if( TargetGameObject == null )
					return false;
				else
					return true;
			}
		}


		//--------------------------------------------------
		
		public float TargetVelocity
		{
			get{ return m_TargetVelocity; }
		}

		//--------------------------------------------------
		
		public Vector3 TargetPosition
		{
			get{ 
				if( IsValid )
					return TargetGameObject.transform.position;
				else
					return Vector3.zero;
			}
		}

		//--------------------------------------------------

		public Vector3 TargetDirection
		{
			get{ 
				if( IsValid )
					return TargetGameObject.transform.forward;
				else
					return Vector3.forward;
			}
		}

		//--------------------------------------------------
		
		public Vector3 TargetOffsetPosition
		{
			get{ 
				 if( IsValid == false)
					return Vector3.zero;

				Vector3 _local_offset = TargetOffset;
		
				_local_offset.x = _local_offset.x/TargetGameObject.transform.lossyScale.x;
				_local_offset.y = _local_offset.y/TargetGameObject.transform.lossyScale.y;
				_local_offset.z = _local_offset.z/TargetGameObject.transform.lossyScale.z;

				return TargetGameObject.transform.TransformPoint( _local_offset );
			}
		}

		//--------------------------------------------------

		public bool UseUpdateOffsetOnActivateTarget = false;
		public bool UseUpdateOffsetOnMovePositionReached = false;
		public bool UseUpdateOffsetOnRandomizedTimer = false;
		public float OffsetUpdateTimeMin = 5;
		public float OffsetUpdateTimeMax = 15;

		private float m_OffsetTime = 0;
		private float m_OffsetTimer = 0;

		private Vector3 m_FixedOffset = Vector3.zero;
		public Vector3 UpdateOffset()
		{
			if( ! IsValid )
				return Vector3.zero;

			m_FixedOffset = TargetOffset;

			if( TargetRandomRange > 0 )
			{
				Vector2 point = UnityEngine.Random.insideUnitCircle * TargetRandomRange;
				
				m_FixedOffset.x += point.x;
				m_FixedOffset.z += point.y;
			}

			return m_FixedOffset;

		}

		public void UpdateOffset( Vector3 _offset )
		{
			TargetOffset = _offset;
			//Offset.y = 0;
			float _angle = Tools.GetOffsetAngle( TargetOffset );
			
			m_OffsetAngle = _angle;
			m_OffsetDistance = Vector3.Distance( Vector3.zero, TargetOffset );
		}
		
		public void UpdateOffset( float _angle , float _distance )
		{
			if( ! IsValid )
				return;
			
			m_OffsetAngle = _angle;
			m_OffsetDistance = _distance;
			
			float _offset_angle = OffsetAngle;
			_offset_angle += TargetGameObject.transform.eulerAngles.y;
			
			if( _offset_angle > 360 )
				_offset_angle = _offset_angle - 360;
			
			Vector3 _world_offset = Tools.GetAnglePosition( TargetGameObject.transform.position, _offset_angle, m_OffsetDistance );
			Vector3 _local_offset = TargetGameObject.transform.InverseTransformPoint( _world_offset );
			
			_local_offset.x = _local_offset.x*TargetGameObject.transform.lossyScale.x;
			_local_offset.y = _local_offset.y*TargetGameObject.transform.lossyScale.y;
			_local_offset.z = _local_offset.z*TargetGameObject.transform.lossyScale.z;	
			
			float f = 0.01f;
			_local_offset.x = Mathf.RoundToInt( _local_offset.x/f)*f;
			_local_offset.y = Mathf.RoundToInt( _local_offset.y/f)*f;
			_local_offset.z = Mathf.RoundToInt( _local_offset.z/f)*f;

			Vector3 _diff = m_FixedOffset - TargetOffset;
			TargetOffset = _local_offset;

			m_FixedOffset = TargetOffset + _diff;
		}


		private float m_SmoothingSpeed = 0;

		private Vector3 m_TargetMovePositionRaw = Vector3.zero;
		public Vector3 TargetMovePositionRaw{
			get{ 
				if( ! IsValid )
					return Vector3.zero;
				
				if( m_FixedOffset == Vector3.zero )
					m_FixedOffset = TargetOffset;
				
				Vector3 _local_offset = m_FixedOffset;
				
				_local_offset.x = _local_offset.x/TargetGameObject.transform.lossyScale.x;
				_local_offset.y = _local_offset.y/TargetGameObject.transform.lossyScale.y;
				_local_offset.z = _local_offset.z/TargetGameObject.transform.lossyScale.z;

				m_TargetMovePositionRaw = TargetGameObject.transform.TransformPoint( _local_offset ); 

				return m_TargetMovePositionRaw; 		
			}
		}

		public bool TargetMoveComplete = false;
		private Vector3 m_TargetMovePosition = Vector3.zero;
		public Vector3 TargetMovePosition{
			get{ 

				Vector3 _old_position = m_TargetMovePosition;
				m_TargetMovePosition = TargetMovePositionRaw; 

				if( TargetSmoothingMultiplier > 0 )
				{
					float _distance = Vector3.Distance( _old_position, m_TargetMovePosition );

					if( _distance > 0 )
					{
						// TODO: dependent of several distances not really nice 
						float speed = _distance - ( _distance * TargetSmoothingMultiplier ); 
						if( speed > 0 )
							m_SmoothingSpeed = speed;

						if( m_SmoothingSpeed > 0 )
							m_TargetMovePosition = Vector3.Lerp( _old_position, m_TargetMovePosition, m_SmoothingSpeed * Time.deltaTime );
					}
				}

				return m_TargetMovePosition;
			}
		}

		//--------------------------------------------------
		
		public bool TargetInMaxRange( Vector3 position )
		{
			if( TargetOffsetPositionDistanceTo( position) <= TargetMaxRange )
				return true;
			else
				return false;
		}

		public bool TargetMovePositionReached( Vector3 position )
		{
			if( TargetMovePositionDistanceTo( position) <= TargetStopDistance )
				return true;
			else
				return false;
		}

		public float TargetMovePositionDistanceTo( Vector3 position )
		{
			Vector3 pos_1 = TargetMovePosition;
			Vector3 pos_2 = position;
			
			if( TargetIgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}
			
			return Vector3.Distance( pos_1, pos_2 );
		}

		public float TargetOffsetPositionDistanceTo( Vector3 position )
		{
			Vector3 pos_1 = TargetOffsetPosition;
			Vector3 pos_2 = position;

			if( TargetIgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}

			return Vector3.Distance( pos_1, pos_2 );
		}

		public float TargetDistanceTo( Vector3 position )
		{
			Vector3 pos_1 = TargetPosition;
			Vector3 pos_2 = position;
			
			if( TargetIgnoreLevelDifference )
			{
				pos_1.y = 0;
				pos_2.y = 0;
			}
			
			return Vector3.Distance( pos_1, pos_2 );

		}

		public void Update( GameObject _owner )
		{
			TargetMoveComplete = TargetMovePositionReached( _owner.transform.position );

			if( UseUpdateOffsetOnRandomizedTimer )
			{
				if( m_OffsetTimer >= m_OffsetTime )
				{
					UpdateOffset();
					m_OffsetTimer = 0;

					m_OffsetTime = Random.Range( OffsetUpdateTimeMin, OffsetUpdateTimeMax );
				}

				m_OffsetTimer += Time.deltaTime;
			}

			if( UseUpdateOffsetOnMovePositionReached && TargetMoveComplete )
				UpdateOffset();
		}

		//--------------------------------------------------
		
		public float FixedUpdate()
		{
			if( ! IsValid )
				return 0.0f;
			
			m_LastTargetVelocity = m_TargetVelocity;
			float _velocity = (( TargetGameObject.transform.position - m_LastTargetPosition ).magnitude) / Time.deltaTime;
			
			if( _velocity == 0 && m_TargetVelocity > 0.5f && m_LastTargetVelocity > 0.75f )
				_velocity = ( _velocity + m_TargetVelocity + m_LastTargetVelocity ) / 3;
			
			m_LastTargetVelocity = m_TargetVelocity;
			m_TargetVelocity = _velocity;
			
			m_LastTargetPosition = TargetGameObject.transform.position;
			
			//UnityEngine.Debug.Log ( "TargetVelocityUpdate - " + m_TargetGameObject.name + " (" +m_TargetVelocity+ "/" + m_LastTargetVelocity + ")");
			
			return m_TargetVelocity;
		}
	}

}
