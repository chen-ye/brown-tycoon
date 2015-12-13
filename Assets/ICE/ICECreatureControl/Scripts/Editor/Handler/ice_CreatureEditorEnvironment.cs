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
	
	public static class EditorEnvironment
	{	
		public static void Print( ICECreatureControl _control )
		{
			if( ! _control.Display.ShowEnvironmentSettings )
				return;
			
			string _surfaces = _control.Creature.Environment.SurfaceHandler.Surfaces.Count.ToString();
			string _impacts = _control.Creature.Environment.CollisionHandler.Collisions.Count.ToString();
			
			ICEEditorStyle.SplitterByIndent( 0 );
			_control.Display.FoldoutEnvironment = ICEEditorLayout.Foldout( _control.Display.FoldoutEnvironment, "Environment (" + _surfaces + "/" + _impacts + ")" , Info.ENVIROMENT );
			
			if( ! _control.Display.FoldoutEnvironment ) 
				return;

			EditorGUILayout.Separator();
				DrawEnvironmentSurfaceSettings( _control );				
				DrawEnvironmentCollisionSettings( _control );	
			EditorGUILayout.Separator();
				
		}

		
		private static void DrawEnvironmentSurfaceSettings( ICECreatureControl _control )
		{
			ICEEditorLayout.BeginHorizontal();
			_control.Creature.Environment.SurfaceHandler.Enabled = ICEEditorLayout.ToggleLeft( "Surfaces","", _control.Creature.Environment.SurfaceHandler.Enabled, true );
			if (GUILayout.Button(new GUIContent("ADD", "add a new surface rule"), ICEEditorStyle.ButtonMiddle))
				_control.Creature.Environment.SurfaceHandler.Surfaces.Add( new SurfaceDataObject() );	
			if (GUILayout.Button(new GUIContent("RESET", "removes all surface rules"), ICEEditorStyle.ButtonMiddle))
				_control.Creature.Environment.SurfaceHandler.Surfaces.Clear();	
			ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_SURFACE );

			if( _control.Creature.Environment.SurfaceHandler.Surfaces.Count == 0 )
				_control.Creature.Environment.SurfaceHandler.Enabled = false;

			EditorGUI.BeginDisabledGroup( _control.Creature.Environment.SurfaceHandler.Enabled == false );

			EditorGUI.indentLevel++;
			
				for (int i = 0; i < _control.Creature.Environment.SurfaceHandler.Surfaces.Count; ++i)
				{
					// HEADER BEGIN
					SurfaceDataObject _surface = _control.Creature.Environment.SurfaceHandler.Surfaces[i];
					
					if(_surface.Name == "")
						_surface.Name = "Surface Rule #"+(i+1);
					
					ICEEditorLayout.BeginHorizontal();
						_surface.Foldout = ICEEditorLayout.Foldout( _surface.Foldout, _surface.Name );	
						_surface.Enabled = ICEEditorLayout.ButtonCheck( "ACTIVE", "activates/deactivates the rule", _surface.Enabled , ICEEditorStyle.ButtonMiddle );
				
						if (GUILayout.Button( new GUIContent( "REMOVE", "removes the selected surface rule"), ICEEditorStyle.ButtonMiddle ))
						{
							_control.Creature.Environment.SurfaceHandler.Surfaces.RemoveAt(i);
							--i;
						}
					ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_SURFACE_RULE );
					// HEADER END

					// CONTENT
					
					if( _surface.Foldout ) 
					{
						EditorGUI.BeginDisabledGroup( _surface.Enabled == false );							
							ICEEditorLayout.BeginHorizontal();
								_surface.Name = ICEEditorLayout.Text("Name", "", _surface.Name );	
								if( GUILayout.Button( new GUIContent( "CLR", ""), ICEEditorStyle.CMDButtonDouble ))
									_surface.Name = "";
							ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_SURFACE_RULE_NAME );											
							_surface.Interval = ICEEditorLayout.DefaultSlider( "Interval", "", _surface.Interval, 0.25f, 0, 30, 1, Info.ENVIROMENT_SURFACE_RULE_INTERVAL );
							
							//ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );
							
							DrawEnvironmentTextures( _surface );
							
							ICEEditorLayout.Label( "Procedures" , true, Info.ENVIROMENT_SURFACE_RULE_PROCEDURES );
							EditorGUI.indentLevel++;
							EditorGUI.BeginDisabledGroup( _surface.Textures.Count == 0 );
								_surface.Audio = EditorSharedTools.DrawSharedAudio( _surface.Audio, Info.ENVIROMENT_SURFACE_AUDIO );
								_surface.Effect = EditorSharedTools.DrawSharedEffect( _surface.Effect, Info.ENVIROMENT_SURFACE_EFFECT );
								_surface.Influences = EditorSharedTools.DrawSharedInfluences( _surface.Influences, Info.ENVIROMENT_SURFACE_INFLUENCES );
							EditorGUI.EndDisabledGroup();
							EditorGUI.indentLevel--;
							EditorGUILayout.Separator();							
						EditorGUI.EndDisabledGroup();
						ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );
					}
				}
				
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();

		}
		
		private static void DrawEnvironmentCollisionSettings( ICECreatureControl _control )
		{

			// IMPACT HEADER BEGIN
			ICEEditorLayout.BeginHorizontal();
			_control.Creature.Environment.CollisionHandler.Enabled = ICEEditorLayout.ToggleLeft( "Collisions", "", _control.Creature.Environment.CollisionHandler.Enabled, true );
			if (GUILayout.Button( new GUIContent("ADD", "add a new impact rule"), ICEEditorStyle.ButtonMiddle ))
			{
				_control.Creature.Environment.CollisionHandler.Collisions.Add( new CollisionDataObject() ); 
				_control.Creature.Environment.CollisionHandler.Enabled = true;
			}
			if (GUILayout.Button(new GUIContent("RESET", "removes all impact rules"), ICEEditorStyle.ButtonMiddle))
				_control.Creature.Environment.CollisionHandler.Collisions.Clear();	
			ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_COLLISION );
			// IMPACT HEADER END

			if( _control.Creature.Environment.CollisionHandler.Collisions.Count == 0 )
				_control.Creature.Environment.CollisionHandler.Enabled = false;

			// IMPACT CONTENT BEGIN
			EditorGUI.BeginDisabledGroup( _control.Creature.Environment.CollisionHandler.Enabled == false );
			EditorGUI.indentLevel++;
				for( int i = 0; i < _control.Creature.Environment.CollisionHandler.Collisions.Count ; i++ )
				{
					CollisionDataObject _collision = _control.Creature.Environment.CollisionHandler.Collisions[i];
					
					if( _collision != null )
					{
						string _title = _collision.Name;
						if( _title == "" )
						{
							_title = "Collision Rule #"+(i+1);

							if( _collision.TagPriority > 0 )
							{
								if( _title != "" )
									_title += " ";

								_title += "T:" + _collision.Tag;
							}
							
							if( _collision.LayerPriority > 0 )
							{
								if( _title != "" )
									_title += " ";
								
								_title += "L:" + LayerMask.LayerToName( _collision.Layer );
							}

							_collision.Name = _title;
						}
						
						// IMPACT RULE HEADER BEGIN
						ICEEditorLayout.BeginHorizontal();						
							_collision.Foldout = ICEEditorLayout.Foldout( _collision.Foldout, _collision.Name );
							_collision.Enabled = ICEEditorLayout.ButtonCheck( "ACTIVE", "activates/deactivates the selected collision rule", _collision.Enabled , ICEEditorStyle.ButtonMiddle );

							if( GUILayout.Button( new GUIContent( "REMOVE", "removes the selected collision rule"), ICEEditorStyle.ButtonMiddle ))
							{
								_control.Creature.Environment.CollisionHandler.Collisions.Remove( _collision );
								return;
							}
						ICEEditorLayout.EndHorizontal(  Info.ENVIROMENT_COLLISION_RULE  );
						// IMPACT RULE HEADER END

						// IMPACT RULE CONTENT BEGIN
						if( _collision.Foldout ) 
						{
							EditorGUI.BeginDisabledGroup( _collision.Enabled == false );		
								ICEEditorLayout.BeginHorizontal();
									_collision.Name = ICEEditorLayout.Text("Name", "", _collision.Name );	
									if( GUILayout.Button( new GUIContent( "CLR", ""), ICEEditorStyle.CMDButtonDouble ))
										_collision.Name = "";
								ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_COLLISION_RULE_NAME );

								EditorGUILayout.Separator();
								ICEEditorLayout.Label( "Conditions" , true, Info.ENVIROMENT_COLLISION_RULE_CONDITIONS );
								EditorGUI.indentLevel++;

									_collision.Type = (CollisionConditionType)ICEEditorLayout.EnumPopup( "Type","Collision ", _collision.Type, Info.ENVIROMENT_COLLISION_RULE_TYPE );
									
									if( _collision.Type == CollisionConditionType.LAYER )
									   	_collision.TagPriority = 0;
									else if( _collision.Type == CollisionConditionType.LAYER )
									 	_collision.LayerPriority = 0;	

									if( _collision.Type == CollisionConditionType.LAYER || _collision.Type == CollisionConditionType.TAG_AND_LAYER )
									{
										_collision.Layer = ICEEditorLayout.Layer( "Layer","Desired collision layer", _collision.Layer, Info.ENVIROMENT_COLLISION_RULE_LAYER );
										EditorGUI.indentLevel++;
											_collision.LayerPriority = (int)ICEEditorLayout.DefaultSlider( "Priority", "", _collision.LayerPriority, 1, 0 , 10, 1, Info.ENVIROMENT_COLLISION_RULE_LAYER_PRIORITY);
										EditorGUI.indentLevel--;
									}
									
									if( _collision.Type == CollisionConditionType.TAG || _collision.Type == CollisionConditionType.TAG_AND_LAYER  )
									{	
										_collision.Tag = ICEEditorLayout.Tag( "Tag","Desired collision tag", _collision.Tag, Info.ENVIROMENT_COLLISION_RULE_TAG );
										EditorGUI.indentLevel++;
											_collision.TagPriority = (int)ICEEditorLayout.DefaultSlider( "Priority", "", _collision.TagPriority, 1, 0 , 10, 1, Info.ENVIROMENT_COLLISION_RULE_TAG_PRIORITY);
										EditorGUI.indentLevel--;
									}

								EditorGUI.indentLevel--;	
									
								EditorGUILayout.Separator();
								ICEEditorLayout.Label( "Procedures" , true, Info.ENVIROMENT_COLLISION_RULE_PROCEDURES );
								EditorGUI.indentLevel++;
									//_impact.ForceInteraction = EditorGUILayout.Toggle("Force Interaction", _impact.ForceInteraction );
									_collision.Influences.Enabled = EditorGUILayout.Toggle("Influences", _collision.Influences.Enabled );
									_collision.Influences = EditorSharedTools.DrawShareInfluencesContent( _collision.Influences, Info.ENVIROMENT_COLLISION_INFLUENCES );
									_collision.BehaviourModeKey = EditorBehaviour.BehaviourSelect( _control, "Behaviour","Reaction to this impact", _collision.BehaviourModeKey, "IMPACT_" + _collision.Tag.ToUpper() );
								EditorGUI.indentLevel--;
								EditorGUILayout.Separator();
							
														
							EditorGUI.EndDisabledGroup();
							ICEEditorStyle.SplitterByIndent( EditorGUI.indentLevel + 1 );
						}
						// IMPACT RULE CONTENT END
					}
				}
				
				EditorGUILayout.Separator();
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();
		}
		
		
		private static void DrawEnvironmentTextures( SurfaceDataObject _environment )
		{
			if( _environment == null )
				return;

			ICEEditorLayout.BeginHorizontal();
				ICEEditorLayout.Label( "Trigger Textures", true );
				if (GUILayout.Button( new GUIContent( "ADD", "Add a texture"), ICEEditorStyle.ButtonMiddle ))
					_environment.Textures.Add( new Texture() );		
			ICEEditorLayout.EndHorizontal( Info.ENVIROMENT_SURFACE_RULE_TEXTURES );

			if( _environment.Textures != null && _environment.Textures.Count > 0 )
			{
				int _width = 90;
				int _tolerance_space = 50 + (EditorGUI.indentLevel * 15) + _width;
				int _inspector_width = Screen.width - _tolerance_space;
				int _textures_width = 0;		
				int _max_count = 0;
				int _counter = 0;

				if( _inspector_width < 120 )
					_max_count = 3;

				for (int i = 0; i < _environment.Textures.Count; i++)
				{	
					if(_counter == 0)
					{
						ICEEditorLayout.BeginHorizontal();
						GUILayout.Space( EditorGUI.indentLevel * 15 );
					}

					int _indent = EditorGUI.indentLevel;
					
					EditorGUI.indentLevel = 0;
					GUILayout.BeginVertical("box", GUILayout.MinWidth(_width), GUILayout.MaxWidth(_width), GUILayout.MinHeight(90));
						_environment.Textures[i] = (Texture)EditorGUILayout.ObjectField(_environment.Textures[i], typeof(Texture), false, GUILayout.Height(75) );

					if( GUILayout.Button( "DELETE" ) )
					{
						_environment.Textures.RemoveAt(i);
						--i;
					}

					GUILayout.EndVertical();
					EditorGUI.indentLevel = _indent;					

					_counter++;					
					_textures_width = _counter * _width;
					if( _textures_width > _inspector_width || _counter == _max_count || i == _environment.Textures.Count - 1 )
					{
						ICEEditorLayout.EndHorizontal();
						EditorGUILayout.Separator();
						_counter = 0;
					}
				}
			}

			if(_environment.Textures.Count == 0)
				EditorGUILayout.HelpBox("No textures assigned. Press ADD to assign a texture!", MessageType.Info);
			
			EditorGUILayout.Separator();
			
		}
		

		

	}
}
