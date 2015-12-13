using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;

namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class EssentialsObject : System.Object 
	{
		public EssentialsObject()
		{
			m_Target = new TargetObject( TargetType.HOME );
		}

		private GameObject m_Owner = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;
		}

		[SerializeField]
		private TargetObject m_Target = null;
		public virtual TargetObject Target 
		{
			set{ m_Target = value; }
			get{ return m_Target; }
		}

		public string BehaviourModeTravel;
		public string BehaviourModeRendezvous;
		public string BehaviourModeLeisure;
		public string BehaviourModeRespawn;
		public string BehaviourModeDead;

		public bool TargetReady()
		{
			if( Target.TargetGameObject != null )
				return true;
			else
				return false;
		}

		public TargetObject PrepareTarget( CreatureObject _creature )
		{
			if( TargetReady() == false || m_Owner == null || _creature == null || _creature.Behaviour == null )
				return null;
			
			// if the active target is not a HOME or if the creature outside the max range it have to travel to reach its target
			if( ! Target.TargetInMaxRange( m_Owner.transform.position ))
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
}
