using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;

namespace ICE.Creatures.Objects
{
	
	[System.Serializable]
	public class SurfaceDataObject : System.Object
	{
		public SurfaceDataObject()
		{
			Audio = new AudioDataObject();
		}

		public bool Foldout = true;
		public bool Enabled = false;
		public float Interval = 1;
		public string Name = ""; 

		//public List<AudioClip> Sounds = new List<AudioClip>(); 
		public List<Texture> Textures = new List<Texture>(); 
		public string BehaviourModeKey = "";

		public EffectContainer Effect;
		public StatusContainer Influences;
		public AudioDataObject Audio;
	}
	
	[System.Serializable]
	public class SurfaceObject : System.Object
	{
		public bool Enabled = false;
		public float Interval = 1;

		private float m_IntervalTimer = 0.0f;
		private string m_TextureName = "";
		
		private Terrain m_SurfaceTerrain;
		private TerrainData m_SurfaceTerrainData;

		private SurfaceDataObject m_ActiveSurface = null;
		public SurfaceDataObject ActiveSurface{
			get{ return m_ActiveSurface; }
		}
		
		public List<SurfaceDataObject> Surfaces = new List<SurfaceDataObject>();

		private GameObject m_Owner = null;
		
		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			Audio.Init( m_Owner );
		}

		private AudioObject m_Audio = null;
		public AudioObject Audio
		{
			get{
				if( m_Audio == null )
					m_Audio = new AudioObject( m_Owner );
				
				return m_Audio;
			}
		}

		public void Update( Vector3 _velocity )
		{
			if( Enabled == false )
				return;

			if( m_SurfaceTerrain == null )
			{
				m_SurfaceTerrain = Terrain.activeTerrain;
				m_SurfaceTerrainData = m_SurfaceTerrain.terrainData;
			}
			
			RaycastHit hit;
			Vector3 _pos = m_Owner.transform.position;
			_pos.y += 0.1f;
			Vector3 _ray = m_Owner.transform.TransformDirection( Vector3.down );
			if ( Physics.Raycast( _pos, _ray, out hit, 10 ) )
			{
				if( hit.transform.GetComponent<Terrain>() != null  )
				{
					int _texture_id = GetMainTerrainTexture( m_Owner.transform.position, hit.transform.GetComponent<Terrain>());
					
					if( m_SurfaceTerrainData.splatPrototypes.Length > 0 )
						m_TextureName = m_SurfaceTerrainData.splatPrototypes[_texture_id].texture.name;
				}
				else if( hit.collider.gameObject.GetComponent<MeshRenderer>() != null )
				{
					MeshRenderer _renderer = hit.collider.gameObject.GetComponent<MeshRenderer>();
					
					if( _renderer.material != null && _renderer.material.mainTexture != null  )
						m_TextureName = _renderer.material.mainTexture.name;
				}
			}
			
			
			HandleEnvironment( _velocity );
		}

		public void HandleEnvironment( Vector3 _velocity )
		{
			if( _velocity.z == 0 )
				return;
				
			if( m_IntervalTimer > 0)
				m_IntervalTimer -= Time.deltaTime;
			
			
			if( m_IntervalTimer <= 0 )
			{
				m_IntervalTimer = Interval / _velocity.z;
				
				SurfaceDataObject _new_surface = null;
				
				foreach( SurfaceDataObject _surface in Surfaces)
				{
					foreach( Texture _texture in _surface.Textures )
					{
						if( _texture != null && _texture.name == m_TextureName )
						{
							_new_surface = _surface;
							break;
						}
					}
				}
				
				if( _new_surface != null )
				{
					m_IntervalTimer = _new_surface.Interval / _velocity.z;
					
					if( m_ActiveSurface != null && m_ActiveSurface != _new_surface )
						m_ActiveSurface.Effect.StopEffect();
					
					if( m_ActiveSurface != _new_surface )
					{
						m_ActiveSurface = _new_surface;
						m_ActiveSurface.Effect.StartEffect( m_Owner );
					}

					Audio.Play( m_ActiveSurface.Audio );
				}
				else
				{
					if( m_ActiveSurface != null )
						m_ActiveSurface.Effect.StopEffect();
					
					m_ActiveSurface = null;

					Audio.Stop();
				}
			}
		}
		

		//********************************************************************************
		// GetMainTerrainTexture
		// Returns the zero-based index of the most dominant texture
		// on the main _terrain at this world position.
		//********************************************************************************
		public static int GetMainTerrainTexture(Vector3 _world_pos, Terrain _terrain )
		{
			TerrainData _terrain_data = _terrain .terrainData;
			Vector3 _terrain_position = _terrain.transform.position;
			
			// calculate which splat map cell the worldPos falls within (ignoring y)
			int _map_x = (int)((( _world_pos.x - _terrain_position.x ) / _terrain_data.size.x ) * _terrain_data.alphamapWidth );
			int _map_z = (int)((( _world_pos.z - _terrain_position.z ) / _terrain_data.size.z ) * _terrain_data.alphamapHeight );
			
			// get the splat map data for this cell as a 1x1xN 3d array (where N = number of textures)
			float[,,] _map_data = _terrain_data.GetAlphamaps(_map_x,_map_z,1,1);			
			
			// extract the 3D array data to a 1D array:
			float[] _mix = new float[_map_data.GetUpperBound(2)+1];
			for( int i=0; i<_mix.Length; ++i )
				_mix[i] = _map_data[0,0,i];    
					
			float _max_mix = 0;
			int _max_index = 0;
			
			// loop through each mix value and find the maximum
			for (int n=0; n<_mix.Length; ++n)
			{
				if( _mix[n] > _max_mix )
				{
					_max_index = n;
					_max_mix = _mix[n];
				}
			}
			
			return _max_index;
			
		}
	}
}