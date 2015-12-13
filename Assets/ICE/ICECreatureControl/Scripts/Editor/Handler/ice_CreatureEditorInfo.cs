using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using ICE;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;
using ICE.Styles;
using ICE.Layouts;
using ICE.Creatures.EditorInfos;



namespace ICE.Creatures.EditorHandler
{
	
	public static class EditorInfo
	{	
		public static void Print( ICECreatureControl _control ){

			EditorGUI.indentLevel++;
			if( ! _control.Display.ShowInfo )
				return;

			string _info = "\n";
			_info += "Targets: " + GetTargetsCount( _control ) + " (currently available: " + _control.Creature.AvailableTargets.Count + ")\n";
			_info += "  Active Target: '" + _control.Creature.ActiveTargetName + "' velocity (z): " + _control.Creature.ActiveTargetVelocity + " (previous: '" + _control.Creature.PreviousTargetName + "')\n\n";

			_info += "Behaviours: " + _control.Creature.Behaviour.BehaviourModes.Count + " Modes with " + GetBehaviorModeRulesCount( _control ) + " Rules \n";
			_info += "  Active Mode: '" + _control.Creature.Behaviour.BehaviourModeKey + "' runtime: " + _control.Creature.Behaviour.BehaviourTimer + " secs.\n";
			_info += "  Previous Behaviour: '" + _control.Creature.Behaviour.LastBehaviourModeKey + "'\n\n";

			_info += "Move: " + _control.Creature.Move.CurrentMove.Enabled.ToString().ToUpper() + " type: " + _control.Creature.Move.CurrentMove.Type.ToString() + "\n";
			_info += "  Velocity: " + _control.Creature.Move.CurrentMove.Velocity.Velocity.ToString() + "/" + _control.Creature.Move.CurrentMove.Velocity.AngularVelocity + "\n"; 
			_info += "  Stopping Distance: " + _control.Creature.Move.CurrentMove.MoveStopDistance + " (default: " + _control.Creature.Move.DefaultMove.MoveStopDistance + ")\n";
			_info += "    Ignore Level Difference: " + _control.Creature.Move.CurrentMove.MoveIgnoreLevelDifference.ToString().ToUpper() + " (default: " + _control.Creature.Move.DefaultMove.MoveIgnoreLevelDifference.ToString().ToUpper() + ")\n";
			_info += "  Segment Length: " + _control.Creature.Move.CurrentMove.MoveSegmentLength + " (default: " + _control.Creature.Move.DefaultMove.MoveSegmentLength + ")\n";
			_info += "    Segment Variance: " + _control.Creature.Move.CurrentMove.MoveSegmentVariance + " (default: " + _control.Creature.Move.DefaultMove.MoveSegmentVariance + ")\n";
			_info += "    Lateral Variance: " + _control.Creature.Move.CurrentMove.MoveLateralVariance + " (default: " + _control.Creature.Move.DefaultMove.MoveLateralVariance + ")\n\n";

			if( _control.Creature.Move.UseDeadlockHandling )
			{
				_info += "Deadlocked: " + (_control.Creature.Move.Deadlocked?"TRUE":"FALSE") + " (distance: " + _control.Creature.Move.DeadlocksDistance + " time: " + _control.Creature.Move.DeadlockMoveTimer + "/" + _control.Creature.Move.DeadlockLoopTimer + " secs.)\n";
				_info += "  deadlocks: " + _control.Creature.Move.DeadlocksCount + " - critical positions: " + _control.Creature.Move.DeadlocksCriticalPositions;
				_info += "  loops: " + _control.Creature.Move.DeadlockLoopsCount + " - critical loops: " + _control.Creature.Move.DeadlocksCriticalLoops;
			}
			else
				_info += "Deadlock Handling: deactivated";

			_info += "\n";
			
			//_info += "Active Behaviour Rule : '" + _control.Creature.Behaviour.c + "'";
			Info.Note( _info );

			EditorGUI.indentLevel--;
		}

		private static int GetBehaviorModeRulesCount( ICECreatureControl _control )
		{
			int _i = 0;
			foreach( BehaviourModeObject _mode in _control.Creature.Behaviour.BehaviourModes )
				_i += _mode.Rules.Count;
			return _i;
		}

		private static int GetTargetsCount( ICECreatureControl _control )
		{
			int _i = 0;

			if( _control.Creature.Essentials.TargetReady() )
				_i++;
			if( _control.Creature.Missions.Outpost.TargetReady() )
				_i++;
			if( _control.Creature.Missions.Escort.TargetReady() )
				_i++;
			if( _control.Creature.Missions.Patrol.TargetReady() )
				_i += _control.Creature.Missions.Patrol.Waypoints.GetValidWaypoints().Count;

				_i += _control.Creature.Interaction.GetValidInteractors().Count;

			return _i;
		}
	}
}