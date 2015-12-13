using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;

namespace ICE.Creatures
{
	/// <summary>
	/// ICE creature control.
	/// </summary>
	/// <description>You can use this class for your own code and|or settings, but please consider,
	/// that this class and also your code will be overwritten by each update/upgrade of the creature package, 
	/// so please save your work whenever you reimport this package and copied it back if the update is done.</description>
	public class ICECreatureControl : ICECreatureController 
	{
		/// <summary>
		/// Update begins.
		/// </summary>
		/// <description>This is the first call of a new update cycle. You could use this abstract method to modify the status of your creature.</description>
		/// <example>
		/// Action.Status.AddDamage( 10 ); // increased the damage of your creature (value in percent) ...
		/// Action.Status.AddDamage( -10 ); // reduced the damage of your creature (value in percent) ...
		/// Action.Status.Temperatur = 25 // adapt the current environmental temperature if required (value in celsius or fahrenheit)...
		/// Action.Status.SetTimeInSeconds( 100 ); coming soon 
		/// Action.Status.SetDateSeconds( 100 ); coming soon
		/// Action.Status.Weather = WeatherType.RAIN; coming soon
		/// Action.Status. ... check the Status member to see all parameter
		/// </example>
		public override void UpdateBegin()
		{
			// Action.Status.AddDamage( 10 ); // increased the damage of your creature (value in percent) ...
			// Action.Status.AddDamage( -10 ); // reduced the damage of your creature (value in percent) ...
			// Action.Status.Temperatur = 25 // adapt the current environmental temperature if required (value in celsius or fahrenheit)...
			// Action.Status.SetTimeInSeconds( 100 ); coming soon 
			// Action.Status.SetDateSeconds( 100 ); coming soon
			// Action.Status.Weather = WeatherType.RAIN; coming soon
			// Action.Status. ... check the Status member to see all parameter
			//Debug.Log ("UpdateBegin");
		}

		/// <summary>
		/// Handles all sensory perception of your creature.
		/// </summary>
		/// <description>CAUTION: USE ONLY IF YOU WANT TO HANDLE ALL SENSE ACTIVITIES BY YOURSELF</description>
		/*
		public override void Sense() // CAUTION: USE ONLY IF YOU WANT TO HANDLE ALL SENSE ACTIVITIES BY YOURSELF
		{
			// that's a short delay to slowing the sensory perception and it's also advantageous for the performance ... :) 
			if( ! Action.Status.IsSenseTime() )
				return;
					
			Debug.Log ("Sense");
		}
		 */

		/// <summary>
		/// sensory perception complete.
		/// </summary>
		/// <description>NOTE: SenseComplete() is called at the end of the Sense method, so please consider the delay due to the SenseTime.</description>
		public override void SenseComplete()
		{
	
			//Debug.Log ("SenseComplete");
		}

		/// <summary>
		/// Handles all reactions and behaviours of your creature.
		/// </summary>
		/// <description>CAUTION: USE ONLY IF YOU WANT TO HANDLE ALL REACT ACTIVITIES BY YOURSELF</description>
		/*
		public override void React() // CAUTION: USE ONLY IF YOU WANT TO HANDLE ALL REACTIONS BY YOURSELF
		{
			Debug.Log ("React");
		}
		*/


		/// <summary>
		/// Reaction complete.
		/// </summary>
		/// <description>NOTE: ReactComplete() is called at the end of the React method, so please consider the delay due to the ReactionTime.</description>
		public override void ReactComplete()
		{
			//Action.Behaviour.SetBehaviourModeByKey( "YOUR_BEHAVIOUR_KEY" );
			//Action.UpdateBehaviour( your_own_target_object );
			//Debug.Log ("ReactComplete");
		}

		/// <summary>
		/// Handles all movements of your creature.
		/// </summary>
		/// <description>This virtual method should be overridden only if you want to handle all movements by yourself, otherwise 
		/// use the abstract method MoveComplete to correct the movements.</description>
		/*
		public override void Move() // CAUTION: USE ONLY IF YOU WANT TO HANDLE ALL MOVEMENTS BY YOURSELF
		{
			Debug.Log ("Move");
		}
		*/

		/// <summary>
		/// All moves complete.
		/// </summary>
		/// <description>You could use this abstract method to improve the movements as required</description>
		public override void MoveComplete()
		{
			//Action.Move.MovePosition ... 	intermediate step for the current move ... this vector is the target vector for the transform position 
			//Action.Move.TargetMovePosition ... the final destination of the current path 
			//transform.position ... direct access to the creature transform ... her you could modify the position and rotation of your creature
			//Debug.Log ("MoveComplete");
		}


		/// <summary>
		/// Update complete.
		/// </summary>
		/// <description>This is the last call of the update cycle. You could use this virtual method to correct all values before drawing the scene </description>
		public override void UpdateComplete()
		{
				/*
			
			if( Creature.TargetChanged )
				Debug.Log( "TARGET INFO : '" + gameObject.name.ToUpper() + "' CHANGED TARGET '" + Creature.PreviousTargetName + "' TO '" + Creature.ActiveTargetName + "'!");
			
			
			if( Creature.Behaviour.BehaviourModeChanged )
				Debug.Log( "BEHAVIOUR INFO : '" + gameObject.name.ToUpper() + "' CHANGED BEHAVIOURMODE '" + Creature.Behaviour.LastBehaviourModeKey + "' TO '" + Creature.Behaviour.BehaviourModeKey + "'!");
			
			if( Creature.Behaviour.BehaviourModeRulesChanged )
				Debug.Log( "BEHAVIOUR INFO : '" + gameObject.name.ToUpper() + "' PREPARES " + Creature.Behaviour.BehaviourMode.Rules.Count +  " RULES FOR '" + Creature.Behaviour.BehaviourModeKey + "'!");
			
			
			if( Creature.Behaviour.BehaviourModeRuleChanged )
				Debug.Log( "BEHAVIOUR INFO : '" + gameObject.name.ToUpper() + "' SELECT 'RULE " + (int)(Creature.Behaviour.BehaviourMode.RuleIndex + 1 )+ "' OF '" + Creature.Behaviour.BehaviourModeKey + "'!");
			*/

		}

		/// <summary>
		/// FixedUpdate begin.
		/// </summary>
		public override void FixedUpdateBegin()
		{
			//Debug.Log ("FixedUpdateBegin");
		}

		/// <summary>
		/// FixedUpdate complete.
		/// </summary>
		public override void FixedUpdateComplete()
		{
			//Debug.Log ("FixedUpdateComplete");

		}
	}
}


