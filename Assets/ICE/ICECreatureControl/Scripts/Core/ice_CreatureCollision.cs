using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class CollisionDataObject : System.Object
	{
		public bool Foldout = true;
		public bool Enabled = true;
		public string Name = "";
		public bool ForceInteraction = false;
		public CollisionConditionType Type = CollisionConditionType.TAG;
		public int TagPriority = 0;
		public string Tag = "";
		public int LayerPriority = 0;
		public int Layer = 0;
		public StatusContainer Influences;
		public string BehaviourModeKey = "";

	}

	[System.Serializable]
	public class CollisionObject : System.Object
	{
		public CollisionObject()
		{

		}

		public bool Foldout = true;
		public bool Enabled = false;

		private TargetObject m_Target = null;
		public virtual TargetObject Target 
		{
			get{ 

				if( m_Target == null )
				   m_Target = new TargetObject( TargetType.UNDEFINED );

				return m_Target; 
			
			}
		}

		public List<CollisionDataObject> Collisions = new List<CollisionDataObject>();
		
		//private GameObject m_Owner = null;
		
		public void Init( GameObject gameObject )
		{
			//m_Owner = gameObject;
		}
		/*
		public CollisionDataObject CheckExternal( GameObject _object )
		{
			if( _object == null )
				return null;

			CollisionDataObject _current_collision = null;
			
			foreach( CollisionDataObject _collision in Collisions )
			{
				if( _collision.Enabled && _collision.Type == ImpactType.EXTERN )
					_current_collision = _collision;
			}
			
			if( _current_collision != null && _current_collision.ForceInteraction )
			{
				Target.TargetGameObject = _object;
				Target.TargetOffset.z = 2;
				Target.TargetStopDistance = 2;
				Target.TargetRandomRange = 0;
				Target.BehaviourModeKey = _current_collision.BehaviourModeKey;
			}
			
			return _current_collision;
		}*/

		public CollisionDataObject CheckCollision( Collision _collision )
		{
			if( _collision == null || _collision.collider == null )
				return null;

			string _tag = _collision.collider.tag;
			int _layer = _collision.collider.gameObject.layer;
			int _current_total = 0;

			CollisionDataObject _current_impact = null;

			foreach( CollisionDataObject _impact in Collisions )
			{
				if( _impact.Enabled )
				{
					int _total = 0;

					if( _impact.LayerPriority > 0 && _impact.Layer == _layer && ( _current_impact == null || _impact.LayerPriority > _current_impact.LayerPriority  ) )
						_total += _impact.LayerPriority;

					if( _impact.TagPriority > 0 && _impact.Tag == _tag && ( _current_impact == null || _impact.TagPriority > _current_impact.TagPriority  ) )
						_total += _impact.TagPriority;

					if( _total > _current_total )
					{
						_current_total = _total;
						_current_impact = _impact;
					}
				}
			}

			if( _current_impact != null && _current_impact.ForceInteraction )
			{
				Target.TargetGameObject = _collision.collider.gameObject;
				Target.TargetOffset.z = 2;
				Target.TargetStopDistance = 2;
				Target.TargetRandomRange = 0;
				Target.BehaviourModeKey = _current_impact.BehaviourModeKey;
			}

			return _current_impact;
		}

		public bool HasTarget()
		{
			if( m_Target != null && m_Target.TargetGameObject != null ) 
				return true;
			else
				return false;
		}
	}
}