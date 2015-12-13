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

namespace ICE.Creatures
{

	[CustomEditor(typeof(ICECreatureRegister))]
	public class ICECreatureRegisterEditor : Editor {
		

		public ICECreatureRegister m_creature_register;

		public bool m_foldout_register = true;
		public bool m_foldout_options = true;
		public bool m_foldout_environment = true;
	
		public virtual void OnEnable()
		{
			m_creature_register = (ICECreatureRegister)target;
		}


		public override void OnInspectorGUI()
		{
			GUI.changed = false;

			EditorGUILayout.Separator();

			EditorGUI.indentLevel++;

				m_foldout_options =  EditorGUILayout.Foldout( m_foldout_options, "Options", ICEEditorStyle.Foldout );
				if( m_foldout_options )
				{

					EditorGUI.indentLevel++;
						m_creature_register.UsePoolManagenent = ICEEditorLayout.Toggle( "Use Pool Managenent", "", m_creature_register.UsePoolManagenent );

						EditorGUI.BeginDisabledGroup( m_creature_register.UsePoolManagenent == false );
						m_creature_register.GroundCheck = (GroundCheckType)ICEEditorLayout.EnumPopup( "Spawn Ground Check","", m_creature_register.GroundCheck );
							
							if( m_creature_register.GroundCheck == GroundCheckType.RAYCAST )
							{
								EditorGUI.indentLevel++;
								m_creature_register.GroundLayerMask = EditorGUILayout.LayerField( "Ground Layer", m_creature_register.GroundLayerMask );
								EditorGUI.indentLevel--;
							}
						EditorGUI.EndDisabledGroup();
						EditorGUILayout.Separator();

					m_creature_register.RandomSeed = (RandomSeedType)ICEEditorLayout.EnumPopup( "Random Seed","", m_creature_register.RandomSeed );
					if( m_creature_register.RandomSeed == RandomSeedType.CUSTOM )
					{
						EditorGUI.indentLevel++;
							m_creature_register.CustomRandomSeed = ICEEditorLayout.IntField( "Seed Value","Custom RandomSeed Integer Value", m_creature_register.CustomRandomSeed  );
						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}

					m_creature_register.UseEnvironmentManagenent = ICEEditorLayout.Toggle( "Handle Environment", "", m_creature_register.UseEnvironmentManagenent );
					
					if( m_creature_register.UseEnvironmentManagenent )
					{
						EditorGUI.indentLevel++;
						
						
						m_creature_register.EnvironmentInfos.UpdateTemperatureScale( (TemperatureScaleType)ICEEditorLayout.EnumPopup( "Temperature Scale","", m_creature_register.EnvironmentInfos.TemperatureScale ) );
						
						EditorGUI.indentLevel++;
							m_creature_register.EnvironmentInfos.MinTemperature = EditorGUILayout.FloatField( "Min. Temperature", m_creature_register.EnvironmentInfos.MinTemperature );
							m_creature_register.EnvironmentInfos.MaxTemperature = EditorGUILayout.FloatField( "Max. Temperature", m_creature_register.EnvironmentInfos.MaxTemperature );
						EditorGUI.indentLevel--;
						
						m_creature_register.EnvironmentInfos.Temperature = ICEEditorLayout.Slider( "Temperature","", m_creature_register.EnvironmentInfos.Temperature, 1,m_creature_register.EnvironmentInfos.MinTemperature,m_creature_register.EnvironmentInfos.MaxTemperature );

						
						EditorGUI.indentLevel--;
						EditorGUILayout.Separator();
					}

					m_creature_register.UseDontDestroyOnLoad = ICEEditorLayout.Toggle( "Dont Destroy On Load", "", m_creature_register.UseDontDestroyOnLoad );

					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}

				ICEEditorLayout.BeginHorizontal();
				m_foldout_register =  EditorGUILayout.Foldout( m_foldout_register, "Reference Creatures", ICEEditorStyle.Foldout );
				if( m_foldout_register )
				{
					if (GUILayout.Button("SCAN", ICEEditorStyle.ButtonMiddle ))
					{
						m_creature_register.Scan();
					}
				}

				ICEEditorLayout.EndHorizontal();

				if( m_foldout_register )
				{

					EditorGUILayout.Separator();

					for( int i = 0 ; i < m_creature_register.ReferenceCreatures.Count ; i++ )
					{
						CreatureReferenceObject _obj = m_creature_register.ReferenceCreatures[i];

						UpdateCreatureStatus( _obj );
			
						if( _obj != null && _obj.Creature != null )
						{
							string _title = _obj.Name;
							if( m_creature_register.UsePoolManagenent )
							{
								ICEEditorLayout.Label( _title , true );
								_title = "Reference Object";
								EditorGUI.indentLevel++;
								EditorGUILayout.Separator();
							}

							ICEEditorLayout.BeginHorizontal();

								_obj.Creature = (GameObject)EditorGUILayout.ObjectField( _title , _obj.Creature, typeof(GameObject), true );

								if(  _obj.Controller != null )
								{
									if (GUILayout.Button("SELECT", ICEEditorStyle.ButtonMiddle ))
										Selection.activeGameObject = _obj.Creature;
								}
								else
								{
									if (GUILayout.Button("ADD CC", ICEEditorStyle.ButtonMiddle ))
										_obj.Creature.AddComponent<ICECreatureControl>();
								}

								if( m_creature_register.UsePoolManagenent )
								{
									if( _obj.PoolManagementEnabled )
										GUI.backgroundColor = Color.yellow;

									if (GUILayout.Button("POOL", ICEEditorStyle.ButtonMiddle ))
										_obj.PoolManagementEnabled = ! _obj.PoolManagementEnabled;

									GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;
								}

								if (GUILayout.Button("x", ICEEditorStyle.CMDButton ))
								{
									m_creature_register.ReferenceCreatures.Remove( _obj );
									i--;
								}
							ICEEditorLayout.EndHorizontal();


							string[] _flags = new string[10];

							// CC controlled
							if( _obj.Status.HasCreatureController )
								_flags[0] = "icons/cc_1";
							else if( _obj.Status.HasCreatureAdapter )
								_flags[0] = "icons/failed";
							else
								_flags[0] = "icons/failed";

							if( _obj.Status.HasHome )
								_flags[1] = "icons/home_ready";
							else
								_flags[1] = "icons/home_failed";

							if( _obj.Status.HasMissionOutpost )
								_flags[2] = "icons/cc_1";
							else
								_flags[2] = "icons/failed";

							if( _obj.Status.HasMissionEscort ) 
								_flags[3] = "icons/cc_1";
							else
								_flags[3] = "icons/failed";

							if( _obj.Status.HasMissionPatrol ) 
								_flags[4] = "icons/cc_1";
							else
								_flags[4] = "icons/failed";

							if( _obj.Status.isActiveAndEnabled ) 
								_flags[5] = "icons/cc_1";
							else
								_flags[5] = "icons/failed";

							if( _obj.Status.isActiveInHierarchy ) 
								_flags[6] = "icons/cc_1";
							else
								_flags[6] = "icons/failed";

							if( _obj.Status.isPrefab ) 
								_flags[7] = "icons/cc_1";
							else
								_flags[7] = "icons/failed";

							if( m_creature_register.UsePoolManagenent )
							{
								//EditorGUILayout.Separator();
								//ICEEditorLayout.DrawLabelIconBar( "Status", _flags, 16, 16, 0,0,5);

								//EditorGUILayout.Separator();
								
						
								EditorGUI.BeginDisabledGroup( _obj.PoolManagementEnabled == false );
							
									_obj.MaxCreatures = (int)ICEEditorLayout.Slider( "Max. Creatures (" + _obj.CreaturesCount + ")","", _obj.MaxCreatures, 1, 0, 250 );

									//EditorGUILayout.Separator();

									ICEEditorLayout.Label( "Spawn Interval (cur. " + ( (int)(_obj.RespawnInterval/0.25f+0.5f)*0.25f) + " secs.)", false );
									EditorGUI.indentLevel++;
									_obj.MinRespawnInterval = (int)ICEEditorLayout.Slider( "Min. Spawn Interval","", _obj.MinRespawnInterval, 1, 1, _obj.MaxRespawnInterval );
									_obj.MaxRespawnInterval = (int)ICEEditorLayout.Slider( "Max. Spawn Interval","", _obj.MaxRespawnInterval, 1, _obj.MinRespawnInterval, 360 );
									EditorGUI.indentLevel--;

									EditorGUILayout.Separator();
									_obj.UseRandomScale = EditorGUILayout.Toggle( "Random Scale", _obj.UseRandomScale );

									EditorGUI.BeginDisabledGroup( _obj.UseRandomScale == false );
									EditorGUI.indentLevel++;
										bool _auto = false;
										_obj.UseRandomScaleMultiplier = ICEEditorLayout.AutoSlider( "Scale Multiplier","", _obj.UseRandomScaleMultiplier, 0.01f, 0, 1, ref _auto, 0.5f );

										if( _auto )
											_obj.UseRandomScaleMultiplier = (float)Random.Range(0,100) / 100;
							/*
										_obj.RandomScaleMin = EditorGUILayout.FloatField( "Min. Scale", _obj.RandomScaleMin ) ;
										_obj.RandomScaleMax = EditorGUILayout.FloatField( "Min. Scale", _obj.RandomScaleMax ) ;*/
									EditorGUI.indentLevel--;
									EditorGUI.EndDisabledGroup();

									EditorGUILayout.Separator();
									_obj.UseSoftRespawn = EditorGUILayout.Toggle( "Soft Respawn", _obj.UseSoftRespawn );
									_obj.UseGroupObject = EditorGUILayout.Toggle( "Use Group", _obj.UseGroupObject );
								
								EditorGUI.EndDisabledGroup();
								EditorGUI.indentLevel--;
								EditorGUILayout.Separator();
								
							}
						}
						else
						{
							m_creature_register.ReferenceCreatures.RemoveAt(i);
							i--;
						}
					}
				}

				EditorGUILayout.Separator();
				ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );

				ICEEditorLayout.BeginHorizontal();
					GameObject _new = (GameObject)EditorGUILayout.ObjectField( "Add Creature", null, typeof(GameObject), true );

					if( _new != null )
						m_creature_register.AddReferenceCreature( _new );

				ICEEditorLayout.EndHorizontal();


			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();

			if (GUI.changed)
				EditorUtility.SetDirty( m_creature_register );
		}

		/// <summary>
		/// Updates the creature status.
		/// </summary>
		/// <param name="_object">_object.</param>
		public void UpdateCreatureStatus( CreatureReferenceObject _object )
		{
			if( _object == null )
				return;

			_object.Status.HasCreatureController = false;
			_object.Status.HasCreatureAdapter = false;
			_object.Status.HasHome = false;
			_object.Status.HasMissionOutpost = false;
			_object.Status.HasMissionEscort = false;
			_object.Status.HasMissionPatrol = false;
			_object.Status.isActiveAndEnabled = false;
			_object.Status.isActiveInHierarchy = false;
			_object.Status.isPrefab = false;
			
			if( _object.Creature != null )
			{
				if( _object.Controller != null )
				{
					_object.Status.HasCreatureController = true;
					
					if( _object.Controller.isActiveAndEnabled )
						_object.Status.isActiveAndEnabled = true;

					if( _object.Controller.Creature.Essentials.TargetReady() )
						_object.Status.HasHome = true;
					
					if( _object.Controller.Creature.Missions.Outpost.TargetReady() )
						_object.Status.HasMissionOutpost = true;
					
					if( _object.Controller.Creature.Missions.Escort.TargetReady() )
						_object.Status.HasMissionEscort = true;
					
					if( _object.Controller.Creature.Missions.Patrol.TargetReady() )
						_object.Status.HasMissionPatrol = true;
					
				}
				
				if( _object.Creature.activeInHierarchy )
					_object.Status.isActiveInHierarchy = true;
				else if( PrefabUtility.GetPrefabParent( _object.Creature ) == null && PrefabUtility.GetPrefabObject( _object.Creature ) != null ) // Is a prefab
					_object.Status.isPrefab = true;

			}
		}

	}

}
