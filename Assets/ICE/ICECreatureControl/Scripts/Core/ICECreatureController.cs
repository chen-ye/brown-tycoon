using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;

namespace ICE.Creatures
{


	/// <summary>
	/// ICECreatureController Core Component 
	/// </summary>
	public abstract class ICECreatureController : MonoBehaviour {

		public DisplayData Display = new DisplayData();

		private bool m_CoUpdateIsRunning = false;

		[SerializeField]
		private CreatureObject m_Creature = new CreatureObject();
		public CreatureObject Creature{
			set{ m_Creature = value; }
			get{ return m_Creature; }
		}

		void Awake () {
			
			if( Creature.DontDestroyOnLoad )
				DontDestroyOnLoad(this);

			// wakes up the creature ...
			Creature.Init( gameObject );
		}

		void Start () {

			// initial start of the update coroutine (if required)
			if( Creature.UseCoroutine && m_CoUpdateIsRunning == false )
				StartCoroutine( CoUpdate() );
		}

		void OnEnable() {

			// if script or gameobject were disabled, we have to start the coroutine again ... 
			if( Creature.UseCoroutine && m_CoUpdateIsRunning == false )
				StartCoroutine( CoUpdate() );
		}

		void OnDisable() {

			// deactivating the gameobject will stopping the coroutine, so we capture the ondisable 
			// and stops the coroutine controlled ... btw. if only the script was disabled, the coroutine would be 
			// still running, but we don't need the coroutine if the rest of the script isn't running and so we 
			// capture all cases
			if( Creature.UseCoroutine )
				StopCoroutine( CoUpdate() );

			m_CoUpdateIsRunning = false;
		}

		void OnDestroy() {
			// informs the creature register that the creature leaves 
			Creature.Bye();
		}

		//********************************************************************************
		// Coroutine Update
		//********************************************************************************
		IEnumerator CoUpdate()
		{
			while( Creature.UseCoroutine )
			{
				// coroutine is alive ... 
				m_CoUpdateIsRunning = true;

				// sense is using its internal timer ... so we don't need an other yield here ...
				Sense();

				// react is using its internal timer ... so we don't need an other yield here ...
				React();

				yield return null;
			}		
			m_CoUpdateIsRunning = false;
			yield break;
		}

		//********************************************************************************
		// Update
		//********************************************************************************
		void Update () {

			UpdateBegin();
			Creature.UpdateBegin();

			// if you 
			if( Creature.UseCoroutine == false )
			{
				Sense();
				React();
			}

			Move();

			Creature.UpdateComplete();
			UpdateComplete();
		}

		//********************************************************************************
		// Fixed Update
		//********************************************************************************
		void FixedUpdate()
		{
			FixedUpdateBegin();
			Creature.FixedUpdate();
			FixedUpdateComplete();

			m_CoUpdateIsRunning = false;
		}

		void OnCollisionEnter(Collision collision) 
		{
			Creature.HandleCollision( collision );
		}

		public abstract void UpdateBegin();
		public abstract void SenseComplete();
		public abstract void ReactComplete();
		public abstract void MoveComplete();
		public abstract void UpdateComplete();
		
		public abstract void FixedUpdateBegin();
		public abstract void FixedUpdateComplete();

		//********************************************************************************
		// Sense
		//********************************************************************************
		public virtual void Sense()
		{
			// that's a short delay to slowing the creatures sense, because it looks more natural if the 
			// creature can't sence everything in milliseconds. Btw. if you want to use a large crowd/herd 
			// you can use the sence and reaction time also to optimize the performance
			if( Creature.IsSenseTime( transform ) )
			{
				Creature.Interaction.Sense();			
				SenseComplete();
			}
		}

		//********************************************************************************
		// React
		//********************************************************************************
		public virtual void React()
		{
			// that's a short delay to slowing the creatures reaction time, because it looks more natural if the 
			// creature don't react within milliseconds. Btw. if you want to use a large crowd/herd 
			// you can use the sence and reaction time also to optimize the performance
			if( Creature.IsReactionTime( transform ) )
			{
				Creature.AvailableTargets.Clear();

				// PREDECISIONS HOME
				if( Creature.Essentials.TargetReady() )
					Creature.AvailableTargets.Add ( Creature.Essentials.PrepareTarget( Creature ) );

				// PREDECISIONS MISSION OUTPOST
				if( Creature.Missions.Outpost.TargetReady() )
					Creature.AvailableTargets.Add ( Creature.Missions.Outpost.PrepareTarget( transform.gameObject, Creature ) );

				// PREDECISIONS MISSION ESCORT
				if( Creature.Missions.Escort.TargetReady() )
					Creature.AvailableTargets.Add ( Creature.Missions.Escort.PrepareTarget( transform.gameObject, Creature ) );
			
				// PREDECISIONS MISSION PATROL
				if( Creature.Missions.Patrol.TargetReady() )
					Creature.AvailableTargets.Add ( Creature.Missions.Patrol.PrepareTarget( transform.gameObject, Creature ) );

				// PREDECISIONS INTERACTOR
				if( Creature.Interaction.TargetReady( Creature.Environment.CollisionHandler.Target.TargetGameObject ) )
					Creature.AvailableTargets.Add ( Creature.Interaction.PrepareTarget() );

				Creature.SelectBestTarget();

				ReactComplete();
			}
		}

		//********************************************************************************
		// Move
		//********************************************************************************
		public virtual void Move()
		{
			// move should be the only update which is required in each frame ...
			Creature.UpdateMove();
			MoveComplete();
		}

	}
}
