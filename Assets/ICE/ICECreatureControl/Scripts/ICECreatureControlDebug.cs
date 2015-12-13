using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.Objects;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures
{
	[RequireComponent (typeof (ICECreatureControl))]
	public class ICECreatureControlDebug : MonoBehaviour 
	{
		//--------------------------------------------------
		// Action Object
		//--------------------------------------------------
		[SerializeField]
		private DebugObject m_CreatureDebug = new DebugObject();
		public DebugObject CreatureDebug
		{
			set{ m_CreatureDebug = value; }
			get{ return m_CreatureDebug; }
		}

		void Awake () {
			
			CreatureDebug.Init( gameObject );

			CreatureDebug.CreatureControl.Creature.Move.OnMoveComplete += OnMoveComplete;
			CreatureDebug.CreatureControl.Creature.Move.OnUpdateMovePosition += OnMoveUpdatePosition;

		}

		void Start () 
		{


		}


		void Update () 
		{

		}

		void OnMoveComplete( GameObject _sender, TargetObject _target  )
		{
	
			if( m_CreatureDebug.MovePointer.Enabled && m_CreatureDebug.MovePointer.Pointer != null )
				m_CreatureDebug.MovePointer.Pointer.transform.position = m_CreatureDebug.CreatureControl.Creature.Move.MovePosition;

			if( m_CreatureDebug.TargetPositionPointer.Enabled && m_CreatureDebug.TargetPositionPointer.Pointer != null &&  _target != null )
				m_CreatureDebug.TargetPositionPointer.Pointer.transform.position = _target.TargetMovePosition;

			m_CreatureDebug.DebugLog();
		}

		void OnMoveUpdatePosition(  GameObject _sender, Vector3 _origin_position, ref Vector3 _new_position )
		{
			/*if( m_CreatureDebug.DebugLogEnabled )
				Debug.Log ( _origin_position.ToString() + " - " + _new_position.ToString() );*/
		}
		

		
		private void OnDrawGizmosSelected()
		{
			if( ! m_CreatureDebug.Gizmos.Enabled || ! this.enabled )
				return;

			m_CreatureDebug.Init( gameObject );
			/*
			if( ready )
				StartCoroutine( DoDrawGizmosSelected() );	*/
			m_CreatureDebug.Gizmos.DrawHome();
			m_CreatureDebug.Gizmos.DrawOutpost();
			m_CreatureDebug.Gizmos.DrawEscort();
			m_CreatureDebug.Gizmos.DrawPatrol();
			m_CreatureDebug.Gizmos.DrawInteraction();

		}
/*
		private bool ready = true;

		private IEnumerator DoDrawGizmosSelected()
		{
			ready = false;
			m_CreatureDebug.Gizmos.DrawHome();
			m_CreatureDebug.Gizmos.DrawOutpost();
			m_CreatureDebug.Gizmos.DrawEscort();
			m_CreatureDebug.Gizmos.DrawPatrol();
			m_CreatureDebug.Gizmos.DrawInteraction();
			ready = true;
			yield return null;
		}*/
		
		private void OnDrawGizmos()
		{
			//Action.DrawGizmoIcons( transform );
			
		}
	}
}
