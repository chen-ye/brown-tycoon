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
	public static class EditorInteraction
	{	
		private static ICECreatureRegister m_creature_register = null;

		public static void Print( ICECreatureControl _control )
		{
			if( m_creature_register == null )
				m_creature_register = ICECreatureRegister.Register;

			if( m_creature_register == null )
				return;
			
			if( ! _control.Display.ShowInteractionSettings )
				return;
			
			ICEEditorStyle.SplitterByIndent( 0 );			
			ICEEditorLayout.BeginHorizontal();
				_control.Display.FoldoutInteraction = ICEEditorLayout.Foldout( _control.Display.FoldoutInteraction, "Interaction" );			
				if (GUILayout.Button( new GUIContent( "SAVE", "Saves the complete interaction settings to file" ), ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveInteractionToFile( _control.Creature.Interaction, _control.gameObject.name );			
				if (GUILayout.Button( new GUIContent( "LOAD", "Loads existing interaction settings form file" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Interaction = CreatureIO.LoadInteractionFromFile( _control.Creature.Interaction );			
				if (GUILayout.Button( new GUIContent( "RESET", "Removes all interaction settings" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Interaction.Reset();			
			ICEEditorLayout.EndHorizontal( Info.INTERACTION );
			
			if ( ! _control.Display.FoldoutInteraction ) 
				return;
				
			EditorGUILayout.Separator();
			EditorGUI.indentLevel++;				
				for (int _interactor_index = 0; _interactor_index < _control.Creature.Interaction.Interactors.Count; ++_interactor_index )
					DrawInteractor( _control, _control.Creature.Interaction, _interactor_index );					
			EditorGUI.indentLevel--;
			
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );			
			ICEEditorLayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Add Interactor", ICEEditorStyle.LabelBold );				
				if (GUILayout.Button( new GUIContent( "LOAD", "Load existing interactor settings from file" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Interaction.Interactors.Add( CreatureIO.LoadInteractorFromFile( new InteractorObject() ) );				
				if (GUILayout.Button( new GUIContent( "ADD", "Create a new interactor record" ), ICEEditorStyle.ButtonMiddle ))
					_control.Creature.Interaction.Interactors.Add( new InteractorObject() );				
			ICEEditorLayout.EndHorizontal();
						
			EditorGUILayout.Separator();

		}


		private static void DrawInteractor( ICECreatureControl _control, InteractionObject _interaction_object, int _index )
		{
			ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel );
			
			InteractorObject _interactor_object = _interaction_object.Interactors[_index];
			
			ICEEditorLayout.BeginHorizontal();				
				_interactor_object.Foldout = EditorGUILayout.Foldout(_interactor_object.Foldout, "Interactor '" + _interactor_object.Name + "' (" + _interactor_object.Rules.Count + " Rules) " , ICEEditorStyle.Foldout);
				
				if (GUILayout.Button(new GUIContent( "SAVE", "Saves selected interactor to file" ), ICEEditorStyle.ButtonMiddle ))
					CreatureIO.SaveInteractorToFile( _interactor_object, _interactor_object.Name );
				
				if (GUILayout.Button( new GUIContent( "LOAD", "Replaces selected interactor settings" ), ICEEditorStyle.ButtonMiddle ))
				{
					_control.Creature.Interaction.Interactors.Insert(_index, CreatureIO.LoadInteractorFromFile( new InteractorObject() ) );
					_interaction_object.Interactors.Remove( _interactor_object );
					return;
				}
				
				if (GUILayout.Button( new GUIContent( "REMOVE", "Removes selected interactor" ), ICEEditorStyle.ButtonMiddle ))
				{
					_interaction_object.Interactors.RemoveAt(_index);
					--_index;
					return;
				}
			ICEEditorLayout.EndHorizontal( Info.INTERACTION_INTERACTOR );
			
			
			if( ! _interactor_object.Foldout )
				return;

			EditorGUILayout.Separator();
			_interactor_object.Enabled = ICEEditorLayout.ToggleLeft("Enabled","", _interactor_object.Enabled, false, Info.INTERACTION_INTERACTOR_ENABLED );
			
			EditorGUI.BeginDisabledGroup ( _interactor_object.Enabled == false);
			
				EditorGUILayout.Separator();
				EditorSharedTools.DrawTargetSelectors( _control, _interactor_object.Selectors, TargetType.INTERACTOR, Init.SELECTION_RANGE_MIN, Init.SELECTION_RANGE_MAX );
				
				if( _interactor_object.Selectors.SelectionRange == 0 )
				{
					EditorGUI.indentLevel++;
					Info.Note( "Selection Range adjusted to zero - no regional selection restriction!" );
					EditorGUI.indentLevel--;
				}
				
				_interactor_object = DrawInteractorOffset( _control, _interactor_object );
				_interactor_object.BehaviourModeKey = EditorBehaviour.BehaviourSelect( _control, "Behaviour", "Behaviour while sensing this interactor", _interactor_object.BehaviourModeKey, "SENSE" ); 
				
				EditorGUILayout.Separator();
				ICEEditorLayout.Label( "Additional Rules for meeting '" + _interactor_object.Name + "' creatures.", true );	
				
				if( _interactor_object.Rules.Count == 0 )
					Info.Note( Info.INTERACTION_INTERACTOR_NO_RULES );
				else
				{
					EditorGUILayout.Separator();
					for( int _behaviour_index = 0 ; _behaviour_index < _interactor_object.Rules.Count ; _behaviour_index++ )
						DrawInteractorRule( _control, _interactor_object, _behaviour_index );
				}
				
				
				ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );					
				ICEEditorLayout.BeginHorizontal();
				EditorGUILayout.LabelField( "Add Interaction Rule for '" + _interactor_object.Name + "'", EditorStyles.boldLabel );				
				if (GUILayout.Button("ADD", ICEEditorStyle.ButtonMiddle ))
					_interactor_object.Rules.Add( new InteractorRuleObject( "" ) );
				ICEEditorLayout.EndHorizontal();				
				
				EditorGUILayout.Separator();
			EditorGUI.EndDisabledGroup();

		}
		
		private static InteractorObject DrawInteractorOffset( ICECreatureControl _control,InteractorObject _interactor )
		{

			TargetObject _target = new TargetObject( TargetType.INTERACTOR );
			_target.TargetGameObject = m_creature_register.GetReferenceCreatureByName( _interactor.Name );
			
			ICEEditorLayout.BeginHorizontal();
				_interactor.Name = EditorSharedTools.RegisterPopup( "Target Object", _interactor.Name );		
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
			ICEEditorLayout.EndHorizontal( Info.INTERACTION_INTERACTOR_TARGET );
			
			
			_target.UpdateOffset( _interactor.DefaultOffset );
			_target.TargetStopDistance = _interactor.DefaultStopDistance;
			_target.TargetRandomRange = _interactor.DefaultRandomRange;
			_target.UseUpdateOffsetOnActivateTarget = _interactor.UpdateOffsetOnActivateTarget;
			_target.UseUpdateOffsetOnMovePositionReached = _interactor.UpdateOffsetOnMovePositionReached;
			_target.TargetSmoothingMultiplier = _interactor.DefaultSmoothingMultiplier;
			_target.TargetIgnoreLevelDifference = _interactor.DefaultIgnoreLevelDifference;
			
			_target = EditorSharedTools.DrawTargetOffset( _control, _target);
			
			_interactor.DefaultOffset = _target.TargetOffset;
			_interactor.DefaultStopDistance = _target.TargetStopDistance;
			_interactor.DefaultRandomRange = _target.TargetRandomRange;
			_interactor.UpdateOffsetOnActivateTarget = _target.UseUpdateOffsetOnActivateTarget;
			_interactor.UpdateOffsetOnMovePositionReached = _target.UseUpdateOffsetOnMovePositionReached;
			_interactor.DefaultIgnoreLevelDifference = _target.TargetIgnoreLevelDifference;
			
			_interactor.DefaultSmoothingMultiplier = _target.TargetSmoothingMultiplier;
			
			_target.TargetGameObject = null;
			_target = null;
			
			return _interactor;
		}
		
		private static InteractorRuleObject DrawInteractorRuleOffset( ICECreatureControl _control,InteractorObject _interactor, InteractorRuleObject _rule )
		{
			EditorGUI.indentLevel++;
			
			TargetObject _target = new TargetObject(  TargetType.INTERACTOR );
			_target.TargetGameObject = m_creature_register.GetReferenceCreatureByName( _interactor.Name );
			
			
			ICEEditorLayout.BeginHorizontal();
			
			string _title = "Target Object";
			if( PrefabUtility.GetPrefabParent( _target.TargetGameObject ) == null && PrefabUtility.GetPrefabObject( _target.TargetGameObject ) != null ) // Is a prefab
				_title += " (prefab)";
			else if( _target.TargetGameObject != null )
				_title += " (scene)";
			else 
				_title += " (null)";
			
			EditorGUILayout.LabelField( _title,  _interactor.Name );
			
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
			ICEEditorLayout.EndHorizontal();
			
			_target.UpdateOffset( _rule.Offset );
			_target.TargetStopDistance = _rule.StopDistance;
			_target.TargetRandomRange = _rule.RandomRange;
			_target.TargetIgnoreLevelDifference = _rule.IgnoreLevelDifference;
			_target.UseUpdateOffsetOnActivateTarget = _rule.UpdateOffsetOnActivateTarget;
			_target.UseUpdateOffsetOnMovePositionReached = _rule.UpdateOffsetOnMovePositionReached;
			_target.TargetSmoothingMultiplier = _rule.SmoothingMultiplier;
			
			_target = EditorSharedTools.DrawTargetOffset( _control, _target);
			
			_rule.Offset = _target.TargetOffset;
			_rule.StopDistance = _target.TargetStopDistance;
			_rule.RandomRange = _target.TargetRandomRange;
			_rule.UpdateOffsetOnActivateTarget = _target.UseUpdateOffsetOnActivateTarget;
			_rule.UpdateOffsetOnMovePositionReached = _target.UseUpdateOffsetOnMovePositionReached;
			_rule.SmoothingMultiplier = _target.TargetSmoothingMultiplier;
			_rule.IgnoreLevelDifference = _target.TargetIgnoreLevelDifference;
			
			_target.TargetGameObject = null;
			_target = null;
			
			EditorGUI.indentLevel--;
			
			return _rule;
		}
		
		private static void DrawInteractorRule( ICECreatureControl _control, InteractorObject _interactor, int _index )
		{
			InteractorRuleObject _rule = _interactor.Rules[_index];
			InteractorRuleObject _prev_rule = null;
			InteractorRuleObject _next_rule = null;
			
			float _rule_max_distance = Init.DEFAULT_MAX_DISTANCE;
			float _rule_min_distance = 0;
			
			
			if( _interactor.Selectors.SelectionRange >= _rule.Selectors.SelectionRange + Init.SELECTION_RANGE_STEP )
				_rule_max_distance = _interactor.Selectors.SelectionRange - Init.SELECTION_RANGE_STEP;
			
			int _prev_index = _index - 1;
			int _next_index = _index + 1;
			
			
			if( _prev_index >= 0 )
			{
				_prev_rule = _interactor.Rules[_prev_index];
				_rule_max_distance = _prev_rule.Selectors.SelectionRange - Init.SELECTION_RANGE_STEP;
			}
			
			if( _next_index < _interactor.Rules.Count )
			{
				_next_rule = _interactor.Rules[_next_index];
				_rule_min_distance = _next_rule.Selectors.SelectionRange + Init.SELECTION_RANGE_STEP;
			}
			
			
			
			
			ICEEditorLayout.BeginHorizontal();				
				_rule.Enabled = ICEEditorLayout.ToggleLeft( " RULE #" + _index + " - " + (_rule.BehaviourModeKey.Trim() != ""?_rule.BehaviourModeKey:"UNDEFINED" ) ,"", _rule.Enabled ,true );
				if( _interactor.Rules.Count > 1 )
				{
					EditorGUI.BeginDisabledGroup( _index <= 0 );					
						if( ICEEditorLayout.ButtonUp() )
						{
							InteractorRuleObject _obj = _interactor.Rules[_index]; 
							_interactor.Rules.RemoveAt( _index );
							float _obj_selection_range = _obj.Selectors.SelectionRange;
							
							if( _index - 1 < 0 )
								_interactor.Rules.Add( _obj );
							else
								_interactor.Rules.Insert( _index - 1, _obj );
							
							if( _prev_rule != null )
							{	
								_obj.Selectors.SelectionRange = _prev_rule.Selectors.SelectionRange;
								_prev_rule.Selectors.SelectionRange = _obj_selection_range;
							}	
							return;
						}					
					EditorGUI.EndDisabledGroup();
					
					EditorGUI.BeginDisabledGroup( _index >= _interactor.Rules.Count - 1 );					
						if( ICEEditorLayout.ButtonDown() )
						{
							InteractorRuleObject _obj = _interactor.Rules[_index]; 
							_interactor.Rules.RemoveAt( _index );
							float _obj_selection_range = _obj.Selectors.SelectionRange;
							
							if( _index + 1 > _interactor.Rules.Count )
								_interactor.Rules.Insert( 0, _obj );
							else
								_interactor.Rules.Insert( _index +1, _obj );
							
							if( _next_rule  != null )
							{	
								_obj.Selectors.SelectionRange = _next_rule.Selectors.SelectionRange;
								_next_rule.Selectors.SelectionRange = _obj_selection_range;
							}	
							return;
						}	
					EditorGUI.EndDisabledGroup();
				}
				
				if( GUILayout.Button("X", ICEEditorStyle.CMDButtonDouble ))
				{
					_interactor.Rules.RemoveAt( _index );
					--_index;
				}
				
			ICEEditorLayout.EndHorizontal( Info.INTERACTION_INTERACTOR_RULE );
			
			EditorGUI.BeginDisabledGroup ( _rule.Enabled == false);
			EditorGUI.indentLevel++;
			
			_rule.Selectors.CanUseDefaultPriority = true;
			_rule.Selectors.DefaultPriority = _interactor.Selectors.Priority;
			EditorSharedTools.DrawTargetSelectors( _control, _rule.Selectors, TargetType.INTERACTOR, _rule_min_distance, _rule_max_distance );
			
			
			EditorGUILayout.Separator();
			
			// TARGET MOVE POSITION
			_rule.OverrideTargetMovePosition = ICEEditorLayout.ToggleLeft( "Override Target Move Specifications", "Overriding the Target Move Specifications", _rule.OverrideTargetMovePosition, false ); 
			if( _rule.OverrideTargetMovePosition )
			{
				_rule = DrawInteractorRuleOffset( _control, _interactor, _rule );
				EditorGUI.indentLevel++;	
				EditorGUILayout.Separator();

				_rule.BlockRuleUpdateUntilMovePositionReached = ICEEditorLayout.Toggle( "Block Next Rule", "Blocking the next rule until the target move position was reached", _rule.BlockRuleUpdateUntilMovePositionReached, Info.INTERACTION_INTERACTOR_RULE_BLOCK );  
				
				if( _rule.BlockRuleUpdateUntilMovePositionReached )
				{
					string _text =  "This rule will be active until your creature reached the given move-position, so please make sure, that all potential positions " +
						"reachable for your creature, otherwise you will provoke a deadlock!";
					
					if( _rule.Selectors.SelectionRange == 0 )
						_text += "\n\nThe SelectionRange of this rule is adjusted to zero ";
					
					Info.Note ( _text );
				}
				
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Separator();
			
			// BEHAVIOUR
			string _auto_key = _interactor.Name + "_action_" + _index;
			_rule.BehaviourModeKey = EditorBehaviour.BehaviourSelect( _control, "Behaviour", "Action behaviour for this interaction rule", _rule.BehaviourModeKey, _auto_key ); 
			EditorGUILayout.Separator();
			
			EditorGUI.indentLevel--;	
			EditorGUI.EndDisabledGroup ();
		}

	}


}
