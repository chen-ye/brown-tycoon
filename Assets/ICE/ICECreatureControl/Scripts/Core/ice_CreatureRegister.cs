using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{
	[System.Serializable]
	public struct EnvironmentInfoContainer
	{
		public TemperatureScaleType TemperatureScale;
		public float Temperature;
		public float MinTemperature;
		public float MaxTemperature;

		public int DateDay;
		public int DateMonth;
		public int DateYear;

		public int TimeHour;
		public int TimeMinutes;
		public int TimeSeconds;

		public WeatherType Weather;

		public void UpdateTemperatureScale( TemperatureScaleType _scale )
		{
			if( _scale == TemperatureScaleType.CELSIUS && TemperatureScale == TemperatureScaleType.FAHRENHEIT )
			{
				TemperatureScale = _scale;
				Temperature = ICE.Creatures.Objects.Tools.FahrenheitToCelsius( Temperature );
				MinTemperature = ICE.Creatures.Objects.Tools.FahrenheitToCelsius( MinTemperature );
				MaxTemperature = ICE.Creatures.Objects.Tools.FahrenheitToCelsius( MaxTemperature );
				
			}
			else if( _scale == TemperatureScaleType.FAHRENHEIT && TemperatureScale == TemperatureScaleType.CELSIUS )
			{
				TemperatureScale = _scale;
				Temperature = ICE.Creatures.Objects.Tools.CelsiusToFahrenheit( Temperature );
				MinTemperature = ICE.Creatures.Objects.Tools.CelsiusToFahrenheit( MinTemperature );
				MaxTemperature = ICE.Creatures.Objects.Tools.CelsiusToFahrenheit( MaxTemperature );
			}
		}

	}

	[System.Serializable]
	public struct CreatureStatusContainer
	{
		public bool HasCreatureController;
		public bool HasCreatureAdapter;
		public bool HasHome;
		public bool HasMissionOutpost;
		public bool HasMissionEscort;
		public bool HasMissionPatrol;
		public bool isPrefab;
		public bool isActiveAndEnabled;
		public bool isActiveInHierarchy;
	}

	[System.Serializable]
	public class CreatureReferenceObject
	{
		public CreatureReferenceObject( GameObject _creature )
		{
			Creature = _creature;
			Enabled = true;

			m_Group = CreatureRegister.AddCreatureGroup( Creature.name );
		}

		private CreatureGroupObject m_Group = null;
		public CreatureGroupObject Group{
			get{

				if( m_Group == null )
					m_Group = CreatureRegister.AddCreatureGroup( Creature.name );

				return m_Group;
			
			}
		}
	
		public bool Foldout = true;
		public bool Enabled = false;
		public GameObject Creature = null;

		public CreatureStatusContainer Status;

		public bool PoolManagementEnabled = false;
		public int MaxCreatures = 100;

		public bool m_UseSoftRespawn = true;	
		public bool UseSoftRespawn{
			set{ 
				m_UseSoftRespawn = value; 

				if( Group != null )
					Group.UseSoftRespawn = m_UseSoftRespawn;
			}
			get{ return m_UseSoftRespawn;}
		}

		public float MinRespawnInterval = 10;
		public float MaxRespawnInterval = 60;

		public bool UseRandomScale = true;
		public float UseRandomScaleMultiplier = 0.5f;

		public float RandomScaleMin = -1;
		public float RandomScaleMax = 1;
				
		public bool UseGroupObject = true;	
		private GameObject m_GroupObject = null;
		
		private float m_RespawnTimer = 0;
		private float m_RespawnInterval = 20;
		public float RespawnInterval{
			get{return m_RespawnInterval; }
		}

		private int m_CreaturesCount = 0;
		public int CreaturesCount{
			get{return m_CreaturesCount; }
		}

		public string Name
		{
			get{
				if( Creature != null )
					return Creature.name;
				else
					return null;
			}
		}

		public ICECreatureController Controller
		{
			get{
				if( Creature != null )
					return Creature.GetComponent<ICECreatureController>();
				else
					return null;
			}
		}

		private ICECreatureRegister m_Register = null;
		public ICECreatureRegister Register
		{
			get{
				if( m_Register == null )
					m_Register = GameObject.FindObjectOfType<ICECreatureRegister>();;
		
				return m_Register;
			}
		}

		public Vector3 GetSpawnPosition() 
		{
			Vector3 _position = Vector3.zero;

			if( Controller != null )
			{
				Controller.Creature.Essentials.Target.UpdateOffset();
				_position = Controller.Creature.Essentials.Target.TargetMovePosition;
			}
			else if( Creature != null )
			{
				_position = Tools.GetRandomPosition( Creature.transform.position, 25 );
			}

			_position.y = Tools.GetGroundLevel( _position, ICECreatureRegister.Register.GroundCheck , ICECreatureRegister.Register.GroundLayerMask );

			return _position;
		}

		public void SoftRespawn() 
		{
			if( ! UseSoftRespawn || Group.DeadCreatures.Count == 0 )
				return;

			GameObject _creature = Group.DeadCreatures[0];
			Group.DeadCreatures.RemoveAt(0);

			_creature.transform.position = GetSpawnPosition();
			Quaternion _rotation = Random.rotation;
			_rotation.z = 0;
			_rotation.x = 0;

			_creature.transform.rotation = _rotation;

			if( _creature.GetComponent<ICECreatureControl>() != null )
				_creature.GetComponent<ICECreatureControl>().Creature.Status.Reset();

			_creature.SetActive( true );

		}


		public void Spawn()
		{
			CreatureGroupObject _group = CreatureRegister.ForceCreatureGroup( Name );

			if( _group == null )
				return;

			m_CreaturesCount = _group.Creatures.Count;

			if( m_CreaturesCount < MaxCreatures )
			{

				Vector3 _position = GetSpawnPosition();		
				Quaternion _rotation = Random.rotation;

				_rotation.z = 0;
				_rotation.x = 0;

				GameObject _creature = (GameObject)Object.Instantiate( Creature, _position, _rotation );

				if( UseRandomScale )
				{
					RandomScaleMin = -1;
					RandomScaleMax = 1;
					float _scale_multiplier = (float)Random.Range( RandomScaleMin, RandomScaleMax ) / 100 * UseRandomScaleMultiplier * 0.5f;
				
					_creature.transform.localScale = _creature.transform.localScale + ( _creature.transform.localScale * _scale_multiplier );
				}

				_creature.name = Creature.name;

				if( UseGroupObject )
				{
					if( m_GroupObject == null )
					{

						m_GroupObject = new GameObject();

						m_GroupObject.name = Name + "(Group)";
						m_GroupObject.transform.position = Vector3.zero;

						if( Register != null )
						{
							Register.transform.position = Vector3.zero;
							m_GroupObject.transform.parent = Register.transform;
						}
					}

					_creature.transform.parent = m_GroupObject.transform;

					if( _creature.GetComponent<ICECreatureControl>() != null )
						_creature.GetComponent<ICECreatureControl>().Creature.Status.Reset();
					
					_creature.SetActive( true );

					CreatureRegister.Register( _creature );
				}
			}
		}

		public void Update()
		{
			if( Controller == null )
				return;

			m_RespawnTimer += Time.deltaTime;

			if( m_RespawnTimer >= m_RespawnInterval )
			{
				m_RespawnTimer = 0;
				m_RespawnInterval = Random.Range( MinRespawnInterval, MaxRespawnInterval );

				SoftRespawn();
				Spawn();
			}
		}
	}

	[System.Serializable]
	public class CreatureGroupObject : System.Object
	{
		public CreatureGroupObject( string _name )
		{
			GroupName = _name;

			Creatures = new List<GameObject>();
			DeadCreatures = new List<GameObject>();
		}

		public string GroupName = "";
		 
		public bool UseSoftRespawn = true;
		public List<GameObject> DeadCreatures;
		public List<GameObject> Creatures;

		public bool IsRegistered( GameObject _creature )
		{
			foreach( GameObject _registered_creature in Creatures )
			{
				if( _registered_creature.GetInstanceID() == _creature.GetInstanceID() )
					return true;
			}

			return false;
		}

		public bool Register( GameObject _creature )
		{
			bool _added = false;
			if( ! IsRegistered( _creature ) )
			{
				Creatures.Add( _creature );
				_added = true;
			}
			return _added;
		}

		public bool Deregister( GameObject _creature )
		{
			return Creatures.Remove( _creature );
		}

		public void DestroyCreature( GameObject _creature )
		{
			if( _creature == null )
				return;

			DeadCreatures.Remove( _creature );
			Creatures.Remove( _creature );
			GameObject.Destroy( _creature );
		}

		public void DestroyCreatures()
		{
			for( int i = 0 ; i < Creatures.Count ; i++ )
			{
				DestroyCreature( Creatures[i] );
				--i;	
			}
		}

		public void DeathNotice( GameObject _creature )
		{
			if( _creature == null )
				return;

			if( UseSoftRespawn )
			{
				_creature.SetActive( false );
				DeadCreatures.Add( _creature );
			}
			else
			{
				DestroyCreature( _creature );
			}
		}

		/// <summary>
		/// Finds the random creature.
		/// </summary>
		/// <returns>The random creature.</returns>
		/// <param name="_origin">_origin.</param>
		/// <param name="_dist">_dist.</param>
		public GameObject FindRandomCreature( Vector3 _origin, float _dist  )
		{
			List<GameObject> _creatures = new List<GameObject>();

			GameObject _best_creature = null;

			if( _dist > 0 )
			{
				for( int i = 0; i < Creatures.Count; i++ )
				{
					GameObject _creature = Creatures[i];
					
					if( _creature != null && _creature.activeInHierarchy && _creature.transform.position != _origin )
					{
						float _creature_dist = Tools.GetHorizontalDistance( _origin, _creature.transform.position );
						
						if( _creature_dist <= _dist )
							_creatures.Add( _creature );
					}
				}
			}
			else
				_creatures = Creatures;

			if( _creatures.Count > 0 )
				_best_creature = _creatures[ Random.Range( 0,_creatures.Count-1 ) ]; 
			
			return _best_creature;
		}

		/// <summary>
		/// Finds the nearest creature.
		/// </summary>
		/// <returns>The nearest creature.</returns>
		/// <param name="_origin">_origin.</param>
		/// <param name="_dist">_dist.</param>
		public GameObject FindNearestCreature( Vector3 _origin, float _dist  )
		{

			GameObject _best_creature = null;
			float _best_dist = _dist;
			
			for( int i = 0; i < Creatures.Count; i++ )
			{
				GameObject _creature = Creatures[i];

				if( _creature != null && _creature.activeInHierarchy && _creature.transform.position != _origin )
				{
					float _creature_dist = Tools.GetHorizontalDistance( _origin, _creature.transform.position );

					if( _creature_dist < _best_dist )
					{
						_best_dist = _creature_dist;					
						_best_creature = _creature;
					}
				}
			}

			return _best_creature;
		}
	}


	/// <summary>
	/// Creature Register.
	/// </summary>
	[System.Serializable]
	public static class CreatureRegister : System.Object
	{
		public static EnvironmentInfoContainer EnvironmentInfos;

		private static List<CreatureGroupObject> m_CreatureGroups;

		/// <summary>
		/// Initializes the <see cref="ICE.Creatures.Objects.CreatureRegister"/> class.
		/// </summary>
		static CreatureRegister()
		{
			m_CreatureGroups = new List<CreatureGroupObject>();
		}

		/// <summary>
		/// Register the specified _creature.
		/// </summary>
		/// <param name="_creature">_creature.</param>
		public static void Register( GameObject _creature )
		{
			if( _creature == null )
				return;

			CreatureGroupObject _group = GetCreatureGroup( _creature.name );

			if( _group == null )
				_group = AddCreatureGroup( _creature.name );
	
			if( _group != null )
				_group.Register( _creature );
		}

		public static bool Deregister( GameObject _creature )
		{
			if( _creature == null )
				return false;

			CreatureGroupObject _group = GetCreatureGroup( _creature.name );

			if( _group != null )
				return _group.Deregister( _creature );
			else
				return false;
		}

		public static void DeathNotice( GameObject _creature )
		{
			if( _creature == null )
				return;
			
			CreatureGroupObject _group = GetCreatureGroup( _creature.name );
			
			if( _group != null )
				_group.DeathNotice( _creature );
		}
		/*
		public static bool Destroy( GameObject _creature )
		{
			if( _creature == null )
				return false;
			
			CreatureGroupObject _group = GetCreatureGroup( _creature.name );
			
			if( _group != null )
				return _group.DestroyCreature( _creature );
			else
				return false;
		}*/

		public static void DestroyGroup( string _name )
		{
			CreatureGroupObject _group = GetCreatureGroup( _name );
			
			if( _group != null )
			{
			 	_group.DestroyCreatures();
				m_CreatureGroups.Remove( _group );
			}
	
		}



		/// <summary>
		/// Adds a new creature group.
		/// </summary>
		/// <returns>The creature group.</returns>
		/// <param name="_name">_name.</param>
		public static CreatureGroupObject AddCreatureGroup( string _name )
		{
			_name = CleanName( _name );

			CreatureGroupObject _group = GetCreatureGroup( _name );

			if( _group == null )
			{
				_group = new CreatureGroupObject( CleanName( _name ) );
				
				m_CreatureGroups.Add( _group );
			}

			return _group;
		}

		/// <summary>
		/// Gets a creature group by name.
		/// </summary>
		/// <returns>The creature group.</returns>
		/// <param name="_name">_name.</param>
		public static CreatureGroupObject GetCreatureGroup( string _name )
		{
			_name = CleanName( _name );
			
			foreach( CreatureGroupObject _group in m_CreatureGroups )
			{
				if( _group.GroupName == _name )
					return _group;
			}
			
			return null;
		}

		public static CreatureGroupObject ForceCreatureGroup( string _name )
		{
			_name = CleanName( _name );
			
			foreach( CreatureGroupObject _group in m_CreatureGroups )
			{
				if( _group.GroupName == _name )
					return _group;
			}
			
			return new CreatureGroupObject( _name );;
		}

		public static int GroupCount
		{
			get{ return m_CreatureGroups.Count; }
		}

		public static int TotalCreatureCount
		{
			get{ 

				int _total = 0;
			
				foreach( CreatureGroupObject _group in m_CreatureGroups )
					_total += _group.Creatures.Count;
			
				return _total; 
			}
		}

		public static List<CreatureGroupObject> CreatureGroups
		{
			get{ return m_CreatureGroups; }
		}



		private static string CleanName( string _name )
		{
			if( _name == null || _name == "" )
				return "";

			return _name.Replace("(Clone)", ""); 
		}

		/// <summary>
		/// Finds the random creature.
		/// </summary>
		/// <returns>The random creature.</returns>
		/// <param name="_name">_name.</param>
		/// <param name="_origin">_origin.</param>
		/// <param name="_dist">_dist.</param>
		public static GameObject FindRandomCreature( string _name, Vector3 _origin, float _dist  )
		{
			CreatureGroupObject _group = GetCreatureGroup( _name );
			
			if( _group == null )
				return null;
			
			return _group.FindRandomCreature( _origin, _dist );
		}

		/// <summary>
		/// Finds the nearest creature.
		/// </summary>
		/// <returns>The nearest creature.</returns>
		/// <param name="_name">_name.</param>
		/// <param name="_origin">_origin.</param>
		/// <param name="_dist">_dist.</param>
		public static GameObject FindNearestCreature( string _name, Vector3 _origin, float _dist  )
		{
			CreatureGroupObject _group = GetCreatureGroup( _name );

			if( _group == null )
				return null;

			return _group.FindNearestCreature( _origin, _dist );
		}


	}

}
