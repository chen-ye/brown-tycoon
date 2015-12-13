using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;


namespace ICE.Creatures.Objects
{

	[System.Serializable]
	public class AudioDataObject
	{
		public AudioDataObject( AudioDataObject _audio )
		{
			Enabled = _audio.Enabled;
			Loop = _audio.Loop;
			MaxDistance = _audio.MaxDistance;
			MinDistance =_audio.MinDistance;
			MaxPitch = _audio.MaxPitch;
			MinPitch = _audio.MinPitch;
			RolloffMode = _audio.RolloffMode;
			Volume = _audio.Volume;

			m_Clips = new List<AudioClip>(); 
			foreach( AudioClip _clip in _audio.Clips )
				m_Clips.Add( _clip );
		}

		public AudioDataObject()
		{
			m_Clips = new List<AudioClip>(); 
		}

		public bool Enabled = false;


		[SerializeField]
		private List<AudioClip> m_Clips = new List<AudioClip>(); 

		//[XmlArray("Clips"),XmlArrayItem("AudioClip")]
		[XmlIgnore]
		public List<AudioClip> Clips{
			set{m_Clips = value;}
			get{return m_Clips;}
		}


		public float Volume = 0.5f;	
		public float MinPitch = 1.0f;	
		public float MaxPitch = 1.5f;	
		public float MinDistance = 2;	
		public float MaxDistance = 7;	
		public bool Loop = false;
		public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;


		private AudioClip m_Selected = null;
		[XmlIgnore]
		public AudioClip Selected{
			get{return m_Selected;}
		}

		public void AddClip( AudioClip _clip )
		{
			if( _clip != null )
				Clips.Add( _clip );
		}

		public void AddClip()
		{
			m_Clips.Add( new AudioClip() );
		}

		public void DeleteClip( int _index )
		{
			if( _index >= 0 && _index < m_Clips.Count )
				m_Clips.RemoveAt( _index );
		}

		public AudioClip GetClip()
		{
			if( m_Clips.Count == 0 )
				return null;

			reroll:	
			AudioClip _clip = m_Clips[Random.Range(0,m_Clips.Count)];
			
			if( _clip != null )
			{
				if ( m_Clips.Count > 1 && _clip == m_Selected )
					goto reroll;

				m_Selected = _clip;
			}

			return m_Selected;
		}
	}
	
	[System.Serializable]
	public class AudioObject : System.Object
	{
		public AudioObject( GameObject gameObject )
		{
			Init( gameObject );
		}

		private GameObject m_Owner = null;

		private AudioSource m_AudioSource = null;
		public AudioSource Source{
			set{ m_AudioSource = value; }
			get{ return m_AudioSource;}
		}

		public AudioClip CurrentClip{
			get{ 
				if( m_AudioSource == null )
					return null;

				return m_AudioSource.clip;
			}
		}

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;
			
			if( m_AudioSource == null )
			{
				m_AudioSource = m_Owner.AddComponent<AudioSource>(); 

				m_AudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
				m_AudioSource.volume = 0.5f;
				m_AudioSource.pitch = 1f;
				m_AudioSource.minDistance = 10f;
				m_AudioSource.maxDistance = 100f;
				m_AudioSource.spatialBlend = 1f;
			}
		}

		public void Play( AudioDataObject _audio )
		{
			if( m_AudioSource == null )
				return;

			if( _audio.Enabled && _audio.GetClip() != null )
			{
				m_AudioSource.clip = _audio.Selected;
				
				m_AudioSource.volume = _audio.Volume;
				m_AudioSource.pitch = Random.Range( _audio.MinPitch, _audio.MaxPitch) * Time.timeScale;
				m_AudioSource.minDistance = _audio.MinDistance;
				m_AudioSource.maxDistance = _audio.MaxDistance;
				m_AudioSource.spatialBlend = 1.0f;							
				m_AudioSource.rolloffMode = _audio.RolloffMode;	
				m_AudioSource.loop = _audio.Loop;				
				m_AudioSource.Play();
			}
			else
			{
				m_AudioSource.Stop();
			}
		}

		/*
		 * 

		private AudioSource m_AudioSource = null;	
		private AudioClip m_AudioClip = null;			
		private AudioClip m_AudioClipLast = null;	
		private void PlaySound( SurfaceDataObject _surface )
		{
			if( _surface == null || _surface.Sounds == null || ! _surface.SoundsEnabled || _surface.Sounds.Count == 0)
				return;
			
		reroll:
				m_AudioClip = _surface.Sounds[Random.Range(0,_surface.Sounds.Count)]; // get a random sound
			
			if( m_AudioClip != null )
			{
				if (  _surface.Sounds.Count > 1 && m_AudioClip == m_AudioClipLast )
					goto reroll;
				
				//m_AudioSource.maxDistance 
				m_AudioSource.pitch = Random.Range(_surface.Audio.MinPitch, _surface.Audio.MaxPitch ) * Time.timeScale;
				m_AudioSource.clip = m_AudioClip;
				m_AudioSource.Play();
				m_AudioClipLast = m_AudioClip;
			}
		}*/

		public void Stop()
		{
			if( m_AudioSource == null )
				return;

			m_AudioSource.Stop();
		}
	}
}
