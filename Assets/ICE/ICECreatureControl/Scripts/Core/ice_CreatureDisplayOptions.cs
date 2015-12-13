using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ICE.Creatures;
using ICE.Creatures.EnumTypes;
using ICE.Creatures.Objects;


namespace ICE.Creatures.Objects
{
	public static class GlobalDisplayData
	{
		public static bool UseGlobalAll = false;

		public static bool ShowHelp = false;
		public static bool ShowHelpDescription = false;
		public static bool ShowHelpTarget = false;
		public static bool ShowHelpBehaviour = false;
		public static bool ShowDebug = false;
		public static bool ShowInfo = false;

		public static bool ShowEssentials = false;

		public static bool ShowStatus = false;
		public static bool ShowStatusAdvanced = false;

		public static bool ShowMissions = false;
		public static bool ShowMissionsHome = false;
		public static bool ShowMissionsEscort = false;
		public static bool ShowMissionsPatrol = false;
		
		public static bool ShowEnvironmentSettings = false;
		public static bool ShowInteractionSettings = false;

		public static bool ShowBehaviour = false;
		public static bool ShowBehaviourMove = false;
		public static bool ShowBehaviourAudio = false;
		public static bool ShowBehaviourInluences = false;
		public static bool ShowBehaviourEffect = false;
		public static bool ShowBehaviourLink = false;
		
		public static bool ShowBeta = false;
		
		public static DisplayOptionType DisplayOptions = DisplayOptionType.START;

		public static bool FoldoutEssentials = true;
		public static bool FoldoutBehaviours = true;
		public static bool FoldoutMissions = true;
		public static bool FoldoutMissionOutpost = true;
		public static bool FoldoutMissionEscort = true;
		public static bool FoldoutMissionPatrol = true;
		public static bool FoldoutStatus = true;
		public static bool FoldoutInteraction = true;
		public static bool FoldoutEnvironment = true;
	}


	[System.Serializable]
	public class DisplayData
	{
		[SerializeField]
		private bool m_UseGlobal = false;
		public bool UseGlobal
		{
			set{ 
				if( ! GlobalDisplayData.UseGlobalAll ) 
					m_UseGlobal = value;
			}
			get{ 
				if( GlobalDisplayData.UseGlobalAll ) 
					return GlobalDisplayData.UseGlobalAll; 
				else 
					return m_UseGlobal; 
			}
		}

		[SerializeField]
		//private bool m_UseGlobalAll = false;
		public bool UseGlobalAll{
			set{ GlobalDisplayData.UseGlobalAll = value; }
			get{ return GlobalDisplayData.UseGlobalAll; }
		}

		public void SetLocalToGlobal()
		{
			GlobalDisplayData.ShowHelp = m_ShowHelp;
			GlobalDisplayData.ShowInfo = m_ShowInfo;
			GlobalDisplayData.ShowHelpDescription = m_ShowHelpDescription;
			GlobalDisplayData.ShowHelpTarget = m_ShowHelpTarget;
			GlobalDisplayData.ShowHelpBehaviour = m_ShowHelpBehaviour;
			GlobalDisplayData.ShowDebug = m_ShowDebug;

			GlobalDisplayData.ShowEssentials = m_ShowEssentials;
			GlobalDisplayData.ShowStatus = m_ShowStatus;
			GlobalDisplayData.ShowStatusAdvanced = m_ShowStatusAdvanced;

			GlobalDisplayData.ShowEnvironmentSettings = m_ShowEnvironmentSettings;
			
			GlobalDisplayData.ShowMissions = m_ShowMissions;
			GlobalDisplayData.ShowMissionsHome = m_ShowMissionsHome;
			GlobalDisplayData.ShowMissionsEscort = m_ShowMissionsEscort;
			GlobalDisplayData.ShowMissionsPatrol = m_ShowMissionsPatrol;
			
			GlobalDisplayData.ShowInteractionSettings = m_ShowInteractionSettings;

			GlobalDisplayData.ShowBehaviour = m_ShowBehaviour;
			GlobalDisplayData.ShowBehaviourMove = m_ShowBehaviourMove;
			GlobalDisplayData.ShowBehaviourAudio = m_ShowBehaviourAudio;
			GlobalDisplayData.ShowBehaviourInluences = m_ShowBehaviourInluences;
			GlobalDisplayData.ShowBehaviourEffect = m_ShowBehaviourEffect;
			GlobalDisplayData.ShowBehaviourLink = m_ShowBehaviourLink;
			
			GlobalDisplayData.ShowBeta = m_ShowBeta;
			
			GlobalDisplayData.DisplayOptions = m_DisplayOptions;

			GlobalDisplayData.FoldoutEssentials = m_FoldoutEssentials;
			GlobalDisplayData.FoldoutBehaviours = m_FoldoutBehaviours;
			GlobalDisplayData.FoldoutStatus = m_FoldoutStatus;
			GlobalDisplayData.FoldoutInteraction = m_FoldoutInteraction;
			GlobalDisplayData.FoldoutEssentials = m_FoldoutEssentials;
			GlobalDisplayData.FoldoutMissions = m_FoldoutMissions;
			GlobalDisplayData.FoldoutMissionOutpost = m_FoldoutMissionOutpost;
			GlobalDisplayData.FoldoutMissionEscort = m_FoldoutMissionEscort;
			GlobalDisplayData.FoldoutMissionPatrol = m_FoldoutMissionPatrol;
		}

		public void SetGlobalToLocal()
		{
			m_ShowHelp = GlobalDisplayData.ShowHelp;
			m_ShowHelpDescription = GlobalDisplayData.ShowHelpDescription;
			m_ShowInfo = GlobalDisplayData.ShowInfo;
			m_ShowDebug = GlobalDisplayData.ShowDebug;

			m_ShowEssentials = GlobalDisplayData.ShowEssentials;
			m_ShowStatus = GlobalDisplayData.ShowStatus;
			m_ShowStatusAdvanced = GlobalDisplayData.ShowStatusAdvanced;

			m_ShowEnvironmentSettings = GlobalDisplayData.ShowEnvironmentSettings;
			
			m_ShowMissions = GlobalDisplayData.ShowMissions;
			m_ShowMissionsHome = GlobalDisplayData.ShowMissionsHome;
			m_ShowMissionsEscort = GlobalDisplayData.ShowMissionsEscort;
			m_ShowMissionsPatrol = GlobalDisplayData.ShowMissionsPatrol;
			
			m_ShowInteractionSettings = GlobalDisplayData.ShowInteractionSettings;

			m_ShowBehaviour = GlobalDisplayData.ShowBehaviour;
			m_ShowBehaviourMove = GlobalDisplayData.ShowBehaviourMove;
			m_ShowBehaviourAudio = GlobalDisplayData.ShowBehaviourAudio;
			m_ShowBehaviourInluences = GlobalDisplayData.ShowBehaviourInluences;
			m_ShowBehaviourEffect = GlobalDisplayData.ShowBehaviourEffect;
			m_ShowBehaviourLink = GlobalDisplayData.ShowBehaviourLink;

			m_ShowBeta = GlobalDisplayData.ShowBeta;
			m_DisplayOptions = GlobalDisplayData.DisplayOptions;

			m_FoldoutEssentials = GlobalDisplayData.FoldoutEssentials;
			m_FoldoutBehaviours = GlobalDisplayData.FoldoutBehaviours;
			m_FoldoutStatus = GlobalDisplayData.FoldoutStatus;
			m_FoldoutInteraction = GlobalDisplayData.FoldoutInteraction;
			m_FoldoutEnvironment = GlobalDisplayData.FoldoutEnvironment;
			m_FoldoutMissions = GlobalDisplayData.FoldoutMissions;
			m_FoldoutMissionOutpost = GlobalDisplayData.FoldoutMissionOutpost;
			m_FoldoutMissionEscort = GlobalDisplayData.FoldoutMissionEscort;
			m_FoldoutMissionPatrol = GlobalDisplayData.FoldoutMissionPatrol;
		}


		[SerializeField]
		private bool m_ShowHelp = false;
		public bool ShowHelp{
			set{ if( UseGlobal ) GlobalDisplayData.ShowHelp = value; else m_ShowHelp = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowHelp; else return m_ShowHelp; }
		}

		[SerializeField]
		private bool m_ShowHelpDescription = false;
		public bool ShowHelpDescription{
			set{ if( UseGlobal ) GlobalDisplayData.ShowHelpDescription = value; else m_ShowHelpDescription = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowHelpDescription; else return m_ShowHelpDescription; }
		}

		[SerializeField]
		private bool m_ShowHelpTarget = false;
		public bool ShowHelpTarget{
			set{ if( UseGlobal ) GlobalDisplayData.ShowHelpTarget = value; else m_ShowHelpTarget = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowHelpTarget; else return m_ShowHelpTarget; }
		}

		[SerializeField]
		private bool m_ShowHelpBehaviour = false;
		public bool ShowHelpBehaviour{
			set{ if( UseGlobal ) GlobalDisplayData.ShowHelpBehaviour = value; else m_ShowHelpBehaviour = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowHelpBehaviour; else return m_ShowHelpBehaviour; }
		}

		
		[SerializeField]
		private bool m_ShowDebug = false;
		public bool ShowDebug {
			set{ if( UseGlobal ) GlobalDisplayData.ShowDebug = value; else m_ShowDebug = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowDebug; else return m_ShowDebug; }
		}

		[SerializeField]
		private bool m_ShowInfo = false;
		public bool ShowInfo {
			set{ if( UseGlobal ) GlobalDisplayData.ShowInfo = value; else m_ShowInfo = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowInfo; else return m_ShowInfo; }
		}

		[SerializeField]
		private bool m_ShowEssentials = false;
		public bool ShowEssentials{
			set{ if( UseGlobal ) GlobalDisplayData.ShowEssentials = value; else m_ShowEssentials = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowEssentials; else return m_ShowEssentials; }
		}


		[SerializeField]
		private bool m_ShowEnvironmentSettings = false;
		public bool ShowEnvironmentSettings {
			set{ if( UseGlobal ) GlobalDisplayData.ShowEnvironmentSettings  = value; else m_ShowEnvironmentSettings  = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowEnvironmentSettings ; else return m_ShowEnvironmentSettings ; }
		}



		[SerializeField]
		private bool m_ShowStatus = false;
		public bool ShowStatus{
			set{ if( UseGlobal ) GlobalDisplayData.ShowStatus = value; else m_ShowStatus = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowStatus; else return m_ShowStatus; }
		}

		[SerializeField]
		private bool m_ShowStatusAdvanced = false;
		public bool ShowStatusAdvanced{
			set{ if( UseGlobal ) GlobalDisplayData.ShowStatusAdvanced = value; else m_ShowStatusAdvanced = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowStatusAdvanced; else return m_ShowStatusAdvanced; }
		}

		[SerializeField]
		private bool m_ShowMissions = false;
		public bool ShowMissions{
			set{ if( UseGlobal ) GlobalDisplayData.ShowMissions = value; else m_ShowMissions = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowMissions; else return m_ShowMissions; }
		}

		[SerializeField]
		private bool m_ShowMissionsHome = false;
		public bool ShowMissionsHome{
			set{ if( UseGlobal ) GlobalDisplayData.ShowMissionsHome = value; else m_ShowMissionsHome = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowMissionsHome; else return m_ShowMissionsHome; }
		}

		[SerializeField]
		private bool m_ShowMissionsEscort = false;
		public bool ShowMissionsEscort{
			set{ if( UseGlobal ) GlobalDisplayData.ShowMissionsEscort = value; else m_ShowMissionsEscort = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowMissionsEscort; else return m_ShowMissionsEscort; }
		}

		[SerializeField]
		private bool m_ShowMissionsPatrol = false;
		public bool ShowMissionsPatrol{
			set{ if( UseGlobal ) GlobalDisplayData.ShowMissionsPatrol = value; else m_ShowMissionsPatrol = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowMissionsPatrol; else return m_ShowMissionsPatrol; }
		}

		[SerializeField]
		private bool m_ShowInteractionSettings = false;
		public bool ShowInteractionSettings{
			set{ if( UseGlobal ) GlobalDisplayData.ShowInteractionSettings = value; else m_ShowInteractionSettings = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowInteractionSettings; else return m_ShowInteractionSettings; }
		}

		[SerializeField]
		private bool m_ShowBehaviour = false;
		public bool ShowBehaviour{
			set{ if( UseGlobal ) GlobalDisplayData.ShowBehaviour = value; else m_ShowBehaviour = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowBehaviour; else return m_ShowBehaviour; }
		}

		[SerializeField]
		private bool m_ShowBehaviourMove = false;
		public bool ShowBehaviourMove{
			set{ if( UseGlobal ) GlobalDisplayData.ShowBehaviourMove = value; else m_ShowBehaviourMove = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowBehaviourMove; else return m_ShowBehaviourMove; }
		}

		[SerializeField]
		private bool m_ShowBehaviourAudio = false;
		public bool ShowBehaviourAudio{
			set{ if( UseGlobal ) GlobalDisplayData.ShowBehaviourAudio = value; else m_ShowBehaviourAudio = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowBehaviourAudio; else return m_ShowBehaviourAudio; }
		}

		[SerializeField]
		private bool m_ShowBehaviourInluences = false;
		public bool ShowBehaviourInluences{
			set{ if( UseGlobal ) GlobalDisplayData.ShowBehaviourInluences = value; else m_ShowBehaviourInluences = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowBehaviourInluences; else return m_ShowBehaviourInluences; }
		}

		[SerializeField]
		private bool m_ShowBehaviourEffect = false;
		public bool ShowBehaviourEffect{
			set{ if( UseGlobal ) GlobalDisplayData.ShowBehaviourEffect = value; else m_ShowBehaviourEffect = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowBehaviourEffect; else return m_ShowBehaviourEffect; }
		}

		[SerializeField]
		private bool m_ShowBehaviourLink = false;
		public bool ShowBehaviourLink{
			set{ if( UseGlobal ) GlobalDisplayData.ShowBehaviourLink = value; else m_ShowBehaviourLink = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowBehaviourLink; else return m_ShowBehaviourLink; }
		}

		[SerializeField]
		private bool m_ShowBeta = false;
		public bool ShowBeta{
			set{ if( UseGlobal ) GlobalDisplayData.ShowBeta = value; else m_ShowBeta = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.ShowBeta; else return m_ShowBeta; }
		}

		[SerializeField]
		private DisplayOptionType m_DisplayOptions = DisplayOptionType.START;
		public DisplayOptionType DisplayOptions{
			set{ if( UseGlobal ) GlobalDisplayData.DisplayOptions = value; else m_DisplayOptions = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.DisplayOptions; else return m_DisplayOptions; }
		}

		/// <summary>
		/// FOLDOUTS
		/// </summary>

		[SerializeField]
		private bool m_FoldoutEssentials = true;
		public bool FoldoutEssentials{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutEssentials = value; else m_FoldoutEssentials = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutEssentials; else return m_FoldoutEssentials; }
		}

		[SerializeField]
		private bool m_FoldoutBehaviours = true;
		public bool FoldoutBehaviours{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutBehaviours = value; else m_FoldoutBehaviours = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutBehaviours; else return m_FoldoutBehaviours; }
		}

		[SerializeField]
		private bool m_FoldoutMissions = true;
		public bool FoldoutMissions{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutMissions = value; else m_FoldoutMissions = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutMissions; else return m_FoldoutMissions; }
		}

		[SerializeField]
		private bool m_FoldoutMissionOutpost = true;
		public bool FoldoutMissionOutpost{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutMissionOutpost = value; else m_FoldoutMissionOutpost = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutMissionOutpost; else return m_FoldoutMissionOutpost; }
		}

		[SerializeField]
		private bool m_FoldoutMissionEscort = true;
		public bool FoldoutMissionEscort{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutMissionEscort = value; else m_FoldoutMissionEscort = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutMissionEscort; else return m_FoldoutMissionEscort; }
		}

		[SerializeField]
		private bool m_FoldoutMissionPatrol = true;
		public bool FoldoutMissionPatrol{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutMissionPatrol = value; else m_FoldoutMissionPatrol = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutMissionPatrol; else return m_FoldoutMissionPatrol; }
		}

		[SerializeField]
		private bool m_FoldoutStatus = true;
		public bool FoldoutStatus{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutStatus = value; else m_FoldoutStatus = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutStatus; else return m_FoldoutStatus; }
		}

		[SerializeField]
		private bool m_FoldoutInteraction = true;
		public bool FoldoutInteraction{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutInteraction = value; else m_FoldoutInteraction = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutInteraction; else return m_FoldoutInteraction; }
		}

		[SerializeField]
		private bool m_FoldoutEnvironment = true;
		public bool FoldoutEnvironment{
			set{ if( UseGlobal ) GlobalDisplayData.FoldoutEnvironment = value; else m_FoldoutEnvironment = value; }
			get{ if( UseGlobal ) return GlobalDisplayData.FoldoutEnvironment; else return m_FoldoutEnvironment; }
		}

	}
}
