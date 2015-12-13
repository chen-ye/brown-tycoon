using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;

namespace ICE.Creatures{

	public class ICECreatureRegister : MonoBehaviour 
	{
		public bool UsePoolManagenent = false;
		public bool UseDontDestroyOnLoad = false;
		public GroundCheckType GroundCheck = GroundCheckType.NONE;
		public LayerMask GroundLayerMask;
		public RandomSeedType RandomSeed = RandomSeedType.DEFAULT;
		public int CustomRandomSeed = 23;

		public bool UseEnvironmentManagenent = false;
		public EnvironmentInfoContainer EnvironmentInfos;

		//Here is a private reference only this class can access
		private static ICECreatureRegister m_Register;
		
		//This is the public reference that other classes will use
		public static ICECreatureRegister Register
		{
			get
			{
				//If m_Register hasn't been set yet, we grab it from the scene!
				//This will only happen the first time this reference is used.
				if(m_Register == null)
				{
					m_Register = GameObject.FindObjectOfType<ICECreatureRegister>();
				}

				return m_Register;
			}
		}

		public List<CreatureReferenceObject> ReferenceCreatures = new List<CreatureReferenceObject>();

		void Awake () 
		{
			transform.gameObject.isStatic = true;

			if( UseDontDestroyOnLoad ) 
				DontDestroyOnLoad(transform.gameObject);
	
			if( RandomSeed == RandomSeedType.CUSTOM )
				Random.seed = CustomRandomSeed;
			else if( RandomSeed == RandomSeedType.TIME )
				Random.seed = (int)System.DateTime.Now.Second;
		}

		void Update () 
		{
			if( UseEnvironmentManagenent )
			{
				CreatureRegister.EnvironmentInfos = EnvironmentInfos;
			}

			foreach( CreatureReferenceObject _creature in ReferenceCreatures )
			{
				if( _creature.PoolManagementEnabled )
					_creature.Update();
			}
		}

		public void Scan()
		{
			ICECreatureControl[] _creatures = FindObjectsOfType<ICECreatureControl>();

			foreach( ICECreatureControl _creature in _creatures )
			{
				if( _creature != null )
					AddReferenceCreature( _creature.gameObject );
			}

			ICECreatureResident[] _residents = FindObjectsOfType<ICECreatureResident>();
			
			foreach( ICECreatureResident _resident in _residents )
			{
				if( _resident != null )
					AddReferenceCreature( _resident.gameObject );
			}
		}

		/// <summary>
		/// Registers the reference object.
		/// </summary>
		/// <returns><c>true</c>, if reference object was registered, <c>false</c> otherwise.</returns>
		/// <param name="_object">_object.</param>
		public bool AddReferenceCreature( GameObject _object )
		{
			if( _object == null )
				return false;

			if( ! IsRegistered( _object ) )
			{
				ReferenceCreatures.Add( new CreatureReferenceObject( _object ) );

				return true;
			}
			else
			{
				return false;
			}
		}

	/// <summary>
	/// Determines whether this instance is registered the specified _object.
	/// </summary>
	/// <returns><c>true</c> if this instance is registered the specified _object; otherwise, <c>false</c>.</returns>
	/// <param name="_object">_object.</param>
		public bool IsRegistered( GameObject _object )
		{
			if( _object == null )
				return false;

			return IsRegistered( _object.name );
		}

		/// <summary>
		/// Determines whether this instance is registered the specified _object_name.
		/// </summary>
		/// <returns><c>true</c> if this instance is registered the specified _object_name; otherwise, <c>false</c>.</returns>
		/// <param name="_object_name">_object_name.</param>
		public bool IsRegistered( string _object_name )
		{
			if( _object_name.Length == 0 )
				return false;

			bool _registered = false;

			foreach( CreatureReferenceObject _item in ReferenceCreatures )
			{
				if( _item.Creature && _item.Creature.name == _object_name )
					_registered = true;
			}

			return _registered;
		}

		public GameObject GetReferenceCreatureByName( string _object_name )
		{
			GameObject _creature = null;
			
			foreach( CreatureReferenceObject _item in ReferenceCreatures )
			{
				if( _item.Creature != null )
				{
					if( _object_name.Length == 0 || _item.Creature.name == _object_name )
						_creature = _item.Creature;
				}
				
			}
			
			return _creature;
		}

		public List<GameObject> GetCreaturesByName( string _name )
		{
			CreatureGroupObject _group = CreatureRegister.GetCreatureGroup( _name );
			
			if( _group != null && _group.Creatures.Count > 0 )
				return _group.Creatures;
			else
			{
				List<GameObject> _creatures = new List<GameObject>();

				foreach( CreatureReferenceObject _item in ReferenceCreatures )
				{
					if( _item.Creature != null )
					{
						if( _name.Length == 0 || _item.Creature.name == _name )
							_creatures.Add( _item.Creature );
					}
				}

				return _creatures;
			}
		}

		public void UpdateWeather( WeatherType _weather )
		{
			CreatureRegister.EnvironmentInfos.Weather = _weather;
		}

		public void UpdateTemperature( float _temperature )
		{
			CreatureRegister.EnvironmentInfos.Temperature = _temperature;
		}

		public void UpdateTime( int _hour, int _minutes, int _seconds )
		{
			CreatureRegister.EnvironmentInfos.TimeHour = _hour;
			CreatureRegister.EnvironmentInfos.TimeMinutes = _minutes;
			CreatureRegister.EnvironmentInfos.TimeSeconds = _seconds;
		}

		public void UpdateDate( int _day, int _month, int _year )
		{
			CreatureRegister.EnvironmentInfos.DateDay = _day;
			CreatureRegister.EnvironmentInfos.DateMonth = _month;
			CreatureRegister.EnvironmentInfos.DateYear = _year;
		}
	}

}
