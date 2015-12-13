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
	
	public static class EditorSharedTools
	{	
		public static string RegisterPopup( string _title, string _group, string _help = "" )
		{
			ICECreatureRegister _register = ICECreatureRegister.Register;// GameObject.FindObjectOfType<ICECreatureRegister>();
			
			if( _register == null )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else if( _register.ReferenceCreatures.Count == 0 )
			{
				EditorGUILayout.LabelField( _title );
				return null;
			}
			else
			{
				List<CreatureReferenceObject> _creatures = _register.ReferenceCreatures;
				
				string[] _names = new string[_creatures.Count];

				int _index = 0;
				for(int i=0;i < _creatures.Count ;i++)
				{
					_names[i] = _creatures[i].Name;
					
					if( _group == _names[i] )
						_index = i;
					
				}

				if( _creatures[_index] != null && _creatures[_index].Status.isPrefab )
					_title += " (prefab)";
				else
					_title += " (scene)";

				_index = ICEEditorLayout.Popup( _title,"", _index, _names, _help );
				
				return _names[_index];
				
			}		
		}

		public static void DrawInRangeBehaviour( ICECreatureControl _control, ref string _leisure, ref string rendezvous, ref float _duration, ref bool _transit, float _range, int _index = 0 )
		{
			_duration = ICEEditorLayout.DurationSlider( "Duration Of Stay", "Desired duration of stay", _duration, Init.DURATION_OF_STAY_STEP, Init.DURATION_OF_STAY_MIN, Init.DURATION_OF_STAY_MAX,Init.DURATION_OF_STAY_DEFAULT, ref _transit );

			if( _transit == false )
			{
				EditorGUI.indentLevel++;

				if( _range > 0 )
				{
					_leisure = EditorBehaviour.BehaviourSelect( _control, "Leisure", "Randomized leisure activities after reaching the Random Range of the target. Please note, if the Random Range is adjusted to zero, leisure is not available.", _leisure, "WP_LEISURE" + (_index>0?"_"+_index:"") );
				}

				EditorGUI.BeginDisabledGroup( _duration == 0 );
				rendezvous = EditorBehaviour.BehaviourSelect( _control, "Rendezvous", "Action behaviour after reaching the Stop Distance of the given target move position.", rendezvous, "WP_RENDEZVOUS" + (_index>0?"_"+_index:"") );
				EditorGUI.EndDisabledGroup();
				
				EditorGUI.indentLevel--;
			}
		}

		public static EffectContainer DrawSharedEffect( EffectContainer _effect, string _help )
		{
			_effect.Enabled = ICEEditorLayout.ToggleLeft("Effect","", _effect.Enabled, true, _help );
			
			if( _effect.Enabled )
			{
				EditorGUI.indentLevel++;
						
					_effect.ReferenceObject = (GameObject)EditorGUILayout.ObjectField( "Reference", _effect.ReferenceObject, typeof(GameObject), false);
					

					_effect.OffsetType = (RandomOffsetType)ICEEditorLayout.EnumPopup( "Offset Type","", _effect.OffsetType );

					EditorGUI.indentLevel++;
					
						if( _effect.OffsetType == RandomOffsetType.EXACT )
							_effect.Offset = EditorGUILayout.Vector3Field( "Offset", _effect.Offset );
						else 
							_effect.OffsetRadius = ICEEditorLayout.Slider( "Offset Radius", "", _effect.OffsetRadius, 0.25f, 0, 100 );

					EditorGUI.indentLevel--;

			 		_effect.Detach = EditorGUILayout.Toggle( "Detach", _effect.Detach );
				EditorGUI.indentLevel--;
			}
			
			return _effect;
		}
		
		public static AudioSource _test;

		public static AudioDataObject DrawSharedAudio( AudioDataObject _audio, string _help )
		{
			ICEEditorLayout.BeginHorizontal();
			
			_audio.Enabled = ICEEditorLayout.ToggleLeft("Audio", "", _audio.Enabled, true );



			/*
			if( _audio.Enabled )
			{
				if (GUILayout.Button("SAVE", ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveAudioContainerToFile( _audio, m_creature_control.gameObject.name );
				
				if (GUILayout.Button("LOAD", ICEEditorStyle.ButtonMiddle ))
					_audio = CreatureIO.LoadAudioContainernFromFile( _audio );
			}*/
			ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_AUDIO );

			if( _audio.Enabled )
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.Separator();
				
				for( int i = 0 ; i < _audio.Clips.Count ; i++ )
				{
					ICEEditorLayout.BeginHorizontal();
					
						_audio.Clips[i] = (AudioClip)EditorGUILayout.ObjectField("Audio Clip #" + (int)(i+1), _audio.Clips[i], typeof(AudioClip), false);
						
						if ( GUILayout.Button("x", ICEEditorStyle.CMDButton ) )
						{
							_audio.DeleteClip( i );
							--i;
						}
					ICEEditorLayout.EndHorizontal( Info.BEHAVIOUR_AUDIO );
				}
				
				
				ICEEditorLayout.BeginHorizontal();
				
				_audio.AddClip( (AudioClip)EditorGUILayout.ObjectField("New Audio Clip", null, typeof(AudioClip), false) );
				
				if ( GUILayout.Button("ADD", ICEEditorStyle.ButtonMiddle ) )
					_audio.AddClip();
				
				
				ICEEditorLayout.EndHorizontal();
				
				
				
				EditorGUILayout.Separator();
				
				_audio.Volume = ICEEditorLayout.DefaultSlider( "Volume", "The volume of the sound at the MinDistance",_audio.Volume, 0.05f, 0, 1, 0.5f );
				
				EditorGUILayout.Separator();
				
				_audio.MinPitch = ICEEditorLayout.DefaultSlider( "Min. Pitch", "Lowest value of the random pitch of the audio source.", _audio.MinPitch, 0.05f, -3, _audio.MaxPitch, 1 );
				_audio.MaxPitch = ICEEditorLayout.DefaultSlider( "Max. Pitch", "Highest value of the random pitch of the audio source.", _audio.MaxPitch, 0.05f, _audio.MinPitch, 3, 1.5f );
				
				EditorGUILayout.Separator();
				
				_audio.MinDistance = ICEEditorLayout.DefaultSlider( "Min. Distance", "Within the Min distance the AudioSource will cease to grow louder in volume. Outside the min distance the volume starts to attenuate.", _audio.MinDistance, 0.25f, 0, _audio.MaxDistance, 10 );
				_audio.MaxDistance = ICEEditorLayout.DefaultSlider( "Max. Distance", "Dependent to the RolloffMode, the MaxDistance is the distance where the sound is completely inaudible.", _audio.MaxDistance, 0.25f, _audio.MinDistance, 250, 50 );
				
				EditorGUILayout.Separator();
				
				_audio.Loop = ICEEditorLayout.Toggle("Loop","Is the audio clip looping?", _audio.Loop);
				_audio.RolloffMode = (AudioRolloffMode)ICEEditorLayout.EnumPopup("RolloffMode", "Rolloff modes that a 3D sound can have in an audio source.", _audio.RolloffMode );
				
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();
			}
			
			return _audio;
		}
		

		
		public static StatusContainer DrawSharedInfluences( StatusContainer _influences, string _help )
		{
			_influences.Enabled = ICEEditorLayout.ToggleLeft("Influences","", _influences.Enabled, true, Info.STATUS_INFLUENCES );
			
			return DrawShareInfluencesContent( _influences, _help );
		}
		
		public static StatusContainer DrawShareInfluencesContent( StatusContainer _influences, string _help )
		{
			if( _influences.Enabled )
			{
				EditorGUI.indentLevel++;

				_influences.Damage = ICEEditorLayout.DefaultSlider( "Damage (%)", "Damage influence in percent", _influences.Damage, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_DAMAGE );		
				_influences.Stress = ICEEditorLayout.DefaultSlider( "Stress (%)", "Stress influence in percent", _influences.Stress, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_STRESS );				
				_influences.Debility = ICEEditorLayout.DefaultSlider( "Debility (%)", "Debility influence in percent", _influences.Debility, 0.0025f, -100, 100,0,Info.STATUS_INFLUENCES_DEBILITY );
				_influences.Hunger = ICEEditorLayout.DefaultSlider( "Hunger (%)", "Hunger influence in percent", _influences.Hunger, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_HUNGER );				
				_influences.Thirst = ICEEditorLayout.DefaultSlider( "Thirst (%)", "Thirst influence in percent", _influences.Thirst, 0.0025f, -100, 100,0, Info.STATUS_INFLUENCES_THIRST );				
				
				EditorGUI.indentLevel--;
				
				EditorGUILayout.Separator();
			}
			
			return _influences;
		}

		public static string DrawTargetPopup( ICECreatureControl _control, string _title,string _hint, string _name, string _help = "" )
		{
			List<string> _targets = new List<string>();

			if( _control.Creature.Essentials.TargetReady() )
				_targets.Add( _control.Creature.Essentials.Target.TargetGameObject.name );
			if( _control.Creature.Missions.Outpost.Enabled && _control.Creature.Missions.Outpost.TargetReady() )
				_targets.Add( _control.Creature.Missions.Outpost.Target.TargetGameObject.name );
			if( _control.Creature.Missions.Escort.Enabled && _control.Creature.Missions.Escort.TargetReady() )
				_targets.Add( _control.Creature.Missions.Escort.Target.TargetGameObject.name );

			if( _control.Creature.Missions.Patrol.Enabled && _control.Creature.Missions.Patrol.TargetReady() )
			{
				foreach( WaypointObject _wp in _control.Creature.Missions.Patrol.Waypoints.GetValidWaypoints())
				{
					if( _wp.TargetGameObject != null )
						_targets.Add( _wp.TargetGameObject.name );
				}
			}
			/*
			foreach( InteractorObject _interactor in _control.Creature.Interaction.Interactors )
				_targets.Add( _interactor.Name );*/


			GUIContent[] _options = new GUIContent[ _targets.Count + 1];
			int _selected = 0;
			
			_options[0] = new GUIContent( " ");
			for( int i = 0 ; i < _targets.Count ; i++ )
			{
				int _index = i + 1;

				if( _targets[i] != "" )
				{		
					_options[ _index ] = new GUIContent( _targets[i] );
					
					if( _targets[i] == _name )
						_selected = _index;
				}
				else
				{
					_options[ _index ] = new GUIContent( "ERROR" );
				}
			}

			_selected = ICEEditorLayout.Popup( _title, _hint, _selected , _options, _help  );
			
			return _options[ _selected ].text;
		}

		public static void DrawTargetSelectors( ICECreatureControl _control, TargetSelectorsObject _selectors, TargetType _type, float _min_distance , float _max_distance  )
		{
			string _help = Info.TARGET_SELECTION_CRITERIA ;
			if( _type == TargetType.HOME )
				_help = Info.TARGET_SELECTION_CRITERIA + "\n\n" + Info.TARGET_SELECTION_CRITERIA_HOME;

			// TARGET SELECTION CRITERIAS
			if( _type == TargetType.HOME )
				_selectors.UseSelectionCriteriaForHome = ICEEditorLayout.Toggle( "Target Selection Criteria", "The HOME target should always have the lowest priority, but if you want you could adapt these settings also.", _selectors.UseSelectionCriteriaForHome, _help );
			else
				ICEEditorLayout.Label( "Target Selection Criteria", false );
	
			if( _type == TargetType.HOME && _selectors.UseSelectionCriteriaForHome == false )
			{
				_selectors.Priority = 0;
				_selectors.SelectionRange = 0;
				_selectors.SelectionAngle = 0;
				_selectors.UseAdvanced = false;
				return;
			}

			EditorGUI.indentLevel++;
				



				// PRIORITY BEGIN
				ICEEditorLayout.BeginHorizontal();				
					if( _selectors.CanUseDefaultPriority )
						_selectors.Priority = (int)ICEEditorLayout.AutoSlider( "Priority","", _selectors.Priority, 1,0, 100, ref _selectors.UseDefaultPriority, _selectors.DefaultPriority );
					else
					_selectors.Priority = (int)ICEEditorLayout.DefaultSlider( "Priority", "Priority to select this target!", _selectors.Priority, 1, 0, 100, _selectors.GetDefaultPriorityByType( _type ) );
				ICEEditorLayout.EndHorizontal( _help );
				// PRIORITY END
/*
			if( _selectors.UseAdvanced )
			{
				ICEEditorLayout.DrawProgressBar("Dynamic Priority", _selectors.GetPriority( _type ) );
			}*/

				string _range_title = "Selection Range";
				if( _selectors.SelectionRange == 0 )
					_range_title += " (infinite)";
				else
					_range_title += " (limited)";


				// SELECTION RANGE BEGIN
				ICEEditorLayout.BeginHorizontal();	
					_selectors.SelectionRange = ICEEditorLayout.DefaultSlider( _range_title , "If the selection range greater than 0 this target will only select if the creature is within the specified range", _selectors.SelectionRange, Init.SELECTION_RANGE_STEP, _min_distance, _max_distance, _selectors.GetDefaultRangeByType( _type ) );

			
					if( _selectors.UseFieldOfView )
						GUI.backgroundColor = Color.yellow;
	
					if (GUILayout.Button( new GUIContent( "FOV", "Field Of View - the target must be visible for the creature" ) , ICEEditorStyle.CMDButtonDouble ) )
						_selectors.UseFieldOfView = ! _selectors.UseFieldOfView;

					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

					if( _selectors.UseAdvanced )
					{
						if (GUILayout.Button( new GUIContent( "AND", "Add additional AND conditions" ) , ICEEditorStyle.CMDButtonDouble ) )
							_selectors.Selectors.Add( new TargetSelectorObject( ConditionalOperatorType.AND ) );
						if (GUILayout.Button( new GUIContent( "OR", "Add additional OR conditions" ) , ICEEditorStyle.CMDButtonDouble ) )
							_selectors.Selectors.Add( new TargetSelectorObject( ConditionalOperatorType.AND ) );
						//if (GUILayout.Button( new GUIContent( "THEN", "Add optional statements for cases the conditions are complied with or completed within the timescales determined" ) , ICEEditorStyle.CMDButtonDouble ) )
						//	_selectors.Statements.Add( new TargetSelectorStatementObject() );
						if (GUILayout.Button( new GUIContent( "RESET", "Removes all groups and conditions" ) , ICEEditorStyle.CMDButtonDouble ) )
							_selectors.Selectors.Clear();
					}

					if( _selectors.UseAdvanced )
						GUI.backgroundColor = new Vector4( 1,0.6f,0.3f,1);
					else
						GUI.backgroundColor = Color.green;
					
					if (GUILayout.Button( new GUIContent( "ADV", "Use advanced selector settings" ) , ICEEditorStyle.CMDButtonDouble ) )
						_selectors.UseAdvanced = ! _selectors.UseAdvanced;
					
					GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

				ICEEditorLayout.EndHorizontal();
				// SELECTION RANGE END

			EditorGUI.BeginDisabledGroup( _selectors.SelectionRange == 0 );
			EditorGUI.indentLevel++;

				string _angle_title = "Angular Restriction";
				if( _selectors.SelectionAngle == 0 || _selectors.SelectionAngle == 180 )
					_angle_title += " (full-circle)";
				else if( _selectors.SelectionAngle == 90 )
					_angle_title += " (semi-circle)";
				else if( _selectors.SelectionAngle == 45 )
					_angle_title += " (quadrant)";
				else
					_angle_title += " (sector)";

				// SELECTION ANGLE BEGIN
				ICEEditorLayout.BeginHorizontal();	
				_selectors.SelectionAngle = ICEEditorLayout.DefaultSlider( _angle_title , "", _selectors.SelectionAngle * 2, Init.SELECTION_ANGLE_STEP, Init.SELECTION_ANGLE_MIN, Init.SELECTION_ANGLE_MAX, _selectors.GetDefaultAngleByType( _type ) ) / 2;

				if (GUILayout.Button( new GUIContent( "90", "" ) , ICEEditorStyle.CMDButtonDouble ) )
					_selectors.SelectionAngle = 45;

				if (GUILayout.Button( new GUIContent( "180", "" ) , ICEEditorStyle.CMDButtonDouble ) )
					_selectors.SelectionAngle = 90;

				if (GUILayout.Button( new GUIContent( "360", "" ) , ICEEditorStyle.CMDButtonDouble ) )
					_selectors.SelectionAngle = 180;

				ICEEditorLayout.EndHorizontal();
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();
			// SELECTION ANGLE END

				if( _selectors.UseAdvanced )
				{
					Info.Help( Info.TARGET_SELECTION_CRITERIA_ADVANCED );

					// SELECTOR GROUPS BEGIN
					foreach( TargetSelectorObject _selector in _selectors.Selectors )
					{
						EditorGUI.indentLevel++;

							for( int i = 0 ; i < _selector.Conditions.Count ; i++ )
							{
								if( i > 0 ) EditorGUI.indentLevel++;

								TargetSelectorConditionObject _condition = _selector.Conditions[i];

								string _prefix = _condition.ConditionToString();									
								string _operator = _condition.OperatorToString();		
				
								ICEEditorLayout.BeginHorizontal();

									if( _condition.ExpressionType == TargetSelectorExpressionType.NONE )
									{
										_condition.ExpressionType = (TargetSelectorExpressionType)ICEEditorLayout.EnumPopup( _prefix, "Adds an additional selector of the defined type", TargetSelectorExpressionType.NONE );
									}
									else if( _condition.ExpressionType == TargetSelectorExpressionType.DISTANCE )								
									{
										_condition.Distance = ICEEditorLayout.Slider( _prefix + " Distance " + _operator, "If the selection range greater than 0 this target will only select if the creature is within the specified range", _condition.Distance, Init.SELECTION_RANGE_STEP, Init.SELECTION_RANGE_MIN, Init.SELECTION_RANGE_MAX );
									}
									else if( _condition.ExpressionType == TargetSelectorExpressionType.BEHAVIOR )								
									{
										_condition.BehaviourModeKey = EditorBehaviour.BehaviourPopup( _control, _prefix + " Behaviour " + _operator, "", _condition.BehaviourModeKey );
									}
									else if( _condition.ExpressionType == TargetSelectorExpressionType.POSITION )								
									{
										_condition.PositionType = (TargetSelectorPositionType)ICEEditorLayout.EnumPopup( _prefix + " Creatures Position " + _operator, "Adds an additional selector of the defined type", _condition.PositionType );
									}
									else if( _condition.ExpressionType == TargetSelectorExpressionType.PRECURSOR )
									{		
										
										if( _condition.PrecursorType == TargetPrecursorType.TYPE )
											_condition.PrecursorTargetType = (TargetType)ICEEditorLayout.EnumPopup( _prefix + " Precursor Type " + _operator, "This target will only select if the active target has the specified type.", _condition.PrecursorTargetType ); 
										else if( _condition.PrecursorType == TargetPrecursorType.TAG )
											_condition.PrecursorTargetTag = EditorGUILayout.TagField( _prefix + " Precursor Tag " + _operator, _condition.PrecursorTargetTag );
										else if( _condition.PrecursorType == TargetPrecursorType.NAME )
											_condition.PrecursorTargetName = DrawTargetPopup( _control, _prefix + " Precursor Name " + _operator, "This target will only select if the active target has the specified name.", _condition.PrecursorTargetName );
										
										if (GUILayout.Button(  new GUIContent( _condition.PrecursorType.ToString(), "Select precursor type" ), ICEEditorStyle.CMDButtonDouble ) )
										{
											if( _condition.PrecursorType == TargetPrecursorType.TAG )
												_condition.PrecursorType = 0;
											else
												_condition.PrecursorType++;
										}
									}

									// STANDARD BUTTONS BEGIN
									if (GUILayout.Button( new GUIContent( _prefix, "Changes conditional operator" ) , ICEEditorStyle.CMDButtonDouble ) )
									{
										if( _condition.ConditionType == ConditionalOperatorType.AND )
											_condition.ConditionType = ConditionalOperatorType.OR;
										else
											_condition.ConditionType = ConditionalOperatorType.AND;
									}

									if (GUILayout.Button( new GUIContent( _operator, "Changes relational operator" ) , ICEEditorStyle.CMDButtonDouble ) )
									{
										if( _condition.ExpressionType == TargetSelectorExpressionType.DISTANCE )
										{
											if( _condition.Operator == LogicalOperatorType.GREATER_OR_EQUAL )
												_condition.Operator = 0;
											else
												_condition.Operator++;
										}
										else
										{
											if( _condition.Operator == LogicalOperatorType.NOT )
												_condition.Operator = 0;
											else
												_condition.Operator++;
										}
									}

									if (GUILayout.Button( new GUIContent( "DEL", "Removes this condition" ) , ICEEditorStyle.CMDButtonDouble ) )
									{
										_selector.Conditions.Remove( _condition );
										return;
									}
									// STANDARD BUTTONS END
					

									ICEEditorLayout.EndHorizontal();
					
								if( i > 0 ) EditorGUI.indentLevel--;
							}

							// SELECTOR STATMENTS BEGIN
							//if( _selector.Statements.Count > 0 )
							//	DrawTargetSelectorStatement( _control, _selector.Statements );
							// SELECTOR STATMENTS END

			
							// SELECTOR ADD BEGIN
							ICEEditorLayout.BeginHorizontal();
							
								if( _selector.Statements.Count > 0 )
									DrawTargetSelectorStatementContent( _control, _selector.Statements[0] );
								else
									ICEEditorLayout.Label( " ", false );

								if (GUILayout.Button( new GUIContent( "THEN", "Add Statement" ) , ICEEditorStyle.CMDButtonDouble ) )
								{
									if( _selector.Statements.Count == 0 )
										_selector.Statements.Add( new TargetSelectorStatementObject() );
									else
										_selector.Statements.Clear();
									return;
								}

								
								if (GUILayout.Button( new GUIContent( "AND", "Add AND condition" ) , ICEEditorStyle.CMDButtonDouble ) )
								{
									_selector.Conditions.Add( new TargetSelectorConditionObject( ConditionalOperatorType.AND ) );
									return;
								}
								
								if (GUILayout.Button( new GUIContent( "OR", "Add OR condition" ) , ICEEditorStyle.CMDButtonDouble ) )
								{
									_selector.Conditions.Add( new TargetSelectorConditionObject( ConditionalOperatorType.OR ) );
									return;
								}

								
								if (GUILayout.Button( new GUIContent( "DEL", "Removes the selected group" ) , ICEEditorStyle.CMDButtonDouble ) )
								{
									_selectors.Selectors.Remove( _selector );
									return;
								}
							
							ICEEditorLayout.EndHorizontal();
							// SELECTOR ADD END
							



						EditorGUI.indentLevel--;

					}
					// SELECTOR GROUPS END
			

				// SELECTOR STATMENTS BEGIN
				//if( _selectors.Statements.Count > 0 )
				//	DrawTargetSelectorStatement( _control, _selectors.Statements );
				// SELECTOR STATMENTS END
				}
			EditorGUI.indentLevel--;

		}

		public static void DrawTargetSelectorStatementContent( ICECreatureControl _control, TargetSelectorStatementObject _statement )
		{
			if( _statement == null )
				return;
			

				string _prefix = "THEN";
				
				if( _statement.StatementType == TargetSelectorStatementType.NONE )
				{
					_statement.StatementType = (TargetSelectorStatementType)ICEEditorLayout.EnumPopup( _prefix, "Select the desired statement type", TargetSelectorStatementType.NONE );
				}
				else if( _statement.StatementType == TargetSelectorStatementType.PRIORITY )
				{					
					_statement.Priority = (int)ICEEditorLayout.Slider( _prefix + " Priority", "Priority to select this target!", _statement.Priority, 1, 0, 100 );
					
				}
				/*else if( _statement.StatementType == TargetSelectorStatementType.MULTIPLIER )
				{		
					_statement.Priority = (int)ICEEditorLayout.DefaultSlider()
				}*/
				else if( _statement.StatementType == TargetSelectorStatementType.SUCCESSOR )
				{											
					if( _statement.SuccessorType == TargetSuccessorType.TYPE )
						_statement.SuccessorTargetType = (TargetType)ICEEditorLayout.EnumPopup( _prefix + " Successor Type", "This target will only select if the active target has the specified type.", _statement.SuccessorTargetType ); 
					else if( _statement.SuccessorType == TargetSuccessorType.TAG )
						_statement.SuccessorTargetTag = EditorGUILayout.TagField( _prefix + " Successor Tag", _statement.SuccessorTargetTag );
					else if( _statement.SuccessorType == TargetSuccessorType.NAME )
						_statement.SuccessorTargetName = DrawTargetPopup( _control, _prefix + " Successor Name", "This target will only select if the active target has the specified name.", _statement.SuccessorTargetName );
					
					if (GUILayout.Button(  new GUIContent( _statement.SuccessorType.ToString(), "Select precursor type" ), ICEEditorStyle.CMDButtonDouble ) )
					{
						if( _statement.SuccessorType == TargetSuccessorType.TAG )
							_statement.SuccessorType = 0;
						else
							_statement.SuccessorType++;
					}
				}
				
		}

		public static void DrawTargetSelectorStatement( ICECreatureControl _control, List<TargetSelectorStatementObject> _statements )
		{
			if( _statements == null )
				return;

			for( int i = 0 ; i < _statements.Count ; i++ )
			{
				if( i > 0 ) EditorGUI.indentLevel++;
				
				ICEEditorLayout.BeginHorizontal();
				
				TargetSelectorStatementObject _statement = _statements[i];
				
				//string _prefix = "THEN";

				DrawTargetSelectorStatementContent( _control, _statement );
				/*
				if( _statement.StatementType == TargetSelectorStatementType.NONE )
				{
					_statement.StatementType = (TargetSelectorStatementType)ICEEditorLayout.EnumPopup( _prefix, "Select the desired statement type", TargetSelectorStatementType.NONE );
				}
				else if( _statement.StatementType == TargetSelectorStatementType.PRIORITY )
				{					
					_statement.Priority = (int)ICEEditorLayout.DefaultSlider( _prefix + " Priority", "Priority to select this target!", _statement.Priority, 1, 0, 100, 0 );

				}
				else if( _statement.StatementType == TargetSelectorStatementType.MULTIPLIER )
				{		
					_statement.Priority = (int)ICEEditorLayout.DefaultSlider()
				}
				else if( _statement.StatementType == TargetSelectorStatementType.SUCCESSOR )
				{											
					if( _statement.SuccessorType == TargetSuccessorType.TYPE )
						_statement.SuccessorTargetType = (TargetType)ICEEditorLayout.EnumPopup( _prefix + " Successor Type", "This target will only select if the active target has the specified type.", _statement.SuccessorTargetType ); 
					else if( _statement.SuccessorType == TargetSuccessorType.TAG )
						_statement.SuccessorTargetTag = EditorGUILayout.TagField( _prefix + " Successor Tag", _statement.SuccessorTargetTag );
					else if( _statement.SuccessorType == TargetSuccessorType.NAME )
						_statement.SuccessorTargetName = DrawTargetPopup( _control, _prefix + " Successor Name", "This target will only select if the active target has the specified name.", _statement.SuccessorTargetName );
					
					if (GUILayout.Button(  new GUIContent( _statement.SuccessorType.ToString(), "Select precursor type" ), ICEEditorStyle.CMDButtonDouble ) )
					{
						if( _statement.SuccessorType == TargetSuccessorType.TAG )
							_statement.SuccessorType = 0;
						else
							_statement.SuccessorType++;
					}
				}*/
				
				if (GUILayout.Button( new GUIContent( "DEL", "Removes selected statement" ) , ICEEditorStyle.CMDButtonDouble ) )
				{
					_statements.Remove( _statement );
					return;
				}
				
				
				ICEEditorLayout.EndHorizontal();
				
				if( i > 0 ) EditorGUI.indentLevel--;
			}

		}

		public static void DrawTargetSelector( ICECreatureControl _control, TargetSelectorConditionObject _selector )
		{
			if( _selector == null )
				return;

			ICEEditorLayout.BeginHorizontal();

			ICEEditorLayout.EndHorizontal();

		}

		public static void DrawMove( ref float _segment, ref float _stop , ref float _directional_variance, ref float _laterally_variance, ref bool _level, string _help = "" )
		{
			if( _help == "" )
				Info.Help ( Info.MOVE );
			else
				Info.Help ( Info.MOVE + "\n\n" + _help );

			_segment = ICEEditorLayout.DefaultSlider( "Move Segment Length", "Subdivides the main path in segments of unitary length", _segment, Init.MOVE_DISTANCE_STEP, Init.MOVE_MIN_DISTANCE, Init.MOVE_MAX_DISTANCE, Init.MOVE_DISTANCE_DEFAULT, Info.MOVE_SEGMENT_LENGTH );
			EditorGUI.BeginDisabledGroup( _segment == 0 );
			EditorGUI.indentLevel++;
				_directional_variance = ICEEditorLayout.DefaultSlider( "Segment Variance Multiplier", "Generates randomized deviations along the path.", _directional_variance, Init.MOVE_INTERFERENCE_STEP, Init.MOVE_INTERFERENCE_MIN, Init.MOVE_INTERFERENCE_MAX, Init.MOVE_INTERFERENCE_DEFAULT, Info.MOVE_SEGMENT_VARIANCE);
				_laterally_variance = ICEEditorLayout.DefaultSlider( "Lateral Variance Multiplier", "Generates randomized deviations along the path.", _laterally_variance, Init.MOVE_INTERFERENCE_STEP, Init.MOVE_INTERFERENCE_MIN, Init.MOVE_INTERFERENCE_MAX, Init.MOVE_INTERFERENCE_DEFAULT,  Info.MOVE_LATERAL_VARIANCE );
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();

			_stop = ICEEditorLayout.DefaultSlider( "Move Stopping Distance (" + (_level?"circular":"spherical") + ")", "Stop within this distance from the target move position.", _stop, Init.STOP_DISTANCE_STEP, Init.STOP_MIN_DISTANCE, Init.STOP_MAX_DISTANCE, Init.STOP_DISTANCE_DEFAULT, Info.MOVE_STOPPING_DISTANCE );
			EditorGUI.indentLevel++;
				_level = ICEEditorLayout.Toggle( "Move Ignore Level Differences", "Provides linear distances without consideration of level differences.", _level, Info.MOVE_IGNORE_LEVEL_DIFFERENCE );
			EditorGUI.indentLevel--;
		}

		/// <summary>
		/// Draws the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="m_creature_control">M_creature_control.</param>
		/// <param name="_target">_target.</param>
		/// <param name="_help">_help.</param>
		public static TargetObject DrawTarget( ICECreatureControl _control, TargetObject _target, string _title, string _help = "" )
		{
			EditorGUILayout.Separator();
			ICEEditorLayout.Label( _title , true, _help );
			EditorGUI.indentLevel++;


				DrawTargetSelectors( _control, _target.Selectors, _target.Type, Init.SELECTION_RANGE_MIN, Init.SELECTION_RANGE_MAX );
				ICEEditorLayout.BeginHorizontal();

				_target.IsPrefab = false;
				if( PrefabUtility.GetPrefabParent( _target.TargetGameObject ) == null && PrefabUtility.GetPrefabObject( _target.TargetGameObject ) != null ) // Is a prefab
					_target.IsPrefab = true;

				if( _target.TargetGameObject == null )
					GUI.backgroundColor = Color.red;

				_target.TargetGameObject = (GameObject)EditorGUILayout.ObjectField("Target Object " + (_target.IsValid?(_target.IsPrefab?"(prefab)":"(scene)"):"(null)"), _target.TargetGameObject, typeof(GameObject), true);
				
				GUI.backgroundColor = ICEEditorLayout.DefaultBackgroundColor;

				if( _target.TargetGameObject != null )
				{
					if (GUILayout.Button("SHOW", ICEEditorStyle.ButtonMiddle ) )
					{
						var view = SceneView.currentDrawingSceneView;
						if(view != null)
						{
							Vector3 _pos = _target.TargetGameObject.transform.position; 
							
							_pos.y += 20;
							view.rotation        = new Quaternion(1,0,0,1);
							view.LookAt( _pos );
							
							//	view.AlignViewToObject(_target.TargetGameObject.transform);
							
						}
					}
					
					if (GUILayout.Button("SELECT", ICEEditorStyle.ButtonMiddle ) )
						Selection.activeGameObject = _target.TargetGameObject;
				}

				ICEEditorLayout.EndHorizontal( Info.TARGET_OBJECT );

				EditorGUI.BeginDisabledGroup( _target.TargetGameObject == null );
					_target  = DrawTargetOffset( _control, _target );	
				EditorGUI.EndDisabledGroup();
			EditorGUI.indentLevel--;
			return _target;
		}

		/// <summary>
		/// Draws the target.
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="m_creature_control">M_creature_control.</param>
		/// <param name="_target">_target.</param>
		/// <param name="_help">_help.</param>
		public static TargetObject DrawTargetOffset( ICECreatureControl _control, TargetObject _target )
		{
			//ICEEditorLayout.Label( "TargetMovePosition Specifications", false, Info.TARGET_MOVE_SPECIFICATIONS );
		
			EditorGUI.indentLevel++;
				Vector3 _offset = ICEEditorLayout.OffsetGroup( "Offset", _target.TargetOffset, _control.gameObject, _target.TargetGameObject, 0.5f, 25, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET );
				
				if( _offset != _target.TargetOffset || ( _offset != Vector3.zero && _target.OffsetDistance == 0 ) || ( _offset == Vector3.zero && _target.OffsetDistance != 0 ) )
					_target.UpdateOffset( _offset );
				
				EditorGUI.indentLevel++;
				
				float _distance = ICEEditorLayout.DefaultSlider( "Distance", "",_target.OffsetDistance, Init.TARGET_OFFSET_DISTANCE_STEP, Init.TARGET_OFFSET_DISTANCE_MIN, Init.TARGET_OFFSET_DISTANCE_MAX, Init.TARGET_OFFSET_DISTANCE_DEFAULT, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_DISTANCE );
				EditorGUI.BeginDisabledGroup( _distance == 0 );
					float _angle = ICEEditorLayout.DefaultSlider( "Angle", "", _target.OffsetAngle, 1, 0, 360, 0, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_ANGLE );					
					
					if( _distance != _target.OffsetDistance || _angle != _target.OffsetAngle )
						_target.UpdateOffset( _angle, _distance );			
				EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;

				EditorGUILayout.Separator();
				_target.UpdateRandomRange( ICEEditorLayout.DefaultSlider( "Random Positioning Range", "", _target.TargetRandomRange, Init.TARGET_RANDOM_RANGE_STEP, Init.TARGET_RANDOM_RANGE_MIN, Init.TARGET_RANDOM_RANGE_MAX, Init.TARGET_RANDOM_RANGE_DEFAULT, Info.TARGET_MOVE_SPECIFICATIONS_RANDOM_RANGE ) );
				
				EditorGUI.BeginDisabledGroup( _target.TargetRandomRange == 0 );				
					EditorGUI.indentLevel++;
						_target.UseUpdateOffsetOnActivateTarget  = ICEEditorLayout.Toggle( "Update on activate","", _target.UseUpdateOffsetOnActivateTarget, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_UPDATE_ACTIVATE );
						_target.UseUpdateOffsetOnMovePositionReached  = ICEEditorLayout.Toggle( "Update on reached","", _target.UseUpdateOffsetOnMovePositionReached, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_UPDATE_REACHED );
						_target.UseUpdateOffsetOnRandomizedTimer  = ICEEditorLayout.Toggle( "Update on timer","", _target.UseUpdateOffsetOnRandomizedTimer, Info.TARGET_MOVE_SPECIFICATIONS_OFFSET_UPDATE_TIMER );
						EditorGUI.BeginDisabledGroup( _target.UseUpdateOffsetOnRandomizedTimer == false );
							EditorGUI.indentLevel++;
								_target.OffsetUpdateTimeMin = ICEEditorLayout.DefaultSlider( "Min. Interval", "", _target.OffsetUpdateTimeMin, 0.25f, 0, 360, 5 );
								_target.OffsetUpdateTimeMax = ICEEditorLayout.DefaultSlider( "Max. Interval", "", _target.OffsetUpdateTimeMax, 0.25f, 0, 360, 30 );
							EditorGUI.indentLevel--;
						EditorGUI.EndDisabledGroup();
					EditorGUI.indentLevel--;
				EditorGUI.EndDisabledGroup();

				EditorGUILayout.Separator();
				_target.TargetSmoothingMultiplier = ICEEditorLayout.DefaultSlider( "Smoothing", "Smoothing affects step-size and update speed of the TargetMovePosition.", _target.TargetSmoothingMultiplier, 0.01f, 0, 1, 0.5f, Info.TARGET_MOVE_SPECIFICATIONS_SMOOTHING );
				_target.TargetStopDistance = ICEEditorLayout.DefaultSlider( "Stopping Distance (" + (_target.TargetIgnoreLevelDifference?"circular":"spherical") + ")","Stop within this distance from the target move position.", _target.TargetStopDistance, Init.TARGET_STOP_DISTANCE_STEP, Init.TARGET_STOP_MIN_DISTANCE, Init.TARGET_STOP_MAX_DISTANCE, Init.TARGET_STOP_DISTANCE_DEFAULT, Info.TARGET_MOVE_SPECIFICATIONS_STOP_DISTANCE );
				EditorGUI.indentLevel++;
					_target.TargetIgnoreLevelDifference = ICEEditorLayout.InfoToggle( "Ignore Level Differences","Provides linear distances without consideration of level differences.", _target.TargetIgnoreLevelDifference, Info.TARGET_MOVE_SPECIFICATIONS_IGNORE_LEVEL_DIFFERENCE );
				EditorGUI.indentLevel--;


			EditorGUI.indentLevel--;

			return _target;
		}
	}
}