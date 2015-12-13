using UnityEngine;
using System.Collections;

namespace ICE.Creatures.EnumTypes
{
	public enum DeadlockActionType
	{
		DIE=0,
		BEHAVIOUR,
		UPDATE
	}

	public enum ViewingDirectionType
	{
		DEFAULT = 0,
		OFFSET,
		MOVE,
		CENTER,
		POSITION
	}

	public enum RandomSeedType
	{
		DEFAULT = 0,
		TIME,
		CUSTOM
	}

	public enum WeatherType
	{
		UNDEFINED = 0,
		FOGGY,
		RAIN,
		HEAVY_RAIN,
		WINDY,
		STORMY,
		CLEAR,
		PARTLY_CLOUDY,
		MOSTLY_CLOUDY,
		CLOUDY
	}

	public enum CollisionConditionType
	{
		TAG,
		LAYER,
		TAG_AND_LAYER
	}

	public enum TemperatureScaleType
	{
		CELSIUS,
		FAHRENHEIT
	}

	public enum DisplayOptionType
	{
		START,
		BASIC,
		FULL,
		MENU
	}

	public enum SequenceOrderType
	{
		CYCLE,
		RANDOM,
		PINGPONG
	}

	public enum RandomOffsetType
	{
		EXACT,
		CIRCLE,
		HEMISPHERE,
		SPHERE
	}

	public enum NavMeshType
	{
		NONE,
		PERMANENT,
		AVOIDANCE,
		COLLISION,
		DEADLOCKED
	}

	public enum GroundCheckType
	{
		NONE,
		RAYCAST,
		SAMPLEHEIGHT
	}

	public enum GroundOrientationType
	{
		NONE,
		BIPED,
		QUADRUPED,
		CRAWLER
	}

	public enum CollisionType
	{
		NONE,
		TERRAIN,
		MESH,
		UNKNOWN
	}

	public enum WaypointOrderType
	{
		PINGPONG,
		CYCLE,
		RANDOM
	}

	public enum MissionType
	{
		HOME,
		ESCORT,
		PATROL
	}


	public enum TargetType
	{
		UNDEFINED,
		HOME,
		OUTPOST,
		ESCORT,
		PATROL,
		WAYPOINT,
		INTERACTOR
	}

	public enum StringOperatorType
	{
		EQUAL,
		NOT
	}
	
	public enum LogicalOperatorType
	{
		EQUAL = 0,
		NOT = 1,
		LESS = 2,
		LESS_OR_EQUAL = 3,
		GREATER = 4,
		GREATER_OR_EQUAL = 5,
	}
	
	public enum ConditionalOperatorType
	{
		AND = 0,
		OR = 1
	}
	
	public enum TargetPrecursorType
	{
		NAME=0,
		TYPE=1,
		TAG=2
	}
	
	public enum TargetSuccessorType
	{
		TYPE=0,
		NAME,
		TAG
	}
	
	
	public enum TargetSelectorExpressionType
	{
		NONE = 0,
		DISTANCE,
		POSITION,
		PRECURSOR,
		BEHAVIOR,
		VISUAL,
		AUDIBLE,
		CONTACTED
	}
	
	public enum TargetSelectorStatementType
	{
		NONE = 0,
		PRIORITY,
		MULTIPLIER,
		SUCCESSOR
	}
	
	
	public enum TargetSelectorPositionType
	{
		TARGETMOVEPOSITION = 0,
		TARGETMAXRANGE = 1
	}


	public enum LinkType
	{
		MODE,
		RULE
	}

	public enum VelocityType
	{
		DEFAULT = 0,
		ADVANCED
	}


	public enum MoveType
	{
		DEFAULT = 0,
		CUSTOM,
		ESCAPE,
		AVOID,
		ORBIT,
		DETOUR,
		RANDOM
	}

	public enum MoveCompleteType
	{
		DEFAULT,
		NEXTRULE,
		CHANGE_MODE,
		FORCE_SENSE,
		FORCE_REACT
	}

	public enum AnimationInterfaceType
	{
		NONE,
		ANIMATOR,
		ANIMATION,
		CLIP
	}

	public enum AnimatorControlType
	{
		DIRECT,
		CONDITIONS,
		ADVANCED
	}

	public enum LabelType {
		Gray = 0,
		Blue,
		Teal,
		Green,
		Yellow,
		Orange,
		Red,
		Purple,
		None
	}
}


