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
	public struct AnimationContainer
	{
		
		public AnimationInterfaceType InterfaceType;
		
		public AnimatorDataContainer Animator;
		public AnimationDataContainer Animation;
		public AnimationClipDataContainer Clip;

		public float GetAnimationLength()
		{
			if( InterfaceType == AnimationInterfaceType.ANIMATION )
				return Animation.Length;
			else if( InterfaceType == AnimationInterfaceType.ANIMATOR )
				return Animator.Length;
			else if( InterfaceType == AnimationInterfaceType.CLIP && Clip.Clip != null )
				return Clip.Clip.length;
			else 
				return 0;
		}

		public void Init()
		{
			Animator.Init();
			Animation.Init();
			Clip.Init();
		}
	}

	[System.Serializable]
	public struct AnimatorDataContainer
	{
		public string Name;
		public int Index;
		public float Length;
		public WrapMode DefaultWrapMode;
		public bool AutoSpeed;
		public float Speed;
		public float TransitionDuration;

		public AnimatorControlType Type;

		public string Integer;
		public int IntegerValue;
		public string Float;
		public float FloatValue;
		public string Trigger;
		public string Boolean;

		public void Init()
		{
			this.AutoSpeed = false;
			this.Speed = 1;
			this.TransitionDuration = 0.5f;
		}
	}
	
	[System.Serializable]
	public struct AnimationDataContainer
	{
		public string Name;
		public int Index;
		public float Length;
		public WrapMode wrapMode;
		public float DefaultSpeed;
		public WrapMode DefaultWrapMode;
		public float Speed;
		public bool AutoSpeed;
		public float TransitionDuration;

		public void Init()
		{
			this.AutoSpeed = false;
			this.Speed = 1;
			this.TransitionDuration = 0.5f;
		}
	}
	
	[System.Serializable]
	public struct AnimationClipDataContainer
	{
		[XmlIgnore]
		public AnimationClip Clip;

		public  float TransitionDuration;

		public void Init()
		{
			//this.AutoSpeed = false;
			//this.Speed = 1;
			this.TransitionDuration = 0.5f;
		}
	}

	public class AnimationObject 
	{
		public AnimationObject( GameObject gameObject )
		{
			Init( gameObject );
		}
		GameObject m_Owner = null;
		Animation m_Animation = null;
		Animator m_Animator = null;

		public void Init( GameObject gameObject )
		{
			m_Owner = gameObject;

			m_Animation = m_Owner.GetComponent<Animation>();
			m_Animator = m_Owner.GetComponent<Animator>();
		}

		//private bool m_AnimatorAutoSpeed = false;
		//private bool m_AnimationAutoSpeed = false;

		private string m_animator_last_boolean = "";

		public void Play( BehaviourModeRuleObject _rule )
		{
			if( _rule == null || _rule.Animation.InterfaceType == AnimationInterfaceType.NONE )
				return;

			if( m_Animator != null && m_animator_last_boolean != "" )
				m_Animator.SetBool( m_animator_last_boolean, false );

			//m_AnimatorAutoSpeed = false;

			if( _rule.Animation.InterfaceType == AnimationInterfaceType.ANIMATION )
			{
				if ( m_Animation == null ) 
				{	
					Debug.LogError( "CAUTION : Missing Animation Component on " + m_Owner.gameObject.name );
					return;
				}

				WrapMode _mode = _rule.Animation.Animation.wrapMode;

				m_Animation[ _rule.Animation.Animation.Name ].wrapMode = _mode;
				//animation[ m_BehaviourData.AnimationName ].speed = Mathf.Clamp( m_BehaviourData.MoveVelocity. controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
				m_Animation.CrossFade( _rule.Animation.Animation.Name, _rule.Animation.Animation.TransitionDuration );	

			}
			else if( _rule.Animation.InterfaceType == AnimationInterfaceType.CLIP )
			{
				if ( m_Animation == null ) 
					m_Animation = m_Owner.AddComponent<Animation>();

				if ( m_Animation != null && _rule.Animation.Clip.Clip != null ) 
				{
					m_Animation.AddClip( _rule.Animation.Clip.Clip, _rule.Animation.Clip.Clip.name );
					m_Animation.CrossFade( _rule.Animation.Clip.Clip.name, _rule.Animation.Clip.TransitionDuration );	
				}

			}
			else if( _rule.Animation.InterfaceType == AnimationInterfaceType.ANIMATOR )
			{
				if ( m_Animator == null ) 
				{	
					Debug.LogError( "Missing Animator Component!" );
					return;
				}
			
				if( _rule.Animation.Animator.Type == AnimatorControlType.DIRECT )
				{
			
					m_Animator.CrossFade( _rule.Animation.Animator.Name, _rule.Animation.Animator.TransitionDuration, -1, 0); 
					m_Animator.speed = _rule.Animation.Animator.Speed;
					//m_AnimatorAutoSpeed = _rule.Animation.Animator.AutoSpeed;
					/*if( _rule.Animation.Animator.AutoSpeed )
						m_Animator.speed = Mathf.Clamp( m_BehaviourData.MoveVelocity. controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);
					else*/

				}
				else if( _rule.Animation.Animator.Type == AnimatorControlType.CONDITIONS )
				{
					if( _rule.Animation.Animator.Boolean != "-" )
					{
						m_Animator.SetBool( _rule.Animation.Animator.Boolean, true );
						m_animator_last_boolean = _rule.Animation.Animator.Boolean;
					}
					
					if( _rule.Animation.Animator.Trigger != "-" )
						m_Animator.SetTrigger( _rule.Animation.Animator.Trigger );
				}
				else if( _rule.Animation.Animator.Type == AnimatorControlType.ADVANCED )
				{
				}


			}
		}
				
	

		//--------------------------------------------------
		// Update
		//--------------------------------------------------
		public void Update()
		{/*
			if( m_AnimatorAutoSpeed && m_Animator != null )
				m_Animator.speed = Mathf.Clamp( m_BehaviourData.MoveVelocity. controller.velocity.magnitude, 0.0, runMaxAnimationSpeed);*/
		}
	}

}
